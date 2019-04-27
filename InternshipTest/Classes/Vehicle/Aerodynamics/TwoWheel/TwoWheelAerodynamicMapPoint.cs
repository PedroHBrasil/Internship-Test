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
        /// Vehicle slip angle [rad].
        /// </summary>
        public double CarSlipAngle { get; set; }
        public double CarSlipAngleDisplay {
            get { return CarSlipAngle * 180 / Math.PI; }
            set { CarSlipAngle = value * Math.PI / 180; }
        }
        /// <summary>
        /// Aerodynamic side force coefficient.
        /// </summary>
        public double SideForceCoefficient { get; set; }
        /// <summary>
        /// Aerodynamic pitch moment coefficient.
        /// </summary>
        public double PitchMomentCoefficient { get; set; }
        /// <summary>
        /// Aerodynamic yaw moment coefficient.
        /// </summary>
        public double YawMomentCoefficient { get; set; }
        #endregion
        #region Contructors
        public TwoWheelAerodynamicMapPoint() { }

        public TwoWheelAerodynamicMapPoint(double windRelativeSpeed, double frontRideHeight, double rearRideHeight, double carSlipAngle, double dragCoefficient, double sideForceCoefficient, double liftCoefficient, double pitchMomentCoefficient, double yawMomentCoefficient)
        {
            WindRelativeSpeedDisplay = windRelativeSpeed;
            FrontRideHeightDisplay = frontRideHeight;
            RearRideHeightDisplay = rearRideHeight;
            CarSlipAngleDisplay = carSlipAngle;
            DragCoefficient = dragCoefficient;
            SideForceCoefficient = sideForceCoefficient;
            LiftCoefficient = liftCoefficient;
            PitchMomentCoefficient = pitchMomentCoefficient;
            YawMomentCoefficient = yawMomentCoefficient;
        }
        #endregion
    }
}
