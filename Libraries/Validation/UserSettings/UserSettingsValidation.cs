﻿using CasDotnetSdk.PasswordHashers;
using DataLayer.Mongo.Entities;
using DataLayer.Mongo.Repositories;
using Models.UserAuthentication;
using Models.UserSettings;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.Marshalling;
using System.Threading.Tasks;

namespace Validation.UserSettings
{
    public class UserSettingsValidation : ValidationRegex
    {
        private readonly IForgotPasswordRepository _forgotPasswordRepository;
        private readonly IUserRepository _userRepository;
        public UserSettingsValidation(
            IForgotPasswordRepository forgotPasswordRepository,
            IUserRepository userRepository
            )
        {
            this._forgotPasswordRepository = forgotPasswordRepository;
            this._userRepository = userRepository;
        }

        public async Task<Tuple<bool, string>> IsChangePasswordValid(ChangePassword body, string userId, string currentPassword, Argon2Wrapper argon2Wrapper)
        {
            Tuple<bool, string> isValid = new Tuple<bool, string>(true, "");
            if (!this._passwordRegex.IsMatch(body.NewPassword))
            {
                isValid = new Tuple<bool, string>(false, "New Password must be 8 characters long and must contain at least one uppercase letter, one lowercase letter, one number and one special character");
            }
            else if (!this._passwordRegex.IsMatch(body.CurrentPassword))
            {
                isValid = new Tuple<bool, string>(false, "Current Password must be 8 characters long and must contain at least one uppercase letter, one lowercase letter, one number and one special character");
            }
            else if (!argon2Wrapper.Verify(currentPassword, body.CurrentPassword))
            {
                isValid = new Tuple<bool, string>(false, "The Current Password you entered does not match our records");
            }
            List<string> lastFivePasswords = await this._forgotPasswordRepository.GetLastFivePassword(userId);
            foreach (string password in lastFivePasswords)
            {
                if (argon2Wrapper.Verify(password, body.NewPassword))
                {
                    isValid = new Tuple<bool, string>(false, "You need to enter a password that has not been used the last 5 times");
                    break;
                }
            }
            return isValid;
        }

        public async Task<Tuple<bool, string>> IsChangeUsernameValid(ChangeUserName body)
        {
            Tuple<bool, string> isValid = new Tuple<bool, string>(true, "");
            if (!this._userRegex.IsMatch(body.NewUsername))
            {
                isValid = new Tuple<bool, string>(false, "Username must be 3-16 characters long and can contain only letters and numbers");
            }
            else
            {
                User user = await this._userRepository.GetUserByUsername(body.NewUsername);
                if (user != null && user.Username == body.NewUsername)
                {
                    isValid = new Tuple<bool, string>(false, "You cannot change your username to your current username");
                }
                else if (user != null)
                {
                    isValid = new Tuple<bool, string>(false, "Username is already taken");
                }
            }
            return isValid;
        }

        public EmergencyKitValidationResult IsEmergencyKitValid(EmergencyKitRecoveryBody body)
        {
            EmergencyKitValidationResult result = new EmergencyKitValidationResult();
            if (string.IsNullOrEmpty(body.SecretKey))
            {
                result.IsValid = false;
                result.ErrorMessage = "No secret key supplied";
                return result;
            }
            Span<byte> buffer = new Span<byte>(new byte[body.SecretKey.Length]);
            bool isValidBase64 = Convert.TryFromBase64String(body.SecretKey, buffer, out int _);
            if (!isValidBase64)
            {
                result.IsValid = false;
                result.ErrorMessage = "Secret key not in correct format";
                return result;
            }
            if (string.IsNullOrEmpty(body.Email))
            {
                result.IsValid = false;
                result.ErrorMessage = "No email address with supplied";
                return result;
            }
            if (!this._emailRegex.IsMatch(body.Email))
            {
                result.IsValid = false;
                result.ErrorMessage = "Not a valid email";
                return result;
            }
            User user = this._userRepository.GetUserByEmail(body.Email).GetAwaiter().GetResult();
            if (user == null)
            {
                result.IsValid = false;
                result.ErrorMessage = "No user exists with that email";
                return result;
            }

            result.IsValid = true;
            result.AccountRecoverySettings = user.EmergencyKitAccountRecoverySettings;
            return result;
        }
    }
}
