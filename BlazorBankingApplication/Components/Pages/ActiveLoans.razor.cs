using BlazorBankingApplication.DataAccess;
using BlazorBankingApplication.Models;
using Microsoft.VisualBasic;
using Mysqlx.Crud;
using static BlazorBankingApplication.Components.Pages.SendMoney;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace BlazorBankingApplication.Components.Pages
{
    public partial class ActiveLoans
    {
        bool payEMIon = false;
        bool lastfunc = false;
        List <Applyloan> thisLoan;// iske andar saare loans hai
        //List<List<EMIModel>> thisLoanEMI = new List<List<EMIModel>>(); // iske andar individual loan k emi ke details hai
        List<Customer> thisCust;
        string firstName = "";

        public class LoanSummary
        {
            public int LoanID { get; set; }
            public double LoanAmount { get; set; }
            public double InterestRate { get; set; } = 12;
            public int Duration { get; set; }
            public DateTime StartDate { get; set; }
            public string RepaymentMode { get; set; }

            //iskse baad emi model wala content hai, iske pehle loans table wala
            public double EMIAmount { get; set; }
            public DateTime due_date { get; set; }

            //ye apne ko emi table se banana hai
            public int paidEMIs { get; set; }
            public int unpaidEMIs { get; set; }
        }
        List<LoanSummary> CompleteLoanDetails = new List<LoanSummary>();
        protected override async Task OnInitializedAsync()
        {
            string sql = "SELECT * FROM customers WHERE customer_id = @customer_id1";
            var customerParam = new { customer_id1 = SharedDataService.customerID };
            thisCust = await _data.LoadData<Customer, dynamic>(sql, new { customer_id1 = SharedDataService.customerID },
                _config.GetConnectionString("MySQLConnection"));
            firstName = thisCust[0].first_name;

            string sql1 = "SELECT * FROM loans WHERE CustomerID = @customer_id1 AND Status = true";
            thisLoan = await _data.LoadData<Applyloan, dynamic>(sql1, new { customer_id1 = SharedDataService.customerID },
                _config.GetConnectionString("MySQLConnection"));


            foreach (var item in thisLoan)
            {
                LoanSummary newLoan = new LoanSummary();
                newLoan.LoanID = item.LoanID;
                newLoan.LoanAmount = item.LoanAmount;
                newLoan.Duration = item.Duration;
                newLoan.StartDate = item.StartDate;
                newLoan.InterestRate = 12;
                newLoan.RepaymentMode = item.RepaymentMode;

                string sql2 = "SELECT * FROM emi WHERE LoanID = @loanID1 ORDER BY due_date ASC;";
                List<EMIModel> emiList = await _data.LoadData<EMIModel, dynamic>(sql2, new { loanID1 = item.LoanID },
                    _config.GetConnectionString("MySQLConnection"));

                int paidEMi = 0, unpaidEMi = emiList.Count;
                //status true matlab emi is active and unpaid
                newLoan.due_date = emiList[0].due_date;
                foreach (var currEMI in emiList)
                {
                    if (currEMI.status == false)
                    {
                        paidEMi++;
                        unpaidEMi--;
                        newLoan.due_date = currEMI.due_date;
                    }
                    else
                    {
                        break;
                    }
                }
                newLoan.paidEMIs = paidEMi;
                newLoan.unpaidEMIs = unpaidEMi;
                newLoan.EMIAmount = emiList[0].Amount;
                CompleteLoanDetails.Add(newLoan);
            }
        }

        LoanSummary currentLoan = new LoanSummary();

        DateTime today = DateTime.UtcNow;
        int daysDifference;
        
        private async Task PayEMIFunc(LoanSummary currLoan)
        {
            payEMIon = true;
            currentLoan = currLoan;
            daysDifference = GetDaysDifference(today, currentLoan.due_date);

        }
        public static int GetDaysDifference(DateTime currDate, DateTime dueDate)
        {
            // Calculate the time difference
            TimeSpan difference = dueDate - currDate;

            // Return only the whole number of days (discard hours, minutes, etc.)
            return difference.Days;
        }

        public class EMIdummy
        {
            public double emiAmount { get; set; }
        };
        EMIdummy emidum = new EMIdummy();

        bool balanceAboveEMI = false;
        bool emiDeducted = false;
        int EMi_ID;
        DateTime nxt = DateTime.UtcNow;
        private async Task EMIPaymentFunc()
        {
            payEMIon = false;
            lastfunc = true;
            List<SavingsAccount> currAccount = new List<SavingsAccount>();
            string sql1 = "SELECT * FROM savings_account WHERE customer_id = @customer_id1";
            currAccount = await _data.LoadData<SavingsAccount, dynamic>(sql1, new { customer_id1 = SharedDataService.customerID }
            , _config.GetConnectionString("MySQLConnection"));
            if (currentLoan.EMIAmount <= (double)currAccount[0].current_balance)
            {
                balanceAboveEMI = true;
                //savings account update 
                string sql2 = "UPDATE savings_account SET current_balance = @current_balance1 WHERE customer_id = @customer_id1;";
                await _data.SaveData(sql2, new { current_balance1 = currAccount[0].current_balance - (decimal)currentLoan.EMIAmount, customer_id1 = SharedDataService.customerID }, _config.GetConnectionString("MySQLConnection"));

                //adding transaction
                TransDone t = new TransDone
                {
                    amount = (decimal)currentLoan.EMIAmount,
                    customer_id = SharedDataService.customerID,
                    trans_time = DateTime.UtcNow,
                    transaction_type = false
                    //deposit means true
                };
                string sql = "INSERT INTO transactions (customer_id, amount, trans_time, transaction_type) VALUES (@customer_id, @amount, @trans_time, @transaction_type);";
                await _data.SaveData(sql, t, _config.GetConnectionString("MySQLConnection"));

                //finding the emi id of emi being paid of the currentLoan
                string sql4 = "SELECT * FROM emi WHERE LoanID = @loanID1 ORDER BY due_date ASC;";
                List<EMIModel> newEMIList = await _data.LoadData<EMIModel, dynamic>(sql4, new { loanID1 = currentLoan.LoanID },
                    _config.GetConnectionString("MySQLConnection"));

                //status true matlab emi is active and unpaid
                int paidEMi = 0, unpaidEMi = newEMIList.Count;

                foreach (var currEMI in newEMIList)
                {
                    if (currEMI.status == false)
                    {
                        paidEMi++;
                        unpaidEMi--;
                    }
                    else //ye last emi jo pay krne wale hai woh bhi gin liya
                    {
                        paidEMi++;
                        unpaidEMi--;
                        EMi_ID = currEMI.emi_id;
                        break;
                    }
                }
                if (paidEMi == newEMIList.Count)
                {
                    //all emis are paid so make the status of the loan in loans table false
                    string loansql1 = "UPDATE loans SET Status = @status1 WHERE LoanID = @loandid1;";
                    await _data.SaveData(loansql1, new { status1 = false, loanid1 = currentLoan.LoanID}, 
                        _config.GetConnectionString("MySQLConnection"));

                }
                else
                {
                    nxt = newEMIList[paidEMi].due_date;
                    string loansql2 = "UPDATE loans SET next_pay_date = @nxt1 WHERE LoanID = @loandid11;";
                    await _data.SaveData(loansql2, new { nxt1 = nxt, loandid11 = currentLoan.LoanID }, 
                        _config.GetConnectionString("MySQLConnection"));
                }   

                //updating the emi
                string sql3 = "UPDATE emi SET payment_date = @pay_date1, status = @status11 WHERE emi_id = @EMI1;";
                await _data.SaveData(sql3, new { pay_date1 = DateTime.UtcNow, status11 = false, EMI1 = EMi_ID }, _config.GetConnectionString("MySQLConnection"));


                emiDeducted = true;
            }
            
        }
    }

}
