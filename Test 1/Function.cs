using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.Control;

namespace Test_1
{
    public class Function
    {
        private int serial; //serial number of the function
        private Queue<string> equation; //equation of the function (after being rearranged to a queue)
        private Pen pen; //pen to draw the function
        private Color color; //color of the function
        private bool isHighlighted; //is the function highlighted?
        public Function(string eq, Color color, int serial)
        {
            this.color = color;
            this.pen = new Pen(color, 2);
            this.serial = serial;
            this.isHighlighted = false;
            this.equation = Shunting.Eval(Shunting.Convert(eq));
        }
        public int GetSerial()
        {
            return this.serial;
        }

        public void SetHighlighted(bool b)
        {
            if (b) //highlight the function
            {
                Globals.HighlightedFunction = this.serial;
                Globals.HighlightedActualFunction = this;
                if (!this.isHighlighted) this.AddSpecialPoints();
                this.pen.Color = Color.Black;
                this.isHighlighted = true;
            }
            else //unhighlight the function
            {
                Globals.HighlightedFunction = -1;
                if (this.isHighlighted)
                {
                    Globals.RemoveSpecialPoints();
                }
                this.pen.Color = this.color;
                this.isHighlighted = false;
            }
        }
        public bool GetHighlighted()
        {
            return this.isHighlighted;
        }
        public double f(double x) //get the value of the function at x
        {
            return Shunting.Calc_asym(equation, x,0).Item1;
        }
        public (double, List<bool>, List<bool>, bool) fFull(double x,int recursion)
        {
            return Shunting.Calc_asym(equation, x,recursion);
        }
        public void AddSpecialPoints() //add minima/maxima/intersections with other functions
        {
            SpecialPointControl[] toAdd = new SpecialPointControl[50];
            int index = 0;
            double offset_x = Globals.offset_x;
            double offset_y = Globals.offset_y;
            double cur_x = 0;
            double step = Globals.zoomX / 20000;
            double epsilon = Globals.zoomX / 200;
            for (int i = 0; i < Globals.currentF; i++) //add intersections
            {
                try //try some values to see if function is even valid
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
                cur_x = -Globals.zoomX / 2 + offset_x;
                while (cur_x <= Globals.zoomX / 2 + offset_x)
                {
                    try
                    {
                        if (Math.Abs(this.f(cur_x) - Globals.functionsArr[i].f(cur_x)) < epsilon)
                        {
                            if (Math.Abs(this.f(cur_x) - Globals.functionsArr[i].f(cur_x)) < Math.Abs(this.f(cur_x - step) - Globals.functionsArr[i].f(cur_x - step)) && Math.Abs(this.f(cur_x) - Globals.functionsArr[i].f(cur_x)) < Math.Abs(this.f(cur_x + step) - Globals.functionsArr[i].f(cur_x + step)))
                            {
                                Globals.AddSpecialPoint(new SpecialPointControl(cur_x.ToString("." + new string('#', 324)), this.f(cur_x).ToString("." + new string('#', 324))));
                            }
                        }
                    }
                    catch { }
                    cur_x += step;
                }
            }
            
            cur_x = -Globals.zoomX / 2 + offset_x;
            while (cur_x <= Globals.zoomX / 2 + offset_x) //add minimums/maximums
            {
                try
                {
                    if (Double.IsNaN(this.f(cur_x - step)) || Double.IsNaN(this.f(cur_x)) || Double.IsNaN(this.f(cur_x + step)))
                    {
                        cur_x += step;
                        continue;
                    }
                }
                catch
                {
                    cur_x += step;
                    continue;
                }

                double value = this.f(cur_x);
                if (value < this.f(cur_x - step) && value < this.f(cur_x + step) || value > this.f(cur_x - step) && value > this.f(cur_x + step)) //if point is a minimum/maximum
                {

                    Globals.AddSpecialPoint(new SpecialPointControl(cur_x.ToString("." + new string('#', 324)), value.ToString("." + new string('#', 324))));
                    //if (index < 50)
                    //{
                    //    toAdd[index] = new SpecialPointControl(cur_x.ToString("." + new string('#', 324)), value.ToString("." + new string('#', 324)));
                    //    index++;
                    //}
                    //else
                    //{
                    //    break;
                    //}
                }
                cur_x += step;
            }
            //Globals.AddMultipleSpecialPoints(toAdd);

        }
        public void Draw(Graphics g,double quality)
        {
            if (equation.Count == 0) return;
            try //try a couple of values to see if the equation is even a valid equation and if not we return
            {
                Shunting.Calc_asym(equation, -15,0);
                Shunting.Calc_asym(equation, 1, 0);
                Shunting.Calc_asym(equation, -1, 0);
                Shunting.Calc_asym(equation, 1.31, 0);
            }
            catch (NotSupportedException e)
            {
                if(quality>0.9) Globals.ShowError("¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯", e.Message,this.serial);
                return;
            }
            catch (InvalidOperationException e)
            {
                if (quality > 0.9) Globals.ShowError("¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯", e.Message,this.serial);
                return;
            }
            catch { } //normal math error probably (division by 0 or similar)
            Globals.RemoveError(this.serial);
            double offset_x = Globals.offset_x;
            double offset_y = Globals.offset_y;
            double cur_x = -Globals.zoomX / 2 + offset_x;
            double step = Globals.zoomX / (quality*Globals.grid2.Width);
            Point p1, p2;
            double v1, v2; //value at current x
            double v3, v0; //for checking asymptotes
            List<bool> asyms1, asyms2, jumps1, jumps2;
            bool logs1, logs2;
            while (cur_x <= Globals.zoomX / 2 + offset_x)
            {
                try
                {
                    (v1, asyms1, jumps1,logs1) = Shunting.Calc_asym(equation, cur_x, 0);
                    (v2, asyms2, jumps2,logs2) = Shunting.Calc_asym(equation, cur_x + step, 0);
                    if (logs1 || logs2) //dealing with log of negative number
                    {
                        if (!logs2) //if we are going from a weird point to a defined one
                        {
                            (v3, asyms1, jumps1,logs1) = Shunting.Calc_asym(equation, cur_x + 2 * step, 0); //calculate 1 step after next
                            if (v2 > v3) //going down to a defined point
                            {
                                p1 = Globals.convert_point(cur_x + step, v2);
                                p2 = Globals.convert_point(cur_x + step, Globals.zoomY / 2 + offset_y);
                                g.DrawLine(this.pen, p1, p2);
                            }
                            else
                            {
                                p1 = Globals.convert_point(cur_x + step, v2);
                                p2 = Globals.convert_point(cur_x + step, -Globals.zoomY / 2 + offset_y);
                                g.DrawLine(this.pen, p1, p2);
                            }
                        }
                        else if (!(logs1)) //if we are going from a defined point to a weird one
                        {
                            (v0, asyms1, jumps1,logs1) = Shunting.Calc_asym(equation, cur_x - step, 0); //calculate 1 step before current
                            if (v0 > v1) //going down to a weird point
                            {
                                p1 = Globals.convert_point(cur_x + step, v1);
                                p2 = Globals.convert_point(cur_x + step, -Globals.zoomY / 2 + offset_y);
                                g.DrawLine(this.pen, p1, p2);
                            }
                            else
                            {
                                p1 = Globals.convert_point(cur_x + step, v1);
                                p2 = Globals.convert_point(cur_x + step, Globals.zoomY / 2 + offset_y);
                                g.DrawLine(this.pen, p1, p2);
                            }
                        }
                        cur_x += step;
                        continue;
                    }
                    if (!asyms1.SequenceEqual(asyms2)) //there's probably an asymptote (draw it)
                    {
                        (v3, asyms1, jumps1,logs1) = Shunting.Calc_asym(equation, cur_x + 2 * step, 0); //calculate 1 step after next
                        (v0, asyms2, jumps2,logs2) = Shunting.Calc_asym(equation, cur_x - step, 0); //calculate 1 step before current
                        if (v0 > v1 && v2 > v3 && v1 < v2) //sudden jump downwards
                        {
                            p1 = Globals.convert_point(cur_x, v1);
                            p2 = Globals.convert_point(cur_x, -Globals.zoomY / 2 + offset_y);
                            g.DrawLine(this.pen, p1, p2);
                            p1 = Globals.convert_point(cur_x + step, v2);
                            p2 = Globals.convert_point(cur_x + step, Globals.zoomY / 2 + offset_y);
                            g.DrawLine(this.pen, p1, p2);
                        }
                        else if (v0 < v1 && v2 < v3 && v1 > v2) //sudden jump upwards
                        {
                            p1 = Globals.convert_point(cur_x, v1);
                            p2 = Globals.convert_point(cur_x, Globals.zoomY / 2 + offset_y);
                            g.DrawLine(this.pen, p1, p2);
                            p1 = Globals.convert_point(cur_x + step, v2);
                            p2 = Globals.convert_point(cur_x + step, -Globals.zoomY / 2 + offset_y);
                            g.DrawLine(this.pen, p1, p2);
                        }
                        cur_x += step;
                        continue;
                    }
                    if (!jumps1.SequenceEqual(jumps2)) //there is probably an incontinuity (don't connect these points)
                    {
                        cur_x += step;
                        continue;
                    }
                    if(Double.IsNaN(v1) || Double.IsNaN(v2))
                    {
                        cur_x += step;
                        continue;
                    }
                    
                    p1 = Globals.convert_point(cur_x, v1);
                    p2 = Globals.convert_point(cur_x + step, v2);
                    if (!(p1.Y == 0 && p2.Y == 0 || p1.Y == Globals.grid2.Height && p2.Y == Globals.grid2.Height))
                    { g.DrawLine(this.pen, p1, p2); }
                }
                catch
                {
                    //cur_x += step;
                }
                cur_x += step;
            }
        }

        public void HighlightIntegral(Graphics g, double a, double b) //hightlight the region under the funcion from a to b
        {
            double curX = a;
            double height;
            double dx = Globals.zoomX / 1000;
            Rectangle r;
            Brush brush = new SolidBrush(Color.FromArgb(100, this.color.R, this.color.G, this.color.B)); //slightly transparent version of the functions color
            while (curX <= b)
            {
                height = this.f(curX);
                if (height == 0)
                {
                    curX += dx;
                    continue;
                }
                r = new Rectangle(Globals.convert_point(curX, Math.Max(0, height)), new Size((int)(Globals.grid2.Width * (dx / Globals.zoomX)) + 1, (int)(Globals.grid2.Height * (Math.Abs(height) / Globals.zoomY))));
                g.FillRectangle(brush, r);
                curX += dx;
            }
        }
        public double Integral(double a, double b) //calculate the integral/area under the funcion from a to b
        {
            double curX = a;
            double ans = 0;
            double height;
            double dx = Globals.zoomX / 1000;
            while (curX <= b)
            {
                height = this.f(curX);
                ans += height * dx;
                curX += dx;
            }
            return ans;
        }
    }
}
