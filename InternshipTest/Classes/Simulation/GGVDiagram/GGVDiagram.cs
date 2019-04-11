using MathNet.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternshipTest.Simulation
{
    /// <summary>
    /// Contains a GGV diagram information.
    /// </summary>
    [Serializable]
    public class GGVDiagram : GenericInfo
    {
        #region Properties
        /// <summary>
        /// Car and setup associated with the GGV diagram.
        /// </summary>
        public Vehicle.OneWheel.Car Car { get; set; }
        /// <summary>
        /// Amount of points at each GG diagram.
        /// </summary>
        public int AmountOfPointsPerSpeed { get; set; }
        /// <summary>
        /// Amount of speeds of the GGV diagram.
        /// </summary>
        public int AmountOfSpeeds { get; set; }
        /// <summary>
        /// Lowest speed of the GG diagrams [m/s].
        /// </summary>
        public double LowestSpeed { get; set; }
        /// <summary>
        /// Highest speed of the GG diagrams [m/s].
        /// </summary>
        public double HighestSpeed { get; set; }
        /// <summary>
        /// Speeds of the GGV Diagram [m/s].
        /// </summary>
        public double[] Speeds { get; set; }
        /// <summary>
        /// GG diagrams which constitute the GGV diagram.
        /// </summary>
        public List<GGDiagram> GGDiagrams { get; set; }
        #endregion

        #region Constructors
        public GGVDiagram() { }

        public GGVDiagram(string id, string description, Vehicle.OneWheel.Car car, int amountOfPointsPerSpeed, int amountOfSpeeds, double lowestSpeed, double highestSpeed)
        {
            ID = id;
            Description = description;
            Car = car;
            AmountOfPointsPerSpeed = amountOfPointsPerSpeed;
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
            GGDiagrams = new List<GGDiagram>();
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
        /// <summary>
        /// Gets a GG diagram for a given speed.
        /// </summary>
        /// <param name="speed"> Car speed [m/s]. </param>
        /// <param name="car"> Car and Setup object. </param>
        /// <returns> Interpolated GG diagram </returns>
        public GGDiagram GetGGDiagramForASpeed(double speed, Vehicle.OneWheel.Car car)
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
        private GGDiagram _GetGGDiagramByExtrapolationOfTheGGVDiagram(double speed)
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
        /// <summary>
        /// Gets a GG diagram via interpolation of the GGV diagram.
        /// </summary>
        /// <param name="speed"> Car's speed [m/s]. </param>
        /// <param name="car"> Car and Setup object. </param>
        /// <returns> Interpolated GG diagram </returns>
        private GGDiagram _GetGGDiagramByInterpolationOfTheGGVDiagram(double speed, Vehicle.OneWheel.Car car)
        {
            // Result initialization
            GGDiagram interpolatedGGDiagram = new GGDiagram(car);
            // Gets the index of the index of the immediately lower speed GG diagram
            int iLowerSpeed;
            for (iLowerSpeed = 0; iLowerSpeed < AmountOfSpeeds - 1; iLowerSpeed++)
                if (speed > Speeds[iLowerSpeed] && speed <= Speeds[iLowerSpeed + 1]) break;
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
        #endregion
    }
}
