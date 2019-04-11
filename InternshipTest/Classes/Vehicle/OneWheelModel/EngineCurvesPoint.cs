using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace InternshipTest.Vehicle.OneWheel
{
    /// <summary>
    /// Contains the information about a one wheel model's engie subsystem (torque, braking torque and specific fuel consumption as functions of the rotational speed).
    /// </summary>
    public class EngineCurvesPoint
    {
        #region Properties
        /// <summary>
        /// Engine's rotational speed [rad/s].
        /// </summary>
        public double RotationalSpeed { get; set; }
        public double RPM // Used in the UI display
        {
            get { return RotationalSpeed * 60 / (2 * Math.PI); }
            set { RotationalSpeed = value * 2 * Math.PI / 60; }
        }
        /// <summary>
        /// Engine's torque at a given rotational speed [N*m].
        /// </summary>
        public double Torque { get; set; }
        /// <summary>
        /// Engine's resistance torque at a given rotational speed [N*m].
        /// </summary>
        public double BrakingTorque { get; set; }
        /// <summary>
        /// Engine's specific fuel consumption at a given rotational speed [kg/W].
        /// </summary>
        public double SpecFuelCons { get; set; }
        public double SpecFuelConsDisplay
        {
            get { return SpecFuelCons * Math.Pow(10, 6); }
            set { SpecFuelCons = value * Math.Pow(10, -6); }
        }
        #endregion
        #region Constructors
        public EngineCurvesPoint() { }
        public EngineCurvesPoint(double rpm, double torque, double brakingTorque, double specificFuelConsumption)
        {
            RPM = Math.Abs(rpm);
            Torque = Math.Abs(torque);
            BrakingTorque = -Math.Abs(brakingTorque);
            SpecFuelConsDisplay = Math.Abs(specificFuelConsumption);
        }
        #endregion
    }
}
