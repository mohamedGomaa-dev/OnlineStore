using AutoMapper;
using Store.DataAccess.Helpers;
using Store.DataAccess.Units.interfaces;
using Store.Models.Entities;
using Store.Services.Dtos.ProductDtos;
using Store.Services.Helpers;
using Store.Services.Services.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Services.Services.implementations
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<ProductDto>> AddProductAsync(ProductCreateDto dto)
        {
            // check if category exists
            if (!await _unitOfWork.Categories.ExistsAsync(c => c.Id == dto.CategoryId))
            {
                return Utility.Failure<ProductDto>("please enter a valid category id");
            }
            // check if required fields like name and description are not empty
            if (string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.Description))
            {
                // it's better to be percise about what field is required, but for the sake of simplicity I combined them together
                return Utility.Failure<ProductDto>("Some fields are required, please enter their value");
            }

            // the check of price and stock quantity will be handled by the dto annotations


            // map the product and add it to db 
            var product = _mapper.Map<Product>(dto);
            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.CommitChanges();
            return Utility.Success<ProductDto>("product added succesffuly", _mapper.Map<ProductDto>(product));
        }

        public async Task<Result> DeleteProductAsync(int id)
        {
            // get the product you want to delete from db
            var product = await _unitOfWork.Products.GetAsync(p =>  p.Id == id);

            // check if product exist
            if (product is null) {
                return Utility.Failure($"product with id: {id} not found");
            }

            // delete the product if found
            _unitOfWork.Products.Delete(product);
            await _unitOfWork.CommitChanges();
            return Utility.Success($"product with id: {id} deleted successfully");
        }

        public async Task<PagedResult<IEnumerable<ProductDto>>> GetAllProductsAsync(ProductQuery query)
        {
            // get the products with its total count (size of the list)
            var result = await _unitOfWork.Products.GetProductsAsync(query);
            // map the products from the result to a dto to pass it to the controller
            var products = _mapper.Map<IEnumerable<ProductDto>>(result.Items);
            return Utility.SuccessPaged<IEnumerable<ProductDto>>(products, result.TotalCount, query.PageNumber, query.PageSize);
        }

        public async Task<Result<ProductDto?>> GetProductByIdAsync(int id)
        {
            var product = await _unitOfWork.Products.GetAsync(p => p.Id == id, "Category", "ProductImages");
            // check if product doesn't exist
            if (product == null)
            {
                return Utility.Failure<ProductDto?>($"Product with id: {id}, not found");
            }
            
            return Utility.Success<ProductDto?>("product found!", _mapper.Map<ProductDto>(product));
        }

        public async Task<bool> ProductExistsAsync(int id)
        {
            return await _unitOfWork.Products.ExistsAsync(p => p.Id == id);
        }

        public async Task<Result> UpdateProductAsync(ProductUpdateDto dto)
        {

            // check if product exists
            var existingProduct = await _unitOfWork.Products.GetAsync(p => p.Id == dto.Id);
            if (existingProduct == null) return Utility.Failure("Product not found");
            // check if category exists
            if (!await _unitOfWork.Categories.ExistsAsync(c => c.Id == dto.CategoryId))
            {
                return Utility.Failure("please enter a valid category id");
            }
            // check if required fields like name and description are not empty
            if (string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.Description))
            {
                // it's better to be percise about what field is required, but for the sake of simplicity I combined them together
                return Utility.Failure("Some fields are required, please enter their value");
            }

            // I don't think that we should the name unique because some products will be the same name from different seller?

            var product = _mapper.Map(dto, existingProduct);
            _unitOfWork.Products.Update(existingProduct);
            await _unitOfWork.CommitChanges();
            return Utility.Success($"product with id: {dto.Id}, updaetd successfully");
        }
    }
}
