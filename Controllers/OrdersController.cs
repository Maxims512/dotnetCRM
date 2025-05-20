using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcApp.Data;
using MvcApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

public class OrdersController : Controller
{
    private readonly ApplicationDbContext _context;

    public OrdersController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var orders = await _context.Orders.Include(o => o.User).ToListAsync();
        return View("~/Views/Shared/Orders/Index.cshtml", orders);
    }

    public IActionResult Create()
    {
        ViewBag.Users = new SelectList(_context.Users.ToList(), "Id", "FullName");
        return View("~/Views/Shared/Orders/Create.cshtml");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Price,OrderDate,UserId")] Order order)
    {
        if (ModelState.IsValid)
        {
            _context.Add(order);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }
        
        ViewBag.Users = new SelectList(_context.Users.ToList(), "Id", "FullName", order.UserId);
        return View("~/Views/Shared/Orders/Create.cshtml", order);
    }

    
    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        
        var order = await _context.Orders.FindAsync(id);
        if (order == null) return NotFound();
        
        var users = await _context.Users
            .Select(u => new { u.Id, u.FullName })
            .ToListAsync();
        
        if (users == null || !users.Any())
        {
            TempData["ErrorMessage"] = "Нет доступных пользователей. Сначала создайте пользователя.";
            return RedirectToAction("Index", "Home");
        }
        
        ViewBag.Users = new SelectList(users, "Id", "FullName", order.UserId);
        return View("~/Views/Shared/Orders/Edit.cshtml", order);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Order order)
    {
        if (id != order.Id) return NotFound();
        
        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(order);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(order.Id)) return NotFound();
                throw;
            }
        }

        var users = await _context.Users
            .Select(u => new { u.Id, u.FullName })
            .ToListAsync();
        
        ViewBag.Users = users != null ? new SelectList(users, "Id", "FullName", order.UserId) : null;
        return View("~/Views/Shared/Orders/Edit.cshtml", order);
    }

    
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var order = await _context.Orders
            .Include(o => o.User)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (order == null) return NotFound();

        return View("~/Views/Shared/Orders/Delete.cshtml", order);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order != null)
        {
            _context.Orders.Remove(order);
        }
        
        await _context.SaveChangesAsync();
        return RedirectToAction("Index", "Home");
    }

    private bool OrderExists(int id)
    {
        return _context.Orders.Any(e => e.Id == id);
    }
}