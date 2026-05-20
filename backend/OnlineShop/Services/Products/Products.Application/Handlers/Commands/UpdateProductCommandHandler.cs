using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Products.Application.Commands;
using Products.Application.Responses;
using Products.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Products.Application.Handlers.Commands
{
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ProductDTO>
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly IFileStorageService _fileStorageService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UpdateProductCommandHandler(IProductRepository productRepository, IMapper mapper, IFileStorageService fileStorageService, IHttpContextAccessor httpContextAccessor)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _fileStorageService = fileStorageService;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<ProductDTO> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var ctx = _httpContextAccessor.HttpContext;
                var baseUrl = $"{ctx.Request.Scheme}://{ctx.Request.Host}";

                var olditem = await _productRepository.GetByIdAsync(request.Id);

                string? imageName = null;

                if (request.Image is not null && request.Image.Length > 0)
                {
                    if (olditem != null)
                    {
                        var imagePath = $"uploads/products/{olditem.ImagePath}";
                        _fileStorageService.DeleteFile(imagePath);

                    }
                    using var stream = request.Image.OpenReadStream();
                    imageName = await _fileStorageService.SaveFileAsync(
                        stream,
                        request.Image.FileName,
                        "uploads/products"
                    );
                }



                olditem.Name = request.Name;
                olditem.Description = request.Description;
                olditem.OldPrice = request.OldPrice;
                olditem.NewPrice = request.NewPrice;
                olditem.ImagePath = imageName ?? olditem.ImagePath;
                olditem.UpdatedDate = DateTime.Now;
                olditem.UpdatedBy = request.UpdatedBy;





                await _productRepository.UpdateAsync(olditem);
                var productDTO = _mapper.Map<ProductDTO>(olditem);
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
