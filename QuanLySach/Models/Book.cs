using System.ComponentModel.DataAnnotations;

namespace QuanLySach.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên sách")]
        [Display(Name = "Tên sách")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập tên tác giả")]
        [Display(Name = "Tác giả")]
        public string Author { get; set; } = string.Empty;

        [Display(Name = "Nhà xuất bản")]
        public string Publisher { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập giá")]
        [Range(0, 100000000, ErrorMessage = "Giá không hợp lệ")]
        [Display(Name = "Giá")]
        public decimal Price { get; set; }

        [Range(1900, 2100, ErrorMessage = "Năm xuất bản không hợp lệ")]
        [Display(Name = "Năm xuất bản")]
        public int PublishedYear { get; set; }
    }
}