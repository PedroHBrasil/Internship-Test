using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternshipTest
{
    /// <summary>
    /// Contains a path's sectors set.
    /// </summary>
    public class PathSectorsSet : GenericInfo
    {
        #region Properties
        /// <summary>
        /// Path sectors.
        /// </summary>
        public List<PathSector> Sectors { get; set; }
        #endregion
        #region Constructors
        public PathSectorsSet() { }
        public PathSectorsSet(string id, string description, List<PathSector> sectors)
        {
            ID = id;
            Description = description;
            Sectors = sectors;
        }
        #endregion
    }
}
