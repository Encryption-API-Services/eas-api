using API.ControllerLogic;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UIDataController : ControllerBase
    {
        private readonly IUIDataControllerLogic _logic;
        public UIDataController(IUIDataControllerLogic logic)
        {
            this._logic = logic;
        }

        [HttpGet]
        [Route("GetHomeBenchmarkData")]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any, NoStore = false)]
        public async Task<IActionResult> GetHomePageBenchMarkData()
        {
            return await this._logic.GetHomePageBenchMarkData(HttpContext);
        }
    }
}
