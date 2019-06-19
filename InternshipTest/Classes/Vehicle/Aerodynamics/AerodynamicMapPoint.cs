using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternshipTest.Vehicle
{
    [Serializable]
    public class AerodynamicMapPoint
    {
        #region Properties
        /// <summary>
        /// Aerodynamic drag coefficient.
        /// </summary>
        public double DragCoefficient { get; set; }
        /// <summary>
        /// Aerodynamic lift coefficient.
        /// </summary>
        public double LiftCoefficient { get; set; }
        #endregion
    }
}
