using System.ComponentModel.DataAnnotations;

namespace MvcApp.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "ФИО")]
        public string? FullName { get; set; } 

        [Required]
        [Display(Name = "Дата рождения")]
        [DataType(DataType.Date)]
        private DateTime _birthDate;

        public DateTime BirthDate
        {
            get => _birthDate;
            set => _birthDate = DateTime.SpecifyKind(value, DateTimeKind.Utc);
        }

        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}