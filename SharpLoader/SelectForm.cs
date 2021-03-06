﻿using System;
using System.Threading;
using System.Windows.Forms;

namespace SharpLoader
{
    public partial class SelectForm : Form
    {
        public SelectForm()
        {
            InitializeComponent();

            // Focus form
            Theme.Select();
        }

        private void ThemeKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter ||
                e.KeyCode == Keys.Space)
            {
                SelectClick(null, null);
            }
        }

        private void CloseClick(object sender, EventArgs e)
        {
            Program.CleanTemp();
            Application.Exit();
        }

        private void ContinueClick(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(PathText.Text))
            {
                Program.ProcessZip(PathText.Text);
                Close();
            }
            else
            {
                NotSelectedText.Visible = true;

                var thr = new Thread(() =>
                    {
                        Thread.Sleep(600);
                        Invoke((MethodInvoker) delegate
                        {
                            NotSelectedText.Visible = false;
                        });
                    })
                    {IsBackground = true};
                thr.Start();
            }
        }

        private void SelectClick(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog
            {
                AddExtension = true,
                CheckFileExists = true,
                CheckPathExists = true,
                Filter = "Compressed File (ZIP)|*.zip",
                DefaultExt = ".zip",
            };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                PathText.Text = ofd.FileName;
            }
        }
    }
}
