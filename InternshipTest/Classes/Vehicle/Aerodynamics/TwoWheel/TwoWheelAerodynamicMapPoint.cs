using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternshipTest.Vehicle
{
    /// <summary>
    /// Contains the information of a two wheel vehicle model's aerodynamic map point.
    /// </summary>
    [Serializable]
    public class TwoWheelAerodynamicMapPoint : AerodynamicMapPoint
    {
        #region Properties
        /// <summary>
        /// Distance between the ground and the front of the car [m].
        /// </summary>
        public double FrontRideHeight { get; set; }
        public double FrontRideHeightDisplay {
            get { return FrontRideHeight * 1000; }
            set { FrontRideHeight = value / 1000; }
        }
        /// <summary>
        /// Distance between the ground and the rear of the car [m].
        /// </summary>
        public double RearRideHeight { get; set; }
        public double RearRideHeightDisplay {
            get { return RearRideHeight * 1000; }
            set { RearRideHeight = value / 1000; }
        }
        /// <summary>
        /// Aerodynamic pitch moment coefficient.
        /// </summary>
        public double DownforceDistribution { get; set; }
        public double DownforceDistributionDisplay {
            get { return DownforceDistribution * 100; }
            set { DownforceDistribution = value / 100; }
        }
        #endregion
        #region Contructors
        public TwoWheelAerodynamicMapPoint() { }

        public TwoWheelAerodynamicMapPoint(double frontRideHeight, double rearRideHeight, double dragCoefficient, double liftCoefficient, double downforceDistribution)
        {
            FrontRideHeightDisplay = frontRideHeight;
            RearRideHeightDisplay = rearRideHeight;
            DragCoefficient = dragCoefficient;
            LiftCoefficient = liftCoefficient;
            DownforceDistributionDisplay = downforceDistribution;
        }
        #endregion
    }
}
