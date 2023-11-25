using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace Test_1
{
    public partial class Form1 : Form
    {
        private Graphics g = null;
        private Pen pen = new Pen(Color.Black);
        //public Bitmap bitmap = new Bitmap(1, 1);

        //public int functions = 1;
        //public int points = 1;

        //public Equation[] equations = new Equation[5];
        //public Color[] colors = { Color.Red, Color.Blue, Color.Green, Color.Purple, Color.Yellow };

        //public double zoomFactor = 1;
        //public double baseSize = 20;
        //public double offset_x = 0;
        //public double offset_y = 0;
        //public bool refreshNeeded = true;
        //public int dull = 0;

        public Form1()
        {
            this.ResizeRedraw = true;
            InitializeComponent();
            grid2.BackColor = Color.White; //set background color to white
            this.grid2.MouseWheel += grid2_MouseWheel; //add functionality of grid2_MouseWheel to the mousewheel
            this.tableLayoutPanel1.BackColor = Color.FromArgb(255, 178, 206, 222);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true); //some kind of optimization
            Globals.toolTip1 = new ToolTip();

            // set up the delays for the tooltip
            Globals.toolTip1.InitialDelay = 1;
            Globals.toolTip1.ReshowDelay = 500;
            // force the tooltip text to be displayed whether or not the form is active
            Globals.toolTip1.ShowAlways = true;
            Timer scrollRefresh = new Timer();
            scrollRefresh.Interval = 200;
            scrollRefresh.Tick += ScrollRefresh_Tick;
            scrollRefresh.Start();
            Timer general = new Timer();
            general.Interval = 20;
            general.Tick += General_Tick;
            general.Start();
            Globals.grid2 = grid2;
            Globals.flowLayoutPanel1 = flowLayoutPanel1;
            Globals.flowLayoutPanel2 = flowLayoutPanel2;
        }
        bool changed = false;
        private void General_Tick(object sender, EventArgs e)
        {
            Globals.time = (Globals.time + 1) % 1000000;
            if (Globals.quality < 0.99)
            {
                Globals.quality = Math.Min(1, 2 * Globals.quality);
                grid2.Refresh();
            }
            //if(Globals.time%5==0)
            //{
            //    if(Globals.currentQual<3)
            //    {
            //        Globals.currentQual++;
            //        grid2.Refresh();
            //    }

            //}
        }

        private void ScrollRefresh_Tick(object sender, EventArgs e)
        {
            if (needScrollRefresh && Globals.HighlightedActualFunction != null && (DateTime.Now - lastScroll).TotalMilliseconds > 300) //if user finished scrolling in the last little while
            {
                Globals.HighlightedActualFunction.AddSpecialPoints(); //add back special points
                needScrollRefresh = false;
            }
        }

        //protected override CreateParams CreateParams
        //{
        //    get
        //    {
        //        CreateParams createParams = base.CreateParams;
        //        createParams.ExStyle |= 0x00000020;
        //        return createParams;
        //    }
        //}

        private void AddFunction_Click(object sender, EventArgs e)
        {
            this.flowLayoutPanel1.Controls.Remove(AddFunction); //button needs to move 1 place down so we get rid of it first
            Globals.functions++;
            TextBox t = new TextBox(); //create the new textbox
            t.Name = "function" + Globals.functions; //name of the text box is based on how many functions have been created so that every function has a unique name
            t.TextChanged += this.function1_TextChanged;
            t.ForeColor = Globals.colors[(Globals.functions - 1) % 5]; //alternate between 5 colors
            this.flowLayoutPanel1.Controls.Add(t);
            this.flowLayoutPanel1.Controls.Add(AddFunction);
            Button b = new Button(); //add the button to remove this newly created function
            b.Name = "RemoveF" + Globals.functions;
            b.Size = new Size(30, 20);
            b.BackColor = Color.FromArgb(225, 225, 225);
            b.Text = "-";
            b.Click += RemoveF1_Click;
            this.flowLayoutPanel2.Controls.Add(b);
            grid2.Refresh();
        }
        public void AddFunction_public(string function)
        {
            this.flowLayoutPanel1.Controls.Remove(AddFunction);
            Globals.functions++;
            TextBox t = new TextBox();
            t.Name = "function" + Globals.functions;
            //t.Name = "function";
            t.TextChanged += this.function1_TextChanged;
            t.ForeColor = Globals.colors[(Globals.functions - 1) % 5];
            t.Text = function;
            //t.Visible = true;
            this.flowLayoutPanel1.Controls.Add(t);
            this.flowLayoutPanel1.Controls.Add(AddFunction);
            Button b = new Button();
            b.Name = "RemoveF" + Globals.functions;
            b.Size = new Size(30, 20);
            b.BackColor = Color.FromArgb(225, 225, 225);
            b.Text = "-";
            b.Click += RemoveF1_Click;
            this.flowLayoutPanel2.Controls.Add(b);
            grid2.Refresh();
            //button1.Visible = true;
            //t.Location = new Point(textboxLocation,20);
            //textboxLocation += t.Width+50;

        }

        private TrackBar AddSlider(double serial, TextBox tb)
        {
            TrackBar b = new TrackBar();
            b.Name = "Slider" + serial;
            if (this.flowLayoutPanel1.Controls[b.Name] != null) return this.flowLayoutPanel1.Controls[b.Name] as TrackBar; //if there is already a slider with this name then we don't need another one
            int index = this.flowLayoutPanel1.Controls.GetChildIndex(tb);
            b.Minimum = 0;
            b.Maximum = 1000;
            b.TickFrequency = 1;
            b.Scroll += B_Scroll;
            b.TickStyle = TickStyle.None;
            b.ValueChanged += B_ValueChanged;
            b.AutoSize = false;
            b.Height = tb.Height;
            b.AccessibleName = tb.Text.Split('=')[0].Trim();
            this.flowLayoutPanel1.Controls.Add(b);
            this.flowLayoutPanel1.Controls.SetChildIndex(b, index + 1);
            this.flowLayoutPanel2.Controls.Find("RemoveF" + serial, true).FirstOrDefault().Height = tb.Height * 2 + 5; //change the width of the remove button of this function this slider takes space
            return b;
        }

        private void B_ValueChanged(object sender, EventArgs e)
        {
            if(Globals.HighlightedActualFunction != null)
            {
                Globals.HighlightedActualFunction.SetHighlighted(false);
                Globals.HighlightedActualFunction = null;
            }
            Variable var;
            if (!Globals.variables.TryGetValue((sender as TrackBar).AccessibleName, out var)) return;
            Globals.toolTip1.SetToolTip(sender as Control, var.GetValue().ToString());
        }

        private void B_Scroll(object sender, EventArgs e)
        {
            grid2.Refresh();
        }

        private List<string> GetStrings(string text) //gets a string and returns a list of all the comma-separated entries (for example: "(1,2+2)" -> List("(1","2+2)") deals with functions that have a comma in their inputs (min and max functions) so that it works correctly.
        {
            List<string> ls = new List<string>();
            string s = "";
            int count = 0;
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == ',') count++;
                if (count > 0)
                {
                    ls.Add(s);
                    s = "";
                    count = 0;
                    continue;
                }
                if (text[i] == 'n' || text[i] == 'x')
                {
                    if (i > 2 && (text.Substring(i - 2, 3) == "min" || text.Substring(i - 2, 3) == "max"))
                    {
                        count--;
                    }
                }
                s += text[i];
            }
            if (s != "") ls.Add(s);
            return ls;
        }

        private void function1_TextChanged(object sender, EventArgs e)
        {
            Globals.quality = Globals.defaultQuality;
            (sender as TextBox).BackColor = Color.White; //make background white (this is relevant just for "setting" textboxes because they sometimes change color)
            string txt = (sender as TextBox).Text;
            int serial = int.Parse((sender as TextBox).Name.Substring(8));
            Globals.Remove(serial); //remove whatever this textbox represents because we are about to change it
            Globals.RemoveError(serial);
            if (txt == "") //probably just deleted something and we need to refresh to remove it
            {
                grid2.Refresh();
                return;
            }
            try
            {
                if (GetStrings(txt).Count() != 1 && GetStrings(txt).Count() != 2 && GetStrings(txt).Count() != 4) throw new Exception("Sorry, I don't understand what you're trying to do");
                if (txt[0] == '(' && txt[txt.Length - 1] == ')' && GetStrings(txt).Count()==2) //is a point (looks like (a,b))
                {
                    string x, y;
                    List<string> ls = GetStrings(txt);
                    x = ls[0].Substring(1);
                    y = ls[1].Substring(0, ls[1].Length - 1);
                    Globals.AddPoint(new UserPointControl(x, y, Globals.colors[(serial - 1) % 5], serial));
                }
                else
                {
                    if (txt[0] == '[' && txt[txt.Length - 1] == ']' && GetStrings(txt).Count() == 4) //starts with a square bracket and has 3 commas (looks like [x1,y1,x2,y2])
                    {
                        string x1, y1, x2, y2;
                        List<string> ls = GetStrings(txt);
                        x1 = ls[0].TrimStart('[');
                        y1 = ls[1];
                        x2 = ls[2];
                        y2 = ls[3].TrimEnd(']');
                        Globals.AddLine(new Line(x1, y1, x2, y2, Globals.colors[(serial - 1) % 5], serial));
                    }
                    else
                    {
                        if (txt.Replace(" ", "").Contains("(x)=")) // function with a name (like f(x) = x^2+3*x...)
                        {
                            string name = txt.Split('=')[0].Replace(" ", "").Replace("(x)", "");
                            Function func = new Function(txt.Split('=')[1].Replace(" ", ""), Globals.colors[(serial - 1) % 5], serial);
                            Globals.AddNamedFunction(name, func);
                            Globals.AddFunction(func);
                        }
                        else
                        {
                            if (txt[0] != '{' && txt.Replace(" ", "").Contains("=")) //variable (like a=3 or b=a+f(5))
                            {
                                string name = txt.Replace(" ", "").Split('=')[0];
                                if (txt.Contains("[") && txt.Contains(']')) //variable with a range (like w=[1,5])
                                {
                                    string range = txt.Replace(" ", "").Split('=')[1];
                                    double min = double.Parse(range.Split(',')[0].TrimStart('['));
                                    double max = double.Parse(range.Split(',')[1].TrimEnd(']'));
                                    TrackBar b = AddSlider(serial, (sender as TextBox));
                                    Globals.AddVariable(name, b, min, max, serial);
                                }
                                else
                                {
                                    string eq = txt.Replace(" ", "").Split('=')[1];
                                    Shunting.Evaluate(eq);
                                    Globals.AddVariable(name, eq, serial);
                                }
                            }
                            else
                            {
                                if (((txt.Length > 7 && (txt.Substring(0, 7) == "{xzoom=" || txt.Substring(0, 7) == "{yzoom=")) || (txt.Length > 9 && (txt.Substring(0, 9) == "{xoffset=" || txt.Substring(0, 9) == "{yoffset="))) && txt[txt.Length - 1] == '}') //setting (looks like {xzoom=7})
                                {
                                    switch (txt.Substring(0, 3)) //change relevant setting
                                    {
                                        case "{xz":
                                            if (Globals.zoomXID != -1) break;
                                            Globals.zoomXeq = txt.Substring(7).TrimEnd('}');
                                            Globals.zoomXID = serial;
                                            (sender as TextBox).BackColor = Color.AntiqueWhite;
                                            break;
                                        case "{yz":
                                            if (Globals.zoomYID != -1) break;
                                            Globals.zoomYeq = txt.Substring(7).TrimEnd('}');
                                            Globals.zoomYID = serial;
                                            (sender as TextBox).BackColor = Color.AntiqueWhite;
                                            break;
                                        case "{xo":
                                            if (Globals.offsetXID != -1) break;
                                            Globals.offsetXeq = txt.Substring(9).TrimEnd('}');
                                            Globals.offsetXID = serial;
                                            (sender as TextBox).BackColor = Color.AntiqueWhite;
                                            break;
                                        case "{yo":
                                            if (Globals.offsetYID != -1) break;
                                            Globals.offsetYeq = txt.Substring(9).TrimEnd('}');
                                            Globals.offsetYID = serial;
                                            (sender as TextBox).BackColor = Color.AntiqueWhite;
                                            break;
                                        default:
                                            break;

                                    }
                                }
                                else //if it's not any of the other things we just assume it's a function
                                {
                                    Globals.AddFunction(new Function(txt, Globals.colors[(serial - 1) % 5], serial));
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception err) //if anything wrong happens, show error and remove this object
            {
                Globals.ShowError("¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯", err.Message, serial);
                Globals.Remove(serial);
            }
            grid2.Refresh();
        }

        //private (Function[],GridPointControl[]) GetFunctions()
        //{
        //    int amountF = 0;
        //    int amountP = 0;

        //    //for (int i = 1; i <= functions; i++)
        //    //{
        //    //    t = this.Controls.Find("function" + i, true).FirstOrDefault() as TextBox;
        //    //    equations[i - 1] = new Function(t.Text,colors[(i-1) % 5],this);
        //    //}
        //    foreach (Control control in flowLayoutPanel1.Controls)
        //    {
        //        if (control.Name.Contains("function"))
        //        {
        //            if(control.Text!="" && control.Text[0] == '(') { amountP++; }
        //            else { amountF++; }
        //        }

        //    }
        //    Function[] equations = new Function[amountF];
        //    GridPointControl[] points = new GridPointControl[amountP];
        //    Control[] ps = new Control[amountP];
        //    Control[] fs = new Control[amountF];
        //    amountF = 0;
        //    amountP = 0;
        //    foreach (Control control in flowLayoutPanel1.Controls)
        //    {
        //        if (control.Name.Contains("function"))
        //        {
        //            if (control.Text != "" && control.Text[0] == '(') 
        //            {
        //                ps[amountP] = control;
        //                amountP++;
        //            }
        //            else 
        //            {
        //                fs[amountF] = control;
        //                amountF++;
        //            }
        //        }
        //    }


        //    //t = this.Controls.Find("function", true);
        //    for (int i = 0; i < amountF; i++)
        //    {
        //        equations[i] = new Function((fs[i] as TextBox).Text, colors[(int.Parse((fs[i] as TextBox).Name.Substring(8)) - 1) % 5], this);
        //    }
        //    double x, y;
        //    int comma;
        //    for (int i = 0; i < amountP; i++)
        //    {
        //        try
        //        {
        //            comma = (ps[i] as TextBox).Text.IndexOf(',');
        //            x = double.Parse((ps[i] as TextBox).Text.Substring(1, comma - 1));
        //            y = double.Parse((ps[i] as TextBox).Text.Substring(comma + 1).TrimEnd(')'));
        //            points[i] = new GridPointControl(x, y, colors[(int.Parse((ps[i] as TextBox).Name.Substring(8)) - 1) % 5], this);
        //        }
        //        catch 
        //        {
        //            x = double.NaN;
        //            y = double.NaN;
        //            points[i] = new GridPointControl(x, y, colors[(int.Parse((ps[i] as TextBox).Name.Substring(8)) - 1) % 5], this);
        //        }

        //    }
        //    return (equations,points);

        //}

        //private void grid_Paint(object sender, PaintEventArgs e)
        //{
        //    //if (false)
        //    //{
        //    //    refreshNeeded = false;
        //    //    grid.Refresh();
        //    //    grid.BackColor = Color.AliceBlue;
        //    //    g = grid.CreateGraphics();
        //    //    double lowest_x = -baseSize * zoomFactor + offset_x;
        //    //    double highest_x = baseSize * zoomFactor + offset_x;
        //    //    double lowest_y = -baseSize * zoomFactor + offset_y;
        //    //    double highest_y = baseSize * zoomFactor + offset_y;
        //    //    g.DrawLine(pen, convert_point(0, lowest_y), convert_point(0, highest_y));
        //    //    g.DrawLine(pen, convert_point(lowest_x, 0), convert_point(highest_x, 0));
        //    //    //g.DrawLine(pen, new Point(0, grid.Height / 2), new Point(grid.Width, grid.Height / 2));
        //    //    //g.DrawString("0", ariel, drawBrush, convert_point(0, 0));
        //    //    place_numbers();
        //    //    Debug.WriteLine("banana");

        //    //}
        //}



        //private void place_numbers()
        //{
        //    double step = Round(baseSize * zoomFactor / 10, (int)Math.Floor(Math.Log10(baseSize * zoomFactor)) - 1); //we want (about) 10 numbers, then we want (the differences between) them to look nice so we round to an appropriate power of 10
        //    double num = (-baseSize * zoomFactor + offset_y) - (-baseSize * zoomFactor + offset_y) % step; //lowest number on the screen, rounded to a multiple of the step
        //    //int step = 1;
        //    //(int)Math.Floor(Math.Log10(baseSize * zoomFactor / 2))
        //    while (num <= baseSize * zoomFactor + offset_y)
        //    {
        //        num += step;
        //        g.DrawString(num.ToString(), ariel, drawBrush, Globals.convert_point(0, num));
        //    }
        //    //g.DrawString("0", new Font("ariel", 10), drawBrush, convert_point(0, 0));
        //    num = (-baseSize * zoomFactor + offset_x) - (-baseSize * zoomFactor + offset_x) % step;
        //    while (num <= baseSize * zoomFactor + offset_x)
        //    {
        //        num += step;
        //        g.DrawString(num.ToString(), ariel, drawBrush, Globals.convert_point(num, 0));
        //    }
        //}

        private double Mod(double x, double m)
        {
            return (x % m + m) % m;
        }

        private double HowClose(double x, double m) //returns how close x is to the closest multiple of m - there is probably an easier way to do this but i like this
        {
            //return m/2 - Math.Abs(Math.Abs(Mod(x, 2*m) - m) - m/2);
            return Math.Abs(x - Math.Round(x / m) * m);
        }
        private void place_numbers()
        {
            SolidBrush drawBrush = new SolidBrush(Color.Black);
            SolidBrush lightBrush = new SolidBrush(Color.Gray); //for offscreen numbers
            SolidBrush brush;
            double lowest_x = -Globals.zoomX / 2 + Globals.offset_x;
            double highest_x = Globals.zoomX / 2 + Globals.offset_x;
            double lowest_y = -Globals.zoomY / 2 + Globals.offset_y;
            double highest_y = Globals.zoomY / 2 + Globals.offset_y;
            Font ariel = new Font("ariel", 11);
            double step = Round(Globals.zoomY / 10, (int)Math.Floor(Math.Log10(Globals.zoomY)) - 1); //we want (about) 10 numbers, then we want them (and the differences between them) to look nice so we round to an appropriate power of 10
            double num = (-Globals.zoomY / 2 + Globals.offset_y) - (-Globals.zoomY / 2 + Globals.offset_y) % step; //lowest number on the screen, rounded to a multiple of the step
            int dec = Math.Max(0, -((int)Math.Floor(Math.Log10(Globals.zoomY)) - 1)); //round small numbers to a couple decimal places to prevent precision errors
            Point p;
            while (num <= Globals.zoomY / 2 + Globals.offset_y) //y axis
            {
                p = Globals.convert_point(0, num);
                if (p.X > grid2.Width - 10 * (num.ToString($"N{dec}").Length) || p.X < 155 * (disappear? 0:1)) //offscreen numbers are drawn at the edge of the screen and in lighter color
                {
                    p.X = Math.Max(Math.Min(p.X, grid2.Width - 10 * (num.ToString($"N{dec}").Length)), 155);
                    brush = lightBrush;
                }
                else
                {
                    brush = drawBrush;
                }
                g.DrawString(num.ToString($"N{dec}"), ariel, brush, p);
                num += step;
            }
            step = Round(Globals.zoomX / 10, (int)Math.Floor(Math.Log10(Globals.zoomX)) - 1);
            num = (-Globals.zoomX / 2 + Globals.offset_x) - (-Globals.zoomX / 2 + Globals.offset_x) % step;
            dec = Math.Max(0, -((int)Math.Floor(Math.Log10(Globals.zoomX)) - 1));
            while (num <= Globals.zoomX / 2 + Globals.offset_x) //x axis
            {
                p = Globals.convert_point(num, 0);
                if (p.Y > grid2.Height-10 || p.Y < 10)
                {
                    p.Y = Math.Max(Math.Min(p.Y, grid2.Height - 15), 5);
                    brush = lightBrush;
                }
                else
                {
                    brush = drawBrush;
                }
                g.DrawString(num.ToString($"N{dec}"), ariel, brush, p);
                num += step;
            }
        }

        private void draw_lines()
        {
            //draw main axis
            double lowest_x = -Globals.zoomX / 2 + Globals.offset_x;
            double highest_x = Globals.zoomX / 2 + Globals.offset_x;
            double lowest_y = -Globals.zoomY / 2 + Globals.offset_y;
            double highest_y = Globals.zoomY / 2 + Globals.offset_y;
            g.DrawLine(pen, Globals.convert_point(0, lowest_y), Globals.convert_point(0, highest_y));
            g.DrawLine(pen, Globals.convert_point(lowest_x, 0), Globals.convert_point(highest_x, 0));

            //draw other lines
            double step = Round(Globals.zoomX / 10, (int)Math.Floor(Math.Log10(Globals.zoomX)) - 1) / 5; //same line from draw_numbers but now it's a 50th of the screen (and not a 10th) because we want 5 lines between the numbers
            double num = lowest_x - lowest_x % step; //lowest number on the screen, rounded to a multiple of the step
            Pen light = new Pen(Color.FromArgb(20, 0, 0, 0), 1);
            Pen bold = new Pen(Color.FromArgb(70, 0, 0, 0), 1);
            double NumbersStep = Round(Globals.zoomX / 10, (int)Math.Floor(Math.Log10(Globals.zoomX)) - 1); //same line from draw_numbers exactly (10th of the screen)
            double epsilon = NumbersStep / 10;
            while (num <= highest_x) //x axis
            {
                if (HowClose(num, NumbersStep) < epsilon) //if current num is (sufficiently close to) a multiple of NumbersStep, it means that it's a 10th of the screen and needs to be bold
                    g.DrawLine(bold, Globals.convert_point(num, lowest_y), Globals.convert_point(num, highest_y));
                else
                    g.DrawLine(light, Globals.convert_point(num, lowest_y), Globals.convert_point(num, highest_y));
                num += step;
            }
            step = Round(Globals.zoomY / 10, (int)Math.Floor(Math.Log10(Globals.zoomY)) - 1) / 5;
            num = lowest_y - lowest_y % step;
            NumbersStep = Round(Globals.zoomY / 10, (int)Math.Floor(Math.Log10(Globals.zoomY)) - 1);
            epsilon = NumbersStep / 10;
            while (num <= highest_y) //y axis
            {
                if (HowClose(num, NumbersStep) < epsilon)
                    g.DrawLine(bold, Globals.convert_point(lowest_x, num), Globals.convert_point(highest_x, num));
                else
                    g.DrawLine(light, Globals.convert_point(lowest_x, num), Globals.convert_point(highest_x, num));
                num += step;
            }
        }

        private void grid2_Paint(object sender, PaintEventArgs e)
        {
            Globals.RefreshPoints();
            try
            {
                double offset_x, offset_y, zoomX, zoomY;
                if (Globals.offsetXeq != "$")
                {
                    offset_x = Shunting.Evaluate(Globals.offsetXeq);
                    if (!Double.IsNaN(offset_x)) Globals.offset_x = offset_x;
                }
                if (Globals.offsetYeq != "$")
                {
                    offset_y = Shunting.Evaluate(Globals.offsetYeq);
                    if (!Double.IsNaN(offset_y)) Globals.offset_y = offset_y;
                }
                if (Globals.zoomXeq != "$")
                {
                    zoomX = Shunting.Evaluate(Globals.zoomXeq);
                    if (!Double.IsNaN(zoomX)) Globals.zoomX = zoomX;
                }
                if (Globals.zoomYeq != "$")
                {
                    zoomY = Shunting.Evaluate(Globals.zoomYeq);
                    if (!Double.IsNaN(zoomY)) Globals.zoomY = zoomY;
                }

            }
            catch
            {
            }
            int bound = 1000000000; //axes won't go any higher than this or else we start going into program-crashing territory
            Globals.zoomX = Math.Min(Math.Max(Globals.zoomX, 100.0 / bound), 2 * bound); // limit x & y zooms to between 1/bound and bound
            Globals.zoomY = Math.Min(Math.Max(Globals.zoomY, 100.0 / bound), 2 * bound);

            Globals.offset_x = Math.Min(Math.Max(Globals.offset_x, -bound + Globals.zoomX / 2), bound - Globals.zoomX / 2); //math says that this limits the values to between the bounds
            Globals.offset_y = Math.Min(Math.Max(Globals.offset_y, -bound + Globals.zoomY / 2), bound - Globals.zoomY / 2);

            g = e.Graphics;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            double lowest_x = -Globals.zoomX / 2 + Globals.offset_x;
            double highest_x = Globals.zoomX / 2 + Globals.offset_x;
            double lowest_y = -Globals.zoomY / 2 + Globals.offset_y;
            double highest_y = Globals.zoomY / 2 + Globals.offset_y;


            draw_lines();
            place_numbers();

            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.None;
            for (int i = 0; i < Globals.currentF; i++) //draw functions
            {
                Globals.functionsArr[i].Draw(g, Globals.quality);
            }
            for (int i = 0; i < Globals.currentL; i++) //draw lines
            {
                Globals.lineArr[i].Draw(g);
            }

            ignoreTextChange = true;
            textBox1.Text = lowest_x.ToString();
            textBox2.Text = highest_x.ToString();
            textBox3.Text = lowest_y.ToString();
            textBox4.Text = highest_y.ToString();
            ignoreTextChange = false;

            e.Dispose();
        }

        private void grid2_Resize(object sender, EventArgs e)
        {
            Globals.quality = Globals.defaultQuality;
            grid2.Refresh();
        }

        private DateTime lastScroll = DateTime.Now;
        private bool needScrollRefresh = false;
        private void grid2_MouseWheel(object sender, MouseEventArgs e)
        {
            Globals.quality = Globals.defaultQuality;
            double zoomAmount = 1.2;
            int zoomTimes = 5; //split the zoomAmount into this amount of little steps for a "smoother" feeling
            double zoomStep = Math.Pow(zoomAmount, 1.0 / zoomTimes);
            double posX, posY;
            (posX, posY) = Globals.convert_point_inverse(e.X, e.Y);
            if (Globals.HighlightedActualFunction != null) Globals.RemoveSpecialPoints();
            if (e.Delta < 0)
            {
                for (int i = 0; i < zoomTimes; i++)
                {
                    Globals.zoomX *= zoomStep; //increase zoom by xzoomAmount (which actually zooms out)
                    Globals.offset_x = -zoomStep * (posX - Globals.offset_x) + posX; //math says that this will center the zoom around the cursor
                    Globals.zoomY *= zoomStep;
                    Globals.offset_y = -zoomStep * (posY - Globals.offset_y) + posY;
                    grid2.Refresh();
                }
            }
            else
            {
                for (int i = 0; i < zoomTimes; i++)
                {
                    Globals.zoomX /= zoomStep;
                    Globals.offset_x = (-1 / zoomStep) * (posX - Globals.offset_x) + posX;
                    Globals.zoomY /= zoomStep;
                    Globals.offset_y = (-1 / zoomStep) * (posY - Globals.offset_y) + posY;
                    grid2.Refresh();
                }

            }
            lastScroll = DateTime.Now;
            needScrollRefresh = true;
        }

        private bool mouseDown = false;
        private int mouseDownX = -1;
        private int mouseDownY = -1;
        private void grid2_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = true;
            mouseDownX = e.X;
            mouseDownY = e.Y;
            if (Globals.LineFromPoint != null)
            {
                Globals.LineFromPoint = null;
                grid2.Refresh();
            }

        }
        bool disappear = false;
        private void grid2_MouseUp(object sender, MouseEventArgs e)
        {
            flowLayoutPanel1.Visible = true;
            flowLayoutPanel2.Visible = true;
            tableLayoutPanel1.Visible = true;
            disappear = false;

            mouseDown = false;
            if (prevX == -1 && prevY == -1 || (Math.Pow(mouseDownX - e.X, 2) + Math.Pow(mouseDownY - e.Y, 2) < 50)) //clicked without moving or with little (probably accidental) movement
            {
                double x, y;
                bool found = false;
                Function toHighlight = null;
                (x, y) = Globals.convert_point_inverse(e.X, e.Y);
                double epsilon = Globals.zoomY / 50;
                for (int i = 0; i < Globals.currentF; i++)
                {
                    try //check for invalid equations
                    {
                        Globals.functionsArr[i].f(1);
                        Globals.functionsArr[i].f(0);
                        Globals.functionsArr[i].f(-1);
                        Globals.functionsArr[i].f(1.31);
                    }
                    catch (NotSupportedException)
                    {
                        continue;
                    }
                    catch (InvalidOperationException)
                    {
                        continue;
                    }
                    catch { }

                    try
                    {
                        if (Math.Abs(Globals.functionsArr[i].f(x) - y) < epsilon && !found) //distance between the mouse and this function is small enough, highlight it
                        {
                            found = true;
                            toHighlight = Globals.functionsArr[i];
                            //Globals.HighlightedActualFunction = toHighlight;
                        }
                        else
                        {
                            Globals.HighlightedActualFunction = null;
                            Globals.functionsArr[i].SetHighlighted(false);
                            grid2.Refresh();
                        }
                    }
                    catch
                    {
                        Globals.HighlightedActualFunction = null;
                        Globals.functionsArr[i].SetHighlighted(false);
                        grid2.Refresh();
                    }
                }
                if (found)
                {
                    toHighlight.SetHighlighted(true);
                    grid2.Refresh();
                }


            }
            if (Globals.HighlightedActualFunction != null) Globals.HighlightedActualFunction.AddSpecialPoints(); //add special points to highlighted function
            prevX = -1;
            prevY = -1;
            mouseDownX = -1;
            mouseDownY = -1;
            grid2.Refresh();
        }

        private int prevX = -1;
        private int prevY = -1;
        private void grid2_MouseMove(object sender, MouseEventArgs e)
        {
            if (Globals.LineFromPoint != null) //draw a line from the point to the cursor
            {
                Graphics a = this.grid2.CreateGraphics();
                Point p1 = Globals.LineFromPoint.GetCoords();
                grid2.Refresh();
                a.DrawLine(pen, p1.X, p1.Y, e.X, e.Y);
                return;
            }
            if (mouseDown)
            {
                flowLayoutPanel1.Visible = false;
                flowLayoutPanel2.Visible = false;
                tableLayoutPanel1.Visible = false;
                disappear = true;
                Globals.quality = Globals.defaultQuality;
                if (Globals.HighlightedActualFunction != null) Globals.RemoveSpecialPoints(); //remove special point while dragging to improve performance
                double step_x = (grid2.Width / (Globals.zoomX));
                double step_y = (grid2.Height / (Globals.zoomY));
                if (prevX != -1 && prevY != -1)
                {
                    //change offset according to dragging
                    Globals.offset_x += (prevX - e.X) / step_x;
                    Globals.offset_y += (e.Y - prevY) / step_y;
                }
                prevX = e.X;
                prevY = e.Y;
                grid2.Refresh();
            }
        }

        public double Round(double val, int power)
        {
            if (power >= 0)
            {
                return (int)val / (int)Math.Pow(10, power) * (int)Math.Pow(10, power);
            }
            else
            {
                power = Math.Abs(power);
                return (int)(val * Math.Pow(10, power)) / Math.Pow(10, power);
            }
        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, this.flowLayoutPanel1.ClientRectangle, Color.DarkBlue, ButtonBorderStyle.Dashed);
        }

        private void flowLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {
            //ControlPaint.DrawBorder(e.Graphics, this.flowLayoutPanel1.ClientRectangle, Color.DarkBlue, ButtonBorderStyle.Dashed);
        }

        private void RemoveF1_Click(object sender, EventArgs e) //remove function/point/line/whatever associated with button
        {
            int serial = int.Parse((sender as Button).Name.Substring(7));
            Globals.Remove(serial);
            Globals.RemoveError(serial);
            TextBox t = this.Controls.Find("function" + serial, true).FirstOrDefault() as TextBox;
            flowLayoutPanel1.Controls.Remove(t);
            flowLayoutPanel2.Controls.Remove((sender as Button));
            grid2.Refresh();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click_1(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void UpdateSize()
        {
            try
            {
                double xmin = Shunting.Evaluate(textBox1.Text);
                double xmax = Shunting.Evaluate(textBox2.Text);
                double ymin = Shunting.Evaluate(textBox3.Text);
                double ymax = Shunting.Evaluate(textBox4.Text);
                if (xmin == xmax || ymin == ymax) return;
                Globals.offset_x = (xmin + xmax) / 2;
                Globals.zoomX = Math.Abs(xmax - xmin);
                Globals.offset_y = (ymin + ymax) / 2;
                Globals.zoomY = Math.Abs(ymax - ymin);
                grid2.Refresh();
            }
            catch { } //bad input, better just do nothing


        }
        private bool ignoreTextChange = false;
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            //grid2.Refresh();
            if (!ignoreTextChange)
                UpdateSize();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            //grid2.Refresh();
            if (!ignoreTextChange)
                UpdateSize();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            //grid2.Refresh();
            if (!ignoreTextChange)
                UpdateSize();
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            //grid2.Refresh();
            if (!ignoreTextChange)
                UpdateSize();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Globals.zoomX = 20;
            Globals.zoomY = 20;
            Globals.offset_x = 0;
            Globals.offset_y = 0;
            grid2.Refresh();
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }
    }
}
