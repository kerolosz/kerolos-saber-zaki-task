using AutoMapper;
using MediatR;
using Products.Application.Commands;
using Products.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Products.Application.Handlers.Commands
{
    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, bool>
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly IFileStorageService _fileStorageService;
        public DeleteProductCommandHandler(IProductRepository productRepository, IMapper mapper, IFileStorageService fileStorageService)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _fileStorageService = fileStorageService;
        }
        public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            try
            {

                var item = await _productRepository.GetByIdAsync(request.Id);

                if (item != null)
                {
                    var imagePath = $"uploads/products/{item.ImagePath}";
                    _fileStorageService.DeleteFile(imagePath);
                    await _productRepository.DeleteAsync(request.Id);
                    return true;
                }
                return false;

            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
