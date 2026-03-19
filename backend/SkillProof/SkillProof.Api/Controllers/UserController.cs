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
    public class UserController
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
                var roles = await userManager.GetRolesAsync(user);

                result.Add(new ViewUser
                {
                    Id = user.Id,
                    Email = user.Email,
                    FullName = user.FirstName + " " + user.LastName,                            
                    Image = Convert.ToBase64String(user.ProfilePicture),
                    Role = roles.FirstOrDefault(),
                    Headline = user.Headline,
                    Bio = user.Bio
                });
            }

            return result;
        }




        private bool IsValidEmail(string email)
        {
            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
        }

    }

    
}
