using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Job.Entity;

public record UserModel
{
    [Display(Name = "ユーザーID")]
    [Required(ErrorMessage = "入力必須です。")]
    public string Id { get; set; } = string.Empty;

    [Display(Name = "ユーザー名")]
    [Required(ErrorMessage = "入力必須です。")]
    [MaxLength(20)]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "性別")]
    public Sex SexId { get; set; }
}

public enum Sex
{
    None,
    Male,
    Female,
}

public record NullUserModel : UserModel
{

    public static UserModel Instance { get; } = new UserModel()
    {
        Id = "存在しないId",
        Name = "存在しない名前",
        SexId = Sex.None
    };

    protected NullUserModel()
    {
    }
}
