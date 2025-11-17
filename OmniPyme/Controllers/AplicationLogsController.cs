using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OmniPyme.Web.Core.Attributes;
using OmniPyme.Web.DTOs;
using OmniPyme.Web.Services;

namespace OmniPyme.Web.Controllers
{
    public class AplicationLogsController : Controller
    {
        private readonly IReadLogsService _readLogsService;

        public AplicationLogsController(IReadLogsService readLogsService)
        {
            _readLogsService = readLogsService;
        }

        [HttpGet]
        [CustomAuthorize(permission: "ShowLogs", module: "Logs")]
        [Authorize]
        public IActionResult Index(DateTime? date)
        {
            LogViewerDTO dto = new LogViewerDTO
            {
                Logs = _readLogsService.GetLogs(date),
                SelectedDate = date
            };
            return View(dto);
        }
    }
}
