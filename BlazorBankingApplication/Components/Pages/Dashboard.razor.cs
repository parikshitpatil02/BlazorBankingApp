
using BlazorBankingApplication.DataAccess;
using BlazorBankingApplication.Models;
using System.Transactions;
using static BlazorBankingApplication.Components.Pages.CreateAccount;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace BlazorBankingApplication.Components.Pages
{
    public partial class Dashboard
    {
        List<Customer> thisCust;
        string firstName = "";
        bool dashboard_status = false;
        //yaha reminder msg wala code hai
        public class due10Loans
        {
            public int LoanID { get; set; }
            public DateTime due_date { get; set; }
        };
        List<due10Loans> dueLoans = new List<due10Loans>();
        List<Applyloan> thisLoan;
        int daysDifferece;
        DateTime today = DateTime.UtcNow;
        //yaha tak
        protected override async Task OnInitializedAsync()
        {
            string sql = "SELECT * FROM customers WHERE customer_id = @customer_id1";
            var customerParam = new { customer_id1 = SharedDataService.customerID };
            thisCust = await _data.LoadData<Customer, dynamic>(sql, new { customer_id1 = SharedDataService.customerID },
                _config.GetConnectionString("MySQLConnection"));
            firstName = thisCust[0].first_name;

            //ye bhi reminder msg ka code hai
            string sql11 = "SELECT * FROM loans WHERE CustomerID = @customer_id1 AND Status = true";
            thisLoan = await _data.LoadData<Applyloan, dynamic>(sql11, new { customer_id1 = SharedDataService.customerID },
                _config.GetConnectionString("MySQLConnection"));
            foreach (var item in thisLoan)
            {
                daysDifferece = GetDaysDifference(today, item.next_pay_date);
                if (daysDifferece <= 10)
                {
                    due10Loans newdue = new due10Loans();
                    newdue.LoanID = item.LoanID;
                    newdue.due_date = item.next_pay_date;
                    dueLoans.Add(newdue);
                }
            }
        }

        public static int GetDaysDifference(DateTime currDate, DateTime dueDate)
        {
            // Calculate the time difference
            TimeSpan difference = dueDate - currDate;

            // Return only the whole number of days (discard hours, minutes, etc.)
            return difference.Days;
        }



        //aur yaha tak ka code

        List<SavingsAccount> thisAccount;
        List<TransDone> transaction5;
        decimal currBalance;
        private async Task StartDashboard()
        {
            dashboard_status = true;
            string sql = "SELECT * FROM savings_account WHERE customer_id = @customer_id1";
            var customerParam = new { customer_id1 = SharedDataService.customerID };
            thisAccount = await _data.LoadData<SavingsAccount, dynamic>(sql, new { customer_id1 = SharedDataService.customerID },
                _config.GetConnectionString("MySQLConnection"));
            currBalance = thisAccount[0].current_balance;


            transaction5 = new List<TransDone>();
            const string sql1 = "SELECT * FROM transactions ORDER BY trans_time DESC LIMIT 5";
            transaction5 = await _data.LoadData<TransDone, dynamic>(sql1, new { customer_id1 = SharedDataService.customerID },
                _config.GetConnectionString("MySQLConnection"));
            Console.WriteLine(transaction5[0].amount);

        }
    }
}


 //iske aage add money and send money wala feature implement kr denge
 //firr recent transactions wala tab add kr denge
 //firr loans wala page develop krenge