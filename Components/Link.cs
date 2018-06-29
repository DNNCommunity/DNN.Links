using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetNuke.Common.Utilities;
using DotNetNuke.ComponentModel.DataAnnotations;
using DotNetNuke.Entities.Modules;

namespace DotNetNuke.Modules.Links.Components
{
    /// <summary>
    /// Represents a link
    /// </summary>
    [TableName("Links")]
    [Serializable]
    [PrimaryKey("ItemId")]
    [Cacheable("Dnn_Links", System.Web.Caching.CacheItemPriority.Normal, 20)]
    public class Link
    {
        [ColumnName("ItemID")]
        public int ItemId { get; set; }

        [ColumnName("ModuleID")]
        public int ModuleId { get; set; }

        public string Title { get; set; }

        public string Url { get; set; }

        public int ViewOrder { get; set; }

        public string Description { get; set; }

        public int CreatedByUser { get; set; }

        public DateTime CreatedDate { get; set; }

        public int RefreshInterval { get; set; }

        public string GrantRoles { get; set; }

        [IgnoreColumn]
        public string ImageURL { get; set; }

        [ReadOnlyColumn]
        public bool TrackClicks { get; set; }

        [ReadOnlyColumn]
        public bool NewWindow { get; set; }        

        [IgnoreColumn]
        public bool RefreshContent
        {
            get
            {
                return RefreshInterval > 0;
            }   
        }        
    }
}
