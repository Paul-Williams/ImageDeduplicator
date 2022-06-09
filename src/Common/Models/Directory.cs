# nullable enable

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models
{
  [Table("Directories")]
  public class Directory
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Required(ErrorMessage = "Id is required")]
    [Display(Name = "Id")]
    public int Id { get; set; }

    [MaxLength(300)]
    [StringLength(300)]
    [Required(ErrorMessage = "Path is required")]
    [Display(Name = "Path")]
    public string Path { get; set; } = string.Empty;
  }
}
