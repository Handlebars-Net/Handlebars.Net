# Handlebars Behavioral Specification

Derived from the Handlebars.js 4.x official documentation, the canonical JS test suite
(`handlebars-lang/handlebars.js/spec/`), and the Mustache spec. Each section describes
exact behaviors that can be independently tested.

---

## 1. Basic Expressions

### 1.1 Simple Property Lookup
```
Template: {{name}}
Data:     { name: "World" }
Output:   World
```

### 1.2 Missing Property Renders Empty
```
Template: {{missing}}
Data:     {}
Output:   (empty string)
```

### 1.3 Null Value Renders Empty
```
Template: {{val}}
Data:     { val: null }
Output:   (empty string — NOT "null")
```

### 1.4 Undefined Value Renders Empty
```
Template: {{val}}
Data:     { val: undefined }
Output:   (empty string — NOT "undefined")
```

### 1.5 false Renders as String "false"
```
Template: {{val}}
Data:     { val: false }
Output:   false
```

### 1.6 Zero Renders as String "0"
```
Template: {{val}}
Data:     { val: 0 }
Output:   0
```

### 1.7 Empty String Renders as Empty
```
Template: {{val}}
Data:     { val: "" }
Output:   (empty string)
```

### 1.8 Whitespace Inside Delimiters Is Ignored
```
Template: {{ name }}
Data:     { name: "World" }
Output:   World

Template: {{  name  }}
Data:     { name: "World" }
Output:   World
```

### 1.9 Property Whose Value Is a Function — Function Is Called
```
Template: {{greeting}}
Data:     { greeting: () => "Hello" }
Output:   Hello

Template: {{greeting}}
Data:     { greeting: function() { return this.name; }, name: "Rex" }
Output:   Rex
```

### 1.10 Undefined Root Context Renders Silently
```
Template: {{name}}
Data:     undefined (null root context)
Output:   (empty string — no error)
```

---

## 2. Nested Paths (Dot Notation)

### 2.1 Simple Nesting
```
Template: {{person.name}}
Data:     { person: { name: "Alice" } }
Output:   Alice
```

### 2.2 Deep Nesting
```
Template: {{a.b.c}}
Data:     { a: { b: { c: "deep" } } }
Output:   deep
```

### 2.3 Null Intermediate Renders Empty (No Error)
```
Template: {{person.name}}
Data:     { person: null }
Output:   (empty string)

Template: {{a.b.c}}
Data:     { a: null }
Output:   (empty string)
```

### 2.4 Numeric Index in Path
```
Template: {{list.0}}
Data:     { list: ["first", "second"] }
Output:   first

Template: {{list.1}}
Data:     { list: ["first", "second"] }
Output:   second
```

### 2.5 Forward-Slash Path Separator (Legacy)
```
Template: {{person/name}}
Data:     { person: { name: "Alice" } }
Output:   Alice
```

### 2.6 Hyphenated Identifiers
```
Template: {{foo-bar}}
Data:     { "foo-bar": "baz" }
Output:   baz
```

---

## 3. Segment-Literal Paths

### 3.1 Key With Spaces
```
Template: {{[foo bar]}}
Data:     { "foo bar": "value" }
Output:   value
```

### 3.2 Key With Dot
```
Template: {{[foo.bar]}}
Data:     { "foo.bar": "value" }
Output:   value
```

### 3.3 Nested With Segment Literal
```
Template: {{obj.[a b]}}
Data:     { obj: { "a b": "y" } }
Output:   y
```

### 3.4 Array Index via Bracket
```
Template: {{list.[0]}}
Data:     { list: ["first"] }
Output:   first

Template: {{list.[1]}}
Data:     { list: ["a", "b"] }
Output:   b
```

### 3.5 Segment Literal With Special Chars
```
Template: {{[foo.bar]}}
Data:     { "foo.bar": "x" }
Output:   x

Template: {{obj.[key/with/slash]}}
Data:     { obj: { "key/with/slash": "ok" } }
Output:   ok
```

### 3.6 Chained Segment Literals
```
Template: {{a.[b c].[d e]}}
Data:     { a: { "b c": { "d e": "found" } } }
Output:   found
```

### 3.7 Double-Quote String Literal Path (Bracket Notation Variant)
```
Template: {{"foo bar"}}
Data:     { "foo bar": "x" }
Output:   x
```

### 3.8 Key Starting With `[`
```
Template: {{[[startsWithBracket]}}
Data:     { "[startsWithBracket": "x" }
Output:   x
```

---

## 4. This / Self Reference

### 4.1 `this` and `.` Are Equivalent
```
Template: {{this}}
Data:     "hello"   (string as root context)
Output:   hello

Template: {{.}}
Data:     42
Output:   42
```

### 4.2 `this.property`
```
Template: {{this.name}}
Data:     { name: "Alice" }
Output:   Alice
```

### 4.3 `./property` Forces Property Lookup (Bypasses Helper)
```
Template: {{./name}}
Data:     { name: "Alice" }   (even if helper named "name" is registered)
Output:   Alice   (property wins)
```

### 4.4 Inside Block — `this` Is Current Iteration Context
```
Template: {{#each list}}{{this}}{{/each}}
Data:     { list: ["a", "b", "c"] }
Output:   abc
```

---

## 5. Parent Context Navigation (`../`)

### 5.1 Single Parent
```
Template: {{#with child}}{{../name}}{{/with}}
Data:     { name: "parent", child: {} }
Output:   parent
```

### 5.2 Two Levels Up
```
Template: {{#with a}}{{#with b}}{{../../top}}{{/with}}{{/with}}
Data:     { top: "X", a: { b: {} } }
Output:   X
```

### 5.3 Parent Reference Inside `#each`
```
Template: {{#each list}}{{../prefix}}-{{this}}{{/each}}
Data:     { prefix: "p", list: ["a","b"] }
Output:   p-ap-b
```

### 5.4 `../` With Data Variable: Parent Loop Index
```
Template: {{#each outer}}{{#each inner}}{{../@index}}{{/each}}{{/each}}
Data:     { outer: ["x","y"], inner: [1,2] }
Output:   (for each inner, prints the outer @index)
Expected: 0011   (outer index 0 twice, then 1 twice)
```
Note: `../@index` accesses the `@index` of the enclosing `#each`.

