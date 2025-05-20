using Microsoft.AspNetCore.Mvc;
using MvcApp.Data;
using Microsoft.EntityFrameworkCore;


public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var model = new HomeIndexViewModel
        {
            Users = await _context.Users.ToListAsync(),
            Orders = await _context.Orders.Include(o => o.User).ToListAsync()
        };

        return View(model);
    }

}