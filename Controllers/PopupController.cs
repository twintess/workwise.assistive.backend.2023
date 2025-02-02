using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace workwise.assistive.backend.Controllers
{
    [Authorize(Policy = "popupReaderPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class PopupController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly PopupService _popupService;

        public PopupController(ILogger<UserService> logger, PopupService popupService)
        {
            _logger = logger;
            _popupService = popupService;
        }

        // GET api/<PopupController>/5
        [HttpGet("get-schedule")]
        public IEnumerable<PopupScheduleResponse> GetPopupSchedule(DateTime from, DateTime to)
        {
            var schedule = _popupService.GetPopupSchedule(from, to);
            return schedule;
        }

        [HttpGet("get-popup-list")]
        public ActionResult<IEnumerable<PopupDetailsResponse>> GetPopupList()
        {
            try
        {
                var user = Encoding.UTF8.GetString(this.HttpContext.Session.Get("username"));
                var popupList = _popupService.GetPopupList(user);
                return this.Ok(popupList);
        }
            catch(Exception ex)
        {
                _logger.LogError(ex.ToString());
                return this.BadRequest();
            }
        }

        // DELETE api/<PopupController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
