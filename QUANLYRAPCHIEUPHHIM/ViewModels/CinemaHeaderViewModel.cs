using System.ComponentModel.DataAnnotations;

public class CinemaHeaderViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Tên rạp không được để trống")]
    [StringLength(200, ErrorMessage = "Tên rạp không được vượt quá 200 ký tự")]
    public string Name { get; set; }
}