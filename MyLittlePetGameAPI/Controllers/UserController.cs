using Microsoft.AspNetCore.Mvc;
using MyLittlePetGameAPI.Models;

namespace MyLittlePetGameAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {


        public readonly AppDbContext _context;
        public UserController(AppDbContext context)
        {
            _context = context;
        }
        //get all users from the database
        [HttpGet]
        public ActionResult<IEnumerable<User>> Get()
        {
            return Ok(_context.Users.ToList());
        }
        //get a user by id from the database
        [HttpGet("{id}")]
        public ActionResult<User> GetById(int id)
        {
            var user = _context.Users.Find(id);
            // If user is not found, return NotFound; 404 status code
            if (user == null)
            {
                return NotFound();
            }
            // Return the user if found
            return Ok(user);
        }

        //get a user by password and username from the database
        [HttpGet("login")]
        public ActionResult<User> GetByLogin(String UserName, String Password)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserName == UserName && u.Password == Password);
            // If user is not found, return NotFound; 404 status code
            if (user == null)
            {
                return NotFound();
            }
            // Return the user if found
            return Ok(user);
        }
        // Add a new user to the database
        [HttpPost]
        public ActionResult<User> Create(String Role, String UserName, String Password,String Email, String UserStatus, int Level, int Coin, int Diamond, int Gem)

        {
            User newUser = new User();
            newUser.Role = Role;
            newUser.UserName = UserName;
            newUser.Password = Password;
            newUser.Email = Email;
            newUser.UserStatus = UserStatus;
            newUser.Level = Level;
            newUser.Coin = Coin;
            newUser.Diamond = Diamond;
            newUser.Gem = Gem;
            newUser.JoinDate = DateTime.Now; // Set JoinDate to current date and time
            _context.Users.Add(newUser);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetById), new { id = newUser.Id}, newUser);   
        }
        //Update an existing user in the database
        [HttpPut("{id}")]
        public ActionResult<User> Update(int id, String Role, String UserName, String Password, String Email, int Level, int Coin, int Diamond, int Gem)
        {
            var user = _context.Users.Find(id);
            // If user is not found, return NotFound; 404 status code
            if (user == null)
            {
                return NotFound();
            }
            // Update the user properties
            user.Role = Role;
            user.UserName = UserName;
            user.Password = Password;
            user.Email = Email;
            user.Level = Level;
            user.Coin = Coin;
            user.Diamond = Diamond;
            user.Gem = Gem;
            _context.Users.Update(user);
            _context.SaveChanges();
            return Ok(user);
        }
    }
}
