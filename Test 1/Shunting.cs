using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics;

namespace Test_1
{
    class Shunting
    {

        public static Stack<string> Convert(string exp) //splits string into stack of numbers and operators (in original order)
        {
            if (exp.Contains("()")) throw new Exception("Empty parentheses.");
            Stack<string> stack = new Stack<string>();
            int i = 0;
            string num = ""; //for saving numbers
            while (i < exp.Length)
            {

                if (exp[i] == '+' || exp[i] == '-' || exp[i] == '*' || exp[i] == '/' || exp[i] == '^' || exp[i] == '!' || exp[i] == '%' || exp[i] == ',' || exp[i] == '(' || exp[i] == ')')
                {
                    if (num != "") { stack.Push(num); num = ""; } //this operation came right after a number, put the number into the stack before doing anything else
                    if (exp[i] == '-' && (i == 0 || exp[i - 1] == '+' || exp[i - 1] == '-' || exp[i - 1] == '*' || exp[i - 1] == '/' || exp[i - 1] == '^' || exp[i - 1] == '%' || exp[i - 1] == ',' || exp[i - 1] == '('))
                    {
                        stack.Push("neg"); //this is a negative sign and not a subtraction sign
                    }
                    else
                    {
                        stack.Push(exp[i].ToString()); //push whatever operation this is to the stack
                    }
                    i++;
                }
                else //this is a digit of a number
                {
                    num += exp[i];
                    i++;
                }
            }
            Stack<string> stack2 = new Stack<string>();
            if (num != "") stack2.Push(num);
            int l = stack.Count;
            for (i = 0; i < l; i++) //reverse stack
            {
                stack2.Push(stack.Pop());
            }
            return stack2;
        }
        public static Queue<string> Eval(Stack<string> exp) //takes the stack from Convert() and reorders it to a queue according to the Shunting-yard algorithm (slightly modified to include functions)
        {
            Stack<string> stack = new Stack<string>(); //operator stack
            Queue<string> queue = new Queue<string>();
            stack.Push("$");
            string cur;
            double dull;
            while (exp.Count > 0)
            {
                cur = exp.Pop();
                if (double.TryParse(cur, out dull) || cur == "pi" || cur == "e" || cur == "x") //if it's a number (or something that represents a number), put it in the queue right away
                {
                    queue.Enqueue(cur);
                }
                else if (cur == "(" || cur == ")" || cur == ",")
                {
                    if (cur == "(") { stack.Push(cur); }
                    else //')' or ','
                    {
                        try
                        {
                            while (stack.Peek() != "(") //pop the stack until the last opening bracket
                            {
                                queue.Enqueue(stack.Pop());
                            }
                            if (cur == ")") stack.Pop(); //if it's a closing bracket we also want to remove the opening bracket but if it's a comma we don't
                        }
                        catch (InvalidOperationException) { throw new InvalidOperationException("Couldn't parse the equation correctly, have you misplaced a comma or a parentheses?"); }
                    }
                }
                else
                {
                    if (cur == "^" || cur == "neg") //negative signs and exponantiation operators are weird
                    {
                        while (precedence(cur) < precedence(stack.Peek()))
                        {
                            queue.Enqueue(stack.Pop());
                        }
                    }
                    else
                    {
                        while (precedence(cur) <= precedence(stack.Peek()))
                        {
                            queue.Enqueue(stack.Pop());
                        }
                    }
                    stack.Push(cur);
                }
            }
            while (stack.Count > 1)
            {
                queue.Enqueue(stack.Pop());
            }
            return queue;
        }

        private static int precedence(string op) //defines order of operations (higher number means higher priority)
        {
            char ch;
            Char.TryParse(op, out ch);
            if (ch == '$' || ch == '(') return -1;
            if (ch == '+' || ch == '-') return 1;
            if (ch == '*' || ch == '/' || ch == '%') return 2;
            if (ch == '^' || op == "neg") return 3;
            if (ch == '!') return 4;
            else
            {
                return 5; //any other thing that this might get is a function so of course it needs to be evaluated immediately
            }
        }

