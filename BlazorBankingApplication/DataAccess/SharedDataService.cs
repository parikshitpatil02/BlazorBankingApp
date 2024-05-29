using BlazorBankingApplication.Models;

namespace BlazorBankingApplication.DataAccess
{
    public class SharedDataService
    {
        public int customerID { get; set; }
        public bool loginValid { get; set; } = false;
    }
}
