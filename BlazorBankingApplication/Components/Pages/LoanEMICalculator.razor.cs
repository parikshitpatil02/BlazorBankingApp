
using BlazorBankingApplication.DataAccess;
using BlazorBankingApplication.Models;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace BlazorBankingApplication.Components.Pages
{
    public partial class LoanEMICalculator
    {
        List<Customer> thisCust;
        string firstName = "";
        protected override async Task OnInitializedAsync()
        {
            string sql = "SELECT * FROM customers WHERE customer_id = @customer_id1";
            var customerParam = new { customer_id1 = SharedDataService.customerID };
            thisCust = await _data.LoadData<Customer, dynamic>(sql, new { customer_id1 = SharedDataService.customerID },
                _config.GetConnectionString("MySQLConnection"));
            firstName = thisCust[0].first_name;


        }
        public class LoanDetails
        {
            public double LoanAmount { get; set; }
            public double InterestRate { get; set; }
            public string RepaymentMethod { get; set; }
            public int RepaymentTenure { get; set; }
            public double EMIAmount { get; set; }
            public DateTime? NextEMIDue { get; set; }
            public List<DateTime> EMIDates { get; set; } = new List<DateTime>(); // Initialize empty list
        }


        LoanDetails loanDetails = new LoanDetails();

        async Task HandleSubmit()
        {
            if (loanDetails.LoanAmount <= 0 || loanDetails.InterestRate <= 0 || loanDetails.RepaymentTenure <= 0)
            {
                // Handle validation errors (e.g., display error message)
                return;
            }

            loanDetails.EMIAmount = CalculateEMIAmount(loanDetails.LoanAmount, loanDetails.InterestRate, loanDetails.RepaymentTenure, loanDetails.RepaymentMethod);
            GenerateEMISchedule(loanDetails); // Call new function
            loanDetails.NextEMIDue = loanDetails.EMIDates.FirstOrDefault(); // Set first EMI date as next due
        }

            private double CalculateEMIAmount(double loanAmount, double interestRate, int repaymentTenure, string repaymentMethod)
        {
            // Convert interest rate to a monthly decimal value
            double monthlyInterestRate = interestRate / (100 * GetNumberOfPaymentsPerYear(repaymentMethod));

            // Calculate the EMI using the EMI formula
            double emi = (loanAmount * monthlyInterestRate) / (1 - Math.Pow(1 + monthlyInterestRate, -repaymentTenure));

            return Math.Round(emi, 2); // Round to two decimal places
        }

        private int GetNumberOfPaymentsPerYear(string repaymentMethod)
        {
            if (repaymentMethod == "Monthly")
                return 12;
            else if (repaymentMethod == "Quarterly")
                return 4;
            else if (repaymentMethod == "HalfYearly")
                return 2;
            return 1;
        }
        private void GenerateEMISchedule(LoanDetails loanDetails)
        {
            loanDetails.EMIDates.Clear(); // Clear any existing dates

            DateTime startDate = DateTime.Today.AddMonths(1); // Start date (next month)

            switch (loanDetails.RepaymentMethod)
            {
                case "Monthly":
                    for (int i = 0; i < loanDetails.RepaymentTenure; i++)
                    {
                        loanDetails.EMIDates.Add(startDate.AddMonths(i));
                    }
                    break;
                case "Quarterly":
                    for (int i = 0; i < loanDetails.RepaymentTenure * 3; i += 3)
                    {
                        loanDetails.EMIDates.Add(startDate.AddMonths(i));
                    }
                    break;
                case "HalfYearly":
                    for (int i = 0; i < loanDetails.RepaymentTenure * 6; i += 6)
                    {
                        loanDetails.EMIDates.Add(startDate.AddMonths(i));
                    }
                    break;
                case "Yearly":
                    for (int i = 0; i < loanDetails.RepaymentTenure; i++)
                    {
                        loanDetails.EMIDates.Add(startDate.AddYears(i));
                    }
                    break;
                default:
                    throw new ArgumentException("Invalid repayment method");
            }
        }

    }
}
