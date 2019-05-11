using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternshipTest.Vehicle
{
    [Serializable]
    public class TwoWheelTransmission : Transmission
    {
        #region Properties
        /// <summary>
        /// Torque bias ratio between the front and rear axis [ratio] (% front).
        /// </summary>
        public double TorqueBias { get; set; }
        #endregion
        #region Constructors
        public TwoWheelTransmission() { }
        public TwoWheelTransmission(string id, string description, double torqueBias, double primaryRatio,
            double finalRatio, double gearShiftTime, double efficiency, GearRatiosSet gearRatios)
        {
            ID = id;
            Description = description;
            TorqueBias = torqueBias;
            PrimaryRatio = Math.Abs(primaryRatio);
            FinalRatio = Math.Abs(finalRatio);
            GearShiftTime = Math.Abs(gearShiftTime);
            Efficiency = Math.Abs(efficiency);
            GearRatiosSet = gearRatios;
        }
        #endregion
    }
}
