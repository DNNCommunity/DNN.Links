using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetNuke.Modules.Links.Components
{
    /// <summary>
    /// Represents a link target
    /// </summary>
    public class TargetInfo
    {    
        #region Constructor
        public TargetInfo(string url)
        {
            this.Url = url;
        }
        #endregion        
        
        #region Public Properties
        public string Url { get; }

        public string Description { get; set; }

        public string Title { get; set; }
        #endregion
    }
}
