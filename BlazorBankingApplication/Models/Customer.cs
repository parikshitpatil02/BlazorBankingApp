namespace BlazorBankingApplication.Models
{
    public class Customer
    {
        public int customer_id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public DateTime date_of_birth { get; set; }
        public string address { get; set; }
        public string phone_number { get; set; }
        public string email { get; set; }
    }
}
