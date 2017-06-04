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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.Theme = new SharpLoader.NSTheme();
            this.CloseButton = new SharpLoader.NSButton();
            this.nsLabel4 = new SharpLoader.NSLabel();
            this.ResultText = new SharpLoader.NSTextBox();
            this.VersionText = new SharpLoader.NSLabel();
            this.AuthorText = new SharpLoader.NSLabel();
            this.nsSeperator1 = new SharpLoader.NSSeperator();
            this.OutputText = new SharpLoader.NSTextBox();
            this.nsLabel3 = new SharpLoader.NSLabel();
            this.nsLabel2 = new SharpLoader.NSLabel();
            this.nsLabel1 = new SharpLoader.NSLabel();
            this.HashText = new SharpLoader.NSTextBox();
            this.SeedText = new SharpLoader.NSTextBox();
            this.CompileBtn = new SharpLoader.NSButton();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.Theme.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // Theme
            // 
            this.Theme.AccentOffset = 0;
            this.Theme.BorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Theme.Colors = new Bloom[0];
            this.Theme.Controls.Add(this.CloseButton);
            this.Theme.Controls.Add(this.nsLabel4);
            this.Theme.Controls.Add(this.ResultText);
            this.Theme.Controls.Add(this.VersionText);
            this.Theme.Controls.Add(this.AuthorText);
            this.Theme.Controls.Add(this.nsSeperator1);
            this.Theme.Controls.Add(this.OutputText);
            this.Theme.Controls.Add(this.nsLabel3);
            this.Theme.Controls.Add(this.nsLabel2);
            this.Theme.Controls.Add(this.nsLabel1);
            this.Theme.Controls.Add(this.HashText);
            this.Theme.Controls.Add(this.SeedText);
            this.Theme.Controls.Add(this.CompileBtn);
            this.Theme.Controls.Add(this.pictureBox1);
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
            this.Theme.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ThemeKeyDown);
            // 
            // CloseButton
            // 
            this.CloseButton.Location = new System.Drawing.Point(477, 2);
            this.CloseButton.Name = "CloseButton";
            this.CloseButton.Size = new System.Drawing.Size(21, 23);
            this.CloseButton.TabIndex = 16;
            this.CloseButton.Text = "X";
            this.CloseButton.Click += new System.EventHandler(this.CloseClick);
            // 
            // nsLabel4
            // 
            this.nsLabel4.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold);
            this.nsLabel4.Location = new System.Drawing.Point(12, 265);
            this.nsLabel4.Name = "nsLabel4";
            this.nsLabel4.Size = new System.Drawing.Size(56, 23);
            this.nsLabel4.TabIndex = 15;
            this.nsLabel4.Value1 = "Result:";
            this.nsLabel4.Value2 = "";
            // 
            // ResultText
            // 
            this.ResultText.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.ResultText.Location = new System.Drawing.Point(74, 265);
            this.ResultText.MaxLength = 32767;
            this.ResultText.Multiline = false;
            this.ResultText.Name = "ResultText";
            this.ResultText.ReadOnly = true;
            this.ResultText.Size = new System.Drawing.Size(108, 23);
            this.ResultText.TabIndex = 14;
            this.ResultText.Text = "- NOT COMPILED -";
            this.ResultText.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.ResultText.UseSystemPasswordChar = false;
            // 
            // VersionText
            // 
            this.VersionText.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.VersionText.Location = new System.Drawing.Point(312, 60);
            this.VersionText.Name = "VersionText";
            this.VersionText.Size = new System.Drawing.Size(172, 14);
            this.VersionText.TabIndex = 13;
            this.VersionText.Text = "nsLabel5";
            this.VersionText.Value1 = "Version: ";
            this.VersionText.Value2 = "";
            // 
            // AuthorText
            // 
            this.AuthorText.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold);
            this.AuthorText.Location = new System.Drawing.Point(312, 40);
            this.AuthorText.Name = "AuthorText";
            this.AuthorText.Size = new System.Drawing.Size(172, 23);
            this.AuthorText.TabIndex = 12;
            this.AuthorText.Text = "nsLabel4";
            this.AuthorText.Value1 = "SharpLoader by";
            this.AuthorText.Value2 = "";
            // 
            // nsSeperator1
            // 
            this.nsSeperator1.Location = new System.Drawing.Point(12, 93);
            this.nsSeperator1.Name = "nsSeperator1";
            this.nsSeperator1.Size = new System.Drawing.Size(476, 10);
            this.nsSeperator1.TabIndex = 10;
            this.nsSeperator1.Text = "nsSeperator1";
            // 
            // OutputText
            // 
            this.OutputText.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.OutputText.Font = new System.Drawing.Font("Verdana", 7F);
            this.OutputText.Location = new System.Drawing.Point(12, 128);
            this.OutputText.MaxLength = 32767;
            this.OutputText.Multiline = true;
            this.OutputText.Name = "OutputText";
            this.OutputText.ReadOnly = true;
            this.OutputText.Size = new System.Drawing.Size(476, 131);
            this.OutputText.TabIndex = 9;
            this.OutputText.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.OutputText.UseSystemPasswordChar = false;
            // 
            // nsLabel3
            // 
            this.nsLabel3.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.nsLabel3.Location = new System.Drawing.Point(213, 102);
            this.nsLabel3.Name = "nsLabel3";
            this.nsLabel3.Size = new System.Drawing.Size(66, 24);
            this.nsLabel3.TabIndex = 8;
            this.nsLabel3.Value1 = " Output:";
            this.nsLabel3.Value2 = "";
            // 
            // nsLabel2
            // 
            this.nsLabel2.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold);
            this.nsLabel2.Location = new System.Drawing.Point(12, 69);
            this.nsLabel2.Name = "nsLabel2";
            this.nsLabel2.Size = new System.Drawing.Size(45, 23);
            this.nsLabel2.TabIndex = 6;
            this.nsLabel2.Value1 = "Hash:";
            this.nsLabel2.Value2 = "";
            // 
            // nsLabel1
            // 
            this.nsLabel1.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold);
            this.nsLabel1.Location = new System.Drawing.Point(12, 40);
            this.nsLabel1.Name = "nsLabel1";
            this.nsLabel1.Size = new System.Drawing.Size(45, 23);
            this.nsLabel1.TabIndex = 5;
            this.nsLabel1.Value1 = "Seed:";
            this.nsLabel1.Value2 = "";
            // 
            // HashText
            // 
            this.HashText.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.HashText.Location = new System.Drawing.Point(63, 69);
            this.HashText.MaxLength = 32767;
            this.HashText.Multiline = false;
            this.HashText.Name = "HashText";
            this.HashText.ReadOnly = true;
            this.HashText.Size = new System.Drawing.Size(233, 23);
            this.HashText.TabIndex = 4;
            this.HashText.Text = "- NOT COMPILED -";
            this.HashText.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.HashText.UseSystemPasswordChar = false;
            // 
            // SeedText
            // 
            this.SeedText.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.SeedText.Location = new System.Drawing.Point(63, 40);
            this.SeedText.MaxLength = 32767;
            this.SeedText.Multiline = false;
            this.SeedText.Name = "SeedText";
            this.SeedText.ReadOnly = true;
            this.SeedText.Size = new System.Drawing.Size(233, 23);
            this.SeedText.TabIndex = 1;
            this.SeedText.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.SeedText.UseSystemPasswordChar = false;
            // 
            // CompileBtn
            // 
            this.CompileBtn.BackColor = System.Drawing.SystemColors.Control;
            this.CompileBtn.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.CompileBtn.Location = new System.Drawing.Point(415, 265);
            this.CompileBtn.Name = "CompileBtn";
            this.CompileBtn.Size = new System.Drawing.Size(73, 23);
            this.CompileBtn.TabIndex = 0;
            this.CompileBtn.Text = "COMPILE";
            this.CompileBtn.Click += new System.EventHandler(this.CompileClick);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBox1.BackgroundImage")));
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBox1.Location = new System.Drawing.Point(180, 114);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(382, 277);
            this.pictureBox1.TabIndex = 11;
            this.pictureBox1.TabStop = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(500, 300);
            this.Controls.Add(this.Theme);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SharpLoader";
            this.Theme.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private NSTheme Theme;
        private NSButton CompileBtn;
        private NSTextBox SeedText;
        private NSTextBox HashText;
        private NSLabel nsLabel1;
        private NSLabel nsLabel2;
        private NSLabel nsLabel3;
        private NSSeperator nsSeperator1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private NSLabel AuthorText;
        private NSLabel VersionText;
        public NSTextBox OutputText;
        private NSLabel nsLabel4;
        private NSTextBox ResultText;
        private NSButton CloseButton;
    }
}