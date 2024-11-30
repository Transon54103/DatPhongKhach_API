using DatPhongKhach_AIP.Models.Dto;

namespace DatPhongKhach_AIP.Data
{
    public class VillaStore
    {
        public static List<VillaDTO> villalist = new List<VillaDTO>
        {
            new VillaDTO{Id = 1, Name="VanSon", Sqft= 100, Occupancy= 4},
            new VillaDTO{Id = 2, Name= "VanAn", Sqft= 150, Occupancy= 5}
        };
    }
}
