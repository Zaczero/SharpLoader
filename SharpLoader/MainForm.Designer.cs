namespace SharpLoader
{
    partial class MainForm
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
            this.Theme = new SharpLoader.NSTheme();
            this.nsButton1 = new SharpLoader.NSButton();
            this.Theme.SuspendLayout();
            this.SuspendLayout();
            // 
            // Theme
            // 
            this.Theme.AccentOffset = 0;
            this.Theme.BorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Theme.Colors = new Bloom[0];
            this.Theme.Controls.Add(this.nsButton1);
            this.Theme.Customization = "";
            this.Theme.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Theme.Font = new System.Drawing.Font("Verdana", 8F);
            this.Theme.Image = null;
            this.Theme.Location = new System.Drawing.Point(0, 0);
            this.Theme.Movable = true;
            this.Theme.Name = "Theme";
            this.Theme.NoRounding = false;
            this.Theme.Sizable = false;
            this.Theme.Size = new System.Drawing.Size(500, 300);
            this.Theme.SmartBounds = true;
            this.Theme.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Theme.TabIndex = 0;
            this.Theme.Text = "SharpLoader";
            this.Theme.TransparencyKey = System.Drawing.Color.Empty;
            this.Theme.Transparent = false;
            // 
            // nsButton1
            // 
            this.nsButton1.Location = new System.Drawing.Point(423, 265);
            this.nsButton1.Name = "nsButton1";
            this.nsButton1.Size = new System.Drawing.Size(65, 23);
            this.nsButton1.TabIndex = 0;
            this.nsButton1.Text = "COMPILE";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(500, 300);
            this.Controls.Add(this.Theme);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MainForm";
            this.Theme.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private NSTheme Theme;
        private NSButton nsButton1;
    }
}