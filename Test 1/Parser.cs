using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test_1
{
    class Parser //UNUSED CLASS
    {
        public static double Eval(string exp)
        {
            double result;
            if (double.TryParse(exp, out result)) return result;

            for (int i = 0; i < exp.Length; i++)
            {
                if (exp[i] == '+')
                {
                    return Eval(exp.Substring(0, i)) + Eval(exp.Substring(i + 1));
                }
                else if (exp[i] == '-')
                {
                    return Eval(exp.Substring(0, i)) - Eval(exp.Substring(i + 1));
                }
            }

            for (int i = 0; i < exp.Length; i++)
            {
                if (exp[i] == '*')
                {
                    return Eval(exp.Substring(0, i)) * Eval(exp.Substring(i + 1));
                }
                else if (exp[i] == '/')
                {
                    return Eval(exp.Substring(0, i)) / Eval(exp.Substring(i + 1));
                }
            }

            return -1;
        }

        public static double Eval2(string exp)
        {
            char[] arr = exp.ToCharArray();
            Array.Reverse(arr);
            return Eval(new string(arr));
        }

    }
}
