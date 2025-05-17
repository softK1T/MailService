using MailService.Models;
using MailService.Services;
using Microsoft.OpenApi.Models;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

Env.Load();

builder.Services.AddControllers();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Mail Service", Version = "v1" });
});

builder.Services.Configure<MailSettings>(options =>
{
    options.Mail = Environment.GetEnvironmentVariable("MAIL_ADDRESS") ?? "";
    options.DisplayName = Environment.GetEnvironmentVariable("MAIL_DISPLAY_NAME") ?? "";
    options.Password = Environment.GetEnvironmentVariable("MAIL_PASSWORD") ?? "";
    options.Host = Environment.GetEnvironmentVariable("MAIL_HOST") ?? "";
    options.Port = int.TryParse(Environment.GetEnvironmentVariable("MAIL_PORT"), out var port) ? port : 587;
    options.UseSSL = bool.TryParse(Environment.GetEnvironmentVariable("MAIL_USE_SSL"), out var useSsl) ? useSsl : true;
});

builder.Services.AddTransient<IEmailService, EmailService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
