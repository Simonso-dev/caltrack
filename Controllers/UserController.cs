using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace caltrack.Data;

[ApiController]
[Route("api/users")]
[Authorize(Roles = "Administrator")]
public class UserController : Controller {
    private readonly CaltrackContext _db;
    private readonly IConfiguration _configuration;

    public UserController(IConfiguration configuration, CaltrackContext db) {
        _db = db;
       _configuration = configuration;
    }
    
    [HttpGet]
    public async Task<ActionResult<List<User>>> GetAllUsers() =>
        await _db.Users.ToListAsync();

    [HttpGet("{UserId:int}"), Authorize(Roles = "Administrator, User")]
    public async Task<ActionResult<User>> GetUser(int userId) {
        var user = await _db.Users.FindAsync(userId);

        if(user is null) 
            return NotFound();

        return user;
    }

    [HttpGet("{username}"), Authorize(Roles = "Administrator, User")]
    public async Task<ActionResult<User>> GetUserByUsername(string username) {
        var user = await _db.Users.Where(u => u.Username == username).FirstOrDefaultAsync<User>();
        
        if(user is null) 
            return NotFound();

        return user;
    }

    [HttpPost("register"), AllowAnonymous]
    public async Task<ActionResult<User>> AddUser(User user) {
        // Hash the password and overwrite user.password with the hash
        string passwordHash = BCrypt.Net.BCrypt.HashPassword(user.Password);
        user.Password = passwordHash;

        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return Ok(user);
    }

    [HttpPut("{UserId:int}")]
    public async Task<ActionResult<User>> UpdateUser(User user, int userId) {
        var updateUser = await _db.Users.FindAsync(userId);

        if(updateUser is null) 
            return NotFound();

        updateUser.Username = user.Username;
        updateUser.Role = user.Role;
        await _db.SaveChangesAsync();

        return updateUser;
    }

    [HttpDelete("{UserId:int}")]
    public async Task<ActionResult<User>> DeleteUser(int userId) {
        var user = await _db.Users.FindAsync(userId);
        
        if(user is null) 
            return NotFound();

        _db.Users.Remove(user);
        await _db.SaveChangesAsync();

        return user;
    }

    [HttpPost("login"), AllowAnonymous]
    public async Task<ActionResult<User>> Login(User loginUser) {
        var user = await _db.Users.Where(u => u.Username == loginUser.Username).FirstOrDefaultAsync<User>();

        bool hashMatch = BCrypt.Net.BCrypt.Verify(loginUser.Password, user.Password);

        if(user.Username != loginUser.Username || hashMatch == false)
            return BadRequest("Wrong username or password!");

        string token = CreateToken(user);

        return Ok(token);
    }
    
    public string CreateToken(User user) {
        List<Claim> clamis = new List<Claim> {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            _configuration.GetSection("Appsettings:Token").Value!
        ));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var token = new JwtSecurityToken(
            claims: clamis,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: creds
        );

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return jwt;
    }
}