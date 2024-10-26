using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SwimmingSchool_MVC.Models;

public partial class Day
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Поле не може бути порожнім")]
    [Display(Name = "День тижня")]
    [RegularExpression(@"^[а-яА-Я'їЇіІєЄ]*$", ErrorMessage = "Поле може містити тільки літери та апострофи")]

    public string NameOfDay { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<GroupSchedule> GroupSchedules { get; set; } = new List<GroupSchedule>();
}
