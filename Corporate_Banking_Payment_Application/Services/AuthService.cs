﻿using Corporate_Banking_Payment_Application.DTOs;
using Corporate_Banking_Payment_Application.Models;
using Corporate_Banking_Payment_Application.Repository.IRepository;
using Corporate_Banking_Payment_Application.Services.IService;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Corporate_Banking_Payment_Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepo;
        private readonly IConfiguration _config;

        public AuthService(IUserRepository userRepo, IConfiguration config)
        {
            _userRepo = userRepo;
            _config = config;
        }

        public async Task<AuthResponseDto> Register(RegisterDto dto)
        {
            // Check for existing username or email
            if (await _userRepo.GetByUserName(dto.UserName) != null)
                throw new Exception("Username already exists.");

            if (await _userRepo.GetByEmail(dto.EmailId) != null)
                throw new Exception("Email already registered.");

            // Hash password using BCrypt
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var newUser = new User
            {
                UserName = dto.UserName,
                Password = hashedPassword,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                EmailId = dto.EmailId,
                PhoneNumber = dto.PhoneNumber,
                Address = dto.Address,
                UserRole = dto.UserRole,
                IsActive = true
            };

            await _userRepo.AddUser(newUser);

            var token = GenerateJwtToken(newUser);
            return new AuthResponseDto
            {
                Token = token,
                UserName = newUser.UserName,
                Role = newUser.UserRole.ToString(),
                EmailId = newUser.EmailId,
                FullName = $"{newUser.FirstName} {newUser.LastName}"
            };
        }

        public async Task<AuthResponseDto> Login(LoginDto dto)
        {
            var user = await _userRepo.GetByUserName(dto.UserName);
            if (user == null)
                throw new Exception("Invalid username or password.");

            if (!user.IsActive)
                throw new Exception("User account is inactive.");

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
                throw new Exception("Invalid username or password.");

            var token = GenerateJwtToken(user);
            return new AuthResponseDto
            {
                Token = token,
                UserName = user.UserName,
                Role = user.UserRole.ToString(),
                EmailId = user.EmailId,
                FullName = $"{user.FirstName} {user.LastName}"
            };
        }

        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            //var claims = new[]
            //{
            //    new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
            //    new Claim(JwtRegisteredClaimNames.Email, user.EmailId),
            //    new Claim("UserId", user.UserId.ToString()),
            //    new Claim(ClaimTypes.Role, user.UserRole.ToString())
            //};

            var claims = new List<Claim>
            {

                new Claim("userid", user.UserId.ToString()),
                new Claim("username", user.UserName),
                new Claim("email", user.EmailId),
                new Claim("role", user.UserRole.ToString()),
            };
            var tokenValidityMins = _config.GetValue<int>("Jwt:TokenValidityMins");

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                //expires: DateTime.UtcNow.AddHours(3), 
                expires: DateTime.UtcNow.AddMinutes(tokenValidityMins),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
