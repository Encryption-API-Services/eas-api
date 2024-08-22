namespace Models.UserAdmin
{
    public class UserActivationStatusRequest
    {
        public string UserId { get; set; }
        public bool IsActive { get; set; }
    }
}
