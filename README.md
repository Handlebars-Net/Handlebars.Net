Handlebars.Net
==============

Blistering-fast [Handlebars.js templates](http://handlebars.js) in your .NET application.

>Handlebars.js is an extension to the Mustache templating language created by Chris Wanstrath. Handlebars.js and Mustache are both logicless templating languages that keep the view and the code separated like we all know they should be.

Check out the handlebars.js documentation for how to write Handlebars templates.

##Usage

```c#
string source = @"<div class=""entry"">
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
- [x] Support for block helpers
- [ ] Give helpers access to current context
- [ ] `if` / `else` / `unless` helper
- [ ] `with` helper
- [ ] Escape everything by default
- [ ] HTML escaping expressions (triple-stash, {{{ }}})
- [ ] Object enumeration
- [ ] @key, @index, @first, @last context variables
- [ ] **Add unit tests!**
