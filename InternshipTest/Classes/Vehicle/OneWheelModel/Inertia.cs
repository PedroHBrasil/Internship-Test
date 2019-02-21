using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternshipTest.Vehicle.OneWheel
{
    /*
     * Contains the inertia properties of a car:
     *  - InertiaID: string which identifies the object;
     *  - TotalMass: double which represents the vehicle's total mass in kg;
     *  - UnsprungMass: double which represents the vehicle's total unsprung mass in kg; and
     *  - RotPartsMI: double which represents the total moment of inertia of the vehicle's rotational parts.
     */
    class Inertia
    {
        // Properties ------------------------------------------------------------------------
        public string InertiaID { get; set; }
        public double TotalMass { get; set; }
        public double UnsprungMass { get; set; }
        public double RotPartsMI { get; set; }
        public double Gravity { get; set; }

        // Constructors ----------------------------------------------------------------------
        public Inertia()
        {
            InertiaID = "Default";
            TotalMass = 260; // kg
            UnsprungMass = 50; // kg
            RotPartsMI = 5; // kg*m^2
            Gravity = 10;   // m/s²
        }

        public Inertia(string inertiaID, double totalMass, double unsprungMass, double rotPartsMI, double gravity)
        {
            InertiaID = inertiaID;
            TotalMass = Math.Abs(totalMass); // kg
            UnsprungMass = Math.Abs(unsprungMass); // kg
            RotPartsMI = Math.Abs(rotPartsMI); // kg*m^2
            Gravity = Math.Abs(gravity);   // m/s²
        }
        // Methods ---------------------------------------------------------------------------
        public override string ToString()
        {
            return InertiaID;
        }
    }
}
