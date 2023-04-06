using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

[PrimaryKey(nameof(UserId))]
public class User {

    public int UserId { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }

    // Collection navigation property 
    // Collection that references many related entities
    public List<CaloriesModel>? CaloriesList { get; set; } = null!;
}