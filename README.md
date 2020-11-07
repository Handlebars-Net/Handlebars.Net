# Handlebars.Net

#### [![CI](https://github.com/Handlebars-Net/Handlebars.Net/workflows/CI/badge.svg)](https://github.com/Handlebars-Net/Handlebars.Net/actions?query=workflow%3ACI) [![Nuget](https://img.shields.io/nuget/vpre/Handlebars.Net)](https://www.nuget.org/packages/Handlebars.Net/) [![performance](https://img.shields.io/badge/benchmark-statistics-blue)](http://handlebars-net.github.io/Handlebars.Net/dev/bench/)

---

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=Handlebars-Net_Handlebars.Net&metric=alert_status)](https://sonarcloud.io/dashboard?id=Handlebars-Net_Handlebars.Net) [![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=Handlebars-Net_Handlebars.Net&metric=reliability_rating)](https://sonarcloud.io/dashboard?id=Handlebars-Net_Handlebars.Net) [![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=Handlebars-Net_Handlebars.Net&metric=security_rating)](https://sonarcloud.io/dashboard?id=Handlebars-Net_Handlebars.Net)

[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=Handlebars-Net_Handlebars.Net&metric=bugs)](https://sonarcloud.io/dashboard?id=Handlebars-Net_Handlebars.Net) [![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=Handlebars-Net_Handlebars.Net&metric=code_smells)](https://sonarcloud.io/dashboard?id=Handlebars-Net_Handlebars.Net) [![Coverage](https://sonarcloud.io/api/project_badges/measure?project=Handlebars-Net_Handlebars.Net&metric=coverage)](https://sonarcloud.io/dashboard?id=Handlebars-Net_Handlebars.Net) 

---

**[Call for Input on v2](https://github.com/rexm/Handlebars.Net/issues/294)**

Blistering-fast [Handlebars.js templates](http://handlebarsjs.com) in your .NET application.

>Handlebars.js is an extension to the Mustache templating language created by Chris Wanstrath. Handlebars.js and Mustache are both logicless templating languages that keep the view and the code separated like we all know they should be.

Check out the [handlebars.js documentation](http://handlebarsjs.com) for how to write Handlebars templates.

Handlebars.Net doesn't use a scripting engine to run a Javascript library - it **compiles Handlebars templates directly to IL bytecode**. It also mimics the JS library's API as closely as possible.

## Install

    dotnet add package Handlebars.Net

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
Handlebars.RegisterHelper("link_to", (writer, context, parameters) => 
{
    writer.WriteSafeString($"<a href='{context["url"]}'>{context["text"]}</a>");
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
Handlebars.RegisterHelper("StringEqualityBlockHelper", (output, options, context, arguments) => 
{
    if (arguments.Length != 2)
    {
        throw new HandlebarsException("{{#StringEqualityBlockHelper}} helper must have exactly two arguments");
    }

    var left = arguments.At<string>(0);
    var right = arguments[1] as string;
    if (left == right) options.Template(output, context);
    else options.Inverse(output, context);
});

var animals = new Dictionary<string, string>() 
{
	{"Fluffy", "cat" },
	{"Fido", "dog" },
	{"Chewy", "hamster" }
};

var template = "{{#each this}}The animal, {{@key}}, {{#StringEqualityBlockHelper @value 'dog'}}is a dog{{else}}is not a dog{{/StringEqualityBlockHelper}}.\r\n{{/each}}";
var compiledTemplate = Handlebars.Compile(template);
string templateOutput = compiledTemplate(animals);

/* Would render
The animal, Fluffy, is not a dog.
The animal, Fido, is a dog.
The animal, Chewy, is not a dog.
*/
```

### Compatibility feature toggles

Compatibility feature toggles defines a set of settings responsible for controlling compilation/rendering behavior. Each of those settings would enable certain feature that would break compatibility with canonical Handlebars.
By default all toggles are set to `false`. 

##### Legend
- Areas
  - `Compile-time`: takes affect at the time of template compilation
  - `Runtime`: takes affect at the time of template rendering

#### `RelaxedHelperNaming`
If `true` enables support for Handlebars.Net helper naming rules.
This enables helper names to be not-valid Handlebars identifiers (e.g. `{{ one.two }}`).
Such naming is not supported in Handlebarsjs and would break compatibility.

##### Areas
- `Compile-time`

##### Example
```c#
[Fact]
public void HelperWithDotSeparatedName()
{
    var source = "{{ one.two }}";
    var handlebars = Handlebars.Create();
    handlebars.Configuration.Compatibility.RelaxedHelperNaming = true;
    handlebars.RegisterHelper("one.two", (context, arguments) => 42);

    var template = handlebars.Compile(source);
    var actual = template(null);
    
    Assert.Equal("42", actual);
}
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

## Future roadmap

TBD

## Contributing

Pull requests are welcome! The guidelines are pretty straightforward:
- Only add capabilities that are already in the Mustache / Handlebars specs
- Avoid dependencies outside of the .NET BCL
- Maintain cross-platform compatibility (.NET/Mono; Windows/OSX/Linux/etc)
- Follow the established code format
