using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternshipTest.Vehicle
{
    public class AerodynamicMapPoint
    {
        #region Properties
        /// <summary>
        /// Wind's relative speed to the car (longitudinal) [m/s].
        /// </summary>
        public double WindRelativeSpeed { get; set; }
        public double WindRelativeSpeedDisplay {
            get { return WindRelativeSpeed * 3.6; }
            set { WindRelativeSpeed = value / 3.6; }
        }
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
