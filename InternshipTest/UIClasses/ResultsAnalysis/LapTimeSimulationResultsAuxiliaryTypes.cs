using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternshipTest.UIClasses.ResultsAnalysis
{
    public class LapTimeSimulationResultsAuxiliaryTypes
    {
        #region enums
        public enum ResultTypes
        {
            AerodynamicDragCoefficient,
            AerodynamicLiftCoefficient,
            AerodynamicDownforceDistribution,
            AerodynamicDragForce,
            AerodynamicLiftForce,
            AerodynamicFrontLiftForce,
            AerodynamicRearLiftForce,
            Distance,
            EngineAvailablePower,
            EnginePower,
            EnginePowerUsage,
            FuelConsumption,
            Gear,
            InertiaEfficiency,
            LateralAcceleration,
            LateralForce,
            LongitudinalAcceleration,
            LongitudinalForce,
            LongitudinalLoadTransferSprung,
            LongitudinalLoadTransferTotal,
            LongitudinalLoadTransferUnsprung,
            FrontBrakesAvailablePower,
            FrontBrakesPower,
            FrontBrakesPowerUsage,
            FrontRideHeight,
            FrontSuspensionDeflection,
            FrontWheelAngularSpeed,
            FrontWheelLoad,
            FrontWheelLongitudinalForce,
            FrontWheelRadius,
            FrontWheelTorque,
            RearBrakesAvailablePower,
            RearBrakesPower,
            RearBrakesPowerUsage,
            RearRideHeight,
            RearSuspensionDeflection,
            RearWheelAngularSpeed,
            RearWheelLoad,
            RearWheelLongitudinalForce,
            RearWheelRadius,
            RearWheelTorque,
            Speed,
            Time,
            VerticalLoadTotal
        }
        public enum LineTypes { Line, Scatter }
        #endregion
        #region Types to Strings Association
        /// <summary>
        /// Gets the result type based on a string.
        /// </summary>
        /// <param name="resultTypeString"> String to extract the result type from. </param>
        /// <returns> The associated result type. </returns>
        public static ResultTypes GetResultTypeFromString(string resultTypeString)
        {
            LapTimeSimulationResultsAuxiliaryTypes.ResultTypes resultType = 0;
            switch (resultTypeString)
            {
                case "Aerodynamic Drag Coefficient":
                    resultType = LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.AerodynamicDragCoefficient;
                    break;
                case "Aerodynamic Lift Coefficient":
                    resultType = LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.AerodynamicLiftCoefficient;
                    break;
                case "Downforce Distribution":
                    resultType = LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.AerodynamicDownforceDistribution;
                    break;
                case "Aerodynamic Drag Force":
                    resultType = LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.AerodynamicDragForce;
                    break;
                case "Aerodynamic Lift Force":
                    resultType = LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.AerodynamicLiftForce;
                    break;
                case "Aerodynamic Front Lift Force":
                    resultType = LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.AerodynamicFrontLiftForce;
                    break;
                case "Aerodynamic Rear Lift Force":
                    resultType = LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.AerodynamicRearLiftForce;
                    break;
                case "Distance":
                    resultType = LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.Distance;
                    break;
                case "Engine Available Power":
                    resultType = LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.EngineAvailablePower;
                    break;
                case "Engine Power":
                    resultType = LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.EnginePower;
                    break;
                case "Engine Power Usage":
                    resultType = LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.EnginePowerUsage;
                    break;
                case "Fuel Consumption":
                    resultType = LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.FuelConsumption;
                    break;
                case "Gear":
                    resultType = LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.Gear;
                    break;
                case "Inertia Efficiency":
                    resultType = LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.InertiaEfficiency;
                    break;
                case "Lateral Acceleration":
                    resultType = LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.LateralAcceleration;
                    break;
                case "Lateral Force":
                    resultType = LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.LateralForce;
                    break;
                case "Longitudinal Acceleration":
                    resultType = LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.LongitudinalAcceleration;
                    break;
                case "Longitudinal Force":
                    resultType = LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.LongitudinalForce;
                    break;
                case "Longitudinal Load Transfer - Sprung":
                    resultType = LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.LongitudinalLoadTransferSprung;
                    break;
                case "Longitudinal Load Transfer - Total":
                    resultType = LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.LongitudinalLoadTransferTotal;
                    break;
                case "Longitudinal Load Transfer - Unsprung":
                    resultType = LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.LongitudinalLoadTransferSprung;
                    break;
                case "Front Brakes Available Power":
                    resultType = LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.FrontBrakesAvailablePower;
                    break;
                case "Front Brakes Power":
                    resultType = LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.FrontBrakesPower;
                    break;
                case "Front Brakes Power Usage":
                    resultType = LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.FrontBrakesPowerUsage;
                    break;
                case "Front Ride Height":
                    resultType = LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.FrontRideHeight;
                    break;
                case "Front Suspension Deflection":
                    resultType = LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.FrontSuspensionDeflection;
                    break;
                case "Front Wheel Angular Speed":
                    resultType = LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.FrontWheelAngularSpeed;
                    break;
                case "Front Wheel Load":
                    resultType = LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.FrontWheelLoad;
                    break;
                case "Front Wheel Longitudinal Force":
                    resultType = LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.FrontWheelLongitudinalForce;
                    break;
                case "Front Wheel Radius":
                    resultType = LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.FrontWheelRadius;
                    break;
                case "Front Wheel Torque":
                    resultType = LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.FrontWheelTorque;
                    break;
                case "Rear Brakes Available Power":
                    resultType = LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.RearBrakesAvailablePower;
                    break;
                case "Rear Brakes Power":
                    resultType = LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.RearBrakesPower;
                    break;
                case "Rear Brakes Power Usage":
                    resultType = LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.RearBrakesPowerUsage;
                    break;
                case "Rear Ride Height":
                    resultType = LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.RearRideHeight;
                    break;
                case "Rear Suspension Deflection":
                    resultType = LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.RearSuspensionDeflection;
                    break;
                case "Rear Wheel Angular Speed":
                    resultType = LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.RearWheelAngularSpeed;
                    break;
                case "Rear Wheel Load":
                    resultType = LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.RearWheelLoad;
                    break;
                case "Rear Wheel Longitudinal Force":
                    resultType = LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.RearWheelLongitudinalForce;
                    break;
                case "Rear Wheel Radius":
                    resultType = LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.RearWheelRadius;
                    break;
                case "Rear Wheel Torque":
                    resultType = LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.RearWheelTorque;
                    break;
                case "Speed":
                    resultType = LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.Speed;
                    break;
                case "Time":
                    resultType = LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.Time;
                    break;
                case "Vertical Load - Total":
                    resultType = LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.VerticalLoadTotal;
                    break;
                default:
                    break;
            }
            return resultType;
        }
        /// <summary>
        /// Gets the line type based on a string.
        /// </summary>
        /// <param name="lineTypeString"> String to extract the line type from. </param>
        /// <returns> The associated line type. </returns>
        public static LineTypes GetLineTypeFromString(string lineTypeString)
        {
            LapTimeSimulationResultsAuxiliaryTypes.LineTypes lineType = 0;
            switch (lineTypeString)
            {
                case "Line":
                    lineType = LapTimeSimulationResultsAuxiliaryTypes.LineTypes.Line;
                    break;
                case "Scatter":
                    lineType = LapTimeSimulationResultsAuxiliaryTypes.LineTypes.Scatter;
                    break;
                default:
                    break;
            }
            return lineType;
        }
        #endregion
    }
}
