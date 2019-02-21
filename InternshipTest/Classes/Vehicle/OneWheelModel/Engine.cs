using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternshipTest.Vehicle.OneWheel
{
    /*
     * Contains the inertia properties of a car:
     *  - EngineID: string which identifies the variable;
     *  - EngineCurves: list which contains the torque, braking torque and specific consumption curves;
     *  - MaxThrottle: double which represents the vehicle's maximum throttle opening(torque curve scalling factor); and
     *  - FuelDensity: double which represents the vehicle's fuel's density.
     */
    class Engine
    {
        // Properties ---------------------------------------------------------------------------------------------------------
        public string EngineID { get; set; }
        public List<EngineCurvesPoint> EngineCurves { get; set; }
        public double MaxThrottle { get; set; }
        public double FuelDensity { get; set; }

        // Constructors -------------------------------------------------------------------------------------------------------
        public Engine()
        {
            EngineID = "Default";
            EngineCurves = new List<EngineCurvesPoint>();
            MaxThrottle = 100; // %
            FuelDensity = 719.7; // kg/m^3 (gasoline) 
        }

        public Engine(string engineID, List<EngineCurvesPoint> engineCurves, double maxThrottle, double fuelDensity)
        {
            EngineID = engineID;
            EngineCurves = engineCurves;
            MaxThrottle = Math.Abs(maxThrottle); // %
            FuelDensity = Math.Abs(fuelDensity); // kg/m^3
        }
        // Methods ------------------------------------------------------------------------------------------------------------
        public override string ToString()
        {
            return EngineID;
        }
    }
}
