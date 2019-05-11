using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternshipTest.Vehicle
{
    /// <summary>
    /// Contains the information about a simplified suspension subsystem (single, front or rear).
    /// </summary>
    [Serializable]
    public class SimplifiedSuspension : GenericInfo
    {
        #region Properties
        /// <summary>
        /// Equivalent stiffness in heave of the car/axis [N/m].
        /// </summary>
        public double HeaveStiffness { get; set; }
        /// <summary>
        /// Distance between the ground and the car/axis (reference for the aerodynamic map) [m].
        /// </summary>
        public double RideHeight { get; set; }
        #endregion
        #region Constructors
        public SimplifiedSuspension() { }

        public SimplifiedSuspension(string suspensionID, string description, double heaveStiffness, double rideHeight)
        {
            ID = suspensionID;
            Description = description;
            HeaveStiffness = Math.Abs(heaveStiffness);
            RideHeight = Math.Abs(rideHeight);
        }
        #endregion
    }
}
