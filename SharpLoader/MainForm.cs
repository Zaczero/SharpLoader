using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharpLoader.Core;

namespace SharpLoader
{
    public partial class MainForm : Form
    {
        private int _lastSeed;

        public MainForm()
        {
            InitializeComponent();

            AuthorText.Value2 = Program.Author;
            VersionText.Value1 += Program.Version;
            SeedText.Text = Program.Seed.ToString();

            // Focus form
            Select();
        }

        private void ThemeKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter ||
                e.KeyCode == Keys.Space)
            {
                CompileClick(null, null);
            }
        }

        private void CompileClick(object sender, EventArgs e)
        {
            if (_lastSeed == Program.Seed)
            {
                Program.Seed = new Random(Environment.TickCount).Next(0, int.MaxValue);
                SeedText.Text = Program.Seed.ToString();
                Refresh();
            }

            OutputText.Text = string.Empty;

            var result = Program.Compile();

            if (Program.Hash != null)
            {
                HashText.Text = Program.Hash;
            }

            if (result == 0)
            {
                ResultText.Text = "SUCCESS";
                _lastSeed = Program.Seed;
            }
            else
            {
                ResultText.Text = "FAIL";
            }
        }

        private void CloseClick(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
