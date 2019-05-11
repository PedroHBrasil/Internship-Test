using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternshipTest.Vehicle
{
    /// <summary>
    /// Contains the information of a one wheel model's aerodynamic map point.
    /// </summary>
    [Serializable]
    public class OneWheelAerodynamicMapPoint : AerodynamicMapPoint
    {
        #region Properties
        /// <summary>
        /// Distance between the ground and the car [m].
        /// </summary>
        public double RideHeight { get; set; }
        public double RideHeightDisplay {
            get { return RideHeight * 1000; }
            set { RideHeight = value / 1000; }
        }
        #endregion
        #region Contructors
        public OneWheelAerodynamicMapPoint() { }

        public OneWheelAerodynamicMapPoint(double windRelativeSpeed, double rideHeight, double dragCoefficient, double liftCoefficient)
        {
            WindRelativeSpeedDisplay = windRelativeSpeed;
            RideHeightDisplay = rideHeight;
            DragCoefficient = dragCoefficient;
            LiftCoefficient = liftCoefficient;
        }
        #endregion
    }
}
