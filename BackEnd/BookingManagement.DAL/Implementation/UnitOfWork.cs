using BookingManagement.DAL.Interfaces;

namespace BookingManagement.DAL.Implementation
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly BookingDbContext _dbContext;

        public UnitOfWork(BookingDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
