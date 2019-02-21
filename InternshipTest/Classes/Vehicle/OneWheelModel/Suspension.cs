using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternshipTest.Vehicle.OneWheel
{
    /*
     * Contains the parametrs of a car's suspension.
     *  - SuspensionID: string which identifies the object;
     *  - HeaveStiffness: Equivalent stiffness of the suspension in heave (without tires) [N/mm]; and
     *  - CarHeight: When loaded only with the car's weight, the distance 
     *  between a reference point in the chassis and the ground [mm].
     */
    class Suspension
    {
        // Properties -------------------------------------------------------------------------
        public string SuspensionID { get; set; }
        public double HeaveStiffness { get; set; }
        public double CarHeight { get; set; }

        // Constructors -----------------------------------------------------------------------
        public Suspension()
        {
            SuspensionID = "Default";
            HeaveStiffness = 1; // N/mm
            CarHeight = 1; // mm
        }

        public Suspension(string suspensionID, double heaveStiffness, double carHeight)
        {
            SuspensionID = suspensionID;
            HeaveStiffness = heaveStiffness; // N/mm
            CarHeight = carHeight; // mm
        }
        // Methods ----------------------------------------------------------------------------
        public override string ToString()
        {
            return SuspensionID;
        }
    }
}
