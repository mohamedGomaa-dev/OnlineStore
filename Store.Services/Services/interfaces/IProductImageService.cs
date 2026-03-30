using Store.Services.Dtos.ImageDtos;
using Store.Services.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Services.Services.interfaces
{
    public interface IProductImageService
    {
        Task<Result<ProductImageDto>> AddProductImageAsync(int productId, string imageUrl, int imageOrder);
        Task<Result<string>> DeleteProductImageAsync(int imageId);
        Task<Result<IEnumerable<ProductImageDto>>> GetProductImagesAsync(int productId);
    }
}
