using BlazorBankingApplication.Models;

namespace BlazorBankingApplication.Components.Pages
{
    public partial class ClosedLoans
    {
        List<Applyloan> thisLoan;// iske andar saare loans hai
        //List<List<EMIModel>> thisLoanEMI = new List<List<EMIModel>>(); // iske andar individual loan k emi ke details hai
        List<Customer> thisCust;
        string firstName = "";
        protected override async Task OnInitializedAsync()
        {
            string sql = "SELECT * FROM customers WHERE customer_id = @customer_id1";
            var customerParam = new { customer_id1 = SharedDataService.customerID };
            thisCust = await _data.LoadData<Customer, dynamic>(sql, new { customer_id1 = SharedDataService.customerID },
                _config.GetConnectionString("MySQLConnection"));
            firstName = thisCust[0].first_name;

            string sql1 = "SELECT * FROM loans WHERE CustomerID = @customer_id1 AND Status = false";
            thisLoan = await _data.LoadData<Applyloan, dynamic>(sql1, new { customer_id1 = SharedDataService.customerID },
                _config.GetConnectionString("MySQLConnection"));


            //foreach(var item in thisLoan)
            //{
            //    Console.WriteLine("This is LoanID " + item.LoanID);
            //    string sql2 = "SELECT * FROM emi WHERE LoanID = @loanID1";
            //    List<EMIModel> emiList = await _data.LoadData<EMIModel, dynamic>(sql2, new { loanID1 = item.LoanID },
            //        _config.GetConnectionString("MySQLConnection"));
            //    thisLoanEMI.Add(emiList);
            //}
        }
    }
}
