# Booking Management Service

A small booking service built with **.NET 10**, **Entity Framework Core** and **PostgreSQL**, plus an
**ASP.NET Core MVC** web app that uses it.

## What it does

Teams share resources — meeting rooms, projectors, equipment. This service keeps track of who booked
what and when, and makes sure the same resource cannot be booked twice for the same time.

You can:

* create a booking (resource, user, start and end, all in UTC),
* pick the resource from a fixed list rather than typing its name,
* list the bookings of a resource, with a date filter and paging,
* cancel a booking,
* read an audit trail of every booking created or cancelled (the *Auditability* extension).

There are two applications. The **API** owns all the rules and is the only one that touches the
database. The **MVC app** is just the user interface: it has no rules and no database access, and calls
the API over HTTP. The API base URL comes from `appsettings.json` (`ApiSettings:BaseUrl`), so it is not
hardcoded.

**Resources come from a fixed list.** They live in the API's `appsettings.json` and are served by
`GET /api/v1/resources`.

**Creating a booking, end to end:** the user fills the form → the MVC app sends
`POST /api/v1/bookings` → the API validates the input → the API asks the database whether an active
booking already overlaps that time → if it does, `409 Conflict` and the page shows "This resource is
already booked…" → if it does not, the booking row **and** a `BookingCreated` audit row are saved
together. Cancelling is the same, except nothing is deleted: the status becomes `Cancelled`, a
`BookingCancelled` audit row is added, and the slot becomes free again.

---

## Architecture

One solution, split into two folders plus one shared library:

```
BookingManagement.Common      DTOs, PagedResult<T>, ApiResult<T>, enums, exception types.
                              Referenced by every project; references nothing itself.

BackEnd/
  BookingManagement.API       Controllers, error-handling middleware, DI setup. No rules here.
        ↓
  BookingManagement.BL        The rules: validation, the overlap check, cancelling, audit records,
                              and the catalog of bookable resources.
        ↓
  BookingManagement.DAL       EF Core: BookingDbContext, repositories, unit of work, migrations.
        ↓
  BookingManagement.Domain    The Booking and AuditLog entities and the overlap rule.

  BookingManagement.Tests     50 xUnit tests for the overlap rule and the business services.

FrontEnd/
  BookingManagement.MVC           Controllers, view models, Razor views, wwwroot.
        ↓
  BookingManagement.MVC.Services  BookingAppService / AuditLogAppService — the only code that
                                  speaks HTTP, plus their interfaces.
```

### API endpoints

| Method | Route | Purpose |
|---|---|---|
| `POST` | `/api/v1/bookings` | Create a booking. `201`, or `409` if it overlaps |
| `GET` | `/api/v1/bookings/{id}` | One booking |
| `POST` | `/api/v1/bookings/{id}/cancel` | Cancel a booking |
| `GET` | `/api/v1/resources/{resourceId}/bookings` | A resource's bookings — `from`, `to`, `includeCancelled`, `page`, `pageSize` |
| `GET` | `/api/v1/resources` | The bookable resources, for the dropdowns |
| `GET` | `/api/v1/audit-logs` | The audit trail — `bookingId`, `page`, `pageSize` |

All times are UTC ISO 8601 with a `Z` suffix. Swagger is at `http://localhost:5159/swagger`.

---

## Running it

1. **Clone the repository** from GitHub.
2. Install the **.NET SDK 10** (check with `dotnet --version`).
3. Install **PostgreSQL** and make sure the server is running. The connection string is in
   `BackEnd/BookingManagement.API/appsettings.json` — change the user and password to match your server.
4. Open `BookingManagement.slnx` in Visual Studio.
5. **Update the database before running anything.** The app does not migrate on startup, so nothing
   works until this has run once. In `Tools ▸ NuGet Package Manager ▸ Package Manager Console`:

   ```powershell
   Update-Database -Project BookingManagement.DAL -StartupProject BookingManagement.API
   ```

   This creates the `BookingManagement` database and both tables. (Command line equivalent, from the
   solution folder: `dotnet tool restore` then
   `dotnet ef database update --project BackEnd/BookingManagement.DAL --startup-project BackEnd/BookingManagement.API`.)

6. **Set two startup projects:** right-click the solution → **Configure Startup Projects** →
   **Multiple startup projects**, and set both to **Start**:

   * `BookingManagement.API`
   * `BookingManagement.MVC`

   Put the API first so it is listening before the MVC app calls it.

7. Press **F5**. The API opens Swagger on `http://localhost:5159`, and the MVC app opens on
   `http://localhost:5212`.

Tests need no database: `dotnet test BookingManagement.slnx` — 50 tests.

`sample-data.sql` adds 25 example bookings if you want the pages to have something in them.

