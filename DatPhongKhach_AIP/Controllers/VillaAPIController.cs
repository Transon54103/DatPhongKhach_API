using AutoMapper;
using DatPhongKhach_AIP.Data;
using DatPhongKhach_AIP.Models;
using DatPhongKhach_AIP.Models.Dto;
using DatPhongKhach_AIP.Repository.IRepository;
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
        private IVillaRepository _dbVilla;
        private readonly IMapper _mapper;
        public VillaAPIController(IVillaRepository dbVilla, IMapper mapper)
        {
            _dbVilla = dbVilla;
            _mapper = mapper;
        }
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        // lấy cơ sở dữ liệu bằng kiểu IEnumerable
        public async Task<ActionResult<IEnumerable<VillaDTO>>> GetVillas()
        {
            //IEnumerable<Villa> villaList = await _db.Villas.ToListAsync();
            //trả về json từ việc chuyển list sang json
            IEnumerable<Villa> villaList = await _dbVilla.GetAllAsync();
            //OK là trạng thái hợp lệ 200
            return Ok(_mapper.Map<List<VillaDTO>>(villaList));
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
            var villa = await _dbVilla.GetAsync(u => u.Id == id);
            //var villa = await _db.Villas.FirstOrDefaultAsync(u => u.Id == id);
            if (villa == null) { return NotFound(); }
            // nếu villa = null nghĩa là không tìm thấy đối tượng thì trả về not found
            return Ok(_mapper.Map<VillaDTO>(villa));
            // firstordefault là một phương thức trả về 1 giá trị đầu tiên thỏa mãn điều kiện lam đa nếu không 
            //có giá trị nào thỏa mãn thì nó sẽ trả về giá trị null( đối với tham chiếu) or 0 (đối với tham trị)
        }
        [HttpPost]
        //thêm đối tượng
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<VillaDTO>> CreateVilla([FromBody] VillaCreateDTO createDTO)
        {
            //nếu nó là false thì chạy trả về thông báo không hợp lệ 
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            if (await _dbVilla.GetAsync(u => u.Name.ToLower() == createDTO.Name.ToLower()) != null)
            {
                ModelState.AddModelError("CustomError", "Villa already Exits!");
                return BadRequest(ModelState);
            }
            if (createDTO == null)
            {
                return BadRequest(createDTO);
            }
            //if (villaDTO.Id > 0)
            //{
            //    return StatusCode(StatusCodes.Status500InternalServerError);
            //}
            Villa model = _mapper.Map<Villa>(createDTO);
            //Villa model = new ()
            //{
            //    Amenity = createDTO.Amenity,
            //    Details = createDTO.Details,
            //    Name = createDTO.Name,
            //    Occupancy = createDTO.Occupancy,
            //    Rate = createDTO.Rate,
            //    Sqft = villaDTO.Sqft,
            //    ImageUrl = createDTO.ImageUrl,
            //};
            //villaDTO.Id = _db.Villas.OrderByDescending(u => u.Id).FirstOrDefault().Id+1;
            await _dbVilla.CreateAsync(model);
            //return Ok(villaDTO); việc sử dụng createdatroute ở dưới có nghĩa nó thích hợp để 
            //cho client dễ dàng xây dựng và cái nó sẽ tạo ra url truy cập ngay tài nguyên mà không cần tự suy đoán
            //kèm theo là ID 
            // thể hiện chuần restful api
            return CreatedAtRoute("GetVilla", new { id = model.Id }, createDTO);
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
            var villa = await _dbVilla.GetAsync(u => u.Id == id);
            if (villa == null)
            {
                return BadRequest();
            }
            await _dbVilla.RemoveAsync(villa);
            return NoContent();
        }

        [HttpPut("{id:int}", Name = "UpdateVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpadteVilla(int id, [FromBody] VillaUpdateDTO updateDTO)
        {
            if (updateDTO == null || id != updateDTO.Id)
            {
                return BadRequest();
            }
            //var villa = _db.Villas.FirstOrDefault(u => u.Id == id);
            //villa.Name = villaDTO.Name;
            //villa.Sqft = villaDTO.Sqft;
            //villa.Occupancy = villaDTO.Occupancy;
            Villa model = _mapper.Map<Villa>(updateDTO);
            //Villa model = new ()
            //{
            //    Amenity = updateDTO.Amenity,
            //    Details = updateDTO.Details,
            //    Id = updateDTO.Id,
            //    Name = villaDTO.Name,
            //    Occupancy = villaDTO.Occupancy,
            //    Rate = villaDTO.Rate,
            //    Sqft = villaDTO.Sqft,
            //    ImageUrl = villaDTO.ImageUrl,
            //};
            await _dbVilla.UpdateAsync(model);
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
            var villa = await _dbVilla.GetAsync(u => u.Id == id, tracked: false);
            VillaUpdateDTO villaDTO = _mapper.Map<VillaUpdateDTO>(villa);
            //VillaUpdateDTO villaDTO = new()
            //{
            //    Amenity = villa.Amenity,
            //    Details = villa.Details,
            //    Id = villa.Id,
            //    Name = villa.Name,
            //    ImageUrl = villa.ImageUrl,
            //    Occupancy = villa.Occupancy,
            //    Rate = villa.Rate,
            //    Sqft = villa.Sqft,
            //};
            if (villa == null)
            {
                return BadRequest();
            }
            patchDTO.ApplyTo(villaDTO, ModelState);
            Villa model = _mapper.Map<Villa>(villaDTO);
            //Villa model = new Villa()
            //{
            //    Amenity = villaDTO.Amenity,
            //    Details = villaDTO.Details,
            //    Id = villaDTO.Id,
            //    Name = villaDTO.Name,
            //    ImageUrl = villaDTO.ImageUrl,
            //    Occupancy = villaDTO.Occupancy,
            //    Rate = villaDTO.Rate,
            //    Sqft = villaDTO.Sqft,
            //};
            await _dbVilla.UpdateAsync(model);
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
