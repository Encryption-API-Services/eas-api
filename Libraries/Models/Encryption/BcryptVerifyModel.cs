namespace Models.Encryption
{
    public class BcryptVerifyModel
    {
        public string Password { get; set; }
        public string HashedPassword { get; set; }
    }
}