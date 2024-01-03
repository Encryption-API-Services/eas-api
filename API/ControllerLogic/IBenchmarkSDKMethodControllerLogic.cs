using CASHelpers.Types.HttpResponses.BenchmarkAPI;
using Microsoft.AspNetCore.Mvc;

namespace API.ControllerLogic
{
    public interface IBenchmarkSDKMethodControllerLogic
    {
        Task<IActionResult> CreateMethodSDKBenchmark(BenchmarkSDKMethod sdkMethod, HttpContext context);
    }
}
