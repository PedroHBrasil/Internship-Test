using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternshipTest.Vehicle
{
    public class Car
    {
        #region Properties
        /// <summary>
        /// Car's identification.
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// Car's setup identification.
        /// </summary>
        public string SetupID { get; set; }
        /// <summary>
        /// Car's main information.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Car's engine subsystem.
        /// </summary>
        public Engine Engine { get; set; }
        /// <summary>
        /// Wheel's rotational speed vector [rad/s].
        /// </summary>
        // Output Properties
        public List<double> WheelRotationalSpeedCurve { get; set; }
        /// <summary>
        /// Wheel's equivalent engine torque vector [N*m]
        /// </summary>
        public List<double> WheelTorqueCurve { get; set; }
        /// <summary>
        /// Wheel's gear number vector.
        /// </summary>
        public List<double> WheelGearCurve { get; set; }
        /// <summary>
        /// Wheel's equivalent engine braking torque vector [N*m]
        /// </summary>
        public List<double> WheelBrakingTorqueCurve { get; set; }
        /// <summary>
        /// Wheel's engine specific fuel consumption vector [kg/W]
        /// </summary>
        public List<double> WheelSpecFuelConsCurve { get; set; }
        /// <summary>
        /// Car's lowest operation speed [m/s].
        /// </summary>
        public double LowestSpeed { get; set; }
        /// <summary>
        /// Car's highest operation speed [m/s].
        /// </summary>
        public double HighestSpeed { get; set; }
        #endregion
        #region Methods
        /// <summary>
        /// Sets up what is displayed at the UI's listbox.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "C: " + ID + " - S: " + SetupID;
        }
        #endregion
    }
}
