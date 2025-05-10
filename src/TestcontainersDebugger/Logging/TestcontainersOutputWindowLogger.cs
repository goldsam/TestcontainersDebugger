using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace TestcontainersDebugger.Logging
{
    [Export(typeof(ILogger))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class TestcontainersOutputWindowLogger : ILogger, IDisposable
    {
        private readonly IVsOutputWindowPane _outputPane;
        private static Guid PaneGuid = new Guid("f20f6a79-3118-47eb-9607-6d7b9faca7be");

        [ImportingConstructor]
        public TestcontainersOutputWindowLogger([Import(typeof(SVsServiceProvider))] IServiceProvider serviceProvider)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            // Get the Output Window service
            IVsOutputWindow outputWindow = serviceProvider.GetService(typeof(SVsOutputWindow)) as IVsOutputWindow;
            if (outputWindow == null)
            {
                throw new InvalidOperationException("Unable to get the Output Window service.");
            }

            // Create a new pane for our custom logging, if it doesn't already exist
            outputWindow.CreatePane(ref PaneGuid, "Testcontainers Debugger", fInitVisible: 1, fClearWithSolution: 1);

            // Retrieve the pane
            outputWindow.GetPane(ref PaneGuid, out _outputPane);
        }

        public void Activate()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            _outputPane?.Activate();
        }

        public void LogInfo(string message)
        {
            Log(message, "INF");
        }

        public void LogError(string message)
        {
            Log(message, "ERR");
            
        }

        private const string format = "{0:yyyy-MM-dd HH:mm:ss.fff} [{1}] {2}";

        private void Log(string message, string level)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            string logMessage = String.Format(format, DateTime.Now, "ERROR", "An error occurred while processing the request.");
            _outputPane?.OutputStringThreadSafe($"{DateTime.Now}: {message}{Environment.NewLine}");
        }

        public void Dispose()
        {
            // Dispose logic if needed; typically not much is required for output panes
        }
    }
}
