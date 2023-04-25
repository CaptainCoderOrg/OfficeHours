# Intro to Delegates in C#

A **delegate** in C# can be thought of as a variable that refers to a method.
This allows you to use a method as a parameter. At first glance, this may not
seem all that useful. However, **delegates** can be used for very powerful
abstractions to help manage the complexity of your code.

## Polyglot Notebook

An interactive version of this document can be downloaded and run in VS Code
using the Polyglot Notebook extension.

Notebook: [LINK](Intro to Delegates in CSharp.dib)
Polyglot Notebook Extension: [LINK](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.dotnet-interactive-vscode)

## An Example

Imagine you have a method which makes a copy of the values in a list and increases each element by 1:

```csharp
// Creates a copy of the specified list with each value increased by 1
List<int> CopyElementsAndIncrement(List<int> elements)
{
    List<int> copy = new();
    foreach (int value in elements)
    {
        copy.Add(value + 1);
    }
    return copy;
}

List<int> original = new List<int>(){ 1, 2, 3, 4, 5 };
List<int> copy = CopyElementsAndIncrement(original);
Console.WriteLine(string.Join(", ", copy));
```

Next, imagine you now need to be able to make a copy of a list with each element doubled:

```csharp

// Creates a copy of the specified list with each value doubled
List<int> CopyElementsAndDouble(List<int> elements)
{
    List<int> copy = new();
    foreach (int value in elements)
    {
        copy.Add(value * 2);
    }
    return copy;
}

List<int> original = new List<int>(){ 1, 2, 3, 4, 5 };
List<int> copy = CopyElementsAndDouble(original);
Console.WriteLine(string.Join(", ", copy));
```

Notice anything interesting about these two methods?

They are **almost** identical. Anytime you find yourself copying / pasting code, it is often an indication of a code smell. In this case, the two methods only differ by one line:

```csharp
copy.Add(value + 1);
```

and 

```csharp
copy.Add(value * 2);
```

### Refactoring Using a Delegate

When you write two (or more) methods that are identical with only a minor difference in them, this is often a sign that a delegate could be used to help manage the complexity.

First, start by defining methods to replace the portion of code that we would like to generalize. In this case `value + 1` and `value * 2`. These become:

```csharp
int IncrementVal(int value) => value + 1;
int DoubleVal(int value) => value * 2;

Console.WriteLine(IncrementVal(5));
Console.WriteLine(DoubleVal(5));
```

Notice, these two methods share the same parameter list and return type. This is often referred to as the **method signature**. With this information, we can define a **delegate** type that we can use to refer to methods that have this **signature**:

```csharp
public delegate int IntCalculation(int value);
```

With this **delegate** defined, it is now possible to write a generalize version of our original method:

```csharp
// Creates a copy of the specified list applying the specified calculateion to each element
List<int> CopyAndModifyElements(List<int> elements, IntCalculation calculate)
{
    List<int> copy = new();
    foreach (int value in elements)
    {
        // Notice the following line uses the Invoke method to explicitly
        // call the delegate
        copy.Add(calculate.Invoke(value));
    }
    return copy;
}

List<int> original = new List<int>(){ 1, 2, 3, 4, 5 };
List<int> incrementedCopy = CopyAndModifyElements(original, IncrementVal);
Console.WriteLine(string.Join(", ", incrementedCopy));

List<int> doubledCopy = CopyAndModifyElements(original, DoubleVal);
Console.WriteLine(string.Join(", ", doubledCopy));
```

For ease of use and clarity, you might find it useful to write the two original methods in the following way:

```csharp
List<int> CopyElementsAndIncrement(List<int> elements) => CopyAndModifyElements(elements, IncrementVal);
List<int> CopyElementsAndDouble(List<int> elements) => CopyAndModifyElements(elements, DoubleVal);
```

## Multicast Delegate

The example above demonstrates how you can use a **delegate** to write a
flexible method with a "hole" that can be replaced with a call to a delegate
method. This alone is quite powerful.

However, it is also possible to compose delegates together to create a
**MulticastDelegate** that combines and chains two or more **delegate**s
together.