### 5.5 Deeply Nested Parent Index
See `IteratorTests.WithParentIndex` — three levels of `../` for `@index`, `@first`, `@last`.

---

## 6. HTML Escaping

### 6.1 Characters Escaped in `{{expr}}`

| Character | Escaped As |
|-----------|-----------|
| `&`       | `&amp;`   |
| `<`       | `&lt;`    |
| `>`       | `&gt;`    |
| `"`       | `&quot;`  |
| `'`       | `&#x27;`  |
| `` ` ``   | `&#x60;`  |
| `=`       | `&#x3D;`  |

```
Template: {{val}}
Data:     { val: "&<>\"'`=" }
Output:   &amp;&lt;&gt;&quot;&#x27;&#x60;&#x3D;
```

### 6.2 Individual Escape Cases
```
Template: {{val}}
Data:     { val: "<b>bold</b>" }
Output:   &lt;b&gt;bold&lt;/b&gt;

Template: {{val}}
Data:     { val: "a & b" }
Output:   a &amp; b

Template: {{val}}
Data:     { val: "\"quoted\"" }
Output:   &quot;quoted&quot;

Template: {{val}}
Data:     { val: "it's" }
Output:   it&#x27;s

Template: {{val}}
Data:     { val: "a=b" }
Output:   a&#x3D;b

Template: {{val}}
Data:     { val: "a`b" }
Output:   a&#x60;b
```

### 6.3 Safe String Bypasses Encoding
```
// If a helper returns a SafeString value, {{helper}} does not double-encode it.
Template: {{safeHelper}}
Helper returns: new Handlebars.SafeString("<b>bold</b>")
Output:   <b>bold</b>   (not encoded)
```

### 6.4 `{{{expr}}}` Triple-Stash — No Escaping
```
Template: {{{val}}}
Data:     { val: "<b>bold</b>" }
Output:   <b>bold</b>

Template: {{{val}}}
Data:     { val: "&\"'" }
Output:   &"'
```

### 6.5 `{{& expr}}` Ampersand — No Escaping
```
Template: {{& val}}
Data:     { val: "<b>bold</b>" }
Output:   <b>bold</b>

Template: {{&val}}
Data:     { val: "<b>" }
Output:   <b>
```

### 6.6 Non-String Values Are Coerced Then Rendered
```
Template: {{val}}
Data:     { val: 42 }     → Output: 42
Data:     { val: true }   → Output: true
Data:     { val: false }  → Output: false
```
Null and undefined → empty string (not "null"/"undefined").

---

## 7. Unescaped Output Equivalence
`{{{expr}}}` and `{{& expr}}` are identical in behavior. Both produce unescaped output.

---

## 8. Escaping Handlebars Delimiters

### 8.1 `\{{` Produces Literal `{{...}}`
```
Template: \{{name}}
Data:     { name: "Alice" }
Output:   {{name}}

Template: a \{{b}} c
Data:     {}
Output:   a {{b}} c
```

### 8.2 Escaped Then Non-Escaped
```
Template: \{{name}} {{name}}
Data:     { name: "Alice" }
Output:   {{name}} Alice
```

### 8.3 Backslash Before Backslash Before Expression
```
Template: \\{{name}}
Data:     { name: "Alice" }
Output:   \Alice         (one backslash, then the value — the \\ becomes \ and {{name}} is evaluated)
```

### 8.4 Data Value Containing `{{` Is Never Interpreted
```
Template: {{val}}
Data:     { val: "{{foo}}" }
Output:   {{foo}}   (curly braces in values are NOT HTML-escaped and NOT re-evaluated)
```

---

## 9. Comments

### 9.1 Inline Comment — Produces No Output
```
Template: a{{! comment }}b
Output:   ab

Template: {{! ignored }}hello
Output:   hello
```

### 9.2 Inline Comment — Closed by First `}}`
```
Template: a{{! foo }}rest
Output:   rest   (comment ends at first `}}`)
```

### 9.3 Block Comment — Can Contain `}}`
```
Template: a{{!-- foo }} bar --}}b
Output:   ab   (block comment swallows the `}}`)
```

### 9.4 Multiline Block Comment
```
Template: a{{!--
  multi
  line
--}}b
Output:   ab
```

### 9.5 Block Comment With Nested `{{...}}`
```
Template: a{{!-- has {{nested}} expressions --}}b
Output:   ab
```

### 9.6 Comment Does Not Strip Surrounding Whitespace (Without `~`)
```
Template: "a {{! c }} b"
Output:   "a  b"   (two spaces — one each side of the comment position)

Template: "a\n{{! c }}\nb"
Output:   "a\n\nb"   (blank line where comment was)
```

---

## 10. Whitespace Control

### 10.1 Strip Left with `{{~`
```
Template: "Hello, {{~name}} !"
Data:     { name: "World" }
Output:   "Hello,World !"
```

### 10.2 Strip Right with `~}}`
```
Template: "Hello, {{name~}} !"
Data:     { name: "World" }
Output:   "Hello, World!"
```

### 10.3 Strip Both with `{{~` and `~}}`
```
Template: "Hello, {{~name~}} !"
Data:     { name: "World" }
Output:   "Hello,World!"
```

### 10.4 Strips All Whitespace Including Newlines
```
Template: "1\n{{foo~}} \n\n 23\n{{bar}}4"
Data:     { foo: "A", bar: "B" }
Output:   "1\nA23\nB4"
```

### 10.5 Works on Block Helper Open/Close Tags
```
Template: "{{#if cond~}}\n  B\n{{~/if}}"
Data:     { cond: true }
Output:   "B"
```

### 10.6 Works on `{{else}}`
```
Template: "{{#if cond~}} A {{~else~}} B {{~/if}}"
Data (cond=true):  Output: "A"
Data (cond=false): Output: "B"
```

### 10.7 Works on Comments
```
Template: "a {{~! comment ~}} b"
Output:   "ab"

