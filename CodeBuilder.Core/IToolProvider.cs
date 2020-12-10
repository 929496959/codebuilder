using System.Windows.Forms;

namespace CodeBuilder.Core
{
    public interface IToolProvider : IPlugin
    {
        string Name { get; }

        void Show(IWin32Window window);
    }
}
