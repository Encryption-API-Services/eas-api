using Microsoft.AspNetCore.Mvc;
using Models.BenchmarkSDKSend;

namespace API.ControllerLogic
{
    public interface IBenchmarkSDKMethodControllerLogic
    {
        Task<IActionResult> CreateMethodSDKBenchmark(BenchmarkMacAddressSDKMethod sdkMethod, HttpContext context);
        Task<IActionResult> GetUserBenchmarksByDays(int daysAgo, HttpContext context);
    }
}
