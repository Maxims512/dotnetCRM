using MvcApp.Models;

public class HomeIndexViewModel
{
    public IEnumerable<User> Users { get; set; } = new List<User>();
    public IEnumerable<Order> Orders { get; set; } = new List<Order>();
    
}