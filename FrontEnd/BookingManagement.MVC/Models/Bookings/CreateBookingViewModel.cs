using System.ComponentModel.DataAnnotations;
using BookingManagement.Common.DTOs.V1;
using BookingManagement.MVC.Helpers;

namespace BookingManagement.MVC.Models.Bookings
{
    public class CreateBookingViewModel : IValidatableObject
    {
        [Required(ErrorMessage = "Please select a resource.")]
        [StringLength(100, ErrorMessage = "Resource must not exceed 100 characters.")]
        [Display(Name = "Resource")]
        public string? ResourceId { get; set; }

        [Required(ErrorMessage = "User ID is required.")]
        [StringLength(100, ErrorMessage = "User ID must not exceed 100 characters.")]
        [Display(Name = "User ID")]
        public string? UserId { get; set; }

        [Required(ErrorMessage = "Start is required.")]
        [Display(Name = "Start (UTC)")]
        public DateTime? StartDateTime { get; set; }

        [Required(ErrorMessage = "End is required.")]
        [Display(Name = "End (UTC)")]
        public DateTime? EndDateTime { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (StartDateTime.HasValue && EndDateTime.HasValue && StartDateTime >= EndDateTime)
            {
                yield return new ValidationResult("End must be after Start.", [nameof(EndDateTime)]);
            }
        }

        public CreateBookingRequest ToDto()
        {
            return new CreateBookingRequest
            {
                ResourceId = ResourceId!.Trim(),
                UserId = UserId!.Trim(),
                StartDateTime = StartDateTime!.Value.AsUtc(),
                EndDateTime = EndDateTime!.Value.AsUtc()
            };
        }
    }
}
