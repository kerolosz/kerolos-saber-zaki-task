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
    public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, List<ProductDTO>>
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetAllProductsQueryHandler(IProductRepository productRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<ProductDTO>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        {
            var ctx = _httpContextAccessor.HttpContext;
            var baseUrl = $"{ctx.Request.Scheme}://{ctx.Request.Host}";
            var products = await _productRepository.GetAllAsync();
            var ModifiedProducts = products.Select(p =>
            {
                p.ImagePath = $"{baseUrl}/uploads/products/{p.ImagePath}";
                return p;
            }).ToList();

            return _mapper.Map<List<ProductDTO>>(ModifiedProducts);
        }
    }
}
