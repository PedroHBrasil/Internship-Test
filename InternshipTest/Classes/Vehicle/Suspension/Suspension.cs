using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternshipTest.Vehicle
{
    /// <summary>
    /// Contains the information about a one wheel model's suspension subsystem.
    /// </summary>
    public class Suspension : GenericInfo
    {
        #region Properties
        /// <summary>
        /// Equivalent stiffness in heave of the car [N/m].
        /// </summary>
        public double HeaveStiffness { get; set; }
        /// <summary>
        /// Distance between the ground and the car (reference for the aerodynamic map) [m].
        /// </summary>
        public double RideHeight { get; set; }
        #endregion
        #region Constructors
        public Suspension() { }

        public Suspension(string suspensionID, string description, double heaveStiffness, double rideHeight)
        {
            ID = suspensionID;
            Description = description;
            HeaveStiffness = Math.Abs(heaveStiffness);
            RideHeight = Math.Abs(rideHeight);
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
