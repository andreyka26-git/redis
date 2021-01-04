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

        [HttpGet("partition")]
        public ActionResult<string> Get([FromQuery] string key, [FromQuery] int hashKey)
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
