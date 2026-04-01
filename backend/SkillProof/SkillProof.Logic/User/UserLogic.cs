using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SkillProof.Data.Repositorys;
using SkillProof.Entities.Dtos.Users;
using SkillProof.Entities.Helper;
using SkillProof.Entities.Models;

namespace SkillProof.Logic.User
{

    public class UserLogic : IUserLogic
    {
        private readonly UserManager<Users> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IRepository<Entities.Models.Companies> _companyRepository;
        private readonly IWebHostEnvironment _env;
        private readonly JwtSettings _jwtSettings;

        public UserLogic(
            UserManager<Users> userManager,
            RoleManager<IdentityRole> roleManager,
            IRepository<Entities.Models.Companies> companyRepository,
            IWebHostEnvironment env,
            IOptions<JwtSettings> jwtSettings)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _companyRepository = companyRepository;
            _env = env;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task RegisterUserAsync(RegisterUser dto)
        {
            if (dto.Password.Length < 8)
                throw new ArgumentException("The password must be at least 8 characters long.");

            if (await _userManager.FindByEmailAsync(dto.Email) != null)
                throw new ArgumentException("Profile with this email already exists.");

            if (!IsValidEmail(dto.Email))
                throw new ArgumentException("The email address format is invalid.");

            var user = new Users
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                UserName = dto.Email.Split('@')[0]
            };

            var defaultImagePath = Path.Combine(_env.WebRootPath, "image", "default.png");
            user.ProfilePicture = await System.IO.File.ReadAllBytesAsync(defaultImagePath);

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Registration failed: {errors}");
            }

            if (_userManager.Users.Count() == 1)
            {
                if (!await _roleManager.RoleExistsAsync("Admin"))
                {
                    await _roleManager.CreateAsync(new IdentityRole("Admin"));
                }

                await _userManager.AddToRoleAsync(user, "Admin");
            }
        }

        public async Task RegisterEmployerAsync(RegisterEmployer dto)
        {
            if (dto.Password.Length < 8)
                throw new ArgumentException("The password must be at least 8 characters long.");
            if (await _userManager.FindByEmailAsync(dto.Email) != null)
                throw new ArgumentException("Profile with this email already exists.");
            if (!IsValidEmail(dto.Email)) throw new ArgumentException("The email address format is invalid.");

            var newCompany = new Entities.Models.Companies
            {
                Id = Guid.NewGuid().ToString(),
                Name = dto.CompanyName,
                Description = dto.CompanyDescription,
                Website = dto.CompanyWebsite,
                CreatedAt = DateTime.UtcNow
            };

            await _companyRepository.Create(newCompany);

            var user = new Users
            {
                FirstName = dto.CompanyName,
                LastName = "Account",
                Email = dto.Email,
                UserName = dto.Email.Split('@')[0],
                CompanyId = newCompany.Id,
                CompanyRole = "Owner"
            };

            var defaultImagePath = Path.Combine(_env.WebRootPath, "image", "default.png");
            user.ProfilePicture = await System.IO.File.ReadAllBytesAsync(defaultImagePath);

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Registration failed: {errors}");
            }

            if (!await _roleManager.RoleExistsAsync("Employer"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Employer"));
            }

            await _userManager.AddToRoleAsync(user, "Employer");
        }

        public async Task<IEnumerable<ViewUser>> GetAllUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync();
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

        public async Task<ViewUser> GetUserByIdAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            return new ViewUser
            {
                Id = user.Id,
                FullName = $"{user.FirstName} {user.LastName}",
                Email = user.Email,
                Image = Convert.ToBase64String(user.ProfilePicture),
                Bio = user.Bio,
                Headline = user.Headline,
            };
        }

        public async Task UpdateUserAsync(string id, UpdateUser dto)
        {
            var currentUser = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (currentUser == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            currentUser.Email = dto.Email;
            currentUser.FirstName = dto.FirstName;
            currentUser.LastName = dto.LastName;
            currentUser.Bio = dto.Bio;
            currentUser.Headline = dto.HeadLine;

            if (!string.IsNullOrWhiteSpace(dto.ProfilePicture))
            {
                currentUser.ProfilePicture = Convert.FromBase64String(dto.ProfilePicture);
            }

            await _userManager.SetEmailAsync(currentUser, dto.Email);
            await _userManager.UpdateAsync(currentUser);
        }

        public async Task DeleteUserAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Delete failed: {errors}");
            }
        }

        public async Task<LoginResultDto> LoginAsync(LoginUser dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                throw new UnauthorizedAccessException("Incorrect email.");
            }

            var result = await _userManager.CheckPasswordAsync(user, dto.Password);
            if (!result)
            {
                throw new UnauthorizedAccessException("Incorrect password.");
            }

            var claim = new List<Claim>();
            claim.Add(new Claim(ClaimTypes.Name, user.UserName!));
            claim.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            if (!string.IsNullOrEmpty(user.CompanyId))
            {
                claim.Add(new Claim("CompanyId", user.CompanyId));
            }
            foreach (var role in await _userManager.GetRolesAsync(user))
            {
                claim.Add(new Claim(ClaimTypes.Role, role));
            }

            int expiryInMinutes = 24 * 60;
            var token = GenerateAccessToken(claim, expiryInMinutes);

            return new LoginResultDto()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = DateTime.Now.AddMinutes(expiryInMinutes)
            };
        }

        public async Task GrantAdminRoleAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException("User not found.");

            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains("Admin"))
            {
                throw new ArgumentException("User already has admin role.");
            }

            await _userManager.AddToRoleAsync(user, "Admin");
        }

        public async Task RevokeRoleAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException("User not found.");

            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains("Admin"))
            {
                var admins = await _userManager.GetUsersInRoleAsync("Admin");
                if (admins.Count <= 1)
                    throw new ArgumentException("You cannot remove the last remaining Admin user.");
            }

            if (!roles.Any())
            {
                throw new ArgumentException("User has no roles.");
            }

            await _userManager.RemoveFromRolesAsync(user, roles);
        }

        private bool IsValidEmail(string email)
        {
            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
        }

        private JwtSecurityToken GenerateAccessToken(IEnumerable<Claim>? claims, int expiryInMinutes)
        {
            var signinKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));

            return new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Issuer,
                claims: claims?.ToArray(),
                expires: DateTime.Now.AddMinutes(expiryInMinutes),
                signingCredentials: new SigningCredentials(signinKey, SecurityAlgorithms.HmacSha256)
            );
        }
    }
}