        public static double Calc(Queue<string> exp)
        {
            Stack<double> stack = new Stack<double>();
            string cur;
            double num;
            double a, b;
            while (exp.Count > 0)
            {
                cur = exp.Dequeue();
                try
                {
                    if (double.TryParse(cur, out num)) stack.Push(num);
                    else
                    {
                        switch (cur) //perform the right operation
                        {
                            case "+":
                                stack.Push(stack.Pop() + stack.Pop());
                                break;
                            case "-":
                                a = stack.Pop();
                                b = stack.Pop();
                                stack.Push(b - a);
                                break;
                            case "*":
                                stack.Push(stack.Pop() * stack.Pop());
                                break;
                            case "/":
                                a = stack.Pop();
                                b = stack.Pop();
                                stack.Push(b / a);
                                break;
                            case "%":
                                a = stack.Pop();
                                b = stack.Pop();
                                stack.Push(b % a);
                                break;
                            case "^":
                                a = stack.Pop();
                                b = stack.Pop();
                                stack.Push(Math.Pow(b, a));
                                break;
                            case "neg":
                                stack.Push(-stack.Pop());
                                break;
                            case "sqrt":
                                stack.Push(Math.Sqrt(stack.Pop()));
                                break;
                            case "abs":
                                stack.Push(Math.Abs(stack.Pop()));
                                break;
                            case "e":
                                stack.Push(Math.E);
                                break;
                            case "pi":
                                stack.Push(Math.PI);
                                break;
                            case "sin":
                                stack.Push(Math.Sin(stack.Pop()));
                                break;
                            case "arcsin":
                                stack.Push(Math.Asin(stack.Pop()));
                                break;
                            case "sinh":
                                stack.Push(Math.Sinh(stack.Pop()));
                                break;
                            case "cos":
                                stack.Push(Math.Cos(stack.Pop()));
                                break;
                            case "arccos":
                                stack.Push(Math.Acos(stack.Pop()));
                                break;
                            case "cosh":
                                stack.Push(Math.Cosh(stack.Pop()));
                                break;
                            case "tan":
                                a = stack.Pop();
                                stack.Push(Math.Tan(a));
                                break;
                            case "arctan":
                                stack.Push(Math.Atan(stack.Pop()));
                                break;
                            case "tanh":
                                stack.Push(Math.Tanh(stack.Pop()));
                                break;
                            case "log":
                                stack.Push(Math.Log10(stack.Pop()));
                                break;
                            case "ln":
                                stack.Push(Math.Log(stack.Pop()));
                                break;
                            case "!":
                                a = stack.Pop();
                                stack.Push(SpecialFunctions.Gamma(a + 1));
                                break;
                            case "int":
                                a = stack.Pop();
                                stack.Push((double)(int)a);
                                break;
                            case "floor":
                                a = stack.Pop();
                                stack.Push((Math.Floor(a)));
                                break;
                            case "ceil":
                                a = stack.Pop();
                                stack.Push((Math.Ceiling(a)));
                                break;
                            case "min":
                                a = stack.Pop();
                                b = stack.Pop();
                                stack.Push((Math.Min(a, b)));
                                break;
                            case "max":
                                a = stack.Pop();
                                b = stack.Pop();
                                stack.Push((Math.Max(a, b)));
                                break;
                            case "frac":
                                a = stack.Pop();
                                stack.Push(a - (double)(int)a);
                                break;
                            default:
                                Function f;
                                if (Globals.namedFunctions.TryGetValue(cur.TrimEnd('\''), out f))
                                {
                                    if (cur.Length == 1)
                                    {
                                        stack.Push(f.f(stack.Pop()));
                                        break;
                                    }
                                    int n = cur.Length - cur.TrimEnd('\'').Length; //nth derivative, calculate n based on the amount of 's
                                    double ans = 0;
                                    double h = 0.001;
                                    double x0 = stack.Pop();
                                    for (int i = 0; i <= n; i++) //formula for nth derivative
                                    {
                                        ans += Math.Pow(-1, i) * Choose(n, i) * f.f(x0 + h * (n / 2.0 - i));
                                    }
                                    stack.Push(ans / Math.Pow(h, n));
                                    break;
                                }
                                Variable var;
                                if (Globals.variables.TryGetValue(cur, out var))
                                {
                                    double val;
                                    try
                                    {
                                        val = var.GetValue();
                                    }
                                    catch
                                    {
                                        throw new NotSupportedException("Variable \'" + cur + "\' is not defined properly.");
                                    }
                                    stack.Push(val);
                                    break;
                                }
                                if (cur == "(") throw new NotSupportedException("Missing closing parentheses.");
                                throw new NotSupportedException("Couldn't recognize variable/function \'" + cur + "\'.");
                        }
                    }
                }
                catch (InvalidOperationException) { throw new InvalidOperationException("Missing arguments for a function/operation (\'" + cur + "\'?)."); }
            }
            if (stack.Count != 1) throw new NotSupportedException("Couldn't compute expression.");
            return stack.Pop();
        }

