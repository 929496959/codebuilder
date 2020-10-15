// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using CodeBuilder.Core;
using System;

namespace CodeBuilder
{
    public partial class frmProperty : DockForm
    {
        public frmProperty()
        {
            InitializeComponent();
            Icon = Properties.Resources.property;
            PropertyUnity.Register(obj => propertyGrid1.SelectedObject = obj);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            PropertyUnity.Register(null);
        }
    }
}
