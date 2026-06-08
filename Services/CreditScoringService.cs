using LoanApplicationSystem.Models;

namespace LoanApplicationSystem.Services
{
    public class CreditScoringService
    {
        public void Calculate(LoanApplication application)
        {
            application.DisposableIncome = application.MonthlyIncome - application.MonthlyExpenses;

            application.EstimatedMonthlyInstallment = CalculateEstimatedMonthlyInstallment(
                application.RequestedAmount,
                application.RepaymentMonths);

            application.DtiPercent = CalculateDti(application);

            int score = 35;

            if (application.MonthlyIncome >= 12000)
                score += 18;
            else if (application.MonthlyIncome >= 9000)
                score += 14;
            else if (application.MonthlyIncome >= 6000)
                score += 10;
            else if (application.MonthlyIncome >= 4000)
                score += 5;
            else
                score -= 15;

            if (application.EstimatedMonthlyInstallment > 0)
            {
                if (application.DisposableIncome >= application.EstimatedMonthlyInstallment * 3)
                    score += 20;
                else if (application.DisposableIncome >= application.EstimatedMonthlyInstallment * 2)
                    score += 15;
                else if (application.DisposableIncome >= application.EstimatedMonthlyInstallment * 1.3m)
                    score += 8;
                else if (application.DisposableIncome >= application.EstimatedMonthlyInstallment)
                    score += 3;
                else
                    score -= 25;
            }

            if (application.DtiPercent <= 30)
                score += 18;
            else if (application.DtiPercent <= 40)
                score += 10;
            else if (application.DtiPercent <= 50)
                score += 3;
            else if (application.DtiPercent <= 65)
                score -= 18;
            else
                score -= 35;

            if (application.EmploymentType == EmploymentType.UmowaOPrace)
                score += 12;
            else if (application.EmploymentType == EmploymentType.EmeryturaRenta)
                score += 8;
            else if (application.EmploymentType == EmploymentType.DzialalnoscGospodarcza)
                score += 4;
            else if (application.EmploymentType == EmploymentType.UmowaZlecenie)
                score -= 6;
            else if (application.EmploymentType == EmploymentType.BrakZatrudnienia)
                score -= 25;

            if (application.CreditHistory == CreditHistory.Dobra)
                score += 15;
            else if (application.CreditHistory == CreditHistory.Brak)
                score -= 5;
            else if (application.CreditHistory == CreditHistory.Opoznienia)
                score -= 20;
            else if (application.CreditHistory == CreditHistory.Zla)
                score -= 35;

            if (application.HasProperty)
                score += 5;

            if (application.Dependents == 1)
                score -= 2;
            else if (application.Dependents == 2)
                score -= 5;
            else if (application.Dependents >= 3)
                score -= 10;

            if (application.RepaymentMonths <= 60)
                score += 4;
            else if (application.RepaymentMonths > 180)
                score -= 8;

            if (score < 0)
                score = 0;

            if (score > 100)
                score = 100;

            application.CreditScore = score;
            application.CreditDecision = GetDecision(application);
        }

        private decimal CalculateEstimatedMonthlyInstallment(decimal amount, int months)
        {
            if (amount <= 0 || months <= 0)
                return 0;

            decimal annualInterestRate = 0.10m;
            decimal monthlyRate = annualInterestRate / 12;

            decimal power = (decimal)Math.Pow((double)(1 + monthlyRate), months);

            decimal installment = amount * monthlyRate * power / (power - 1);

            return Math.Round(installment, 2);
        }

        private decimal CalculateDti(LoanApplication application)
        {
            if (application.MonthlyIncome <= 0)
                return 100;

            decimal totalObligations =
                application.MonthlyExpenses + application.EstimatedMonthlyInstallment;

            decimal dti = totalObligations / application.MonthlyIncome * 100;

            return Math.Round(dti, 2);
        }

        private string GetDecision(LoanApplication application)
        {
            if (application.CreditScore >= 80 &&
                application.DisposableIncome >= application.EstimatedMonthlyInstallment * 1.3m &&
                application.DtiPercent <= 45)
            {
                return "Zdolność kredytowa pozytywna";
            }

            if (application.CreditScore >= 55 &&
                application.DisposableIncome >= application.EstimatedMonthlyInstallment &&
                application.DtiPercent <= 60)
            {
                return "Zdolność kredytowa warunkowa";
            }

            return "Brak zdolności kredytowej";
        }
    }
}