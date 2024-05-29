using BlazorBankingApplication.DataAccess;
using BlazorBankingApplication.Models;
using Google.Protobuf.WellKnownTypes;
using static BlazorBankingApplication.Components.Pages.ApplyLoan;
using static BlazorBankingApplication.Components.Pages.LoanEMICalculator;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace BlazorBankingApplication.Components.Pages
{
    
    public partial class ApplyLoan
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

        private Applyloan newLoan = new Applyloan();
        bool confirmLoan = false;
        bool loanPassed = false;
        public class LoanDetails
        {
            public double EMIAmount { get; set; }
            public DateTime? NextEMIDue { get; set; }
            public List<DateTime> EMIDates { get; set; } = new List<DateTime>(); // Initialize empty list
        }
        private LoanDetails loanDetails = new LoanDetails();
        async Task HandleSubmit()
        {
            loanDetails.EMIAmount = CalculateEMIAmount(newLoan.LoanAmount, newLoan.InterestRate, newLoan.Duration, newLoan.RepaymentMode);
            GenerateEMISchedule(loanDetails, newLoan); // Call new function
            loanDetails.NextEMIDue = loanDetails.EMIDates.FirstOrDefault(); // Set first EMI date as next due

            confirmLoan = true;
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

        private void GenerateEMISchedule(LoanDetails loanDetails, Applyloan newLoan)
        {
            loanDetails.EMIDates.Clear(); // Clear any existing dates

            DateTime startDate = DateTime.Today.AddMonths(1); // Start date (next month)

            switch (newLoan.RepaymentMode)
            {
                case "Monthly":
                    for (int i = 0; i < newLoan.Duration; i++)
                    {
                        loanDetails.EMIDates.Add(startDate.AddMonths(i));
                    }
                    break;
                case "Quarterly":
                    for (int i = 0; i < newLoan.Duration * 3; i += 3)
                    {
                        loanDetails.EMIDates.Add(startDate.AddMonths(i));
                    }
                    break;
                case "HalfYearly":
                    for (int i = 0; i < newLoan.Duration * 6; i += 6)
                    {
                        loanDetails.EMIDates.Add(startDate.AddMonths(i));
                    }
                    break;
                case "Yearly":
                    for (int i = 0; i < newLoan.Duration; i++)
                    {
                        loanDetails.EMIDates.Add(startDate.AddYears(i));
                    }
                    break;
                default:
                    throw new ArgumentException("Invalid repayment method");
            }
        }
        
        private async Task LoanRequest()
        {
            Applyloan a = new Applyloan
            {
                LoanAmount = newLoan.LoanAmount,
                CustomerID = SharedDataService.customerID,
                Duration = newLoan.Duration,
                Status = true,
                StartDate = DateTime.UtcNow,
                RepaymentMode = newLoan.RepaymentMode
            };
            string sql = "INSERT INTO loans (LoanAmount, CustomerID, Duration, Status, StartDate, RepaymentMode) VALUES (@LoanAmount, @CustomerID, @Duration, @Status, @StartDate, @RepaymentMode);";
            await _data.SaveData(sql, a, _config.GetConnectionString("MySQLConnection"));

            List<Applyloan> thisLoan;
            
            string sql1 = "SELECT * FROM loans WHERE CustomerID = @cust";
                thisLoan = await _data.LoadData<Applyloan, dynamic>(sql1, new { cust = SharedDataService.customerID },
                    _config.GetConnectionString("MySQLConnection"));

            int loanid = thisLoan[thisLoan.Count-1].LoanID;
            //filling the emi table
            foreach (var emidate in loanDetails.EMIDates)
            {
                EMIModel b = new EMIModel
                {
                    LoanID = loanid,
                    Amount = loanDetails.EMIAmount,
                    due_date = emidate,
                    status = true
                };
                string emiSql = "INSERT INTO emi (LoanID, Amount, due_date, status) VALUES (@LoanID, @Amount, @due_date, @status);";
                await _data.SaveData(emiSql, b, _config.GetConnectionString("MySQLConnection"));
            }

            TransDone t = new TransDone
            {
                amount = (decimal)newLoan.LoanAmount,
                customer_id = SharedDataService.customerID,
                trans_time = DateTime.UtcNow,
                transaction_type = true
                //deposit means true
            };
            string sql5 = "INSERT INTO transactions (customer_id, amount, trans_time, transaction_type) VALUES (@customer_id, @amount, @trans_time, @transaction_type);";
            await _data.SaveData(sql5, t, _config.GetConnectionString("MySQLConnection"));

            List<SavingsAccount> thisAccount;   
            //Getting the current Balance
            string sqll = "SELECT * FROM savings_account WHERE customer_id = @customer_id1";
            thisAccount = await _data.LoadData<SavingsAccount, dynamic>(sqll, new { customer_id1 = SharedDataService.customerID }
            , _config.GetConnectionString("MySQLConnection"));

            //Updating the current balance with adding depositing amoount
            string sql2 = "UPDATE savings_account SET current_balance = @current_balance1 WHERE customer_id = @customer_id1;";
            await _data.SaveData(sql2, new { current_balance1 = thisAccount[0].current_balance + (decimal)newLoan.LoanAmount, customer_id1 = SharedDataService.customerID }, _config.GetConnectionString("MySQLConnection"));

            loanPassed = true;
        }
    }
}
