namespace BlazorBankingApplication.Models
{
    public class Applyloan
    {
            public int LoanID { get; set; }
            public int CustomerID { get; set; }
            public double LoanAmount { get; set; }
            public double InterestRate { get; set; } = 12;
            public int Duration { get; set; } 
            public bool Status { get; set; } // Assuming boolean represents active/inactive
            public DateTime StartDate { get; set; }
            public string RepaymentMode { get; set; }
            public DateTime next_pay_date { get; set; }

    }
}
