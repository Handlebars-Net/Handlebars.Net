---
name: Bug report
about: Create a report to help us improve
title: ''
labels: bug
assignees: ''

---

### Describe the bug
A clear and concise description of what the bug is.

### Expected behavior:
A clear and concise description of what you expected to happen.

### Test to reproduce
```csharp
[Fact]
public void Descriptive_Test_Name_Here()
{
    var handlebars = Handlebars.Create();
    var render = handlebars.Compile("{{input}}");
    object data = new { input = 42 };
    
    var actual = render(data);
    Assert.Equal(42, actual);
}
```

### Other related info
Provide additional information if any.

---

### After submitting the issue
Please consider contributing to the project by submitting a PR with a fix to the issue. 
This would help to solve your problem in a shorter time as well as help other users of the project.

In case you do not know where to start - feel free to ask for help in the issue thread.

Building an active community is essential for any project survival as time of maintainers is limited.
