using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternshipTest.Vehicle
{
    /// <summary>
    /// Contains the information about a one wheel model's engine subsystem.
    /// </summary>
    [Serializable]
    public class Engine : GenericInfo
    {
        #region Properties
        /// <summary>
        /// Engine's curves (torque, braking torque and specific fuel consumption as functions of the rotational speed).
        /// </summary>
        public EngineCurves EngineCurves { get; set; }
        /// <summary>
        /// Ratio which represents the maximum avaliable throttle openning.
        /// </summary>
        public double MaxThrottle { get; set; }
        /// <summary>
        /// Density of the engine's fuel [kg/m³].
        /// </summary>
        public double FuelDensity { get; set; }
        #endregion
        #region Constructors
        public Engine() { }

        public Engine(string engineID, string description, EngineCurves engineCurves, double maxThrottle, double fuelDensity)
        {
            ID = engineID;
            Description = description;
            EngineCurves = engineCurves;
            MaxThrottle = Math.Abs(maxThrottle); // %
            FuelDensity = Math.Abs(fuelDensity); // kg/m^3
        }
        #endregion
        #region Methods
        public override string ToString()
        {
            return ID;
        }
        #endregion
    }
}
