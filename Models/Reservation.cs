using System.ComponentModel.DataAnnotations;

namespace Lesson5.Models
{
    public class Reservation : IValidatableObject
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        [Required]
        [MinLength(1)]
        public string OrganizerName { get; set; } = String.Empty;
        [Required]
        [MinLength(1)]
        public string Topic { get; set; } = String.Empty;
        public DateOnly Date { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public String Status { get; set; } = String.Empty;
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (EndTime <= StartTime)
            {
                yield return new ValidationResult(
                    "End time must be later than Start time.",
                    new[] { nameof(EndTime) }
                );
            }
        }
    }
}
