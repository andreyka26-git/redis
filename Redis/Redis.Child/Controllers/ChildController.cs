using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Redis.Child.Application;
using Redis.Common.Dto;

namespace Redis.Child.Controllers
{
    [ApiController]
    public class ChildController : ControllerBase
    {
        private readonly IPartition _partition;

        public ChildController(IPartition partition)
        {
            _partition = partition;
        }

        [HttpGet("partition/entries")]
        public IActionResult GetEntries()
        {
            var entries = _partition
                .GetEntries()
                .Select(b =>
                    new BucketDto(b.hashKey, b.entries.Select(e => new EntryDto(e.HashCode, e.Key, e.Value))));

            return Ok(entries);
        }

        [HttpGet("partition")]
        public ActionResult<string> Get([FromQuery] string key, [FromQuery] uint hashKey)
        {
            var val = _partition.Get(key, hashKey);
            return Ok(val);
        }

        [HttpPost("partition")]
        public IActionResult Add([FromBody] EntryDto entry)
        {
            _partition.Add(entry.Key, entry.HashKey, entry.Value);
            return Ok();
        }
    }
}
