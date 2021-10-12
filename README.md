# ⏳⌚ Chrono
A C# Time parser &amp; time event handler.

### Table of contents:
- [How to install](#how-to-install)
- [Applications](#applications)
- [Dependency injection](#dependency-injection)
- [Examples](#examples)

## 📩 How to install

Chrono is a drop and go assembly and can easily be installed from [here](https://github.com/Rozen4334/Chrono/releases/) or through NuGet.

## 👨‍💻 Applications

Chrono comes with 2 classes: 
- TimeParser
- TimeEvents
These classes are both non-static and need to be initialized by `new`. 
Uses for these classes namely appear in functions that require a timer or require the time to be retrieved from a string.

Made to make handling time easier, its specifically designed to make handling time in applications easier and avoiding additional timer setups.

## 💉 Dependency injections

Chrono classes work perfectly as services and can be injected through constructors. 
To assign events, a call to the service is required as the constructor will not assign anything until called.

### Example of implementing Chrono classes as services:
```cs
  public class Program
  {
      private readonly IServiceProvider _services;

      public Program()
          => _services = ConfigureServices();

      static void Main(string[] args)
          => new Program().RunAsync().GetAwaiter().GetResult();

      public async Task RunAsync()
      {
          // Whatever other code you may want to use.

          _services.GetRequiredService<MyService>().AssignEvents();

          // Never stop the program as it awaits a never-passing delay.
          await Task.Delay(Timeout.Infinite);
      }

      // Configure the serviceprovider and inject requested services by depending classes in the provider.
      private static IServiceProvider ConfigureServices()
          => new ServiceCollection()
              .AddSingleton(new TimeEvents(InvokeType.System))
              .AddSingleton<MyService>()
              .BuildServiceProvider();
  }
```
### The service assigned to handle these events:
```cs
  public class MyService
  {
      private readonly TimeEvents _timeHandler;

      // Constructs the class. The ConfigureService method recognizes the TimeEvents class while building and injects it by itself.
      public MyService(TimeEvents eventHandler)
          => _timeHandler = eventHandler;

      // Assigns the events to their receiver methods.
      public void AssignEvents()
      { 
          _timeHandler.OnSecondPassed += OnSecondReceived;
          _timeHandler.OnMinutePassed += OnMinuteReceived;
      }

      // Invoked each time the OnSecondPassed event is raised.
      private async Task OnSecondReceived(TimeEventArgs args)
      {
          Console.WriteLine(args.GlobalTime);

          // Whatever other code you want to execute on each second received.

          await Task.CompletedTask;
      }

      private async Task OnMinuteReceived(TimeEventArgs args)
      {
          // Send a fancy red time message for each minute passing.
          Console.ForegroundColor = ConsoleColor.Red;
          Console.WriteLine(args.GlobalTime);
          Console.ResetColor();

          await Task.CompletedTask;
      }
  }
```
#### This example makes use of:
- [Microsoft.Extensions.DependencyInjection](https://www.nuget.org/packages/Microsoft.Extensions.DependencyInjection/)
- [Microsoft.Extensions.DependencyInjection.Abstractions](https://www.nuget.org/packages/Microsoft.Extensions.DependencyInjection.Abstractions/)

## 📺 Examples

```cs
  public class ChronoExample
  {
      private readonly TimeEvents _timeHandler;
      private readonly TimeParser _timeParser;

      public ChronoExample()
      {
          // create new instances of both available chrono classes. There are no static fields in either of these classes.
          _timeHandler = new(InvokeType.Global);
          _timeParser = new();
      }

      // An example command coming in would be:
      // '/gettime 10 days, 5 hours and 16 seconds'
      public void HandleTimeInputExample(string command)
      {
          // remove the command name & prefix.
          command = command.Remove(0, 9);

          // GetFromString returns a bool, true if 'command' included any valid time, false if it did not.
          if (_timeParser.GetFromString(command, ParseOperator.Add, DateTime.UtcNow, out DateTime output))
          {
              // 'output' is the result of the given time in the GetFromString command with the time in 'command' added to it.
              Console.WriteLine(output);
          }
          else Console.WriteLine("No time found in '/gettime'. Are you sure you used the command properly?");
      }

      public void SubscribeToEvents()
      {
          _timeHandler.OnHourPassed += HourReceived;
      }

      private async Task HourReceived(TimeEventArgs arg)
      {
          // do whatever you want to handle here.
      }
  }
```
