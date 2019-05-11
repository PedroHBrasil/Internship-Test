using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternshipTest.Vehicle
{
    /// <summary>
    /// Contains the information about a one wheel model brakes subsystem.
    /// </summary>
    [Serializable]
    public class OneWheelBrakes : GenericInfo
    {
        #region Properties
        /// <summary>
        /// Maximum appliable torque by the brakes subsystem [N*m].
        /// </summary>
        public double MaximumTorque { get; set; }
        #endregion
        #region Constructors
        public OneWheelBrakes() { }

        public OneWheelBrakes(string brakesID, string description, double maxTorque)
        {
            ID = brakesID;
            Description = description;
            MaximumTorque = Math.Abs(maxTorque); // Nm
        }
        #endregion
    }
}
