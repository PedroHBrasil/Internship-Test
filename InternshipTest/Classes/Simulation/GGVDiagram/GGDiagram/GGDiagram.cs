using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternshipTest.Simulation
{
    public class GGDiagram
    {
        #region Properties
        /// <summary>
        /// Speed at which the accelerations are calculated [m/s].
        /// </summary>
        public double Speed { get; set; }
        /// <summary>
        /// Amount of points of the GG diagram.
        /// </summary>
        public int AmountOfPoints { get; set; }
        /// <summary>
        /// Amount of directions of the GG diagram accelerations.
        /// </summary>
        public int AmountOfDirections { get; set; }
        /// <summary>
        /// Longitudinal accelerations [m/s²].
        /// </summary>
        public List<double> LongitudinalAccelerations { get; set; }
        /// <summary>
        /// Lateral accelerations [m/s²].
        /// </summary>
        public List<double> LateralAccelerations { get; set; }
        /// <summary>
        /// Curvatures [1/m].
        /// </summary>
        public List<double> Curvatures { get; set; }
        #endregion
        #region Methods
        /// <summary>
        /// Gets the curvatures associated with each point of the GG diagram.
        /// </summary>
        public void GetAssociatedCurvatures()
        {
            // Gets and registers the curvatures associated with each point of the GG diagram
            for (int iLateralAcceleration = 0; iLateralAcceleration < LateralAccelerations.Count; iLateralAcceleration++)
            {
                // Corrects the speed if it is equal to zero
                double currentSpeed = Speed;
                if (currentSpeed == 0) currentSpeed = 0.1;
                Curvatures.Add(LateralAccelerations[iLateralAcceleration] / Math.Pow(currentSpeed, 2));
            }
        }
        #endregion
    }
}
