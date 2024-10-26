using NuGet.DependencyResolver;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SwimmingSchool_MVC.Models;

public partial class Group
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Поле не може бути порожнім")]
    [Display(Name = "Назва групи")]
    [RegularExpression(@"^[а-яА-Я'їЇіІєЄ]*$", ErrorMessage = "Поле може містити тільки літери та апострофи")]
    public string GroupName { get; set; } = null!;

    [Required(ErrorMessage = "Поле не може бути порожнім")]
    [Display(Name = "ПІБ тренера")]
    public int TrainerId { get; set; }

    [Required(ErrorMessage = "Поле не може бути порожнім")]
    [Display(Name = "Тип групи")]
    public int GroupTypeId { get; set; }

    public virtual ICollection<GroupSchedule> GroupSchedules { get; set; } = new List<GroupSchedule>();

    public virtual ICollection<Pupil> Pupils { get; set; } = new List<Pupil>();

    [JsonIgnore]
    public virtual Trainer? Trainer { get; set; }

    [JsonIgnore]
    public virtual GroupType? GroupType { get; set; }
}