using System;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.Web.Models.Users;


namespace UserManagement.WebMS.Controllers
{
    [Route("logs")]
    public class LogsController : Controller
    {
        private readonly ILogService _logService;

        public LogsController(ILogService logService)
        {
            _logService = logService;
        }

        [HttpGet]
        public async Task<IActionResult> List(string searchString)
        {
            var logs  = await _logService.GetAllLogsAsync();

            // Filter logs based on search string
            if (!string.IsNullOrEmpty(searchString))
            {
                logs = logs.Where(log => 
                    log.Action.ToLower().Contains(searchString.ToLower()) ||
                    (log.Source?.ToLower().Contains(searchString.ToLower()) ?? false)
                );
            }

            var logViewModels = logs.Select(log => new LogItemViewModel
            {
                Id = log.Id,
                UserId = log.UserId,
                Action = log.Action, // Ensure the 'action' property is included
                Timestamp = log.Timestamp,
                Source = log.Source,
                Payload = log.Payload
            }).ToList();

            // return View("List", logViewModels);
            return Ok(logViewModels);
        }

        [HttpGet("details/{id}")]
        public async Task<IActionResult> DetailsAsync(long id)
        {
            Console.WriteLine("got" + id);
            var logtask = await _logService.GetLogByIdAsync(id);
            var log= logtask.FirstOrDefault();

            if (log == null)
            {
                return NotFound();
            }

            var model = new LogItemViewModel
            {
                Id = log.Id,
                UserId = log.UserId,
                Action = log.Action,
                Timestamp = log.Timestamp,
                Source = log.Source,
                Payload = log.Payload
            };

            // return View(model);
            return Ok(model);
        }
    }
}
