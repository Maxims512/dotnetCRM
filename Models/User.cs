using System.ComponentModel.DataAnnotations;

namespace MvcApp.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "ФИО")]
        public string? FullName { get; set; }

        private DateTime _birthDate;

        [Required]
        [Display(Name = "Дата рождения")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime BirthDate
        {
            get => _birthDate.Date;  // Всегда возвращаем только дату
            set => _birthDate = value.Date;  // Обрезаем время
        }

        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}