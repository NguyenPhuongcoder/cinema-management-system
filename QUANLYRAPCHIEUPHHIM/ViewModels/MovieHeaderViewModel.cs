// File: ViewModels/HeaderViewModel.cs
using System.ComponentModel.DataAnnotations;

public class MovieHeaderViewModel
{
    public int MovieId { get; set; }

    [Required(ErrorMessage = "Tên phim không được để trống")]
    [StringLength(200, ErrorMessage = "Tên phim không được vượt quá 200 ký tự")]
    public string Title { get; set; }

    [Required(ErrorMessage = "Poster không được để trống")]
    [StringLength(300, ErrorMessage = "URL poster không được vượt quá 300 ký tự")]
    public string PosterUrl { get; set; }

    [StringLength(10, ErrorMessage = "Giới hạn tuổi không hợp lệ")]
    public string AgeLimit { get; set; }

    [Range(0, 10, ErrorMessage = "Đánh giá phải từ 0 đến 10")]
    public double Rating { get; set; }
}
