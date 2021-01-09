using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Redis.Common.Abstractions;
using Redis.Common.Dto;
using Redis.Master.Application;

namespace Redis.Master.Controllers
{
    //TODO add authentication (at least some token in header)
    //TODO add replication (for availability)
    //TODO add rebalancing of the partitions
    //TODO add load test
    [ApiController]
    [Route("master")]
    public class MasterController : ControllerBase
    {
        private readonly IMasterService _masterService;
        private readonly IReplicationService _replicationService;
        private readonly MasterOptions _options;

        public MasterController(IMasterService masterService,
            IReplicationService replicationService,
            IOptions<MasterOptions> options)
        {
            _masterService = masterService;
            _replicationService = replicationService;
            _options = options.Value;
        }

        //for monitoring purposes
        [HttpGet("partitions")]
        public async Task<ActionResult<List<ChildEntriesDto>>> GetAllPartitionsAsync(CancellationToken cancellationToken)
        {
            var entries = await _masterService.GetAllEntriesAsync(cancellationToken);
            return Ok(entries);
        }

        [HttpGet]
        public async Task<ActionResult<string>> Get([FromQuery] string key, CancellationToken cancellationToken)
        {
            var val = await _masterService.GetAsync(key, cancellationToken);
            return Ok(val);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromQuery] string key, [FromBody] string value, CancellationToken cancellationToken)
        {
            await _masterService.AddAsync(key, value, cancellationToken);

            //we don't await because we're using asynchronous type of replication because of performance considerations.
            _replicationService.ReplicateToSlavesAsync(key, value, cancellationToken);

            return Ok();
        }

        [HttpPost("replication")]
        public async Task<IActionResult> ReceiveReplicationRequestAsync([FromQuery] string key, [FromBody] string value, CancellationToken cancellationToken)
        {
            if (_options.ReplicationActor == ReplicationActor.Master.ActorName)
                throw new Exception("Master cannot receive synchronization request from another master, since we are using master-slave replication");

            await _masterService.AddAsync(key, value, cancellationToken);
            return Ok();
        }
    }
}
