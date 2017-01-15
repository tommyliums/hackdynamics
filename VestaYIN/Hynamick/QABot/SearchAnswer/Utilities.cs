// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Utilities.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//     Defines the Utilities.cs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Hynamick.SearchAnswer
{
    using System;
    using System.IO;

    public sealed class Utilities
    {
        public static readonly string CurrentDirectory = AppDomain.CurrentDomain.RelativeSearchPath ??
                                                         AppDomain.CurrentDomain.BaseDirectory;

        public static string GetFilePath(string relativePath)
        {
            return Path.Combine(CurrentDirectory, relativePath);
        }
    }
}