using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternshipTest.Vehicle.OneWheel
{
    /*
     * Contains an engine's curves point parameters.
     *  - RPM: Engine's rotation speed [rpm];
     *  - Torque: Engine's maximum outing torque [Nm];
     *  - BrakingTorque: Engine's minimum outing torque [Nm]; and
     *  - SpecFuelCons: Engine's specific fuel consumption [g/kW].
     */
    public class EngineCurvesPoint
    {
        // Properties --------------------------------------------------------------------------------------------------------------------------------
        public int RPM { get; set; }
        public double Torque { get; set; }
        public double BrakingTorque { get; set; }
        public double SpecFuelCons { get; set; }

        // Constructors ------------------------------------------------------------------------------------------------------------------------------
        public EngineCurvesPoint()
        {
            RPM = 0; // rpm
            Torque = 0; // Nm
            BrakingTorque = 0; // Nm
            SpecFuelCons = 0; // g/kW
        }

        public EngineCurvesPoint(int rpm, double torque, double brakingTorque, double specificFuelConsumption)
        {
            RPM = Math.Abs(rpm); // rpm
            Torque = Math.Abs(torque); // Nm
            BrakingTorque = -Math.Abs(brakingTorque); // Nm
            SpecFuelCons = Math.Abs(specificFuelConsumption); //g/kW
        }
    }
}
