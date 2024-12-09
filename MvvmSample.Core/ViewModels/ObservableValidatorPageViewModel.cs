﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Blazing.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using MvvmSample.Core.Services;

namespace MvvmSample.Core.ViewModels;

[ViewModelDefinition(Lifetime = ServiceLifetime.Transient)]
public partial class ObservableValidatorPageViewModel : SamplePageViewModel
{
    public ObservableValidatorPageViewModel(IFilesService filesService)
        : base(filesService)
    {
    }
}
