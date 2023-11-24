namespace Models.UserAuthentication
{
    public class ResetPasswordRequest
    {
        public string Id { get; set; }
        public string Token { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
