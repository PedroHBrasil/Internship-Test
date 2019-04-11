using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternshipTest.Vehicle.OneWheel
{
    /// <summary>
    /// Contains the information about a one wheel model's transmission subsystem.
    /// </summary>
    public class Transmission : GenericInfo
    {
        #region Properties
        /// <summary>
        /// Amount of driven wheels.
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// Primary gear ratio of the transmission (ratio between the engine and the gearbox) [(engine's speed)/(speed into gearbox)].
        /// </summary>
        public double PrimaryRatio { get; set; }
        /// <summary>
        /// Set of a gearbox's gear ratios [(speed into gearbox)/(speed out of gearbox)].
        /// </summary>
        public GearRatiosSet GearRatiosSet { get; set; }
        /// <summary>
        /// Final gear ratio of the transmission (ratio between the gearbox and the driven wheels) [(speed out of gear box)/(speed at driven wheels)].
        /// </summary>
        public double FinalRatio { get; set; }
        /// <summary>
        /// Time taken to shift gears [s].
        /// </summary>
        public double GearShiftTime { get; set; }
        /// <summary>
        /// Share of energy that actually arrives at the wheels due to friction losses [(power at wheels)/(power at engine)].
        /// </summary>
        public double Efficiency { get; set; }

        public int AmountOfDrivenWheels { get; set; }
        #endregion
        #region Constructors
        public Transmission() { }

        public Transmission(string transmissionID, string description, int amountOfDrivenWheels, double primaryRatio,
            double finalRatio, double gearShiftTime, double efficiency, GearRatiosSet gearRatios)
        {
            ID = transmissionID;
            Description = description;
            AmountOfDrivenWheels = Math.Abs(amountOfDrivenWheels);
            PrimaryRatio = Math.Abs(primaryRatio);
            FinalRatio = Math.Abs(finalRatio);
            GearShiftTime = Math.Abs(gearShiftTime);
            Efficiency = Math.Abs(efficiency);
            GearRatiosSet = gearRatios;
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
