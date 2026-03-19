using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using SkillProof.Entities.Models;
using SkillProof.Entities.Dtos.Users;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using System.Text;

namespace SkillProof.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController:ControllerBase
    {
        private readonly IWebHostEnvironment env;
        UserManager<Users> userManager;
        RoleManager<IdentityRole> roleManager;

        public UserController(UserManager<Users> userManager, IWebHostEnvironment env, RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.env = env;
            this.roleManager = roleManager;
        }

        [HttpPost("Register")]
        public async Task RegiterUser(RegisterUser dto)
        {
            if (dto.Password.Length < 8) throw new ArgumentException("The password must be at least 8 characters long");

            if (await userManager.FindByEmailAsync(dto.Email) != null) throw new ArgumentException("Profile with this email already exists");

            if (!(IsValidEmail(dto.Email))) throw new ArgumentException("The email address format is invalid");

            var user = new Users();
            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.Email = dto.Email;
            user.UserName = dto.Email.Split('@')[0];

            var defaultImagePath = Path.Combine(env.WebRootPath, "image", "default.png");

            var fileBytes = await System.IO.File.ReadAllBytesAsync(defaultImagePath);

            user.ProfilePicture = fileBytes;

            var result = await userManager.CreateAsync(user, dto.Password);

            if (userManager.Users.Count() == 1)
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
                await userManager.AddToRoleAsync(user, "Admin");
            }


        }

        [HttpGet("GetAllUsers")]
        public async Task<IEnumerable<ViewUser>> GetAllUsers()
        {
            var users = await userManager.Users.ToListAsync();

            var result = new List<ViewUser>();

            foreach (var user in users)
            {
                result.Add(new ViewUser
                {
                    Id = user.Id,
                    Email = user.Email,
                    FullName = user.FirstName + " " + user.LastName,                            
                    Image = Convert.ToBase64String(user.ProfilePicture),              
                    Headline = user.Headline,
                    Bio = user.Bio
                });
            }

            return result;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ViewUser>> GetUserById(string id)
        {
            var user = await userManager.FindByIdAsync(id);

            if (user is null) return NotFound("User not found");

            var userView = new ViewUser
            {
                Id = user.Id,
                FullName = $"{user.FirstName} {user.LastName}",
                Email = user.Email,
                Image = Convert.ToBase64String(user.ProfilePicture),
                Bio = user.Bio,
                Headline = user.Headline,            
            };

            return userView;
        }

        [HttpPut("{id}")]
        public async Task UpdateUser(string id, [FromBody] UpdateUser dto)
        {
            var currentUser = await userManager.Users.FirstOrDefaultAsync(u => u.Id == id);

            currentUser.Email = dto.Email;
            currentUser.FirstName = dto.FirstName;
            currentUser.LastName = dto.LastName;
            currentUser.Bio = dto.Bio;
            currentUser.Headline = dto.HeadLine;
            currentUser.ProfilePicture = Convert.FromBase64String(dto.ProfilePicture); //Majd a Fe Alakítja át Base64re byte[]-ból


            await userManager.SetEmailAsync(currentUser, dto.Email);
            await userManager.UpdateAsync(currentUser);

        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(AppUserLoginDto dto)
        {
            var user = await userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                return BadRequest(new { message = "Incorrect Email" });
            }

            // Check if user is banned
            if (user.IsBanned)
            {
                return Unauthorized(new { message = "Your account has been banned. Please contact support." });
            }
            else
            {
                var result = await userManager.CheckPasswordAsync(user, dto.Password);
                if (!result)
                {
                    return BadRequest(new { message = "Incorrect Password" });
                }
                else
                {
                    //todo: generate token
                    var claim = new List<Claim>();
                    claim.Add(new Claim(ClaimTypes.Name, user.UserName!));
                    claim.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));

                    foreach (var role in await userManager.GetRolesAsync(user))
                    {
                        claim.Add(new Claim(ClaimTypes.Role, role));
                    }

                    int expiryInMinutes = 24 * 60;
                    var token = GenerateAccessToken(claim, expiryInMinutes);

                    return Ok(new LoginResultDto()
                    {
                        Token = new JwtSecurityTokenHandler().WriteToken(token),
                        Expiration = DateTime.Now.AddMinutes(expiryInMinutes)
                    });

                }
            }
        }


        private bool IsValidEmail(string email)
        {
            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
        }

    }

    
}