---

## 03 DESIGN WRITE-UP

### A. How did you define and enforce overlapping bookings, and why?

I treated a booking as covering its start time but not its end time, so `[Start, End)`.

That means 10:00 to 11:00 and 11:00 to 12:00 don't clash, but 10:00 to 11:00 and 10:30 to 11:30 do.

So two bookings clash if they are on the same resource, the existing one is still `Active`, and
`existing.Start < new.End && existing.End > new.Start`.

The check happens in `BookingService.CreateAsync` before anything is saved, and it runs as SQL, not in
memory. If it finds a row, the service throws `ConflictException` and the API returns `409`.

I did it this way because it is how people actually book rooms. If your meeting ends at 11:00 it should
not block mine starting at 11:00. I also kept the condition in one place,
`BookingSpecifications.ActiveOverlapping`, so EF Core turns it into SQL and the unit tests use the same
code. If I had written it twice the two copies could drift apart.

The comparison on the resource is exact, on purpose. A "contains" match would make "Room 1" clash with
"Room 10", which is why the UI picks the resource from a dropdown instead of searching for it by name.

### B. What did you assume about concurrency?

There is no login in this project, the user is just an id typed into the form, so I assumed one person
is using it at a time. Two people booking the same slot in the same second is not really something that
happens here yet.

That said, the code would not survive it. Creating a booking is a `SELECT` and then an `INSERT`, and
nothing locks in between, so both requests could see a free slot and both could save.

I did not fix it now because there is nothing yet that can trigger it, and fixing it properly means
the database has to enforce the rule, not C#.

When we add login and user sessions in the future, that is when it becomes a real problem, because
then there are real people on real accounts clicking book at the same moment. At that point I would
fix it like this:

1. Add a PostgreSQL exclusion constraint so the database itself refuses a second overlapping booking.
2. Catch that error in the DAL and throw the same `ConflictException` we already throw.
3. Leave the C# check where it is, so most conflicts are still caught early with a nicer message.

```sql
CREATE EXTENSION IF NOT EXISTS btree_gist;

ALTER TABLE "Bookings" ADD CONSTRAINT no_overlapping_active_booking
EXCLUDE USING gist ("ResourceId" WITH =, tstzrange("StartDateTime", "EndDateTime") WITH &&)
WHERE ("Status" = 1);
```

Nothing changes for the user, they still get `409` and the same "already booked" message. The
constraint is just the part that cannot be raced.

### C. What would break at scale, and where would the first bottleneck be?

The database first, on creating bookings. Every create does a `SELECT` and then a transaction that
writes two rows, and it holds a connection the whole time. The connection pool would run out before
anything else does.

Things that would show up after that:

* There is only one API instance, so it caps throughput and it is a single point of failure.
* Paging uses `Skip`/`Take`, which is `OFFSET` in SQL, and that gets slow on later pages.
* Every list page runs two queries, a `COUNT` and then the rows.
* `AuditLogs` only grows. Nothing ever deletes from it.

The overlap check itself should be fine, there is an index on `(ResourceId, Status, StartDateTime)`.

### D. How would you evolve this into a distributed system?

Run more than one API instance behind a load balancer. Nothing is kept in memory, so that part already
works.

Then move the overlap rule into the database, like in B. Once there is more than one instance the C#
check definitely is not enough on its own.

After that, read replicas so listing and reports do not slow down booking, and Redis for the pages
people open a lot, cleared whenever a booking is created or cancelled.

I would use a queue for things that can wait, like sending emails. Not for the audit row, that has to
be saved in the same transaction as the booking, otherwise you can end up with a booking that has no
audit record.

Before any of it I would add health checks and proper logging. With one instance you can just look at
it, with five you cannot.

### E. Which tradeoff did you prioritize, simplicity, correctness, or performance?

I did not really put one above the others, they mattered in different places.

The overlap rule had to be correct, that is the whole point of the service, so it has one definition,
it runs as SQL, and the tests cover the edge cases like times that touch. Cancelling only changes the
status, it does not delete anything, so the audit trail stays honest.

The rest I kept simple on purpose: offset paging, exceptions and one middleware instead of returning a
result object from every method, no cache, no background jobs. It is an internal tool with a small
amount of data, so most of that would just be extra code to look after.

Resources are the same kind of call. They are a list in configuration, not a table, because today they
are only a name and an id. The day they need an owner, a capacity or opening hours, `ResourceCatalog`
becomes a table and a repository and nothing above it changes.

Performance I only thought about where it was cheap, which is the index for the overlap check and
paging so lists are not loaded whole.

The part I know is not finished is the concurrency issue in B. I left it written down rather than
pretend it is not there.
