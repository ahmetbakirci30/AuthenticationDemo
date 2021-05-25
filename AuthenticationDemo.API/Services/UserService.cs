using AuthenticationDemo.API.Models;
using AuthenticationDemo.API.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationDemo.API.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;

        public UserService(UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<UserManagerResponse> Login(LoginViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
                return new UserManagerResponse
                {
                    Succeeded = false,
                    Message = $"An account with the email '{model.Email}' was not found."
                };

            if (!await _userManager.CheckPasswordAsync(user, model.Password))
                return new UserManagerResponse
                {
                    Succeeded = false,
                    Message = "The password is incorrect!"
                };

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["AuthSettings:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["AuthSettings:Issuer"],
                audience: _configuration["AuthSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(30),
                signingCredentials: credentials);

            var tokenAsString = new JwtSecurityTokenHandler().WriteToken(token);

            return new UserManagerResponse
            {
                Succeeded = true,
                Message = tokenAsString
            };
        }

        public async Task<UserManagerResponse> Register(RegisterViewModel model)
        {
            var user = new IdentityUser
            {
                Email = model.Email,
                UserName = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            return new UserManagerResponse
            {
                Succeeded = result.Succeeded,
                Message = result.Succeeded ? "Registration has been successfully!" :
                    result.Errors.Select(e => e.Description).FirstOrDefault()
            };
        }

        public async Task<(bool succeeded, string message)> RegisterAsync(RegisterViewModel model)
        {
            var result = await _userManager.CreateAsync(new IdentityUser
            {
                Email = model.Email,
                UserName = model.Email
            }, model.Password);

            return (result.Succeeded, result.Succeeded ? "Registration has been successfully!" :
                result.Errors.Select(e => e.Description).FirstOrDefault());
        }

        public async Task<(bool succeeded, string message)> LoginAsync(LoginViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
                return (false, $"An account with the email '{model.Email}' was not found.");

            if (!await _userManager.CheckPasswordAsync(user, model.Password))
                return (false, "The password is incorrect!");

            var claims = new[]
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["AuthSettings:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["AuthSettings:Issuer"],
                audience: _configuration["AuthSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(30),
                signingCredentials: credentials);

            var tokenAsString = new JwtSecurityTokenHandler().WriteToken(token);

            return (true, tokenAsString);
        }
    }
}