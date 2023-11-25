using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test_1
{
    class Derivative
    {
        //public static Queue<string> Der(Queue<string> exp1)
        //{
        //    Queue<string> result = new Queue<string>();
        //    Queue<string> exp = new Queue<string>(exp1.Reverse());
        //    double num;
        //    string cur = exp.Dequeue();
        //    if (double.TryParse(cur, out num))
        //    {
        //        result.Enqueue("0");
        //        return result;
        //    }
        //    switch (cur)
        //    {
        //        case "+":
        //            result.Enqueue("+");
        //            result = MergeQueues(result, Der(exp));
        //            exp.Dequeue();
        //            result = MergeQueues(result, Der(exp));
        //            return result;
        //        case "-":
        //            result.Enqueue("-");
        //            result = MergeQueues(result, Der(exp));
        //            exp.Dequeue();
        //            result = MergeQueues(result, Der(exp));
        //            return result;
        //        case "*":
        //            Queue<string> first = 
        //            result.Enqueue("+");
        //            result.Enqueue("*");
        //            result = MergeQueues(result, Der(exp));
        //            exp.Dequeue();
        //            result = MergeQueues(result, Der(exp));
        //            return result;
        //        case "/":
        //            a = stack.Pop();
        //            b = stack.Pop();
        //            stack.Push(b / a);
        //            break;
        //        case "^":
        //            a = stack.Pop();
        //            b = stack.Pop();
        //            stack.Push(Math.Pow(b, a));
        //            break;
        //        case "neg":
        //            stack.Push(-stack.Pop());
        //            break;
        //        case "sqrt":
        //            stack.Push(Math.Sqrt(stack.Pop()));
        //            break;
        //        case "abs":
        //            stack.Push(Math.Abs(stack.Pop()));
        //            break;
        //        case "e":
        //            stack.Push(Math.E);
        //            break;
        //        case "pi":
        //            stack.Push(Math.PI);
        //            break;
        //        case "sin":
        //            stack.Push(Math.Sin(stack.Pop()));
        //            break;
        //        case "arcsin":
        //            stack.Push(Math.Asin(stack.Pop()));
        //            break;
        //        case "sinh":
        //            stack.Push(Math.Sinh(stack.Pop()));
        //            break;
        //        case "cos":
        //            stack.Push(Math.Cos(stack.Pop()));
        //            break;
        //        case "arccos":
        //            stack.Push(Math.Acos(stack.Pop()));
        //            break;
        //        case "cosh":
        //            stack.Push(Math.Cosh(stack.Pop()));
        //            break;
        //        case "tan":
        //            a = stack.Pop();
        //            stack.Push(Math.Tan(a));
        //            break;
        //        case "arctan":
        //            stack.Push(Math.Atan(stack.Pop()));
        //            break;
        //        case "tanh":
        //            stack.Push(Math.Tanh(stack.Pop()));
        //            break;
        //        case "log":
        //            stack.Push(Math.Log10(stack.Pop()));
        //            break;
        //        case "ln":
        //            stack.Push(Math.Log(stack.Pop()));
        //            break;
        //        //case "!":
        //        //    a = stack.Pop();
        //        //    stack.Push(SpecialFunctions.Gamma(a + 1));
        //        //    break;
        //        case "int":
        //            a = stack.Pop();
        //            stack.Push((double)(int)a);
        //            break;
        //        case "floor":
        //            a = stack.Pop();
        //            stack.Push((Math.Floor(a)));
        //            break;
        //        case "ceil":
        //            a = stack.Pop();
        //            stack.Push((Math.Ceiling(a)));
        //            break;
        //    }

        //}

        public static Queue<string> MergeQueues(Queue<string> q1, Queue<string> q2)
        {
            while(q2.Count>0)
            {
                q1.Enqueue(q2.Dequeue());
            }
            return q1;
        }
    }
}
