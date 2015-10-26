using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Example.DataObjects;
using Keith5000.ModelBinding;

namespace Example.Controllers.API
{
	public class SampleApiController : ApiController
	{

		// This API method uses MultiPostParameterBinding to allow each parameter to be specified via HTTP Post.
		[HttpPost, MultiPostParameters]
		public string DoSomething(CustomType param1, CustomType param2, string param3)
		{
			// validate parameters
			if (param1 == null) throw new ArgumentNullException("param1");
			if (string.IsNullOrEmpty(param1.Text)) throw new ArgumentException("param1.Text cannot be blank.", "param3");
			if (param2 == null) throw new ArgumentNullException("param2");
			if (string.IsNullOrEmpty(param2.Text)) throw new ArgumentException("param1.Text cannot be blank.", "param3");
			if (string.IsNullOrEmpty(param3)) throw new ArgumentNullException("param3");

			return string.Format("{0} {1} {2}", param1.Text, param2.Text, param3);
		}

	}
}
