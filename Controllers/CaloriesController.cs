using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace caltrack.Data;

[ApiController]
[Route("calories")]
public class CaloriesController : Controller {
    private readonly CaltrackContext _db;

    public CaloriesController(CaltrackContext db) =>
        _db = db;
    
    // This method/route shouldn't be used since it concers infomration security practicies.
    // That is because this route essentially exposes all data in the calories table. 
    [HttpGet]
    public async Task<ActionResult<List<CaloriesModel>>> GetAllCalories() {
        return await _db.Calories.ToListAsync();
    }

    // Same as the method/route above can be said about this, although you need an ID here.
    // As long as you understand the ID pattern/data type you can query anything you want in the calories table.
    [HttpGet("{CaloriesId:int}")]
    public async Task<ActionResult<CaloriesModel>> GetCaloriesItem(int CaloriesId) {
        var calorie = await _db.Calories.FindAsync(CaloriesId);
        if(calorie == null) {
            return NotFound();
        }
        return calorie;
    }

    [HttpGet("byUser/{UserId:int}")]
    public async Task<ActionResult<List<CaloriesModel>>> GetCaloriesItemByUserID(int userId) {
        return await _db.Calories.Where(c => c.UserId == userId).ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<CaloriesModel>> AddCalories(CaloriesModel calories) {
        _db.Calories.Add(calories);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetCaloriesItem), new {CaloriesId = calories.CaloriesId}, calories);
    }

    [HttpPut("{CaloriesId:int}")]
    public async Task<ActionResult<CaloriesModel>> UpdateCalories(CaloriesModel calories, int caloriesId) {
        var updateCalories = await _db.Calories.FindAsync(caloriesId);
        if(updateCalories == null) {
            return NotFound();
        }

        updateCalories.Calories = calories.Calories;
        await _db.SaveChangesAsync();

        return updateCalories;
    }

    [HttpDelete("{CaloriesId:int}")]
    public async Task<ActionResult<CaloriesModel>> DeleteCalories(int caloriesId) {
        var calories = await _db.Calories.FindAsync(caloriesId);
        if(calories == null) {
            return NotFound();
        }

        _db.Calories.Remove(calories);
        await _db.SaveChangesAsync();

        return calories;
    }
}