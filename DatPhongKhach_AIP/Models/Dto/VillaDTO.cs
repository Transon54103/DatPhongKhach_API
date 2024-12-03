using System.ComponentModel.DataAnnotations;

namespace DatPhongKhach_AIP.Models.Dto
{
    public class VillaDTO
    {
        //DTO cung cấp một lớp vỏ bóc giữa thực thể hoặc mô hình cơ sở
        //dữ liệu và những gì đang được phơi bày ở giao diện API
        public int Id { get; set; }
        [Required]
        [MaxLength(30)]
        public string Name { get; set; }
        public int Occupancy { get; set; }
        public int Sqft { get; set; }
        public string ImageUrl { get; set; }
        public string Amenity { get; set; }
        public string Details { get; set; }
        [Required]
        public double Rate { get; set; }

    }
}
