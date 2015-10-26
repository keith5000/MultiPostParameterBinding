using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Keith5000.ModelBinding
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public sealed class MultiPostParametersAttribute : Attribute
	{
	}
}