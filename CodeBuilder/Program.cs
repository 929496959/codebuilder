using CodeBuilder.Core;
using CodeBuilder.Core.Variable;
using Fireasy.Common.Logging;
using Fireasy.Windows.Forms;
using System;
using System.Windows.Forms;

namespace CodeBuilder
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(params string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ThreadException += Application_ThreadException;
            Util.ClearTempFiles();
            
            SchemaExtensionManager.Initialize();
            ProfileExtensionManager.Initialize();

            Application.Run(new frmMain());
        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            var log = LoggerFactory.CreateLogger();
            log.Error("应用程序错误", e.Exception);
            ErrorMessageBox.Show("应用程序错误", e.Exception);
        }
    }
}
