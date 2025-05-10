using Grpc.Core;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using TestcontainersDebugger.Contracts;
using System.Threading;
using TestcontainersDebugger.Logging;
using Microsoft.VisualStudio.Shell;
using System;
using System.Diagnostics;

[Export(typeof(TestcontainersDebuggingHandler))]
[PartCreationPolicy(CreationPolicy.Shared)]
public class TestcontainersDebuggingHandler : TestcontainersDebugging.TestcontainersDebuggingBase, IDisposable 
{
    private readonly ILogger _logger;
    
    int _isHandlingClient = 0;

    int _nextSessionId = 0;
    
    [ImportingConstructor]
    public TestcontainersDebuggingHandler([Import(typeof(ILogger))] ILogger logger)
    {
        _logger = logger;
    }

    public override async Task DebugSession(IAsyncStreamReader<Request> requestStream, IServerStreamWriter<Response> responseStream, ServerCallContext context)
    {
        var sessionContext = new SessionContext()
        {
            HostProcessId = Process.GetCurrentProcess().Id,
            ClientProcessId = -1,
            SessionId = -1
        };

        _logger.LogInfo("Debug session channel opening...");

        int nonce = -1;

        bool nonceReceived = false;
        if (await requestStream.MoveNext() && requestStream.Current.RequestCase == Request.RequestOneofCase.HandshakeRequested)
        {
            // Extract the nonce and client process id from the handshake request
            var handshakeRequested = requestStream.Current.HandshakeRequested;
            nonce = handshakeRequested.Nonce;
            sessionContext.ClientProcessId = handshakeRequested.ClientProcessId;
            nonceReceived = true;

            // Provision a new session id if we are not already handling a client
            if (Interlocked.CompareExchange(ref _isHandlingClient, 1, 0) == 0)
                sessionContext.SessionId = Interlocked.Increment(ref _nextSessionId);
        }

        var response = new Response();

        if (!nonceReceived)
        {
            _logger.LogError($"Did not receive handshake request. Rejecting debug session request. (Nonce={nonce}).");
            response.HandshakeRejected = new HandshakeRejectedResponsePayload()
            {
                Reason = "Did not received expected handshake request.",
                Nonce = nonce,
                SessionContext = sessionContext
            };
        }
        else if (sessionContext.SessionId < 0)
        {
            _logger.LogError($"Did not receive handshake request. Rejecting debug session request. (Nonce={nonce}).");
            response.HandshakeRejected = new HandshakeRejectedResponsePayload()
            {
                Reason = $"Session {sessionContext.SessionId} was already being handled.",
                Nonce = nonce,
                SessionContext = sessionContext
            };
        }
        else
        {
            response.HandshakeAccepted = new HandshakeAcceptedResponsePayload()
            {
                Nonce = nonce,
                SessionContext = sessionContext
            };

            _logger.LogInfo("Successfully opened debug session (SessionId={sessionContext.SessionId}, Nonce={nonce}).");

            using var debugSession = new DebugSession(sessionContext, _logger, requestStream, responseStream, context);
            await debugSession.Start();
        }
        ///await responseStream.WriteAsync(
            
        //    try
        //    {
        //            
        //        }
        //        finally
        //        {
        //            Interlocked.Exchange(ref _isHandlingClient, 0);
        //        }
        //    }
        //}
        //else
        //{
        //    _logger.LogError($"Did not receive handshake request. Rejecting debug session request (Nonce={nonce}).");
        //    await responseStream.WriteAsync(new Response
        //    {
        //        HandshakeRejected = new HandshakeRejectedResponsePayload()
        //        {
        //            Reason = "Did not received expected handshake request.",
        //            Nonce = nonce,
        //            SessionContext = sessionContext
        //        }
        //    });

        //    return;
        //}
    }
}
