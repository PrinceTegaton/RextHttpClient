using Microsoft.AspNetCore.Mvc;

namespace Rext.ApiSimulator.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AppController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetByQuery(int id)
        {
            var u = AppDataset.Users.FirstOrDefault(a => a.Id == id);
            if (u == null)
            {
                return BadRequest(new Result("User not found"));
            }

            return Ok(new Result<User>(u, "User found"));
        }

        [HttpGet("{id}")]
        public IActionResult GetByPath(int id)
        {
            var u = AppDataset.Users.FirstOrDefault(a => a.Id == id);
            if (u == null)
            {
                return BadRequest(new Result("User not found"));
            }

            return Ok(new Result<User>(u, "User found"));
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(new Result<IEnumerable<User>>(AppDataset.Users, "Users listed"));
        }

        [HttpGet]
        public IActionResult GetAllWithQuery(string filter_name, bool withWrapper)
        {
            var res = AppDataset.Users.Where(a => a.Name.Contains(filter_name, StringComparison.InvariantCultureIgnoreCase));

            if (withWrapper)
                return Ok(new Result<IEnumerable<User>>(res, "Users listed"));
            else
                return Ok(res);
        }

        [HttpPost]
        public IActionResult Create(User user)
        {
            if (user == null || string.IsNullOrEmpty(user.Name))
            {
                return BadRequest(new Result("Payload is required with complete Name field."));
            }

            if (AppDataset.Users.Any(a => a.Name.Contains(user.Name, StringComparison.InvariantCultureIgnoreCase)))
            {
                return BadRequest(new Result("User with same name already exist"));
            }

            user.Id = AppDataset.Users.Max(a => a.Id) + 1;
            AppDataset.Users.Add(user);

            return Ok(new Result<int>(user.Id, "User created"));
        }

        [HttpPut]
        public IActionResult Update(User user)
        {
            if (user == null || user.Id <= 0)
            {
                return BadRequest(new Result("Payload is required"));
            }

            var u = AppDataset.Users.FirstOrDefault(a => a.Id == user.Id);
            if (u == null)
            {
                return BadRequest(new Result("User not found"));
            }

            AppDataset.Users.Remove(u);
            AppDataset.Users.Add(user);

            return Ok(new Result<User>(user, "User updated"));
        }

        [HttpPatch]
        public IActionResult UpdatePartial(int id, string newEmail)
        {
            if (id <= 0 || string.IsNullOrEmpty(newEmail))
            {
                return BadRequest(new Result("Id and new email is required"));
            }

            var u = AppDataset.Users.FirstOrDefault(a => a.Id == id);
            if (u == null)
            {
                return BadRequest(new Result("User not found"));
            }

            u.Contact = new Contact
            {
                EmailAddress = newEmail,
                MobileNumber = u.Contact?.MobileNumber
            };

            return Ok(new Result<User>(u, "User updated"));
        }

        [HttpDelete]
        public IActionResult Delete(int id, string note)
        {
            if (string.IsNullOrEmpty(note))
            {
                return BadRequest(new Result<bool>(false, "Note is required"));
            }

            var u = AppDataset.Users.FirstOrDefault(a => a.Id == id);
            if (u == null)
            {
                return BadRequest(new Result<bool>(false, "User not found"));
            }

            AppDataset.Users.Remove(u);

            return Ok(new Result<bool>(true, "User deleted"));
        }

        [HttpGet]
        public IActionResult HandledError()
        {
            return StatusCode(500, "This is an unhandled exception");
        }

        [HttpGet]
        public IActionResult UnhandledError()
        {
            throw new Exception("This is an unhandled exception");
        }

        [HttpGet]
        public IActionResult GetByHeaderInput([FromHeader]int id, [FromHeader]string fancyHeader)
        {
            if (id <= 0 || string.IsNullOrEmpty(fancyHeader))
            {
                return BadRequest(new Result("Id and FancyHeader are required"));
            }

            var u = AppDataset.Users.FirstOrDefault(a => a.Id == id);
            if (u == null)
            {
                return BadRequest(new Result("User not found"));
            }

            return Ok(new Result<User>(u, fancyHeader));
        }

        [HttpGet]
        public IActionResult GetByHeaderOutput(int id, string fancyHeader)
        {
            if (id <= 0 || string.IsNullOrEmpty(fancyHeader))
            {
                return BadRequest(new Result("Id and FancyHeader are required"));
            }

            Response.Headers.TryAdd("r-id", id.ToString());
            Response.Headers.TryAdd("r-fancyHeader", fancyHeader);

            return Ok(new Result("Response written to header"));
        }
    }
}