        //public static double Calc(Queue<string> exp, double x)
        //{
        //    Stack<double> stack = new Stack<double>();
        //    string cur;
        //    double num;
        //    double a, b;
        //    while (exp.Count > 0)
        //    {
        //        cur = exp.Dequeue();
        //        if (double.TryParse(cur, out num)) stack.Push(num);
        //        else
        //        {
        //            switch (cur)
        //            {
        //                case "+":
        //                    stack.Push(stack.Pop() + stack.Pop());
        //                    break;
        //                case "-":
        //                    a = stack.Pop();
        //                    b = stack.Pop();
        //                    stack.Push(b - a);
        //                    break;
        //                case "*":
        //                    stack.Push(stack.Pop() * stack.Pop());
        //                    break;
        //                case "/":
        //                    a = stack.Pop();
        //                    b = stack.Pop();
        //                    stack.Push(b / a);
        //                    break;
        //                case "^":
        //                    a = stack.Pop();
        //                    b = stack.Pop();
        //                    stack.Push(Math.Pow(b, a));
        //                    break;
        //                case "neg":
        //                    stack.Push(-stack.Pop());
        //                    break;
        //                case "sqrt":
        //                    stack.Push(Math.Sqrt(stack.Pop()));
        //                    break;
        //                case "abs":
        //                    stack.Push(Math.Abs(stack.Pop()));
        //                    break;
        //                case "pi":
        //                    stack.Push(Math.PI);
        //                    break;
        //                case "sin":
        //                    stack.Push(Math.Sin(stack.Pop()));
        //                    break;
        //                case "cos":
        //                    stack.Push(Math.Cos(stack.Pop()));
        //                    break;
        //                case "tan":
        //                    stack.Push(Math.Tan(stack.Pop()));
        //                    break;
        //                case "x":
        //                    stack.Push(x);
        //                    break;
        //            }
        //        }
        //    }
        //    return stack.Pop();
        //}

