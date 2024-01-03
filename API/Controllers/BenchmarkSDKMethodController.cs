using API.ControllerLogic;
using CASHelpers;
using CASHelpers.Types.HttpResponses.BenchmarkAPI;
using Microsoft.AspNetCore.Mvc;
using System.Drawing.Text;

namespace API.Controllers
{
    public class BenchmarkSDKMethodController : ControllerBase
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IBenchmarkSDKMethodControllerLogic _benchmarkSDKMethodControllerLogic;
        public BenchmarkSDKMethodController(
            IHttpContextAccessor contextAccessor,
            IBenchmarkSDKMethodControllerLogic benchmarkSDKMethodControllerLogic
            ) 
        { 
            this._contextAccessor = contextAccessor;
            this._benchmarkSDKMethodControllerLogic = benchmarkSDKMethodControllerLogic;
        }


        [HttpPost]
        [Route(Constants.ApiRoutes.MethodBenchmark)]
        public async Task<IActionResult> CreateMethodSDKBenchmark([FromBody] BenchmarkSDKMethod sdkMethod)
        {
            return await this._benchmarkSDKMethodControllerLogic.CreateMethodSDKBenchmark(sdkMethod, this._contextAccessor.HttpContext);
        }
    }
}
