using BookingManagement.BL.Interfaces;
using BookingManagement.Common.DTOs.V1;

namespace BookingManagement.BL.Implementation
{
    // The list of bookable resources. It is configuration rather than user data, so it is read once
    // at startup and held in memory. When resources need their own fields (owner, capacity, opening
    // hours) this is the class that becomes a table and a repository.
    public class ResourceCatalog : IResourceCatalog
    {
        private readonly IReadOnlyList<ResourceDto> _resources;

        public ResourceCatalog(IEnumerable<ResourceDto> resources)
        {
            _resources = resources
                .Where(resource => !string.IsNullOrWhiteSpace(resource.Id))
                .Select(resource => new ResourceDto
                {
                    Id = resource.Id.Trim(),
                    Name = string.IsNullOrWhiteSpace(resource.Name) ? resource.Id.Trim() : resource.Name.Trim()
                })
                .ToList();
        }

        public IReadOnlyList<ResourceDto> GetAll() => _resources;
    }
}
