using System;

namespace TestcontainersDebugger.Logging
{
    public interface ILogger
    {
        void LogInfo(string message);

        void LogError(string message);
    }
}
