# nullable-type-reflection

C# 8 introduces the concept of **non-nullable reference types** which can be
a big help in getting rid of code that throws `NullReferenceExcpetion`. While
compiling the code, the compiler generates warnings in a clever way, whenever
it detects a potential unwanted dereference of `null`.

## Example

```c#
string? x = null; // x is a nullable reference to string
string  y = "hi"; // y is a non-nullable reference to string
```

When accessing `x`, the compiler will make sure that you have tested that `x`
is not null befoere letting you call any of its methods, whereas it assumes
that `y` being declared as _non-nullable_ won't ever contain `null`, and thus
won't require any tests.

```c#
var len1 = x.Length; // warning CS8602: Possible dereference of a null reference.
var len2 = y.Length;

y = x; // warning CS8600: Converting null literal or possible null value to non-nullable type.
```

## After the Elvis operator comes... the dammit operator!

I've been using `?.` to conditionally access members of possibly null
references for some time now; this operator is fondly known as the _Elvis
operator_ (for obvious reasons).

Now comes the _dammit operator_, as Mads Torgerson coined it during one of
his talks at NDC London 2019: if you think you know what you are doing,
you can use `!` to silence the compiler. Call that method `!.` or assign
that value `!` &ndash; dammit.

```c#
var len1 = x!.Length; // no warning as I'm telling the compiler "I know what I do"
var len2 = y.Length;

y = x!; // no warning either
```

## What's going on under the hood?

I wanted to know if I could figure out by using reflection, if a property in a
class was being declared as `string` or as `string?`. Since code compiled with
the C# 8 compiler should still run on an unmodified CLR, I was on the lookout
for a compiler-generated attribute to represent that additional runtime-type
information. And indeed, if you open a DLL compiled with C# 8, you'll see that
now all usage of reference types gets decorated with a `NullableAttribute`.

```il
.custom instance void System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 01 00 00 )
```

Searching for `NullableAttribute` on GitHub didn't reveal any new class in
`dotnet` (which is no surprise, as this would mean that we'd have to use an
update version of .NET to get the assembly to resolve the attribute). The
only place where I found it, was inside of the Roslyn compiler.

The compiler is in fact emitting a minimal implementation of `NullableAttribute`
which has two empty constructors:

```c#
NullableAttribute(byte a) {}
NullableAttribute(byte[] a) {}
```

When using reflection (e.g. `GetProperties()` and `GetCustomAttributes()` I do
see the `NullableAttribute`, but I cannot reference the type itself; it is as
if the compiler did not know that it existed before it emitted the code. So in
order to play with it, I provided my own implementation:

```c#
namespace System.Runtime.CompilerServices
{
    [AttributeUsage (AttributeTargets.Class | AttributeTargets.Event | AttributeTargets.Field |
                     AttributeTargets.GenericParameter | AttributeTargets.Module | AttributeTargets.Parameter |
                     AttributeTargets.Property | AttributeTargets.ReturnValue,
                     AllowMultiple = false)]
    public class NullableAttribute : Attribute
    {
        public byte Mode { get; }

        public NullableAttribute(byte mode)
        {
            this.Mode = mode;
        }

        public NullableAttribute(byte[] _) => throw new System.NotImplementedException ();
    }
}
```

The compiler stopped emitting its own version and used mine. And now I could see
the difference between `string` and `string?`. In the first case, the constructor
of `NullableAttribute` is fed with `1`. And in the case of a nullable reference
type, it is fed with `2`.

I've put together [this project](https://github.com/epsitec/nullable-type-reflection)
to explore this in more depth. You'll see what happens when mixing reference types
and arrays, which can also be nullable or non-nullable.

## Project settings

For now, to get everything working properly, the `*.csproj` settings need to be
set like this:

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>netcoreapp3.0</TargetFramework>
    <LangVersion>8.0</LangVersion>
    <NullableContextOptions>enable</NullableContextOptions>
    ...
  </PropertyGroup>

</Project>
```
