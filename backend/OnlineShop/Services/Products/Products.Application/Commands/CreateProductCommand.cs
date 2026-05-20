using MediatR;
using Microsoft.AspNetCore.Http;
using Products.Application.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Products.Application.Commands
{
    public class CreateProductCommand : IRequest<ProductDTO>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal OldPrice { get; set; }
        public decimal NewPrice { get; set; }
        public IFormFile? Image { get; set; }
        public string CreatedBy { get; set; }


    }
}
