using Microsoft.VisualStudio;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using TestcontainersDebugger.Logging;
using Task = System.Threading.Tasks.Task;

namespace TestcontainersDebugger
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [ProvideAutoLoad(UIContextGuids80.NoSolution, PackageAutoLoadFlags.BackgroundLoad)]
    [Guid(PackageGuidString)]
    public sealed class TestcontainersDebuggerPackage : AsyncPackage, IVsDebuggerEvents
    {
        private const string ExtensionName = "Testcontainers Debugger";

        //IVsSolution vsSolution;
        private IVsDebugger _debuggerService;
        //IVsRunningDocumentTable runningDocTable;
        //IVsOutputWindowPane debugOutputPane;
        //IVsOutputWindowPane generalOutputPane;
        //IVsStatusbar statusBar;
        //SolutionEvents solutionEvents;
        //VisualStudioWorkspace workspace;

        //DocumentEvents documentEvents;
        //TextEditorEvents textEditorEvents;

        //uint debugEventsCookie = 0;
        //uint docTableCookie = 0;
        //DBGMODE debugMode = DBGMODE.DBGMODE_Design;
        //bool isDebugging = false;
        //bool triggerReloadByTypeing = false;


        
        private uint _debugEventsCookie = 0;
        /// <summary>
        /// TestcontainersDebuggerPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "d2f11835-bb82-4b47-aaf5-fea4f56003d0";

        [Import]
        internal ILogger _logger { get; set; }

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to monitor for initialization cancellation, which can occur when VS is shutting down.</param>
        /// <param name="progress">A provider for progress updates.</param>
        /// <returns>A task representing the async work of package initialization, or an already completed task if there is none. Do not return null from this method.</returns>
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await base.InitializeAsync(cancellationToken, progress);

            // Switch to the UI thread since some services require it.
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            // Note: see https://github.com/Clancey/Reloadify3000/blob/da8040f0717e73bd3615f57a13cf43eb0cd8a8f4/Reloadify.VS/CometReloadVisixPackage.cs as a refernce.

            // Report initial progress.
            var componentModel = (IComponentModel)await GetServiceAsync(typeof(SComponentModel));
            // Fail fast if MEF imports are misconfgured.
            componentModel.DefaultCompositionService.SatisfyImportsOnce(this);

            // NOTE: GetServiceAsync for IVsDebugger failed with "No Such Interface Supported",
            //       which is why we still use the synchronous call that requires main thread access.
            //_debuggerService = (IVsDebugger)GetService(typeof(IVsDebugger));
            _debuggerService = (IVsDebugger)await GetServiceAsync(typeof(IVsDebugger));

            _debuggerService.AdviseDebuggerEvents(this, out _debugEventsCookie);
        }

        public int OnModeChange(DBGMODE mode)
        {
            //if (mode == DBGMODE.DBGMODE_Run)
            //{
            //    _logger.Log("Debugger session started (Test debug launch).");
            //}
            
            _logger.Log($"Debugger mode changed to: {mode}");
       
            return VSConstants.S_OK;
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                //ide.AgentStatusChanged -= IdeManager_AgentStatusChanged;
                //ide.AgentViewAppeared -= IdeManager_AgentViewAppeared;
                //ide.AgentViewDisappeared -= IdeManager_AgentViewDisappeared;
                //ide.AgentReloadResultReceived -= IdeManager_AgentXamlResultReceived;
                //ide = null;

                _debuggerService?.UnadviseDebuggerEvents(_debugEventsCookie);
                //if (documentEvents != null)
                //{
                //    documentEvents.DocumentSaved -= DocumentEvents_DocumentSaved;
                //}
                //if (textEditorEvents != null)
                //{
                //    textEditorEvents.LineChanged -= TextEditorEvents_LineChanged;
                //}
                //if (solutionEvents != null)
                //{
                //    solutionEvents.Opened -= OnSolutionOpened;
                //    solutionEvents.AfterClosing -= OnAfterSolutionClosing;

                //}

                //Cleanup();

                Trace.Flush();
            }

            base.Dispose(disposing);
        }

    }
}
