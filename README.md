[![Build status](https://ci.appveyor.com/api/projects/status/wwh7dl52o27sah9x?svg=true)](https://ci.appveyor.com/project/tom-englert/lazy-fody) [![NuGet Status](http://img.shields.io/nuget/v/Lazy.Fody.svg?style=flat-square)](https://www.nuget.org/packages/Lazy.Fody)

 ![Icon](Icon.png)

Automates the plumbing around System.Lazy

This is an add-in for [Fody](https://github.com/Fody/Fody/); it is available via [NuGet](https://www.nuget.org/packages/Lazy.Fody):

    PM> Install-Package Lazy.Fody
---
Your code
```c#
class Sample
{
    [Lazy(LazyThreadSafetyMode.ExecutionAndPublication)]
    public double Value => GetValue();

    private double GetValue()
    {
        return 3.14;
    }
}
```
What gets compiled:
```C#
class Sample
{
    private Lazy<double> Value_lazy;

    public Sample()
    {
        Value_lazy = new Lazy<double>(GetValue, LazyThreadSafetyMode.ExecutionAndPublication);
    }

    public double Value => Value_lazy.Value;

    private double GetValue()
    {
        return 3.14;
    }
}
```

---
### Icon

[Sloth](https://thenounproject.com/chanut-is/collection/zoo-welcome-to-zootopia/?i=1871708) by [Chanut is Industries](https://thenounproject.com/chanut-is/) from the Noun Project
