namespace BlazorBankingApplication.Models
{

    public class TransDone
    {
        public bool transaction_type { get; set; }
        public DateTime trans_time { get; set; }
        public int customer_id { get; set; }
        public decimal amount { get; set; }
    }
}
