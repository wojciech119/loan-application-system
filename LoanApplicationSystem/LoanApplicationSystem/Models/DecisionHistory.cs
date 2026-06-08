using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace LoanApplicationSystem.Models
{
    public class DecisionHistory
    {
        public int Id { get; set; }

        [Required]
        public int LoanApplicationId { get; set; }

        public LoanApplication? LoanApplication { get; set; }

        [Display(Name = "Poprzedni status")]
        public string OldStatus { get; set; } = string.Empty;

        [Display(Name = "Nowy status")]
        public string NewStatus { get; set; } = string.Empty;

        [Display(Name = "Komentarz")]
        public string? Comment { get; set; }

        public string? ChangedByUserId { get; set; }

        public IdentityUser? ChangedByUser { get; set; }

        [Display(Name = "Data zmiany")]
        public DateTime ChangedAt { get; set; } = DateTime.Now;
    }
}