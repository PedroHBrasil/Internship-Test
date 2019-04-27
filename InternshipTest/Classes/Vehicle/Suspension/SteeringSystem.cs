using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternshipTest.Vehicle
{
    /// <summary>
    /// Contains the information about a steering subsystem of one axis (front or rear).
    /// </summary>
    public class SteeringSystem : GenericInfo
    {
        #region Properties
        /// <summary>
        /// Ratio between the steering wheel angle and the wheel steer angle [(wheel steer angle)/(steering wheel angle)].
        /// </summary>
        public double SteeringRatio { get; set; }
        /// <summary>
        /// Maximum steering wheel angle [rad].
        /// </summary>
        public double MaximumSteeringWheelAngle { get; set; }
        #endregion
        #region Constructors
        public SteeringSystem() { }
        public SteeringSystem(string suspensionID, string description, double steeringRatio, double maximumSteeringWheelAngle)
        {
            ID = suspensionID;
            Description = description;
            SteeringRatio = steeringRatio;
            MaximumSteeringWheelAngle = Math.Abs(maximumSteeringWheelAngle);
        }
        #endregion
    }
}
