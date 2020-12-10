using CodeBuilder.Core;
using System.ComponentModel.Composition;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace CodeBuilder.Tools
{
    [Export(typeof(IToolProvider))]
    public class Remoter : IToolProvider
    {
        public string Name
        {
            get { return "远程连接器"; }
        }

        public void Show(IWin32Window window)
        {
            new frmRemoter().Show((DockPanel)window, DockState.Document);
        }

        public class Connection
        {
            public string name { get; set; }
            public string host { get; set; }
            public string group { get; set; }
        }
    }
}
