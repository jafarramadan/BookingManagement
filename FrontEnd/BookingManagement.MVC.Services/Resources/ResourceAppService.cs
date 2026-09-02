using BookingManagement.Common.DTOs.V1;
using BookingManagement.Common.Models;
using BookingManagement.MVC.Services.Common;
using Microsoft.Extensions.Logging;

namespace BookingManagement.MVC.Services.Resources
{
    public class ResourceAppService : ApiServiceBase, IResourceAppService
    {
        public ResourceAppService(HttpClient httpClient, ILogger<ResourceAppService> logger)
            : base(httpClient, logger)
        {
        }

        public Task<ApiResult<List<ResourceDto>>> GetAllAsync()
        {
            return SendAsync<List<ResourceDto>>(() => HttpClient.GetAsync("api/v1/resources"));
        }
    }
}
