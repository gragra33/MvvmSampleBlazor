﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Input;
using Blazing.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using MvvmSample.Core.Services;

namespace MvvmSample.Core.ViewModels;

[ViewModelDefinition(Lifetime = ServiceLifetime.Transient)]
public class RelayCommandPageViewModel : SamplePageViewModel
{
    public RelayCommandPageViewModel(IFilesService filesService) 
        : base(filesService)
    {
        IncrementCounterCommand = new RelayCommand(IncrementCounter);
    }

    /// <summary>
    /// Gets the <see cref="ICommand"/> responsible for incrementing <see cref="Counter"/>.
    /// </summary>
    public ICommand IncrementCounterCommand { get; }

    private int counter;

    /// <summary>
    /// Gets the current value of the counter.
    /// </summary>
    public int Counter
    {
        get => counter;
        private set => SetProperty(ref counter, value);
    }

    /// <summary>
    /// Increments <see cref="Counter"/>.
    /// </summary>
    private void IncrementCounter() => Counter++;

}
