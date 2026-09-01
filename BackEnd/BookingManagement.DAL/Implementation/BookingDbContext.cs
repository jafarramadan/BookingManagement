using BookingManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookingManagement.DAL.Implementation
{
    public class BookingDbContext : DbContext
    {
        #region DbSets

        public virtual DbSet<Booking> Bookings { get; set; } = null!;
        public virtual DbSet<AuditLog> AuditLogs { get; set; } = null!;

        #endregion

        public BookingDbContext(DbContextOptions<BookingDbContext> options) : base(options)
        {
        }

        #region Methods

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var booking = modelBuilder.Entity<Booking>();

            booking.ToTable("Bookings");
            booking.HasKey(x => x.Id);
            booking.Ignore(x => x.IsActive);
            booking.Property(x => x.ResourceId).HasMaxLength(100).IsRequired();
            booking.Property(x => x.UserId).HasMaxLength(100).IsRequired();
            booking.Property(x => x.StartDateTime).HasColumnType("timestamp with time zone");
            booking.Property(x => x.EndDateTime).HasColumnType("timestamp with time zone");
            booking.Property(x => x.CreatedAt).HasColumnType("timestamp with time zone");
            booking.Property(x => x.CancelledAt).HasColumnType("timestamp with time zone");

            // Covers the overlap check and the resource/date-range retrieval query.
            booking.HasIndex(x => new { x.ResourceId, x.Status, x.StartDateTime });
            booking.HasIndex(x => x.UserId);

            var auditLog = modelBuilder.Entity<AuditLog>();

            auditLog.ToTable("AuditLogs");
            auditLog.HasKey(x => x.Id);
            auditLog.Property(x => x.ResourceId).HasMaxLength(100).IsRequired();
            auditLog.Property(x => x.UserId).HasMaxLength(100).IsRequired();
            auditLog.Property(x => x.OccurredAt).HasColumnType("timestamp with time zone");

            auditLog.HasIndex(x => x.OccurredAt);
            auditLog.HasIndex(x => x.BookingId);
        }

        #endregion
    }
}
