Handlebars.Net [![Build Status](https://travis-ci.org/rexm/Handlebars.Net.svg?branch=master)](https://travis-ci.org/rexm/Handlebars.Net)
==============

Blistering-fast [Handlebars.js templates](http://handlebarsjs.com) in your .NET application.

>Handlebars.js is an extension to the Mustache templating language created by Chris Wanstrath. Handlebars.js and Mustache are both logicless templating languages that keep the view and the code separated like we all know they should be.

Check out the [handlebars.js documentation](http://handlebarsjs.com) for how to write Handlebars templates.

Handlebars.Net doesn't use a scripting engine to run a Javascript library - it **compiles Handlebars templates directly to IL bytecode**. It also mimics the JS library's API as closely as possible.

##Install

    nuget install Handlebars.Net

##Usage

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

###Registering Partials
```c#
string source =
@"<h2>Names</h2>
{{#names}}
  {{> user}}
{{/names}}";

string partialSource =
@"<strong>{{name}}</strong>";

using (var reader = new StringReader(partialSource))
{
  var partialTemplate = Handlebars.Compile(reader);
  Handlebars.RegisterTemplate("user", partialTemplate);
}

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

###Registering Helpers
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

###ASP MVC ViewEngine

    nuget install Handlebars.Net.Mvc.ViewEngine

There is a functioning example of an MVC project using Handlebars views - see Handlebars.Mvc.Example.csproj

To use it, you need the following code in global.asax.cs / Startup.cs or similar

```c#
      var handlebars = Handlebars.Create(new HandlebarsConfiguration
      {
          FileSystem = new HandlebarsMvcViewEngineFileSystem(),
      });
      ViewEngines.Engines.Add(new HandlebarsMvcViewEngine(handlebars));
```

This will expect your views to be in the /Views folder like so:

```
Views\layout.hbs                |<--shared as in \Views            
Views\partials\somepartial.hbs   <--shared as in  \Views\partials
Views\{Controller}\{Action}.hbs 
Views\{Controller}\{Action}\partials\somepartial.hbs 
```

##Future roadmap

- [ ] **Add unit tests!**
- [ ] [Support for sub-expressions](https://github.com/rexm/Handlebars.Net/issues/48)
- [ ] `lookup`, `log`, and `helperMissing` helpers
- [x] [Support for whitespace control](https://github.com/rexm/Handlebars.Net/issues/52)
- [ ] Set delimiters
- [ ] Mustache(5) Lambdas
- [x] MVC view engine
- [ ] Nancy view engine

##Contributing

Pull requests are welcome! The guidelines are pretty straightforward:
- Only add capabilities that are already in the Mustache / Handlebars specs
- Avoid dependencies outside of the .NET BCL
- Follow the established code format
