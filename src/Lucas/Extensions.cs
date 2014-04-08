using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lucas
{
    using Lucas.Drawing;
    using Lucas.Drawing.Shapes;
using System.IO;
    using System.IO.Pipes;

    public static class Extensions
    {
        public static void Assert<T>(this T self, T other, string message = null)
            where T : IComparable<T>
        {
            if (!self.Equals(other))
                throw new Exception(message);
        }

        public static void Assert<T>(this T self, T other, Func<string> message)
            where T : IComparable<T>
        {
            if (!self.Equals(other))
                throw new Exception(message != null ? message() : null);
        }

        public static string Print(this IShape shape)
        {
            return shape != null ? shape.ToString() : null;
        }

        public static string Stringify(this IShape shape)
        {
            return shape != null ? string.Join(" ", shape.GetType().Name.ToLower(), string.Join(" ", shape.Arguments)) : null;
        }

        public static bool TryParse(this string s, out int value)
        {
            return int.TryParse(s, out value);
        }

        public static bool TryParse(this IEnumerable<string> source, out double[] values)
        {
            var pass = true;
            var result = new List<double>();

            foreach (var s in source)
            {
                double value;
                pass = double.TryParse(s, out value);

                if (!pass)
                    break;

                result.Add(value);
            }

            values = result.ToArray();

            return pass;
        }

        public static void Send(this TextWriter writer, string message, Func<bool> constraint)
        {
            if (constraint == null || constraint())
            {
                writer.WriteLine(message);
                writer.Flush();
            }
        }

        public static bool Send(this NamedPipeServerStream stream, string message, params object[] args)
        {
            if (!stream.IsConnected)
                return false;

            var writer = new StreamWriter(stream);

            writer.WriteLine(message, args);
            writer.Flush();

            return true;
        }
    }
}
