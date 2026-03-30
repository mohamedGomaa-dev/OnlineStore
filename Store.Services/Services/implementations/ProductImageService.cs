using AutoMapper;
using Store.DataAccess.Data;
using Store.DataAccess.Units.interfaces;
using Store.Models.Entities;
using Store.Services.Dtos.ImageDtos;
using Store.Services.Helpers;
using Store.Services.Services.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Services.Services.implementations
{
    public class ProductImageService : IProductImageService
    {

        public ProductImageService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        private IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public async Task<Result<ProductImageDto>> AddProductImageAsync(int productId, string imageUrl, int imageOrder)
        {
            if (imageOrder <= 0 || await _unitOfWork.ProductImages.ExistsAsync(i => i.ImageOrder == imageOrder && i.ProductId == productId))
            {
                return Utility.Failure<ProductImageDto>("please enter a valid image order");
            }
            // make sure product id exists
            if (!await _unitOfWork.Products.ExistsAsync(p => p.Id == productId))
            {
                return Utility.Failure<ProductImageDto>("product not found");
            }

            
            var productImage = new ProductImage
            {
                ProductId = productId,
                ImageURL = imageUrl,
                ImageOrder = imageOrder
            };
            await _unitOfWork.ProductImages.AddAsync(productImage);
            await _unitOfWork.CommitChanges();

            return Utility.Success("image added successfully", _mapper.Map<ProductImageDto>(productImage));
        }

        public async Task<Result<string>> DeleteProductImageAsync(int imageId)
        {
           var productImage = await _unitOfWork.ProductImages.GetAsync(i => i.Id ==  imageId);
           if (productImage is null)
            {
                return Utility.Failure<string>($"image with id: {imageId} not found");
            }

            _unitOfWork.ProductImages.Delete(productImage);
            await _unitOfWork.CommitChanges();
            return Utility.Success("image deleted successfully", productImage.ImageURL);
        }

        public async Task<Result<IEnumerable<ProductImageDto>>> GetProductImagesAsync(int productId)
        {

            // check if product id exists first
            if (!await _unitOfWork.Products.ExistsAsync(p => p.Id == productId))
            {
                return Utility.Failure<IEnumerable<ProductImageDto>>($"product with id: {productId} doesn't exist");
            }

            var images = await _unitOfWork.ProductImages.GetImagesByProductIdAsync(productId);
            return Utility.Success<IEnumerable<ProductImageDto>>($"sucess", _mapper.Map<IEnumerable<ProductImageDto>>(images));
        }
    }
}
