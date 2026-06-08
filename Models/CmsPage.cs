using System.ComponentModel.DataAnnotations;

namespace LoanApplicationSystem.Models
{
    public class CmsPage
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Klucz strony")]
        public string Key { get; set; } = string.Empty;

        [Required(ErrorMessage = "Podaj tytuł.")]
        [Display(Name = "Tytuł")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Podaj treść.")]
        [Display(Name = "Treść")]
        public string Content { get; set; } = string.Empty;

        [Display(Name = "Data aktualizacji")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}