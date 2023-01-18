using System;
using System.Collections;
using System.Data.SQLite;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace SQLiteWorkshop
{
    public static class Functions
    {

        /*
         * 
         * Aggregate: 
         * stdev, variance, mode, median, lower_quartile, upper_quartile.
         */
        /// <summary>
        /// Bind a UDF to a connection.
        /// </summary>
        /// <param name="connection">SQLite Connection</param>
        /// <param name="function">Function to bind to the connection.</param>
        public static void BindFunction(SQLiteConnection connection, SQLiteFunction function)
        {
            var attributes = function.GetType().GetCustomAttributes(typeof(SQLiteFunctionAttribute), true).Cast<SQLiteFunctionAttribute>().ToArray();
            if (attributes.Length == 0)
            {
                throw new InvalidOperationException("SQLiteFunction doesn't have SQLiteFunctionAttribute");
            }
            connection.BindFunction(attributes[0], function);
        }

        public class SQWFunction : SQLiteFunction
        {
            public string Description { get; set; }
            public string Category { get; set; }
        }

        #region DateFunctions

        /// <summary>
        /// SQLite Function to return date in a format compatible with SQLite.  This date can be inserted into
        /// a column defined as type 'date' or 'datetime'.
        /// </summary>
        [SQLiteFunction(Name = "ToDate", Arguments = 1, FuncType = FunctionType.Scalar)]
        public class ToDate : SQWFunction
        {

            public ToDate()
            {
                Description = "Change a Date value to an insertable SQLite format.";
                Category = "Date";
            }

            public override object Invoke(object[] args)
            {
                if (DateTime.TryParse(args[0].ToString(), out DateTime wrkDate))
                    return wrkDate.ToString("o");
                return null;
            }
        }

        #endregion

        #region StringFunctions
        // String: 
        // indexof, lastindexof, padleft, padright, proper, regex, reverse, right 

        /// <summary>
        /// SQLite Function that returns a the first occurrence of a string in a value.
        /// </summary>
        [SQLiteFunction(Name = "IndexOf", Arguments = 2, FuncType = FunctionType.Scalar)]
        public class IndexOf : SQWFunction
        {

            public IndexOf()
            {
                Description = "Find the first occurrence of a string.";
                Category = "String";
            }

            public override object Invoke(object[] args)
            {
                if (args[0] == null || args[1] == null) return -1;
                return args[0].ToString().IndexOf(args[1].ToString());
            }
        }
        /// <summary>
        /// SQLite Function that returns a the first occurrence of a string in a value.
        /// </summary>
        [SQLiteFunction(Name = "LastIndexOf", Arguments = 2, FuncType = FunctionType.Scalar)]
        public class LastIndexOf : SQWFunction
        {

            public LastIndexOf()
            {
                Description = "Find the last occurrence of a string..";
                Category = "String";
            }

            public override object Invoke(object[] args)
            {
                if (args[0] == null || args[1] == null) return -1;
                return args[0].ToString().LastIndexOf(args[1].ToString());
            }
        }
        /// <summary>
        /// SQLite Function that returns a string padded on the left to a specific length 
        /// with a fill character'
        /// </summary>
        [SQLiteFunction(Name = "PadLeft", Arguments = 3, FuncType = FunctionType.Scalar)]
        public class PadLeft : SQWFunction
        {

            public PadLeft()
            {
                Description = "Left Pad a with a fill character.";
                Category = "String";
            }

            public override object Invoke(object[] args)
            {
                args[0] = args[0]?.ToString() ?? string.Empty;
                if (string.IsNullOrEmpty(args[1]?.ToString())) args[1] = " ";       //Default to blank
                if (!int.TryParse(args[2]?.ToString(), out int len)) return null;
                return args[0].ToString().PadLeft(len, args[1].ToString()[0]);
            }
        }
        /// <summary>
        /// SQLite Function that returns a string padded on the right to a specific length 
        /// with a fill character'
        /// </summary>
        [SQLiteFunction(Name = "PadRight", Arguments = 3, FuncType = FunctionType.Scalar)]
        public class PadRight : SQWFunction
        {

            public PadRight()
            {
                Description = "Right Pad a with a fill character.";
                Category = "String";
            }

            public override object Invoke(object[] args)
            {
                args[0] = args[0]?.ToString() ?? string.Empty;
                if (string.IsNullOrEmpty(args[1]?.ToString())) args[1] = " ";       //Default to blank
                if (!int.TryParse(args[2]?.ToString(), out int len)) return null;
                return args[0].ToString().PadRight(len, args[1].ToString()[0]);
            }
        }
        /// <summary>
        /// SQLite Function that returns a string in Proper Case. For Example, if the db
        /// column contains JOE SMITH, this function returns Joe Smith.
        /// </summary>
        [SQLiteFunction(Name = "Proper", Arguments = 1, FuncType = FunctionType.Scalar)]
        public class Proper : SQWFunction
        {

            public Proper()
            {
                Description = "Change a string to Proper or Title Case.";
                Category = "String";
            }

            public override object Invoke(object[] args)
            {
                TextInfo ti = new CultureInfo(CultureInfo.CurrentCulture.Name).TextInfo;
                return args[0] == null ? null : ti.ToTitleCase(args[0].ToString().ToLower());
            }
        }

        /// <summary>
        /// SQLite Function to natch a Regular Expression.
        /// </summary>
        [SQLiteFunction(Name = "StrRegex", Arguments = 2, FuncType = FunctionType.Scalar)]
        public class StrRegex : SQWFunction
        {
            public StrRegex()
            {
                Description = "Determine if the value matches the Regular Expression.";
                Category = "String";
            }

            public override object Invoke(object[] args)
            {
                if (args[0] == null || args[1] == null) return null;
                Match m = Regex.Match(args[0].ToString(), args[1].ToString());
                return m.Success ? m.Index : -1;
            }
        }


        /// <summary>
        /// SQLite Function that reverses a string.
        /// </summary>
        [SQLiteFunction(Name = "Reverse", Arguments = 1, FuncType = FunctionType.Scalar)]
        public class Reverse : SQWFunction
        {

            public Reverse()
            {
                Description = "Reverse a string.";
                Category = "String";
            }

            public override object Invoke(object[] args)
            {
                args[0] = args[0]?.ToString() ?? string.Empty;
                char[] c = args[0].ToString().ToCharArray();
                Array.Reverse(c);
                return new string(c);
            }
        }
        /// <summary>
        /// SQLite Function that returns the rightmost characters of a string.
        /// </summary>
        [SQLiteFunction(Name = "StrRight", Arguments = 2, FuncType = FunctionType.Scalar)]
        public class StrRight : SQWFunction
        {

            public StrRight()
            {
                Description = "Return the rightmost characters of a string.";
                Category = "String";
            }

            public override object Invoke(object[] args)
            {
                args[0] = args[0]?.ToString() ?? string.Empty;
                if (!int.TryParse(args[1]?.ToString(), out int len)) return null;
                return args[0].ToString().Length <= len ? args[0].ToString() : args[0].ToString().Substring(args[0].ToString().Length - len, len);
            }
        }


        #endregion

        #region MathFunctions

        /*
         * Math: 
         * acos, asin, atan, atan2, acosh, asinh, atanh, degrees, radians, 
         * cos, sin, tan, cot, cosh, sinh, tanh, coth, exp, log, log10, log2, power, 
         * sign, sqrt, ceil, floor, pi, sec, cosec 
        */

        /// <summary>
        /// SQLite Function that returns arccosine.
        /// values
        /// </summary>
        [SQLiteFunction(Name = "ACos", Arguments = 1, FuncType = FunctionType.Scalar)]
        public class ACos : SQWFunction
        {
            public ACos()
            {
                Description = "Return ArcCosine in radians.";
                Category = "Math";
            }

            public override object Invoke(object[] args)
            {
                if (!double.TryParse(args[0]?.ToString(), out double cos)) return null;
                if (Math.Abs(cos) > 1) return null;
                return Math.Acos(cos);
            }
        }

        /// <summary>
        /// SQLite Function that returns inverse hyperbolic cosine.
        /// values
        /// </summary>
        [SQLiteFunction(Name = "ACosh", Arguments = 1, FuncType = FunctionType.Scalar)]
        public class ACosh : SQWFunction
        {
            public ACosh()
            {
                Description = "Return inverse hyperbolic cosine in radians.";
                Category = "Math";
            }

            public override object Invoke(object[] args)
            {
                if (!double.TryParse(args[0]?.ToString(), out double cosh)) return null;
                if (Math.Abs(cosh) > 1) return null;
                return (double)(Math.Exp(cosh) + Math.Exp(-cosh)) / 2;
            }
        }

        /// <summary>
        /// SQLite Function that returns arcsine.
        /// values
        /// </summary>
        [SQLiteFunction(Name = "ASin", Arguments = 1, FuncType = FunctionType.Scalar)]
        public class ASin : SQWFunction
        {
            public ASin()
            {
                Description = "Return ArcSine in radians.";
                Category = "Math";
            }

            public override object Invoke(object[] args)
            {
                if (!double.TryParse(args[0]?.ToString(), out double sin)) return null;
                if (Math.Abs(sin) > 1) return null;
                return Math.Asin(sin);
            }
        }

        /// <summary>
        /// SQLite Function that returns arctangent in radians.
        /// values
        /// </summary>
        [SQLiteFunction(Name = "ATan", Arguments = 1, FuncType = FunctionType.Scalar)]
        public class ATan : SQWFunction
        {
            public ATan()
            {
                Description = "Return ArcTangent in radians.";
                Category = "Math";
            }

            public override object Invoke(object[] args)
            {
                if (!double.TryParse(args[0]?.ToString(), out double tan)) return null;
                return Math.Atan(tan);
            }
        }

        /// <summary>
        /// SQLite Function that returns the angle whose tangent is the quotient of two specified numbers.
        /// values
        /// </summary>
        [SQLiteFunction(Name = "ATan2", Arguments = 2, FuncType = FunctionType.Scalar)]
        public class ATan2 : SQWFunction
        {
            public ATan2()
            {
                Description = "Return the angle in radians whose tangent is the quotient of two specified numbers.";
                Category = "Math";
            }

            public override object Invoke(object[] args)
            {
                if (!double.TryParse(args[0]?.ToString(), out double num0)) return null;
                if (!double.TryParse(args[1]?.ToString(), out double num1)) return null;
                return Math.Atan2(num0, num1);
            }
        }

        /// <summary>
        /// SQLite Function that returns the angle whose inverse hyberbolic tangent is the specified number.
        /// values
        /// </summary>
        [SQLiteFunction(Name = "ATanh", Arguments = 1, FuncType = FunctionType.Scalar)]
        public class ATanh : SQWFunction
        {
            public ATanh()
            {
                Description = "Return the angle in radians whose inverse hyberbolic tangent is the specified number.";
                Category = "Math";
            }

            public override object Invoke(object[] args)
            {
                if (!double.TryParse(args[0]?.ToString(), out double num)) return null;
                return (double)(Math.Exp(num) - Math.Exp(-num)) / (double)(Math.Exp(num) + Math.Exp(-num));
            }
        }

        /// <summary>
        /// SQLite Function that returns the Cube root of a value.
        /// </summary>
        [SQLiteFunction(Name = "CBRT", Arguments = 1, FuncType = FunctionType.Scalar)]
        public class CBRT : SQWFunction
        {
            public CBRT()
            {
                Description = "Compute the cube root of a value.";
                Category = "Math";
            }

            public override object Invoke(object[] args)
            {
                if (!double.TryParse(args[0]?.ToString(), out double d)) return null;
                return Math.Pow(d, (double)1/3);
            }
        }


        /// <summary>
        /// SQLite Function that returns the smallest integer greater than the specified number.
        /// values
        /// </summary>
        [SQLiteFunction(Name = "Ceil", Arguments = 1, FuncType = FunctionType.Scalar)]
        public class Ceil : SQWFunction
        {
            public Ceil()
            {
                Description = "Return the smallest integer greater than the specified number.";
                Category = "Math";
            }

            public override object Invoke(object[] args)
            {
                if (!double.TryParse(args[0]?.ToString(), out double num)) return null;
                return Math.Ceiling(num);
            }
        }

        /// <summary>
        /// SQLite Function that returns Cosine.
        /// values
        /// </summary>
        [SQLiteFunction(Name = "Cos", Arguments = 1, FuncType = FunctionType.Scalar)]
        public class Cos : SQWFunction
        {
            public Cos()
            {
                Description = "Return Cosine.";
                Category = "Math";
            }

            public override object Invoke(object[] args)
            {
                if (!double.TryParse(args[0]?.ToString(), out double rad)) return null;
                return Math.Cos(rad);
            }
        }

        /// <summary>
        /// SQLite Function that returns Codecant.
        /// values
        /// </summary>
        [SQLiteFunction(Name = "Cosec", Arguments = 1, FuncType = FunctionType.Scalar)]
        public class Cosec : SQWFunction
        {
            public Cosec()
            {
                Description = "Return Cosecant.";
                Category = "Math";
            }

            public override object Invoke(object[] args)
            {
                if (!double.TryParse(args[0]?.ToString(), out double rad)) return null;
                return (double)(1 / Math.Sin(rad));
            }
        }

        /// <summary>
        /// SQLite Function that returns Hyperbolic Cosine.
        /// values
        /// </summary>
        [SQLiteFunction(Name = "Cosh", Arguments = 1, FuncType = FunctionType.Scalar)]
        public class Cosh : SQWFunction
        {
            public Cosh()
            {
                Description = "Return Hyperbolic Cosine.";
                Category = "Math";
            }

            public override object Invoke(object[] args)
            {
                if (!double.TryParse(args[0]?.ToString(), out double rad)) return null;
                return Math.Cosh(rad);
            }
        }

        /// <summary>
        /// SQLite Function that returns Cotangent.
        /// values
        /// </summary>
        [SQLiteFunction(Name = "Cot", Arguments = 1, FuncType = FunctionType.Scalar)]
        public class Cot : SQWFunction
        {
            public Cot()
            {
                Description = "Return Cotangent.";
                Category = "Math";
            }

            public override object Invoke(object[] args)
            {
                if (!double.TryParse(args[0]?.ToString(), out double rad)) return null;
                return (double)(1/Math.Tan(rad));
            }
        }

        /// <summary>
        /// SQLite Function that returns Hyberbolic Cotangent.
        /// values
        /// </summary>
        [SQLiteFunction(Name = "Coth", Arguments = 1, FuncType = FunctionType.Scalar)]
        public class Coth : SQWFunction
        {
            public Coth()
            {
                Description = "Return Hyberbolic Cotangent.";
                Category = "Math";
            }

            public override object Invoke(object[] args)
            {
                if (!double.TryParse(args[0]?.ToString(), out double rad)) return null;
                return (double)(Math.Exp(rad) + Math.Exp(-rad)) / (double)(Math.Exp(rad) - Math.Exp(-rad));
            }
        }

        /// <summary>
        /// SQLite Function that converts radians to Degrees.
        /// values
        /// </summary>
        [SQLiteFunction(Name = "Degrees", Arguments = 1, FuncType = FunctionType.Scalar)]
        public class Degrees : SQWFunction
        {
            public Degrees()
            {
                Description = "Convert Radians to Degrees.";
                Category = "Math";
            }

            public override object Invoke(object[] args)
            {
                if (!double.TryParse(args[0]?.ToString(), out double rad)) return null;
                return (double)(((double)180 / Math.PI) * rad);
            }
        }

        /// <summary>
        /// SQLite Function that raises e to the specified power.
        /// values
        /// </summary>
        [SQLiteFunction(Name = "Exp", Arguments = 1, FuncType = FunctionType.Scalar)]
        public class Exp : SQWFunction
        {
            public Exp()
            {
                Description = "Raise e to the specified power.";
                Category = "Math";
            }

            public override object Invoke(object[] args)
            {
                if (!double.TryParse(args[0]?.ToString(), out double pow)) return null;
                return Math.Exp(pow);
            }
        }

        /// <summary>
        /// SQLite Function that returns the largest integer smaller than the specified number.
        /// values
        /// </summary>
        [SQLiteFunction(Name = "Floor", Arguments = 1, FuncType = FunctionType.Scalar)]
        public class Floor : SQWFunction
        {
            public Floor()
            {
                Description = "Return the largest integer smaller than the specified number.";
                Category = "Math";
            }

            public override object Invoke(object[] args)
            {
                if (!double.TryParse(args[0]?.ToString(), out double num)) return null;
                return Math.Floor(num);
            }
        }


        /// <summary>
        /// SQLite Function that returns true for integer values and false for non-integer
        /// values
        /// </summary>
        [SQLiteFunction(Name = "IsInteger", Arguments = 1, FuncType = FunctionType.Scalar)]
        public class IsInteger : SQWFunction
        {
            public IsInteger()
            {
                Description = "Determine if a value is an integer.";
                Category = "Math";
            }

            public override object Invoke(object[] args)
            {
                return args[0] == null ? false : int.TryParse(args[0].ToString(), out _);
            }
        }

        /// <summary>
        /// SQLite Function that returns logarithm base e of the specified number.
        /// values
        /// </summary>
        [SQLiteFunction(Name = "Log", Arguments = 1, FuncType = FunctionType.Scalar)]
        public class Log : SQWFunction
        {
            public Log()
            {
                Description = "Return logarithm base e of the specified number.";
                Category = "Math";
            }

            public override object Invoke(object[] args)
            {
                if (!double.TryParse(args[0]?.ToString(), out double num)) return null;
                return Math.Log(num);
            }
        }

        /// <summary>
        /// SQLite Function that returns base 10 logarithm of the specified number.
        /// values
        /// </summary>
        [SQLiteFunction(Name = "Log10", Arguments = 1, FuncType = FunctionType.Scalar)]
        public class Log10 : SQWFunction
        {
            public Log10()
            {
                Description = "Return base 10 logarithm of the specified number.";
                Category = "Math";
            }

            public override object Invoke(object[] args)
            {
                if (!double.TryParse(args[0]?.ToString(), out double num)) return null;
                return Math.Log10(num);
            }
        }

        /// <summary>
        /// SQLite Function that returns base 2 logarithm of the specified number.
        /// values
        /// </summary>
        [SQLiteFunction(Name = "Log2", Arguments = 1, FuncType = FunctionType.Scalar)]
        public class Log2 : SQWFunction
        {
            public Log2()
            {
                Description = "Return base 2 logarithm of the specified number.";
                Category = "Math";
            }

            public override object Invoke(object[] args)
            {
                if (!double.TryParse(args[0]?.ToString(), out double num)) return null;
                return Math.Log(num, 2);
            }
        }

        /// <summary>
        /// SQLite Function that returns the value of PI.
        /// values
        /// </summary>
        [SQLiteFunction(Name = "IsInteger", Arguments = 0, FuncType = FunctionType.Scalar)]
        public class PI : SQWFunction
        {
            public PI()
            {
                Description = "Return the value of PI.";
                Category = "Math";
            }

            public override object Invoke(object[] args)
            {
                return Math.PI;
            }
        }

        /// <summary>
        /// SQLite Function that returns number raised to the specified power.
        /// values
        /// </summary>
        [SQLiteFunction(Name = "Pow", Arguments = 2, FuncType = FunctionType.Scalar)]
        public class Pow : SQWFunction
        {
            public Pow()
            {
                Description = "Return number raised to the specified power.";
                Category = "Math";
            }

            public override object Invoke(object[] args)
            {
                if (!double.TryParse(args[0]?.ToString(), out double num)) return null;
                if (!double.TryParse(args[1]?.ToString(), out double pow)) return null;
                return Math.Pow(num, pow);
            }
        }

        /// <summary>
        /// SQLite Function that converts Degrees to Radians.
        /// values
        /// </summary>
        [SQLiteFunction(Name = "Radians", Arguments = 1, FuncType = FunctionType.Scalar)]
        public class Radians : SQWFunction
        {
            public Radians()
            {
                Description = "Convert Degrees to Radians.";
                Category = "Math";
            }

            public override object Invoke(object[] args)
            {
                if (!double.TryParse(args[0]?.ToString(), out double deg)) return null;
                return (double)((Math.PI / (double)180) * deg);
            }
        }

        /// <summary>
        /// SQLite Function that returns Secant.
        /// values
        /// </summary>
        [SQLiteFunction(Name = "Sec", Arguments = 1, FuncType = FunctionType.Scalar)]
        public class Sec : SQWFunction
        {
            public Sec()
            {
                Description = "Return Secant.";
                Category = "Math";
            }

            public override object Invoke(object[] args)
            {
                if (!double.TryParse(args[0]?.ToString(), out double rad)) return null;
                return (double)(1 / Math.Cos(rad));
            }
        }

        /// <summary>
        /// SQLite Function that returns the sign of a number.
        /// values
        /// </summary>
        [SQLiteFunction(Name = "Sign", Arguments = 1, FuncType = FunctionType.Scalar)]
        public class Sign : SQWFunction
        {
            public Sign()
            {
                Description = "Return sign (-1-Neg, 0-Zero, 1-Pos).";
                Category = "Math";
            }

            public override object Invoke(object[] args)
            {
                if (!double.TryParse(args[0]?.ToString(), out double num)) return null;
                return Math.Sign(num);            }
        }

        /// <summary>
        /// SQLite Function that returns Sine.
        /// values
        /// </summary>
        [SQLiteFunction(Name = "Sin", Arguments = 1, FuncType = FunctionType.Scalar)]
        public class Sin : SQWFunction
        {
            public Sin()
            {
                Description = "Return Sine.";
                Category = "Math";
            }

            public override object Invoke(object[] args)
            {
                if (!double.TryParse(args[0]?.ToString(), out double rad)) return null;
                return Math.Sin(rad);
            }
        }

        /// <summary>
        /// SQLite Function that returns Hyperbolic Sine.
        /// values
        /// </summary>
        [SQLiteFunction(Name = "Sinh", Arguments = 1, FuncType = FunctionType.Scalar)]
        public class Sinh : SQWFunction
        {
            public Sinh()
            {
                Description = "Return Hyperbolic Sine.";
                Category = "Math";
            }

            public override object Invoke(object[] args)
            {
                if (!double.TryParse(args[0]?.ToString(), out double rad)) return null;
                return Math.Sinh(rad);
            }
        }

        /// <summary>
        /// SQLite Function that returns the Square root of a value.
        /// </summary>
        [SQLiteFunction(Name = "SQRT", Arguments = 1, FuncType = FunctionType.Scalar)]
        public class SQRT : SQWFunction
        {
            public SQRT()
            {
                Description = "Compute the square root of a value.";
                Category = "Math";
            }

            public override object Invoke(object[] args)
            {
                if (!double.TryParse(args[0]?.ToString(), out double d)) return null;
                return Math.Sqrt(d);
            }
        }

        /// <summary>
        /// SQLite Function that returns Tangent.
        /// values
        /// </summary>
        [SQLiteFunction(Name = "Tan", Arguments = 1, FuncType = FunctionType.Scalar)]
        public class Tan : SQWFunction
        {
            public Tan()
            {
                Description = "Return Tangent.";
                Category = "Math";
            }

            public override object Invoke(object[] args)
            {
                if (!double.TryParse(args[0]?.ToString(), out double rad)) return null;
                return Math.Tan(rad);
            }
        }

        /// <summary>
        /// SQLite Function that returns Hyperbolic Tangent.
        /// values
        /// </summary>
        [SQLiteFunction(Name = "Tanh", Arguments = 1, FuncType = FunctionType.Scalar)]
        public class Tanh : SQWFunction
        {
            public Tanh()
            {
                Description = "Return Hyperbolic Tangent.";
                Category = "Math";
            }

            public override object Invoke(object[] args)
            {
                if (!double.TryParse(args[0]?.ToString(), out double rad)) return null;
                return Math.Tanh(rad);
            }
        }

        /// <summary>
        /// SQLite Function that returns the integral part of the specified number.
        /// values
        /// </summary>
        [SQLiteFunction(Name = "Trunc", Arguments = 1, FuncType = FunctionType.Scalar)]
        public class Trunc : SQWFunction
        {
            public Trunc()
            {
                Description = "Return the integral part of the specified number.";
                Category = "Math";
            }

            public override object Invoke(object[] args)
            {
                if (!double.TryParse(args[0]?.ToString(), out double num)) return null;
                return Math.Truncate(num);
            }
        }
        #endregion

        #region OtherFunctions
        /// <summary>
        /// SQLite Function that returns the median value of a column.  Applicable to numeric values only.
        /// </summary>
        [SQLiteFunction(Name = "Median", Arguments = 1, FuncType = FunctionType.Aggregate)]
        public class Median : SQWFunction
        {
            public Median()
            {
                Description = "Determine the median value of a column.";
                Category = "Other";
            }

            public override void Step (object[] args, int stepNumber, ref object contextData)
            {
                if (!double.TryParse(args[0]?.ToString(), out double d)) return;
                ArrayList a = contextData == null ? new ArrayList() : (ArrayList)contextData;
                a.Add(d);
                contextData = a;
            }

            public override object Final(object contextData)
            {
                if (contextData == null) return null;
                ArrayList a = (ArrayList)contextData;
                if (a.Count == 0) return null;
                a.Sort();
                return a.Count % 2 == 0 ? (double)((double)a[(a.Count / 2)] + (double)a[(a.Count / 2) + 1]) /2 : a[(a.Count / 2)];
            }
        }

        /// <summary>
        /// SQLite Function that returns standard deviation.  Applicable to numeric values only.
        /// </summary>
        [SQLiteFunction(Name = "StdDev", Arguments = 1, FuncType = FunctionType.Aggregate)]
        public class StdDev : SQWFunction
        {
            internal struct sdev
            {
                internal double m;
                internal double s;
                internal long k;
            }
            public StdDev()
            {
                Description = "Determine the standard deviation of a column.";
                Category = "Other";
            }

            public override void Step(object[] args, int stepNumber, ref object contextData)
            {
                if (!double.TryParse(args[0]?.ToString(), out double d)) return;
                sdev sd = stepNumber == 1 ? new sdev() { m = 0, s = 0, k = 0 } : (sdev)contextData;
                sd.k++;
                double t = sd.m;
                sd.m += (d - t) / sd.k;
                sd.s += (d - t) * (d - sd.m);
                contextData = sd;
            }

            public override object Final(object contextData)
            {
                if (contextData == null) return null;
                sdev sd = (sdev)contextData;
                return Math.Sqrt(sd.s / (sd.k - 1));
            }
        }
        #endregion

        /*
        public static class MathHelper
        {
            // Secant 
            public static double Sec(double x)
            {
                return 1 / Math.Cos(x);
            }

            // Cosecant
            public static double Cosec(double x)
            {
                return 1 / Math.Sin(x);
            }

            // Cotangent 
            public static double Cotan(double x)
            {
                return 1 / Math.Tan(x);
            }

            // Inverse Sine 
            public static double Arcsin(double x)
            {
                return Math.Atan(x / Math.Sqrt(-x * x + 1));
            }

            // Inverse Cosine 
            public static double Arccos(double x)
            {
                return Math.Atan(-x / Math.Sqrt(-x * x + 1)) + 2 * Math.Atan(1);
            }


            // Inverse Secant 
            public static double Arcsec(double x)
            {
                return 2 * Math.Atan(1) - Math.Atan(Math.Sign(x) / Math.Sqrt(x * x - 1));
            }

            // Inverse Cosecant 
            public static double Arccosec(double x)
            {
                return Math.Atan(Math.Sign(x) / Math.Sqrt(x * x - 1));
            }

            // Inverse Cotangent 
            public static double Arccotan(double x)
            {
                return 2 * Math.Atan(1) - Math.Atan(x);
            }

            // Hyperbolic Sine 
            public static double HSin(double x)
            {
                return (double)(Math.Exp(x) - Math.Exp(-x)) / 2;
            }

            // Hyperbolic Cosine 
            public static double HCos(double x)
            {
                return (double)(Math.Exp(x) + Math.Exp(-x)) / 2;
            }

            // Hyperbolic Tangent 
            public static double HTan(double x)
            {
                return (double)(Math.Exp(x) - Math.Exp(-x)) / (double)(Math.Exp(x) + Math.Exp(-x));
            }

            // Hyperbolic Secant 
            public static double HSec(double x)
            {
                return 2 / (double)(Math.Exp(x) + Math.Exp(-x));
            }

            // Hyperbolic Cosecant 
            public static double HCosec(double x)
            {
                return 2 / (double)(Math.Exp(x) - Math.Exp(-x));
            }

            // Hyperbolic Cotangent 
            public static double HCotan(double x)
            {
                return (double)(Math.Exp(x) + Math.Exp(-x)) / (double)(Math.Exp(x) - Math.Exp(-x));
            }

            // Inverse Hyperbolic Sine 
            public static double HArcsin(double x)
            {
                return Math.Log(x + Math.Sqrt(x * x + 1));
            }

            // Inverse Hyperbolic Cosine 
            public static double HArccos(double x)
            {
                return Math.Log(x + Math.Sqrt(x * x - 1));
            }

            // Inverse Hyperbolic Tangent 
            public static double HArctan(double x)
            {
                return Math.Log((double)(1 + x) / (double)(1 - x)) / 2;
            }

            // Inverse Hyperbolic Secant 
            public static double HArcsec(double x)
            {
                return Math.Log((double)(Math.Sqrt(-x * x + 1) + 1) / x);
            }

            // Inverse Hyperbolic Cosecant 
            public static double HArccosec(double x)
            {
                return Math.Log((double)(Math.Sign(x) * Math.Sqrt(x * x + 1) + 1) / x);
            }

            // Inverse Hyperbolic Cotangent 
            public static double HArccotan(double x)
            {
                return Math.Log((double)(x + 1) / (double)(x - 1)) / 2;
            }

            // Logarithm to base N 
            public static double LogN(double x, double n)
            {
                return Math.Log(x) / Math.Log(n);
            }
        }
        */
    }
}
