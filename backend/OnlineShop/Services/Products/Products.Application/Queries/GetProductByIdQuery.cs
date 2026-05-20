using MediatR;
using Products.Application.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Products.Application.Queries
{
    public class GetProductByIdQuery : IRequest<ProductDTO>
    {
        public GetProductByIdQuery(int id)
        {
            this.Id = id;
        }
        public int Id { get; set; }

    }
}
