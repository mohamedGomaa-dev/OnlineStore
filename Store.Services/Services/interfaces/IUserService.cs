using Store.DataAccess.Helpers;
using Store.Models.Entities;
using Store.Services.Dtos.UserDtos;
using Store.Services.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Services.Services.interfaces
{
    public interface IUserService
    {
        Task<Result<UserDto?>> GetUserByIdAsync(int id);
        Task<PagedResult<ICollection<UserDto>>> GetAllUsersAsync(UserQuery query);
        Task<Result<UserDto>> AddUserAsync(UserCreateDto dto, string plainPassword);
        Task<Result> UpdateUserAsync(UserUpdateDto dto);
        Task<Result> DeleteUserAsync(int id);
        Task<bool> UserExistsAsync(int id);
    }
}
