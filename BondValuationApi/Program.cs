using Azure.Storage.Blobs;
using BondValuationApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure Swagger/OpenAPI with file upload support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Bond Valuation API",
 Version = "v1",
        Description = "API for uploading bond positions and retrieving valuation results"
    });
});

// Configure CORS (optional, for development)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
   policy.AllowAnyOrigin()
          .AllowAnyMethod()
   .AllowAnyHeader();
    });
});

// Configure Blob Storage
var blobStorageOptions = new BlobStorageOptions();
builder.Configuration.GetSection(BlobStorageOptions.SectionName).Bind(blobStorageOptions);

// If not in config, try to get from connection strings
if (string.IsNullOrEmpty(blobStorageOptions.ConnectionString))
{
    blobStorageOptions.ConnectionString =
        builder.Configuration.GetConnectionString("BondValuationStorageConnection")
  ?? throw new InvalidOperationException("Blob storage connection string not configured");
}

builder.Services.AddSingleton(blobStorageOptions);
builder.Services.AddSingleton(x => new BlobServiceClient(blobStorageOptions.ConnectionString));
builder.Services.AddScoped<IBlobStorageService, BlobStorageService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
     options.SwaggerEndpoint("/swagger/v1/swagger.json", "Bond Valuation API v1");
        options.DocumentTitle = "Bond Valuation API";
    });
  app.UseCors("AllowAll");
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
