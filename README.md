# MultiPostParameterBinding
A model binder for .NET Web API to support multiple POST parameters in an action method.

-- **Warning: Currently in beta and not fully tested** --

## Why is this needed?
Because of how Web API interprets POSTed data, an action method cannot contain more than one POST parameter, per the default behavior. For example, calls to this method will fail because Web API does not know how to parse the POST content into multiple parameters:
```
[HttpPost]
public string DoSomething(CustomType param1, CustomType param2, [FromBody]string param3) { ... }
```

<sup>For a detailed technical explanation of this limitation see Rick Strahl's article [Passing multiple POST parameters to Web API Controller Methods](http://weblog.west-wind.com/posts/2012/May/08/Passing-multiple-POST-parameters-to-Web-API-Controller-Methods) on which this code is based.</sup>

You can work around this limitation by including this repository's code in your project.

## How to use MultiPostParameterBinding
1) Download the code in the **[Source](https://github.com/keith5000/MultiPostParameterBinding/tree/master/Source)** folder and add it to your Web API project or any other project in the solution.

2) Use attribute **[MultiPostParameters]** on the action methods that need to support multiple POST parameters. I recommend coupling it with [HttpPost] since [not all methods support POST by default](http://stackoverflow.com/questions/23686841/is-there-a-default-verb-applied-to-a-web-api-apicontroller-method).
```
[MultiPostParameters]
public string DoSomething(CustomType param1, CustomType param2, string param3) { ... }

[HttpPost, MultiPostParameters]
public string DoSomethingElse(string param1, int param2) { ... }
```
3) Add this line in Global.asax.cs to the Application_Start method anywhere *before* the call to **GlobalConfiguration.Configure(WebApiConfig.Register)**:
```
GlobalConfiguration.Configuration.ParameterBindingRules.Insert(0, MultiPostParameterBinding.CreateBindingForMarkedParameters);
```
4) Have your clients pass the parameters as properties of an object. An example JSON object for the `DoSomething(param1, param2, param3)` method is:
```
{ param1:{ Text:"" }, param2:{ Text:"" }, param3:"" }
```
Example JQuery:
```
$.ajax({
	data: JSON.stringify({ param1:{ Text:"" }, param2:{ Text:"" }, param3:"" }),
	url: '/MyService/DoSomething',
	contentType: "application/json", method: "POST", processData: false
})
.success(function (result) { ... });
```

## What is supported
- The method's parameters can be of any primitive or complex type, including your custom objects, and in any order.
- The method must allow HTTP POST or PUT.
- Tested only (and not thoroughly) against .NET Web API 2 with .NET 4.5.

## Bugs?
This project is in beta. The code has not been fully tested. Please let me know of any issues.

## Acknowledgements
This code is based on Royi Namir's [SimplePostVariableParameterBindingExtended](https://github.com/RoyiNamir/SimplePostVariableParameterBindingExtended) which is in turn based on Rick Strahl's article [Passing multiple POST parameters to Web API Controller Methods](http://weblog.west-wind.com/posts/2012/May/08/Passing-multiple-POST-parameters-to-Web-API-Controller-Methods). Their versions supported only parameters that had simple types. I extended support to all types and made some fixes.
