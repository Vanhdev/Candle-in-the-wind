namespace CandleInTheWind.API.Models.Accounts
{
    public class AccountResponseDTO
    {
        public bool Success { get; set; }
        public string AccessToken { get; set; }
        public string Error { get; set; }
    }
}
