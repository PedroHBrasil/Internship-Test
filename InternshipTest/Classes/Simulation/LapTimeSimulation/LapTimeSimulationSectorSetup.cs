using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternshipTest.Simulation
{
    /// <summary>
    /// Contains the GGV diagram associated with a given path sector.
    /// </summary>
    public class LapTimeSimulationSectorSetup
    {
        #region Properties
        /// <summary>
        /// Sector's GGV diagram.
        /// </summary>
        public GGVDiagram SectorGGVDiagram { get; set; }
        /// <summary>
        /// Sector's index.
        /// </summary>
        public int SectorIndex { get; set; }
        #endregion
        #region Constructor
        public LapTimeSimulationSectorSetup(int index, GGVDiagram ggvDiagram)
        {
            SectorIndex = index;
            SectorGGVDiagram = ggvDiagram;
        }
        #endregion
    }

}
