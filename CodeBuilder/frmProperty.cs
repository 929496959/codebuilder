// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Windows.Forms;
using System;

namespace CodeBuilder
{
    public partial class frmProperty : DockForm
    {
        public frmProperty()
        {
            InitializeComponent();
            Icon = Properties.Resources.property;
        }

        public void SetObject(object obj)
        {
            propertyGrid1.SelectedObject = obj;
        }
    }
}
