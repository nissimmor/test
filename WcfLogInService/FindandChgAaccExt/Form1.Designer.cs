namespace FindandChgAaccExt
{
    partial class Form1
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
            this.b_findext = new System.Windows.Forms.Button();
            this.t_ext = new System.Windows.Forms.TextBox();
            this.l_hname = new System.Windows.Forms.Label();
            this.l_hloginId = new System.Windows.Forms.Label();
            this.l_name = new System.Windows.Forms.Label();
            this.l_loginId = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.t_newext = new System.Windows.Forms.TextBox();
            this.b_chgext = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // b_findext
            // 
            this.b_findext.Location = new System.Drawing.Point(63, 43);
            this.b_findext.Name = "b_findext";
            this.b_findext.Size = new System.Drawing.Size(57, 23);
            this.b_findext.TabIndex = 0;
            this.b_findext.Text = "find ext";
            this.b_findext.UseVisualStyleBackColor = true;
            this.b_findext.Click += new System.EventHandler(this.b_findext_Click);
            // 
            // t_ext
            // 
            this.t_ext.Location = new System.Drawing.Point(157, 45);
            this.t_ext.Name = "t_ext";
            this.t_ext.Size = new System.Drawing.Size(58, 20);
            this.t_ext.TabIndex = 1;
            // 
            // l_hname
            // 
            this.l_hname.AutoSize = true;
            this.l_hname.Location = new System.Drawing.Point(30, 16);
            this.l_hname.Name = "l_hname";
            this.l_hname.Size = new System.Drawing.Size(35, 13);
            this.l_hname.TabIndex = 4;
            this.l_hname.Text = "Name";
            // 
            // l_hloginId
            // 
            this.l_hloginId.AutoSize = true;
            this.l_hloginId.Location = new System.Drawing.Point(30, 48);
            this.l_hloginId.Name = "l_hloginId";
            this.l_hloginId.Size = new System.Drawing.Size(44, 13);
            this.l_hloginId.TabIndex = 5;
            this.l_hloginId.Text = "LoginID";
            // 
            // l_name
            // 
            this.l_name.AutoSize = true;
            this.l_name.Location = new System.Drawing.Point(121, 16);
            this.l_name.Name = "l_name";
            this.l_name.Size = new System.Drawing.Size(35, 13);
            this.l_name.TabIndex = 6;
            this.l_name.Text = "Name";
            // 
            // l_loginId
            // 
            this.l_loginId.AutoSize = true;
            this.l_loginId.Location = new System.Drawing.Point(121, 48);
            this.l_loginId.Name = "l_loginId";
            this.l_loginId.Size = new System.Drawing.Size(44, 13);
            this.l_loginId.TabIndex = 7;
            this.l_loginId.Text = "LoginID";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.t_newext);
            this.panel1.Controls.Add(this.b_chgext);
            this.panel1.Controls.Add(this.l_loginId);
            this.panel1.Controls.Add(this.l_name);
            this.panel1.Controls.Add(this.l_hloginId);
            this.panel1.Controls.Add(this.l_hname);
            this.panel1.Location = new System.Drawing.Point(33, 75);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(202, 136);
            this.panel1.TabIndex = 8;
            this.panel1.Visible = false;
            // 
            // t_newext
            // 
            this.t_newext.Location = new System.Drawing.Point(124, 93);
            this.t_newext.Name = "t_newext";
            this.t_newext.Size = new System.Drawing.Size(58, 20);
            this.t_newext.TabIndex = 9;
            // 
            // b_chgext
            // 
            this.b_chgext.Location = new System.Drawing.Point(30, 90);
            this.b_chgext.Name = "b_chgext";
            this.b_chgext.Size = new System.Drawing.Size(57, 23);
            this.b_chgext.TabIndex = 8;
            this.b_chgext.Text = "chg ext";
            this.b_chgext.UseVisualStyleBackColor = true;
            this.b_chgext.Click += new System.EventHandler(this.b_chgext_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.t_ext);
            this.Controls.Add(this.b_findext);
            this.Name = "Form1";
            this.Text = "Form1";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button b_findext;
        private System.Windows.Forms.TextBox t_ext;
        private System.Windows.Forms.Label l_hname;
        private System.Windows.Forms.Label l_hloginId;
        private System.Windows.Forms.Label l_name;
        private System.Windows.Forms.Label l_loginId;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox t_newext;
        private System.Windows.Forms.Button b_chgext;
    }
}

