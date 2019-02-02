//	Copyright © 2019, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Experiment.NullableTypeReflection
{
    public class Probe
    {
        public string A { get; set; }
        public string? B { get; set; }
        public string? C { get; }

        public int[] ArrayA { get; set; }
        public int[]? ArrayB { get; set; }
        public string[]? ArrayC { get; set; }
        public string?[] ArrayD { get; set; }
        public string?[]? ArrayE { get; set; }

        public int N { get; }

        public Probe(string? a, string? b, string? c, int n)
        {
            this.A = a ?? "";
            this.B = b;
            this.C = c;
            this.N = n;
            this.ArrayA = null!;
            this.ArrayB = null;
            this.ArrayC = null;
            this.ArrayD = null!;
            this.ArrayE = null;
        }
    }
}
