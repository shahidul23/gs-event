using GSEvent.Extensions;
using GSEvent.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationService(
    builder.Configuration
);

builder.Services.AddExternalServices(
    builder.Configuration
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/swagger/v1/swagger.json",
            "GSEvent API v1"
        );
        options.RoutePrefix = "swagger"; 
    });
}
// Global Exception Middleware
// app.UseMiddleware<ExceptionMiddleware>();

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapGet("/", () => "Api is running");

app.Run();
