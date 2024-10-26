using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SwimmingSchool_MVC.Models;

public partial class Event
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Поле не може бути порожнім")]
    [Display(Name = "Назва події")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Поле не може бути порожнім")]
    [Display(Name = "Дата проведення")]
    public DateTime Date { get; set; }

    [Required(ErrorMessage = "Поле не може бути порожнім")]
    [Display(Name = "Максимальна кількість учасників")]
    [RegularExpression(@"^[0-9]*$", ErrorMessage = "Максимальна кількість учасників має бути додатнім числом")]
    public short MaxPupilsAmount { get; set; }

    [Required(ErrorMessage = "Поле не може бути порожнім")]
    [Display(Name = "Адреса проведення")]
    public string Locations { get; set; } = null!;

    [Display(Name = "Опис")]
    public string? Description { get; set; }

    [Display(Name = "Розрорядження")]
    public string? Decree { get; set; }

    [Display(Name = "Логотип")]
    [JsonIgnore]
    public byte[]? Logo { get; set; }

    [Display(Name = "Чи проведений")]
    public bool IsHeld { get; set; } = false;

    [JsonIgnore]
    public virtual ICollection<PupilsEvent> PupilsEvents { get; set; } = new List<PupilsEvent>();
}