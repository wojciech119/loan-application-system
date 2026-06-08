using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoanApplicationSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddCreditworthinessFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreditDecision",
                table: "LoanApplications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "CreditHistory",
                table: "LoanApplications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Dependents",
                table: "LoanApplications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "DisposableIncome",
                table: "LoanApplications",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DtiPercent",
                table: "LoanApplications",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "EmploymentType",
                table: "LoanApplications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "EstimatedMonthlyInstallment",
                table: "LoanApplications",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "HasProperty",
                table: "LoanApplications",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreditDecision",
                table: "LoanApplications");

            migrationBuilder.DropColumn(
                name: "CreditHistory",
                table: "LoanApplications");

            migrationBuilder.DropColumn(
                name: "Dependents",
                table: "LoanApplications");

            migrationBuilder.DropColumn(
                name: "DisposableIncome",
                table: "LoanApplications");

            migrationBuilder.DropColumn(
                name: "DtiPercent",
                table: "LoanApplications");

            migrationBuilder.DropColumn(
                name: "EmploymentType",
                table: "LoanApplications");

            migrationBuilder.DropColumn(
                name: "EstimatedMonthlyInstallment",
                table: "LoanApplications");

            migrationBuilder.DropColumn(
                name: "HasProperty",
                table: "LoanApplications");
        }
    }
}
