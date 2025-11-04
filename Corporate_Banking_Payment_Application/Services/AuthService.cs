using Corporate_Banking_Payment_Application.DTOs;
using Corporate_Banking_Payment_Application.Models;
using Corporate_Banking_Payment_Application.Repository.IRepository;
using Corporate_Banking_Payment_Application.Services.IService;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace Corporate_Banking_Payment_Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepo;
        private readonly ICustomerRepository _customerRepo;
        private readonly IClientRepository _clientRepo;
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _httpClientFactory;

        public AuthService(IUserRepository userRepo, IConfiguration config, IHttpClientFactory httpClientFactory)
        public AuthService(IUserRepository userRepo, ICustomerRepository customerRepo,
        IClientRepository clientRepo, IConfiguration config)
        {
            _userRepo = userRepo;
            _customerRepo = customerRepo;
            _clientRepo = clientRepo;
            _config = config;
            _httpClientFactory = httpClientFactory;
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
            if (!await IsCaptchaValid(dto.RecaptchaToken))
            {
                throw new Exception("CAPTCHA validation failed. Please try again.");
            }

            var user = await _userRepo.GetByUserName(dto.UserName);
            if (user == null)
                throw new Exception("Invalid username or password.");

            if (!user.IsActive)
                throw new Exception("User account is inactive.");

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
                throw new Exception("Invalid username or password.");

            // Get optional customer & client (may be null)
            var customer = await _customerRepo.GetCustomerByUserId(user.UserId);
            Client? client = null;
            if (customer != null)
            {
                client = await _clientRepo.GetClientByCustomerId(customer.CustomerId);
            }

            //var token = GenerateJwtToken(user);
            //return new AuthResponseDto
            //{
            //    Token = token,
            //    UserName = user.UserName,
            //    Role = user.UserRole.ToString(),
            //    EmailId = user.EmailId,
            //    FullName = $"{user.FirstName} {user.LastName}"
            //};

            var token = GenerateJwtToken(user, client); // pass client to token generator

            return new AuthResponseDto
            {
                Token = token,
                UserName = user.UserName,
                Role = user.UserRole.ToString(),
                EmailId = user.EmailId,
                FullName = $"{user.FirstName} {user.LastName}",
                ClientId = client?.ClientId
            };
        }

        private async Task<bool> IsCaptchaValid(string token)
        {
            try
            {
                var secretKey = _config["GoogleReCaptcha:SecretKey"];
                var client = _httpClientFactory.CreateClient();

                // Send the token to Google's verification API
                var response = await client.PostAsync(
                    $"https://www.google.com/recaptcha/api/siteverify?secret={secretKey}&response={token}",
                    null
                );

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<RecaptchaResponseDto>(jsonString);
                    return result?.Success ?? false;
                }

                return false;
            }
            catch (Exception)
            {
                // If Google's API fails, block the login
                return false;
            }
        }

        private string GenerateJwtToken(User user)
        //private string GenerateJwtToken(User user)
        //{
        //    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        //    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        //    //var claims = new[]
        //    //{
        //    //    new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
        //    //    new Claim(JwtRegisteredClaimNames.Email, user.EmailId),
        //    //    new Claim("UserId", user.UserId.ToString()),
        //    //    new Claim(ClaimTypes.Role, user.UserRole.ToString())
        //    //};

        //    var claims = new List<Claim>
        //    {

        //        new Claim("userid", user.UserId.ToString()),
        //        new Claim("username", user.UserName),
        //        new Claim("email", user.EmailId),
        //        new Claim("role", user.UserRole.ToString()),
        //    };
        //    var tokenValidityMins = _config.GetValue<int>("Jwt:TokenValidityMins");

        //    var token = new JwtSecurityToken(
        //        issuer: _config["Jwt:Issuer"],
        //        audience: _config["Jwt:Audience"],
        //        claims: claims,
        //        //expires: DateTime.UtcNow.AddHours(3), 
        //        expires: DateTime.UtcNow.AddMinutes(tokenValidityMins),
        //        signingCredentials: creds
        //    );

        //    return new JwtSecurityTokenHandler().WriteToken(token);
        //}

        private string GenerateJwtToken(User user, Client? client = null)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
        {
            new Claim("userid", user.UserId.ToString()),
            new Claim("username", user.UserName),
            new Claim("email", user.EmailId),
            new Claim("role", user.UserRole.ToString())
        };

            // Add clientid claim only if user is linked to a client
            if (client != null)
            {
                claims.Add(new Claim("clientid", client.ClientId.ToString()));
            }

            var tokenValidityMins = _config.GetValue<int>("Jwt:TokenValidityMins");

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(tokenValidityMins),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
