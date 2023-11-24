using System.Text.RegularExpressions;

namespace Validation
{
    public class ValidationRegex
    {
        public Regex _userRegex { get; set; }
        public Regex _passwordRegex { get; set; }
        public Regex _emailRegex { get; set; }
        public Regex _phoneNumberRegex { get; set; }

        public ValidationRegex()
        {
            this._userRegex = new Regex(@"^(?=.*?[a-zA-Z0-9]).{3,16}$");
            this._passwordRegex = new Regex(@"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$");
            this._emailRegex = new Regex(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$");
            this._phoneNumberRegex = new Regex(@"^((\\+91-?)|0)?[0-9]{10}$");
        }
    }
}
