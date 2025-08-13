

        using BKU.Data;
        using BKU.Repository;
        using BKU.Repository.Interfaces;
        using Microsoft.AspNetCore.Authentication.JwtBearer;
        using Microsoft.EntityFrameworkCore;
        using Microsoft.IdentityModel.Tokens;
        using Serilog;
        using Serilog.Events;
        using BKU.Hubs;
        using System.Text;

        var builder = WebApplication.CreateBuilder(args);
        var logDir = Path.Combine(AppContext.BaseDirectory, "logs");
        Directory.CreateDirectory(logDir);

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Error)
            .WriteTo.Console()
            .WriteTo.File(Path.Combine(logDir, "log-.txt"), rollingInterval: RollingInterval.Day, shared: true)
            .CreateLogger();

        builder.Host.UseSerilog();
      //  builder.WebHost.UseUrls("http://0.0.0.0:5058", "http://localhost:5058");
       builder.WebHost.UseUrls("http://172.22.18.61:5058");

        builder.Services.AddDbContext<ApplicationDbContext>(opt =>
            opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddScoped<IAnswerRepository, AnswerRepository>();
        builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();
        builder.Services.AddScoped<IKullanicilarRepository, KullanicilarRepository>();
        builder.Services.AddScoped<IUserAnswerRepository, UserAnswerRepository>();

        builder.Services.AddControllers().AddJsonOptions(o =>
        {
            o.JsonSerializerOptions.ReferenceHandler =
                System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        });

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
        });
        builder.Host.UseSerilog();
        builder.Services.AddOpenApi();

        // ---- JWT Bearer ----
        // Not: Key'i appsettings.json -> "Jwt:Key" içine taþý ve en az 32 karakter yap
        //var key = Encoding.UTF8.GetBytes("CHANGE_THIS_TO_MIN_32_CHARS_SECRET_KEY_2025!");
        //builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        //    .AddJwtBearer(o =>
        //    {
        //        o.TokenValidationParameters = new TokenValidationParameters
        //        {
        //            ValidateIssuer = false,
        //            ValidateAudience = false,
        //            ValidateLifetime = true,
        //            ValidateIssuerSigningKey = true,
        //            IssuerSigningKey = new SymmetricSecurityKey(key),
        //            ClockSkew = TimeSpan.Zero
        //        };
        //    });

        builder.Services.AddAuthorization();
        builder.Services.AddSignalR();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        // app.UseHttpsRedirection();

        app.UseSerilogRequestLogging();

        app.UseCors("AllowAll");

        app.UseAuthentication();  
       // app.UseAuthorization();
        app.MapHub<QuizHub>("/hubs/quiz");
        app.MapControllers();

        app.Lifetime.ApplicationStarted.Register(() =>
        {
            Log.Information("Exe çalýþmaya baþladý : {Tarih}", DateTime.Now);
        });

        app.Run();
