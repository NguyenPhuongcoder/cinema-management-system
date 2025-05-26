using System;
using System.ComponentModel.DataAnnotations;
using QUANLYRAPCHIEUPHHIM.Models;

namespace QUANLYRAPCHIEUPHHIM.ViewModels
{
    public class DiscountViewModel
    {
        public int DiscountId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên khuyến mãi")]
        [Display(Name = "Tên khuyến mãi")]
        public string DiscountName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập giá trị khuyến mãi")]
        [Range(0, 100, ErrorMessage = "Giá trị khuyến mãi phải từ 0 đến 100")]
        [Display(Name = "Giá trị khuyến mãi (%)")]
        public decimal DiscountValue { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày bắt đầu")]
        [Display(Name = "Ngày bắt đầu")]
        public DateOnly StartDate { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày kết thúc")]
        [Display(Name = "Ngày kết thúc")]
        public DateOnly EndDate { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mã giảm giá")]
        [StringLength(20, ErrorMessage = "Mã giảm giá không được vượt quá 20 ký tự")]
        [Display(Name = "Mã giảm giá")]
        public string CouponCode { get; set; }

        [Display(Name = "Trạng thái")]
        public bool IsActive { get; set; }

        [Display(Name = "Giới hạn sử dụng")]
        public int? UsageLimit { get; set; }

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
} 