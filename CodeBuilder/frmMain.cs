// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using CodeBuilder.Core;
using CodeBuilder.Core.Source;
using CodeBuilder.Core.Template;
using Fireasy.Common.Composition;
using Fireasy.Common.Extensions;
using Fireasy.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Linq;
using WeifenLuo.WinFormsUI.Docking;
using System.Net;
using Fireasy.Common.Serialization;

namespace CodeBuilder
{
    public partial class frmMain : Form
    {
        private frmTable frmTable;
        private frmProperty frmProperty;
        private frmTemplate frmTemplate;
        private frmProfile frmProfile;
        private frmOutput frmOutput;

        public frmMain()
        {
            InitializeComponent();
            Icon = Util.GetIcon();
            mnuPropertyWnd.Image = Properties.Resources.property.ToBitmap();
            mnuProfileWnd.Image = Properties.Resources.profile.ToBitmap();
            mnuTemplateWnd.Image = Properties.Resources.template.ToBitmap();
            mnuOutputWnd.Image = Properties.Resources.output.ToBitmap();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            StaticUnity.Profile = ProfileUnity.LoadCurrent();
            InitializeSourceMenus();
            InitializeTemplateMenus();

            OpenOutputForm();
            GetTableForm();
            OpenPeopertyForm();
            OpenProfileForm();
            OpenTemplateForm();
            frmProperty.Activate();

            Process.Start("AutoUpdate.exe");
        }

        private frmTable GetTableForm()
        {
            if (frmTable == null)
            {
                frmTable = new frmTable();
                frmTable.SelectItemAct = o =>
                    {
                        if (frmProperty != null)
                        {
                            frmProperty.SetObject(o);
                        }
                    };
                frmTable.CloseAct = () =>
                    {
                        frmTable = null;
                        if (frmProperty != null)
                        {
                            frmProperty.SetObject(null);
                        }
                    };
                frmTable.Show(dockMgr, DockState.Document);
            }

            return frmTable;
        }

        private void OpenPeopertyForm()
        {
            if (frmProperty == null)
            {
                frmProperty = new frmProperty();
                frmProperty.CloseAct = () => frmProperty = null;
                frmProperty.Show(dockMgr, DockState.DockRight);

                if (frmTable != null)
                {
                    frmProperty.SetObject(frmTable.GetSelectedObject());
                }
            }
            else
            {
                frmProperty.Activate();
            }
        }

        private void OpenTemplateForm()
        {
            if (frmTemplate == null)
            {
                frmTemplate = new frmTemplate();
                frmTemplate.OpenAct = t =>
                    {
                        OpenFileName(t);
                    };
                frmTemplate.CloseAct = () => frmTemplate = null;
                frmTemplate.TemplateAct = () => ReInitializeTemplateSubMenus();

                frmTemplate.Show(dockMgr, DockState.DockRight);
            }
            else
            {
                frmTemplate.Activate();
            }
        }
        
        private void OpenProfileForm()
        {
            if (frmProfile == null)
            {
                frmProfile = new frmProfile();
                frmProfile.CloseAct = () => frmProfile = null;
                frmProfile.Show(dockMgr, DockState.DockRight);
            }
            else
            {
                frmProfile.Activate();
            }
        }

        private void OpenOutputForm()
        {
            if (frmOutput == null)
            {
                frmOutput = new frmOutput();
                frmOutput.CloseAct = () => frmOutput = null;
            }

            frmOutput.Show(dockMgr, DockState.DockBottom);
            frmOutput.DockState = DockState.DockBottomAutoHide;
        }

        private void InitializeSourceMenus()
        {
            var sources = Imports.GetServices<ISourceProvider>();
            foreach (var p in sources)
            {
                var sItem = new ToolStripMenuItem();
                sItem.Text = p.Name;
                sItem.Name = p.Name;
                sItem.Tag = p;
                sItem.Click += sourceProvider_Click;

                mnuSource.DropDownItems.Add(sItem);
            }
        }

        private void InitializeTemplateMenus()
        {
            var providers = Imports.GetServices<ITemplateProvider>();
            foreach (var p in providers)
            {
                var sItem = new ToolStripMenuItem();
                sItem.Text = p.Name;
                sItem.Name = p.Name;
                sItem.Tag = p;

                var current = Config.Instance.TemplateProvider == p.Name;
                if (current)
                {
                    sItem.ForeColor = Color.Blue;
                    StaticUnity.TemplateProvider = p;
                }

                var templates = p.GetTemplates();
                foreach (var f in templates)
                {
                    var fItem = new ToolStripMenuItem();
                    fItem.Text = f.Name;
                    fItem.Tag = f;
                    if (current && Config.Instance.TemplateFileName.Equals(f.Id, StringComparison.CurrentCultureIgnoreCase))
                    {
                        StaticUnity.Template = f;
                        fItem.Checked = true;
                    }

                    fItem.Click += templateProvider_Click;

                    sItem.DropDownItems.Add(fItem);
                }

                mnuTemplate.DropDownItems.Add(sItem);
            }
        }

