-- Sample data for the Bookings table (25 rows).
-- Status: 1 = Active, 2 = Cancelled. The +00 offset on every timestamp is what makes it UTC.
-- Active bookings never overlap within the same resource, so the data matches the business rules.

INSERT INTO "Bookings" ("Id", "ResourceId", "UserId", "StartDateTime", "EndDateTime", "Status", "CreatedAt", "CancelledAt")
VALUES
  -- meeting-room-1, 2026-09-01: two back-to-back bookings, then gaps
  ('0a5b1e00-0000-4000-8000-000000000001', 'meeting-room-1', 'user-101', '2026-09-01 09:00:00+00', '2026-09-01 10:00:00+00', 1, '2026-08-31 08:00:00+00', NULL),
  ('0a5b1e00-0000-4000-8000-000000000002', 'meeting-room-1', 'user-102', '2026-09-01 10:00:00+00', '2026-09-01 11:00:00+00', 1, '2026-08-31 08:00:00+00', NULL),
  ('0a5b1e00-0000-4000-8000-000000000003', 'meeting-room-1', 'user-103', '2026-09-01 11:30:00+00', '2026-09-01 12:30:00+00', 1, '2026-08-31 08:00:00+00', NULL),
  ('0a5b1e00-0000-4000-8000-000000000004', 'meeting-room-1', 'user-104', '2026-09-01 14:00:00+00', '2026-09-01 15:30:00+00', 1, '2026-08-31 08:00:00+00', NULL),
  -- cancelled, and deliberately overlapping the active ones: it must not block anything
  ('0a5b1e00-0000-4000-8000-000000000005', 'meeting-room-1', 'user-105', '2026-09-01 10:15:00+00', '2026-09-01 11:15:00+00', 2, '2026-08-31 08:00:00+00', '2026-08-31 09:30:00+00'),
  ('0a5b1e00-0000-4000-8000-000000000006', 'meeting-room-1', 'user-106', '2026-09-01 16:00:00+00', '2026-09-01 17:00:00+00', 2, '2026-08-31 08:00:00+00', '2026-08-31 10:15:00+00'),

  -- meeting-room-1, 2026-09-02: a second day for the same resource, for date-range filtering
  ('0a5b1e00-0000-4000-8000-000000000007', 'meeting-room-1', 'user-107', '2026-09-02 09:00:00+00', '2026-09-02 10:30:00+00', 1, '2026-08-31 08:00:00+00', NULL),
  ('0a5b1e00-0000-4000-8000-000000000008', 'meeting-room-1', 'user-108', '2026-09-02 11:00:00+00', '2026-09-02 12:00:00+00', 1, '2026-08-31 08:00:00+00', NULL),
  ('0a5b1e00-0000-4000-8000-000000000009', 'meeting-room-1', 'user-109', '2026-09-02 15:00:00+00', '2026-09-02 16:00:00+00', 2, '2026-08-31 08:00:00+00', '2026-08-31 11:00:00+00'),

  -- meeting-room-2, 2026-09-01
  ('0a5b1e00-0000-4000-8000-000000000010', 'meeting-room-2', 'user-201', '2026-09-01 08:30:00+00', '2026-09-01 09:15:00+00', 1, '2026-08-31 08:00:00+00', NULL),
  ('0a5b1e00-0000-4000-8000-000000000011', 'meeting-room-2', 'user-202', '2026-09-01 09:30:00+00', '2026-09-01 10:30:00+00', 1, '2026-08-31 08:00:00+00', NULL),
  ('0a5b1e00-0000-4000-8000-000000000012', 'meeting-room-2', 'user-203', '2026-09-01 13:00:00+00', '2026-09-01 14:30:00+00', 1, '2026-08-31 08:00:00+00', NULL),
  ('0a5b1e00-0000-4000-8000-000000000013', 'meeting-room-2', 'user-204', '2026-09-01 15:30:00+00', '2026-09-01 16:00:00+00', 1, '2026-08-31 08:00:00+00', NULL),
  ('0a5b1e00-0000-4000-8000-000000000014', 'meeting-room-2', 'user-205', '2026-09-01 11:00:00+00', '2026-09-01 12:00:00+00', 2, '2026-08-31 08:00:00+00', '2026-08-31 12:20:00+00'),

  -- meeting-room-2, 2026-09-02
  ('0a5b1e00-0000-4000-8000-000000000015', 'meeting-room-2', 'user-206', '2026-09-02 10:00:00+00', '2026-09-02 11:00:00+00', 1, '2026-08-31 08:00:00+00', NULL),

  -- meeting-room-3, 2026-09-01: booked solid from 09:00 to 17:00
  ('0a5b1e00-0000-4000-8000-000000000016', 'meeting-room-3', 'user-301', '2026-09-01 09:00:00+00', '2026-09-01 13:00:00+00', 1, '2026-08-31 08:00:00+00', NULL),
  ('0a5b1e00-0000-4000-8000-000000000017', 'meeting-room-3', 'user-302', '2026-09-01 13:00:00+00', '2026-09-01 17:00:00+00', 1, '2026-08-31 08:00:00+00', NULL),

  -- projector-1, 2026-09-02: short bookings with 30 minute gaps
  ('0a5b1e00-0000-4000-8000-000000000018', 'projector-1', 'user-401', '2026-09-02 09:00:00+00', '2026-09-02 09:30:00+00', 1, '2026-08-31 08:00:00+00', NULL),
  ('0a5b1e00-0000-4000-8000-000000000019', 'projector-1', 'user-402', '2026-09-02 10:00:00+00', '2026-09-02 10:30:00+00', 1, '2026-08-31 08:00:00+00', NULL),
  ('0a5b1e00-0000-4000-8000-000000000020', 'projector-1', 'user-403', '2026-09-02 11:00:00+00', '2026-09-02 11:30:00+00', 1, '2026-08-31 08:00:00+00', NULL),
  ('0a5b1e00-0000-4000-8000-000000000021', 'projector-1', 'user-404', '2026-09-02 12:00:00+00', '2026-09-02 12:30:00+00', 2, '2026-08-31 08:00:00+00', '2026-08-31 13:45:00+00'),
  ('0a5b1e00-0000-4000-8000-000000000022', 'projector-1', 'user-405', '2026-09-02 13:00:00+00', '2026-09-02 13:30:00+00', 1, '2026-08-31 08:00:00+00', NULL),

  -- company-car-1, 2026-09-02: three bookings that touch exactly, with no gap
  ('0a5b1e00-0000-4000-8000-000000000023', 'company-car-1', 'user-501', '2026-09-02 08:00:00+00', '2026-09-02 12:00:00+00', 1, '2026-08-31 08:00:00+00', NULL),
  ('0a5b1e00-0000-4000-8000-000000000024', 'company-car-1', 'user-502', '2026-09-02 12:00:00+00', '2026-09-02 13:00:00+00', 1, '2026-08-31 08:00:00+00', NULL),
  ('0a5b1e00-0000-4000-8000-000000000025', 'company-car-1', 'user-503', '2026-09-02 13:00:00+00', '2026-09-02 18:00:00+00', 1, '2026-08-31 08:00:00+00', NULL);


