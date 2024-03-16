namespace Models.UserAdmin
{
    public class UserTableItem
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public bool IsActive { get; set; }
        public bool IsAdmin { get; set; }
    }
}
