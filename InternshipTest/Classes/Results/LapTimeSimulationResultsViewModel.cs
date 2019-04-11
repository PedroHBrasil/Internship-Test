using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternshipTest.Results
{
    /// <summary>
    /// Usedto dislpay the lap time simulation results.
    /// </summary>
    class LapTimeSimulationResultsViewModel
    {
        #region Properties
        /// <summary>
        /// Results collection.
        /// </summary>
        public ObservableCollection<LapTimeSimulationResultPoint> ResultsDisplayCollection { get; set; }
        #endregion
        #region Constructors
        public LapTimeSimulationResultsViewModel(LapTimeSimulationResults results)
        {
            ResultsDisplayCollection = new ObservableCollection<LapTimeSimulationResultPoint>();
            for (int iResult = 0; iResult < results.ElapsedDistance.Count(); iResult++)
            {
                LapTimeSimulationResultPoint resultPoint = new LapTimeSimulationResultPoint(
                    results.ElapsedTime[iResult],
                    results.ElapsedDistance[iResult],
                    results.Speeds[iResult],
                    results.LongitudinalAccelerations[iResult],
                    results.LateralAccelerations[iResult],
                    results.GearNumbers[iResult]);
                ResultsDisplayCollection.Add(resultPoint);
            }
        }
        #endregion
    }
    /// <summary>
    /// Contanis the information of one lap time simulation result point.
    /// </summary>
    class LapTimeSimulationResultPoint
    {
        #region Properties
        /// <summary>
        /// Elapsed time [s]
        /// </summary>
        public double ElapsedTime { get; set; }
        /// <summary>
        /// Elapsed distance [m]
        /// </summary>
        public double ElapsedDistance { get; set; }
        /// <summary>
        /// Speed [km/h]
        /// </summary>
        public double Speed { get; set; }
        /// <summary>
        /// Longitudinal Acceleration [m/s²]
        /// </summary>
        public double LongitudinalAcceleration { get; set; }
        /// <summary>
        /// Lateral acceleration [m/s²]
        /// </summary>
        public double LateralAcceleration { get; set; }
        /// <summary>
        /// Gear number
        /// </summary>
        public int GearNumber { get; set; }
        #endregion
        #region Constructors
        public LapTimeSimulationResultPoint(double elapsedTime, double elapsedDistance, double speed,
            double longitudinalAcceleration, double lateralAcceleration, int gearNumber)
        {
            ElapsedTime = elapsedTime;
            ElapsedDistance = elapsedDistance;
            Speed = speed * 3.6;
            LongitudinalAcceleration = longitudinalAcceleration;
            LateralAcceleration = lateralAcceleration;
            GearNumber = gearNumber;
        }
        #endregion
    }
}
