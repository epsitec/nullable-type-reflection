//	Copyright © 2019, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace System.Runtime.CompilerServices
{
    [AttributeUsage (AttributeTargets.Class | AttributeTargets.Event | AttributeTargets.Field | AttributeTargets.GenericParameter | AttributeTargets.Module | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue, AllowMultiple = false)]
    public class NullableAttribute : Attribute
    {
        public byte Mode { get; }
        public byte[]? Modes { get; } 

        public NullableAttribute(byte mode)
        {
            this.Mode = mode;
        }

        public NullableAttribute(byte[] modes)
        {
            this.Modes = modes;
        }
    }
}