Template: "a {{~!-- block comment --~}} b"
Output:   "ab"
```

### 10.8 Works on Partials
```
Template: "foo {{~> dude~}} bar"
Partial dude: "baz"
Output:   "foobazbar"
```

### 10.9 Standalone Tags Strip Their Own Line
A tag that is the only non-whitespace content on a line will strip the entire line
(including the newline). This applies to block open/close tags, `{{else}}`, and
standalone comments/partials.
```
Template: "a\n  {{#if t}}\nb\n  {{/if}}\nc"
Data:     { t: true }
Output:   "a\nb\nc"
```

### 10.10 Standalone Comment Strips Its Line
```
Template: "a\n{{! comment }}\nb"
Output:   "a\nb"   (the entire comment line is removed)
```

---

## 11. `#if` Helper

### 11.1 Basic True
```
Template: {{#if val}}yes{{/if}}
Data:     { val: true }
Output:   yes
```

### 11.2 Basic False
```
Template: {{#if val}}yes{{/if}}
Data:     { val: false }
Output:   (empty)
```

### 11.3 With `{{else}}`
```
Template: {{#if val}}yes{{else}}no{{/if}}
Data (val=true):  Output: yes
Data (val=false): Output: no
```

### 11.4 `{{else if}}` Chain
```
Template: {{#if a}}A{{else if b}}B{{else}}C{{/if}}
Data (a=true):        Output: A
Data (b=true, a=false): Output: B
Data (neither):       Output: C
```

### 11.5 Falsy Values (All Render `{{else}}` / No Body)
```
Data values that are falsy:
- false
- null / undefined / missing property
- 0
- "" (empty string)
- [] (empty array)
- NaN (if applicable)

Template: {{#if val}}yes{{else}}no{{/if}}
{ val: false }     → no
{ val: null }      → no
{ val: 0 }         → no
{ val: "" }        → no
{ val: [] }        → no
(missing val)      → no
```

### 11.6 Truthy Values
```
- true
- non-zero numbers (including negative)
- non-empty string
- non-empty array
- any object
- function (function reference itself is truthy)

{ val: true }         → yes
{ val: 1 }            → yes
{ val: -0.1 }         → yes
{ val: "x" }          → yes
{ val: ["a"] }        → yes
{ val: { a: 1 } }     → yes
```

### 11.7 `includeZero=true` Hash Argument
```
Template: {{#if val includeZero=true}}yes{{else}}no{{/if}}
Data:     { val: 0 }
Output:   yes   (zero is treated as truthy when includeZero=true)
```

### 11.8 Function Value — Function Is Called
```
Template: {{#if val}}yes{{else}}no{{/if}}
Data:     { val: () => true }
Output:   yes

Data:     { val: () => false }
Output:   no
```

### 11.9 Non-Empty Array Is Truthy
```
Template: {{#if val}}yes{{else}}no{{/if}}
Data:     { val: ["a"] }
Output:   yes

Data:     { val: [] }
Output:   no
```

### 11.10 Nested `#if`
```
Template: {{#if a}}{{#if b}}both{{else}}only a{{/if}}{{else}}neither{{/if}}
Data (a=true, b=true):   both
Data (a=true, b=false):  only a
Data (a=false):          neither
```

---

## 12. `#unless` Helper

### 12.1 Basic — Renders When Value Is Falsy
```
Template: {{#unless val}}no{{/unless}}
Data (val=false):   Output: no
Data (val=true):    Output: (empty)
```

### 12.2 With `{{else}}`
```
Template: {{#unless val}}no{{else}}yes{{/unless}}
Data (val=false):  Output: no
Data (val=true):   Output: yes
```

### 12.3 Same Falsy Rules as `#if`
All values that are falsy for `#if` are truthy for `#unless` (i.e., render the main block).

---

## 13. `#each` Helper

### 13.1 Array Iteration — Basic
```
Template: {{#each list}}{{this}} {{/each}}
Data:     { list: ["a", "b", "c"] }
Output:   a b c 
```

### 13.2 Array Iteration — Object Items
```
Template: {{#each people}}{{name}} {{/each}}
Data:     { people: [{ name: "Alice" }, { name: "Bob" }] }
Output:   Alice Bob 
```

### 13.3 `@index` — Zero-Based Index
```
Template: {{#each list}}{{@index}}:{{this}} {{/each}}
Data:     { list: ["a", "b", "c"] }
Output:   0:a 1:b 2:c 
```

### 13.4 `@key` — Index as Key for Arrays, Property Name for Objects
```
Template: {{#each list}}{{@key}} {{/each}}
Data:     { list: ["a", "b"] }
Output:   0 1    (same as @index for arrays)
```

### 13.5 `@first` and `@last`
```
Template: {{#each list}}{{#if @first}}[{{/if}}{{this}}{{#if @last}}]{{/if}}{{/each}}
Data:     { list: ["a", "b", "c"] }
Output:   [abc]
```

### 13.6 Object Iteration — `@key` and `this`
```
Template: {{#each obj}}{{@key}}={{this}} {{/each}}
Data:     { obj: { a: 1, b: 2 } }
Output:   a=1 b=2   (order follows insertion order)
```

### 13.7 `{{else}}` — Empty Array
```
Template: {{#each list}}{{this}}{{else}}empty{{/each}}
Data:     { list: [] }
Output:   empty
```

### 13.8 `{{else}}` — Null / Undefined
```
Template: {{#each list}}{{this}}{{else}}empty{{/each}}
Data:     { list: null }       → Output: empty
Data:     (list missing)       → Output: empty
Data:     { list: false }      → Output: empty
```

### 13.9 No Argument — Throws
```
Template: {{#each}}{{this}}{{/each}}
Output:   Throws HandlebarsException "Must pass iterator to #each"
```

### 13.10 Block Params
```
Template: {{#each list as |item idx|}}{{idx}}:{{item}} {{/each}}
Data:     { list: ["a", "b"] }
Output:   0:a 1:b 
```

### 13.11 Block Params for Object Iteration
```
Template: {{#each obj as |val key|}}{{key}}={{val}} {{/each}}
Data:     { obj: { x: 1, y: 2 } }
Output:   x=1 y=2 
```

### 13.12 Nested `#each` — Parent Access
```
Template: {{#each outer}}{{#each inner}}{{../name}}-{{this}} {{/each}}{{/each}}
Data:     { outer: [{ name: "A", inner: [1,2] }, { name: "B", inner: [3] }] }
Output:   A-1 A-2 B-3 
```

