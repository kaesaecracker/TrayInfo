using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace TrayInfo
{
    public partial class HiddenForm : Form
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        extern static bool DestroyIcon(IntPtr handle);

        private static PerformanceCounter cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        private static Microsoft.VisualBasic.Devices.ComputerInfo computerInfo = new Microsoft.VisualBasic.Devices.ComputerInfo();

        private static int cpuValuesIndex = 0;
        private static int?[] cpuValues = new int?[NUM_CPU_VALUES];
        private const int NUM_CPU_VALUES = 20;

        public HiddenForm()
        {
            InitializeComponent();

            for (int i = 0; i < NUM_CPU_VALUES; i++)
            {
                cpuValues[i] = null;
            }

            this.exitItem.Click += this.ExitClick;
            this.refreshTimer.Enabled = true;
        }

        private void ExitClick(object sender, EventArgs e)
        {
            this.ramIcon.Visible = false;
            this.cpuIcon.Visible = false;
            Application.Exit();
        }

        enum TextIconStyle
        {
            LINE_TOP,
            LINE_BOTTOM
        }

        private Icon CreateTextIcon(string str, out IntPtr hIcon, TextIconStyle s)
        {
            Font fontToUse = new Font("FiraCode", 16, FontStyle.Regular, GraphicsUnit.Pixel);
            Brush brushToUse = new SolidBrush(Color.White);
            Pen penToUse = new Pen(brushToUse);
            Bitmap bitmap = new Bitmap(16, 16);


            Graphics g = Graphics.FromImage(bitmap);
            g.Clear(Color.Transparent);
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
            g.DrawString(str, fontToUse, brushToUse, -4, -2);

            switch (s)
            {
                case TextIconStyle.LINE_TOP:
                    g.DrawLine(penToUse, new Point(0, 0), new Point(15, 0));
                    break;
                case TextIconStyle.LINE_BOTTOM:
                    g.DrawLine(penToUse, new Point(0, 15), new Point(15, 15));
                    break;
            }

            hIcon = bitmap.GetHicon();
            return Icon.FromHandle(hIcon);
        }

        private void RefreshTimerTick(object sender, EventArgs e)
        {

            ulong ramAvailiable = computerInfo.TotalPhysicalMemory;
            ulong ramFree = computerInfo.AvailablePhysicalMemory;
            ulong ramUsed = ramAvailiable - ramFree;
            ulong ramPercentage = ramUsed * 100 / ramAvailiable;

            this.ramIcon.Icon = CreateTextIcon(ramPercentage.ToString("00"), out IntPtr tempPtr, TextIconStyle.LINE_TOP);
            DestroyIcon(tempPtr);

            cpuValues[cpuValuesIndex++] = Convert.ToInt32(cpuCounter.NextValue());
            cpuValuesIndex = cpuValuesIndex < NUM_CPU_VALUES ? cpuValuesIndex : 0;

            int cnt = 0;
            int sum = 0;
            for (int i = 0; i < NUM_CPU_VALUES; i++)
            {
                if (cpuValues[i] != null)
                {
                    cnt++;
                    sum += cpuValues[i] ?? 0;
                }
            }

            int cpuPercentage = Convert.ToInt32(sum / cnt);
            this.cpuIcon.Icon = CreateTextIcon(cpuPercentage.ToString("00"), out tempPtr, TextIconStyle.LINE_BOTTOM);
            DestroyIcon(tempPtr);
        }
    }
}
