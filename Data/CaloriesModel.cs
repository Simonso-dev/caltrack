using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;


[PrimaryKey(nameof(CaloriesId))]
public class CaloriesModel {

    public int CaloriesId { get; set; }
    public int Calories { get; set; }
    public DateTime Date { get; set; }

    // Foreign key
    public int UserId { get; set; }
    // Inverse navigation property
    // It refers to the navigation property in the other end of the relationship
    // For some reason I have to make use of the "?" operator to make the user object nullable
    // becasue otherwise I get an error message in postman saying that "The User field is required"
    // when I try to create an entry in the calories table. 
    public User? User { get; set; } = null!;
}