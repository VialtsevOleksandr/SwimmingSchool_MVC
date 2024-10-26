using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SwimmingSchool_MVC.Models;

public partial class Trainer
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Поле не може бути порожнім")]
    [Display(Name = "Прізвище")]
    [RegularExpression(@"^[а-яА-Я'їЇіІєЄ]*$", ErrorMessage = "Поле може містити тільки літери та апострофи")]
    public string LastName { get; set; } = null!;

    [Required(ErrorMessage = "Поле не може бути порожнім")]
    [Display(Name = "Ім'я")]
    [RegularExpression(@"^[а-яА-Я'їЇіІєЄ]*$", ErrorMessage = "Поле може містити тільки літери та апострофи")]
    public string FirstName { get; set; } = null!;

    [Display(Name = "По батькові")]
    [RegularExpression(@"^[а-яА-Я'їЇіІєЄ]*$", ErrorMessage = "Поле може містити тільки літери та апострофи")]
    public string? MiddleName { get; set; }

    [Required(ErrorMessage = "Поле не може бути порожнім")]
    [Display(Name = "Дата народження")]
    //[RegularExpression(@"^\d{2}.\d{2}.\d{4}$", ErrorMessage = "Дата народження має бути у форматі DD.MM.YYYY")]
    public DateOnly Birthday { get; set; }

    [Required(ErrorMessage = "Поле не може бути порожнім")]
    [Display(Name = "Номер телефону")]
    [RegularExpression(@"^\+380\d{9}$", ErrorMessage = "Номер телефону має бути у форматі +380XXXXXXXXX")]
    public string PhoneNumber { get; set; } = null!;

    [Required(ErrorMessage = "Поле не може бути порожнім")]
    [Display(Name = "Email")]
    [EmailAddress(ErrorMessage = "Це поле має бути у форматі email")]
    public string Email { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<Group> Groups { get; set; } = new List<Group>();
}
