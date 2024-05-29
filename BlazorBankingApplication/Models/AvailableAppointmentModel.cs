namespace BlazorBankingApplication.Models
{
    public class AvailableAppointmentModel
    {
        public int Id { get; set; }
        public bool Reserved { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
        public bool Active { get; set; }
    }
}
