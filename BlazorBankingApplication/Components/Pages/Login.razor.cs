
using BlazorBankingApplication.Models;
using BlazorBankingApplication.DataAccess;
using static Org.BouncyCastle.Math.EC.ECCurve;
using static BlazorBankingApplication.Components.Pages.ActiveLoans;
using Microsoft.AspNetCore.Mvc.Routing;



namespace BlazorBankingApplication.Components.Pages
{
    public partial class Login
    {
        

       

        

        bool createStatus = true;
        private UserLog newUser = new UserLog();
        public class AddUser
        {
            public string UName { get; set; }
            public string Pword { get; set; }
        }
        private AddUser addUser = new AddUser();

        List<UserLog> thisUser;
        bool sos = false;
        bool userVerified = false;
        bool ver = false;

        //Edit Appointment
        private async Task VerifyLogin()
        {

            string sql = "SELECT * FROM users WHERE Username = @Username";
            thisUser = await _data.LoadData<UserLog, dynamic>(sql, new { Username = addUser.UName },
                _config.GetConnectionString("MySQLConnection"));


            ver = true;
            if (thisUser.Count > 0)
            {
                sos = true;
                string passwordInDB = thisUser[0].Password;
                if (passwordInDB == addUser.Pword)
                {
                    userVerified = true;
                    SharedDataService.customerID = thisUser[0].customer_id;
                    SharedDataService.loginValid = true;
                    
                    NavigationManager.NavigateTo("/dashboard");
                }
                else
                {
                    NavigationManager.NavigateTo("/login");
                }
            }
            else
            {
                sos = false;
            }

        }

        
    }
}
