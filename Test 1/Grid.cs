using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Test_1
{
    public partial class Grid : Form
    {
        //Graphics g = new Graphics();
        Pen pen = new Pen(Color.Black);
        private int zoomFactor = 1;
        private int baseSize = 10;
        public Grid()
        {
            this.ResizeRedraw = true;
            InitializeComponent();
            this.BackColor = Color.AliceBlue;
            this.Paint += Grid_Lines;
            //this.Paint += Draw_Line(new Point(1, 1), new Point(0, 0));
        }

        private void Grid_Lines(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawLine(pen, new Point(this.Width / 2, 0), new Point(this.Width / 2, this.Height));
            e.Graphics.DrawLine(pen, new Point(0, this.Height / 2), new Point(this.Width, this.Height / 2));
        }
        private void Draw_Line(object sender, PaintEventArgs e, Point p1, Point p2)
        {
            e.Graphics.DrawLine(pen, p1, p2);
        }

        private Point convert_point(double x, double y)
        {
            double actual_x = x / (zoomFactor * baseSize) * this.Width + this.Width / 2;
            double actual_y = y / (zoomFactor * baseSize) * this.Height + this.Height / 2;
            return new Point((int)actual_x, (int)actual_y);
        }

        private void add_numbers()
        {
            int actualSize = zoomFactor * baseSize;
            for (int i = 0; i < actualSize; i++)
            {
                Label l = new Label();
            }
        }

    }
}
