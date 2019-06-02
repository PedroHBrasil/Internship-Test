using Syncfusion.UI.Xaml.Charts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace InternshipTest.UIClasses.ResultsAnalysis
{
    [Serializable]
    public class LapTimeSimulation2DChart : SfChart
    {
        #region Enums
        public enum ResultTypes { Time, Distance, Speed, LongitudinalAcceleration, LateralAcceleration, Gear }
        public enum LineTypes { Line, Scatter }
        #endregion
        #region Fields
        public FastLineSeries currentFastLine;
        public FastScatterBitmapSeries currentFastScatter;
        #endregion
        #region Properties
        public ResultTypes ResultTypeX { get; set; }
        public ResultTypes ResultTypeY { get; set; }
        public LineTypes LineType { get; set; }
        #endregion
        #region Constructors
        public LapTimeSimulation2DChart() { }
        public LapTimeSimulation2DChart(string resultTypeX, string resultTypeY, string lineType)
        {
            ResultTypeX = _GetResultTypeFromString(resultTypeX);
            ResultTypeY = _GetResultTypeFromString(resultTypeY);
            LineType = _GetLineTypeFromString(lineType);
        }
        #endregion
        #region Methods
        #region Initialization
        public void InitializeChart()
        {
            Legend = new ChartLegend();
            PrimaryAxis = new NumericalAxis();
            SecondaryAxis = new NumericalAxis();
            Header = "Lap Time Simulation Analysis: 2D Chart";
            FontSize = 20;
            FontWeight = FontWeights.Bold;
            Margin = new Thickness(10);
        }
        #endregion
        #region Types to Strings Association
        /// <summary>
        /// Gets the result type based on a string.
        /// </summary>
        /// <param name="resultTypeString"> String to extract the result type from. </param>
        /// <returns> The associated result type. </returns>
        private ResultTypes _GetResultTypeFromString(string resultTypeString)
        {
            ResultTypes resultType = 0;
            switch (resultTypeString)
            {
                case "Time":
                    resultType = ResultTypes.Time;
                    break;
                case "Distance":
                    resultType = ResultTypes.Distance;
                    break;
                case "Speed":
                    resultType = ResultTypes.Speed;
                    break;
                case "Longitudinal Acceleration":
                    resultType = ResultTypes.LongitudinalAcceleration;
                    break;
                case "Lateral Acceleration":
                    resultType = ResultTypes.LateralAcceleration;
                    break;
                case "Gear":
                    resultType = ResultTypes.Gear;
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
        private LineTypes _GetLineTypeFromString(string lineTypeString)
        {
            LineTypes lineType = 0;
            switch (lineTypeString)
            {
                case "Line":
                    lineType = LineTypes.Line;
                    break;
                case "Scatter":
                    lineType = LineTypes.Scatter;
                    break;
                default:
                    break;
            }
            return lineType;
        }
        #endregion
        #region Data Manipulation
        /// <summary>
        /// Adds the results data to the chart.
        /// </summary>
        /// <param name="lapTimeSimulationResultsViewModel"> Lap time simulation results view model. </param>
        public void AddDataToChart(Results.LapTimeSimulationResultsViewModel lapTimeSimulationResultsViewModel)
        {
            currentFastLine = new FastLineSeries() { ItemsSource = lapTimeSimulationResultsViewModel.ResultsDisplayCollection };
            currentFastScatter = new FastScatterBitmapSeries() { ItemsSource = lapTimeSimulationResultsViewModel.ResultsDisplayCollection };
            _AddXDataToSeries();
            _AddYDataToSeries();
            _AddSeriesDataToChart();
        }
        /// <summary>
        /// Adds the data of the X axis to the series.
        /// </summary>
        private void _AddXDataToSeries()
        {
            switch (ResultTypeX)
            {
                case ResultTypes.Time:
                    PrimaryAxis.Header = "Time [s]";
                    PrimaryAxis.LabelFormat = "N2";
                    currentFastLine.XBindingPath = "ElapsedTime";
                    currentFastScatter.XBindingPath = "ElapsedTime";
                    break;
                case ResultTypes.Distance:
                    PrimaryAxis.Header = "Distance [m]";
                    PrimaryAxis.LabelFormat = "N2";
                    currentFastLine.XBindingPath = "ElapsedDistance";
                    currentFastScatter.XBindingPath = "ElapsedDistance";
                    break;
                case ResultTypes.Speed:
                    PrimaryAxis.Header = "Speed [km/h]";
                    PrimaryAxis.LabelFormat = "N2";
                    currentFastLine.XBindingPath = "Speed";
                    currentFastScatter.XBindingPath = "Speed";
                    break;
                case ResultTypes.LongitudinalAcceleration:
                    PrimaryAxis.Header = "Longitudinal Acceleration [m/s²]";
                    PrimaryAxis.LabelFormat = "N2";
                    currentFastLine.XBindingPath = "LongitudinalAcceleration";
                    currentFastScatter.XBindingPath = "LongitudinalAcceleration";
                    break;
                case ResultTypes.LateralAcceleration:
                    PrimaryAxis.Header = "Lateral Acceleration [m/s²]";
                    PrimaryAxis.LabelFormat = "N2";
                    currentFastLine.XBindingPath = "LateralAcceleration";
                    currentFastScatter.XBindingPath = "LateralAcceleration";
                    break;
                case ResultTypes.Gear:
                    PrimaryAxis.Header = "Gear Number";
                    PrimaryAxis.LabelFormat = "N0";
                    currentFastLine.XBindingPath = "GearNumber";
                    currentFastScatter.XBindingPath = "GearNumber";
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// Adds the data of the Y axis to the series.
        /// </summary>
        private void _AddYDataToSeries()
        {
            switch (ResultTypeY)
            {
                case ResultTypes.Time:
                    SecondaryAxis.Header = "Time [s]";
                    SecondaryAxis.LabelFormat = "N2";
                    currentFastLine.YBindingPath = "ElapsedTime";
                    currentFastScatter.YBindingPath = "ElapsedTime";
                    break;
                case ResultTypes.Distance:
                    SecondaryAxis.Header = "Distance [m]";
                    SecondaryAxis.LabelFormat = "N2";
                    currentFastLine.YBindingPath = "ElapsedDistance";
                    currentFastScatter.YBindingPath = "ElapsedDistance";
                    break;
                case ResultTypes.Speed:
                    SecondaryAxis.Header = "Speed [km/h]";
                    SecondaryAxis.LabelFormat = "N2";
                    currentFastLine.YBindingPath = "Speed";
                    currentFastScatter.YBindingPath = "Speed";
                    break;
                case ResultTypes.LongitudinalAcceleration:
                    SecondaryAxis.Header = "Longitudinal Acceleration [m/s²]";
                    SecondaryAxis.LabelFormat = "N2";
                    currentFastLine.YBindingPath = "LongitudinalAcceleration";
                    currentFastScatter.YBindingPath = "LongitudinalAcceleration";
                    break;
                case ResultTypes.LateralAcceleration:
                    SecondaryAxis.Header = "Lateral Acceleration [m/s²]";
                    SecondaryAxis.LabelFormat = "N2";
                    currentFastLine.YBindingPath = "LateralAcceleration";
                    currentFastScatter.YBindingPath = "LateralAcceleration";
                    break;
                case ResultTypes.Gear:
                    SecondaryAxis.Header = "Gear Number";
                    SecondaryAxis.LabelFormat = "N0";
                    currentFastLine.YBindingPath = "GearNumber";
                    currentFastScatter.YBindingPath = "GearNumber";
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// Adds the generated series to the chart.
        /// </summary>
        private void _AddSeriesDataToChart()
        {
            switch (LineType)
            {
                case LineTypes.Line:
                    Series.Add(currentFastLine);
                    // Adds a trackball to the chart
                    ChartTrackBallBehavior trackBall = new ChartTrackBallBehavior();
                    Behaviors.Add(trackBall);
                    break;
                case LineTypes.Scatter:
                    Series.Add(currentFastScatter);
                    break;
                default:
                    break;
            }
        }
        #endregion

        #endregion
    }
}
