using HA_Ossooll.Data.DTOs;
using HA_Ossooll.Data.Models;
using HA_Ossooll.Services.IService;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HA_Ossooll.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthService(UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<AuthModel> RegisterAsync(RegisterModel model)
        {
            if (await _userManager.FindByEmailAsync(model.Email) != null)
                return new AuthModel { Message = "Email already exists ❌" };

            if (await _userManager.FindByNameAsync(model.Username) != null)
                return new AuthModel { Message = "Username already exists ❌" };

            var user = new ApplicationUser
            {
                Email = model.Email,
                UserName = model.Username,
                FirstName = model.FirstName,
                LastName = model.LastName
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return new AuthModel { Message = errors };
            }

            return new AuthModel
            {
                IsAuthenticated = true,
                Email = user.Email,
                Username = user.UserName,
                Message = "User registered successfully ✅"
            };
        }

        public async Task<AuthModel> LoginAsync(TokenRequestModel model)
        {
            var authModel = new AuthModel();

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                authModel.Message = "Email or Password is incorrect ❌";
                return authModel;
            }

            var jwtToken = CreateJwtToken(user);

            authModel.IsAuthenticated = true;
            authModel.Email = user.Email;
            authModel.Username = user.UserName;
            authModel.Message = "Login successful ✅";
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            authModel.ExpiresOn = jwtToken.ValidTo;

            return authModel;
        }

        private JwtSecurityToken CreateJwtToken(ApplicationUser user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var jwtToken = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(double.Parse(_configuration["JWT:DurationInDays"])),
                signingCredentials: creds
            );

            return jwtToken;
        }
    }
}