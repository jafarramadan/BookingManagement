using BookingManagement.DAL.Interfaces;

namespace BookingManagement.Tests.TestSupport
{
    internal class FakeUnitOfWork : IUnitOfWork
    {
        public int SaveChangesCount { get; private set; }

        public Task SaveChangesAsync()
        {
            SaveChangesCount++;

            return Task.CompletedTask;
        }
    }
}
