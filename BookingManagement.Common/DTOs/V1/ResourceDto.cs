namespace BookingManagement.Common.DTOs.V1
{
    // A bookable resource. Id is what a booking stores, Name is what the dropdown shows.
    public class ResourceDto
    {
        public string Id { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;
    }
}
