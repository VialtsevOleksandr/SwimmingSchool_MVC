using NuGet.DependencyResolver;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SwimmingSchool_MVC.Models;

public class GroupType
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Поле не може бути порожнім")]
    [Display(Name = "Тип групи")]
    [RegularExpression(@"^[а-яА-Я'їЇіІєЄ]*$", ErrorMessage = "Поле може містити тільки літери та апострофи")]
    public string Name { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<Group> Groups { get; set; } = new List<Group>();
}
