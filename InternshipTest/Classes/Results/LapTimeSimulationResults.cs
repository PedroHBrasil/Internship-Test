using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternshipTest.Results
{
    /// <summary>
    /// Contains the results of a lap time simulation
    /// </summary>
    [Serializable]
    public class LapTimeSimulationResults : GenericInfo
    {
        private double lapTime;
        #region Properties
        // Results for the maximum possible speed case:
        /// <summary>
        /// Speeds given only the lateral acceleration limitations [m/s]
        /// </summary>
        public double[] MaximumPossibleSpeeds { get; set; }
        /// <summary>
        /// Longitudinal acceleration given only the lateral acceleration limitations [m/s²]
        /// </summary>
        public double[] LongitudinalAccelerationsForMaximumPossibleSpeeds { get; set; }
        /// <summary>
        /// Lateral acceleration given only the lateral acceleration limitations [m/s²]
        /// </summary>
        public double[] LateralAccelerationsForMaximumPossibleSpeeds { get; set; }
        /// <summary>
        /// Gear numbers given only the lateral acceleration limitations
        /// </summary>
        public int[] GearNumbersForMaximumPossibleSpeeds { get; set; }
        // Final dynamic state results:
        /// <summary>
        /// Final speeds [m/s]
        /// </summary>
        public double[] Speeds { get; set; }
        /// <summary>
        /// Final longitudinal accelerations [m/s²]
        /// </summary>
        public double[] LongitudinalAccelerations { get; set; }
        /// <summary>
        /// Final lateral accelerations [m/s²]
        /// </summary>
        public double[] LateralAccelerations { get; set; }
        /// <summary>
        /// Final gear numbers.
        /// </summary>
        public int[] GearNumbers { get; set; }
        // General results:
        /// <summary>
        /// Elapsed time [s].
        /// </summary>
        public double[] ElapsedTime { get; set; }
        // Path properties results:
        /// <summary>
        /// Path's resolution [m]
        /// </summary>
        private double PathResolution { get; set; }
        /// <summary>
        /// Elapsed distance [m]
        /// </summary>
        public double[] ElapsedDistance { get; set; }
        /// <summary>
        /// Path's local curvatures [1/m]
        /// </summary>
        public double[] PathCurvatures { get; set; }
        // Key Performance Indicators:
        /// <summary>
        /// Final lap time [s]
        /// </summary>
        public double LapTime
        {
            get => double.Parse(lapTime.ToString("N4"));
            set => lapTime = value;
        }
        #endregion
        #region Constructors
        public LapTimeSimulationResults() { }
        public LapTimeSimulationResults(Simulation.LapTimeSimulation lapTimeSimulation, int amountOfPointsInPath)
        {
            ElapsedDistance = lapTimeSimulation.Path.ElapsedDistance.ToArray();
            PathResolution = lapTimeSimulation.Path.Resolution;
            PathCurvatures = lapTimeSimulation.Path.LocalCurvatures.ToArray();
            MaximumPossibleSpeeds = new double[amountOfPointsInPath];
            LongitudinalAccelerationsForMaximumPossibleSpeeds = new double[amountOfPointsInPath];
            LateralAccelerationsForMaximumPossibleSpeeds = new double[amountOfPointsInPath];
            GearNumbersForMaximumPossibleSpeeds = new int[amountOfPointsInPath];
            Speeds = new double[amountOfPointsInPath];
            LongitudinalAccelerations = new double[amountOfPointsInPath];
            LateralAccelerations = new double[amountOfPointsInPath];
            GearNumbers = new int[amountOfPointsInPath];
            ElapsedTime = new double[amountOfPointsInPath];
        }
        #endregion
        #region Methods
        /// <summary>
        /// Gets the elapsed time vector and the total time.
        /// </summary>
        public void GetElapsedTimeAndLapTime()
        {
            // Initializes and gets the elapsed time array
            double currentElapsedTime = 0;
            ElapsedTime[0] = currentElapsedTime;
            for (int iPathPoint = 1; iPathPoint < Speeds.Length - 1; iPathPoint++)
            {
                // Updates the current elapsed time
                currentElapsedTime += PathResolution / Speeds[iPathPoint];
                // Adds it to the elapsed time array
                ElapsedTime[iPathPoint + 1] = currentElapsedTime;
            }
            // Gets the lap time
            LapTime = ElapsedTime.Last();
        }
        #endregion
    }
}