### 13.13 Accessing `@root` Inside `#each`
```
Template: {{#each list}}{{@root.prefix}}-{{this}} {{/each}}
Data:     { prefix: "p", list: ["a","b"] }
Output:   p-a p-b 
```

### 13.14 `@key` on Object With HTML-Special Characters in Key
```
Template: {{#each obj}}{{@key}}={{this}} {{/each}}
Data:     { obj: { "<b>": "val" } }
Output:   &lt;b&gt;=val    (@key is HTML-encoded when rendered through {{@key}})
```

### 13.15 `@last` Is True Only on Final Item
```
Template: {{#each list}}{{this}}{{#unless @last}},{{/unless}}{{/each}}
Data:     { list: ["a","b","c"] }
Output:   a,b,c
```

### 13.16 `each` on Dictionary With Various Key Types
```
Template: {{#each dict}}{{@key}}:{{this}} {{/each}}
Data:     Dictionary<int, string>: { 1:"one", 2:"two" }
Output:   1:one 2:two 
```

### 13.17 `each` on Single-Element Array
```
Template: {{#each list}}{{@first}}/{{@last}}{{/each}}
Data:     { list: ["only"] }
Output:   True/True
```

---

## 14. `#with` Helper

### 14.1 Basic Context Change
```
Template: {{#with person}}{{first}} {{last}}{{/with}}
Data:     { person: { first: "Alan", last: "Johnson" } }
Output:   Alan Johnson
```

### 14.2 Access Parent via `../`
```
Template: {{#with foo}}{{#if goodbye}}{{../world}}{{/if}}{{/with}}
Data:     { foo: { goodbye: true }, world: "world" }
Output:   world
```

### 14.3 `{{else}}` — When Context Is Falsy
```
Template: {{#with val}}yes{{else}}no{{/with}}
Data (val=false):   Output: no
Data (val=null):    Output: no
Data (val=[]):      Output: no
(val missing):      Output: no
Data (val={}):      Output: yes   (empty object is truthy)
```

### 14.4 Block Params
```
Template: {{#with person as |p|}}{{p.name}}{{/with}}
Data:     { person: { name: "Alice" } }
Output:   Alice
```

### 14.5 Block Params — Alias Does Not Shadow Same-Named Context Properties
```
Template: {{#with person as |person|}}{{person.name}} is {{age}} years old{{/with}}
Data:     { person: { name: "Erik", age: 42 } }
Output:   Erik is 42 years old
Note: `age` resolves in the outer context (not inside person).
```

---

## 15. `{{lookup}}` Helper

### 15.1 Dynamic Array Lookup by Index
```
Template: {{#each people}}{{lookup ../cities @index}}{{/each}}
Data:     { people: ["Nils","Yehuda"], cities: ["Darmstadt","San Francisco"] }
Output:   DarmstadtSan Francisco
```

### 15.2 Dynamic Object Lookup by Key
```
Template: {{lookup obj key}}
Data:     { obj: { a: "found" }, key: "a" }
Output:   found
```

### 15.3 Undefined Key Renders Empty
```
Template: {{lookup obj key}}
Data:     { obj: { a: "val" }, key: "missing" }
Output:   (empty)
```

### 15.4 Lookup on Undefined Object Renders Empty
```
Template: {{lookup missing key}}
Data:     { key: "a" }
Output:   (empty)
```

### 15.5 As a Subexpression
```
Template: {{#with (lookup ../cities resident)~}}{{name}} ({{country}}){{/with}}
Data:     { resident: "darmstadt", cities: { darmstadt: { name: "Darmstadt", country: "Germany" } } }
Output:   Darmstadt (Germany)
```

### 15.6 Wrong Number of Arguments Throws
```
Template: {{lookup obj}}
Output:   Throws "{{lookup}} helper must have two or three arguments"
```

---

## 16. `{{log}}` Helper

### 16.1 Produces No Output
```
Template: before{{log "message"}}after
Output:   beforeafter
```

### 16.2 With Level
```
Template: {{log "warn" message}}
Output:   (empty — just logs)
```

---

## 17. Custom Helpers

### 17.1 Simple Value Helper
```
handlebars.RegisterHelper("greet", (context, args) => "Hello!");
Template: {{greet}}
Output:   Hello!
```

### 17.2 Helper With Arguments
```
Template: {{link url text}}
Helper writes: <a href='{args[0]}'>{args[1]}</a>
Output:   <a href='http://..'>Link Text</a>
```

### 17.3 Helper Output Is HTML-Encoded When Using `{{helper}}`
```
Template: {{badHelper}}
Helper writes directly (unsafe string): "<b>bold</b>"
Output:   &lt;b&gt;bold&lt;/b&gt;

Template: {{badHelper}}
Helper uses writer.WriteSafeString("<b>bold</b>")
Output:   <b>bold</b>
```

### 17.4 Hash Arguments
```
Template: {{myHelper key1="hello" key2=42}}
Helper accesses: options.hash["key1"] == "hello", options.hash["key2"] == 42
```

### 17.5 Block Helper — `options.fn` and `options.inverse`
```
Template: {{#myBlock}}yes{{else}}no{{/myBlock}}
Helper calls options.fn(context) when truthy, options.inverse(context) when not.
```

### 17.6 Block Helper with Arguments
```
Template: {{#myBlock "arg1" key=value}}body{{/myBlock}}
Helper receives: args[0] == "arg1", options.hash["key"] == value
```

### 17.7 Helper With Same Name as Context Property — Helper Wins
```
// If "name" is registered as a helper AND exists in context, the helper is called.
Template: {{name}}
Helper "name" registered.
Output:   (helper's return value)

// Use ./name to force property lookup:
Template: {{./name}}
Output:   (context property value)
```

### 17.8 Helper Late Binding — Registered After Compile
```
A helper registered after compile() is still invoked at render time.
```

### 17.9 Missing Helper — Resolves to Context Property or Empty
```
Template: {{unknown}}
Data:     { unknown: "found" }
Output:   found   (falls back to context property lookup)

Template: {{unknown}}
Data:     {}
Output:   (empty)
```

