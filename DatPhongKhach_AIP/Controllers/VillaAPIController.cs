using DatPhongKhach_AIP.Data;
using DatPhongKhach_AIP.Models;
using DatPhongKhach_AIP.Models.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatPhongKhach_AIP.Controllers
{
    [Route("api/[controller]")]

    //[Route("api/VillaAPI")]
    [ApiController]
    //cái này còn giúp tự động kiểm tra điều kiện của thuộc tính của DTO

    public class VillaAPIController : ControllerBase
    {
        //tiêm logger của asp cung ccaaps từ bên ngoài vào controller mục đích để thêm thông tin vào nhật ký
        private ApplicationDbContext _db;
        public VillaAPIController(ApplicationDbContext context)
        {
            _db = context;
        }
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<VillaDTO>>> GetVillas()
        {

            //OK là trạng thái hợp lệ 200
            return Ok(await _db.Villas.ToListAsync());
        }
        //[HttpGet("id")]
        //nếu không khai báo hoạt động http nó sẽ mặc định là httpget
        [HttpGet("{id:int}", Name = "GetVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        //ngoài ra chúng ta có 
        //        [ProducesResponseType(200)]
        public async Task<ActionResult<VillaDTO>> GetVillas(int id)
        {
            if (id == 0)
            {

                return BadRequest();
            }
            // nếu id = 0 thì trả về trạng thái 400 lỗi
            var villa = await _db.Villas.FirstOrDefaultAsync(u => u.Id == id);
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
        public async Task<ActionResult<VillaDTO>> CreateVilla([FromBody] VillaCreateDTO villaDTO)
        {
            //nếu nó là false thì chạy trả về thông báo không hợp lệ 
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}
         
            if (await _db.Villas.FirstOrDefaultAsync(u => u.Name.ToLower() == villaDTO.Name.ToLower()) != null)
            {
                ModelState.AddModelError("CustomError", "Villa already Exits!");
                return BadRequest(ModelState);
            }
            if (villaDTO == null)
            {
                return BadRequest(villaDTO);
            }
            //if (villaDTO.Id > 0)
            //{
            //    return StatusCode(StatusCodes.Status500InternalServerError);
            //}
            Villa model = new ()
            {
                Amenity = villaDTO.Amenity,
                Details = villaDTO.Details,
                Name = villaDTO.Name,
                Occupancy = villaDTO.Occupancy,
                Rate = villaDTO.Rate,
                Sqft = villaDTO.Sqft,
                ImageUrl = villaDTO.ImageUrl,
            };
            //villaDTO.Id = _db.Villas.OrderByDescending(u => u.Id).FirstOrDefault().Id+1;
            await _db.Villas.AddAsync(model);
            await _db.SaveChangesAsync();
            //return Ok(villaDTO); việc sử dụng createdatroute ở dưới có nghĩa nó thích hợp để 
            //cho client dễ dàng xây dựng và cái nó sẽ tạo ra url truy cập ngay tài nguyên mà không cần tự suy đoán
            //kèm theo là ID 
            // thể hiện chuần restful api
            return CreatedAtRoute("GetVilla", new { id = model.Id }, villaDTO);
        }
        [HttpDelete("{id:int}", Name = "DeleteVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteVilla(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            var villa = await _db.Villas.FirstOrDefaultAsync(u => u.Id == id);
            if (villa == null)
            {
                return BadRequest();
            }
            _db.Villas.Remove(villa);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id:int}", Name = "UpdateVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpadteVilla(int id, [FromBody] VillaUpdateDTO villaDTO)
        {
            if (villaDTO == null || id != villaDTO.Id)
            {
                return BadRequest();
            }
            //var villa = _db.Villas.FirstOrDefault(u => u.Id == id);
            //villa.Name = villaDTO.Name;
            //villa.Sqft = villaDTO.Sqft;
            //villa.Occupancy = villaDTO.Occupancy;
            Villa model = new ()
            {
                Amenity = villaDTO.Amenity,
                Details = villaDTO.Details,
                Id = villaDTO.Id,
                Name = villaDTO.Name,
                Occupancy = villaDTO.Occupancy,
                Rate = villaDTO.Rate,
                Sqft = villaDTO.Sqft,
                ImageUrl = villaDTO.ImageUrl,
            };
            _db.Villas.Update(model);
            await _db.SaveChangesAsync();
            return NoContent();

        }

        [HttpPatch("{id:int}", Name = "UpdatePartiakVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async  Task<IActionResult> UpdatePartiakVilla(int id, JsonPatchDocument<VillaUpdateDTO> patchDTO)
        {
            if (patchDTO == null || id == 0)
            {
                return BadRequest();
            }
            //vì chỉ có thể update được villa không update được villasDTO
            //nên chúng ta phải làm cách này 
            var villa = await _db.Villas.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
            VillaUpdateDTO villaDTO = new()
            {
                Amenity = villa.Amenity,
                Details = villa.Details,
                Id = villa.Id,
                Name = villa.Name,
                ImageUrl = villa.ImageUrl,
                Occupancy = villa.Occupancy,
                Rate = villa.Rate,
                Sqft = villa.Sqft,
            };
            if (villa == null)
            {
                return BadRequest();
            }
            patchDTO.ApplyTo(villaDTO, ModelState);
            Villa model = new Villa()
            {
                Amenity = villaDTO.Amenity,
                Details = villaDTO.Details,
                Id = villaDTO.Id,
                Name = villaDTO.Name,
                ImageUrl = villaDTO.ImageUrl,
                Occupancy = villaDTO.Occupancy,
                Rate = villaDTO.Rate,
                Sqft = villaDTO.Sqft,
            };
            _db.Villas.Update(model);
            await _db.SaveChangesAsync();
            //AppltTo: truyền đối tượng cần thao tác  là hàm có thể 2 tham số tham số 1 là đối tượng cần thao tác
            //tham số 2 là tính hợp lệ của đối tượng
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return NoContent();
        }
    }
}