### Example: Composable User Display

Imagine you are making a game that has a slew of information available to the
player. However, not all of this information is relevant or necessary to the
player in each situation. Because of this, you would like to provide a feature
to the player that allows them to specify what information they see. You might
consider implementing such a feature using delegates:

```csharp
// The GameState which contains all of the information about the current level / player / state of the game.
public class GameState
{
    public int NumberOfEnemies { get; set; } = 5;
    public int Gold { get; set; } = 17;
    public double Time { get; set; } = 25.33;

    // ... more properties
}

public delegate void ReportOption(GameState state);
public void ReportEnemies(GameState state) => Console.WriteLine($"Number of Enemies: {state.NumberOfEnemies}");
public void ReportGold(GameState state) => Console.WriteLine($"Gold: {state.Gold}");
public void ReportTime(GameState state) => Console.WriteLine($"Time: {state.Time}");
```

With the above **delegates** specified, you might create a `ReportOption` **delegate** that simply reports the number of enemies to the player:

```csharp
GameState gameState = new GameState();
ReportOption playerReport = ReportEnemies; // + ReportGold;
playerReport.Invoke(gameState);
```

If at a later time, the player decides they would like to see the `Time` in their report, they might select an option in the game which updates the `playerReport` **delegate** in the following way:

```csharp
playerReport += ReportTime;
playerReport.Invoke(gameState);
```

Notice, in the example above, `playerReport.Invoke` now invokes both the `ReportEnemies` and `ReportTime` **delegates**.

Next, imagine the player wants to update their report such that it no longer displays the number of enemies but displays `Gold`. They might select an option that executes the following code:

```csharp
playerReport -= ReportEnemies;
playerReport += ReportGold;
playerReport.Invoke(gameState);
```

Some things to note:

* The **delegates** will be invoked in the order they are added
* You can add the same **delegate** more than once
* Removing a **delegate** that is not present in the MulticastDelegate is allowed and does nothing
* If the **delegate** has a return type, only the value of the final **delegate** is returned. Because of this, you should typically only use void **delegates** with **MulticastDelegates**

## Built in Delegates

C# provides several predefined **delegate** types that can and should be used in most simple situations: `Action`, `Func`, and `Predicate`.

### Action

The `Action` predicate defines a generic method signature that has a `void` return type. For example:

```csharp
void DoSomething()
{
    Console.WriteLine("Something!");
}

Action something = DoSomething;
something.Invoke();
```

If you need an `Action` that accepts a parameter, several **delegates** are available: `Action<T>`, `Action<T1, T2>`, `Action<T1, T2, T3>`, etc...

```csharp
void PrintInt(int x)
{
    Console.WriteLine($"Something: {x}");
}

Action<int> printInt = PrintInt;
printInt.Invoke(7);

void SumInts(int x, int y)
{
    Console.WriteLine($"Sum: {x + y}");
}

Action<int, int> sumExample = SumInts;
sumExample.Invoke(2, 3);
```

However, if you find yourself using an `Action` that accepts many parameters, it might be useful to define your own **delegate** for clarity.

### Func

The `Func` predicate **delegate** defines a generic method signature that returns a value. For example:

```csharp
int IncrementVal(int x) => x + 1;

Func<int, int> increment = IncrementVal;
Console.WriteLine(increment.Invoke(5));
```

Just like `Action` there are several variants of `Func`: `Func<T1, T2>`, `Func<T1, T2, T3>`, `Func<T1, T2, T3, T4>`, etc...

### Predicate

The `Predicate` **delegate** defines a method signature that returns a `bool`. For example:

```csharp
bool IsEven(int x) => x % 2 == 0;

Predicate<int> checkEven = IsEven;
Console.WriteLine(checkEven.Invoke(2));
Console.WriteLine(checkEven.Invoke(1));
```

Just like `Action` and `Func`, `Predicate` comes with several variants: `Predicate<T>`, `Predicate<T1, T2>`, `Predicate<T1, T2, T3>`, etc...