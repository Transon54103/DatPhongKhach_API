var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//viết thêm để tránh sự lỏng lẻo nếu web cần xml mà trả về json nó sẽ tb ra lỗi 406 chứ không chuyển về json để tiếp tục ctrinh
builder.Services.AddControllers
( 
    //option =>
    //{
    //bật chế độ kiểu tra định dạng
    //option.ReturnHttpNotAcceptable = true;
//}
).AddNewtonsoftJson().AddXmlDataContractSerializerFormatters();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
