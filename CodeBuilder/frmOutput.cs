// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using CodeBuilder.Core;
using Fireasy.Common.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace CodeBuilder
{
    public partial class frmOutput : DockForm
    {
        public frmOutput()
        {
            InitializeComponent();
            Icon = Properties.Resources.output;
        }

        private void frmOutput_Load(object sender, EventArgs e)
        {
            Console.SetOut(new OutputWriter(this, textBox1));
        }

        private class OutputWriter : TextWriter
        {   
            private Form form;
            private TextBox textbox;

            public OutputWriter(Form form, TextBox textbox)
            {
                this.form = form;
                this.textbox = textbox;
            }

            public override void Write(string str)
            {
                form.Invoke(new Action(() =>
                    {
                        textbox.Text += str;
                        textbox.SelectionStart = textbox.Text.Length;
                        textbox.ScrollToCaret();
                    }));
            }

            public override void WriteLine(string value)
            {
                Write(value);
                WriteLine();
            }

            public override void WriteLine()
            {
                form.Invoke(new Action(() =>
                    {
                        textbox.Text += Environment.NewLine;
                        textbox.SelectionStart = textbox.Text.Length;
                        textbox.ScrollToCaret();
                    }));
            }

            public override Encoding Encoding
            {
                get { return Encoding.Default; }
            }
        }

        private void tlbClear_Click(object sender, EventArgs e)
        {
            textBox1.Text = string.Empty;
        }

        private void tlbCopy_Click(object sender, EventArgs e)
        {
            if (textBox1.SelectionLength > 0)
            {
                Clipboard.SetText(textBox1.Text);
            }
            else if (textBox1.Text.Length > 0)
            {
                Clipboard.SetText(textBox1.SelectedText);
            }
        }
    }
}
