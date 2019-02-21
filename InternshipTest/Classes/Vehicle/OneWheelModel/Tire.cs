using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternshipTest.Vehicle.OneWheel
{
    /*
     * Contains the tire properties of a tire:
     *  - TireID: string which identifies the object;
     *  - TireModel: class which contains the MF5.2 tire model coefficients of the tire; and
     *  - VerticalStiffness: double which represents the tire's vertical stiffness in N/mm;
     */
    class Tire
    {
        // Properties -------------------------------------------------------------------
        public string TireID { get; set; }
        public TireModelMF52 TireModel { get; set; }
        public double VerticalStiffness { get; set; }

        // Constructors -----------------------------------------------------------------
        public Tire()
        {
            TireID = "Default";
            VerticalStiffness = 100; // N/mm
            TireModel = new TireModelMF52();
        }

        public Tire(string tireID, TireModelMF52 tireModel, double verticalStiffness)
        {
            TireID = tireID;
            TireModel = tireModel; // N/mm
            VerticalStiffness = Math.Abs(verticalStiffness);
        }

        // Methods ----------------------------------------------------------------------
        public override string ToString()
        {
            return TireID;
        }
    }
}