        public static (double, List<bool>, List<bool>, bool) Calc_asym(Queue<string> exp, double x, int recursion) //takes the queue from Eval() now thats it's in the right order and actually evaluates it (also deals with a variable and undefined points and recursion)
        {
            if (recursion > 5) throw new NotSupportedException("Too much recursion.");
            Stack<double> stack = new Stack<double>();
            Queue<string> expcopy = new Queue<string>(exp);
            List<bool> asymList = new List<bool>(); //for things that could lead to asymptotes
            List<bool> jumpList = new List<bool>(); //for things that could lead to jumps/incontinuities
            bool badLog = false; //for tracking logs
            string cur;
            double num;
            double a, b;
            while (expcopy.Count > 0)
            {
                cur = expcopy.Dequeue();
                if (double.TryParse(cur, out num)) stack.Push(num);
                else
                {
                    try
                    {
                        switch (cur)
                        {
                            case "+":
                                stack.Push(stack.Pop() + stack.Pop());
                                break;
                            case "-":
                                a = stack.Pop();
                                b = stack.Pop();
                                stack.Push(b - a);
                                break;
                            case "*":
                                stack.Push(stack.Pop() * stack.Pop());
                                break;
                            case "/":
                                a = stack.Pop();
                                b = stack.Pop();
                                asymList.Add(Math.Sign(a) == -1);
                                asymList.Add(Math.Sign(b) == -1);
                                stack.Push(b / a);
                                break;
                            case "%":
                                a = stack.Pop();
                                b = stack.Pop();
                                jumpList.Add(Math.Sin(Math.PI * b / a) > 0);
                                stack.Push(b % a);
                                break;
                            case "^":
                                a = stack.Pop();
                                b = stack.Pop();
                                stack.Push(Math.Pow(b, a));
                                break;
                            case "neg":
                                stack.Push(-stack.Pop());
                                break;
                            case "sqrt":
                                stack.Push(Math.Sqrt(stack.Pop()));
                                break;
                            case "abs":
                                stack.Push(Math.Abs(stack.Pop()));
                                break;
                            case "e":
                                stack.Push(Math.E);
                                break;
                            case "pi":
                                stack.Push(Math.PI);
                                break;
                            case "sin":
                                stack.Push(Math.Sin(stack.Pop()));
                                break;
                            case "arcsin":
                                stack.Push(Math.Asin(stack.Pop()));
                                break;
                            case "sinh":
                                stack.Push(Math.Sinh(stack.Pop()));
                                break;
                            case "cos":
                                stack.Push(Math.Cos(stack.Pop()));
                                break;
                            case "arccos":
                                stack.Push(Math.Acos(stack.Pop()));
                                break;
                            case "cosh":
                                stack.Push(Math.Cosh(stack.Pop()));
                                break;
                            case "tan":
                                a = stack.Pop();
                                asymList.Add(Math.Sign(Math.Sin(a - Math.PI / 2)) == -1);
                                stack.Push(Math.Tan(a));
                                break;
                            case "arctan":
                                stack.Push(Math.Atan(stack.Pop()));
                                break;
                            case "tanh":
                                stack.Push(Math.Tanh(stack.Pop()));
                                break;
                            case "log":
                                a = stack.Pop();
                                if (a <= Globals.zoomX / 5000) badLog = true;
                                stack.Push(Math.Log10(a));
                                break;
                            case "ln":
                                a = stack.Pop();
                                if (a <= Globals.zoomX / 5000) badLog = true;
                                stack.Push(Math.Log(a));
                                break;
                            case "!":
                                a = stack.Pop();
                                asymList.Add((Math.Floor(a)) % 2 == 0 && a < 0); //changes sign at negative integers
                                stack.Push(SpecialFunctions.Gamma(a + 1));
                                break;
                            case "x":
                                stack.Push(x);
                                break;
                            case "int":
                                a = stack.Pop();
                                jumpList.Add(((int)a) % 2 == 0);
                                stack.Push((double)(int)a);
                                break;
                            case "floor":
                                a = stack.Pop();
                                jumpList.Add((Math.Floor(a)) % 2 == 0);
                                stack.Push((Math.Floor(a)));
                                break;
                            case "ceil":
                                a = stack.Pop();
                                jumpList.Add((Math.Ceiling(a)) % 2 == 0);
                                stack.Push((Math.Ceiling(a)));
                                break;
                            case "min":
                                a = stack.Pop();
                                b = stack.Pop();
                                jumpList.Add(a > b);
                                stack.Push((Math.Min(a, b)));
                                break;
                            case "max":
                                a = stack.Pop();
                                b = stack.Pop();
                                jumpList.Add(a > b);
                                stack.Push((Math.Max(a, b)));
                                break;
                            case "frac":
                                a = stack.Pop();
                                jumpList.Add(((int)a) % 2 == 0);
                                stack.Push(a - (double)(int)a);
                                break;
                            default:
                                Function f;
                                if (Globals.namedFunctions.TryGetValue(cur.TrimEnd('\''), out f))
                                {
                                    double x0 = stack.Pop();
                                    double dull;
                                    List<bool> l1, l2, l3, l4, l5, l6; //l1 and l2 are the normal asymptotes and jumps, l3-l6 are for deciding if maybe the derivative calculation will go through an asymptote/jump in which case we shouldn't do it
                                    bool lg1, lg2; //logs (1 sided asymptotes)
                                    (dull, l1, l2, lg1) = f.fFull(x0,recursion+1);
                                    asymList.AddRange(l1);
                                    jumpList.AddRange(l2);
                                    if (lg1) badLog = true;
                                    if (cur.Length == 1)
                                    {
                                        stack.Push(f.f(x0));
                                        break;
                                    }
                                    int n = cur.Length - cur.TrimEnd('\'').Length;
                                    if (n > 4) throw new NotSupportedException("Too many derivatives.");
                                    double ans = 0;
                                    double h = Globals.zoomX / 5000;
                                    (dull, l3, l4, lg1) = f.fFull(x0 + h * n / 1.99, recursion + 1);
                                    (dull, l5, l6, lg2) = f.fFull(x0 - h * n / 1.99, recursion + 1);
                                    if (!l3.SequenceEqual(l5) || !l4.SequenceEqual(l6) || lg1 != lg2) throw new Exception(); //derivative will caclulate using points on 2 sides of an asymptone/incontinuity which is very wrong
                                    (dull, l3, l4, lg1) = f.fFull(x0 + h * n / 1.3, recursion + 1);
                                    (dull, l5, l6, lg2) = f.fFull(x0 - h * n / 1.3, recursion + 1);
                                    if (!l3.SequenceEqual(l5) || !l4.SequenceEqual(l6) || lg1 != lg2) throw new Exception(); // another check with slightly different numbers in case function is so misbehaved here it's basically chaotic (give it more chances to catch that there's an asymptote)
                                    (dull, l3, l4, lg1) = f.fFull(x0 + h * n / 1.7, recursion + 1);
                                    (dull, l5, l6, lg2) = f.fFull(x0 - h * n / 1.7, recursion + 1);
                                    if (!l3.SequenceEqual(l5) || !l4.SequenceEqual(l6) || lg1 != lg2) throw new Exception(); // 1 more check for good luck
                                    for (int i = 0; i <= n; i++) //formula for derivative
                                    {
                                        ans += Math.Pow(-1, i) * Choose(n, i) * f.f(x0 + h * (n / 2.0 - i));
                                    }
                                    stack.Push(ans / Math.Pow(h, n));
                                    break;
                                }
                                Variable var;
                                if (Globals.variables.TryGetValue(cur, out var))
                                {
                                    double val;
                                    try
                                    {
                                        val = var.GetValue();
                                    }
                                    catch
                                    {
                                        throw new NotSupportedException("Variable \'" + cur + "\' is not defined properly.");
                                    }
                                    stack.Push(val);
                                    break;
                                }
                                //whatever we're handling is not a variable nor a function and it's probably wrong so we raise an error
                                if (cur == "(") throw new NotSupportedException("Missing closing parentheses.");
                                throw new NotSupportedException("Couldn't recognize variable/function \'" + cur + "\'.");

                        }
                    }
                    catch (InvalidOperationException)
                    {
                        throw new InvalidOperationException("Missing arguments for a function/operation (\'" + cur + "\'?).");
                    }
                }
            }
            if (stack.Count != 1) throw new NotSupportedException("Couldn't compute expression.");
            //if(!asymptotePossible) return (stack.Pop(), new List<bool>(), jumpList);
            return (stack.Pop(), asymList, jumpList, badLog);
        }

        public static double Evaluate(string exp)
        {
            return Calc(Eval(Convert(exp)));
        }

        //public static double Evaluate(string exp, double x)
        //{
        //    return Calc(Eval(Convert(exp)), x);
        //}
        public static (double, List<bool>, List<bool>, bool) Evaluate_asym(string exp, double x) //combination of all above funtions, input is a math expression with a variable "x", and the value of that variable. returns the answer + list of bools to indicate possible asymptotes/incontinuities
        {
            return Calc_asym(Eval(Convert(exp)), x,0);
        }


        private static double Choose(int n, int k) //needed to compute derivatives
        {
            double ans = 1;
            for (int i = 1; i <= k; i++)
            {
                ans *= (double)(n + 1 - i) / i;
            }
            return ans;
        }
    }
}
