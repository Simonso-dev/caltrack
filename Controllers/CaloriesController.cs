using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace caltrack.Data;

[ApiController]
[Route("api/calories")]
[Authorize(Roles = "Administrator, User")]
public class CaloriesController : Controller {
    private readonly CaltrackContext _db;

    public CaloriesController(CaltrackContext db) =>
        _db = db;
    
    [HttpGet]
    public async Task<ActionResult<List<CaloriesModel>>> GetAllCalories() =>
        await _db.Calories.ToListAsync();

    [HttpGet("{CaloriesId:int}")]
    public async Task<ActionResult<CaloriesModel>> GetCaloriesItem(int CaloriesId) {
        var calorie = await _db.Calories.FindAsync(CaloriesId);

        if(calorie is null) 
            return NotFound();
        
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
        return Ok(calories);
    }

    [HttpPut("{CaloriesId:int}")]
    public async Task<ActionResult<CaloriesModel>> UpdateCalories(CaloriesModel calories, int caloriesId) {
        var updateCalories = await _db.Calories.FindAsync(caloriesId);

        if(updateCalories is null)
            return NotFound();

        updateCalories.Calories = calories.Calories;
        await _db.SaveChangesAsync();

        return updateCalories;
    }

    [HttpDelete("{CaloriesId:int}")]
    public async Task<ActionResult<CaloriesModel>> DeleteCalories(int caloriesId) {
        var calories = await _db.Calories.FindAsync(caloriesId);

        if(calories is null) 
            return NotFound();

        _db.Calories.Remove(calories);
        await _db.SaveChangesAsync();

        return calories;
    }
}