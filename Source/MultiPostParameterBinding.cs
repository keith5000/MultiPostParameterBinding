using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Metadata;
using Newtonsoft.Json;

namespace Keith5000.ModelBinding
{
	/// <summary>
	/// Allows API methods to accept multiple parameters via a POST operation. If a method is marked
	/// with <see cref="MultiPostParametersAttribute"/> and supports HTTP POSTs, clients must pass
	/// a single object with a property for each parameter. Both JSON and standard query string
	/// posting is supported. The parameters can be of any type.
	/// </summary>
	/// <remarks>
	/// The default behavior of .NET Web API is to allow just 1 parameter via a POST operation.
	/// 
	/// Example:
	/// Given this web API method:
	///		[MultiPostParameters] public string MyMethod(CustomObject param1, CustomObject param2, string param3) { ... }
	///		
	/// a client would pass either a JSON object in this format:
	///		{ param1: {...}, param2: {...}, param3: "..." }
	///		
	/// or an encoded query string:
	///		param1=...&param2=...&param3=...
	///		
	/// This class is based on a similar implementation from https://github.com/RoyiNamir/SimplePostVariableParameterBindingExtended
	/// (also based on Rick Strahl's SimplePostVariableParameterBinding class) which only supported simple types.
	/// </remarks>
	public class MultiPostParameterBinding : HttpParameterBinding
	{

		public MultiPostParameterBinding(HttpParameterDescriptor descriptor)
			: base(descriptor)
		{ }


		/// <summary>
		/// Parses the parameter value from the request body.
		/// </summary>
		/// <param name="metadataProvider"></param>
		/// <param name="actionContext"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public override Task ExecuteBindingAsync(ModelMetadataProvider metadataProvider, HttpActionContext actionContext, CancellationToken cancellationToken)
		{
			// read request body (query string or JSON) into name/value pairs
			NameValueCollection parameters = ParseParametersFromBody(actionContext.Request);

			// try to get parameter value from parsed body
			string stringValue = null;
			if (parameters != null)
				stringValue = parameters[Descriptor.ParameterName];

			// if not found in body, try reading query string
			if (stringValue == null)
			{
				var queryStringPairs = actionContext.Request.GetQueryNameValuePairs();
				if (queryStringPairs != null)
					stringValue = queryStringPairs
						.Where(kv => kv.Key == Descriptor.ParameterName)
						.Select(kv => kv.Value)
						.FirstOrDefault();
			}

			// if found, convert/deserialize the parameter and set the binding
			if (stringValue != null)
			{
				object paramValue;
				if (Descriptor.ParameterType == typeof(string))
					paramValue = stringValue;
				else if (Descriptor.ParameterType.IsPrimitive || Descriptor.ParameterType.IsValueType)	// TODO: Are these conditions ok? I'd rather not have to check that the type implements IConvertible.
					paramValue = Convert.ChangeType(stringValue, Descriptor.ParameterType);
				else
					paramValue = JsonConvert.DeserializeObject(stringValue, Descriptor.ParameterType);

				// Set the binding result here
				SetValue(actionContext, paramValue);
			}

			// now, we can return a completed task with no result
			TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();
			tcs.SetResult(default(object));
			return tcs.Task;
		}


		/// <summary>
		/// Read parameters from the body into a collection.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		private NameValueCollection ParseParametersFromBody(HttpRequestMessage request)
		{
			const string cacheKey = "MultiPostParameterBinding";

			// try to read out of cache first
			object result;
			if (!request.Properties.TryGetValue(cacheKey, out result))
			{
				// if not in cache, get value from request body based on the content type
				MediaTypeHeaderValue contentType = request.Content.Headers.ContentType;
				if (contentType != null)
				{
					switch (contentType.MediaType)
					{
						case "application/json":
							// deserialize to Dictionary and convert to NameValueCollection
							string content = request.Content.ReadAsStringAsync().Result;
							var values = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);
							result = values.Aggregate(new NameValueCollection(), (seed, current) =>
							{
								seed.Add(current.Key, current.Value == null ? "" : current.Value.ToString());
								return seed;
							});
							break;

						case "application/x-www-form-urlencoded":
							result = request.Content.ReadAsFormDataAsync().Result;
							break;
					}

					// write to cache
					if (result != null)
						request.Properties.Add(cacheKey, result);
				}
			}

			return result as NameValueCollection;
		}


		/// <summary>
		/// Returns a <see cref="MultiPostParameterBinding"/> object to use for the API method parameter specified.
		/// An object is only returned if the parameter's method is marked with <see cref="MultiPostParametersAttribute"/>,
		/// otherwise null is returned.
		/// </summary>
		/// <param name="descriptor"></param>
		/// <returns></returns>
		public static MultiPostParameterBinding CreateBindingForMarkedParameters(HttpParameterDescriptor descriptor)
		{
			Contract.Requires(descriptor != null);

			// short circuit if action does not have this attribute
			if (!descriptor.ActionDescriptor.GetCustomAttributes<MultiPostParametersAttribute>().Any())
				return null;

			// Only apply this binder on POST and PUT operations
			Collection<HttpMethod> supportedMethods = descriptor.ActionDescriptor.SupportedHttpMethods;
			if (supportedMethods.Contains(HttpMethod.Post) || supportedMethods.Contains(HttpMethod.Put))
				return new MultiPostParameterBinding(descriptor);

			return null;
		}

	}
}