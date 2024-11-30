using DatPhongKhach_AIP.Data;
using DatPhongKhach_AIP.Models;
using DatPhongKhach_AIP.Models.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DatPhongKhach_AIP.Controllers
{
    [Route("api/[controller]")]

    //[Route("api/VillaAPI")]
    [ApiController]
    
    public class VillaAPIController : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<VillaDTO>> GetVillas()
        {
            //OK là trạng thái hợp lệ 200
            return Ok(VillaStore.villalist);
        }

        //[HttpGet("id")]
        //nếu không khai báo hoạt động http nó sẽ mặc định là httpget
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        //ngoài ra chúng ta có 
        //        [ProducesResponseType(200)]
        public ActionResult<VillaDTO> GetVillas(int id)
        {
            if(id == 0) { return BadRequest(); }
            // nếu id = 0 thì trả về trạng thái 400 lỗi
            var villa = VillaStore.villalist.FirstOrDefault(u => u.Id == id);
            if (villa == null) { return NotFound(); }
            // nếu villa = null nghĩa là không tìm thấy đối tượng thì trả về not found
            return Ok(villa);
            // firstordefault là một phương thức trả về 1 giá trị đầu tiên thỏa mãn điều kiện lam đa nếu không 
            //có giá trị nào thỏa mãn thì nó sẽ trả về giá trị null( đối với tham chiếu) or 0 (đối với tham trị)
        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<VillaDTO> CreateVilla([FromBody]VillaDTO villaDTO)
        {
            if (villaDTO == null) 
            {
                return BadRequest(villaDTO);
            }
            if(villaDTO.Id > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            villaDTO.Id = VillaStore.villalist.OrderByDescending(u => u.Id).FirstOrDefault().Id+1;
            VillaStore.villalist.Add(villaDTO);
            return Ok(villaDTO);
        }
    }
}
