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

        public void Log(string message)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
             _outputPane?.OutputStringThreadSafe($"{DateTime.Now}: {message}{Environment.NewLine}");
        }

        public void Dispose()
        {
            // Dispose logic if needed; typically not much is required for output panes
        }
    }

    //internal sealed class AttachToContainerCommand
    //{
    //    public const int CommandId = 0x0100;
    //    public static readonly Guid CommandSet = new Guid("PUT-ANOTHER-GUID-HERE"); // Replace with your own GUID
    //    private readonly AsyncPackage package;
    //    private DTE2 _dte;

    //    private AttachToContainerCommand(AsyncPackage package, OleMenuCommandService commandService)
    //    {
    //        this.package = package ?? throw new ArgumentNullException(nameof(package));

    //        var menuCommandID = new CommandID(CommandSet, CommandId);
    //        var menuItem = new MenuCommand(this.Execute, menuCommandID);
    //        commandService.AddCommand(menuItem);
    //    }

    //    public static async Task InitializeAsync(AsyncPackage package)
    //    {
    //        // Switch to the UI thread to access DTE and add our command.
    //        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
    //        var commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
    //        new AttachToContainerCommand(package, commandService);
    //    }

    //    private void Execute(object sender, EventArgs e)
    //    {
    //        ThreadHelper.ThrowIfNotOnUIThread();

    //        // Retrieve the DTE2 instance.
    //        _dte = (DTE2)Package.GetGlobalService(typeof(DTE));
    //        if (_dte == null)
    //        {
    //            throw new Exception("DTE service not found.");
    //            //System.Windows.Forms.MessageBox.Show("DTE service not found.");
    //            //return;
    //        }

    //        // Run the Docker query and attach logic on a background thread.
    //        Task.Run(async () =>
    //        {
    //            // Connect to the local Docker engine.
    //            DockerClient dockerClient = new DockerClientConfiguration(
    //                new Uri("npipe://./pipe/docker_engine")).CreateClient();

    //            IList<ContainerListResponse> containers = await dockerClient.Containers.ListContainersAsync(
    //                new ContainersListParameters() { All = true }
    //            );

    //            // Filter for containers launched by Testcontainers (assuming they have a specific label).
    //            var testContainers = containers
    //                .Where(c => c.Labels != null && c.Labels.ContainsKey("org.testcontainers"))
    //                .ToList();

    //            if (!testContainers.Any())
    //            {
    //                throw new Exception("No testcontainers found.");
    //                //System.Windows.Forms.MessageBox.Show("No testcontainers found.");
    //                //return;
    //            }

    //            // For this example, we attach to the first test container.
    //            // In practice, you might loop through multiple containers or provide a UI to select one.
    //            var container = testContainers.First();

    //            // Assume the container is set up for remote debugging on port 4020.
    //            int remoteDebuggerPort = 4020;
    //            string qualifier = "localhost:" + remoteDebuggerPort;

    //            // Use the UI thread for debugger operations.
    //            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
    //            AttachToRemoteProcess(qualifier);
    //        });
    //    }

    //    /// <summary>
    //    /// This method demonstrates a way to attach to a remote process.
    //    /// In a real-world scenario, you may need to use IVsDebugger interfaces for robust remote debugging.
    //    /// </summary>
    //    /// <param name="qualifier">The remote machine qualifier (for example, "localhost:4020").</param>
    //    private void AttachToRemoteProcess(string qualifier)
    //    {
    //        ThreadHelper.ThrowIfNotOnUIThread();
    //        try
    //        {
    //            // Here we simulate a call to attach the debugger by executing the standard command.
    //            // Note: The "Debug.AttachToProcess" command might not accept arguments directly,
    //            // so in a complete solution you would use IVsDebugger/IVsDebugger3 to attach to the remote process.
    //            _dte.ExecuteCommand("Debug.AttachToProcess", qualifier);
    //        }
    //        catch (Exception ex)
    //        {
    //            throw;
    //            //System.Windows.Forms.MessageBox.Show("Error attaching debugger: " + ex.Message);
    //        }
    //    }
    //}
}
