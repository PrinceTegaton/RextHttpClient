using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RextAPISimulator.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        [HttpPut]
        public IActionResult Put(User user)
        {
            user.EmailAddress = "solkjaer@manutd.com";

            return Ok(user);
            return Content($"{user.Name} has been updated ok");
        }

        [HttpPost]
        public IActionResult Post(User user)
        {
            user.EmailAddress = "user@email.com";

            return Content(user.Name);
            return BadRequest(user);
            return Ok(user);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var res = new { username = "james", status = "deleted" };

            return Content("deleted ok");
            return BadRequest(res);
            return StatusCode(StatusCodes.Status202Accepted, res);
            return Ok(res);
        }
    }

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string EmailAddress { get; set; }
    }
}
