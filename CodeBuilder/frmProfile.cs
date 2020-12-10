// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using CodeBuilder.Core;
using System;
using System.IO;
using System.Windows.Forms;

namespace CodeBuilder
{
    public partial class frmProfile : DockForm
    {
        private string profileName;
        private string profileDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "profiles");
        private const string FILTER = "Profile Files(*.profile)|*.profile";
        private const string CAPTION = "CodeBuilder Preview";

        public frmProfile()
        {
            InitializeComponent();
            Icon = Properties.Resources.profile;
        }

        public Action PropertyChangeAct { get; set; }

        private void frmProfile_Load(object sender, EventArgs e)
        {
            propertyGrid1.SelectedObject = StaticUnity.Profile;
        }

        private void tlbOpen_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = FILTER;
                dialog.InitialDirectory = profileDir;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    profileName = dialog.FileName;
                    StaticUnity.Profile = ProfileUnity.LoadFile(profileName);
                    propertyGrid1.SelectedObject = StaticUnity.Profile;
                    if (PropertyChangeAct != null)
                    {
                        PropertyChangeAct();
                    }
                }
            }
        }

        private void tlbSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(profileName))
            {
                ProfileUnity.SaveFile(Config.Instance.Profile, StaticUnity.Profile);
            }
            else
            {
                ProfileUnity.SaveFile(StaticUnity.Profile, profileName);
            }
        }

        private void tlbSaveAs_Click(object sender, EventArgs e)
        {
            using (var dialog = new SaveFileDialog())
            {
                dialog.Filter = FILTER;
                dialog.InitialDirectory = profileDir;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    profileName = dialog.FileName;
                    ProfileUnity.SaveFile(StaticUnity.Profile, profileName);
                }
            }
        }

        private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (PropertyChangeAct != null)
            {
                PropertyChangeAct();
            }
        }

        private void tlbSetDefault_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(profileName))
            {
                var info = new FileInfo(profileName).Name;
                Config.Instance.Profile = info;
                Config.Save();
            }
        }
    }
}
