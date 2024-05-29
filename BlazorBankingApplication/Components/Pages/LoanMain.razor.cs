//loan functionality
//All loan records closed and active
//if we click on a loan => details show up
//loan details => payment history and upcoming emi dates
//pay emi option
//reminder of upcoming loan

//calculate emi option done


using BlazorBankingApplication.DataAccess;
using BlazorBankingApplication.Models;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace BlazorBankingApplication.Components.Pages
{
    public partial class LoanMain
    {
        List<Customer> thisCust;
        string firstName = "";
        bool dashboard_status = false;
        List<Applyloan> activeLoans;
        List<Applyloan> closedLoans;
        int aloan = 0;
        int cloan = 0;
        bool loanOn = false;
        protected override async Task OnInitializedAsync()
        {
            string sql = "SELECT * FROM customers WHERE customer_id = @customer_id1";
            var customerParam = new { customer_id1 = SharedDataService.customerID };
            thisCust = await _data.LoadData<Customer, dynamic>(sql, new { customer_id1 = SharedDataService.customerID },
                _config.GetConnectionString("MySQLConnection"));
            firstName = thisCust[0].first_name;

            string sql1 = "SELECT * FROM loans WHERE CustomerID = @customer_id1 AND Status = true";
            activeLoans = await _data.LoadData<Applyloan, dynamic>(sql1, new { customer_id1 = SharedDataService.customerID },
                _config.GetConnectionString("MySQLConnection"));


            string sql2 = "SELECT * FROM loans WHERE CustomerID = @customer_id1 AND Status = false";
            closedLoans = await _data.LoadData<Applyloan, dynamic>(sql2, new { customer_id1 = SharedDataService.customerID },
                _config.GetConnectionString("MySQLConnection"));

            aloan = activeLoans.Count;
            cloan = closedLoans.Count;
            loanOn = true;
        }
    }
}
