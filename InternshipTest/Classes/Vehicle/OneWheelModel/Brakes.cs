using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternshipTest.Vehicle.OneWheel
{
    class Brakes
    {
        /*
         * Contains the brakes properties of a car:
         *  - BrakesID: string which identifies the object; and
         *  - MaxTorque: double which represents the maximum appliable torque allowed by the brake system [Nm].
         */
        // Properties ---------------------------------------------------------------
        public string BrakesID { get; set; }
        public double MaxTorque { get; set; }

        // Constructors -------------------------------------------------------------
        public Brakes()
        {
            BrakesID = "Default";
            MaxTorque = 1; // Nm
        }

        public Brakes(string brakesID, double maxTorque)
        {
            BrakesID = brakesID;
            MaxTorque = Math.Abs(maxTorque); // Nm
        }

        // Methods -----------------------------------------------------------------
        public override string ToString()
        {
            return BrakesID;
        }
    }
}
