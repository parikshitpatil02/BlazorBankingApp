namespace BlazorBankingApplication.Models
{
    public class EMIModel
    {
        public int emi_id { get; }
        public int LoanID { get; set; }
        public double Amount { get; set; }
        public DateTime due_date { get; set; }
        public DateTime payment_date { get; set; }
        public bool status { get; set; }
    }
}
