using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternshipTest.Results
{
    /// <summary>
    /// Used to dislpay the lap time simulation results.
    /// </summary>
    public class LapTimeSimulationResultsViewModel : GenericInfo
    {
        #region Properties
        /// <summary>
        /// Results collection.
        /// </summary>
        public ObservableCollection<LapTimeSimulationResultPoint> ResultsDisplayCollection { get; set; }
        /// <summary>
        /// Total lap time [s]
        /// </summary>
        public double LapTime { get; set; }
        /// <summary>
        /// Total fuel consumption [mL]
        /// </summary>
        public double FuelConsumption { get; set; }
        /// <summary>
        /// Amount of gear shifts.
        /// </summary>
        public int AmountOfGearShifts { get; set; }
        #endregion
        #region Constructors
        public LapTimeSimulationResultsViewModel(LapTimeSimulationResults results)
        {
            ID = results.ID;
            Description = results.Description;
            ResultsDisplayCollection = new ObservableCollection<LapTimeSimulationResultPoint>();
            for (int iResult = 0; iResult < results.ElapsedDistances.Count(); iResult++)
            {
                LapTimeSimulationResultPoint resultPoint = new LapTimeSimulationResultPoint(
                    results.ElapsedTimes[iResult],
                    results.ElapsedDistances[iResult],
                    results.Speeds[iResult],
                    results.LongitudinalAccelerations[iResult],
                    results.LateralAccelerations[iResult],
                    results.GearNumbers[iResult],
                    results.AeroDragCoefficients[iResult],
                    results.AeroLiftCoefficients[iResult],
                    results.AeroDownforceDistributions[iResult],
                    results.AeroDragForces[iResult],
                    results.AeroLiftForces[iResult],
                    results.FrontLiftForces[iResult],
                    results.RearLiftForces[iResult],
                    results.VerticalLoads[iResult],
                    results.TotalLongitudinalLoadTransfers[iResult],
                    results.UnsprungLongitudinalLoadTransfers[iResult],
                    results.SprungLongitudinalLoadTransfers[iResult],
                    results.FrontWheelsLoads[iResult],
                    results.RearWheelsLoads[iResult],
                    results.FrontWheelsRadiuses[iResult],
                    results.RearWheelsRadiuses[iResult],
                    results.FrontSuspensionDeflections[iResult],
                    results.RearSuspensionDeflections[iResult],
                    results.FrontRideHeights[iResult],
                    results.RearRideHeights[iResult],
                    results.LateralForces[iResult],
                    results.InertiaEfficiencies[iResult],
                    results.LongitudinalForces[iResult],
                    results.FrontWheelsLongiudinalForces[iResult],
                    results.RearWheelsLongiudinalForces[iResult],
                    results.FrontWheelsTorques[iResult],
                    results.RearWheelsTorques[iResult],
                    results.FrontWheelsAngularSpeeds[iResult],
                    results.RearWheelsAngularSpeeds[iResult],
                    results.EnginePowers[iResult],
                    results.EngineAvailablePowers[iResult],
                    results.EnginePowerUsages[iResult],
                    results.FrontBrakesPowers[iResult],
                    results.RearBrakesPowers[iResult],
                    results.FrontBrakesAvailablePowers[iResult],
                    results.RearBrakesAvailablePowers[iResult],
                    results.FrontBrakesUsages[iResult],
                    results.RearBrakesUsages[iResult],
                    results.FuelConsumptions[iResult],
                    results.CoordinatesX[iResult],
                    results.CoordinatesY[iResult]
                    );
                ResultsDisplayCollection.Add(resultPoint);
            }
            LapTime = results.LapTime;
            FuelConsumption = results.TotalFuelConsumption *1e6;
            AmountOfGearShifts = results.AmountOfGearShifts;
        }
        public LapTimeSimulationResultsViewModel(LapTimeSimulationResults results, int sectorIndex)
        {
            ID = results.ID;
            Description = results.Description;
            ResultsDisplayCollection = new ObservableCollection<LapTimeSimulationResultPoint>();
            for (int iResult = 0; iResult < results.ElapsedDistances.Count(); iResult++)
            {
                if (results.LocalSectors[iResult] == sectorIndex)
                {
                    LapTimeSimulationResultPoint resultPoint = new LapTimeSimulationResultPoint(
                       results.ElapsedTimes[iResult],
                       results.ElapsedDistances[iResult],
                       results.Speeds[iResult],
                       results.LongitudinalAccelerations[iResult],
                       results.LateralAccelerations[iResult],
                       results.GearNumbers[iResult],
                       results.AeroDragCoefficients[iResult],
                       results.AeroLiftCoefficients[iResult],
                       results.AeroDownforceDistributions[iResult],
                       results.AeroDragForces[iResult],
                       results.AeroLiftForces[iResult],
                       results.FrontLiftForces[iResult],
                       results.RearLiftForces[iResult],
                       results.VerticalLoads[iResult],
                       results.TotalLongitudinalLoadTransfers[iResult],
                       results.UnsprungLongitudinalLoadTransfers[iResult],
                       results.SprungLongitudinalLoadTransfers[iResult],
                       results.FrontWheelsLoads[iResult],
                       results.RearWheelsLoads[iResult],
                       results.FrontWheelsRadiuses[iResult],
                       results.RearWheelsRadiuses[iResult],
                       results.FrontSuspensionDeflections[iResult],
                       results.RearSuspensionDeflections[iResult],
                       results.FrontRideHeights[iResult],
                       results.RearRideHeights[iResult],
                       results.LateralForces[iResult],
                       results.InertiaEfficiencies[iResult],
                       results.LongitudinalForces[iResult],
                       results.FrontWheelsLongiudinalForces[iResult],
                       results.RearWheelsLongiudinalForces[iResult],
                       results.FrontWheelsTorques[iResult],
                       results.RearWheelsTorques[iResult],
                       results.FrontWheelsAngularSpeeds[iResult],
                       results.RearWheelsAngularSpeeds[iResult],
                       results.EnginePowers[iResult],
                       results.EngineAvailablePowers[iResult],
                       results.EnginePowerUsages[iResult],
                       results.FrontBrakesPowers[iResult],
                       results.RearBrakesPowers[iResult],
                       results.FrontBrakesAvailablePowers[iResult],
                       results.RearBrakesAvailablePowers[iResult],
                       results.FrontBrakesUsages[iResult],
                       results.RearBrakesUsages[iResult],
                       results.FuelConsumptions[iResult],
                       results.CoordinatesX[iResult],
                       results.CoordinatesY[iResult]
                       );
                    ResultsDisplayCollection.Add(resultPoint);
                }
                LapTime = results.LapTime;
                FuelConsumption = results.TotalFuelConsumption * 1e6;
                AmountOfGearShifts = results.AmountOfGearShifts;
            }
        }
        #endregion
    }
    /// <summary>
    /// Contanis the information of one lap time simulation result point.
    /// </summary>
    public class LapTimeSimulationResultPoint
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
        /// <summary>
        /// Aerodynamic Drag Coefficient
        /// </summary>
        public double DragCoefficient { get; set; }
        /// <summary>
        /// Aerodynamic Lift Coefficient
        /// </summary>
        public double LiftCoefficient { get; set; }
        /// <summary>
        /// Aerodynamic Roll Moment Coefficient
        /// </summary>
        public double DownforceDistribution { get; set; }
        /// <summary>
        /// Aerodynamic Pitch Moment Coefficient
        /// </summary>
        public double DragForce { get; set; }
        /// <summary>
        /// Aerodynamic Side Force [N]
        /// </summary>
        public double LiftForce { get; set; }
        /// <summary>
        /// Aerodynamic Roll Moment [Nm]
        /// </summary>
        public double FrontLiftForce { get; set; }
        /// <summary>
        /// Aerodynamic Yaw Moment Coefficient
        /// </summary>
        public double RearLiftForce { get; set; }
        /// <summary>
        /// Aerodynamic Drag Force [N]
        /// </summary>
        public double TotalVerticalLoad { get; set; }
        /// <summary>
        /// Total longitudinal load transfer [N]
        /// </summary>
        public double TotalLongitudinalLoadTransfer { get; set; }
        /// <summary>
        /// Longitudinal load transfer due to unsprung mass [N]
        /// </summary>
        public double UnsprungLongitudinalLoadTransfers { get; set; }
        /// <summary>
        /// Longitudinal load transfer due to sprung mass [N]
        /// </summary>
        public double SprungLongitudinalLoadTransfers { get; set; }
        /// <summary>
        /// Front wheel load [N]
        /// </summary>
        public double FrontWheelLoad { get; set; }
        /// <summary>
        /// Rear wheel load [N]
        /// </summary>
        public double RearWheelLoad { get; set; }
        /// <summary>
        /// Front wheel radius [mm]
        /// </summary>
        public double FrontWheelRadius { get; set; }
        /// <summary>
        /// Rear wheel radius [mm]
        /// </summary>
        public double RearWheelRadius { get; set; }
        /// <summary>
        /// Front suspension deflection [mm]
        /// </summary>
        public double FrontSuspensionDeflection { get; set; }
        /// <summary>
        /// Rear suspension deflection [mm]
        /// </summary>
        public double RearSuspensionDeflection { get; set; }
        /// <summary>
        /// Front ride height [mm]
        /// </summary>
        public double FrontRideHeight { get; set; }
        /// <summary>
        /// Rear ride height [mm]
        /// </summary>
        public double RearRideHeight { get; set; }
        /// <summary>
        /// Lateral Force [N]
        /// </summary>
        public double LateralForce { get; set; }
        /// <summary>
        /// Inertia efficiency [%]
        /// </summary>
        public double InertiaEfficiency { get; set; }
        /// <summary>
        /// Longitudinal Force [N]
        /// </summary>
        public double LongitudinalForce { get; set; }
        /// <summary>
        /// Front wheel's longitudinal force [N]
        /// </summary>
        public double FrontWheelLongitudinalForce { get; set; }
        /// <summary>
        /// Rear wheel's longitudinal force [N]
        /// </summary>
        public double RearWheelLongitudinalForce { get; set; }
        /// <summary>
        /// Front wheel's torque [Nm]
        /// </summary>
        public double FrontWheelTorque { get; set; }
        /// <summary>
        /// Rear wheel's torque [Nm]
        /// </summary>
        public double RearWheelTorque { get; set; }
        /// <summary>
        /// Front wheel's angular speed [rpm]
        /// </summary>
        public double FrontWheelAngularSpeed { get; set; }
        /// <summary>
        /// Rear wheel's angular speed [rpm]
        /// </summary>
        public double RearWheelAngularSpeed { get; set; }
        /// <summary>
        /// Engine's Power [hp]
        /// </summary>
        public double EnginePower { get; set; }
        /// <summary>
        /// Engine's available power [hp]
        /// </summary>
        public double EngineAvailablePower { get; set; }
        /// <summary>
        /// Engine's power usage [%]
        /// </summary>
        public double EnginePowerUsage { get; set; }
        /// <summary>
        /// Front brakes power [kW]
        /// </summary>
        public double FrontBrakesPower { get; set; }
        /// <summary>
        /// Rear brakes power [kW]
        /// </summary>
        public double RearBrakesPower { get; set; }
        /// <summary>
        /// Front brakes available power [kW]
        /// </summary>
        public double FrontBrakesAvailablePower { get; set; }
        /// <summary>
        /// Rear brakes available power [kW]
        /// </summary>
        public double RearBrakesAvailablePower { get; set; }
        /// <summary>
        /// Front brakes usage [%]
        /// </summary>
        public double FrontBrakesUsage { get; set; }
        /// <summary>
        /// Rear brakes usage [%]
        /// </summary>
        public double RearBrakesUsage { get; set; }
        /// <summary>
        /// Fuel consumption [mL]
        /// </summary>
        public double FuelConsumption { get; set; }
        /// <summary>
        /// Path's coordinates X [m]
        /// </summary>
        public double CoordinatesX { get; set; }
        /// <summary>
        /// Path's coordinates Y [m]
        /// </summary>
        public double CoordinatesY { get; set; }
        #endregion
        #region Constructors
        public LapTimeSimulationResultPoint(double elapsedTime, double elapsedDistance, double speed,
            double longitudinalAcceleration, double lateralAcceleration, int gearNumber, double dragCoefficient, double liftCoefficient, double downforceDistribution, double dragForce, double liftForce, double frontLiftForce, double rearLiftForce, double totalVerticalLoad, double totalLongitudinalLoadTransfer, double unsprungLoadTransfer, double sprungLoadTransfer, double frontWheelLoad, double rearWheelLoad, double frontWheelRadius, double rearWheelRadius, double frontSuspensionDeflection, double rearSuspensionDeflection, double frontRideHeight, double rearRideHeight, double lateralForce, double inertiaEfficiency, double longitudinalForce, double frontWheelLongitudinalForce, double rearWheelLongitudinalForce, double frontWheelTorque, double rearWheelTorque, double frontWheelAngularSpeed, double rearWheelAngularSpeed, double enginePower, double engineAvailablePower, double enginePowerUsage, double frontBrakesPower, double rearBrakesPower, double frontBrakesAvailablePower, double rearBrakesAvailablePower, double frontBrakesUsage, double rearBrakesUsage, double fuelConsumption, double coordinatesX, double coordinatesY)
        {
            ElapsedTime = elapsedTime;
            ElapsedDistance = elapsedDistance;
            Speed = speed * 3.6;
            LongitudinalAcceleration = longitudinalAcceleration;
            LateralAcceleration = lateralAcceleration;
            GearNumber = gearNumber;
            DragCoefficient = dragCoefficient;
            LiftCoefficient = liftCoefficient;
            DownforceDistribution = downforceDistribution * 100;
            DragForce = dragForce;
            LiftForce = liftForce;
            FrontLiftForce = frontLiftForce;
            RearLiftForce = rearLiftForce;
            TotalVerticalLoad = totalVerticalLoad;
            TotalLongitudinalLoadTransfer = totalLongitudinalLoadTransfer;
            UnsprungLongitudinalLoadTransfers = unsprungLoadTransfer;
            SprungLongitudinalLoadTransfers = sprungLoadTransfer;
            FrontWheelLoad = frontWheelLoad;
            RearWheelLoad = rearWheelLoad;
            FrontWheelRadius = frontWheelRadius * 1000;
            RearWheelRadius = rearWheelRadius * 1000;
            FrontSuspensionDeflection = frontSuspensionDeflection * 1000;
            RearSuspensionDeflection = rearSuspensionDeflection * 1000;
            FrontRideHeight = frontRideHeight * 1000;
            RearRideHeight = rearRideHeight * 1000;
            LateralForce = lateralForce;
            InertiaEfficiency = inertiaEfficiency * 100;
            LongitudinalForce = longitudinalForce;
            FrontWheelLongitudinalForce = frontWheelLongitudinalForce;
            RearWheelLongitudinalForce = rearWheelLongitudinalForce;
            FrontWheelTorque = frontWheelTorque;
            RearWheelTorque = rearWheelTorque;
            FrontWheelAngularSpeed = frontWheelAngularSpeed * 30 / Math.PI;
            RearWheelAngularSpeed = rearWheelAngularSpeed * 30 / Math.PI;
            EnginePower = enginePower / 745.7;
            EngineAvailablePower = engineAvailablePower / 745.7;
            EnginePowerUsage = enginePowerUsage * 100;
            FrontBrakesPower = frontBrakesPower / 1000;
            RearBrakesPower = rearBrakesPower / 1000;
            FrontBrakesAvailablePower = frontBrakesAvailablePower / 1000;
            RearBrakesAvailablePower = rearBrakesAvailablePower / 1000;
            FrontBrakesUsage = frontBrakesUsage * 100;
            RearBrakesUsage = rearBrakesUsage * 100;
            FuelConsumption = fuelConsumption * 1e6;
            CoordinatesX = coordinatesX;
            CoordinatesY = coordinatesY;
        }
        #endregion
    }
}
