// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

namespace DotNetNuke.Modules.Links.Enums
{
    using System;

    /// <summary>
    /// The possible content types for the module.
    /// </summary>
    public enum ModuleContentTypes
    {
        /// <summary>
        /// Normal links mode (editors can add links).
        /// </summary>
        Links = 1,

        /// <summary>
        /// The menu mode displays the children of a page.
        /// </summary>
        Menu = 2,

        /// <summary>
        /// The folder mode displays the files of a folder.
        /// </summary>
        Folder = 3,
    }
}
