public class BookingViewModel
{
    public int BookingId { get; set; }
    public int UserId { get; set; }
    public string Username { get; set; }
    public DateTime BookingDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string BookingStatus { get; set; }
    // Add other properties as needed
}