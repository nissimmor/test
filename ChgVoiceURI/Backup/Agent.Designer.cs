namespace LoginForm
{
    partial class Agent
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
            this.CboServers = new System.Windows.Forms.ComboBox();
            this.button1 = new System.Windows.Forms.Button();
            this.lBoxAgents = new System.Windows.Forms.ListBox();
            this.TokenText = new System.Windows.Forms.Label();
            this.IPText = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // CboServers
            // 
            this.CboServers.FormattingEnabled = true;
            this.CboServers.Location = new System.Drawing.Point(83, 38);
            this.CboServers.Name = "CboServers";
            this.CboServers.Size = new System.Drawing.Size(337, 21);
            this.CboServers.TabIndex = 2;
            this.CboServers.SelectedIndexChanged += new System.EventHandler(this.CboServers_SelectedIndexChanged);
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Location = new System.Drawing.Point(345, 372);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Exit";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // lBoxAgents
            // 
            this.lBoxAgents.FormattingEnabled = true;
            this.lBoxAgents.Location = new System.Drawing.Point(83, 112);
            this.lBoxAgents.Name = "lBoxAgents";
            this.lBoxAgents.Size = new System.Drawing.Size(337, 238);
            this.lBoxAgents.TabIndex = 0;
            this.lBoxAgents.SelectedIndexChanged += new System.EventHandler(this.lBoxAgents_SelectedIndexChanged);
            // 
            // TokenText
            // 
            this.TokenText.AutoSize = true;
            this.TokenText.Location = new System.Drawing.Point(80, 9);
            this.TokenText.Name = "TokenText";
            this.TokenText.Size = new System.Drawing.Size(45, 13);
            this.TokenText.TabIndex = 5;
            this.TokenText.Text = "tokentxt";
            // 
            // IPText
            // 
            this.IPText.AutoSize = true;
            this.IPText.Location = new System.Drawing.Point(80, 77);
            this.IPText.Name = "IPText";
            this.IPText.Size = new System.Drawing.Size(28, 13);
            this.IPText.TabIndex = 6;
            this.IPText.Text = "IPtxt";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(83, 372);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(151, 23);
            this.button2.TabIndex = 7;
            this.button2.Text = "Change Agent Assignments";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // Agent
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(514, 438);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.IPText);
            this.Controls.Add(this.TokenText);
            this.Controls.Add(this.lBoxAgents);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.CboServers);
            this.Name = "Agent";
            this.Text = "Agent Selection";
            this.Load += new System.EventHandler(this.Agent_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox CboServers;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ListBox lBoxAgents;
        private System.Windows.Forms.Label TokenText;
        private System.Windows.Forms.Label IPText;
        private System.Windows.Forms.Button button2;
    }
}