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
    [Serializable]
    public class SteeringSystem : GenericInfo
    {
        #region Properties
        /// <summary>
        /// Ratio between the steering wheel angle and the front wheel steer angle [(wheel steer angle)/(steering wheel angle)].
        /// </summary>
        public double FrontSteeringRatio { get; set; }
        /// <summary>
        /// Ratio between the steering wheel angle and the rear wheel steer angle [(wheel steer angle)/(steering wheel angle)].
        /// </summary>
        public double RearSteeringRatio { get; set; }
        /// <summary>
        /// Maximum steering wheel angle [rad].
        /// </summary>
        public double MaximumSteeringWheelAngle { get; set; }
        #endregion
        #region Constructors
        public SteeringSystem() { }
        public SteeringSystem(string suspensionID, string description, double frontSteeringRatio, double rearSteeringRatio, double maximumSteeringWheelAngle)
        {
            ID = suspensionID;
            Description = description;
            FrontSteeringRatio = frontSteeringRatio;
            RearSteeringRatio = rearSteeringRatio;
            MaximumSteeringWheelAngle = Math.Abs(maximumSteeringWheelAngle);
        }
        #endregion
    }
}
