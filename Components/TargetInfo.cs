// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

namespace DotNetNuke.Modules.Links.Components
{
    /// <summary>
    /// Represents a link target.
    /// </summary>
    public class TargetInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TargetInfo"/> class.
        /// </summary>
        /// <param name="url">The URL.</param>
        public TargetInfo(string url)
        {
            this.Url = url;
        }

        /// <summary>
        /// Gets the link URL.
        /// </summary>
        public string Url { get; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string Title { get; set; }
    }
}
