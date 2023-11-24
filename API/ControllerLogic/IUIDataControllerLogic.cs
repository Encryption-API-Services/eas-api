using Microsoft.AspNetCore.Mvc;

namespace API.ControllerLogic
{
    public interface IUIDataControllerLogic
    {
        Task<IActionResult> GetHomePageBenchMarkData(HttpContext httpContext);
    }
}
