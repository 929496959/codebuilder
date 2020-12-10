using CodeBuilder.Core;
using CodeBuilder.Core.Variable;
using Fireasy.Common.Extensions;
using Fireasy.Common.Logging;
using Fireasy.Windows.Forms;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Xml;

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
            CheckVersion();


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

        static void CheckVersion()
        {

            var file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "version.xml");
            var xml = new XmlDocument();
            xml.Load(file);

            var node = xml.SelectSingleNode("//local/check-date");
            if (Config.Instance.CheckUpdate || node == null || node.InnerText.To<DateTime>().AddDays(7) <= DateTime.Today)
            {
                Process.Start("AutoUpdate.exe");
                if (node == null)
                {
                    node = xml.CreateNode(XmlNodeType.Element, "check-date", string.Empty);
                    xml.SelectSingleNode("//local").AppendChild(node);
                }

                node.InnerText = DateTime.Today.ToShortDateString();

                xml.Save(file);
            }
        }
    }
}
