using Store.DataAccess.Helpers;
using Store.Services.Dtos.ProductDtos;
using Store.Services.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Services.Services.interfaces
{
    public interface IProductService
    {
        Task<Result<ProductDto?>> GetProductByIdAsync(int id);
        Task<PagedResult<IEnumerable<ProductDto>>> GetAllProductsAsync(ProductQuery query);

        Task<Result<ProductDto>> AddProductAsync(ProductCreateDto dto);

        Task<Result> UpdateProductAsync(ProductUpdateDto dto);
        Task<Result> DeleteProductAsync(int id);
        Task<bool> ProductExistsAsync(int id);
    }
}
