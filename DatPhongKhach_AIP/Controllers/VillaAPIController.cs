using DatPhongKhach_AIP.Data;
using DatPhongKhach_AIP.Models;
using DatPhongKhach_AIP.Models.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace DatPhongKhach_AIP.Controllers
{
    [Route("api/[controller]")]

    //[Route("api/VillaAPI")]
    [ApiController]
    //cái này còn giúp tự động kiểm tra điều kiện của thuộc tính của DTO
    
    public class VillaAPIController : ControllerBase
    {
        //tiêm logger của asp cung ccaaps từ bên ngoài vào controller mục đích để thêm thông tin vào nhật ký
        public VillaAPIController()
        {
        }
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<VillaDTO>> GetVillas()
        {

            //OK là trạng thái hợp lệ 200
            return Ok(VillaStore.villalist);
        }

        //[HttpGet("id")]
        //nếu không khai báo hoạt động http nó sẽ mặc định là httpget
        [HttpGet("{id:int}", Name ="GetVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        //ngoài ra chúng ta có 
        //        [ProducesResponseType(200)]
        public ActionResult<VillaDTO> GetVillas(int id)
        {
            if(id == 0) {

                return BadRequest(); }
            // nếu id = 0 thì trả về trạng thái 400 lỗi
            var villa = VillaStore.villalist.FirstOrDefault(u => u.Id == id);
            if (villa == null) { return NotFound(); }
            // nếu villa = null nghĩa là không tìm thấy đối tượng thì trả về not found
            return Ok(villa);
            // firstordefault là một phương thức trả về 1 giá trị đầu tiên thỏa mãn điều kiện lam đa nếu không 
            //có giá trị nào thỏa mãn thì nó sẽ trả về giá trị null( đối với tham chiếu) or 0 (đối với tham trị)
        }
        [HttpPost]
        //thêm đối tượng
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<VillaDTO> CreateVilla([FromBody]VillaDTO villaDTO)
        {
            //nếu nó là false thì chạy trả về thông báo không hợp lệ 
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}
            if(VillaStore.villalist.FirstOrDefault(u => u.Name.ToLower() == villaDTO.Name.ToLower())!= null)
            {
                ModelState.AddModelError("CustomError", "Villa already Exits!");
                return BadRequest(ModelState);
            }    
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
            //return Ok(villaDTO); việc sử dụng createdatroute ở dưới có nghĩa nó thích hợp để 
            //cho client dễ dàng xây dựng và cái nó sẽ tạo ra url truy cập ngay tài nguyên mà không cần tự suy đoán
            //kèm theo là ID 
            // thể hiện chuần restful api
            return CreatedAtRoute("GetVilla", new {id = villaDTO.Id},villaDTO);
        }
        [HttpDelete("{id:int}", Name = "DeleteVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteVilla(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            var villa = VillaStore.villalist.FirstOrDefault(u => u.Id == id);
            if(villa == null)
            {
                return BadRequest();
            }
            VillaStore.villalist.Remove(villa);
            return NoContent();
        }

        [HttpPut("{id:int}", Name = "UpdateVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult UpadteVilla(int id, [FromBody]VillaDTO villaDTO)
        {
            if (villaDTO == null || id != villaDTO.Id)
            {
                return BadRequest();
            }
            var villa = VillaStore.villalist.FirstOrDefault(u => u.Id == id);
            villa.Name = villaDTO.Name;
            villa.Sqft = villaDTO.Sqft;
            villa.Occupancy = villaDTO.Occupancy;
            return NoContent();
        }

        [HttpPatch("{id:int}", Name = "UpdatePartiakVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult UpdatePartiakVilla(int id, JsonPatchDocument<VillaDTO> patchDTO)
        {
            if (patchDTO == null || id == 0)
            {
                return BadRequest();
            }
            var villa = VillaStore.villalist.FirstOrDefault(u => u.Id == id);
            if(villa == null)
            {
                return BadRequest();
            } 
            //AppltTo: truyền đối tượng cần thao tác  là hàm có thể 2 tham số tham số 1 là đối tượng cần thao tác
            //tham số 2 là tính hợp lệ của đối tượng
            patchDTO.ApplyTo(villa, ModelState);
            if (!ModelState.IsValid){
                return BadRequest(ModelState);
            }
            return NoContent();
        }


    }
}