### 17.10 Block Helper With Custom `@data` Variables
```
A block helper can pass custom data to its block via options.data.
Children access it as @customVar.
```

### 17.11 Return Helper (Returns Value, Not Writer)
```
handlebars.RegisterHelper("getData", (context, args) => args[0]);
Template: {{getData "hello"}}
Output:   hello   (return value is rendered as string)
```

---

## 18. Literals as Helper Arguments

### 18.1 String Literals (Single and Double Quotes)
```
Template: {{echo "hello"}}
Helper: returns args[0]
Output:   hello

Template: {{echo 'world'}}
Output:   world

Template: {{echo "it's fine"}}
Output:   it's fine

Template: {{echo 'say "hi"'}}
Output:   say "hi"

Template: {{echo ""}}
Output:   (empty)
```

### 18.2 Number Literals
```
Template: {{echo 42}}
Arg type: number (integer), value: 42

Template: {{echo -1}}
Arg value: -1

Template: {{echo 3.14}}
Arg value: 3.14

Template: {{echo 0}}
Arg value: 0
```

### 18.3 Boolean Literals
```
Template: {{echo true}}
Arg type: boolean, value: true

Template: {{echo false}}
Arg value: false
```

### 18.4 Null Literal
```
Template: {{echo null}}
Arg value: null (not the string "null")
```

### 18.5 String Literal Containing Curly Braces
```
Template: {{echo '{{foo}}'}}
Output:   {{foo}}   (curly braces in string args are literal)
```

---

## 19. Subexpressions

### 19.1 Basic Subexpression
```
Template: {{outer (inner "arg")}}
inner("arg") is called first; its return value is passed to outer as an argument.
```

### 19.2 Nested Subexpressions
```
Template: {{a (b (c d))}}
c(d) is evaluated first, result → b(), result → a().
```

### 19.3 Subexpression as Hash Value
```
Template: {{helper key=(sub arg)}}
sub(arg) is evaluated; its result is passed as hash["key"] to helper.
```

### 19.4 Multiple Subexpressions
```
Template: {{helper (sub1 a) (sub2 b)}}
Both subexpressions evaluated independently; results passed as positional args.
```

### 19.5 Subexpression With Literals
```
Template: {{outer (inner "string" 42 true)}}
inner receives "string", 42, true as args.
```

### 19.6 Subexpression With No Args
```
Template: {{outer (inner)}}
inner() called with no args.
```

### 19.7 Subexpression Inside `#each` and `#with`
```
Template: {{#each (getData)}}{{this}}{{/each}}
getData returns an array; each iterates it.

Template: {{#with (lookup obj key)~}}{{name}}{{/with}}
lookup result used as context for with.
```

### 19.8 Subexpression Result Used as `#if` Argument
```
Template: {{#if (isReady status)}}yes{{/if}}
isReady(status) evaluated; result used as truthy/falsy test.
```

---

## 20. Partials

### 20.1 Basic Partial
```
Template: {{> myPartial}}
Partial "myPartial": "Hello from partial"
Output:   Hello from partial
```

### 20.2 Partial With Parent Context (Inherits by Default)
```
Template: {{#each people}}{{> person}}{{/each}}
Partial "person": {{name}}
Data:     { people: [{ name: "Alice" }] }
Output:   Alice
```

### 20.3 Partial With Explicit Context Argument
```
Template: {{> person data}}
Partial "person": {{name}}
Data:     { data: { name: "Bob" }, name: "Root" }
Output:   Bob   (data is used as context, not root)
```

### 20.4 Partial With Hash Parameters
```
Template: {{> person name="Charlie"}}
Partial "person": {{name}}
Output:   Charlie
(Hash params override/extend the current context for the partial.)
```

### 20.5 Dynamic Partial
```
Template: {{> (partialHelper)}}
Helper "partialHelper": returns "myPartial"
Partial "myPartial": "dynamic!"
Output:   dynamic!
```

### 20.6 Missing Partial Throws
```
Template: {{> missingPartial}}
Output:   Throws error about missing partial
```

### 20.7 Block Partial — Default Content
```
Template: {{#> myPartial}}default content{{/myPartial}}
If partial "myPartial" not registered: Output: default content
If partial "myPartial" registered and uses @partial-block: Output: (partial renders default)
```

### 20.8 Partial Referencing `{{> @partial-block}}`
```
Partial "wrapper": <div>{{> @partial-block}}</div>
Template: {{#> wrapper}}inner content{{/wrapper}}
Output:   <div>inner content</div>
```

### 20.9 Inline Partial (Defined in Template)
```
Template: {{#*inline "myPartial"}}Hello {{name}}!{{/inline}}{{> myPartial}}
Data:     { name: "World" }
Output:   Hello World!
```

### 20.10 Inline Partial Overrides Registered Partial
```
Template: {{#*inline "p"}}inline{{/inline}}{{> p}}
Registered partial "p": "registered"
Output:   inline   (inline takes priority)
```

### 20.11 Inline Partial Scope — Not Accessible Outside Block
```
Template: {{#if true}}{{#*inline "p"}}scoped{{/inline}}{{/if}}{{> p}}
Output:   Throws error (partial not found — "p" was scoped to the if block)
```

### 20.12 Partial Indentation Preserved
```
Template: "  {{> p}}"
Partial "p": "line1\nline2"
Output:   "  line1\n  line2"   (indentation applied to all lines)
```

### 20.13 Partial in `#each`
```
Template: {{#each people}}{{> person}}{{/each}}
Partial "person": {{name}},
Output:   Alice,Bob,
```

---

## 21. Block Params

### 21.1 `#each` With Block Params
```
Template: {{#each list as |item index|}}{{index}}:{{item}} {{/each}}
Data:     { list: ["a", "b"] }
Output:   0:a 1:b 
```

### 21.2 `#with` With Block Params
```
Template: {{#with person as |p|}}{{p.name}}{{/with}}
Data:     { person: { name: "Alice" } }
Output:   Alice
```

### 21.3 `#each` Object With Block Params (Value, Key Order)
```
Template: {{#each obj as |val key|}}{{key}}={{val}} {{/each}}
Data:     { obj: { a: 1, b: 2 } }
Output:   a=1 b=2 
```

