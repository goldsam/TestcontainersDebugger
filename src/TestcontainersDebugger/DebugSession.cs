using Grpc.Core;
using System;
using System.Threading.Tasks;
using TestcontainersDebugger.Contracts;
using TestcontainersDebugger.Logging;

class DebugSession : IDisposable
{
    private readonly SessionContext _sessionContext;
    private readonly ILogger _logger;
    private readonly IAsyncStreamReader<Request> _requestStream;
    private readonly IServerStreamWriter<Response> _responseStream;
    private readonly ServerCallContext _context;

    public DebugSession(
        SessionContext sessionContext, 
        ILogger logger,
        IAsyncStreamReader<Request> requestStream, 
        IServerStreamWriter<Response> responseStream, 
        ServerCallContext context)
    {
        _sessionContext = sessionContext;
        _logger = logger;   
        _requestStream = requestStream;
        _responseStream = responseStream;
        _context = context;
    }

    public void Dispose()
    {
    }

    public Task Start()
    {
        return Task.CompletedTask;

    }

}