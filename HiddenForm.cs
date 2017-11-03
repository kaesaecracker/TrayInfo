using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace TrayInfo
{
    public partial class HiddenForm : Form
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        extern static bool DestroyIcon(IntPtr handle);

        public HiddenForm()
        {
            InitializeComponent();

            this.exitItem.Click += this.ExitClick;
            this.refreshTimer.Enabled = true;
        }

        private void ExitClick(object sender, EventArgs e)
        {
            this.ramIcon.Visible = false;
            Application.Exit();
        }

        public void CreateTextIcon(string str)
        {
            Font fontToUse = new Font("FiraCode", 16, FontStyle.Regular, GraphicsUnit.Pixel);
            Brush brushToUse = new SolidBrush(Color.White);
            Bitmap bitmap = new Bitmap(16, 16);


            Graphics g = System.Drawing.Graphics.FromImage(bitmap);
            g.Clear(Color.Transparent);
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
            g.DrawString(str, fontToUse, brushToUse, -4, -2);

            IntPtr hIcon = bitmap.GetHicon();
            this.ramIcon.Icon = Icon.FromHandle(hIcon);

            DestroyIcon(hIcon);
        }

        private void RefreshTimerTick(object sender, EventArgs e)
        {
            var ci = new Microsoft.VisualBasic.Devices.ComputerInfo();

            ulong availiable = ci.TotalPhysicalMemory;
            ulong free = ci.AvailablePhysicalMemory;

            ulong used = availiable - free;
            ulong percentage = used * 100 / availiable;

            CreateTextIcon(percentage.ToString());
        }
    }
}
