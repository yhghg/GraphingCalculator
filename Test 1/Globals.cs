using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace Test_1
{
    class Globals
    {
        public static Graphics g = null; //Graphics object
        public static Pen pen = new Pen(Color.Black); //drawing pen
        public static Pen penRed = new Pen(Color.Red, 3); //red drawing pen
        public static SolidBrush drawBrush = new SolidBrush(Color.Black); //drawing brush
        public static Font ariel = new Font("ariel", 10); //ariel font
        
        public static int functions = 1; //how many functions have been created

        public static Color[] colors = { Color.Red, Color.Blue, Color.Green, Color.Purple, Color.DarkOrange }; //set of colors for drawing

        public static double zoomX = 20; //amount of zoom in x direction
        public static double zoomY = 20; //amount of zoom in y direction
        public static double offset_x = 0; //amount of offset in x direction
        public static double offset_y = 0; //amount of offset in y direction

        public static string zoomXeq = "$"; //equation that controls zoom in x direction
        public static int zoomXID = -1; //id (serial) of the textbox that contains the equation
        public static string zoomYeq = "$"; //equation that controls zoom in y direction
        public static int zoomYID = -1; //id (serial) of the textbox that contains the equation
        public static string offsetXeq = "$"; //equation that controls offset in x direction
        public static int offsetXID = -1; //id (serial) of the textbox that contains the equation
        public static string offsetYeq = "$"; //equation that controls offset in y direction
        public static int offsetYID = -1; //id (serial) of the textbox that contains the equation


        public static UserPointControl[] UserPointControlsArr = new UserPointControl[10]; //array to save user point controls
        public static int currentP = 0; //current amount of points in the array

        public static SpecialPointControl[] SpecialPointControlsArr = new SpecialPointControl[50]; //array to save user special point controls
        public static int currentIP = 0; //current amount of points in the array
        public static int HighlightedFunction = -1; //serial of highlighted function
        public static Function HighlightedActualFunction = null; //function object of the highlighted function

        public static Function[] functionsArr = new Function[2]; //array to save user created functions
        public static int currentF = 0; //current amount of functions in the array

        public static Dictionary<string, Function> namedFunctions = new Dictionary<string, Function>(); //dictionary that connects between names of named functions and the funcions themselves

        public static Dictionary<string, Variable> variables = new Dictionary<string, Variable>(); //dictionary that connects between names of variables and the variables themselves

        public static Line[] lineArr = new Line[10]; //array to save lines
        public static int currentL = 0; //current amount of lines in the array

        public static ToolTip toolTip1; //shows tooltip when hovering with the mouse on certain things

        public static GridPointControl LineFromPoint = null; //if a line is currently being drawn from a point, this saves the starting point

        public static Display grid2; //main panel the everything is being drawn on
        public static FlowLayoutPanel flowLayoutPanel1; //the two side panels
        public static FlowLayoutPanel flowLayoutPanel2;
        public static int time = 0;
        public static double quality = 0.2;
        public static double defaultQuality = 0.2;



        public static Point convert_point(double x, double y)
        {
            double actual_x = Math.Max(Math.Min(grid2.Width * (((x - offset_x)) / (zoomX) + 0.5), grid2.Width), 0); //equation for converting point from grid coordinates to screen coordinates
            double actual_y = Math.Max(Math.Min(grid2.Height * (((offset_y - y)) / (zoomY) + 0.5), grid2.Height), 0);
            return new Point((int)Math.Round(actual_x), (int)Math.Round(actual_y));
        }
        public static Point convert_point_unbounded(double x, double y)
        {
            double actual_x = grid2.Width * (((x - offset_x)) / (zoomX) + 0.5); //equation for converting point from grid coordinates to screen coordinates
            double actual_y = grid2.Height * (((offset_y - y)) / (zoomY) + 0.5);
            return new Point((int)Math.Round(actual_x), (int)Math.Round(actual_y));
        }
        public static (double, double) convert_point_inverse(int actual_x, int actual_y)
        {
            double graph_x = (actual_x + offset_x / (zoomX) * grid2.Width - grid2.Width / 2.0) / grid2.Width * (zoomX); //equation for converting point from screen coordinates to grid coordinates
            double graph_y = (actual_y - grid2.Height / 2.0 - offset_y / (zoomY) * grid2.Height) / grid2.Height * (zoomY);
            graph_y *= -1;
            return (graph_x, graph_y);
        }
        private static void DoubleLength(ref Function[] f)
        {
            Function[] newarr = new Function[f.Length * 2]; //create new array with twice the length
            for (int i = 0; i < f.Length; i++) //move everything over to the new array
            {
                newarr[i] = f[i];
            }
            f = newarr;
        }
        private static void DoubleLength(ref SpecialPointControl[] p)
        {
            SpecialPointControl[] newarr = new SpecialPointControl[p.Length * 2];
            for (int i = 0; i < p.Length; i++)
            {
                newarr[i] = p[i];
            }
            p = newarr;
        }
        private static void DoubleLength(ref UserPointControl[] p)
        {
            UserPointControl[] newarr = new UserPointControl[p.Length * 2];
            for (int i = 0; i < p.Length; i++)
            {
                newarr[i] = p[i];
            }
            p = newarr;
        }
        private static void DoubleLength(ref Line[] l)
        {
            Line[] newarr = new Line[l.Length * 2];
            for (int i = 0; i < l.Length; i++)
            {
                newarr[i] = l[i];
            }
            l = newarr;
        }
        public static void AddFunction(Function f)
        {
            functionsArr[currentF] = f; //add function to functions array
            currentF++;
            Globals.RemoveSpecialPoints(); //adding this function might have changed the intersection point of the highlighted function so remove those
            for (int i = 0; i < currentF; i++)
            {
                if (functionsArr[i].GetSerial() == Globals.HighlightedFunction)
                {
                    functionsArr[i].AddSpecialPoints(); //add back the special points that we removed, this time with the new good ones
                }
            }
            if (currentF == functionsArr.Length) //if array is too full, double its length
            {
                DoubleLength(ref functionsArr);
            }
        }
        public static void RemoveFunction(int serial)
        {
            bool found = false;
            for (int i = 0; i < currentF; i++)
            {
                if (!found)
                {
                    if (functionsArr[i].GetSerial() == serial) //found the function that they want to remove
                    {
                        found = true;
                        Globals.RemoveSpecialPoints(); //removing this function might have changed the special points
                        if (serial == Globals.HighlightedFunction) //if we are removing the highlighted function, delete the things that save it as highlighted
                        {
                            Globals.HighlightedFunction = -1;
                            Globals.HighlightedActualFunction = null;
                        }
                    }
                }
                else
                {
                    functionsArr[i - 1] = functionsArr[i]; //after we find the function and delete it, move all the other functions 1 place backwards
                }
            }
            if (found)
            {
                currentF--;
                for (int j = 0; j < currentF; j++)
                {
                    if (functionsArr[j].GetSerial() == Globals.HighlightedFunction)
                    {
                        functionsArr[j].AddSpecialPoints(); //add special points back after we removed them
                    }
                }
                foreach (KeyValuePair<string, Function> kvp in Globals.namedFunctions) //if the function had a name, remove it from the named functions dictionary
                {
                    if (kvp.Value.GetSerial() == serial)
                    {
                        Globals.namedFunctions.Remove(kvp.Key);
                        break;
                    }
                }
            }
        }
        public static void AddPoint(UserPointControl p)
        {
            UserPointControlsArr[currentP] = p;
            currentP++;
            if (currentP == UserPointControlsArr.Length)
            {
                DoubleLength(ref UserPointControlsArr);
            }
            grid2.Controls.Add(p);
        }
        public static void AddSpecialPoint(SpecialPointControl p)
        {
            SpecialPointControlsArr[currentIP] = p;
            if (currentIP >= 50) return;
            currentIP++;
            if (currentIP == SpecialPointControlsArr.Length)
            {
                DoubleLength(ref SpecialPointControlsArr);
            }
            grid2.Controls.Add(p);
        }
        //public static void AddMultipleSpecialPoints(SpecialPointControl[] ps)
        //{
        //    currentIP = 0;
        //    for (int i = 0; i < ps.Length; i++)
        //    {
        //        if (ps[i] == null) break;
        //        SpecialPointControlsArr[i] = ps[i];
        //        currentIP++;
        //    }
        //    grid2.Controls.AddRange(ps);
        //}
        public static void AddLine(Line l)
        {
            lineArr[currentL] = l;
            currentL++;
            //Globals.RemoveSpecialPoints();
            //for (int i = 0; i < currentF; i++)
            //{
            //    if (functionsArr[i].GetSerial() == Globals.HighlightedFunction)
            //    {
            //        functionsArr[i].AddSpecialPoints();
            //    }
            //}
            if (currentL == lineArr.Length)
            {
                DoubleLength(ref lineArr);
            }
        }

        public static void RemoveLine(int serial)
        {
            bool found = false;
            for (int i = 0; i < currentL; i++)
            {
                if (!found)
                {
                    if (lineArr[i].GetSerial() == serial)
                    {
                        found = true;
                    }
                }
                else
                {
                    lineArr[i - 1] = lineArr[i];
                }
            }
            if (found) currentL--;
        }
        public static void RemovePoint(int serial)
        {
            bool found = false;
            for (int i = 0; i < currentP; i++)
            {
                if (!found)
                {
                    if (UserPointControlsArr[i].GetSerial() == serial)
                    {
                        found = true;
                        grid2.Controls.Remove(UserPointControlsArr[i]);
                    }
                }
                else
                {
                    UserPointControlsArr[i - 1] = UserPointControlsArr[i];
                }
            }
            if (found) currentP--;
        }

        public static void Remove(int serial) //gets just an id and figures out what to remove and then calls the appropriate function on it
        {
            if (new int[] { offsetXID, offsetYID, zoomYID, zoomXID }.Contains(serial)) //if we are remving a textbox with a setting
            {
                if (offsetXID == serial)
                {
                    offsetXID = -1;
                    offsetXeq = "$";
                    return;
                }
                if (offsetYID == serial)
                {
                    offsetYID = -1;
                    offsetYeq = "$";
                    return;
                }
                if (zoomXID == serial)
                {
                    zoomXID = -1;
                    zoomXeq = "$";
                    return;
                }
                if (zoomYID == serial)
                {
                    zoomYID = -1;
                    zoomYeq = "$";
                    return;
                }
            }
            //check if we are removning a function, point, line, or whatever and then call the appropriate function on it
            for (int i = 0; i < currentF; i++)
            {
                if (functionsArr[i].GetSerial() == serial)
                {
                    RemoveFunction(serial);
                    return;
                }
            }
            for (int i = 0; i < currentP; i++)
            {
                if (UserPointControlsArr[i].GetSerial() == serial)
                {
                    RemovePoint(serial);
                    return;
                }
            }
            for (int i = 0; i < currentL; i++)
            {
                if (lineArr[i].GetSerial() == serial)
                {
                    RemoveLine(serial);
                    return;
                }
            }
            foreach (KeyValuePair<string, Variable> kvp in Globals.variables)
            {
                if (kvp.Value.GetSerial() == serial)
                {
                    if (kvp.Value is SliderVariable sv)
                    {
                        sv.RemoveSlider();
                    }
                    Globals.variables.Remove(kvp.Key);
                    return;
                }
            }
        }

        public static void RemoveSpecialPoints()
        {
            for (int i = 0; i < currentIP; i++)
            {
                grid2.Controls.Remove(SpecialPointControlsArr[i]);
            }
            currentIP = 0;
        }

        public static void AddNamedFunction(string name, Function f)
        {
            if (name.Contains('(') || name.Contains(')') || name.Contains('\'')) { throw new Exception("Illegal character in function name."); }
            if (name == "x") { throw new Exception("Cannot use 'x' as a function name."); }
            Globals.namedFunctions.Add(name, f);
        }
        public static void AddVariable(string name, string eq, int serial)
        {
            char[] bad = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            for (int i = 0; i < bad.Length; i++)
            {
                if (name.Contains(bad[i])) { throw new Exception("Variable name cannot contain numbers."); }
            }
            if (name.Contains('(') || name.Contains(')') || name.Contains('\'')) { throw new Exception("Illegal character in variable name."); }
            if (name=="x") { throw new Exception("Cannot use 'x' as a variable name."); }
            Globals.variables.Add(name, new EqVariable(name, eq, serial));
        }
        public static void AddVariable(string name, TrackBar slider, double min, double max, int serial)
        {
            Globals.variables.Add(name, new SliderVariable(name, slider, min, max, serial));
        }
        public static void RefreshPoints()
        {
            for (int i = 0; i < currentP; i++)
            {
                UserPointControlsArr[i].RefreshPoint();
            }
            for (int i = 0; i < currentIP; i++)
            {
                SpecialPointControlsArr[i].RefreshPoint();
            }
        }
        public static void ShowError(string summary, string text, int serial)
        {
            flowLayoutPanel1.SuspendLayout();
            flowLayoutPanel2.SuspendLayout();
            RemoveError(serial); //if there was already an error here, override it
            Label error = new Label(); //create label for the error
            error.Name = "Error" + serial;
            error.Font = new Font("ariel", 7);
            error.Size = new Size(105, 10);
            Globals.toolTip1.SetToolTip(error, text); //for when you hover over the error with the mouse
            error.Text = summary;
            error.ForeColor = Color.Red;
            TextBox tb = Globals.flowLayoutPanel1.Controls["function" + serial] as TextBox;
            int index = Globals.flowLayoutPanel1.Controls.GetChildIndex(tb);
            flowLayoutPanel1.Controls.Add(error); //add the error to the screen
            flowLayoutPanel1.Controls.SetChildIndex(error, index + 1);
            flowLayoutPanel2.Controls.Find("RemoveF" + serial, true).FirstOrDefault().Height = tb.Height + 10; //make the remove button slighly bigger to include the newly added error
            flowLayoutPanel1.ResumeLayout(true);
            flowLayoutPanel2.ResumeLayout(true);
        }
        public static void RemoveError(int serial)
        {
            Label l = flowLayoutPanel1.Controls.Find("Error" + serial, true).FirstOrDefault() as Label;
            if (l == null) return;
            flowLayoutPanel1.Controls.Remove(l); //remove error of the given textbox
            flowLayoutPanel2.Controls.Find("RemoveF" + serial, true).FirstOrDefault().Height = 20;
        }
    }
}