### 21.4 Block Param Shadows Context Property
```
Template: {{#each list as |name|}}{{name}} {{/each}}{{name}}
Data:     { name: "outer", list: ["inner1", "inner2"] }
Output:   inner1 inner2 outer
```

---

## 22. Data Variables (`@`)

### 22.1 `@root` — Always the Top-Level Context
```
Template: {{#with person}}{{@root.title}}{{/with}}
Data:     { title: "T", person: { name: "Alice" } }
Output:   T
```

### 22.2 `@root` Inside Nested `#each`
```
Template: {{#each list}}{{@root.prefix}}-{{this}} {{/each}}
Data:     { prefix: "p", list: ["a","b"] }
Output:   p-a p-b 
```

### 22.3 Custom Global Data Passed at Render Time
```
Template: {{#with input}}{{first}} {{@global1}}{{/with}}
Data:     { input: { first: 1 } }
Extra:    { global1: 2 }   (passed as second arg to template function)
Output:   1 2
```

### 22.4 `@index` in Array Iteration
```
Template: {{#each list}}{{@index}}{{/each}}
Data:     { list: ["a","b","c"] }
Output:   012
```

### 22.5 `@key` in Object Iteration
```
Template: {{#each obj}}{{@key}}{{/each}}
Data:     { obj: { x:1, y:2 } }
Output:   xy
```

### 22.6 `@first` and `@last`
```
Template: {{#each list}}{{@first}},{{@last}} {{/each}}
Data:     { list: [1, 2, 3] }
Output:   True,False False,False False,True 
```

### 22.7 Parent `@index` via `../`
```
Template: {{#each outer}}{{#each inner}}{{../@index}}-{{@index}} {{/each}}{{/each}}
Data:     { outer: ["A","B"], inner: [1,2] }
Output:   0-0 0-1 1-0 1-1 
```

---

## 23. Inverted Sections

### 23.1 `{{^var}}` — Renders When Var Is Falsy
```
Template: {{^people}}No one{{/people}}
Data:     { people: [] }
Output:   No one

Data:     { people: false }
Output:   No one

Data:     (people missing)
Output:   No one
```

### 23.2 `{{^var}}` — Does Not Render When Truthy
```
Template: {{^people}}No one{{/people}}
Data:     { people: ["Alice"] }
Output:   (empty)
```

### 23.3 Inline `{{^}}` in Block Section
```
Template: {{#people}}{{name}}{{^}}No one{{/people}}
Data:     { people: [] }
Output:   No one

Data:     { people: [{ name: "Alice" }] }
Output:   Alice
```

### 23.4 `{{#var}}...{{else}}...{{/var}}` Is Equivalent to `{{#var}}...{{^}}...{{/var}}`
```
Template A: {{#val}}yes{{else}}no{{/val}}
Template B: {{#val}}yes{{^}}no{{/val}}
Both behave identically.
```

---

## 24. Decorators

### 24.1 Basic Decorator
```
handlebars.RegisterDecorator("myDec", (fn, props, container, options) => { ... })
Template: {{* myDec}}...
```

### 24.2 Inline Partial as Decorator
```
Template: {{#*inline "partialName"}}content{{/inline}}
This is syntactic sugar for registering a partial within the template scope.
```

### 24.3 Block Decorator
```
Template: {{#* myDec}}...{{/myDec}}
```

---

## 25. Interaction: `#if` + `@first`/`@last`

```
Template: {{#each list}}{{#if @first}}FIRST{{/if}}{{this}}{{#if @last}}LAST{{/if}}{{/each}}
Data:     { list: ["a","b","c"] }
Output:   FIRSTabc LAST   (effectively: "FIRSTa" + "b" + "cLAST" = "FIRSTabcLAST")
```

---

## 26. Interaction: `#each` + Partials + `../`

```
Template: {{#each people}}{{> person}}{{/each}}
Partial "person": {{name}} ({{../teamName}})
Data:     { teamName: "Engineering", people: [{ name: "Alice" }, { name: "Bob" }] }
Output:   Alice (Engineering)Bob (Engineering)
```

---

## 27. Interaction: Subexpressions in `#if`

```
Template: {{#if (eq a b)}}same{{else}}different{{/if}}
Helper "eq": (context, args) => args[0] === args[1]
Data:     { a: 1, b: 1 }
Output:   same

Data:     { a: 1, b: 2 }
Output:   different
```

---

## 28. Interaction: `#with` + `../` + `@root`

```
Template: {{#with person}}{{name}} / {{../teamName}} / {{@root.teamName}}{{/with}}
Data:     { teamName: "Eng", person: { name: "Alice" } }
Output:   Alice / Eng / Eng
```

---

## 29. Interaction: Nested `#each` + `@root` + `../@index`

```
Template: {{#each outer}}{{#each inner}}{{@root.title}}/{{../@index}}/{{@index}} {{/each}}{{/each}}
Data:     { title: "T", outer: ["A","B"], inner: [1,2] }
Output:   T/0/0 T/0/1 T/1/0 T/1/1 
```

---

## 30. Interaction: Inline Partial + `#each`

```
Template: {{#*inline "row"}}{{name}}{{/inline}}{{#each people}}{{> row}}{{/each}}
Data:     { people: [{ name: "Alice" }, { name: "Bob" }] }
Output:   AliceBob
```

---

## 31. Interaction: `#each` + Block Partial

```
Template: {{#each people}}{{#> row}}DEFAULT{{/row}}{{/each}}
Partial "row": {{name}}
Data:     { people: [{ name: "Alice" }] }
Output:   Alice   (partial found — default not used)
```

---

## 32. Edge Cases

### 32.1 Empty Template
```
Template: (empty string)
Output:   (empty string)
```

### 32.2 Template With No Expressions (Pure Text)
```
Template: Hello, world!
Output:   Hello, world!
```

### 32.3 Expression With Only Whitespace Inside
```
Template: {{   }}
Output:   Parse error / empty expression
```

### 32.4 Deeply Nested Null Chain
```
Template: {{a.b.c.d.e}}
Data:     { a: null }
Output:   (empty — no error at any level)
```

### 32.5 Block Helper With No Body
```
Template: {{#each list}}{{/each}}
Data:     { list: ["a","b"] }
Output:   (empty — body has no content)
```

