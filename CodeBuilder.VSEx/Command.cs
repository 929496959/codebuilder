using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Task = System.Threading.Tasks.Task;

namespace CodeBuilder.VSEx
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class Command
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("93864444-51c2-433c-8cd6-ac5cf49b002d");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;

        private static DTE2 _dte;

        /// <summary>
        /// Initializes a new instance of the <see cref="Command"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private Command(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(this.Execute, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static Command Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static async Task InitializeAsync(AsyncPackage package, DTE2 dte)
        {
            _dte = dte;

            // Switch to the main thread - the call to AddCommand in Command's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new Command(package, commandService);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void Execute(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (_dte.SelectedItems.Count == 0)
            {
                // Show a message box to prove we were here
                VsShellUtilities.ShowMessageBox(
                    this.package,
                    "未选中项目。",
                    "CodeBuilder",
                    OLEMSGICON.OLEMSGICON_INFO,
                    OLEMSGBUTTON.OLEMSGBUTTON_OK,
                    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
                return;
            }

            var project = _dte.SelectedItems.Item(1).Project;

            var item = GetConfigItem(project);

            //当前类命名空间
            var namespaceStr = project.CodeModel.CodeElements.OfType<CodeNamespace>().First().FullName;
        }

        private string GetProjectPath(Project project)
        {
            var fileName = project.FileName;
            return Path.GetDirectoryName(fileName);
        }

        private ProjectItem GetConfigItem(Project project)
        {
            for (var i = 0; i < project.ProjectItems.Count; i++)
            {
                var item = project.ProjectItems.Item(i + 1);
                if (item.Name == "codebuilder.config")
                {
                    return item;
                }
            }

            var path = GetProjectPath(project);
            string file = Path.Combine(path, "codebuilder.config");
            File.WriteAllText(file, string.Empty, System.Text.Encoding.UTF8);
            return project.ProjectItems.AddFromFileCopy(file);
        }

        /// <summary>
        /// 添加文件到项目中
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="content"></param>
        /// <param name="fileName"></param>
        private void AddFileToProjectItem(ProjectItem folder, string content, string fileName)
        {
            try
            {
                string path = Path.GetTempPath();
                Directory.CreateDirectory(path);
                string file = Path.Combine(path, fileName);
                File.WriteAllText(file, content, System.Text.Encoding.UTF8);
                try
                {
                    folder.ProjectItems.AddFromFileCopy(file);
                }
                finally
                {
                    File.Delete(file);
                }
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// 添加文件到指定目录
        /// </summary>
        /// <param name="directoryPathOrFullPath"></param>
        /// <param name="content"></param>
        /// <param name="fileName"></param>
        private void AddFileToDirectory(string directoryPathOrFullPath, string content, string fileName = "")
        {
            try
            {
                string file = string.IsNullOrEmpty(fileName) ? directoryPathOrFullPath : Path.Combine(directoryPathOrFullPath, fileName);
                File.WriteAllText(file, content, System.Text.Encoding.UTF8);
            }
            catch (Exception ex)
            {

            }
        }
    }
}