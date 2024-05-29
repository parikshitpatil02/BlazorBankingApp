using BlazorBankingApplication.Models;

namespace BlazorBankingApplication.Components.Pages
{
    public partial class CreateAccount
    {
        private Customer newCustomer = new Customer();
        private UserLog newUser = new UserLog();
        public class AddCustomer
        {
            public string first_name {  get; set; }
            public string last_name { get; set; }
            public DateTime date_of_birth { get; set; } = DateTime.Now;
            public string address { get; set; }
            public string phone_number { get; set; }
            public string email { get; set; }
        }
        public class AddUser
        {
            public string Username { get; set; }
            public string Password { get; set; }

        }
        private AddCustomer addCustomer = new AddCustomer();
        private AddUser addUser = new AddUser();

        bool PersonalDetail = true;
        bool credential = false;
        bool accountCreated = false;
        private async Task SaveAccountInfo()
        {
            PersonalDetail = false;
            credential = true;
        }
        bool usernameAvailable = true;
        bool depositMoney = false;
        int custID=-1;

        List<UserLog> thisUser;
        List<Customer> thisCust;
        private async Task UserPass()
        {

            string sql = "SELECT * FROM users WHERE Username = @Username1";
            thisUser = await _data.LoadData<UserLog, dynamic>(sql, new { Username1 = addUser.Username },
                _config.GetConnectionString("MySQLConnection"));

            if(thisUser.Count>0)
            {   
                usernameAvailable = false ;
            }
            else
            {
                usernameAvailable = true;

                credential = false;
                depositMoney = true;

                //ISKE AAGE DEPOSIT MONEY OF RS 500 MINIMUN KRNA HAI AUR ACCOUNT KA TABLE BANANA HAI 
                //Transaction(PASSBOOK) KA BHI SATH MEH BANANA PADEGA

            }
        }

        public class firstDeposit
        {
            public decimal amount { get; set; }
        }
        private firstDeposit firstDepositAmt = new firstDeposit();

        
        private async Task MoneyDeposited()
        {
            Console.WriteLine(firstDepositAmt.amount);
            Customer a = new Customer
            {
                first_name = addCustomer.first_name,
                last_name = addCustomer.last_name,
                date_of_birth = addCustomer.date_of_birth,
                address = addCustomer.address,
                phone_number = addCustomer.phone_number,
                email = addCustomer.email
            };

            string sql1 = "INSERT INTO customers (first_name, last_name, date_of_birth, address, phone_number," +
                "email) VALUES (@first_name, @last_name, @date_of_birth, @address, @phone_number, @email);";
            await _data.SaveData(sql1, a, _config.GetConnectionString("MySQLConnection"));

            a = new Customer();
            thisUser.Clear();

            string sql2 = "SELECT * FROM customers WHERE email = @email1";
            thisCust = await _data.LoadData<Customer, dynamic>(sql2, new
            {
                email1 = addCustomer.email
            }, _config.GetConnectionString("MySQLConnection"));

            custID = thisCust[0].customer_id;
            Console.WriteLine("Customer ID: "+custID);

            addCustomer = new AddCustomer();

            UserLog b = new UserLog
            {
                Username = addUser.Username,
                Password = addUser.Password,
                customer_id = custID
            };
            string sql3 = "INSERT INTO users (Username, Password, customer_id) VALUES (@Username, @Password, " +
                "@customer_id);";
            await _data.SaveData(sql3, b, _config.GetConnectionString("MySQLConnection"));

            accountCreated = true;
            b = new UserLog();
            depositMoney = false;

            //saving first money deposit in saving account and transactions table
            SavingsAccount sa  = new SavingsAccount
            {
                current_balance = firstDepositAmt.amount,
                opening_balance = firstDepositAmt.amount,
                customer_id = custID
            };
            string sql4 = "INSERT INTO savings_account (customer_id, opening_balance, current_balance) VALUES (@customer_id, @opening_balance, " +
                "@current_balance);";
            await _data.SaveData(sql4, sa, _config.GetConnectionString("MySQLConnection"));

            //let's add the deposited money transaction in transactions table
            

            TransDone t = new TransDone
            {
                amount = firstDepositAmt.amount,
                customer_id = custID,
                trans_time = DateTime.UtcNow,
                transaction_type = true
                //deposit means true
            };
            string sql5 = "INSERT INTO transactions (customer_id, amount, trans_time, transaction_type) VALUES (@customer_id, @amount, @trans_time, @transaction_type);";
            await _data.SaveData(sql5, t, _config.GetConnectionString("MySQLConnection"));

        }
    }
}
