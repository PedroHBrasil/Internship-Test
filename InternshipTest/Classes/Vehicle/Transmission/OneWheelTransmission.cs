using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternshipTest.Vehicle
{
    /// <summary>
    /// Contains the information about a one wheel model's transmission subsystem.
    /// </summary>
    public class OneWheelTransmission : Transmission
    {
        #region Properties
        /// <summary>
        /// Dictates the amount of driven wheels.
        /// </summary>
        public string Type 
        {
            get 
            {
                if (AmountOfDrivenWheels == 2) return "2WD";
                else return "4WD";
            }
            set 
            {
                if (value == "2WD") AmountOfDrivenWheels = 2;
                else AmountOfDrivenWheels = 4;
            }
        }
        /// <summary>
        /// Amount of driven wheels.
        /// </summary>
        public int AmountOfDrivenWheels { get; set; }
        #endregion
        #region Constructors
        public OneWheelTransmission() { }

        public OneWheelTransmission(string id, string description, string type, double primaryRatio,
            double finalRatio, double gearShiftTime, double efficiency, GearRatiosSet gearRatios)
        {
            ID = id;
            Description = description;
            Type = type;
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
