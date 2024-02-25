using Microsoft.AspNetCore.Mvc;
using Models.UserAuthentication;

namespace API.ControllersLogic
{
    public interface IPasswordControllerLogic
    {
        Task<IActionResult> ForgotPassword(ForgotPasswordRequest email, HttpContext context);
        Task<IActionResult> ResetPassword(ResetPasswordRequest body, HttpContext context);
    }
}