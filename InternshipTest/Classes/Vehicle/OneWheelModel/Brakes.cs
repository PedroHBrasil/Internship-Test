using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternshipTest.Vehicle.OneWheel
{
    /// <summary>
    /// Containsthe information about a one wheel model brakes subsystem.
    /// </summary>
    public class Brakes : GenericInfo
    {
        #region Properties
        /// <summary>
        /// Maximum appliable torque by the brakes subsystem [N*m].
        /// </summary>
        public double MaxTorque { get; set; }
        #endregion
        #region Constructors
        public Brakes() { }

        public Brakes(string brakesID, string description, double maxTorque)
        {
            ID = brakesID;
            Description = description;
            MaxTorque = Math.Abs(maxTorque); // Nm
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
