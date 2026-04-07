using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Store.DataAccess.Units.interfaces;
using Store.Models.Entities;
using Store.Services.Dtos.AuthDtos;
using Store.Services.Dtos.UserDtos;
using Store.Services.Helpers;
using Store.Services.Services.interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Utility = Store.Services.Helpers.Utility;

namespace Store.Services.Services.implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;


        public AuthService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _config = configuration;
        }
        public async Task<Result<string>> LoginAsync(LoginDto dto)
        {
            var user = await _unitOfWork.Users.GetAsync(u => u.Email ==  dto.Email);
            if (user is null)
            {
                return Utility.Failure<string>($"invalid email or password!");
            }
            bool isPasswordCorrect = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
            if (!isPasswordCorrect)
            {
                return Utility.Failure<string>($"invalid email or password!");
            }
            string token = GenerateJwtToken(user);
            return Utility.Success("logged in successfully!",token);
        }

        public async Task<Result<UserDto>> RegisterAsync(RegisterDto dto)
        {
            // check if the email already exists
            if (await _unitOfWork.Users.ExistsAsync(u => u.Email == dto.Email))
            {
                return Utility.Failure<UserDto>("email already exists");
            }
            // check if the username in use
            if (await _unitOfWork.Users.ExistsAsync(u => u.Username == dto.Username))
            {
                return Utility.Failure<UserDto>("Username already exists");
            }
            // map dto to user to be able to pass it to the unit of work
            var user = _mapper.Map<User>(dto);
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            await _unitOfWork.Users.AddAsync(user);

            await _unitOfWork.CommitChanges();
            return Utility.Success("user created successfully", _mapper.Map<UserDto>(user));

        }

        private string GenerateJwtToken(User user)
        {
            // 1. تحديد البيانات التي ستكون داخل التوكن (Claims)
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Name, user.Username),
        new Claim(ClaimTypes.Role, user.Role.ToString()) // 👈 هنا نضع صلاحية المستخدم
    };
            var secretKey = _config["JWT_SECRET_KEY"];

            if (string.IsNullOrEmpty(secretKey))
            {
                throw new InvalidOperationException("JWT Secret Key is missing!");
            }
            // 2. إنشاء مفتاح التشفير السري (مؤقتاً نضعه هنا، لاحقاً سننقله لـ appsettings.json)
            // يجب أن يكون المفتاح طويلاً ومعقداً (على الأقل 32 حرف)
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            // 3. اختيار خوارزمية التشفير
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 4. تجميع التوكن (تحديد متى ينتهي، مثلاً بعد 7 أيام)
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = creds
            };

            // 5. إنشاء التوكن وإرجاعه كنص
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