### 32.6 Multiple Expressions Concatenated
```
Template: {{a}}{{b}}{{c}}
Data:     { a: "x", b: "y", c: "z" }
Output:   xyz
```

### 32.7 `{{#if}}` With Literal `true` / `false`
```
Template: {{#if true}}yes{{/if}}
Output:   yes

Template: {{#if false}}yes{{else}}no{{/if}}
Output:   no
```

### 32.8 `{{#if}}` With Literal Number
```
Template: {{#if 1}}yes{{/if}}
Output:   yes

Template: {{#if 0}}yes{{else}}no{{/if}}
Output:   no
```

### 32.9 `{{#each}}` Produces No Output for Empty Collections
```
Template: prefix{{#each list}}X{{/each}}suffix
Data:     { list: [] }
Output:   prefixsuffix
```

### 32.10 Helper Registered After Template Compiled (Late Binding)
```
// Compile first, register helper after — helper is still resolved at render time.
var template = hb.Compile("{{lateHelper}}");
hb.RegisterHelper("lateHelper", ...);
Output: (result of lateHelper)
```

### 32.11 Block Helper Conflicts With Context Property — Helper Wins
```
handlebars.RegisterHelper("blockName", ...)
Template: {{#blockName}}...{{/blockName}}
// blockName is the helper, not a property traversal.
```

### 32.12 Context Is Enumerable — Implicit Iteration Without `#each`
```
Template: {{#list}}{{this}} {{/list}}
Data:     { list: ["a", "b", "c"] }
Output:   a b c    ({{#list}} iterates if list is an array)
```

### 32.13 Block Section With Falsy Value — Uses `{{else}}`
```
Template: {{#val}}yes{{else}}no{{/val}}
Data:     { val: false }
Output:   no

Data:     { val: [] }
Output:   no
```

### 32.14 `{{#each}}` on Object With No Properties
```
Template: {{#each obj}}{{@key}}{{else}}empty{{/each}}
Data:     { obj: {} }
Output:   empty   (empty object triggers else — OR renders nothing depending on impl)
```

### 32.15 Partial With Empty Context
```
Partial "p": {{name}} welcome
Template: {{> p}}
Data:     {}   (name missing)
Output:    welcome   (empty before "welcome")
```

### 32.16 String Context — Access `.length`
```
Template: {{.}}{{length}}
Data:     "bye"   (string as root context)
Output:   bye3
```

### 32.17 Number Context
```
Template: {{this}}
Data:     42   (number as root context)
Output:   42
```

---

## Coverage Notes: What Handlebars.Net Already Tests Well

The following areas have solid existing coverage in Handlebars.Net's test suite:
- Basic paths, nested paths, null/missing handling
- HTML encoding (basic characters), triple stash
- `#if`/`#else`/`#else if` with typical values
- `#each` on arrays, objects, dictionaries (various key types)
- `@index`, `@key`, `@first`, `@last`
- `../` parent navigation and `../@index` deep nesting
- `@root` access
- `#with` basic and with block params
- Partials (basic, dynamic, block, inline)
- Subexpressions (basic, nested, hash values)
- Decorators
- Block params (`as |item index|`)
- Whitespace control (basic `~`, standalone tags)
- Numeric literals as args
- Comments (basic and block)
- `{{lookup}}` basic and as subexpression
- Escaped handlebars (`\{{`)
- Inverted sections (`{{^}}`)

## Known Implementation Gaps

These are confirmed behavioral differences between Handlebars.Net and the canonical Handlebars.js spec,
documented as of June 2026. Each gap has a corresponding regression test in `HandlebarsSpecCoverageTests.cs`
that asserts the *current* (non-spec-compliant) behavior so that changes in this area are caught.

---

### Gap 1: Default HTML encoder does not encode `'`, `` ` ``, or `=`

**Behavioral gap:**
Handlebars.js encodes seven characters for XSS safety: `& < > " ' `` =`.
Handlebars.Net's default encoder (`HtmlEncoderLegacy`) only encodes four: `& < > "`.
Single quotes, backticks, and equals signs pass through unescaped with the default configuration.

**Technical root cause:**
`HandlebarsConfiguration.cs` line 91 sets `TextEncoder = new HtmlEncoderLegacy()`.
`HtmlEncoderLegacy.cs` explicitly omits `'`, `` ` ``, and `=` — the class-level comment
documents this as intentional. The spec-compliant `HtmlEncoder` class exists in the library
and encodes all seven characters but must be opted into via
`new HandlebarsConfiguration { TextEncoder = new HtmlEncoder() }`.

**Backward-compatibility risk: HIGH.**
Changing the default encoder to `HtmlEncoder` would be a breaking change for any user whose
templates output `'`, `` ` ``, or `=` in HTML-escaped contexts and expect them to pass through
unescaped. This affects common patterns like `href='{{url}}'`. A new major version (or an
explicit opt-out escape hatch) would be required to change this default safely.

**Regression tests:** `HtmlEncoding_SingleQuote_DefaultEncoderGap`, `HtmlEncoding_Backtick_DefaultEncoderGap`,
`HtmlEncoding_Equals_DefaultEncoderGap`, `HtmlEncoding_AllSpecialCharsAtOnce_DefaultEncoderGap`
(assert current unencoded behavior); the `_FullEncoderSpec` variants verify the correct behavior
is available via opt-in.

---

### Gap 2: `{{log}}` built-in helper is not implemented

**Behavioral gap:**
Handlebars.js provides `{{log expr}}` as a built-in that passes its arguments to a platform logger
and produces no template output. In Handlebars.Net, `{{log ...}}` compiles without error but throws
`HandlebarsRuntimeException: Template references a helper that cannot be resolved. Helper 'log'`
at render time.

**Technical root cause:**
`BuildInHelpersFeature.cs` registers the built-in helpers. The `log` helper is absent from this
registration list. No default logger delegation or no-op fallback is provided.

**Backward-compatibility risk: LOW.**
Adding `log` as a registered no-op (or logger-delegating) helper is purely additive. Because
`{{log ...}}` currently throws at render time, no existing working code can be relying on this
behavior — any template containing `{{log}}` would be broken already.

