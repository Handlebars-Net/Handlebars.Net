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
string partialSource =
@"<div id=""partial"">
  {{partial.content}}
</div>";

using (var reader = new StringReader(partialSource))
{
  var partialTemplate = Handlebars.Compile(reader);
  Handlebars.RegisterTemplate("partialName", partialTemplate);
}

string source =
@"<div id=""mainContainer"">
  {{mainContent}}
  <div id=""partialContainer"">
    {{>partialName}}  
  </div>
</div>";

var template = Handlebars.Compile(source);

var data = new {
  mainContent = "Main content",
  partial = new {
        content = "Partial content"
  }
};

var result = template(data);

/* Would render:
<div id="mainContainer">
  Main content
  <div id="partialContainer">
    <div id="partial">
      Partial content
    </div>  
  </div>
</div>
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

###Using Dictionary
```c#
var source = "<div style='width:{{clientSettings.width}}px'>User: {{userInfo.userName}} Language: {{userInfo.language}}</div>";

var template = Handlebars.Compile(source);

var embeded = new Dictionary<string, object>();
embeded.Add("userInfo", 
  new
  {
      userName = "Ondrej",
      language = "Slovak"
  });
embeded.Add("clientSettings",
  new
  {
      width = 120,
      height = 80
  });

var result = template(embeded);

/* Would render:
<div style='width:120px'>User: Ondrej Language: Slovak</div>
*/
```

##Future roadmap

- [ ] **Add unit tests!**
- [ ] `lookup`, `log`, and `helperMissing` helpers
- [ ] Set delimiters
- [ ] Mustache(5) Lambdas
- [ ] MVC view engine
- [ ] Nancy view engine

##Contributing

Pull requests are welcome! The guidelines are pretty straightforward:
- Only add capabilities that are already in the Mustache / Handlebars specs
- Avoid dependencies outside of the .NET BCL
- Follow the established code format
