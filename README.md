Handlebars.Net [![Build Status](https://travis-ci.org/rexm/Handlebars.Net.svg?branch=master)](https://travis-ci.org/rexm/Handlebars.Net)
==============

Blistering-fast, native [Handlebars.js templates](http://handlebarsjs.com) in your .NET application.

>Handlebars.js is an extension to the Mustache templating language created by Chris Wanstrath. Handlebars.js and Mustache are both logicless templating languages that keep the view and the code separated like we all know they should be.

Check out the [handlebars.js documentation](http://handlebarsjs.com) for how to write Handlebars templates.

Handlebars.Net doesn't use a scripting engine to run the Javascript library - it **compiles Handlebars templates directly to native .NET code**. It also mimics the JS library's API as closely as possible.

_Handlebars.Net is in alpha right now - check out the [todo](#todo) to see what's missing._

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

###Registering Helpers
```c#
Handlebars.RegisterHelper("link_to", (writer, parameters) => {
  writer.Write("<a href='" + parameters[0] + "'>" + parameters[1] + "</a>");
});

string source = @"Click here: {{link_to url text}}";

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


##Todo
_Release Candidate prerequisites_:
- [x] Support for block helpers
- [ ] Give helpers access to current context
- [x] `if` / `else` / `unless` helper
- [x] `with` helper
- [x] `each` / `else` helper
- [x] Escape everything by default
- [ ] HTML escaping expressions ("triple-stash" `{{{ }}}`)
- [x] Object enumeration
- [x] `@key`, `@index`, `@first`, `@last` context variables
- [ ] Implement `>` partials

_Future roadmap_:
- [ ] **Add unit tests!**
- [ ] MVC view engine
- [ ] Nancy view engine
