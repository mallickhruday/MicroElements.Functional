# MicroElements.Functional
Small subset of functional methodology in C#

## Statuses
[![License](https://img.shields.io/github/license/micro-elements/MicroElements.Functional.svg)](https://raw.githubusercontent.com/micro-elements/MicroElements.Functional/master/LICENSE)
[![NuGetVersion](https://img.shields.io/nuget/v/MicroElements.Functional.svg)](https://www.nuget.org/packages/MicroElements.Functional)
![NuGetDownloads](https://img.shields.io/nuget/dt/MicroElements.Functional.svg)
[![MyGetVersion](https://img.shields.io/myget/micro-elements/v/MicroElements.Functional.svg)](https://www.myget.org/feed/micro-elements/package/nuget/MicroElements.Functional)

[![Travis](https://img.shields.io/travis/micro-elements/MicroElements.Functional/master.svg?logo=travis)](https://travis-ci.org/micro-elements/MicroElements.Functional)
[![AppVeyor](https://img.shields.io/appveyor/ci/micro-elements/microelements-functional.svg?logo=appveyor)](https://ci.appveyor.com/project/micro-elements/microelements-functional)
[![Coverage Status](https://img.shields.io/coveralls/micro-elements/MicroElements.Functional.svg)](https://coveralls.io/r/micro-elements/MicroElements.Functional)

[![Gitter](https://img.shields.io/gitter/room/micro-elements/MicroElements.Functional.svg)](https://gitter.im/micro-elements/MicroElements.Functional)

## Summary

MicroElements.Functional is another one try to use functional paradigm in C#. Functional approach can lead error elimination and more explicit logic.

Why I wrote one more lib? I tryed to use many monadic libs: louthy/language-ext, nlkl/Optional, la-yumba/functional-csharp-code, vkhorikov/CSharpFunctionalExtensions but there was some limitations or discomfort. LanguageExt is full featured but but it is very fat, Optional is very verbose and has only Option and Option with error, functional-csharp-code is cool but is a sort of book examples, all libs had not use new performance features of C#. And I decided to replace all functional libs with new... %)

### Main features

- Option
- Result<A, Error> (Either)
- Result<A, Error, Message>
- ValueObject
- Memoise
- Try monad
- Message and MessageList 
- Extensions for parsing, collections

## Installation

### Package Reference:

```
dotnet add package MicroElements.Functional
```

## Build
Windows: Run `build.ps1`

Linux: Run `build.sh`

## License
This project is licensed under the MIT license. See the [LICENSE] file for more info.

## Usage

Option is value that can be in one of two states: Some(a) or None.

### Creating Optional values
```csharp
// Create some option.
var someInt = Some(123);

// Implicit convert to option.
Option<string> name = "Bill";

// None.
Option<string> noneString = None;
Option<int> noneInt = None;

// Extension method to create some value.
var someString = "Sample".ToSome();
```
 ### Match
 ```csharp
// Match returns (123+1).
int plusOne = someInt.Match(i => i + 1, () => 0);
Console.WriteLine($"plusOne: {plusOne}");

// Match returns 0.
int nonePlusOne = noneInt.Match(i => i + 1, () => 0);
Console.WriteLine($"nonePlusOne: {nonePlusOne}");
 ```

### ValueObject
ValueObject is concept from DDD. see: https://martinfowler.com/bliki/ValueObject.html
You need two main principle to implement ValueObject: Structural Equality and Immutability.

To implement ValueObject you need:
- implement Object.Equals
- implement Object.GetHashCode
- implement Equality operator
- implement Inequality operator
- implement IEquatable interface (optional)
- implement IComparable interface (optional)

Or you can just inherit your class from MicroElements.Functional.ValueObject and override GetEqualityComponents method. 

MicroElements.Functional.ValueObject implements all routine for you. see Example.

#### Example
```csharp
    /// <summary>
    /// Address for delivery.
    /// </summary>
    [ImmutableObject(true)]
    public class Address : ValueObject
    {
        public string Country { get; }
        public string City { get; }
        public string State { get; }
        public string ZipCode { get; }

        public string StreetLine1 { get; }
        public string StreetLine2 { get; }

        public string FullName { get; }
        public string PhoneNumber { get; }

        public Address(string country, string city, string state, string zipCode, string streetLine1, string streetLine2, string fullName, string phoneNumber)
        {
            //TODO: Domain checks
            Country = country;
            City = city;
            State = state;
            ZipCode = zipCode;
            StreetLine1 = streetLine1;
            StreetLine2 = streetLine2;
            FullName = fullName;
            PhoneNumber = phoneNumber;
        }

        /// <inheritdoc />
        public override IEnumerable<object> GetEqualityComponents()
        {
            yield return Country;
            yield return City;
            yield return State;
            yield return ZipCode;
            yield return StreetLine1;
            yield return StreetLine2;
            yield return FullName;
            yield return PhoneNumber;
        }
    }
```

### Option
TODO

### Either
TODO

### Result
TODO

## Design Notes
- Most of types are structs so they can not be null
- Don't use monadic values for persistence because structs have default constructors and can be in uninitialized state
- Don't use monadic values in other data structures
- Main monad type is A, result type is B or Res

## Monad checklist
- Readonly struct
- Internal constructors, all factory checks in prelude
- Monadic methods: Match, Map, Bind(FlatMap), Filter
  - inputs should be not null
  - result types named Res
- LINQ operations: AsEnumerable, Select(Map), Where(Filter), SelectMany
- Equality methods
- Implicit conversions
- Intermonad conversions
- Uninitialized checks or bottom state?
- Check Monad laws
- Async interoparability or async version

## Monadic operations
```
- B Match(Func<A, B> map);
- Monad<B> Map(Func<A, B> map);
- Monad<B> Select(Func<A, B> select);
- Monad<B> Bind(Func<A, Monad<B>> bind)
- Monad<C> SelectMany<B, C>(
     Func<A, Monad<B>> bind,
     Func<A, B, C> project)
```

[LICENSE]: https://raw.githubusercontent.com/micro-elements/MicroElements.Functional/master/LICENSE
