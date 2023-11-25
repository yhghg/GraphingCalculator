using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Test_1
{

    class Variable
    {
        public string name; //name of the variable
        public int serial; //serial number of the variable

        public Variable(string name, int serial)
        {
            this.name = name;
            this.serial = serial;
        }

        public double GetSerial()
        {
            return this.serial;
        }

        public virtual double GetValue()
        {
            throw new NotSupportedException();
        }
    }
    class EqVariable : Variable
    {
        public string eq;

        public EqVariable(string name, string eq,int serial) : base(name,serial)
        {
            this.eq = eq;
        }

        public override double GetValue()
        {
            return Shunting.Evaluate(this.eq);
        }
    }

    class SliderVariable : Variable
    {
        public TrackBar slider; //slider associated with the variable
        double min, max; //minimum and maximum values of the variable

        public SliderVariable(string name, TrackBar slider,double min, double max, int serial) : base(name,serial)
        {
            this.slider = slider;
            this.min = min;
            this.max = max;
        }

        public override double GetValue()
        {
            return min+(this.max-this.min)* this.slider.Value / 1000; //the slider can get values between 0-1000, so this takes the appropriate value but between the bounds of the variable
        }

        public void RemoveSlider() //once this variable is removed, remove its slider with it
        {
            slider.Parent.FindForm().Controls.Find("RemoveF" + serial, true).FirstOrDefault().Height = 20;
            slider.Parent.Controls.Remove(slider);
        }

    }
}
