using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace Test_1
{
    class Line
    {
        private int serial;
        private string p1x, p1y;
        private string p2x, p2y;
        private Queue<string> p1xq, p2xq, p1yq, p2yq;
        private double X1, Y1, X2, Y2;
        private Pen pen;

        public Line(string p1x, string p1y, string p2x, string p2y, Color c, int serial)
        {
            this.p1x = p1x;
            this.p1y = p1y;
            if (this.p1x == "") this.p1x = "0";
            if (this.p1y == "") this.p1y = "0";
            this.p2x = p2x;
            this.p2y = p2y;
            if (this.p2x == "") this.p2x = "0";
            if (this.p2y == "") this.p2y = "0";
            this.p1xq = Shunting.Eval(Shunting.Convert(p1x));
            this.p2xq = Shunting.Eval(Shunting.Convert(p2x));
            this.p1yq = Shunting.Eval(Shunting.Convert(p1y));
            this.p2yq = Shunting.Eval(Shunting.Convert(p2y));
            this.pen = new Pen(c, 3);
            this.serial = serial;
        }
        public int GetSerial()
        {
            return this.serial;
        }
        public void Draw(Graphics g)
        {
            Globals.RemoveError(serial);
            try
            {
            this.X1 = Shunting.Calc(new Queue<string>(p1xq));
            this.Y1 = Shunting.Calc(new Queue<string>(p1yq));
            this.X2 = Shunting.Calc(new Queue<string>(p2xq));
            this.Y2 = Shunting.Calc(new Queue<string>(p2yq));
            }
            catch(Exception e)
            {
                Globals.ShowError("¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯", e.Message,this.serial);
                return;
            }

            Point p1 = Globals.convert_point_unbounded(X1, Y1);
            Point p2 = Globals.convert_point_unbounded(X2, Y2);
            try
            {
                g.DrawLine(pen, p1, p2);
            }
            catch { }
        }
    }
}