        private void ReInitializeTemplateSubMenus()
        {
            var root = mnuTemplate.DropDownItems.Find(StaticUnity.TemplateProvider.Name, false).FirstOrDefault() as ToolStripMenuItem;
            if (root == null)
            {
                return;
            }

            root.DropDownItems.Clear();

            var templates = StaticUnity.TemplateProvider.GetTemplates();
            foreach (var f in templates)
            {
                var fItem = new ToolStripMenuItem();
                fItem.Text = f.Name;
                fItem.Tag = f;
                if (Config.Instance.TemplateFileName.Equals(f.Id, StringComparison.CurrentCultureIgnoreCase))
                {
                    StaticUnity.Template = f;
                    fItem.Checked = true;
                }

                fItem.Click += templateProvider_Click;

                root.DropDownItems.Add(fItem);
            }
        }

        private void LoadSourceStruct(ISourceProvider provider)
        {
            var tables = provider.Preview();
            if (tables == null)
            {
                return;
            }

            GetSchemaAsync(provider, tables);
        }

        private void FillTables(IEnumerable<IObject> tables)
        {
            GetTableForm().FillTables(tables);
        }

        /// <summary>
        /// 使用异步方式加载表的构架。
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="source"></param>
        private void GetSchemaAsync(ISourceProvider provider, List<Table> source)
        {
            spbar.Value = 0;
            spbar.Visible = true;

            Processor.Run(this, () =>
                {
                    var tables = provider.GetSchema(source, (t, p) =>
                        {
                            Invoke(new Action(() =>
                                {
                                    spState.Text = string.Format("{0}%，正在获取表 {1} 的结构...", p, t);
                                    spbar.Value = p;
                                }));
                        });

                    Invoke(new Action(() =>
                        {
                            FillTables(tables);
                            spState.Text = "就绪";
                            spbar.Value = 0;
                            spbar.Visible = false;
                        }));
                });
        }

