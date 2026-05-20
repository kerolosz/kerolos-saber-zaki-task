using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Products.Application.Responses
{
    public class Response<T>
    {
        public bool Success { get; init; }
        public T? Data { get; init; }
        public string? Message { get; init; }
        public IEnumerable<string>? Errors { get; init; }
        public int StatusCode { get; init; }

        public static Response<T> SuccessResponse(T data, string? message = null, int statusCode = 200) =>
            new Response<T>
            {
                Success = true,
                Data = data,
                Message = message,
                StatusCode = statusCode
            };

        public static Response<T> Fail(string message, IEnumerable<string>? errors = null, int statusCode = 400) =>
            new Response<T>
            {
                Success = false,
                Message = message,
                Errors = errors,
                StatusCode = statusCode
            };
    }
}
