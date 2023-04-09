using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace caltrack.Data;

[ApiController]
[Route("users")]
public class UserController : Controller {
    private readonly CaltrackContext _db;

    public UserController(CaltrackContext db) =>
        _db = db;
    
    // Read comments in CaloriesController.
    [HttpGet]
    public async Task<ActionResult<List<User>>> GetAllUsers() {
        return await _db.Users.ToListAsync();
    }

    [HttpGet("{UserId:int}")]
    public async Task<ActionResult<User>> GetUser(int UserId) {
        var user = await _db.Users.FindAsync(UserId);
        if(user == null) {
            return NotFound();
        }
        return user;
    }

    [HttpGet("{username}")]
    public async Task<ActionResult<User>> GetUserByUsername(string username) {
        var user = await _db.Users.Where(u => u.Username == username).FirstOrDefaultAsync<User>();
        if(user == null) {
            return NotFound();
        }
        return user;
    }

    [HttpPost]
    public async Task<ActionResult<User>> AddUser(User user) {
        // Hash the password and overwrite user.password with the hash
        string passwordHash = BCrypt.Net.BCrypt.HashPassword(user.Password);
        user.Password = passwordHash;

        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(user), new {UserId = user.UserId}, user);
    }

    [HttpPut("{UserId:int}")]
    public async Task<ActionResult<User>> UpdateUser(User user, int userId) {
        var updateUser = await _db.Users.FindAsync(userId);
        if(updateUser == null) {
            return NotFound();
        }

        updateUser.Username = user.Username;
        updateUser.Role = user.Role;
        await _db.SaveChangesAsync();

        return updateUser;
    }

    [HttpDelete("{UserId:int}")]
    public async Task<ActionResult<User>> DeleteUser(int userId) {
        var user = await _db.Users.FindAsync(userId);
        if(user == null) {
            return NotFound();
        }

        _db.Users.Remove(user);
        await _db.SaveChangesAsync();

        return user;
    }
}