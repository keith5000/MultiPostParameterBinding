# MultiPostParameterBinding
A model binder for .NET Web API to support multiple POST parameters in an action method.

## Why is this needed?
Because of how Web API interprets POSTed data, it is not possible to create an action method that contains more than one POST parameter. This method, for example:

```
[HttpPost]
public string DoSomething(CustomType param1, CustomType param2, [FromBody]string param3) { ... }
```

<sup>For a detailed technical explanation of this limitation see Rick Strahl's article [Passing multiple POST parameters to Web API Controller Methods](http://weblog.west-wind.com/posts/2012/May/08/Passing-multiple-POST-parameters-to-Web-API-Controller-Methods) on which this code is based.</sup>

You can work around this limitation by include this repository's code in your project.

## How to use MultiPostParameterBinding
1) Download the code in the **Source** folder and add it to your Web API project or somewhere else in the same solution.
2) Use attribute **[MultiPostParameters]** on the action methods that need to support multiple POST parameters.
```
[HttpPost, MultiPostParameters]
public string DoSomething(CustomType param1, CustomType param2, string param3) { ... }
```
3) Clients pass the parameters as properties of an object. An example JSON object would be:
```
{ param1:{ Text: x }, param2:{ Text: y }, param3:z }
```
Example JQuery:
```
$.ajax({
	data: JSON.stringify({ param1:{ Text: x }, param2:{ Text: y }, param3:z }),
	url: '/MyService/DoSomething',
	contentType: "application/json", method: "POST", processData: false
})
.success(function (result) { ... });
```

## What is supported
- The method's parameters can be of any primitive or complex type, including your custom objects, and in any order.
- The method must allow HTTP POST or PUT.

## Acknowledgements
This code is based on Royi Namir's [SimplePostVariableParameterBindingExtended](https://github.com/RoyiNamir/SimplePostVariableParameterBindingExtended) which is in turn based on Rick Strahl's article [Passing multiple POST parameters to Web API Controller Methods](http://weblog.west-wind.com/posts/2012/May/08/Passing-multiple-POST-parameters-to-Web-API-Controller-Methods).
