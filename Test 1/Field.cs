using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Test_1
{
    interface IMathObject
    {
        void Draw(Graphics g);
    }
    class Field : TextBox
    {
        //public IMathObject GetContentsAsMathObject()
        //{

        //}
    }
}
