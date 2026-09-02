using BookingManagement.Common.DTOs.V1;

namespace BookingManagement.BL.Interfaces
{
    public interface IResourceCatalog
    {
        IReadOnlyList<ResourceDto> GetAll();
    }
}
