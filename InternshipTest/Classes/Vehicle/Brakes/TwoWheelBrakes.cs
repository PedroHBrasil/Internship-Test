using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternshipTest.Vehicle
{
    /// <summary>
    /// Contains the information about a two wheel model brakes subsystem.
    /// </summary>
    [Serializable]
    public class TwoWheelBrakes : GenericInfo
    {
        #region Properties
        /// <summary>
        /// Share of total brake pressure that gets to the front wheel [ratio] (% front).
        /// </summary>
        public double BrakeBias { get; set; }
        /// <summary>
        /// Maximum appliable torque by the brakes subsystem at the front wheel [N*m].
        /// </summary>
        public double FrontMaximumTorque { get; set; }
        /// <summary>
        /// Maximum appliable torque by the brakes subsystem at the rear wheel [N*m].
        /// </summary>
        public double RearMaximumTorque { get; set; }
        #endregion
        #region Constructors
        public TwoWheelBrakes() { }
        public TwoWheelBrakes(string id, string description, double brakeBias, double frontMaximumTorque, double rearMaximumTorque)
        {
            ID = id;
            Description = description;
            BrakeBias = Math.Abs(brakeBias);
            FrontMaximumTorque = -Math.Abs(frontMaximumTorque);
            RearMaximumTorque = -Math.Abs(rearMaximumTorque);
        }
        #endregion
    }
}
