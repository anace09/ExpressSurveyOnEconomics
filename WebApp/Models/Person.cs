using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class Person
    {

        public int Id { get; set; }

        [Required(ErrorMessage = "ФИО обязательно")] // todo Поменять валидацию.
        [StringLength(100, MinimumLength = 2, ErrorMessage = "ФИО от 2 до 100 символов")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "ФИО обязательно")] // todo Поменять валидацию.
        [StringLength(100, MinimumLength = 2, ErrorMessage = "ФИО от 2 до 100 символов")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "ФИО обязательно")]  // todo Поменять валидацию.
        [StringLength(100, MinimumLength = 2, ErrorMessage = "ФИО от 2 до 100 символов")]
        public string MiddleName { get; set; }

        [Required(ErrorMessage = "Организация обязательна")]// todo Поменять валидацию.
        [StringLength(200, ErrorMessage = "Организация до 200 символов")]
        public string Organization { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
