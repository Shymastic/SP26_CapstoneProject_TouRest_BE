using System.Net;
using System.Text.Json;
using TouRest.Api.Common;
using TouRest.Application.Common.Constants;
using TouRest.Application.Common.Exceptions;

namespace TouRest.Api.Middlewares
{
    public class GlobalExceptionHandler
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(
            RequestDelegate next,
            ILogger<GlobalExceptionHandler> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (NotFoundException ex)
            {
                await HandleException(
                    context,
                    RespCode.NOT_FOUND,
                    ex.Message
                );
            }
            catch (UnauthorizedAccessException ex)
            {
                await HandleException(
                    context,
                    RespCode.UNAUTHORIZED,
                    ex.Message ?? RespMsg.UNAUTHORIZED
                );
            }
            catch (InvalidOperationException ex)
            {
                await HandleException(
                    context,
                    RespCode.BAD_REQUEST,
                    ex.Message ?? RespMsg.BAD_REQUEST
                );
            }
            catch (KeyNotFoundException ex)
            {
                await HandleException(
                    context,
                    RespCode.NOT_FOUND, 
                    ex.Message          
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                await HandleException(
                    context,
                    RespCode.INTERNAL_SERVER_ERROR,
                    RespMsg.INTERNAL_SERVER_ERROR
                );
            }
        }

        private static async Task HandleException(
            HttpContext context,
            int statusCode,
            string message)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var response = new ApiResponse<object?>
            {
                Code = statusCode,
                Message = message,
                Data = null
            };

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(response)
            );
        }
    }
}
