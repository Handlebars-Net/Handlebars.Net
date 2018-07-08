Handlebars.Net [![Build Status](https://travis-ci.org/rexm/Handlebars.Net.svg?branch=master)](https://travis-ci.org/rexm/Handlebars.Net)
==============

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

## Performance

### Compilation

Compared to rendering, compiling is a fairly intensive process. While both are still measured in millseconds, compilation accounts for the most of that time by far. So, it is generally ideal to compile once and cache the resulting function to be re-used for the life of your process.

### Rendering
Nearly all time spent in rendering is in the routine that resolves values against the model. Different types of objects have different performance characteristics when used as models.

#### Model Types
- The absolute fastest model is a dictionary (microseconds), because no reflection is necessary at render time.
- The next fastest is a POCO (typically a few milliseconds for an average-sized template and model), which uses traditional reflection and is fairly fast.
- Rendering starts to get slower (into the tens of milliseconds or more) on dynamic objects.
- The slowest (up to hundreds of milliseconds or worse) tend to be objects with custom type implementations (such as `ICustomTypeDescriptor`) that are not optimized for heavy reflection.

A frequent performance issue that comes up is JSON.NET's `JObject`, which for reasons we haven't fully researched, has very slow reflection characteristics when used as a model in Handlebars.Net. A simple fix is to just use JSON.NET's built-in ability to deserialize a JSON string to an `ExpandoObject` instead of a `JObject`. This will yield nearly an order of magnitude improvement in render times on average.

## Future roadmap

- [ ] **Add unit tests!**
- [x] [Support for sub-expressions](https://github.com/rexm/Handlebars.Net/issues/48)
- [ ] `lookup`, `log`, and `helperMissing` helpers
- [x] [Support for whitespace control](https://github.com/rexm/Handlebars.Net/issues/52)
- [ ] MVC view engine
- [ ] Nancy view engine

## Contributing

Pull requests are welcome! The guidelines are pretty straightforward:
- Only add capabilities that are already in the Mustache / Handlebars specs
- Avoid dependencies outside of the .NET BCL
- Maintain cross-platform compatibility (.NET/Mono; Windows/OSX/Linux/etc)
- Follow the established code format
