using MathNet.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternshipTest.Simulation
{
    /// <summary>
    /// Contains a GGV diagram information.
    /// </summary>
    [Serializable]
    public class GGVDiagram : GenericInfo
    {
        #region Properties
        public int AmountOfPointsPerSpeed { get; set; }
        /// <summary>
        /// Amount of directions of the GGV diagram's accelerations.
        /// </summary>
        public int AmountOfDirections { get; set; }
        /// <summary>
        /// Amount of speeds of the GGV diagram.
        /// </summary>
        public int AmountOfSpeeds { get; set; }
        /// <summary>
        /// Lowest speed of the GG diagrams [m/s].
        /// </summary>
        public double LowestSpeed { get; set; }
        /// <summary>
        /// Highest speed of the GG diagrams [m/s].
        /// </summary>
        public double HighestSpeed { get; set; }
        /// <summary>
        /// Speeds of the GGV Diagram [m/s].
        /// </summary>
        public double[] Speeds { get; set; }
        #endregion

    }
}
