using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcApp.Data;
using MvcApp.Models;

public class UsersController : Controller
{
    private readonly ApplicationDbContext _context;

    public UsersController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        return View("~/Views/Shared/Users/UserList.cshtml",
               await _context.Users.ToListAsync());
        
    }


    public IActionResult Create()
    {
        return View("~/Views/Shared/Users/Create.cshtml");
    }

    [HttpPost]
    public async Task<IActionResult> Create([Bind("FullName,BirthDate")] User user)
    {
        if (ModelState.IsValid)
        {
            _context.Add(user);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }
        return View("~/Views/Shared/Users/Create.cshtml", user);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        
        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound();

        return View("~/Views/Shared/Users/Edit.cshtml", user);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, [Bind("Id,FullName,BirthDate")] User user)
    {
        if (id != user.Id) return NotFound();
        user.BirthDate = user.BirthDate.Date;
        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(user);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(user.Id)) return NotFound();
                throw;
            }
            var savedUser = await _context.Users.FindAsync(id);
            return RedirectToAction("Index", "Home");
        }
        return View("~/Views/Shared/Users/Edit.cshtml", user);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user != null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Index", "Home");
    }

    private bool UserExists(int id)
    {
        return _context.Users.Any(e => e.Id == id);
    }
}