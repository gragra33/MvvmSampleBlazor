﻿using Markdig;
using Microsoft.AspNetCore.Components;

namespace MvvmSampleBlazor;

public static class StringExtensions
{
    public static MarkupString MarkDownToMarkUp(this string markdown)
        => new(Markdown.ToHtml(markdown));
}