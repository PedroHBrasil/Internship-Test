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
        public int AmountOfPointsPerSpeed { get; set; }
        /// <summary>
        /// Amount of directions of the GGV diagram's accelerations.
        /// </summary>
        public int AmountOfDirections { get; set; }
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
        /// <summary>
        /// One Wheel Model Car object.
        /// </summary>
        public Vehicle.OneWheelCar OneWheelCar { get; set; }
        /// <summary>
        /// Two Wheel Model Car object.
        /// </summary>
        public Vehicle.TwoWheelCar TwoWheelCar { get; set; }
        public CarModelType CarModelType { get; set; }
        #endregion
        #region Constructors
        public GGVDiagram() { }
        public GGVDiagram(string id, string description, int amountOfPointsPerSpeed, int amountOfDirections, int amountOfSpeeds, double lowestSpeed, double highestSpeed, Vehicle.OneWheelCar oneWheelCar)
        {
            ID = id;
            Description = description;
            AmountOfPointsPerSpeed = amountOfPointsPerSpeed;
            AmountOfDirections = amountOfDirections;
            AmountOfSpeeds = amountOfSpeeds;
            LowestSpeed = lowestSpeed;
            HighestSpeed = highestSpeed;
            OneWheelCar = oneWheelCar;
            CarModelType = CarModelType.OneWheel;
        }
        public GGVDiagram(string id, string description, int amountOfPointsPerSpeed, int amountOfDirections, int amountOfSpeeds, double lowestSpeed, double highestSpeed, Vehicle.TwoWheelCar twoWheelCar)
        {
            ID = id;
            Description = description;
            AmountOfPointsPerSpeed = amountOfPointsPerSpeed;
            AmountOfDirections = amountOfDirections;
            AmountOfSpeeds = amountOfSpeeds;
            LowestSpeed = lowestSpeed;
            HighestSpeed = highestSpeed;
            TwoWheelCar = twoWheelCar;
            CarModelType = CarModelType.TwoWheel;
        }

        #endregion
        #region Methods
        #region GGV Diagram Generation

        /// <summary>
        /// Generates a GGV diagram for a one wheel model car.
        /// </summary>
        public void GenerateGGVDiagramForTheOneWheelModel()
        {
            GGDiagrams = new List<GGDiagram>();
            // Checks if the inputed speed limits extrapolate the car's operation speed range
            if (LowestSpeed < OneWheelCar.LowestSpeed) LowestSpeed = OneWheelCar.LowestSpeed;
            if (HighestSpeed > OneWheelCar.HighestSpeed) HighestSpeed = OneWheelCar.HighestSpeed;
            // Speed vector
            Speeds = Generate.LinearSpaced(AmountOfSpeeds, LowestSpeed, HighestSpeed);
            // GGV diagram generation
            for (int iSpeed = 0; iSpeed < Speeds.Length; iSpeed++)
            {
                OneWheelGGDiagram oneWheelGGDiagram = new OneWheelGGDiagram(Speeds[iSpeed], OneWheelCar, AmountOfPointsPerSpeed, AmountOfDirections);
                oneWheelGGDiagram.GenerateGGDiagram();
                GGDiagram diagram = new GGDiagram(oneWheelGGDiagram);
                diagram.GetAssociatedCurvatures();
                GGDiagrams.Add(diagram);
            }
        }
        /// <summary>
        /// Generates a GGV diagram for a two wheel model car.
        /// </summary>
        public void GenerateGGVDiagramForTheTwoWheelModel()
        {
            GGDiagrams = new List<GGDiagram>();
            // Checks if the inputed speed limits extrapolate the car's operation speed range
            if (LowestSpeed < TwoWheelCar.LowestSpeed) LowestSpeed = TwoWheelCar.LowestSpeed;
            if (HighestSpeed > TwoWheelCar.HighestSpeed) HighestSpeed = TwoWheelCar.HighestSpeed;
            // Speed vector
            Speeds = Generate.LinearSpaced(AmountOfSpeeds, LowestSpeed, HighestSpeed);
            // GGV diagram generation
            for (int iSpeed = 0; iSpeed < Speeds.Length; iSpeed++)
            {
                TwoWheelGGDiagram twoWheelGGDiagram = new TwoWheelGGDiagram(Speeds[iSpeed], TwoWheelCar, AmountOfPointsPerSpeed, AmountOfDirections);
                twoWheelGGDiagram.GenerateGGDiagram();
                GGDiagram diagram = new GGDiagram(twoWheelGGDiagram);
                diagram.GetAssociatedCurvatures();
                GGDiagrams.Add(diagram);
            }
        }

        #endregion
        #region Lap Time Simulation Methods

        /// <summary>
        /// Gets the car's gear shifting time.
        /// </summary>
        /// <param name="carModelType"> Car Model </param>
        /// <returns> Gear shift time [s] </returns>
        public double GetGearShiftTime(string carModelType)
        {
            switch (carModelType)
            {
                case "One Wheel":
                    return OneWheelCar.Transmission.GearShiftTime;
                case "Two Wheel":
                    return TwoWheelCar.Transmission.GearShiftTime;
                default:
                    return 0;
            }
        }
        /// <summary>
        /// Gets the gear number for a car speed.
        /// </summary>
        /// <param name="carModelType"> Car Model </param>
        /// <param name="speed"> Car speed [m/s] </param>
        /// <returns> Current gear number. </returns>
        public int GetGearNumberFromCarSpeed(string carModelType, double speed)
        {
            switch (carModelType)
            {
                case "One Wheel":
                    return OneWheelCar.GetGearNumberFromCarSpeed(speed);
                case "Two Wheel":
                    return TwoWheelCar.GetGearNumberFromCarSpeed(speed);
                default:
                    return 0;
            }
        }
        /// <summary>
        /// Gets the car highest speed.
        /// </summary>
        /// <param name="carModelType"> Car Model </param>
        /// <returns> Car's highest speed [m/s] </returns>
        public double GetCarHighestSpeed(string carModelType)
        {
            switch (carModelType)
            {
                case "One Wheel":
                    return OneWheelCar.HighestSpeed;
                case "Two Wheel":
                    return TwoWheelCar.HighestSpeed;
                default:
                    return 0;
            }
        }
        /// <summary>
        /// Gets a GG diagram for a given speed.
        /// </summary>
        /// <param name="speed"> Car speed [m/s]. </param>
        /// <returns> Interpolated GG diagram </returns>
        public GGDiagram GetGGDiagramForASpeed(double speed)
        {
            // Checks if the speed is in the GGV diagram speed range
            if (speed < LowestSpeed || speed > HighestSpeed) return _GetGGDiagramByExtrapolationOfTheGGVDiagram(speed);
            else return _GetGGDiagramByInterpolationOfTheGGVDiagram(speed);
        }

        #region Private
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
        /// <returns> Interpolated GG diagram </returns>
        private GGDiagram _GetGGDiagramByInterpolationOfTheGGVDiagram(double speed)
        {
            // Result initialization
            GGDiagram interpolatedGGDiagram = new GGDiagram() { Speed = speed };
            // Gets the index of the immediately lower speed GG diagram
            int iLowerSpeed;
            for (iLowerSpeed = 0; iLowerSpeed < AmountOfSpeeds - 2; iLowerSpeed++)
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

        #endregion
        #endregion
    }
}
