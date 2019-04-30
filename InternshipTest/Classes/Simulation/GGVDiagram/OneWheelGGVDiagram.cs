using MathNet.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternshipTest.Simulation
{
    public class OneWheelGGVDiagram : GGVDiagram
    {
        #region Properties
        /// <summary>
        /// Car and setup associated with the GGV diagram.
        /// </summary>
        public Vehicle.OneWheelCar Car { get; set; }
        /// <summary>
        /// GG diagrams which constitute the GGV diagram.
        /// </summary>
        public List<OneWheelGGDiagram> GGDiagrams { get; set; }
        #endregion
        #region Constructors
        public OneWheelGGVDiagram() { }

        public OneWheelGGVDiagram(string id, string description, Vehicle.OneWheelCar car, int amountOfPointsPerSpeed, int amountOfDirections, int amountOfSpeeds, double lowestSpeed, double highestSpeed)
        {
            ID = id;
            Description = description;
            Car = car;
            AmountOfPointsPerSpeed = amountOfPointsPerSpeed;
            AmountOfDirections = amountOfDirections;
            AmountOfSpeeds = amountOfSpeeds;
            LowestSpeed = lowestSpeed;
            HighestSpeed = highestSpeed;
        }
        #endregion
        #region Methods
        /// <summary>
        /// Generates a GGV diagram.
        /// </summary>
        public void GenerateGGVDiagram()
        {
            GGDiagrams = new List<OneWheelGGDiagram>();
            // Checks if the inputed speed limits extrapolate the car's operation speed range
            if (LowestSpeed < Car.LowestSpeed) LowestSpeed = Car.LowestSpeed;
            if (HighestSpeed > Car.HighestSpeed) HighestSpeed = Car.HighestSpeed;
            // Speed vector
            Speeds = Generate.LinearSpaced(AmountOfSpeeds, LowestSpeed, HighestSpeed);
            // GGV diagram generation
            for (int iSpeed = 0; iSpeed < Speeds.Length; iSpeed++)
            {
                GGDiagrams.Add(new OneWheelGGDiagram(Speeds[iSpeed], Car, AmountOfPointsPerSpeed, AmountOfDirections));
                GGDiagrams[iSpeed].GenerateGGDiagram();
                GGDiagrams[iSpeed].GetAssociatedCurvatures();
            }
        }
        /// <summary>
        /// Gets a GG diagram for a given speed.
        /// </summary>
        /// <param name="speed"> Car speed [m/s]. </param>
        /// <param name="car"> Car and Setup object. </param>
        /// <returns> Interpolated GG diagram </returns>
        public OneWheelGGDiagram GetGGDiagramForASpeed(double speed, Vehicle.OneWheelCar car)
        {
            // Checks if the speed is in the GGV diagram speed range
            if (speed < LowestSpeed || speed > HighestSpeed) return _GetGGDiagramByExtrapolationOfTheGGVDiagram(speed);
            else return _GetGGDiagramByInterpolationOfTheGGVDiagram(speed, car);
        }
        /// <summary>
        /// Gets a GG diagram via extrapolation of the GGV diagram (gets the closest GG).
        /// </summary>
        /// <param name="speed"> Car's speed [m/s]. </param>
        /// <returns> Extrapolated GG diagram </returns>
        private OneWheelGGDiagram _GetGGDiagramByExtrapolationOfTheGGVDiagram(double speed)
        {
            // Gets the index of the GG diagram to be used 
            int extrapolationIndex;
            if (speed < LowestSpeed) extrapolationIndex = 0;
            else extrapolationIndex = GGDiagrams.Count - 1;
            // Gets the GG diagram
            OneWheelGGDiagram extrapolatedGGDiagram = GGDiagrams[extrapolationIndex];
            extrapolatedGGDiagram.Speed = speed;
            extrapolatedGGDiagram.GetAssociatedCurvatures();
            return extrapolatedGGDiagram;
        }
        /// <summary>
        /// Gets a GG diagram via interpolation of the GGV diagram.
        /// </summary>
        /// <param name="speed"> Car's speed [m/s]. </param>
        /// <param name="car"> Car and Setup object. </param>
        /// <returns> Interpolated GG diagram </returns>
        private OneWheelGGDiagram _GetGGDiagramByInterpolationOfTheGGVDiagram(double speed, Vehicle.OneWheelCar car)
        {
            // Result initialization
            OneWheelGGDiagram interpolatedGGDiagram = new OneWheelGGDiagram(car);
            // Gets the index of the immediately lower speed GG diagram
            int iLowerSpeed;
            for (iLowerSpeed = 0; iLowerSpeed < AmountOfSpeeds - 1; iLowerSpeed++)
                if (speed > Speeds[iLowerSpeed] && speed <= Speeds[iLowerSpeed + 1]) break;
            // Gets the GG diagrams to be interpolated
            OneWheelGGDiagram lowerSpeedGGDiagram = GGDiagrams[iLowerSpeed];
            OneWheelGGDiagram higherSpeedGGDiagram = GGDiagrams[iLowerSpeed + 1];
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
        #endregion
    }
}
