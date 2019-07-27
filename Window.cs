using System;
using System.Drawing;
using System.Windows.Forms;

namespace BrokenDetector
{
    public partial class Window : Form
    {
        Point lastPoint = new Point();
        TouhouGame tGame = new TouhouGame();

        public Window()
        {
            InitializeComponent();
        }

        private void Window_MouseDown(object sender, MouseEventArgs e)
        {
            lastPoint = new Point(-e.X, -e.Y);
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point mp = MousePosition;
                mp.Offset(lastPoint.X, lastPoint.Y);
                Location = mp;
            }
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void MinimizeButton_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void GameStateTimer_Tick(object sender, EventArgs e)
        {
            if (TouhouGame.IsRuning("th165"))
            {
                if (!tGame.IsAttached())
                {
                    tGame.Attach("th165");
                    gsLabel.ForeColor = Color.Green;

                } else
                {
                    float cState = tGame.ReadFloat(tGame.CameraChargeAddress);
                    if (cState < 0.9f)
                        tGame.WriteFloat(tGame.CameraChargeAddress, 1.0f);
                }
            } else
            {
                if (tGame.IsAttached())
                    tGame.Detach();
                gsLabel.ForeColor = Color.Red;
            }
        }
    }
}
