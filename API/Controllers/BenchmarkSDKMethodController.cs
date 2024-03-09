using API.ControllerLogic;
using Microsoft.AspNetCore.Mvc;
using Models.BenchmarkSDKSend;
using Validation.Attributes;

namespace API.Controllers
{
    [Route("[controller]")]
    [ApiController]
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


        [HttpGet]
        [Route("GetUserBenchmarksByDays")]
        [ValidateJWT]
        public async Task<IActionResult> GetUserBenchmarksByDays([FromQuery] int daysAgo)
        {
            return await this._benchmarkSDKMethodControllerLogic.GetUserBenchmarksByDays(daysAgo, this._contextAccessor.HttpContext);
        }

        [HttpPost]
        [Route("MethodBenchmark")]
        [ValidateJWT]
        public async Task<IActionResult> CreateMethodSDKBenchmark([FromBody] BenchmarkMacAddressSDKMethod benchmark)
        {
            return await this._benchmarkSDKMethodControllerLogic.CreateMethodSDKBenchmark(benchmark, this._contextAccessor.HttpContext);
        }
    }
}
