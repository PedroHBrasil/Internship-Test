using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternshipTest.Results
{
    class LapTimeSimulation
    {
        // Properties
        public ObservableCollection<LapTimeSimulationResultPoint> Results { get; set; }
        // Constructor
        public LapTimeSimulation(Simulation.LapTimeSimulation lapTimeSimulation)
        {
            // Elapsed time calculation
            List<double> elapsedTimes = new List<double>() { 0 };
            for (int iPoint = 1; iPoint < lapTimeSimulation.SimulationPath.AmountOfPointsInPath; iPoint++)
            {
                elapsedTimes.Add(elapsedTimes[iPoint - 1] + ((double)lapTimeSimulation.SimulationPath.Resolution / 1000) /
                    ((lapTimeSimulation.PathSpeeds[iPoint] + lapTimeSimulation.PathSpeeds[iPoint - 1]) / (2 * 3.6)));
            }
            // Add results
            Results = new ObservableCollection<LapTimeSimulationResultPoint>();
            for (int iPoint = 0; iPoint < lapTimeSimulation.SimulationPath.AmountOfPointsInPath; iPoint++)
            {
                Results.Add(new LapTimeSimulationResultPoint(
                    elapsedTimes[iPoint],
                    lapTimeSimulation.SimulationPath.ElapsedDistance[iPoint],
                    lapTimeSimulation.PathSpeeds[iPoint],
                    lapTimeSimulation.PathLongitudinalAccelerations[iPoint],
                    lapTimeSimulation.PathLateralAccelerations[iPoint],
                    lapTimeSimulation.PathGearNumbers[iPoint]));
            }
        }
    }
    class LapTimeSimulationResultPoint
    {
        // Properties
        public double ElapsedTime { get; set; }
        public double ElapsedDistance { get; set; }
        public double Speed { get; set; }
        public double LongitudinalAcceleration { get; set; }
        public double LateralAcceleration { get; set; }
        public int GearNumber { get; set; }
        // Constructor
        public LapTimeSimulationResultPoint(double elapsedTime, double elapsedDistance, double speed, 
            double longitudinalAcceleration, double lateralAcceleration, int gearNumber)
        {
            ElapsedTime = elapsedTime;
            ElapsedDistance = elapsedDistance;
            Speed = speed;
            LongitudinalAcceleration = longitudinalAcceleration;
            LateralAcceleration = lateralAcceleration;
            GearNumber = gearNumber;
        }
    }
}
