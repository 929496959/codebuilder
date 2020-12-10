namespace CodeBuilder.Tools
{
    partial class frmNamingConverter
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.txtSource = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtFormatter = new System.Windows.Forms.TextBox();
            this.txtCamel = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtHungary = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtPascal = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.ck1 = new System.Windows.Forms.CheckBox();
            this.ck2 = new System.Windows.Forms.CheckBox();
            this.ck3 = new System.Windows.Forms.CheckBox();
            this.ck0 = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(87, 66);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "待转换文本";
            // 
            // txtSource
            // 
            this.txtSource.Font = new System.Drawing.Font("Consolas", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSource.Location = new System.Drawing.Point(167, 56);
            this.txtSource.Name = "txtSource";
            this.txtSource.Size = new System.Drawing.Size(398, 30);
            this.txtSource.TabIndex = 1;
            this.txtSource.Click += new System.EventHandler(this.txtSource_Click);
            this.txtSource.TextChanged += new System.EventHandler(this.txtSource_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(87, 115);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "转换的模板";
            // 
            // txtFormatter
            // 
            this.txtFormatter.Font = new System.Drawing.Font("Consolas", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtFormatter.Location = new System.Drawing.Point(167, 105);
            this.txtFormatter.Name = "txtFormatter";
            this.txtFormatter.Size = new System.Drawing.Size(398, 30);
            this.txtFormatter.TabIndex = 3;
            this.txtFormatter.Text = "{0}";
            // 
            // txtCamel
            // 
            this.txtCamel.Font = new System.Drawing.Font("Consolas", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCamel.Location = new System.Drawing.Point(167, 154);
            this.txtCamel.Name = "txtCamel";
            this.txtCamel.Size = new System.Drawing.Size(398, 30);
            this.txtCamel.TabIndex = 5;
            this.txtCamel.Click += new System.EventHandler(this.txtCamel_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(87, 164);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "驼峰命名";
            // 
            // txtHungary
            // 
            this.txtHungary.Font = new System.Drawing.Font("Consolas", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtHungary.Location = new System.Drawing.Point(167, 203);
            this.txtHungary.Name = "txtHungary";
            this.txtHungary.Size = new System.Drawing.Size(398, 30);
            this.txtHungary.TabIndex = 7;
            this.txtHungary.Click += new System.EventHandler(this.txtHungary_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(87, 213);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 12);
            this.label4.TabIndex = 6;
            this.label4.Text = "匈牙利命名";
            // 
            // txtPascal
            // 
            this.txtPascal.Font = new System.Drawing.Font("Consolas", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPascal.Location = new System.Drawing.Point(167, 252);
            this.txtPascal.Name = "txtPascal";
            this.txtPascal.Size = new System.Drawing.Size(398, 30);
            this.txtPascal.TabIndex = 9;
            this.txtPascal.Click += new System.EventHandler(this.txtPascal_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(87, 262);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 12);
            this.label5.TabIndex = 8;
            this.label5.Text = "帕斯卡命名";
            // 
            // ck1
            // 
            this.ck1.AutoSize = true;
            this.ck1.Location = new System.Drawing.Point(577, 160);
            this.ck1.Name = "ck1";
            this.ck1.Size = new System.Drawing.Size(48, 16);
            this.ck1.TabIndex = 10;
            this.ck1.Text = "锁定";
            this.ck1.UseVisualStyleBackColor = true;
            this.ck1.CheckedChanged += new System.EventHandler(this.ck1_CheckedChanged);
            // 
            // ck2
            // 
            this.ck2.AutoSize = true;
            this.ck2.Location = new System.Drawing.Point(577, 209);
            this.ck2.Name = "ck2";
            this.ck2.Size = new System.Drawing.Size(48, 16);
            this.ck2.TabIndex = 11;
            this.ck2.Text = "锁定";
            this.ck2.UseVisualStyleBackColor = true;
            this.ck2.CheckedChanged += new System.EventHandler(this.ck2_CheckedChanged);
            // 
            // ck3
            // 
            this.ck3.AutoSize = true;
            this.ck3.Location = new System.Drawing.Point(577, 258);
            this.ck3.Name = "ck3";
            this.ck3.Size = new System.Drawing.Size(48, 16);
            this.ck3.TabIndex = 12;
            this.ck3.Text = "锁定";
            this.ck3.UseVisualStyleBackColor = true;
            this.ck3.CheckedChanged += new System.EventHandler(this.ck3_CheckedChanged);
            // 
            // ck0
            // 
            this.ck0.AutoSize = true;
            this.ck0.Location = new System.Drawing.Point(577, 114);
            this.ck0.Name = "ck0";
            this.ck0.Size = new System.Drawing.Size(48, 16);
            this.ck0.TabIndex = 13;
            this.ck0.Text = "锁定";
            this.ck0.UseVisualStyleBackColor = true;
            this.ck0.CheckedChanged += new System.EventHandler(this.ck0_CheckedChanged);
            // 
            // frmNamingConverter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.ck0);
            this.Controls.Add(this.ck3);
            this.Controls.Add(this.ck2);
            this.Controls.Add(this.ck1);
            this.Controls.Add(this.txtPascal);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtHungary);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtCamel);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtFormatter);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtSource);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Name = "frmNamingConverter";
            this.Text = "命名转换器";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtSource;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtFormatter;
        private System.Windows.Forms.TextBox txtCamel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtHungary;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtPascal;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox ck1;
        private System.Windows.Forms.CheckBox ck2;
        private System.Windows.Forms.CheckBox ck3;
        private System.Windows.Forms.CheckBox ck0;
    }
}