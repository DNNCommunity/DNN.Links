// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

namespace DotNetNuke.Modules.Links.Components
{
    using System;

    using DotNetNuke.ComponentModel.DataAnnotations;

    /// <summary>
    /// Represents a link.
    /// </summary>
    [TableName("Links")]
    [Serializable]
    [PrimaryKey("ItemId")]
    [Cacheable("Dnn_Links", System.Web.Caching.CacheItemPriority.Normal, 20)]
    public class Link
    {
        /// <summary>
        /// Gets or sets the item id.
        /// </summary>
        [ColumnName("ItemID")]
        public int ItemId { get; set; }

        /// <summary>
        /// Gets or sets the module id.
        /// </summary>
        [ColumnName("ModuleID")]
        public int ModuleId { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the view order.
        /// </summary>
        public int ViewOrder { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the id of the user that created the link.
        /// </summary>
        public int CreatedByUser { get; set; }

        /// <summary>
        /// Gets or sets the created date.
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the refresh interval.
        /// </summary>
        public int RefreshInterval { get; set; }

        /// <summary>
        /// Gets or sets the grant roles.
        /// </summary>
        public string GrantRoles { get; set; }

        /// <summary>
        /// Gets or sets the image URL.
        /// </summary>
        [IgnoreColumn]
        public string ImageURL { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to track clicks.
        /// </summary>
        [ReadOnlyColumn]
        public bool TrackClicks { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to log the activity.
        /// </summary>
        [ReadOnlyColumn]
        public bool LogActivity { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to open the link in a new window.
        /// </summary>
        [ReadOnlyColumn]
        public bool NewWindow { get; set; }

        /// <summary>
        /// Gets a value indicating whether to refresh the content.
        /// </summary>
        [IgnoreColumn]
        public bool RefreshContent
        {
            get
            {
                return this.RefreshInterval > 0;
            }
        }
    }
}
