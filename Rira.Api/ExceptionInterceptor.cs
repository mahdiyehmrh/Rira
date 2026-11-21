using Grpc.Core;
using Grpc.Core.Interceptors;
using Rira.Application.Common.Exceptions;
using Rira.Domain.Common;

namespace Rira.Api;

public sealed class ExceptionInterceptor : Interceptor
{
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            return await continuation(request, context);
        }
        catch (OperationCanceledException)
        {
            throw new RpcException(new Status(StatusCode.Cancelled, "Request was cancelled."));
        }
        catch (DomainException exception)
        {
            var message = string.Join("; ", exception.Errors.Select(error => error.Message));
            throw new RpcException(new Status(StatusCode.InvalidArgument, message));
        }
        catch (NotFoundException exception)
        {
            throw new RpcException(new Status(StatusCode.NotFound, exception.Message));
        }
        catch (ConflictException exception)
        {
            throw new RpcException(new Status(StatusCode.AlreadyExists, exception.Message));
        }
        catch (Exception)
        {
            throw new RpcException(new Status(StatusCode.Internal, "An unexpected server error occurred."));
        }
    }
}
