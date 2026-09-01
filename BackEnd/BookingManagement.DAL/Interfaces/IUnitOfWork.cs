namespace BookingManagement.DAL.Interfaces
{
    // A booking change and its audit record are written through the same DbContext and
    // committed by one SaveChanges, so they always succeed or fail together.
    public interface IUnitOfWork
    {
        Task SaveChangesAsync();
    }
}
