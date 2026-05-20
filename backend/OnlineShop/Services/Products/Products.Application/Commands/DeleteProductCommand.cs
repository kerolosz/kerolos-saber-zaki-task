using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Products.Application.Commands
{
    public class DeleteProductCommand : IRequest<bool>
    {
        public int Id { get; set; }
        public DeleteProductCommand(int id)
        {
            this.Id = id;
        }
    }
}
