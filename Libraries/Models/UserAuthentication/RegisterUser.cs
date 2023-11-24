namespace Models.UserAuthentication
{
    public class RegisterUser
    {
        public string username { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string AddressOne { get; set; }
        public string AddressTwo { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Country { get; set; }
    }
}