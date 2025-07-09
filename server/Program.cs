using FellowOakDicom;
using FellowOakDicom.Imaging;
using server.Contracts;
using server.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IDicomService, DicomService>();
builder.Services.AddSingleton<IBlobStorageService, BlobStorageService>();

builder.Services.AddCors(options => {
    options.AddPolicy("AllowAll", policy => {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

new DicomSetupBuilder()
    .RegisterServices(s => s.AddImageManager<ImageSharpImageManager>())
    .Build();

builder.Services.AddControllers();
builder.Services.AddOpenApi();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("AllowAll");
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
