using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternshipTest
{
    /// <summary>
    /// Contains a tabular path's sections data.
    /// </summary>
    public class TabularPathSectionsSet : GenericInfo
    {
        #region Properties
        /// <summary>
        /// Path's sections.
        /// </summary>
        public List<TabularPathSection> Sections { get; set; }
        #endregion
        #region Constructors
        public TabularPathSectionsSet() { }
        public TabularPathSectionsSet(string id, string description, List<TabularPathSection> sections)
        {
            ID = id;
            Description = description;
            Sections = sections;
        }
        #endregion
    }
}
