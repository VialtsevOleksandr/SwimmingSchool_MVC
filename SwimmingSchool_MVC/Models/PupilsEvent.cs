using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SwimmingSchool_MVC.Models;

public partial class PupilsEvent
{
    [Required(ErrorMessage = "Поле не може бути порожнім")]
    [Display(Name = "Учень")]
    public int PupilsId { get; set; }

    [Required(ErrorMessage = "Поле не може бути порожнім")]
    [Display(Name = "Назва події")]
    public int EventId { get; set; }

    [Display(Name = "Інформація")]
    public string? Info { get; set; }

    [Display(Name = "Результат")]
    [RegularExpression(@"^([0-5]?[0-9]):([0-5]?[0-9]):([0-9]{1,3})$", ErrorMessage = "Невірний формат часу. Введіть час у форматі хвилини:секунди:мілісекунди")]
    public TimeSpan? Result { get; set; }

    [JsonIgnore]
    public virtual Event? Event { get; set; }

    [JsonIgnore]
    public virtual Pupil? Pupils { get; set; }
}
