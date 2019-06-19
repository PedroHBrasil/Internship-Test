using Syncfusion.UI.Xaml.Charts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace InternshipTest.UIClasses.ResultsAnalysis
{
    public class LapTimeSimulation2DChartParameters
    {
        #region Enums
        #endregion
        #region Fields
        public FastLineSeries currentFastLine;
        public FastScatterBitmapSeries currentFastScatter;
        #endregion
        #region Properties
        public string ResultTypeXString { get; set; }
        public string ResultTypeYString { get; set; }
        public string LineTypeString { get; set; }
        public LapTimeSimulationResultsAuxiliaryTypes.ResultTypes ResultTypeX { get; private set; }
        public LapTimeSimulationResultsAuxiliaryTypes.ResultTypes ResultTypeY { get; private set; }
        public LapTimeSimulationResultsAuxiliaryTypes.LineTypes LineType { get; private set; }
        #endregion
        #region Constructors
        public LapTimeSimulation2DChartParameters() { }
        public LapTimeSimulation2DChartParameters(string resultTypeX, string resultTypeY, string lineType)
        {
            ResultTypeXString = resultTypeX;
            ResultTypeX = LapTimeSimulationResultsAuxiliaryTypes.GetResultTypeFromString(resultTypeX);
            ResultTypeYString = resultTypeY;
            ResultTypeY = LapTimeSimulationResultsAuxiliaryTypes.GetResultTypeFromString(resultTypeY);
            LineTypeString = lineType;
            LineType = LapTimeSimulationResultsAuxiliaryTypes.GetLineTypeFromString(lineType);
        }
        #endregion
        #region Methods
        #region Initialization
        public SfChart InitializeChart()
        {
            return new SfChart
            {
                Legend = new ChartLegend(),
                PrimaryAxis = new NumericalAxis(),
                SecondaryAxis = new NumericalAxis(),
                Header = "Lap Time Simulation Analysis: 2D Chart",
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(10)
            };
        }
        #endregion
        #region Data Manipulation
        /// <summary>
        /// Adds the results data to the chart.
        /// </summary>
        /// <param name="lapTimeSimulationResultsViewModel"> Lap time simulation results view model. </param>
        public SfChart AddDataToChart(SfChart chart, string dataLabel, Results.LapTimeSimulationResultsViewModel lapTimeSimulationResultsViewModel)
        {
            currentFastLine = new FastLineSeries() { ItemsSource = lapTimeSimulationResultsViewModel.ResultsDisplayCollection, Label = lapTimeSimulationResultsViewModel.ID};
            currentFastScatter = new FastScatterBitmapSeries() { ItemsSource = lapTimeSimulationResultsViewModel.ResultsDisplayCollection, Label = lapTimeSimulationResultsViewModel.ID };
            chart = _AddXDataToSeries(chart);
            chart = _AddYDataToSeries(chart);
            chart = _AddSeriesDataToChart(chart);
            return chart;
        }
        /// <summary>
        /// Adds the data of the X axis to the series.
        /// </summary>
        private SfChart _AddXDataToSeries(SfChart chart)
        {
            switch (ResultTypeX)
            {
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.AerodynamicDragCoefficient:
                    chart.PrimaryAxis.Header = "Aerodynamic Drag Cofficient";
                    chart.PrimaryAxis.LabelFormat = "N3";
                    currentFastLine.XBindingPath = "DragCoefficient";
                    currentFastScatter.XBindingPath = "DragCoefficient";
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.AerodynamicLiftCoefficient:
                    chart.PrimaryAxis.Header = "Aerodynamic Lift Cofficient";
                    chart.PrimaryAxis.LabelFormat = "N3";
                    currentFastLine.XBindingPath = "LiftCoefficient";
                    currentFastScatter.XBindingPath = "LiftCoefficient";
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.AerodynamicDownforceDistribution:
                    chart.PrimaryAxis.Header = "Aerodynamic Downforce Distribution [%]";
                    chart.PrimaryAxis.LabelFormat = "N3";
                    currentFastLine.XBindingPath = "DownforceDistribution";
                    currentFastScatter.XBindingPath = "DownforceDistribution";
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.AerodynamicDragForce:
                    chart.PrimaryAxis.Header = "Aerodynamic Drag Force [N]";
                    chart.PrimaryAxis.LabelFormat = "N2";
                    currentFastLine.XBindingPath = "DragForce";
                    currentFastScatter.XBindingPath = "DragForce";
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.AerodynamicLiftForce:
                    chart.PrimaryAxis.Header = "Aerodynamic Lift Force [N]";
                    chart.PrimaryAxis.LabelFormat = "N2";
                    currentFastLine.XBindingPath = "LiftForce";
                    currentFastScatter.XBindingPath = "LiftForce";
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.AerodynamicFrontLiftForce:
                    chart.PrimaryAxis.Header = "Aerodynamic Front Lift Force [N]";
                    chart.PrimaryAxis.LabelFormat = "N2";
                    currentFastLine.XBindingPath = "FrontLiftForce";
                    currentFastScatter.XBindingPath = "FrontLiftForce";
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.AerodynamicRearLiftForce:
                    chart.PrimaryAxis.Header = "Aerodynamic Rear Lift Force [N]";
                    chart.PrimaryAxis.LabelFormat = "N2";
                    currentFastLine.XBindingPath = "RearLiftForce";
                    currentFastScatter.XBindingPath = "RearLiftForce";
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.Distance:
                    chart.PrimaryAxis.Header = "Distance [m]";
                    chart.PrimaryAxis.LabelFormat = "N2";
                    currentFastLine.XBindingPath = "ElapsedDistance";
                    currentFastScatter.XBindingPath = "ElapsedDistance";
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.EngineAvailablePower:
                    chart.PrimaryAxis.Header = "Engine Available Power [hp]";
                    chart.PrimaryAxis.LabelFormat = "N2";
                    currentFastLine.XBindingPath = "EngineAvailablePower";
                    currentFastScatter.XBindingPath = "EngineAvailablePower";
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.EnginePower:
                    chart.PrimaryAxis.Header = "Engine Power [hp]";
                    chart.PrimaryAxis.LabelFormat = "N2";
                    currentFastLine.XBindingPath = "EnginePower";
                    currentFastScatter.XBindingPath = "EnginePower";
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.EnginePowerUsage:
                    chart.PrimaryAxis.Header = "Engine Power Usage [%]";
                    chart.PrimaryAxis.LabelFormat = "N2";
                    currentFastLine.XBindingPath = "EnginePowerUsage";
                    currentFastScatter.XBindingPath = "EnginePowerUsage";
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.FuelConsumption:
                    chart.PrimaryAxis.Header = "Fuel Consumption [mL]";
                    chart.PrimaryAxis.LabelFormat = "N2";
                    currentFastLine.XBindingPath = "FuelConsumption";
                    currentFastScatter.XBindingPath = "FuelConsumption";
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.Gear:
                    chart.PrimaryAxis.Header = "Gear Number";
                    chart.PrimaryAxis.LabelFormat = "N0";
                    currentFastLine.XBindingPath = "GearNumber";
                    currentFastScatter.XBindingPath = "GearNumber";
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.InertiaEfficiency:
                    chart.PrimaryAxis.Header = "Inertia Efficiency [%]";
                    chart.PrimaryAxis.LabelFormat = "N2";
                    currentFastLine.XBindingPath = "InertiaEfficiency";
                    currentFastScatter.XBindingPath = "InertiaEfficiency";
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.LateralAcceleration:
                    chart.PrimaryAxis.Header = "Lateral Acceleration [m/s²]";
                    chart.PrimaryAxis.LabelFormat = "N2";
                    currentFastLine.XBindingPath = "LateralAcceleration";
                    currentFastScatter.XBindingPath = "LateralAcceleration";
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.LateralForce:
                    chart.PrimaryAxis.Header = "Lateral Force [N]";
                    chart.PrimaryAxis.LabelFormat = "N2";
                    currentFastLine.XBindingPath = "LateralForce";
                    currentFastScatter.XBindingPath = "LateralForce";
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.LongitudinalAcceleration:
                    chart.PrimaryAxis.Header = "Longitudinal Acceleration [m/s²]";
                    chart.PrimaryAxis.LabelFormat = "N2";
                    currentFastLine.XBindingPath = "LongitudinalAcceleration";
                    currentFastScatter.XBindingPath = "LongitudinalAcceleration";
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.LongitudinalForce:
                    chart.PrimaryAxis.Header = "Longitudinal Force [N]";
                    chart.PrimaryAxis.LabelFormat = "N2";
                    currentFastLine.XBindingPath = "LongitudinalForce";
                    currentFastScatter.XBindingPath = "LongitudinalForce";
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.LongitudinalLoadTransferSprung:
                    chart.PrimaryAxis.Header = "Sprung Longitudinal Load Transfer [N]";
                    chart.PrimaryAxis.LabelFormat = "N2";
                    currentFastLine.XBindingPath = "SprungLongitudinalLoadTransfer";
                    currentFastScatter.XBindingPath = "SprungLongitudinalLoadTransfer";
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.LongitudinalLoadTransferTotal:
                    chart.PrimaryAxis.Header = "Total Longitudinal Load Transfer [N]";
                    chart.PrimaryAxis.LabelFormat = "N2";
                    currentFastLine.XBindingPath = "TotalLongitudinalLoadTransfer";
                    currentFastScatter.XBindingPath = "TotalLongitudinalLoadTransfer";
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.LongitudinalLoadTransferUnsprung:
                    chart.PrimaryAxis.Header = "Unsprung Longitudinal Load Transfer [N]";
                    chart.PrimaryAxis.LabelFormat = "N2";
                    currentFastLine.XBindingPath = "UnsprungLongitudinalLoadTransfer";
                    currentFastScatter.XBindingPath = "UnsprungLongitudinalLoadTransfer";
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.FrontBrakesAvailablePower:
                    chart.PrimaryAxis.Header = "Front Brakes Available Power [hp]";
                    chart.PrimaryAxis.LabelFormat = "N2";
                    currentFastLine.XBindingPath = "FrontBrakesAvailablePower";
                    currentFastScatter.XBindingPath = "FrontBrakesAvailablePower";
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.FrontBrakesPower:
                    chart.PrimaryAxis.Header = "Front Brakes Power [hp]";
                    chart.PrimaryAxis.LabelFormat = "N2";
                    currentFastLine.XBindingPath = "FrontBrakesPower";
                    currentFastScatter.XBindingPath = "FrontBrakesPower";
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.FrontBrakesPowerUsage:
                    chart.PrimaryAxis.Header = "Front Brakes Power Usage [%]";
                    chart.PrimaryAxis.LabelFormat = "N2";
                    currentFastLine.XBindingPath = "FrontBrakesPowerUsage";
                    currentFastScatter.XBindingPath = "FrontBrakesPowerUsage";
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.FrontRideHeight:
                    chart.PrimaryAxis.Header = "Front Ride Height [mm]";
                    chart.PrimaryAxis.LabelFormat = "N2";
                    currentFastLine.XBindingPath = "FrontRideHeight";
                    currentFastScatter.XBindingPath = "FrontRideHeight";
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.FrontSuspensionDeflection:
                    chart.PrimaryAxis.Header = "Front Suspension Deflection [mm]";
                    chart.PrimaryAxis.LabelFormat = "N2";
                    currentFastLine.XBindingPath = "FrontSuspensionDeflection";
                    currentFastScatter.XBindingPath = "FrontSuspensionDeflection";
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.FrontWheelAngularSpeed:
                    chart.PrimaryAxis.Header = "Front Angular Speed [rpm]";
                    chart.PrimaryAxis.LabelFormat = "N2";
                    currentFastLine.XBindingPath = "FrontWheelAngularSpeed";
                    currentFastScatter.XBindingPath = "FrontWheelAngularSpeed";
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.FrontWheelLoad:
                    chart.PrimaryAxis.Header = "Front Wheel Load [N]";
                    chart.PrimaryAxis.LabelFormat = "N2";
                    currentFastLine.XBindingPath = "FrontWheelLoad";
                    currentFastScatter.XBindingPath = "FrontWheelLoad";
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.FrontWheelLongitudinalForce:
                    chart.PrimaryAxis.Header = "Front Wheel Longitudinal Force [N]";
                    chart.PrimaryAxis.LabelFormat = "N2";
                    currentFastLine.XBindingPath = "FrontWheelLongitudinalForce";
                    currentFastScatter.XBindingPath = "FrontWheelLongitudinalForce";
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.FrontWheelRadius:
                    chart.PrimaryAxis.Header = "Front Wheel Radius [mm]";
                    chart.PrimaryAxis.LabelFormat = "N2";
                    currentFastLine.XBindingPath = "FrontWheelRadius";
                    currentFastScatter.XBindingPath = "FrontWheelRadius";
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.FrontWheelTorque:
                    chart.PrimaryAxis.Header = "Front Wheel Torque [Nm]";
                    chart.PrimaryAxis.LabelFormat = "N2";
                    currentFastLine.XBindingPath = "FrontWheelTorque";
                    currentFastScatter.XBindingPath = "FrontWheelTorque";
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.RearBrakesAvailablePower:
                    chart.PrimaryAxis.Header = "Rear Brakes Available Power [hp]";
                    chart.PrimaryAxis.LabelFormat = "N2";
                    currentFastLine.XBindingPath = "RearBrakesAvailablePower";
                    currentFastScatter.XBindingPath = "RearBrakesAvailablePower";
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.RearBrakesPower:
                    chart.PrimaryAxis.Header = "Rear Brakes Power [hp]";
                    chart.PrimaryAxis.LabelFormat = "N2";
                    currentFastLine.XBindingPath = "RearBrakesPower";
                    currentFastScatter.XBindingPath = "RearBrakesPower";
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.RearBrakesPowerUsage:
                    chart.PrimaryAxis.Header = "Rear Brakes Power Usage [%]";
                    chart.PrimaryAxis.LabelFormat = "N2";
                    currentFastLine.XBindingPath = "RearBrakesPowerUsage";
                    currentFastScatter.XBindingPath = "RearBrakesPowerUsage";
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.RearRideHeight:
                    chart.PrimaryAxis.Header = "Rear Ride Height [mm]";
                    chart.PrimaryAxis.LabelFormat = "N2";
                    currentFastLine.XBindingPath = "RearRideHeight";
                    currentFastScatter.XBindingPath = "RearRideHeight";
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.RearSuspensionDeflection:
                    chart.PrimaryAxis.Header = "Rear Suspension Deflection [mm]";
                    chart.PrimaryAxis.LabelFormat = "N2";
                    currentFastLine.XBindingPath = "RearSuspensionDeflection";
                    currentFastScatter.XBindingPath = "RearSuspensionDeflection";
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.RearWheelAngularSpeed:
                    chart.PrimaryAxis.Header = "Rear Angular Speed [rpm]";
                    chart.PrimaryAxis.LabelFormat = "N2";
                    currentFastLine.XBindingPath = "RearWheelAngularSpeed";
                    currentFastScatter.XBindingPath = "RearWheelAngularSpeed";
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.RearWheelLoad:
                    chart.PrimaryAxis.Header = "Rear Wheel Load [N]";
                    chart.PrimaryAxis.LabelFormat = "N2";
                    currentFastLine.XBindingPath = "RearWheelLoad";
                    currentFastScatter.XBindingPath = "RearWheelLoad";
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.RearWheelLongitudinalForce:
                    chart.PrimaryAxis.Header = "Rear Wheel Longitudinal Force [N]";
                    chart.PrimaryAxis.LabelFormat = "N2";
                    currentFastLine.XBindingPath = "RearWheelLongitudinalForce";
                    currentFastScatter.XBindingPath = "RearWheelLongitudinalForce";
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.RearWheelRadius:
                    chart.PrimaryAxis.Header = "Rear Wheel Radius [mm]";
                    chart.PrimaryAxis.LabelFormat = "N2";
                    currentFastLine.XBindingPath = "RearWheelRadius";
                    currentFastScatter.XBindingPath = "RearWheelRadius";
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.RearWheelTorque:
                    chart.PrimaryAxis.Header = "Rear Wheel Torque [Nm]";
                    chart.PrimaryAxis.LabelFormat = "N2";
                    currentFastLine.XBindingPath = "RearWheelTorque";
                    currentFastScatter.XBindingPath = "RearWheelTorque";
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.Speed:
                    chart.PrimaryAxis.Header = "Speed [km/h]";
                    chart.PrimaryAxis.LabelFormat = "N2";
                    currentFastLine.XBindingPath = "Speed";
                    currentFastScatter.XBindingPath = "Speed";
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.Time:
                    chart.PrimaryAxis.Header = "Time [s]";
                    chart.PrimaryAxis.LabelFormat = "N2";
                    currentFastLine.XBindingPath = "ElapsedTime";
                    currentFastScatter.XBindingPath = "ElapsedTime";
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.VerticalLoadTotal:
                    chart.PrimaryAxis.Header = "Total Vertical Load [N]";
                    chart.PrimaryAxis.LabelFormat = "N2";
                    currentFastLine.XBindingPath = "TotalVerticalLoad";
                    currentFastScatter.XBindingPath = "TotalVerticalLoad";
                    break;
                default:
                    break;
            }
            return chart;
        }
        /// <summary>
        /// Adds the data of the Y axis to the series.
        /// </summary>
        private SfChart _AddYDataToSeries(SfChart chart)
        {

            switch (ResultTypeY)
            {
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.AerodynamicDragCoefficient:
                    chart.SecondaryAxis.Header = "Aerodynamic Drag Cofficient";
                    chart.SecondaryAxis.LabelFormat = "N3";
                    currentFastLine.YBindingPath = "DragCoefficient";
                    currentFastScatter.YBindingPath = "DragCoefficient";
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.AerodynamicLiftCoefficient:
                    chart.SecondaryAxis.Header = "Aerodynamic Lift Cofficient";
                    chart.SecondaryAxis.LabelFormat = "N3";
                    currentFastLine.YBindingPath = "LiftCoefficient";
                    currentFastScatter.YBindingPath = "LiftCoefficient";
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.AerodynamicDownforceDistribution:
                    chart.SecondaryAxis.Header = "Aerodynamic Downforce Distribution [%]";
                    chart.SecondaryAxis.LabelFormat = "N3";
                    currentFastLine.YBindingPath = "DownforceDistribution";
                    currentFastScatter.YBindingPath = "DownforceDistribution";
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.AerodynamicDragForce:
                    chart.SecondaryAxis.Header = "Aerodynamic Drag Force [N]";
                    chart.SecondaryAxis.LabelFormat = "N2";
                    currentFastLine.YBindingPath = "DragForce";
                    currentFastScatter.YBindingPath = "DragForce";
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.AerodynamicLiftForce:
                    chart.SecondaryAxis.Header = "Aerodynamic Lift Force [N]";
                    chart.SecondaryAxis.LabelFormat = "N2";
                    currentFastLine.YBindingPath = "LiftForce";
                    currentFastScatter.YBindingPath = "LiftForce";
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.AerodynamicFrontLiftForce:
                    chart.SecondaryAxis.Header = "Aerodynamic Front Lift Force [N]";
                    chart.SecondaryAxis.LabelFormat = "N2";
                    currentFastLine.YBindingPath = "FrontLiftForce";
                    currentFastScatter.YBindingPath = "FrontLiftForce";
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.AerodynamicRearLiftForce:
                    chart.SecondaryAxis.Header = "Aerodynamic Rear Lift Force [N]";
                    chart.SecondaryAxis.LabelFormat = "N2";
                    currentFastLine.YBindingPath = "RearLiftForce";
                    currentFastScatter.YBindingPath = "RearLiftForce";
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.Distance:
                    chart.SecondaryAxis.Header = "Distance [m]";
                    chart.SecondaryAxis.LabelFormat = "N2";
                    currentFastLine.YBindingPath = "ElapsedDistance";
                    currentFastScatter.YBindingPath = "ElapsedDistance";
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.EngineAvailablePower:
                    chart.SecondaryAxis.Header = "Engine Available Power [hp]";
                    chart.SecondaryAxis.LabelFormat = "N2";
                    currentFastLine.YBindingPath = "EngineAvailablePower";
                    currentFastScatter.YBindingPath = "EngineAvailablePower";
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.EnginePower:
                    chart.SecondaryAxis.Header = "Engine Power [hp]";
                    chart.SecondaryAxis.LabelFormat = "N2";
                    currentFastLine.YBindingPath = "EnginePower";
                    currentFastScatter.YBindingPath = "EnginePower";
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.EnginePowerUsage:
                    chart.SecondaryAxis.Header = "Engine Power Usage [%]";
                    chart.SecondaryAxis.LabelFormat = "N2";
                    currentFastLine.YBindingPath = "EnginePowerUsage";
                    currentFastScatter.YBindingPath = "EnginePowerUsage";
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.FuelConsumption:
                    chart.SecondaryAxis.Header = "Fuel Consumption [mL]";
                    chart.SecondaryAxis.LabelFormat = "N2";
                    currentFastLine.YBindingPath = "FuelConsumption";
                    currentFastScatter.YBindingPath = "FuelConsumption";
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.Gear:
                    chart.SecondaryAxis.Header = "Gear Number";
                    chart.SecondaryAxis.LabelFormat = "N0";
                    currentFastLine.YBindingPath = "GearNumber";
                    currentFastScatter.YBindingPath = "GearNumber";
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.InertiaEfficiency:
                    chart.SecondaryAxis.Header = "Inertia Efficiency [%]";
                    chart.SecondaryAxis.LabelFormat = "N2";
                    currentFastLine.YBindingPath = "InertiaEfficiency";
                    currentFastScatter.YBindingPath = "InertiaEfficiency";
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.LateralAcceleration:
                    chart.SecondaryAxis.Header = "Lateral Acceleration [m/s²]";
                    chart.SecondaryAxis.LabelFormat = "N2";
                    currentFastLine.YBindingPath = "LateralAcceleration";
                    currentFastScatter.YBindingPath = "LateralAcceleration";
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.LateralForce:
                    chart.SecondaryAxis.Header = "Lateral Force [N]";
                    chart.SecondaryAxis.LabelFormat = "N2";
                    currentFastLine.YBindingPath = "LateralForce";
                    currentFastScatter.YBindingPath = "LateralForce";
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.LongitudinalAcceleration:
                    chart.SecondaryAxis.Header = "Longitudinal Acceleration [m/s²]";
                    chart.SecondaryAxis.LabelFormat = "N2";
                    currentFastLine.YBindingPath = "LongitudinalAcceleration";
                    currentFastScatter.YBindingPath = "LongitudinalAcceleration";
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.LongitudinalForce:
                    chart.SecondaryAxis.Header = "Longitudinal Force [N]";
                    chart.SecondaryAxis.LabelFormat = "N2";
                    currentFastLine.YBindingPath = "LongitudinalForce";
                    currentFastScatter.YBindingPath = "LongitudinalForce";
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.LongitudinalLoadTransferSprung:
                    chart.SecondaryAxis.Header = "Sprung Longitudinal Load Transfer [N]";
                    chart.SecondaryAxis.LabelFormat = "N2";
                    currentFastLine.YBindingPath = "SprungLongitudinalLoadTransfer";
                    currentFastScatter.YBindingPath = "SprungLongitudinalLoadTransfer";
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.LongitudinalLoadTransferTotal:
                    chart.SecondaryAxis.Header = "Total Longitudinal Load Transfer [N]";
                    chart.SecondaryAxis.LabelFormat = "N2";
                    currentFastLine.YBindingPath = "TotalLongitudinalLoadTransfer";
                    currentFastScatter.YBindingPath = "TotalLongitudinalLoadTransfer";
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.LongitudinalLoadTransferUnsprung:
                    chart.SecondaryAxis.Header = "Unsprung Longitudinal Load Transfer [N]";
                    chart.SecondaryAxis.LabelFormat = "N2";
                    currentFastLine.YBindingPath = "UnsprungLongitudinalLoadTransfer";
                    currentFastScatter.YBindingPath = "UnsprungLongitudinalLoadTransfer";
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.FrontBrakesAvailablePower:
                    chart.SecondaryAxis.Header = "Front Brakes Available Power [hp]";
                    chart.SecondaryAxis.LabelFormat = "N2";
                    currentFastLine.YBindingPath = "FrontBrakesAvailablePower";
                    currentFastScatter.YBindingPath = "FrontBrakesAvailablePower";
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.FrontBrakesPower:
                    chart.SecondaryAxis.Header = "Front Brakes Power [hp]";
                    chart.SecondaryAxis.LabelFormat = "N2";
                    currentFastLine.YBindingPath = "FrontBrakesPower";
                    currentFastScatter.YBindingPath = "FrontBrakesPower";
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.FrontBrakesPowerUsage:
                    chart.SecondaryAxis.Header = "Front Brakes Power Usage [%]";
                    chart.SecondaryAxis.LabelFormat = "N2";
                    currentFastLine.YBindingPath = "FrontBrakesPowerUsage";
                    currentFastScatter.YBindingPath = "FrontBrakesPowerUsage";
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.FrontRideHeight:
                    chart.SecondaryAxis.Header = "Front Ride Height [mm]";
                    chart.SecondaryAxis.LabelFormat = "N2";
                    currentFastLine.YBindingPath = "FrontRideHeight";
                    currentFastScatter.YBindingPath = "FrontRideHeight";
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.FrontSuspensionDeflection:
                    chart.SecondaryAxis.Header = "Front Suspension Deflection [mm]";
                    chart.SecondaryAxis.LabelFormat = "N2";
                    currentFastLine.YBindingPath = "FrontSuspensionDeflection";
                    currentFastScatter.YBindingPath = "FrontSuspensionDeflection";
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.FrontWheelAngularSpeed:
                    chart.SecondaryAxis.Header = "Front Angular Speed [rpm]";
                    chart.SecondaryAxis.LabelFormat = "N2";
                    currentFastLine.YBindingPath = "FrontWheelAngularSpeed";
                    currentFastScatter.YBindingPath = "FrontWheelAngularSpeed";
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.FrontWheelLoad:
                    chart.SecondaryAxis.Header = "Front Wheel Load [N]";
                    chart.SecondaryAxis.LabelFormat = "N2";
                    currentFastLine.YBindingPath = "FrontWheelLoad";
                    currentFastScatter.YBindingPath = "FrontWheelLoad";
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.FrontWheelLongitudinalForce:
                    chart.SecondaryAxis.Header = "Front Wheel Longitudinal Force [N]";
                    chart.SecondaryAxis.LabelFormat = "N2";
                    currentFastLine.YBindingPath = "FrontWheelLongitudinalForce";
                    currentFastScatter.YBindingPath = "FrontWheelLongitudinalForce";
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.FrontWheelRadius:
                    chart.SecondaryAxis.Header = "Front Wheel Radius [mm]";
                    chart.SecondaryAxis.LabelFormat = "N2";
                    currentFastLine.YBindingPath = "FrontWheelRadius";
                    currentFastScatter.YBindingPath = "FrontWheelRadius";
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.FrontWheelTorque:
                    chart.SecondaryAxis.Header = "Front Wheel Torque [Nm]";
                    chart.SecondaryAxis.LabelFormat = "N2";
                    currentFastLine.YBindingPath = "FrontWheelTorque";
                    currentFastScatter.YBindingPath = "FrontWheelTorque";
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.RearBrakesAvailablePower:
                    chart.SecondaryAxis.Header = "Rear Brakes Available Power [hp]";
                    chart.SecondaryAxis.LabelFormat = "N2";
                    currentFastLine.YBindingPath = "RearBrakesAvailablePower";
                    currentFastScatter.YBindingPath = "RearBrakesAvailablePower";
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.RearBrakesPower:
                    chart.SecondaryAxis.Header = "Rear Brakes Power [hp]";
                    chart.SecondaryAxis.LabelFormat = "N2";
                    currentFastLine.YBindingPath = "RearBrakesPower";
                    currentFastScatter.YBindingPath = "RearBrakesPower";
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.RearBrakesPowerUsage:
                    chart.SecondaryAxis.Header = "Rear Brakes Power Usage [%]";
                    chart.SecondaryAxis.LabelFormat = "N2";
                    currentFastLine.YBindingPath = "RearBrakesPowerUsage";
                    currentFastScatter.YBindingPath = "RearBrakesPowerUsage";
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.RearRideHeight:
                    chart.SecondaryAxis.Header = "Rear Ride Height [mm]";
                    chart.SecondaryAxis.LabelFormat = "N2";
                    currentFastLine.YBindingPath = "RearRideHeight";
                    currentFastScatter.YBindingPath = "RearRideHeight";
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.RearSuspensionDeflection:
                    chart.SecondaryAxis.Header = "Rear Suspension Deflection [mm]";
                    chart.SecondaryAxis.LabelFormat = "N2";
                    currentFastLine.YBindingPath = "RearSuspensionDeflection";
                    currentFastScatter.YBindingPath = "RearSuspensionDeflection";
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.RearWheelAngularSpeed:
                    chart.SecondaryAxis.Header = "Rear Angular Speed [rpm]";
                    chart.SecondaryAxis.LabelFormat = "N2";
                    currentFastLine.YBindingPath = "RearWheelAngularSpeed";
                    currentFastScatter.YBindingPath = "RearWheelAngularSpeed";
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.RearWheelLoad:
                    chart.SecondaryAxis.Header = "Rear Wheel Load [N]";
                    chart.SecondaryAxis.LabelFormat = "N2";
                    currentFastLine.YBindingPath = "RearWheelLoad";
                    currentFastScatter.YBindingPath = "RearWheelLoad";
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.RearWheelLongitudinalForce:
                    chart.SecondaryAxis.Header = "Rear Wheel Longitudinal Force [N]";
                    chart.SecondaryAxis.LabelFormat = "N2";
                    currentFastLine.YBindingPath = "RearWheelLongitudinalForce";
                    currentFastScatter.YBindingPath = "RearWheelLongitudinalForce";
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.RearWheelRadius:
                    chart.SecondaryAxis.Header = "Rear Wheel Radius [mm]";
                    chart.SecondaryAxis.LabelFormat = "N2";
                    currentFastLine.YBindingPath = "RearWheelRadius";
                    currentFastScatter.YBindingPath = "RearWheelRadius";
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.RearWheelTorque:
                    chart.SecondaryAxis.Header = "Rear Wheel Torque [Nm]";
                    chart.SecondaryAxis.LabelFormat = "N2";
                    currentFastLine.YBindingPath = "RearWheelTorque";
                    currentFastScatter.YBindingPath = "RearWheelTorque";
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.Speed:
                    chart.SecondaryAxis.Header = "Speed [km/h]";
                    chart.SecondaryAxis.LabelFormat = "N2";
                    currentFastLine.YBindingPath = "Speed";
                    currentFastScatter.YBindingPath = "Speed";
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.Time:
                    chart.SecondaryAxis.Header = "Time [s]";
                    chart.SecondaryAxis.LabelFormat = "N2";
                    currentFastLine.YBindingPath = "ElapsedTime";
                    currentFastScatter.YBindingPath = "ElapsedTime";
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.VerticalLoadTotal:
                    chart.SecondaryAxis.Header = "Total Vertical Load [N]";
                    chart.SecondaryAxis.LabelFormat = "N2";
                    currentFastLine.YBindingPath = "TotalVerticalLoad";
                    currentFastScatter.YBindingPath = "TotalVerticalLoad";
                    break;
                default:
                    break;
            }
            return chart;
        }
        /// <summary>
        /// Adds the generated series to the chart.
        /// </summary>
        private SfChart _AddSeriesDataToChart(SfChart chart)
        {
            switch (LineType)
            {
                case LapTimeSimulationResultsAuxiliaryTypes.LineTypes.Line:
                    chart.Series.Add(currentFastLine);
                    // Adds a trackball to the chart
                    ChartTrackBallBehavior trackBall = new ChartTrackBallBehavior();
                    chart.Behaviors.Add(trackBall);
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.LineTypes.Scatter:
                    chart.Series.Add(currentFastScatter);
                    break;
                default:
                    break;
            }
            return chart;
        }
        #endregion

        #endregion
    }
}
