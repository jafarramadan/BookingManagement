using BookingManagement.Common.DTOs.V1;
using BookingManagement.Common.Models;

namespace BookingManagement.MVC.Services.Resources
{
    public interface IResourceAppService
    {
        Task<ApiResult<List<ResourceDto>>> GetAllAsync();
    }
}
