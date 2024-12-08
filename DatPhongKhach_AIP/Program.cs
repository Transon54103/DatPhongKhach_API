
using DatPhongKhach_AIP;
using DatPhongKhach_AIP.Data;
using DatPhongKhach_AIP.Repository;
using DatPhongKhach_AIP.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
Log.Logger = new LoggerConfiguration().MinimumLevel.Debug()
    .WriteTo.File("log/villalogs.txt",rollingInterval: RollingInterval.Day).CreateLogger();

builder.Services.AddAutoMapper(typeof(MappingConfig));
builder.Services.AddScoped<IVillaRepository, VillaRepository>();
builder.Host.UseSerilog();
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
builder.Services.AddDbContext<ApplicationDbContext>(
    option =>
    {
        option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    });
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
