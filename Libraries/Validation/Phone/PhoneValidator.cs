using System;

namespace Validation.Phone
{
    public class PhoneValidator : ValidationRegex
    {
        public bool IsPhoneNumberValid(string phoneNumber)
        {
            bool result = false;
            if (!string.IsNullOrEmpty(phoneNumber))
            {
                if (this._phoneNumberRegex.IsMatch(phoneNumber))
                {
                    result = true;
                }
            }
            else
            {
                throw new Exception("Please provide a string to to test for phone number validation");
            }
            return result;
        }
    }
}