using System;

namespace Keith5000.ModelBinding
{
	/// <summary>
	/// Use this attribute on API methods that need to support multiple POST parameters. See the comments in
	/// class <see cref="MultiPostParameterBinding"/> for how to enable support for multiple POST parameters.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public sealed class MultiPostParametersAttribute : Attribute
	{

		/// <summary>
		/// Specifies whether to support multiple POST parameters. This is true by default.
		/// </summary>
		public bool Enabled { get; set; }



		/// <summary>
		/// Initializes a new instance of the <see cref="MultiPostParametersAttribute"/> class with <see cref="MultiPostParametersAttribute.Enabled"/> set to true.
		/// </summary>
		public MultiPostParametersAttribute()
		{
			Enabled = true;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MultiPostParametersAttribute"/> class with <see cref="MultiPostParametersAttribute.Enabled"/> set to the specified value.
		/// </summary>
		public MultiPostParametersAttribute(bool enabled)
		{
			Enabled = enabled;
		}

	}
}