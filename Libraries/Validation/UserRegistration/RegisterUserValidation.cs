using Models.UserAuthentication;

namespace Validation.UserRegistration
{
    public class RegisterUserValidation : ValidationRegex
    {
        public bool IsRegisterUserModelValid(RegisterUser model)
        {
            bool result = false;
            if (this.IsUserNameValid(model.username) && this.IsPasswordValid(model.password) && this.IsEmailValid(model.email))
            {
                result = true;
            }
            return result;
        }

        public bool IsEmailValid(string email)
        {
            bool result = false;
            if (this._emailRegex.IsMatch(email))
            {
                result = true;
            }
            return result;
        }
        public bool IsUserNameValid(string userName)
        {
            bool result = false;
            if (this._userRegex.IsMatch(userName))
            {
                result = true;
            }
            return result;
        }

        public bool IsPasswordValid(string password)
        {
            bool result = false;
            if (this._passwordRegex.IsMatch(password))
            {
                result = true;
            }
            return result;
        }
    }
}