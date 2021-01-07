using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Redis.Common.Dto;
using Redis.Master.Application;

namespace Redis.Master.Controllers
{
    //TODO add authentication (at least some token in header)
    //TODO add replication (for availability)
    //TODO add rebalancing of the partitions
    //TODO add load test
    [ApiController]
    public class MasterController : ControllerBase
    {
        private readonly IMasterService _masterService;

        public MasterController(IMasterService masterService)
        {
            _masterService = masterService;
        }

        //for monitoring purposes
        [HttpGet("master/partitions")]
        public async Task<ActionResult<List<ChildEntriesDto>>> GetAllPartitionsAsync(CancellationToken cancellationToken)
        {
            var entries = await _masterService.GetAllEntriesAsync(cancellationToken);
            return Ok(entries);
        }

        [HttpGet("master")]
        public async Task<ActionResult<string>> Get([FromQuery] string key, CancellationToken cancellationToken)
        {
            var val = await _masterService.GetAsync(key, cancellationToken);
            return Ok(val);
        }

        [HttpPost("master")]
        public async Task<IActionResult> Add([FromQuery] string key, [FromBody] string value, CancellationToken cancellationToken)
        {
            await _masterService.AddAsync(key, value, cancellationToken);
            return Ok();
        }
    }
}
