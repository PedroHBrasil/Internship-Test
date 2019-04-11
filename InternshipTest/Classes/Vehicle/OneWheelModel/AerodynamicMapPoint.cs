using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternshipTest.Vehicle.OneWheel
{
    /// <summary>
    /// Contains the information of a one wheel model's aerodynamic map point.
    /// </summary>
    public class AerodynamicMapPoint
    {
        #region Properties
        /// <summary>
        /// Wind's relative speed to the car (longitudinal) [m/s].
        /// </summary>
        public double WindRelativeSpeed { get; set; }
        public double WindRelativeSpeedDisplay 
        {
            get { return WindRelativeSpeed * 3.6; }
            set { WindRelativeSpeed = value / 3.6; }
        }
        /// <summary>
        /// Distance between the ground and the car [m].
        /// </summary>
        public double RideHeight { get; set; }
        public double RideHeightDisplay {
            get { return RideHeight * 1000; }
            set { RideHeight = value / 1000; }
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
        #region Contructors
        public AerodynamicMapPoint() { }

        public AerodynamicMapPoint(double windRelativeSpeed, double rideHeight, double dragCoefficient, double liftCoefficient)
        {
            WindRelativeSpeedDisplay = windRelativeSpeed;
            RideHeightDisplay = rideHeight;
            DragCoefficient = dragCoefficient;
            LiftCoefficient = liftCoefficient;
        }
        #endregion
    }
}