**Regression tests:** `Log_ProducesNoOutput`, `Log_WithStringLiteralProducesNoOutput`
(assert that render throws `HandlebarsRuntimeException`).

---

### Gap 3: Boolean `false` renders as `"False"` (capital F) instead of `"false"` (lowercase)

**Behavioral gap:**
Handlebars.js renders `{{val}}` with `val = false` as the string `"false"` (lowercase), matching
JavaScript's boolean-to-string conversion. Handlebars.Net renders it as `"False"` (capital F)
because .NET's `Boolean.ToString()` returns `"False"`. The same applies to `@first` and `@last`
data variables, which render as `"True"`/`"False"` instead of `"true"`/`"false"`.
Note: `false` is still correctly falsy for `{{#if val}}` — only the string rendering is wrong.

**Technical root cause:**
The value-rendering pipeline uses the default .NET `ToString()` on `System.Boolean` without
special-casing to match JavaScript casing conventions. No post-processing lowercases boolean strings.

**Backward-compatibility risk: MEDIUM.**
Any code that renders boolean values via `{{expr}}` and checks the output string for `"False"`
(or `"True"`) would need updating. Any stored/displayed output containing these strings would
change. Scope is limited to template outputs of raw boolean values, but this is a subtle
cross-platform behavioral difference that could affect data contracts.

**Regression tests:** `Path_FalseRendersAsFalseString`, `RenderVsFalsy_FalseRendersButIsFalsyForIf`
(assert `"False"` as the current output).

---

### Gap 4: Whitespace control (tilde) is not applied around comment tokens

**Behavioral gap:**
Handlebars.js supports `{{~! comment ~}}` and `{{~!-- block comment --~}}` to strip surrounding
whitespace around comments, just as tilde works on other expression types.

In Handlebars.Net:
- `{{~! inline comment ~}}` compiles without error but does NOT strip surrounding whitespace.
  A template `"a {{~! comment ~}} b"` produces `"a b"` instead of `"ab"`.
- `{{~!-- block comment --~}}` is a **parse error**: `HandlebarsParserException: Reached end of
  template in the middle of a comment`. The `~!--` prefix disrupts the comment-start detection.

**Technical root cause:**
The whitespace-stripping pass does not handle comment expression node types — it strips whitespace
adjacent to expression nodes but comment nodes are not included in that set.
For block comments, the lexer matches the comment-start token on the literal string `{{!--` and
does not accept the `{{~!--` variant, causing the comment to be unterminated.

**Backward-compatibility risk: LOW.**
This is an unimplemented feature. No existing code can be relying on comments NOT stripping
whitespace, and no code using `{{~!--` can be working at all (it's a parse error). Adding correct
behavior is purely additive with zero breakage to existing working templates.

**Regression tests:** `WhitespaceControl_OnInlineComment` (asserts `"a b"`, not `"ab"`),
`WhitespaceControl_OnBlockComment` (asserts `HandlebarsParserException` at compile time).

---

### Gap 5: `{{^}}` (standalone caret) inside a block body is not an `else` separator

**Behavioral gap:**
Handlebars.js allows `{{^}}` (caret with no name) as an inline else equivalent inside any block:
`{{#val}}truthy{{^}}falsy{{/val}}` is semantically identical to
`{{#val}}truthy{{else}}falsy{{/val}}`.

In Handlebars.Net, `{{^}}` inside a block body is not treated as an else separator. Instead, it
is resolved as a path expression referencing the current context (equivalent to `{{this}}`), so:
- `val = true` → the truthy branch runs, then `{{^}}` outputs `context.ToString()`, then the
  "false-branch text" appears as literal body text: output is `"yes{ val = True }no"` (with
  the `=` potentially HTML-encoded depending on the active encoder).
- `val = false` → the block body is skipped entirely (falsy), and no inverse block is registered,
  so output is `""` rather than `"no"`.

The working alternative is `{{#val}}truthy{{else}}falsy{{/val}}` — `{{else}}` works correctly.

**Technical root cause:**
The block section accumulator does not recognize the empty `{{^}}` token (caret with no name) as
an inverse-block separator for non-`#each` block sections. The caret falls through to path
resolution, where it resolves to the current context object.

**Backward-compatibility risk: LOW.**
The `{{else}}` keyword is the canonical documented form and works correctly. `{{^}}` with no name
is an edge case that is rarely used and is arguably ambiguous in documentation. Fixing it would
only affect templates explicitly using the bare `{{^}}` syntax inside blocks, which currently
produce incorrect output anyway.

**Regression tests:** `InvertedSection_InlineCaretSyntax` (asserts that `val=true` output starts
with `"yes"`, ends with `"no"`, and is NOT equal to `"yes"`; `val=false` output is `""`).

---

## Coverage Gaps (Priority Areas for New Tests)

1. HTML encoding: `'`, `` ` ``, `=` characters specifically
2. `{{& expr}}` ampersand unescaped syntax
3. `\\\{{` (backslash before escaped expression → single backslash in output)
4. String context as root (`{{.}}` when root is a string)
5. Function-valued properties (called automatically)
6. `#if includeZero=true` hash argument
7. `#unless` with `{{else}}`
8. `#each` with no argument → should throw
9. `#each` on empty object → `{{else}}` behavior
10. `#each @key` HTML encoding when key has special chars
11. `#with {{else}}` for all falsy cases (null, false, empty array)
12. `{{log}}` produces no output
13. `{{#if true}}` / `{{#if false}}` with literal booleans
14. `{{#if 0}}` / `{{#if 1}}` with literal numbers
15. Block comment containing `}}` — `{{!-- has }} inside --}}`
16. Whitespace control on `{{else}}`, comments, and partials
17. `{{! comment }}` does not strip surrounding whitespace (without `~`)
18. Inline partial scope — not accessible outside defining block
19. Block partial with `{{> @partial-block}}` inside partial
20. Custom `@data` variables from helpers
21. `{{#with}}` when context is a function (function called for value)
22. `{{#each}}` when collection is a function (function called for value)
23. `../` navigation inside partials
24. Partial indentation behavior
25. `{{#if}}` with non-empty array is truthy; empty array is falsy
26. Standalone block tags strip entire line including newline
