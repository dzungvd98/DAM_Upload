namespace DAM_Upload.Models
{
    public class BlacklistedToken
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public DateTime BlacklistedAt { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
