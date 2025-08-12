
using E_Menu.API.Abstraction;
using E_Menu.Caching.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.PowerPlatform.Dataverse.Client;
using Quartz;

namespace E_Menu.API.Controllers
{

    public class JobsController : ApiController
    {
        private readonly IScheduler _scheduler;
        public JobsController(IOrganizationServiceAsync service, IMediator mediator, ICacheService cacheService,IScheduler scheduler) : base(service, mediator, cacheService)
        {
            _scheduler = scheduler ?? throw new ArgumentNullException(nameof(scheduler));
        }

        [HttpPost("{jobName}/trigger")]
        public async Task<IActionResult> TriggerJob(string jobName)
        {
            var jobKey = new JobKey(jobName);
            if (!await _scheduler.CheckExists(jobKey))
                return NotFound($"Job {jobName} not found.");

            await _scheduler.TriggerJob(jobKey);
            return Ok($"Job {jobName} triggered.");
        }
    }
}
