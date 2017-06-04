namespace SharpLoader
{
    partial class SelectForm
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
            this.CloseButton = new SharpLoader.NSButton();
            this.ContinueButton = new SharpLoader.NSButton();
            this.SelectButton = new SharpLoader.NSButton();
            this.PathText = new SharpLoader.NSTextBox();
            this.NotSelectedText = new SharpLoader.NSLabel();
            this.Theme.SuspendLayout();
            this.SuspendLayout();
            // 
            // Theme
            // 
            this.Theme.AccentOffset = 0;
            this.Theme.BorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Theme.Colors = new Bloom[0];
            this.Theme.Controls.Add(this.NotSelectedText);
            this.Theme.Controls.Add(this.CloseButton);
            this.Theme.Controls.Add(this.ContinueButton);
            this.Theme.Controls.Add(this.SelectButton);
            this.Theme.Controls.Add(this.PathText);
            this.Theme.Customization = "";
            this.Theme.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Theme.Font = new System.Drawing.Font("Verdana", 8F);
            this.Theme.Image = null;
            this.Theme.Location = new System.Drawing.Point(0, 0);
            this.Theme.Movable = true;
            this.Theme.Name = "Theme";
            this.Theme.NoRounding = false;
            this.Theme.Sizable = true;
            this.Theme.Size = new System.Drawing.Size(500, 100);
            this.Theme.SmartBounds = true;
            this.Theme.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Theme.TabIndex = 0;
            this.Theme.Text = "Select file";
            this.Theme.TransparencyKey = System.Drawing.Color.Empty;
            this.Theme.Transparent = false;
            this.Theme.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ThemeKeyDown);
            // 
            // CloseButton
            // 
            this.CloseButton.Location = new System.Drawing.Point(477, 2);
            this.CloseButton.Name = "CloseButton";
            this.CloseButton.Size = new System.Drawing.Size(21, 23);
            this.CloseButton.TabIndex = 17;
            this.CloseButton.Text = "X";
            this.CloseButton.Click += new System.EventHandler(this.CloseClick);
            // 
            // ContinueButton
            // 
            this.ContinueButton.Location = new System.Drawing.Point(212, 69);
            this.ContinueButton.Name = "ContinueButton";
            this.ContinueButton.Size = new System.Drawing.Size(75, 23);
            this.ContinueButton.TabIndex = 2;
            this.ContinueButton.Text = "CONTINUE";
            this.ContinueButton.Click += new System.EventHandler(this.ContinueClick);
            // 
            // SelectButton
            // 
            this.SelectButton.Location = new System.Drawing.Point(461, 40);
            this.SelectButton.Name = "SelectButton";
            this.SelectButton.Size = new System.Drawing.Size(27, 23);
            this.SelectButton.TabIndex = 1;
            this.SelectButton.Text = "...";
            this.SelectButton.Click += new System.EventHandler(this.SelectClick);
            // 
            // PathText
            // 
            this.PathText.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.PathText.Location = new System.Drawing.Point(12, 40);
            this.PathText.MaxLength = 32767;
            this.PathText.Multiline = false;
            this.PathText.Name = "PathText";
            this.PathText.ReadOnly = true;
            this.PathText.Size = new System.Drawing.Size(443, 23);
            this.PathText.TabIndex = 0;
            this.PathText.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.PathText.UseSystemPasswordChar = false;
            // 
            // NotSelectedText
            // 
            this.NotSelectedText.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.NotSelectedText.Location = new System.Drawing.Point(293, 69);
            this.NotSelectedText.Name = "NotSelectedText";
            this.NotSelectedText.Size = new System.Drawing.Size(90, 23);
            this.NotSelectedText.TabIndex = 18;
            this.NotSelectedText.Text = "nsLabel1";
            this.NotSelectedText.Value1 = "File not selected.";
            this.NotSelectedText.Value2 = "";
            this.NotSelectedText.Visible = false;
            // 
            // SelectForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(500, 100);
            this.Controls.Add(this.Theme);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "SelectForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SelectForm";
            this.Theme.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private NSTheme Theme;
        private NSTextBox PathText;
        private NSButton SelectButton;
        private NSButton ContinueButton;
        private NSButton CloseButton;
        private NSLabel NotSelectedText;
    }
}