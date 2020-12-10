using CodeBuilder.Core;
using System.ComponentModel.Composition;
using System.Windows.Forms;

namespace CodeBuilder.Tools
{
    [Export(typeof(IToolProvider))]
    public class ConnEncryptor : IToolProvider
    {
        public string Name
        {
            get { return "连接串加解密"; }
        }

        public void Show(IWin32Window window)
        {
            new frmConnEncrypt().ShowDialog(window);
        }
    }
}
