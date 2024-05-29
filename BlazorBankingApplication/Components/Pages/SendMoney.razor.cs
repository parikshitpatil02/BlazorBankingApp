using BlazorBankingApplication.DataAccess;
using BlazorBankingApplication.Models;
using System.ComponentModel.DataAnnotations;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace BlazorBankingApplication.Components.Pages
{
    public partial class SendMoney
    {
        public class TransferModel
        {
            public string RecipientName { get; set; }

            public decimal Amount { get; set; }

            public string AccountNumber { get; set; }

            public string Message { get; set; }
        }


        TransferModel transferModel = new TransferModel();

        bool insufficientBalance = false;
        bool moneysent = false;
        List<SavingsAccount> thisAccount;
        private async Task HandleSubmit()
        {
            TransDone t = new TransDone()
            {
                amount = transferModel.Amount,
                customer_id = SharedDataService.customerID,
                trans_time = DateTime.UtcNow,
                transaction_type = false
                //false means withdrawal

            };
            


            //Getting the current Balance
            string sql1 = "SELECT * FROM savings_account WHERE customer_id = @customer_id1";
            thisAccount = await _data.LoadData<SavingsAccount, dynamic>(sql1, new { customer_id1 = SharedDataService.customerID }
            , _config.GetConnectionString("MySQLConnection"));

            if (transferModel.Amount > thisAccount[0].current_balance)
            {
                insufficientBalance = true;
            }
            else
            {
                //Updating the current balance with subtracting sending amoount
                string sql2 = "UPDATE savings_account SET current_balance = @current_balance1 WHERE customer_id = @customer_id1;";
                await _data.SaveData(sql2, new { current_balance1 = thisAccount[0].current_balance - transferModel.Amount, customer_id1 = SharedDataService.customerID }, _config.GetConnectionString("MySQLConnection"));

                string sql = "INSERT INTO transactions (customer_id, amount, trans_time, transaction_type) VALUES (@customer_id, @amount, @trans_time, @transaction_type);";
                await _data.SaveData(sql, t, _config.GetConnectionString("MySQLConnection"));
                moneysent = true;
                insufficientBalance = false;
            }

            ClearForm();
        }

        void ClearForm()
        {
            transferModel = new TransferModel();
        }
    }
}





