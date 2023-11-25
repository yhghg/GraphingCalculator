using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace Test_1
{

    class SpecialPointControl : GridPointControl
    {
        private static int num;
        public SpecialPointControl(string Xeq, string Yeq) : base(Xeq, Yeq)
        {
            this.Name = "IntersectionPoint" + num;
            num++;
            this.color = Color.Black;
            this.pen = new Pen(Color.Black, 3);
            this.menu = new ContextMenuStrip();
            this.menu.Items.Add("Create point here...");
            this.menu.Items.Add("Create Line...");
            this.menu.ItemClicked += menu_ItemClicked;
        }

        
    }
    class UserPointControl : GridPointControl
    {
        private static int num;
        private int serial;
        private Queue<string> Xqueue;
        private Queue<string> Yqueue;
        public UserPointControl(string Xeq, string Yeq, Color color, int serial) : base(Xeq, Yeq)
        {
            this.Name = "GridPoint" + num;
            num++;
            this.color = color;
            this.pen = new Pen(color, 3);
            this.serial = serial;
            this.Xqueue = Shunting.Eval(Shunting.Convert(Xeq));
            this.Yqueue = Shunting.Eval(Shunting.Convert(Yeq));
            this.menu = new ContextMenuStrip();
            //this.menu.Items.Add("Create point here...");
            this.menu.Items.Add("Create Line...");
            this.menu.ItemClicked += menu_ItemClicked;

            //this.BackColor = Color.Transparent;

            //if (this.X > baseSize * zoomFactor + offset_x || this.X < -baseSize * zoomFactor + offset_x || this.Y > baseSize * zoomFactor + offset_y || this.Y < -baseSize * zoomFactor + offset_y) return; //point is offscreen
            //Point p1 = Globals.convert_point(X, Y);
            //p1.X -= 4;
            //p1.Y -= 4;
            //this.Location = p1;
            //this.Visible = true;
            //this.Paint += GridPointControl_Paint;
        }

        public int GetSerial()
        {
            return this.serial;
        }
        public override void RefreshPoint()
        {
            try
            {
                this.X = Shunting.Evaluate(this.Xeq);
                this.Y = Shunting.Evaluate(this.Yeq);
            }
            catch (Exception e)
            {
                Globals.ShowError("¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯", e.Message, serial);
                this.Visible = false;
                return;
            }
            //Globals.RemoveError(serial);
            if (double.IsNaN(this.X) || double.IsNaN(this.Y)) return;
            double lowest_x = -Globals.zoomX / 2 + Globals.offset_x;
            double highest_x = Globals.zoomX / 2 + Globals.offset_x;
            double lowest_y = -Globals.zoomY / 2 + Globals.offset_y;
            double highest_y = Globals.zoomY / 2 + Globals.offset_y;

            if (this.X > highest_x || this.X < lowest_x || this.Y > highest_y || this.Y < lowest_y)
            {
                this.Visible = false; //point is offscreen
                return;
            }
            //Point p1 = Globals.convert_point(X, Y);
            //this.Location = p1;
            this.Visible = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Globals.toolTip1.SetToolTip(this, String.Format("({0:f2},{1:f2})", this.X, this.Y));
            Globals.RemoveError(serial);
            try
            {
                //this.RefreshPoint();
                this.X = Shunting.Evaluate(Xeq);
                this.Y = Shunting.Evaluate(Yeq);
            }
            catch(Exception ex)
            {
                Globals.ShowError("¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯", ex.Message, serial);
            }
            Point p1 = Globals.convert_point(this.X, this.Y);
            p1.X -= 4; //center the point around where it's corner was (it's supposed to be there)
            p1.Y -= 4;
            bool draw = true;

            this.pen.Color = this.color;
            this.Location = p1;
            if (draw) e.Graphics.FillEllipse(new SolidBrush(this.pen.Color), e.ClipRectangle); //draw the point
            if (this.hover) e.Graphics.DrawEllipse(new Pen(Color.FromArgb(100, this.pen.Color), 10), e.ClipRectangle); //add a little outline to the point if hovering
            e.Dispose();
        }
    }

    class GridPointControl : Control
    {
        protected string Xeq; //equation for x coordinate
        protected string Yeq; //equation for y coordinate
        protected double X; //actual x coordinate
        protected double Y; //actual y coordinate
        protected Pen pen; //pen to draw this with
        protected Color color; //color of this point
        protected bool hover; //is the mouse currently hovering over this point?
        protected ContextMenuStrip menu; //right click menu

        public GridPointControl(string Xeq, string Yeq)
        {
            SetStyle(ControlStyles.Opaque, true); //makes this prettier?
            UpdateStyles();
            this.Xeq = Xeq;
            this.Yeq = Yeq;
            if (Xeq == "") this.Xeq = "0";
            if (Yeq == "") this.Yeq = "0";
            //this.X = Shunting.Evaluate(this.Xeq);
            //this.Y = Shunting.Evaluate(this.Yeq);
            this.Height = 8;
            this.Width = 8;
            this.Cursor = Cursors.Arrow;

            GraphicsPath gp = new GraphicsPath();
            gp.AddEllipse(this.ClientRectangle);
            this.Region = new Region(gp); //make this control a circle
            Globals.toolTip1.SetToolTip(this, String.Format("({0:f2},{1:f2})", this.X, this.Y)); //add tooltip (hover label)
        }
        public Point GetCoords()
        {
            this.X = Shunting.Evaluate(Xeq);
            this.Y = Shunting.Evaluate(Yeq);
            return Globals.convert_point(this.X, this.Y);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            Globals.toolTip1.SetToolTip(this, String.Format("({0:f2},{1:f2})", this.X, this.Y));
            try
            {
                //this.RefreshPoint();
                this.X = Shunting.Evaluate(Xeq);
                this.Y = Shunting.Evaluate(Yeq);
            }
            catch { }
            Point p1 = Globals.convert_point(this.X, this.Y);
            p1.X -= 4; //center the point around where it's corner was (it's supposed to be there)
            p1.Y -= 4;
            bool draw = true;

            this.pen.Color = this.color;
            this.Location = p1;
            if (draw) e.Graphics.FillEllipse(new SolidBrush(this.pen.Color), e.ClipRectangle); //draw the point
            if (this.hover) e.Graphics.DrawEllipse(new Pen(Color.FromArgb(100, this.pen.Color), 10), e.ClipRectangle); //add a little outline to the point if hovering
            e.Dispose();
        }
        protected override void OnPaintBackground(PaintEventArgs pevent)  //basically the same as OnPaint
        {
            try
            {
                this.X = Shunting.Evaluate(Xeq);
                this.Y = Shunting.Evaluate(Yeq);
            }
            catch { }
            Point p1 = Globals.convert_point(this.X, this.Y);
            p1.X -= 4;
            p1.Y -= 4;
            base.OnPaintBackground(pevent);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            this.hover = true;
            this.Refresh();
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            this.hover = false;
            this.Refresh();
        }
        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            switch (e.Button)
            {
                case MouseButtons.Right: //show right click menu
                    this.menu.Show(this, new Point(0, 0));
                    break;
                case MouseButtons.Left when Globals.LineFromPoint != null && Globals.LineFromPoint != this: //endpoint of a line
                    int dec = Math.Max(2, -((int)Math.Floor(Math.Log10(Globals.zoomX))));
                    (this.FindForm() as Form1).AddFunction_public(String.Format($"[{{0:f{dec}}},{{1:f{dec}}},{{2:f{dec}}},{{3:f{dec}}}]", Globals.LineFromPoint.X, Globals.LineFromPoint.Y, this.X, this.Y));
                    Globals.LineFromPoint = null;
                    break;
            }
        }

        public virtual void RefreshPoint()
        {
            try
            {
                this.X = Shunting.Evaluate(this.Xeq);
                this.Y = Shunting.Evaluate(this.Yeq);
            }
            catch { }
            if (double.IsNaN(this.X) || double.IsNaN(this.Y)) return;
            double lowest_x = -Globals.zoomX / 2 + Globals.offset_x;
            double highest_x = Globals.zoomX / 2 + Globals.offset_x;
            double lowest_y = -Globals.zoomY / 2 + Globals.offset_y;
            double highest_y = Globals.zoomY / 2 + Globals.offset_y;

            if (this.X > highest_x || this.X < lowest_x || this.Y > highest_y || this.Y < lowest_y)
            {
                this.Visible = false; //point is offscreen
                return;
            }
            this.Visible = true;
        }

        protected void menu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            switch (e.ClickedItem.Text)
            {
                case "Create point here...":
                    int dec = Math.Max(2, -((int)Math.Floor(Math.Log10(Globals.zoomX)))); //2 or more decimal places depending on how small this is
                    (this.FindForm() as Form1).AddFunction_public(String.Format($"({{0:f{dec}}},{{1:f{dec}}})", this.X, this.Y)); //add a point when this point is
                    break;
                case "Create Line...": //start a line from this point
                    Globals.LineFromPoint = this;
                    break;
            }
        }

    }
    
}
