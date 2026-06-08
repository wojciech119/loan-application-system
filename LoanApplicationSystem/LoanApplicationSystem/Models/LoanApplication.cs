using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace LoanApplicationSystem.Models
{
    public class LoanApplication : IValidatableObject
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        public IdentityUser? User { get; set; }

        [Required(ErrorMessage = "Podaj imię i nazwisko klienta.")]
        [MinLength(5, ErrorMessage = "Imię i nazwisko musi mieć co najmniej 5 znaków.")]
        [MaxLength(100, ErrorMessage = "Imię i nazwisko może mieć maksymalnie 100 znaków.")]
        [Display(Name = "Imię i nazwisko klienta")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Podaj adres e-mail klienta.")]
        [EmailAddress(ErrorMessage = "Podaj poprawny adres e-mail, np. jan.kowalski@example.com.")]
        [Display(Name = "Adres e-mail klienta")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Podaj numer telefonu klienta.")]
        [RegularExpression(@"^(\+48\s?)?(\d{3}\s?\d{3}\s?\d{3})$",
            ErrorMessage = "Podaj poprawny numer telefonu, np. 500 600 700 albo +48 500 600 700.")]
        [Display(Name = "Numer telefonu klienta")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Podaj miesięczny dochód netto klienta.")]
        [Range(1, 1000000, ErrorMessage = "Dochód musi być większy od 0.")]
        [Display(Name = "Miesięczny dochód netto")]
        public decimal MonthlyIncome { get; set; }

        [Required(ErrorMessage = "Podaj miesięczne zobowiązania klienta.")]
        [Range(0, 1000000, ErrorMessage = "Zobowiązania nie mogą być ujemne.")]
        [Display(Name = "Miesięczne zobowiązania")]
        public decimal MonthlyExpenses { get; set; }

        [Required(ErrorMessage = "Podaj kwotę kredytu.")]
        [Range(1000, 10000000, ErrorMessage = "Kwota kredytu musi być z zakresu od 1 000 zł do 10 000 000 zł.")]
        [Display(Name = "Wnioskowana kwota kredytu")]
        public decimal RequestedAmount { get; set; }

        [Required(ErrorMessage = "Podaj okres kredytowania.")]
        [Range(1, 360, ErrorMessage = "Okres kredytowania musi być z zakresu od 1 do 360 miesięcy.")]
        [Display(Name = "Okres kredytowania w miesiącach")]
        public int RepaymentMonths { get; set; }

        [Required(ErrorMessage = "Podaj cel kredytu.")]
        [MinLength(5, ErrorMessage = "Cel kredytu musi mieć co najmniej 5 znaków.")]
        [MaxLength(300, ErrorMessage = "Cel kredytu może mieć maksymalnie 300 znaków.")]
        [Display(Name = "Cel kredytu")]
        public string Purpose { get; set; } = string.Empty;

        [Required(ErrorMessage = "Wybierz rodzaj zatrudnienia.")]
        [Display(Name = "Rodzaj zatrudnienia")]
        public EmploymentType? EmploymentType { get; set; }

        [Required(ErrorMessage = "Wybierz historię kredytową klienta.")]
        [Display(Name = "Historia kredytowa")]
        public CreditHistory? CreditHistory { get; set; }

        [Range(0, 20, ErrorMessage = "Liczba osób na utrzymaniu musi być z zakresu od 0 do 20.")]
        [Display(Name = "Liczba osób na utrzymaniu")]
        public int Dependents { get; set; }

        [Display(Name = "Klient posiada nieruchomość")]
        public bool HasProperty { get; set; }

        [Display(Name = "Dochód rozporządzalny")]
        public decimal DisposableIncome { get; set; }

        [Display(Name = "Szacowana miesięczna rata")]
        public decimal EstimatedMonthlyInstallment { get; set; }

        [Display(Name = "Wskaźnik DTI")]
        public decimal DtiPercent { get; set; }

        [Display(Name = "Wynik scoringowy")]
        public int CreditScore { get; set; }

        [Display(Name = "Rekomendacja systemu")]
        public string CreditDecision { get; set; } = string.Empty;

        [Display(Name = "Status wniosku")]
        public string Status { get; set; } = "Złożony";

        [Display(Name = "Data utworzenia")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "Data aktualizacji")]
        public DateTime? UpdatedAt { get; set; }

        public ICollection<DecisionHistory> DecisionHistory { get; set; } = new List<DecisionHistory>();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!string.IsNullOrWhiteSpace(FullName))
            {
                var parts = FullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length < 2)
                {
                    yield return new ValidationResult(
                        "Podaj imię i nazwisko klienta.",
                        new[] { nameof(FullName) });
                }
            }

            if (MonthlyExpenses > MonthlyIncome)
            {
                yield return new ValidationResult(
                    "Miesięczne zobowiązania nie mogą być większe niż miesięczny dochód klienta.",
                    new[] { nameof(MonthlyExpenses) });
            }
            else if (MonthlyIncome > 0 && MonthlyExpenses == MonthlyIncome)
            {
                yield return new ValidationResult(
                    "Klient nie posiada dochodu rozporządzalnego. Wniosek nie powinien zostać przyjęty do scoringu.",
                    new[] { nameof(MonthlyExpenses) });
            }

            if (RequestedAmount > 0 && MonthlyIncome > 0)
            {
                var ratio = RequestedAmount / MonthlyIncome;

                if (ratio > 120)
                {
                    yield return new ValidationResult(
                        "Wnioskowana kwota jest zbyt wysoka względem miesięcznego dochodu klienta.",
                        new[] { nameof(RequestedAmount) });
                }
            }
        }
    }

    public enum EmploymentType
    {
        [Display(Name = "Umowa o pracę")]
        UmowaOPrace = 1,

        [Display(Name = "Umowa zlecenie")]
        UmowaZlecenie = 2,

        [Display(Name = "Działalność gospodarcza")]
        DzialalnoscGospodarcza = 3,

        [Display(Name = "Emerytura / renta")]
        EmeryturaRenta = 4,

        [Display(Name = "Brak stałego zatrudnienia")]
        BrakZatrudnienia = 5
    }

    public enum CreditHistory
    {
        [Display(Name = "Dobra historia kredytowa")]
        Dobra = 1,

        [Display(Name = "Brak historii kredytowej")]
        Brak = 2,

        [Display(Name = "Opóźnienia w spłatach")]
        Opoznienia = 3,

        [Display(Name = "Zła historia kredytowa")]
        Zla = 4
    }
}