-- Check what was inserted
-- SELECT "ResourceId", COUNT(*) AS total, COUNT(*) FILTER (WHERE "Status" = 1) AS active
-- FROM "Bookings" GROUP BY "ResourceId" ORDER BY "ResourceId";


-- Matching audit trail: one BookingCreated per booking, plus a BookingCancelled for the cancelled
-- ones. EventType: 1 = BookingCreated, 2 = BookingCancelled.
INSERT INTO "AuditLogs" ("Id", "BookingId", "EventType", "OccurredAt", "ResourceId", "UserId")
SELECT
    md5('created' || "Id"::text)::uuid,
    "Id",
    1,
    "CreatedAt",
    "ResourceId",
    "UserId"
FROM "Bookings"
WHERE "Id"::text LIKE '0a5b1e00-0000-4000-8000-%';

INSERT INTO "AuditLogs" ("Id", "BookingId", "EventType", "OccurredAt", "ResourceId", "UserId")
SELECT
    md5('cancelled' || "Id"::text)::uuid,
    "Id",
    2,
    "CancelledAt",
    "ResourceId",
    "UserId"
FROM "Bookings"
WHERE "Id"::text LIKE '0a5b1e00-0000-4000-8000-%'
  AND "CancelledAt" IS NOT NULL;


-- Remove this sample data again
-- DELETE FROM "AuditLogs" WHERE "BookingId"::text LIKE '0a5b1e00-0000-4000-8000-%';
-- DELETE FROM "Bookings" WHERE "Id"::text LIKE '0a5b1e00-0000-4000-8000-%';
