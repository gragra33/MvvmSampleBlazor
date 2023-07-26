# Blazor MVVM Sample for CommunityToolkit.Mvvm using Blazing.Mvvm
I released the [Blazing.MVVM](https://github.com/gragra33/Blazing.Mvvm) library that enables Blazor to use the [CommunityToolkit.Mvvm](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/) with minimal effort. It also incldes Navigation by ViewModel (class/interface) - no more magic strings!

Whilst the [repo](https://github.com/gragra33/Blazing.Mvvm) contains a basic sample project showing how to use the library, I wanted to include a sample that takes an existing project for a different application type and, with minimal changes, make it work for Blazor. So I have taken Microsoft's [Xamarin Sample Project](https://github.com/CommunityToolkit/MVVM-Samples) and converted it to Blazor.

## Changes made to Xamarin Sample for Blazor 

The `MvvmSample.Core` project remains mostly unchanged, I've added base classes to the ViewModels to enable Blazor binding updates.

So, as an example, the `SamplePageViewModel` was changed from:
```csharp
public class MyPageViewModel : ObservableObject
{
    // code goes here
}
```
to:
```csharp
public class MyPageViewModel : ViewModelBase
{
    // code goes here
}
```
The `ViewModelBase` wraps the `ObservableObject` class. No other chages required.

For the Xamarin Pages, wiring up the `DataContext` is done with:
```csharp
BindingContext = Ioc.Default.GetRequiredService<MyPageViewModel>();
```
With [Blazing.MVVM](https://github.com/gragra33/Blazing.Mvvm), it is simply:
```html
@inherits MvvmComponentBase<MyPageViewModel>
```

Lastly, I have updated all of the documentation used in the Sample application from Xamarin-specific to Blazor. If I have missed any changes, please let me know and I will update.

## Components

Xamarin comes with a rich set of controls. Blazor is lean in comparison. To keep this project lean, I have included my own `ListBox` and `Tab` controls - enjoy! When I have time, I will endeavor to complete and release a control library for Blazor.

## WASM + New WPF Blazor Hybrid sample applications

I have added a new WPF Hybrid app to demonstrate Calling into Blazor from WPF using MVVM. To do this, I have:
* moved the core shared parts from the BlazorSample app to a new RCL (Razor Class Library)
* Moved the Assests to a standard `Content` folder as the `wwwroot` is no longer accessable. The `BlazorWebView` host control uses ip address `0.0.0.0` which is invalid for the `httpClient`.
* Added new `FileService` class to the WPF app to use the `File` class and not the `HttpClient`.
* Added  a new App.Razor to the WPF app for custom Blazor layout and hook the shared state for handling navigation requests from WPF

To enable the calling into the Blazor app, I have used a static state class to hold a reference to the `NavigationManager` and `MvvvmNavigationManager` classes.

## Summary

I have kept the code as close to the original solution as possible. Blazor does not have a rich data binding system, so there as small variations to the original code, however no huge changes made. Most of the work is handled by the [Blazing.MVVM](https://github.com/gragra33/Blazing.Mvvm) library.

Download the project, run it, and compare with the [original](https://github.com/CommunityToolkit/MVVM-Samples).

## Support

If you find this library useful, then please consider [buying me a coffee ☕](https://bmc.link/gragra33).