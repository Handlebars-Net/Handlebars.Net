#handlebars.csharp  [![Nuget](https://img.shields.io/nuget/v/handlebars.csharp)](https://www.nuget.org/packages/handlebars.csharp/)

![Build](https://github.com/zjklee/Handlebars.CSharp/workflows/CI/badge.svg)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=zjklee_handlebars.csharp&metric=alert_status)](https://sonarcloud.io/dashboard?id=zjklee_handlebars.csharp)
[![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=zjklee_handlebars.csharp&metric=reliability_rating)](https://sonarcloud.io/dashboard?id=zjklee_handlebars.csharp)
[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=zjklee_handlebars.csharp&metric=bugs)](https://sonarcloud.io/dashboard?id=zjklee_handlebars.csharp)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=zjklee_handlebars.csharp&metric=vulnerabilities)](https://sonarcloud.io/dashboard?id=zjklee_handlebars.csharp)
 
==============

_This is a fork of [rexm/Handlebars.Net](https://github.com/rexm/Handlebars.Net) developed by @rexm. Unfortunately project had no activity for a while. I'd be glad to back-merge all the changes back to original repo if I'd have a chance. Meanwhile I'd try to support the fork._

Blistering-fast [Handlebars.js templates](http://handlebarsjs.com) in your .NET application.

>Handlebars.js is an extension to the Mustache templating language created by Chris Wanstrath. Handlebars.js and Mustache are both logicless templating languages that keep the view and the code separated like we all know they should be.

Check out the [handlebars.js documentation](http://handlebarsjs.com) for how to write Handlebars templates.

handlebars.csharp doesn't use a scripting engine to run a Javascript library - it **compiles Handlebars templates directly to IL bytecode**. It also mimics the JS library's API as closely as possible.

## Install

    dotnet add package handlebars.csharp

## Usage

```c#
string source =
@"<div class=""entry"">
  <h1>{{title}}</h1>
  <div class=""body"">
    {{body}}
  </div>
</div>";

var template = Handlebars.Compile(source);

var data = new {
    title = "My new post",
    body = "This is my first post!"
};

var result = template(data);

/* Would render:
<div class="entry">
  <h1>My New Post</h1>
  <div class="body">
    This is my first post!
  </div>
</div>
*/
```

### Registering Partials

```c#
string source =
@"<h2>Names</h2>
{{#names}}
  {{> user}}
{{/names}}";

string partialSource =
@"<strong>{{name}}</strong>";

Handlebars.RegisterTemplate("user", partialSource);

var template = Handlebars.Compile(source);

var data = new {
  names = new [] {
    new {
        name = "Karen"
    },
    new {
        name = "Jon"
    }
  }
};

var result = template(data);

/* Would render:
<h2>Names</h2>
  <strong>Karen</strong>
  <strong>Jon</strong>
*/
```

### Registering Helpers

```c#
Handlebars.RegisterHelper("link_to", (writer, context, parameters) => {
  writer.WriteSafeString("<a href='" + context.url + "'>" + context.text + "</a>");
});

string source = @"Click here: {{link_to}}";

var template = Handlebars.Compile(source);

var data = new {
    url = "https://github.com/rexm/handlebars.net",
    text = "Handlebars.Net"
};

var result = template(data);

/* Would render:
Click here: <a href='https://github.com/rexm/handlebars.net'>Handlebars.Net</a>
*/
```
 
This will expect your views to be in the /Views folder like so:

```
Views\layout.hbs                |<--shared as in \Views            
Views\partials\somepartial.hbs   <--shared as in  \Views\partials
Views\{Controller}\{Action}.hbs 
Views\{Controller}\{Action}\partials\somepartial.hbs 
```
### Registering Block Helpers

```c#
HandlebarsBlockHelper _stringEqualityBlockHelper = (TextWriter output, HelperOptions options, dynamic context, object[] arguments) => {
	if (arguments.Length != 2)
	{
		throw new HandlebarsException("{{StringEqualityBlockHelper}} helper must have exactly two argument");
	}
	string left = arguments[0] as string;
	string right = arguments[1] as string;
	if (left == right)
	{
		options.Template(output, null);
	}
	else
	{
		options.Inverse(output, null);
	}
};
Handlebars.RegisterHelper("StringEqualityBlockHelper", _stringEqualityBlockHelper);
Dictionary<string, string> animals = new Dictionary<string, string>() {
	{"Fluffy", "cat" },
	{"Fido", "dog" },
	{"Chewy", "hamster" }
};
string template = "{{#each @value}}The animal, {{@key}}, {{StringEqualityBlockHelper @value 'dog'}}is a dog{{else}}is not a dog{{/StringEqualityBlockHelper}}.\r\n{{/each}}";
Func<object, string> compiledTemplate = Handlebars.Compile(template);
string templateOutput = compiledTemplate(animals);

/* Would render
The animal, Fluffy, is not a dog.
The animal, Fido, is a dog.
The animal, Chewy, is not a dog.
*/
```
## Performance

### Compilation

Compared to rendering, compiling is a fairly intensive process. While both are still measured in millseconds, compilation accounts for the most of that time by far. So, it is generally ideal to compile once and cache the resulting function to be re-used for the life of your process.

### Rendering
Nearly all time spent in rendering is in the routine that resolves values against the model. Different types of objects have different performance characteristics when used as models.

#### Model Types
- The absolute fastest model is a `IDictionary<string, object>` (microseconds).
- The next fastest is a POCO (typically a few milliseconds for an average-sized template and model), which uses traditional reflection and is fairly fast.
- Rendering starts to get slower (into the tens of milliseconds or more) on dynamic objects.
- The slowest (up to hundreds of milliseconds or worse) tend to be objects with custom type implementations (such as `ICustomTypeDescriptor`) that are not optimized for heavy reflection.

~~A frequent performance issue that comes up is JSON.NET's `JObject`, which for reasons we haven't fully researched, has very slow reflection characteristics when used as a model in Handlebars.Net. A simple fix is to just use JSON.NET's built-in ability to deserialize a JSON string to an `ExpandoObject` instead of a `JObject`. This will yield nearly an order of magnitude improvement in render times on average.~~

### More
For mo retails see [Performance measurements](Performance.md)

## Future roadmap

TBD

## Contributing

Pull requests are welcome! The guidelines are pretty straightforward:
- Only add capabilities that are already in the Mustache / Handlebars specs
- Avoid dependencies outside of the .NET BCL
- Maintain cross-platform compatibility (.NET/Mono; Windows/OSX/Linux/etc)
- Follow the established code format
