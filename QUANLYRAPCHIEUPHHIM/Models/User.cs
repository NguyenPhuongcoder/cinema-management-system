using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QUANLYRAPCHIEUPHHIM.Models;

public partial class User
{
    public int UserId { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Tên đăng nhập phải từ {2} đến {1} ký tự")]
    [RegularExpression(@"^[a-zA-Z0-9_]*$", ErrorMessage = "Tên đăng nhập chỉ được chứa chữ cái, số và dấu gạch dưới")]
    [Display(Name = "Tên đăng nhập")]
    public string Username { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng nhập email")]
    [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
    [Display(Name = "Email")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải từ {2} đến {1} ký tự")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$",
        ErrorMessage = "Mật khẩu phải chứa ít nhất 1 chữ hoa, 1 chữ thường, 1 số và 1 ký tự đặc biệt")]
    [DataType(DataType.Password)]
    [Display(Name = "Mật khẩu")]
    public string? Password { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập họ tên")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Họ tên phải từ {2} đến {1} ký tự")]
    [RegularExpression(@"^[a-zA-ZÀ-ỹ\s]*$", ErrorMessage = "Họ tên chỉ được chứa chữ cái và khoảng trắng")]
    [Display(Name = "Họ và tên")]
    public string FullName { get; set; } = null!;

    [Phone(ErrorMessage = "Số điện thoại không đúng định dạng")]
    [RegularExpression(@"^(0[1-9]|84[1-9])([0-9]{8})$", ErrorMessage = "Số điện thoại không hợp lệ")]
    [Display(Name = "Số điện thoại")]
    public string? Phone { get; set; }

    public DateTime RegistrationDate { get; set; }

    [Display(Name = "Ngày tạo")]
    public DateTime? CreatedAt { get; set; }

    [Display(Name = "Ngày cập nhật")]
    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
