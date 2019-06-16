using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
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
        public GGVDiagram(string id, string description, int amountOfPointsPerSpeed, int amountOfSpeeds, double lowestSpeed, double highestSpeed, Vehicle.OneWheelCar oneWheelCar)
        {
            ID = id;
            Description = description;
            AmountOfPointsPerSpeed = amountOfPointsPerSpeed;
            AmountOfSpeeds = amountOfSpeeds;
            LowestSpeed = lowestSpeed;
            HighestSpeed = highestSpeed;
            OneWheelCar = oneWheelCar;
            CarModelType = CarModelType.OneWheel;
        }
        public GGVDiagram(string id, string description, int amountOfPointsPerSpeed, int amountOfSpeeds, double lowestSpeed, double highestSpeed, Vehicle.TwoWheelCar twoWheelCar)
        {
            ID = id;
            Description = description;
            AmountOfPointsPerSpeed = amountOfPointsPerSpeed;
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
                OneWheelGGDiagram oneWheelGGDiagram = new OneWheelGGDiagram(Speeds[iSpeed], OneWheelCar, AmountOfPointsPerSpeed);
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
                TwoWheelGGDiagram twoWheelGGDiagram = new TwoWheelGGDiagram(Speeds[iSpeed], TwoWheelCar, AmountOfPointsPerSpeed);
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
        public double GetGearShiftTime()
        {
            switch (CarModelType)
            {
                case CarModelType.OneWheel:
                    return OneWheelCar.Transmission.GearShiftTime;
                case CarModelType.TwoWheel:
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
        public int GetGearNumberFromCarSpeed(double speed)
        {
            switch (CarModelType)
            {
                case CarModelType.OneWheel:
                    return OneWheelCar.GetGearNumberFromCarSpeed(speed);
                case CarModelType.TwoWheel:
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
        public double GetCarHighestSpeed()
        {
            switch (CarModelType)
            {
                case CarModelType.OneWheel:
                    return OneWheelCar.HighestSpeed;
                case CarModelType.TwoWheel:
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
        #region Lap Time Simulation Results Extraction Methods
        /// <summary>
        /// Gets the aerodynamic coefficients for a given set of aerodynamic input parameters.
        /// </summary>
        /// <param name="aeroInputParameters"> Speed [m/s], car slip angle [rad] (two wheel only) and longitudinal acceleration (two wheel only) [rad] </param>
        /// <returns> Aerodynamic coefficients </returns>
        public double[] GetAerodynamicCoefficients(double[] aeroInputParameters)
        {
            double[] aerodynamicCoefficients = new double[6];
            double speed = aeroInputParameters[0];
            switch (CarModelType)
            {
                case CarModelType.OneWheel:
                    Vehicle.OneWheelAerodynamicMapPoint oneWheelAerodynamicMapPoint = OneWheelCar.GetAerodynamicCoefficients(speed);
                    aerodynamicCoefficients[0] = oneWheelAerodynamicMapPoint.DragCoefficient;
                    aerodynamicCoefficients[2] = oneWheelAerodynamicMapPoint.LiftCoefficient;
                    break;
                case CarModelType.TwoWheel:
                    double carSlipAngle = aeroInputParameters[1];
                    double longitudinalAcceleration = aeroInputParameters[2];
                    Vehicle.TwoWheelAerodynamicMapPoint twoWheelAerodynamicMapPoint = TwoWheelCar.GetAerodynamicCoefficients(speed, carSlipAngle, longitudinalAcceleration);
                    aerodynamicCoefficients[0] = twoWheelAerodynamicMapPoint.DragCoefficient;
                    aerodynamicCoefficients[1] = twoWheelAerodynamicMapPoint.SideForceCoefficient;
                    aerodynamicCoefficients[2] = twoWheelAerodynamicMapPoint.LiftCoefficient;
                    aerodynamicCoefficients[3] = twoWheelAerodynamicMapPoint.PitchMomentCoefficient;
                    aerodynamicCoefficients[5] = twoWheelAerodynamicMapPoint.YawMomentCoefficient;
                    break;
                default:
                    break;
            }
            return aerodynamicCoefficients;
        }
        /// <summary>
        /// Gets the aerodynamic force or moment value.
        /// </summary>
        /// <param name="aerodynamicCoefficient"> Coefficient of the aerodynamic force or moment </param>
        /// <param name="speed"> Car's speed [m/s] </param>
        /// <returns> Aerodynamic force [N] or moment [Nm]. </returns>
        public double GetAerodynamicForceOrMoment(double aerodynamicCoefficient, double speed)
        {
            double aerodynamicParameter = 0;
            switch (CarModelType)
            {
                case CarModelType.OneWheel:
                    aerodynamicParameter = -aerodynamicCoefficient * OneWheelCar.Aerodynamics.AirDensity * OneWheelCar.Aerodynamics.FrontalArea * Math.Pow(speed, 2) / 2;
                    break;
                case CarModelType.TwoWheel:
                    aerodynamicParameter = -aerodynamicCoefficient * TwoWheelCar.Aerodynamics.AirDensity * TwoWheelCar.Aerodynamics.FrontalArea * Math.Pow(speed, 2) / 2;
                    break;
                default:
                    break;
            }
            return aerodynamicParameter;
        }
        /// <summary>
        /// Gets the car's total vertical load.
        /// </summary>
        /// <param name="aerodynamicLift"> Aerodynamic lift [N]. </param>
        /// <returns> Total vertical force [N] </returns>
        public double GetCarVerticalLoad(double aerodynamicLift)
        {
            switch (CarModelType)
            {
                case CarModelType.OneWheel:
                    return OneWheelCar.Inertia.TotalMass * OneWheelCar.Inertia.Gravity + aerodynamicLift;
                case CarModelType.TwoWheel:
                    return TwoWheelCar.InertiaAndDimensions.TotalMass * TwoWheelCar.InertiaAndDimensions.Gravity + aerodynamicLift;
                default:
                    return 0;
            }
        }
        /// <summary>
        /// Gets the total longitudinal load transfer
        /// </summary>
        /// <param name="longitudinalAcceleration"> Car's longitudinal acceleration [m/s²]. </param>
        /// <returns> Longitudinal load transfer [N] </returns>
        public double GetCarTotalLongitudinalLoadTransfer(double longitudinalAcceleration)
        {
            switch (CarModelType)
            {
                case CarModelType.TwoWheel:
                    return TwoWheelCar.InertiaAndDimensions.TotalMass * TwoWheelCar.InertiaAndDimensions.TotalMassCGHeight * longitudinalAcceleration / TwoWheelCar.InertiaAndDimensions.Wheelbase;
                default:
                    return 0;
            }
        }
        /// <summary>
        /// Gets the unsprung longitudinal load transfer.
        /// </summary>
        /// <param name="longitudinalAcceleration"> Car's longitudinal acceleration [m/s²]. </param>
        /// <returns> Longitudinal load transfer [N] </returns>
        public double GetCarUnsprungLongitudinalLoadTransfer(double longitudinalAcceleration)
        {
            switch (CarModelType)
            {
                case CarModelType.TwoWheel:
                    double frontUnsprungLongitudinalLoadTransfer = TwoWheelCar.InertiaAndDimensions.FrontUnsprungMass * TwoWheelCar.InertiaAndDimensions.FrontUnsprungMassCGHeight * longitudinalAcceleration / TwoWheelCar.InertiaAndDimensions.Wheelbase;
                    double rearUnsprungLongitudinalLoadTransfer = TwoWheelCar.InertiaAndDimensions.RearUnsprungMass * TwoWheelCar.InertiaAndDimensions.RearUnsprungMassCGHeight * longitudinalAcceleration / TwoWheelCar.InertiaAndDimensions.Wheelbase;
                    return frontUnsprungLongitudinalLoadTransfer + rearUnsprungLongitudinalLoadTransfer;
                default:
                    return 0;
            }
        }
        /// <summary>
        /// Gets the sprung longitudinal load transfer.
        /// </summary>
        /// <param name="totalLongitudinalLoadTransfer"> Total longiudinal load transfer [N] </param>
        /// <param name="unsprungLongitudinalLoadTransfer"> Unsprung longiudinal load transfer [N] </param>
        /// <returns> Sprung longitudinal load transfer [N] </returns>
        public double GetCarSprungLongitudinalLoadTransfer(double totalLongitudinalLoadTransfer, double unsprungLongitudinalLoadTransfer)
        {
            return totalLongitudinalLoadTransfer - unsprungLongitudinalLoadTransfer;
        }
        /// <summary>
        /// Gets the front and rear wheels loads without the weight forces.
        /// </summary>
        /// <param name="aerodynamicLift"> Aerodynamic lift force [N] </param>
        /// <param name="aerodynamicPitchMoment"> Aerodynamic pitch moment [Nm] </param>
        /// <param name="longitudinalLoadTransfer"> Total longitudinal laod transfer [N] </param>
        /// <returns> Loads at the front and rear wheels without the weights [N] </returns>
        public double[] GetWheelsLoadsWithoutWeight(double aerodynamicLift, double aerodynamicPitchMoment, double longitudinalLoadTransfer)
        {
            double[] wheelsLoads = new double[2];
            switch (CarModelType)
            {
                case CarModelType.OneWheel:
                    wheelsLoads[0] = aerodynamicLift / 2;
                    wheelsLoads[1] = aerodynamicLift / 2;
                    break;
                case CarModelType.TwoWheel:
                    wheelsLoads[0] = aerodynamicLift / 2 - aerodynamicPitchMoment / TwoWheelCar.InertiaAndDimensions.Wheelbase - longitudinalLoadTransfer;
                    wheelsLoads[1] = aerodynamicLift / 2 + aerodynamicPitchMoment / TwoWheelCar.InertiaAndDimensions.Wheelbase + longitudinalLoadTransfer;
                    break;
                default:
                    break;
            }
            return wheelsLoads;
        }
        /// <summary>
        /// Gets the front and rear wheels loads.
        /// </summary>
        /// <param name="wheelsLoadsWithoutWeight"> Loads at the front and rear wheels without the weight [N] </param>
        /// <returns> Loads at the front and rear wheels </returns>
        public double[] GetWheelsLoads(double[] wheelsLoadsWithoutWeight)
        {
            double[] wheelsLoads = new double[2];
            switch (CarModelType)
            {
                case CarModelType.OneWheel:
                    wheelsLoads[0] = (OneWheelCar.Inertia.TotalMass * OneWheelCar.Inertia.Gravity) / 2 + wheelsLoadsWithoutWeight[0];
                    wheelsLoads[1] = (OneWheelCar.Inertia.TotalMass * OneWheelCar.Inertia.Gravity) / 2 + wheelsLoadsWithoutWeight[1];
                    break;
                case CarModelType.TwoWheel:
                    wheelsLoads[0] = TwoWheelCar.InertiaAndDimensions.FrontWeight + wheelsLoadsWithoutWeight[0];
                    wheelsLoads[1] = TwoWheelCar.InertiaAndDimensions.FrontWeight + wheelsLoadsWithoutWeight[1];
                    break;
                default:
                    break;
            }
            return wheelsLoads;
        }
        /// <summary>
        /// Gets the front and rear wheels radiuses.
        /// </summary>
        /// <param name="wheelsLoads"> Loads at the front and rear wheels [N] </param>
        /// <returns> Radiuses of the front and rear wheels [m] </returns>
        public double[] GetWheelsRadiuses(double[] wheelsLoads)
        {
            double[] wheelsRadiuses = new double[2];
            switch (CarModelType)
            {
                case CarModelType.OneWheel:
                    wheelsRadiuses[0] = OneWheelCar.Tire.TireModel.RO - wheelsLoads[0] / 2 / OneWheelCar.Tire.VerticalStiffness;
                    wheelsRadiuses[1] = wheelsRadiuses[0];
                    break;
                case CarModelType.TwoWheel:
                    wheelsRadiuses[0] = TwoWheelCar.FrontTire.TireModel.RO - wheelsLoads[0] / 2 / TwoWheelCar.FrontTire.VerticalStiffness;
                    wheelsRadiuses[1] = TwoWheelCar.RearTire.TireModel.RO - wheelsLoads[1] / 2 / TwoWheelCar.RearTire.VerticalStiffness;
                    break;
                default:
                    break;
            }
            return wheelsRadiuses;
        }
        /// <summary>
        /// Gets the suspension deflections 
        /// </summary>
        /// <param name="wheelsLoads"> Loads at the front and rear wheels [N] </param>
        /// <returns> Deflections at the front and rear suspensions [m] </returns>
        public double[] GetSuspensionDeflections(double[] wheelsLoads)
        {
            double[] suspensionDeflections = new double[2];
            switch (CarModelType)
            {
                case CarModelType.OneWheel:
                    suspensionDeflections[0] = 2 * wheelsLoads[0] / OneWheelCar.Suspension.HeaveStiffness;
                    suspensionDeflections[1] = suspensionDeflections[0];
                    break;
                case CarModelType.TwoWheel:
                    suspensionDeflections[0] = (wheelsLoads[0] - TwoWheelCar.InertiaAndDimensions.FrontUnsprungWeight) / TwoWheelCar.FrontSuspension.HeaveStiffness;
                    suspensionDeflections[1] = (wheelsLoads[1] - TwoWheelCar.InertiaAndDimensions.RearUnsprungWeight) / TwoWheelCar.RearSuspension.HeaveStiffness;
                    break;
                default:
                    break;
            }
            return suspensionDeflections;
        }
        /// <summary>
        /// Gets the front and rear car heights.
        /// </summary>
        /// <param name="wheelsRadiuses"> Wheels radiuses [m] </param>
        /// <param name="suspensionDeflections"> Suspension deflections [m] </param>
        /// <returns> Front and rear car heights [m] </returns>
        public double[] GetCarHeights(double[] wheelsRadiuses, double[] suspensionDeflections)
        {
            double[] carHeights = new double[2];
            switch (CarModelType)
            {
                case CarModelType.OneWheel:
                    carHeights[0] = OneWheelCar.Suspension.RideHeight - (OneWheelCar.Tire.TireModel.RO - wheelsRadiuses[0]) - suspensionDeflections[0];
                    carHeights[1] = carHeights[0];
                    break;
                case CarModelType.TwoWheel:
                    carHeights[0] = TwoWheelCar.FrontSuspension.RideHeight - (TwoWheelCar.FrontTire.TireModel.RO - wheelsRadiuses[0]) - suspensionDeflections[0];
                    carHeights[1] = TwoWheelCar.RearSuspension.RideHeight - (TwoWheelCar.RearTire.TireModel.RO - wheelsRadiuses[1]) - suspensionDeflections[1];
                    break;
                default:
                    break;
            }
            return carHeights;
        }
        /// <summary>
        /// Gets the car's inertia efficiency.
        /// </summary>
        /// <param name="wheelsRadiuses"> Radiuses of the front and rear wheels. </param>
        /// <returns> Inertia efficiency </returns>
        public double GetInertiaEfficiency(double[] wheelsRadiuses)
        {
            switch (CarModelType)
            {
                case CarModelType.OneWheel:
                    return Math.Pow(wheelsRadiuses[0], 2) * OneWheelCar.Inertia.TotalMass / (Math.Pow(wheelsRadiuses[0], 2) * OneWheelCar.Inertia.TotalMass + OneWheelCar.Inertia.RotPartsMI);
                case CarModelType.TwoWheel:
                    return TwoWheelCar.GetInertiaEfficiency(wheelsRadiuses.Average());
                default:
                    return 0;
            }
        }
        /// <summary>
        /// Gets the car's resultant longitudinal force.
        /// </summary>
        /// <param name="longitudinalAcceleration"> Car's longitudinal acceleration [m/s²] </param>
        /// <param name="inertiaEfficiency"> Car's inertia efficiency </param>
        /// <returns> Resultant longitudinal force [N] </returns>
        public double GetCarLongitudinalForce(double longitudinalAcceleration, double inertiaEfficiency)
        {
            switch (CarModelType)
            {
                case CarModelType.OneWheel:
                    return OneWheelCar.Inertia.TotalMass * longitudinalAcceleration / inertiaEfficiency;
                case CarModelType.TwoWheel:
                    return TwoWheelCar.InertiaAndDimensions.TotalMass * longitudinalAcceleration / inertiaEfficiency;
                default:
                    return 0;
            }
        }
        /// <summary>
        /// Gets the car's resultant lateral force.
        /// </summary>
        /// <param name="lateralAcceleration"> Car's lateral acceleration [m/s²] </param>
        /// <returns> Resultant lateral force [N] </returns>
        public double GetCarLateralForce(double lateralAcceleration)
        {
            switch (CarModelType)
            {
                case CarModelType.OneWheel:
                    return OneWheelCar.Inertia.TotalMass * lateralAcceleration;
                case CarModelType.TwoWheel:
                    return TwoWheelCar.InertiaAndDimensions.TotalMass * lateralAcceleration;
                default:
                    return 0;
            }
        }
        /// <summary>
        /// Gets the wheels longitudinal forces and torques.
        /// </summary>
        /// <param name="carLongitudinalForce"> Car's resultant longitudinal force [N] </param>
        /// <param name="wheelsRadiuses"> Radiuses of the wheels [m] </param>
        /// <param name="aerodynamicDrag"> Aerodynamic drag force [N] </param>
        /// <returns> Wheels longitudinal forces [N] and torques [Nm] </returns>
        public double[][] GetWheelsTorquesAndLongitudinalForces(double carLongitudinalForce, double[] wheelsRadiuses, double aerodynamicDrag)
        {
            double[] wheelsLongitudinalForces = new double[2];
            double[] wheelsTorques = new double[2];
            switch (CarModelType)
            {
                case CarModelType.OneWheel:
                    if (OneWheelCar.Transmission.Type == "2WD")
                    {
                        wheelsLongitudinalForces[0] = 0;
                        wheelsLongitudinalForces[1] = carLongitudinalForce - aerodynamicDrag;
                    }
                    else
                    {
                        wheelsLongitudinalForces[0] = (carLongitudinalForce - aerodynamicDrag) / 2;
                        wheelsLongitudinalForces[1] = wheelsLongitudinalForces[0];
                    }
                    wheelsTorques[0] = wheelsLongitudinalForces[0] * wheelsRadiuses[0];
                    wheelsTorques[1] = wheelsLongitudinalForces[1] * wheelsRadiuses[1];
                    break;
                case CarModelType.TwoWheel:
                    // Generates the linear system matrix A and vector b of A*x=b adn solves it.
                    // Equations set:
                    // 0: frontLongitudinalForce + rearLonitudinalForce = totalLongitudinalForce
                    // 1: frontTorque * (1-torqueBias) - rearToque = 0
                    // 2: frontLongitudinalForce - frontTorque / frontWheelRadius = 0
                    // 3: rearLongitudinalForce - rearTorque / rearWheelRadius = 0
                    var A = Matrix<double>.Build.DenseOfArray(new double[,] {
                        { 1, 1, 0, 0 },
                        { 0, 0, 1-TwoWheelCar.Transmission.TorqueBias, -1},
                        { 1, 0, -1/wheelsRadiuses[0], 0},
                        { 0, 1, 0, -1/wheelsRadiuses[1]}
                    });
                    var b = Vector<double>.Build.Dense(new double[] { carLongitudinalForce - aerodynamicDrag , 0, 0, 0});
                    var x = A.Solve(b);
                    // Gets the results out of the vector x
                    wheelsLongitudinalForces[0] = x[0];
                    wheelsLongitudinalForces[1] = x[1];
                    wheelsTorques[0] = x[2];
                    wheelsTorques[0] = x[3];
                    break;
                default:
                    break;
            }
            return new double[][] { wheelsLongitudinalForces, wheelsTorques };
        }
        /// <summary>
        /// Gets the wheels angular speeds.
        /// </summary>
        /// <param name="speed"> Car's speed [m/s] </param>
        /// <param name="wheelsRadiuses"> Wheels radiuses [m] </param>
        /// <returns> Wheels angular speeds [rad/s] </returns>
        public double[] GetWheelsAngularSpeeds(double speed, double[] wheelsRadiuses)
        {
            return new double[] { speed / wheelsRadiuses[0], speed / wheelsRadiuses[1] };
        }
        /// <summary>
        /// Ges the current engine power.
        /// </summary>
        /// <param name="wheelsLongitudinalForces"> Wheels longitudinal forces [N] </param>
        /// <param name="speed"> Car speed [m/s] </param>
        /// <returns> Engine power [W] </returns>
        public double GetEnginePower(double[] wheelsTorques, double[] wheelsAngularSpeeds)
        {
            switch (CarModelType)
            {
                case CarModelType.OneWheel:
                    if (OneWheelCar.Transmission.Type == "4WD")
                    {
                        return wheelsTorques[1] * wheelsAngularSpeeds[1] * 2 / OneWheelCar.Transmission.Efficiency;
                    }
                    else
                    {
                        return wheelsTorques[1] * wheelsAngularSpeeds[1] / OneWheelCar.Transmission.Efficiency;
                    }
                case CarModelType.TwoWheel:
                    return (wheelsTorques[0] * wheelsAngularSpeeds[0] + wheelsTorques[1] * wheelsAngularSpeeds[1]) / TwoWheelCar.Transmission.Efficiency;
                default:
                    return 0;
            }
        }
        /// <summary>
        /// Gets the engine avaliable power.
        /// </summary>
        /// <param name="wheelsAngularSpeeds"> Wheels angular speeds [rad/s] </param>
        /// <returns> Engine power [W] </returns>
        public double GetEngineAvailablePower(double[] wheelsAngularSpeeds)
        {
            switch (CarModelType)
            {
                case CarModelType.OneWheel:
                    alglib.spline1dbuildlinear(OneWheelCar.WheelRotationalSpeedCurve.ToArray(), OneWheelCar.WheelTorqueCurve.ToArray(), out alglib.spline1dinterpolant oneWheelTorqueInterp);
                    return alglib.spline1dcalc(oneWheelTorqueInterp, wheelsAngularSpeeds[0]) * wheelsAngularSpeeds[0];
                case CarModelType.TwoWheel:
                    double referenceWheelAngularSpeed = wheelsAngularSpeeds[0] + (1 - TwoWheelCar.Transmission.TorqueBias) * (wheelsAngularSpeeds[1] - wheelsAngularSpeeds[0]);
                    alglib.spline1dbuildlinear(TwoWheelCar.WheelRotationalSpeedCurve.ToArray(), TwoWheelCar.WheelTorqueCurve.ToArray(), out alglib.spline1dinterpolant twoWheelTorqueInterp);
                    return alglib.spline1dcalc(twoWheelTorqueInterp, referenceWheelAngularSpeed) * referenceWheelAngularSpeed;
                default:
                    return 0;
            }
        }
        /// <summary>
        /// Gets the usage of a parameter given its maximum possible current value.
        /// </summary>
        /// <param name="currentParameter"> Parameter's current value </param>
        /// <param name="availableParameter"> Parameter's current maximum value. </param>
        /// <returns> Parameter's usage ratio. </returns>
        public double GetUsageRatio(double currentParameter, double availableParameter)
        {
            return currentParameter / availableParameter;
        }
        /// <summary>
        /// Gets the brakes powers.
        /// </summary>
        /// <param name="wheelsTorques"> Torques at the wheels [N] </param>
        /// <param name="wheelsAngularSpeeds"> Wheels angular speeds [rad/s] </param>
        /// <returns> Brakes powers [W] </returns>
        public double[] GetBrakesPower(double[] wheelsTorques, double[] wheelsAngularSpeeds)
        {
            double frontBrakesPower = -wheelsTorques[0] * wheelsAngularSpeeds[0];
            double rearBrakesPower = -wheelsTorques[1] * wheelsAngularSpeeds[1];
            return new double[] { frontBrakesPower, rearBrakesPower };
        }
        /// <summary>
        /// Gets the brakes available power.
        /// </summary>
        /// <param name="wheelsAngularSpeeds"> Wheels angular speeds [rad/s] </param>
        /// <returns> Brakes powers [W] </returns>
        public double[] GetBrakesAvailablePowers(double[] wheelsAngularSpeeds)
        {
            double[] brakesAvailablePowers = new double[2];
            switch (CarModelType)
            {
                case CarModelType.OneWheel:
                    alglib.spline1dbuildlinear(OneWheelCar.WheelRotationalSpeedCurve.ToArray(), OneWheelCar.WheelBrakingTorqueCurve.ToArray(), out alglib.spline1dinterpolant oneWheelBrakingTorqueInterp);
                    brakesAvailablePowers[0] = wheelsAngularSpeeds[0] * (alglib.spline1dcalc(oneWheelBrakingTorqueInterp, wheelsAngularSpeeds[0]) + OneWheelCar.Brakes.MaximumTorque / 2);
                    brakesAvailablePowers[1] = brakesAvailablePowers[0];
                    break;
                case CarModelType.TwoWheel:
                    double referenceWheelAngularSpeed = wheelsAngularSpeeds[0] + (1 - TwoWheelCar.Transmission.TorqueBias) * (wheelsAngularSpeeds[1] - wheelsAngularSpeeds[0]);
                    alglib.spline1dbuildlinear(TwoWheelCar.WheelRotationalSpeedCurve.ToArray(), TwoWheelCar.WheelBrakingTorqueCurve.ToArray(), out alglib.spline1dinterpolant twoWheelBrakingTorqueInterp);
                    brakesAvailablePowers[0] = wheelsAngularSpeeds[0] * (alglib.spline1dcalc(twoWheelBrakingTorqueInterp, referenceWheelAngularSpeed) * TwoWheelCar.Transmission.TorqueBias + TwoWheelCar.Brakes.FrontMaximumTorque);
                    brakesAvailablePowers[1] = wheelsAngularSpeeds[1] * (alglib.spline1dcalc(twoWheelBrakingTorqueInterp, referenceWheelAngularSpeed) * (1 - TwoWheelCar.Transmission.TorqueBias) + TwoWheelCar.Brakes.RearMaximumTorque);
                    break;
                default:
                    break;
            }
            return brakesAvailablePowers;
        }
        /// <summary>
        /// Gets the car's fuel consumption.
        /// </summary>
        /// <param name="enginePower"> Engine's power [W] </param>
        /// <param name="elapsedDistance"> Elapsed distance [m] </param>
        /// <param name="wheelsAngularSpeeds"> Angular speeds of the wheels [rad/s] </param>
        /// <returns> Fuel consumption [m³] </returns>
        public double GetFuelConsumption(double enginePower, double[] wheelsAngularSpeeds, double speed, double elapsedDistance)
        {
            double specificFuelConsumption;
            double fuelDensity;
            switch (CarModelType)
            {
                case CarModelType.OneWheel:
                    alglib.spline1dbuildlinear(OneWheelCar.WheelRotationalSpeedCurve.ToArray(), OneWheelCar.WheelSpecFuelConsCurve.ToArray(), out alglib.spline1dinterpolant oneWheelSpecFuelConsInterp);
                    specificFuelConsumption = alglib.spline1dcalc(oneWheelSpecFuelConsInterp, wheelsAngularSpeeds[0]) * wheelsAngularSpeeds[0];
                    fuelDensity = OneWheelCar.Engine.FuelDensity;
                    break;
                case CarModelType.TwoWheel:
                    double referenceWheelAngularSpeed = wheelsAngularSpeeds[0] + (1 - TwoWheelCar.Transmission.TorqueBias) * (wheelsAngularSpeeds[1] - wheelsAngularSpeeds[0]);
                    alglib.spline1dbuildlinear(TwoWheelCar.WheelRotationalSpeedCurve.ToArray(), TwoWheelCar.WheelSpecFuelConsCurve.ToArray(), out alglib.spline1dinterpolant twoWheelSpecFuelConsInterp);
                    specificFuelConsumption = alglib.spline1dcalc(twoWheelSpecFuelConsInterp, referenceWheelAngularSpeed) * wheelsAngularSpeeds[0];
                    fuelDensity = TwoWheelCar.Engine.FuelDensity;
                    break;
                default:
                    specificFuelConsumption = 0;
                    fuelDensity = 0;
                    break;
            }
            return specificFuelConsumption * enginePower / fuelDensity * elapsedDistance / speed;
        }
        #endregion
        #endregion
        #endregion
    }
}
