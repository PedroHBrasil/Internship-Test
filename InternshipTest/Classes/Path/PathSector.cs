using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternshipTest
{
    /// <summary>
    /// Contains the information about a path sector.
    /// </summary>
    public class PathSector
    {
        #region Properties
        /// <summary>
        /// Path sector index.
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// Path sector start distance [m].
        /// </summary>
        public double StartDistance { get; set; }
        #endregion

        #region Constructors
        public PathSector() { }
        public PathSector(int index, double startDistance)
        {
            Index = index;
            StartDistance = startDistance;
        }
        #endregion
    }
}
