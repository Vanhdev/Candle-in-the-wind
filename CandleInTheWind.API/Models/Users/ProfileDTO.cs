namespace CandleInTheWind.API.Models.Users
{
    public class ProfileDTO : UserDTO
    {
        public int Id { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public int Points { get; set; }

        public string GenderName { get; set; }
    }
}
