         using BKU.Data;
        using Microsoft.AspNetCore.Authorization;
        using Microsoft.AspNetCore.Mvc;
        using Microsoft.EntityFrameworkCore;
        using Microsoft.AspNetCore.Authentication;
        using Microsoft.AspNetCore.Authentication.Cookies;
        using Serilog;
        using System.Security.Claims;

        namespace BKU.Controllers
        {
            [ApiController]
            [Route("api/[controller]")]
            public class AuthController : ControllerBase
            {
                private readonly ApplicationDbContext _context;
                public AuthController(ApplicationDbContext context) => _context = context;

                // ==== DTO'lar ====
                public class LoginRequest
                {
                    public string eposta { get; set; } = string.Empty;
                    public string password { get; set; } = string.Empty;
                    public bool rememberMe { get; set; } = false;
                }

                public class ChangePasswordRequest
                {
                    public string currentPassword { get; set; } = string.Empty;
                    public string newPassword { get; set; } = string.Empty;
                }

                // ==== LOGIN (Cookie) ====
                [HttpPost("login")]
                [AllowAnonymous]
                public async Task<IActionResult> Login([FromBody] LoginRequest req, CancellationToken ct)
                {
                    var user = await _context.Kullanicilar
                        .FirstOrDefaultAsync(u => u.Email == req.eposta && u.Parola == req.password, ct);

                    if (user is null)
                    {
                        Log.Warning("Login FAILED: Email={Email}, Time={Time}", req.eposta, DateTime.Now);
                        return Unauthorized(new { message = "E-posta veya şifre hatalı." });
                    }

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new Claim(ClaimTypes.Name, user.Email ?? string.Empty),
                        new Claim(ClaimTypes.Role, user.Role ?? "User"),
                        new Claim("username", user.Username ?? string.Empty)
                    };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    var authProps = new AuthenticationProperties
                    {
                        IsPersistent = req.rememberMe,       // tarayıcı kapansa da kalsın mı?
                        AllowRefresh = true,
                        ExpiresUtc = req.rememberMe ? DateTimeOffset.UtcNow.AddDays(7)
                                                      : DateTimeOffset.UtcNow.AddHours(1)
                    };

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProps);

                    Log.Information("Login SUCCESS: Email={Email}, Role={Role}, Time={Time}",
                        user.Email, user.Role ?? "User", DateTime.Now);

                    return Ok(new
                    {
                        message = "Giriş başarılı.",
                        user = new { user.Id, user.Username, user.Email, Role = user.Role ?? "User" },
                        expiresAtUtc = authProps.ExpiresUtc
                    });
                }

                // ==== LOGOUT ====
                [HttpPost("logout")]
                [Authorize] 
                public async Task<IActionResult> Logout()
                {
                    var email = User.FindFirstValue(ClaimTypes.Name) ?? "Bilinmiyor";
                    var role = User.FindFirstValue(ClaimTypes.Role) ?? "Rol yok";

                    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                    Log.Information("Logout: Email={Email}, Role={Role}, Time={Time}", email, role, DateTime.Now);
                    return Ok(new { message = "Kullanıcı başarıyla çıkış yaptı." });
                }

      
                [HttpPut("change_password")]
                [Authorize]
                public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest req, CancellationToken ct)
                {
                    var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    if (!int.TryParse(userIdStr, out int userId))
                        return Unauthorized(new { message = "Oturum bilgisi okunamadı." });

                    var user = await _context.Kullanicilar.FirstOrDefaultAsync(u => u.Id == userId, ct);
                    if (user is null)
                        return NotFound(new { message = "Kullanıcı bulunamadı." });

                    // Düz metin kontrol (geçici). Üretimde: BCrypt.Verify(...)
                    if (!string.Equals(user.Parola, req.currentPassword))
                        return BadRequest(new { message = "Geçerli şifre yanlış." });

                    user.Parola = req.newPassword; // Üretimde: user.Parola = BCrypt.HashPassword(req.newPassword);
                    await _context.SaveChangesAsync(ct);

                    Log.Information("Password changed: UserId={UserId}, Time={Time}", userId, DateTime.Now);
                    return Ok(new { message = "Şifre başarıyla güncellendi." });
                }

                // (Opsiyonel) Oturum kontrolü
                [HttpGet("me")]
                [Authorize]
                public IActionResult Me()
                {
                    var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    var email = User.FindFirstValue(ClaimTypes.Name);
                    var role = User.FindFirstValue(ClaimTypes.Role);
                    var uname = User.FindFirstValue("username");

                    return Ok(new { id, username = uname, email, role });
                }
            }
        }
