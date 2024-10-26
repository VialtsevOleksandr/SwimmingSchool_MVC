using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SwimmingSchool_MVC.Models;

public partial class GroupSchedule
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Поле не може бути порожнім")]
    [Display(Name = "Назва групи")]
    public int GroupId { get; set; }

    [Required(ErrorMessage = "Поле не може бути порожнім")]
    [Display(Name = "День тижня")]
    public int DayId { get; set; }

    [Required(ErrorMessage = "Поле не може бути порожнім")]
    [Display(Name = "Час початку тренування")]
    public TimeOnly TimeOfTrainingStart { get; set; }

    [Required(ErrorMessage = "Поле не може бути порожнім")]
    [Display(Name = "Тривалість тренування(хв)")]
    [RegularExpression(@"^[0-9]*$", ErrorMessage = "Тривалість тренування має бути додатнім числом")]
    public short TrainingTime { get; set; }

    [JsonIgnore]
    public virtual Day? Day { get; set; }

    [JsonIgnore]
    public virtual Group? Group { get; set; }
}
