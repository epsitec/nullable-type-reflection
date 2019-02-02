//	Copyright © 2019, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Experiment.NullableTypeReflection
{
    public static class Program
    {
        public static void Main()
        {
            Program.DumpPublicProperties (typeof (Probe));
            Program.DumpPublicProperties (typeof (System.Collections.ArrayList));
            Program.DumpPublicProperties (typeof (System.Collections.Generic.IList<int>));
        }

        public static void DumpPublicProperties(System.Type type)
        {
            var props = type
                .GetProperties (System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public)
                .Select (p => new
                {
                    Name = p.Name,
                    Type = p.PropertyType.Name,
                    NullableAttribute = 
                        p.GetCustomAttributes (false)
                         .OfType<NullableAttribute> ()
                         .FirstOrDefault ()
                         .GetDescription ()
                });

            foreach (var prop in props)
            {
                Console.WriteLine ($"{type.Name}.{prop.Name}: {prop.Type}, {prop.NullableAttribute}");
            }
        }
    
        public static string GetDescription(this NullableAttribute attr)
        {
            if (attr == null)
            {
                return "no attribute";
            }
            if (attr.Modes != null)
            {
                return "[" + string.Join (", ", attr.Modes.Select (x => x.GetDescription ())) + "]";
            }

            return attr.Mode.GetDescription ();
         }
         public static string GetDescription(this byte value)
         {
            return value switch
            { 
                0 => "value type",
                1 => "non-nullable ref. type",
                2 => "nullable ref. type",
                _ => $"unrecognized {value}"
            };
        }
    }
}
