using System;
using System.Windows.Forms;

namespace CodeBuilder.Database
{
    public interface IConnectionConfig
    {
        string ConnectionString { get; set; }

        DialogResult ShowDialog(IntPtr handle);
    }
}
