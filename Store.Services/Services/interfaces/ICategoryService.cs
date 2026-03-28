using Store.DataAccess.Helpers;
using Store.Services.Dtos.CateogryDtos;
using Store.Services.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Services.Services.interfaces
{
    public interface ICategoryService
    {
        Task<PagedResult<IEnumerable<CategoryDto>>> GetAllCategoriesAsync(CategoryQuery query);
        Task<Result<CategoryDto?>> GetCategoryByIdAsync(int id);
        Task<Result<CategoryDto?>> AddCategoryAsync(CategoryCreateDto dto);
        Task<Result> UpdateCategoryAsync(CategoryUpdateDto dto);
        Task<Result> DeleteCategoryAsync(int id);
        Task<bool> CategoryExistsAsync(int id);

    }
}
