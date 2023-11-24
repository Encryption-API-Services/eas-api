namespace Models.Encryption
{
    public class Argon2VerifyRequest
    {
        public string hashedPassword { get; set; }
        public string password { get; set; }
    }
}
