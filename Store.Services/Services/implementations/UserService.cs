using AutoMapper;
using Store.DataAccess.Helpers;
using Store.DataAccess.Units.interfaces;
using Store.Models.Entities;
using Store.Services.Dtos.UserDtos;
using Store.Services.Helpers;
using Store.Services.Services.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Services.Services.implementations
{
    public class UserService : IUserService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<UserDto>> AddUserAsync(UserCreateDto dto, string plainPassword)
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
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(plainPassword);
            await _unitOfWork.Users.AddAsync(user);

            await _unitOfWork.CommitChanges();

            return Utility.Success("user created successfully", _mapper.Map<UserDto>(user));
        }

        public async Task<Result> DeleteUserAsync(int id)
        {
            // get user from db
            var user = await _unitOfWork.Users.GetAsync(u => u.Id == id);
            
            // if null it is not found so we don't proceed to delete
            if (user is null)
            {
                return Utility.Failure($"user with id: {id} not found");
            }

            // found the user to delete, proceed to complete the deletion
            _unitOfWork.Users.Delete(user);
            await _unitOfWork.CommitChanges();
            return Utility.Success("user deleted successfully");
        }

        public async Task<PagedResult<ICollection<UserDto>>> GetAllUsersAsync(UserQuery query)
        {
            var result = await _unitOfWork.Users.GetUsersAsync(query);

            var users = _mapper.Map<ICollection<UserDto>>(result.items.ToList());

            return Utility.SuccessPaged(users, result.TotalCount, query.PageNumber, query.PageSize);
        }

        public async Task<Result<UserDto?>> GetUserByIdAsync(int id)
        {
            if (id <= 0)
            {
                return Utility.Failure<UserDto?>($"Enter a valid ID");
            }
            var user = await _unitOfWork.Users.GetAsync(u => u.Id == id);

            if (user is null)
            {
                return Utility.Failure<UserDto?>($"user with id: {id} not found");
            }

            return Utility.Success<UserDto?>("success", _mapper.Map<UserDto>(user));
        }

      

        public async Task<Result> UpdateUserAsync(UserUpdateDto dto)
        {
            // check if user exists to update

            var user = await _unitOfWork.Users.GetAsync(u => u.Id == dto.Id);
            if (user is null)
            {
                return Utility.Failure($"user with id: {dto.Id} not found");
            }
            // check if the email already exists for another user
            if (await _unitOfWork.Users.ExistsAsync(u => u.Email == dto.Email
                                                    && u.Id != dto.Id))
            {
                return Utility.Failure("email already exists");
            }
            // check if the username in use for another user
            if (await _unitOfWork.Users.ExistsAsync(u => u.Username == dto.Username 
                                                    && u.Id != dto.Id))
            {
                return Utility.Failure("Username already exists");
            }


            _mapper.Map(dto, user);
            _unitOfWork.Users.Update(user);
            await _unitOfWork.CommitChanges();
            return Utility.Success($"user updated successfully!");
        }

        public async Task<bool> UserExistsAsync(int id)
        {
            return await _unitOfWork.Users.ExistsAsync(u => u.Id == id);
        }
    }
}
