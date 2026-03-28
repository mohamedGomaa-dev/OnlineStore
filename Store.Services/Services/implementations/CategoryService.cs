using AutoMapper;
using Store.DataAccess.Helpers;
using Store.DataAccess.Units.interfaces;
using Store.Models.Entities;
using Store.Services.Dtos.CateogryDtos;
using Store.Services.Helpers;
using Store.Services.Services.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Services.Services.implementations
{
    public class CategoryService : ICategoryService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<CategoryDto?>> AddCategoryAsync(CategoryCreateDto dto)
        {
            // 1. we check that this category name does not exist in the categories
            if (await _unitOfWork.Categories.ExistsAsync(c => c.Name ==  dto.Name))
            {
                return Utility.Failure<CategoryDto?>($"category with name: {dto.Name} already exists");
            }

            var category = _mapper.Map<Category>(dto);
            await _unitOfWork.Categories.AddAsync(category);
            await _unitOfWork.CommitChanges();

            return Utility.Success<CategoryDto?>($"category added successfully", _mapper.Map<CategoryDto>(category));
        }

        public async Task<bool> CategoryExistsAsync(int id)
        {
            return await _unitOfWork.Categories.ExistsAsync(c => c.Id == id);
        }

        public async Task<Result> DeleteCategoryAsync(int id)
        {

            // make sure id is valid
            if (id <= 0)
            {
                return Utility.Failure("invalid id!");
            }
            var category = await _unitOfWork.Categories.GetAsync(c => c.Id == id);

            if (category is null)
            {
                return Utility.Failure($"category with id: {id}, not found");
            }
            
            _unitOfWork.Categories.Delete(category);
            await _unitOfWork.CommitChanges();
            return Utility.Success($"category with id: {id} deleted successfully");
        }

        public async Task<PagedResult<IEnumerable<CategoryDto>>> GetAllCategoriesAsync(CategoryQuery query)
        {
            var result = await _unitOfWork.Categories.GetCategoriesAsync(query);

            var categories = _mapper.Map<IEnumerable<CategoryDto>>(result.items.ToList());

            return Utility.SuccessPaged<IEnumerable<CategoryDto>>(categories, result.TotalCount, query.PageNumber, query.PageSize);
        }

        public async Task<Result<CategoryDto?>> GetCategoryByIdAsync(int id)
        {

            // make sure id is valid
            if (id <= 0)
            {
                return Utility.Failure<CategoryDto?>("invalid id!");
            }
            var category = await _unitOfWork.Categories.GetAsync(c => c.Id == id);
            if ( category is null )
            {
                return Utility.Failure<CategoryDto?>($"category with id: {id}, not found");
            }

            return Utility.Success<CategoryDto?>($"category found!", _mapper.Map<CategoryDto>(category));
        }

        public async Task<Result> UpdateCategoryAsync(CategoryUpdateDto dto)
        {

            // make sure id is valid
            if (dto.Id <= 0)
            {
                return Utility.Failure("invalid id!");
            }
            var category = await _unitOfWork.Categories.GetAsync(c => c.Id == dto.Id);
            // make sure category exists first
            if ( category is null )
            {
                return Utility.Failure($"category with id: {dto.Id} not found");
            }

            // make sure the new name of the category does not exist in the database as a name of another category
            if (await _unitOfWork.Categories.ExistsAsync(c => c.Name == dto.Name && c.Id != dto.Id))
            {
                return Utility.Failure($"can't update category with that name, name already exists");
            }

            _mapper.Map(dto, category);
            _unitOfWork.Categories.Update(category);
            await _unitOfWork.CommitChanges();
            return Utility.Success($"Category updated successfully!");
        }
    }
}
