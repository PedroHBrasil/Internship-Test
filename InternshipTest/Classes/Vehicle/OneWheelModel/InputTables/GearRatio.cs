using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternshipTest.Vehicle.OneWheel
{/*
     * Contains a transmission's gear ratio and its gear number.
    */
    class GearRatio
    {
        // Properties ----------------------------------------------------------------
        public double Ratio { get; set; }

        // Constructors --------------------------------------------------------------
        public GearRatio()
        {
            Ratio = 1;
        }

        public GearRatio(double ratio)
        {
            Ratio = Math.Abs(ratio);
        }
    }
}
