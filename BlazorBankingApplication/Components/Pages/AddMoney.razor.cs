using BlazorBankingApplication.Models;
using BlazorBankingApplication.DataAccess;
using static BlazorBankingApplication.Components.Pages.CreateAccount;
using static BlazorBankingApplication.Components.Pages.Login;
using Google.Protobuf.WellKnownTypes;
using Mysqlx.Crud;
using System.Collections.Generic;
using static Org.BouncyCastle.Math.EC.ECCurve;


namespace BlazorBankingApplication.Components.Pages
{
    public partial class AddMoney
    {
        public class newDeposit
        {
            public decimal amount { get; set; }
        }
        private newDeposit DepositAmt = new newDeposit();

        bool moneyAdded = false;

        List<SavingsAccount> thisAccount;
        private async Task AddMoneyFunc()
        {
            TransDone t = new TransDone()
            {
                amount = DepositAmt.amount,
                customer_id = SharedDataService.customerID,
                trans_time = DateTime.UtcNow,
                transaction_type = true
                //true means deposit

            };
            string sql = "INSERT INTO transactions (customer_id, amount, trans_time, transaction_type) VALUES (@customer_id, @amount, @trans_time, @transaction_type);";
            await _data.SaveData(sql, t, _config.GetConnectionString("MySQLConnection"));

            //Getting the current Balance
            string sql1 = "SELECT * FROM savings_account WHERE customer_id = @customer_id1";
            thisAccount = await _data.LoadData<SavingsAccount, dynamic>(sql1, new { customer_id1 = SharedDataService.customerID }
            , _config.GetConnectionString("MySQLConnection"));

            //Updating the current balance with adding depositing amoount
            string sql2 = "UPDATE savings_account SET current_balance = @current_balance1 WHERE customer_id = @customer_id1;";
            await _data.SaveData(sql2, new { current_balance1 = thisAccount[0].current_balance + DepositAmt.amount, customer_id1 = SharedDataService.customerID }, _config.GetConnectionString("MySQLConnection"));

            moneyAdded = true;
        }
        
    }
}


