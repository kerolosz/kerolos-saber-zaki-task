using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Products.Application.Commands;
using Products.Application.Responses;
using Products.Core.Entities;
using Products.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Products.Application.Handlers.Commands
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDTO>
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly IFileStorageService _fileStorageService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CreateProductCommandHandler(IProductRepository productRepository, IMapper mapper, IFileStorageService fileStorageService, IHttpContextAccessor httpContextAccessor)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _fileStorageService = fileStorageService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ProductDTO> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var ctx = _httpContextAccessor.HttpContext;
                var baseUrl = $"{ctx.Request.Scheme}://{ctx.Request.Host}";

                string? imageName = null;

                if (request.Image.Length > 0)
                {
                    using var stream = request.Image.OpenReadStream();
                    imageName = await _fileStorageService.SaveFileAsync(
                        stream,
                        request.Image.FileName,
                        "uploads/products"
                    );
                }

                var product = new Product
                {
                    Name = request.Name,
                    Description = request.Description,
                    OldPrice = request.OldPrice,
                    NewPrice = request.NewPrice,
                    ImagePath = imageName,
                    CreatedDate = DateTime.Now,
                    CreatedBy = request.CreatedBy
                };

                var result = await _productRepository.AddAsync(product);
                var productDTO = _mapper.Map<ProductDTO>(result);
                productDTO.ImagePath = string.IsNullOrEmpty(productDTO.ImagePath) ? null : $"{baseUrl}/uploads/products/{productDTO.ImagePath.Replace("\\", "/")}";
                return productDTO;
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                throw new ApplicationException("An error occurred while creating the product.", ex);
            }
        }
    }
}
