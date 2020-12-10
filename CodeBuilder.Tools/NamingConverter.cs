using CodeBuilder.Core;
using System.ComponentModel.Composition;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace CodeBuilder.Tools
{
    [Export(typeof(IToolProvider))]
    public class NamingConverter : IToolProvider
    {
        public string Name
        {
            get { return "命名转换器"; }
        }

        public void Show(IWin32Window window)
        {
            new frmNamingConverter().Show((DockPanel)window, DockState.Document);
        }
    }
}
