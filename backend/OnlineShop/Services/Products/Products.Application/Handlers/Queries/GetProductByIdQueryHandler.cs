using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Products.Application.Queries;
using Products.Application.Responses;
using Products.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Products.Application.Handlers.Queries
{
    public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDTO>
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetProductByIdQueryHandler(IProductRepository productRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<ProductDTO> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var ctx = _httpContextAccessor.HttpContext;
            var baseUrl = $"{ctx.Request.Scheme}://{ctx.Request.Host}";
            var product = await _productRepository.GetByIdAsync(request.Id);
            product.ImagePath = $"{baseUrl}/uploads/products/{product.ImagePath}";



            return _mapper.Map<ProductDTO>(product);
        }
    }
}
