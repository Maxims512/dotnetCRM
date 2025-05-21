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

    public IActionResult Index()
    {
        var orders = _context.Orders.Include(o => o.User).ToList();
        return View("~/Views/Shared/Orders/OrderList.cshtml", orders);
    }

    [HttpGet]
    public IActionResult Create()
    {
        var users = _context.Users
            .Select(u => new { u.Id, u.FullName })
            .ToList();
        
        if (users == null || !users.Any())
        {
            ViewBag.NoUsers = true;
        }
        else
        {
            ViewBag.Users = new SelectList(users, "Id", "FullName");
        }
        
        return View("~/Views/Shared/Orders/Create.cshtml");
    }

    [HttpPost]
    public async Task<IActionResult> Create(Order order)
    {
        if (ModelState.IsValid)
        {
            try
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Заказ успешно создан!";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Ошибка при создании заказа: {ex.Message}");
            }
        }

        var users = _context.Users
            .Select(u => new { u.Id, u.FullName })
            .ToList();
        
        ViewBag.Users = users != null ? new SelectList(users, "Id", "FullName") : null;
        
        return View(order);
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

    public async Task<IActionResult> Delete(int id)
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