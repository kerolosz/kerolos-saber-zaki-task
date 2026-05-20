using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Products.Application.Commands;
using Products.Application.Queries;
using Products.Application.Responses;

namespace Products.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IMediator mediator, ILogger<ProductController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet("GetAllProducts")]
       
        public async Task<IActionResult> GetAllProducts()
        {
           
            var command = new GetAllProductsQuery();

            var result = await _mediator.Send<List<ProductDTO>>(command);

            return Ok(Response<List<ProductDTO>>.SuccessResponse(result));
        }

        [HttpGet("GetProductById")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var command = new GetProductByIdQuery(id);

            var result = await _mediator.Send<ProductDTO>(command);
            if (result == null)
            {
                return Ok(Response<ProductDTO>.Fail("Product not found", statusCode: 404));

            }
            return Ok(Response<ProductDTO>.SuccessResponse(result));
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] CreateProductCommand command)
        {

            var result = await _mediator.Send<ProductDTO>(command);
            if (result == null)
            {
                return Ok(Response<ProductDTO>.Fail("Error Occureed", statusCode: 404));

            }
            return Ok(Response<ProductDTO>.SuccessResponse(result));
        }
        [HttpPut]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Update([FromForm] UpdateProductCommand command)
        {

            var result = await _mediator.Send<ProductDTO>(command);
            if (result == null)
            {
                return Ok(Response<ProductDTO>.Fail("Error Occureed", statusCode: 404));

            }
            return Ok(Response<ProductDTO>.SuccessResponse(result));
        }
        [HttpDelete("DeleteProduct/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var cmd = new DeleteProductCommand(id);
            var result = await _mediator.Send<bool>(cmd);
            if (result == false)
            {
                return Ok(Response<bool>.Fail("Error Occureed", statusCode: 404));

            }
            return Ok(Response<bool>.SuccessResponse(result));
        }
    }
}
