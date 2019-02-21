using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternshipTest.Vehicle.OneWheel
{
    /*
     * Contains the parametrs of a car's transmission.
     *  - TransmissionID: string which identifies the object;
     *  - PrimaryRatio: primary gear ratio (between the engine and the gearbox);
     *  - GearRatios: class which contains the gearbox's gear ratios;
     *  - FinalRatio: final gear ratio (between the gearbox and the wheels);
     *  - GearShiftTime: time needed to make a gear shift maneuver [s];
     *  - Efficiency: drivetrain efficiency due to friction [%]; and
     *  - DrivetrainType: amount of wheels that are connected to the transmission.
     */
    class Transmission
    {
        // Properties --------------------------------------------------------------------------------------
        public string TransmissionID { get; set; }
        public string Type { get; set; }
        public double PrimaryRatio { get; set; }
        public List<GearRatio> GearRatios { get; set; }
        public double FinalRatio { get; set; }
        public double GearShiftTime { get; set; }
        public double Efficiency { get; set; }

        public int AmountOfDrivenWheels { get; set; }

        // Constructors -----------------------------------------------------------------------------------
        public Transmission()
        {
            TransmissionID = "Default";
            Type = "2WD";
            PrimaryRatio = 1;
            FinalRatio = 1;
            GearShiftTime = 1;
            Efficiency = 100;
            GearRatios = new List<GearRatio>();
            AmountOfDrivenWheels = 2;
        }

        public Transmission(string transmissionID, string type, double primaryRatio,
            double finalRatio, double gearShiftTime, double efficiency, List<GearRatio> gearRatios)
        {
            TransmissionID = transmissionID;
            Type = type;
            PrimaryRatio = Math.Abs(primaryRatio);
            FinalRatio = Math.Abs(finalRatio);
            GearShiftTime = Math.Abs(gearShiftTime);
            Efficiency = Math.Abs(efficiency);
            GearRatios = gearRatios;

            if (Type == "2WD") AmountOfDrivenWheels = 2;
            else AmountOfDrivenWheels = 4;
        }
        // Methods ----------------------------------------------------------------------------------------
        public override string ToString()
        {
            return TransmissionID;
        }

    }
}