        private void NewEditor()
        {
            using (var frm = new frmNewCode())
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    var editform = new frmEditor { Template = frm.Templage };
                    editform.Show(dockMgr, DockState.Document);
                }
            }
        }

        private void OpenFileName(TemplateFile fileItem)
        {
            foreach (DockContent content in dockMgr.Documents)
            {
                if (content is frmEditor)
                {
                    if ((content as frmEditor).FileName == fileItem.FileName)
                    {
                        content.Activate();
                        return;
                    }
                }
            }

            var editform = new frmEditor { TemplateFile = fileItem };
            editform.Show(dockMgr, DockState.Document);
        }

        private void OpenFileName(string fileName)
        {
            foreach (DockContent content in dockMgr.Documents)
            {
                if (content is frmEditor)
                {
                    if ((content as frmEditor).FileName == fileName)
                    {
                        content.Activate();
                        return;
                    }
                }
            }

            var editform = new frmEditor(fileName);
            editform.Show(dockMgr, DockState.Document);
        }

        void sourceProvider_Click(object sender, EventArgs e)
        {
            var item = sender as ToolStripMenuItem;
            if (item == null || item.Tag == null)
            {
                return;
            }

            var provider = item.Tag as ISourceProvider;
            if (provider == null)
            {
                return;
            }

            LoadSourceStruct(provider);
        }

        void templateProvider_Click(object sender, EventArgs e)
        {
            var menu = sender as ToolStripMenuItem;
            var owner = menu.OwnerItem as ToolStripMenuItem;
            var parent = owner.OwnerItem as ToolStripMenuItem;
            foreach (ToolStripItem sub in parent.DropDownItems)
            {
                var menuItem = sub as ToolStripMenuItem;
                if (menuItem == null || sub.Tag == null)
                {
                    continue;
                }

                foreach (ToolStripMenuItem sub1 in menuItem.DropDownItems)
                {
                    sub1.Checked = false;
                }

                sub.ForeColor = SystemColors.ControlText;
            }

            owner.ForeColor = Color.Blue;
            menu.Checked = true;

            var templateProvider = owner.Tag as ITemplateProvider;
            StaticUnity.Template = menu.Tag as TemplateDefinition;
            var providerChange = false;
            if (templateProvider != StaticUnity.TemplateProvider)
            {
                StaticUnity.TemplateProvider = templateProvider;
                providerChange = true;
            }

            if (providerChange || Config.Instance.TemplateFileName != StaticUnity.Template.Id)
            {
                Config.Instance.TemplateFileName = StaticUnity.Template.Id;
                Config.Instance.TemplateProvider = StaticUnity.TemplateProvider.Name;
                Config.Save();

                if (frmTemplate != null)
                {
                    frmTemplate.Reload();
                }
            }
        }

        private void BuildCode()
        {
            if (frmTable == null)
            {
                MessageBoxHelper.ShowExclamation("你还没有选择要生成的对象，请从【数据源】菜单中选择或配置。");
                return;
            }

            TemplateDefinition template = null;
            if (StaticUnity.TemplateProvider == null ||
                (template = StaticUnity.Template) == null)
            {
                MessageBoxHelper.ShowExclamation("你还没有选择生成模板，请从【模板】菜单中选择。");
                return;
            }

            var option = new TemplateOption();
            option.Template = template;
            option.DynamicAssemblies.AddRange(StaticUnity.DynamicAssemblies);
            option.Profile = StaticUnity.Profile;
            option.WriteToDisk = true;

            using (var frm = new frmPreBuild { Template = StaticUnity.Template })
            {
                if (frm.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                option.Partitions = frm.Partitions;
                option.OutputDirectory = Config.Instance.OutputDirectory;
            }

            spbar.Value = 0;
            spbar.Visible = true;

            var time = Processor.Run(this, () =>
                {
                    var tables = frmTable.GetCheckedTables();
                    StaticUnity.TemplateProvider.GenerateFiles(option, tables, (s, p) =>
                        {
                            Invoke(new Action(() =>
                                {
                                    spState.Text = string.Format("{0}%，正在生成 {1} 的代码文件...", p, s);
                                    spbar.Value = p;
                                }));
                        });

                    Invoke(new Action(() =>
                        {
                            spState.Text = "就绪";
                            spbar.Value = 0;
                            spbar.Visible = false;
                            Process.Start(Config.Instance.OutputDirectory);
                        }));
                });

            Console.WriteLine("代码生成完成，耗时 " + time.ToStringEx());
        }

        #region 菜单事件
        private void mnuProfileWnd_Click(object sender, EventArgs e)
        {
            OpenProfileForm();
        }

        private void mnuPropertyWnd_Click(object sender, EventArgs e)
        {
            OpenPeopertyForm();
        }

        private void mnuTemplateWnd_Click(object sender, EventArgs e)
        {
            OpenTemplateForm();
        }

        private void mnuOutputWnd_Click(object sender, EventArgs e)
        {
            OpenOutputForm();
        }

        private void mnuNew_Click(object sender, EventArgs e)
        {
            NewEditor();
        }

        private void mnuOpen_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = FileTypeHelper.GetAllFilters();
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    OpenFileName(dialog.FileName);
                }
            }
        }

        private void mnuSave_Click(object sender, EventArgs e)
        {
            var frm = dockMgr.ActiveContent as frmEditor;
            if (frm != null)
            {
                frm.SaveFile();
            }
        }

        private void mnuSaveAs_Click(object sender, EventArgs e)
        {
            var frm = dockMgr.ActiveContent as frmEditor;
            if (frm != null)
            {
                frm.SaveAs();
            }
        }

        private void mnuBuild_Click(object sender, EventArgs e)
        {
            BuildCode();
        }

        private void mnuQuit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void mnuCloseCurrent_Click(object sender, EventArgs e)
        {
            if (dockMgr.ActiveDocument is ClosableDockContent)
            {
                dockMgr.ActiveDocument.DockHandler.Close();
            }
        }

        private void mnuCloseAll_Click(object sender, EventArgs e)
        {
            var documents = dockMgr.DocumentsToArray();

            foreach (IDockContent content in documents)
            {
                if (content is ClosableDockContent)
                {
                    content.DockHandler.Close();
                }
            }
        }

        private void mnuCloseOther_Click(object sender, EventArgs e)
        {
            var documents = dockMgr.DocumentsToArray();

            foreach (IDockContent content in documents)
            {
                if (content is ClosableDockContent && !content.Equals(dockMgr.ActiveDocument))
                {
                    content.DockHandler.Close();
                }
            }
        }

        private void mnuAbout_Click(object sender, EventArgs e)
        {
            using (var frm = new frmAbout())
            {
                frm.ShowDialog();
            }
        }

        private void mnuTopic_Click(object sender, EventArgs e)
        {
            Help.ShowHelp(this, "http://www.fireasy.cn/codebuilder/help");
        }

        private void mnuUpdate_Click(object sender, EventArgs e)
        {
            var processName = Process.GetCurrentProcess().ProcessName;
            Process.Start("AutoUpdate.exe", "/T");
        }

        private void mnuFeedback_Click(object sender, EventArgs e)
        {
            Process.Start("http://www.fireasy.cn/codebuilder/feedback");
        }

        #endregion

        #region 工具栏事件
        private void tlbOpen_Click(object sender, EventArgs e)
        {
            mnuOpen_Click(null, null);
        }

        private void tlbSave_Click(object sender, EventArgs e)
        {
            mnuSave_Click(null, null);
        }

        private void tlbBuild_Click(object sender, EventArgs e)
        {
            BuildCode();
        }
        #endregion

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            var list = new List<IChangeManager>();
            foreach (IDockContent content in dockMgr.Contents)
            {
                var chgMgr = content as IChangeManager;
                if (chgMgr != null && chgMgr.IsChanged)
                {
                    list.Add(chgMgr);
                }
            }

            if (list.Count > 0)
            {
                var r = MessageBoxHelper.ShowQuestionEx("有文档未已修改，是否保存后再退出?");
                if (r == System.Windows.Forms.DialogResult.Yes)
                {
                    list.ForEach(s => s.SaveFile());
                }
                else if (r == System.Windows.Forms.DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
            }

            base.OnClosing(e);
        }
    }
}
