using Store.Services.Dtos.AuthDtos;
using Store.Services.Dtos.UserDtos;
using Store.Services.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Services.Services.interfaces
{
    public interface IAuthService
    {
        Task<Result<UserDto>> RegisterAsync(RegisterDto dto);
        Task<Result<string>> LoginAsync(LoginDto dto);
    }
}
