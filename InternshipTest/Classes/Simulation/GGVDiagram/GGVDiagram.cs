using MathNet.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternshipTest.Simulation
{
    [Serializable]
    class GGVDiagram
    {
        // Properties
        public Vehicle.OneWheel.Car Car { get; set; }
        public int AmountOfPointsPerSpeed { get; set; }
        public int AmountOfSpeeds { get; set; }
        public double LowestSpeed { get; set; }
        public double HighestSpeed { get; set; }

        public double[] Speeds { get; set; }
        public List<GGDiagram> GGDiagrams { get; set; }
        // Constructors
        public GGVDiagram(Vehicle.OneWheel.Car car, int amountOfPointsPerSpeed, int amountOfSpeeds, double lowestSpeed, double highestSpeed)
        {
            Car = car;
            AmountOfPointsPerSpeed = amountOfPointsPerSpeed;
            AmountOfSpeeds = amountOfSpeeds;
            LowestSpeed = lowestSpeed;
            HighestSpeed = highestSpeed;

            GGDiagrams = new List<GGDiagram>();

            GenerateGGVDiagram();
        }
        // Methods
        public override string ToString()
        {
            return "C: " + Car.CarID + " - S: " + Car.SetupID + " - nS: " + AmountOfSpeeds.ToString() + " - nP: " + AmountOfPointsPerSpeed.ToString();
        }

        private void GenerateGGVDiagram()
        {
            // Checks if the inputed speed limits extrapolate the car's operation speed range
            if (LowestSpeed < Car.LowestSpeed) LowestSpeed = Car.LowestSpeed;
            if (HighestSpeed > Car.HighestSpeed) HighestSpeed = Car.HighestSpeed;
            // Speed vector
            Speeds = Generate.LinearSpaced(AmountOfSpeeds, LowestSpeed, HighestSpeed);
            // GGV diagram generation
            foreach (double speed in Speeds)
            {
                GGDiagrams.Add(new GGDiagram(speed, Car, AmountOfPointsPerSpeed));
            }
        }

        public GGDiagram GetGGDiagramFromInterpolationBySpeed(double speed, Vehicle.OneWheel.Car car)
        {
            // Checks if the speed is in the GGV diagram speed range
            if (speed < LowestSpeed || speed > HighestSpeed) return GetGGDiagramByExtrapolationOfTheGGVDiagram(speed);
            else return GetGGDiagramByInterpolationOfTheGGVDiagram(speed, car);
        }
        private GGDiagram GetGGDiagramByExtrapolationOfTheGGVDiagram(double speed)
        {
            // Gets the index of the GG diagram to be used 
            int extrapolationIndex;
            if (speed < LowestSpeed) extrapolationIndex = 0;
            else extrapolationIndex = GGDiagrams.Count - 1;
            // Gets the GG diagram
            GGDiagram extrapolatedGGDiagram = GGDiagrams[extrapolationIndex];
            extrapolatedGGDiagram.Speed = speed;
            extrapolatedGGDiagram.GetAssociatedCurvatures();
            return extrapolatedGGDiagram;
        }
        private GGDiagram GetGGDiagramByInterpolationOfTheGGVDiagram(double speed, Vehicle.OneWheel.Car car)
        {
            // Result initialization
            GGDiagram interpolatedGGDiagram = new GGDiagram(car);
            // Gets the index of the index of the immediately lower speed GG diagram
            int iLowerSpeed;
            for (iLowerSpeed = 0; iLowerSpeed < AmountOfSpeeds - 1; iLowerSpeed++)
                if (speed > Speeds[iLowerSpeed] && speed < Speeds[iLowerSpeed + 1]) break;
            // Gets the GG diagrams to be interpolated
            GGDiagram lowerSpeedGGDiagram = GGDiagrams[iLowerSpeed];
            GGDiagram higherSpeedGGDiagram = GGDiagrams[iLowerSpeed + 1];
            // Interpolation ratio
            double interpolationRatio = (speed - Speeds[iLowerSpeed]) / (Speeds[iLowerSpeed + 1] - Speeds[iLowerSpeed]);
            // Interpolated GG Diagram accelerations
            for (int iAccelerations = 0; iAccelerations < GGDiagrams[0].LongitudinalAccelerations.Count; iAccelerations++)
            {
                // Current accelerations interpolation
                double interpolatedLongitudinalAcceleration = lowerSpeedGGDiagram.LongitudinalAccelerations[iAccelerations] +
                    interpolationRatio * (higherSpeedGGDiagram.LongitudinalAccelerations[iAccelerations] - lowerSpeedGGDiagram.LongitudinalAccelerations[iAccelerations]);
                double interpolatedLateralAcceleration = lowerSpeedGGDiagram.LateralAccelerations[iAccelerations] +
                    interpolationRatio * (higherSpeedGGDiagram.LateralAccelerations[iAccelerations] - lowerSpeedGGDiagram.LateralAccelerations[iAccelerations]);
                // Adds current interpolated accelerations to the interpolated GG Diagram
                interpolatedGGDiagram.LongitudinalAccelerations.Add(interpolatedLongitudinalAcceleration);
                interpolatedGGDiagram.LateralAccelerations.Add(interpolatedLateralAcceleration);
            }
            // Gets the associated curvatures
            interpolatedGGDiagram.GetAssociatedCurvatures();
            // Returns the interpolated GG diagram
            return interpolatedGGDiagram;
        }
    }
}
