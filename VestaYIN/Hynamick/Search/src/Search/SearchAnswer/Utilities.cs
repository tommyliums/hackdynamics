// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Utilities.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//    Defines the Utilities.cs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Hynamick.SearchAnswer.Core
{
    using System.IO;

    public sealed class Utilities
    {
        public static string GetFilePath(string currentDirectory, string relativePath)
        {
            return Path.Combine(currentDirectory, relativePath);
        }
    }
}