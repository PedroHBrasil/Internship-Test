#region Copyright Syncfusion Inc. 2001-2019.
// Copyright Syncfusion Inc. 2001-2019. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Syncfusion.SfSkinManager;
using Syncfusion.UI.Xaml.Charts;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.Windows.Tools.Controls;
using MathNet.Numerics;
using System.Threading;
using System.ComponentModel;

namespace InternshipTest
{
    [Serializable]
    public enum CarModelType { OneWheel, TwoWheel }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        #region Fields
        private string currentVisualStyle;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the current visual style.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public string CurrentVisualStyle {
            get {
                return currentVisualStyle;
            }
            set {
                currentVisualStyle = value;
                _OnVisualStyleChanged();
            }
        }
        #endregion
        public MainWindow()
        {
            InitializeComponent();

            _PopulateFieldsWithLMP1();
        }
        /// <summary>
        /// Called when [loaded].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void _OnLoaded(object sender, RoutedEventArgs e)
        {
        }
        /// <summary>
        /// On Visual Style Changed.
        /// </summary>
        /// <remarks></remarks>
        private void _OnVisualStyleChanged()
        {
            VisualStyles visualStyle = VisualStyles.Default;
            Enum.TryParse(CurrentVisualStyle, out visualStyle);
            if (visualStyle != VisualStyles.Default)
            {
                SfSkinManager.ApplyStylesOnApplication = true;
                SfSkinManager.SetVisualStyle(this, visualStyle);
                SfSkinManager.ApplyStylesOnApplication = false;
            }
        }

        #region Sample Preparation
        /// <summary>
        /// Populates the UI fields with sample values.
        /// </summary>
        private void _PopulateFieldsWithFSAE()
        {
            // One Wheel Aerodynamic Map
            oneWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.OneWheelAerodynamicMapPoint(30, 1.1, -2.1));
            oneWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.OneWheelAerodynamicMapPoint(40, 1.2, -2.2));
            oneWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.OneWheelAerodynamicMapPoint(50, 1.3, -2.3));
            oneWheelAerodynamicMapIDTextBox.Text = "oneWheelAeroMap1";

            // Two Wheel Aerodynamic Map
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(30, 30, 1.1, -2.1, 50));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(40, 40, 1.2, -2.2, 50));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(50, 50, 1.3, -2.3, 50));
            twoWheelAerodynamicMapIDTextBox.Text = "twoWheelAeroMap1";

            // Aerodynamics
            aerodynamicsIDTextBox.Text = "aero1";
            frontalAreaTextBox.Text = "1.2";
            airDensityTextBox.Text = "1.3";

            // One Wheel Brakes
            oneWheelBrakesIDTextBox.Text = "oneWheelBrakes1";
            oneWheelBrakesMaximumTorqueTextBox.Text = "2000";

            // Two Wheel Brakes
            twoWheelBrakesIDTextBox.Text = "twoWheelBrakes1";
            twoWheelBrakesBrakeBiasTextBox.Text = "70";
            twoWheelBrakesFrontMaximumTorqueTextBox.Text = "1100";
            twoWheelBrakesRearMaximumTorqueTextBox.Text = "650";

            // Engine Curves
            engineCurvesPointsListBox.Items.Add(new Vehicle.EngineCurvesPoint(3000, 55.77, 3.0, 390.45));
            engineCurvesPointsListBox.Items.Add(new Vehicle.EngineCurvesPoint(3500, 52.50, 3.5, 381.50));
            engineCurvesPointsListBox.Items.Add(new Vehicle.EngineCurvesPoint(4000, 54.71, 4.0, 381.80));
            engineCurvesPointsListBox.Items.Add(new Vehicle.EngineCurvesPoint(4500, 57.77, 4.5, 379.02));
            engineCurvesPointsListBox.Items.Add(new Vehicle.EngineCurvesPoint(5000, 70.67, 5.0, 367.21));
            engineCurvesPointsListBox.Items.Add(new Vehicle.EngineCurvesPoint(5500, 79.63, 5.5, 366.58));
            engineCurvesPointsListBox.Items.Add(new Vehicle.EngineCurvesPoint(6000, 83.18, 6.0, 361.44));
            engineCurvesPointsListBox.Items.Add(new Vehicle.EngineCurvesPoint(6500, 83.82, 6.5, 362.35));
            engineCurvesPointsListBox.Items.Add(new Vehicle.EngineCurvesPoint(7000, 83.86, 7.0, 364.99));
            engineCurvesPointsListBox.Items.Add(new Vehicle.EngineCurvesPoint(7500, 84.50, 7.5, 369.16));
            engineCurvesPointsListBox.Items.Add(new Vehicle.EngineCurvesPoint(8000, 82.56, 8.0, 372.97));
            engineCurvesPointsListBox.Items.Add(new Vehicle.EngineCurvesPoint(8500, 77.88, 8.5, 375.73));
            engineCurvesPointsListBox.Items.Add(new Vehicle.EngineCurvesPoint(9000, 72.16, 9.0, 378.86));
            engineCurvesPointsListBox.Items.Add(new Vehicle.EngineCurvesPoint(9500, 66.65, 9.5, 383.92));
            engineCurvesPointsListBox.Items.Add(new Vehicle.EngineCurvesPoint(10000, 61.16, 10.0, 391.00));
            engineCurvesPointsListBox.Items.Add(new Vehicle.EngineCurvesPoint(10500, 55.94, 10.5, 398.92));
            engineCurvesPointsListBox.Items.Add(new Vehicle.EngineCurvesPoint(11000, 50.23, 11.0, 407.68));
            engineCurvesIDTextBox.Text = "engineCurves1";

            // Engine
            engineIDTextBox.Text = "engine1";
            maxThrottleTextBox.Text = "100";
            fuelDensityTextBox.Text = "870";

            // Inertia
            oneWheelInertiaIDTextBox.Text = "inertia1";
            oneWheelTotalMassTextBox.Text = "300";
            oneWheelUnsprungMassTextBox.Text = "50";
            oneWheelRotPartsMITextBox.Text = "5";
            oneWheelGravityAccelTextBox.Text = "9.81";

            // Inertia And Dimensions
            twoWheelInertiaAndDimensionsIDTextBox.Text = "inertiaAndDimensions1";
            twoWheelTotalMassTextBox.Text = "300";
            twoWheelTotalMassDistributionTextBox.Text = "50";
            twoWheelTotalMassCGHeightTextBox.Text = "300";
            twoWheelFrontUnsprungMassTextBox.Text = "20";
            twoWheelFrontUnsprungMassCGHeightTextBox.Text = "250";
            twoWheelRearUnsprungMassTextBox.Text = "30";
            twoWheelRearUnsprungMassCGHeightTextBox.Text = "250";
            twoWheelWheelbaseTextBox.Text = "1530";
            twoWheelRotPartsMITextBox.Text = "5";
            twoWheelGravityAccelTextBox.Text = "9.81";

            // Simplified Suspension
            simplifiedSuspensionIDTextBox.Text = "suspension1";
            simplifiedSuspensionHeaveStiffnessTextBox.Text = "100";
            simplifiedSuspensionRideHeightTextBox.Text = "50";

            // Steering
            steeringSystemIDTextBox.Text = "steering1";
            steeringSystemFrontSteeringRatioTextBox.Text = ".25";
            steeringSystemRearSteeringRatioTextBox.Text = "0";
            steeringSystemMaximumSteeringWheelAngleTextBox.Text = "120";

            // Tire
            tireIDTextBox.Text = "tire1";
            tireStiffnessTextBox.Text = "120";

            // Tire Model
            tireModelTextBox.Text = @"D:\Google Drive\Work\OptimumG\Internship Test\Programs\Auxiliar Files\TireModelMF52.txt";
            lambdaMuxTextBox.Text = "0.66";
            lambdaMuyTextBox.Text = "0.66";
            lambdaMuVTextBox.Text = "0.00";
            tireModelIDTextBox.Text = "tireModel1";
            tireModelMinSlipAngleTextBox.Text = "-12";
            tireModelMaxSlipAngleTextBox.Text = "12";
            tireModelMinLongitudinalSlipTextBox.Text = "-0.15";
            tireModelMaxLongitudinalSlipTextBox.Text = "0.15";

            // Tire Model Display
            tireModelDisplayParameterSetsCheckListBox.Items.Add(new Vehicle.TireModelMF52Point(0, 0, 500, 0, 40));
            tireModelDisplayParameterSetsCheckListBox.Items.Add(new Vehicle.TireModelMF52Point(0, 0, 1000, 0, 40));
            tireModelDisplayParameterSetsCheckListBox.Items.Add(new Vehicle.TireModelMF52Point(0, 0, 1500, 0, 40));
            tireModelDisplayParameterSetsCheckListBox.Items.Add(new Vehicle.TireModelMF52Point(0, 0, 1000, -2, 40));
            tireModelDisplayParameterSetsCheckListBox.Items.Add(new Vehicle.TireModelMF52Point(0, 0, 1000, 2, 40));
            tireModelDisplayParameterSetsCheckListBox.Items.Add(new Vehicle.TireModelMF52Point(0, -6, 1000, 0, 40));
            tireModelDisplayParameterSetsCheckListBox.Items.Add(new Vehicle.TireModelMF52Point(0, .6, 1000, 0, 40));
            tireModelDisplayParameterSetsCheckListBox.Items.Add(new Vehicle.TireModelMF52Point(-.06, 0, 1000, 0, 40));
            tireModelDisplayParameterSetsCheckListBox.Items.Add(new Vehicle.TireModelMF52Point(.06, 0, 1000, 0, 40));
            tireModelDisplayDataAmountOfPointsTextBox.Text = "100";

            // Gear Ratios
            gearRatiosListBox.Items.Add(new Vehicle.GearRatio(2.75));
            gearRatiosListBox.Items.Add(new Vehicle.GearRatio(1.938));
            gearRatiosListBox.Items.Add(new Vehicle.GearRatio(1.556));
            gearRatiosListBox.Items.Add(new Vehicle.GearRatio(1.348));
            gearRatiosListBox.Items.Add(new Vehicle.GearRatio(1.208));
            gearRatiosListBox.Items.Add(new Vehicle.GearRatio(1.095));
            gearRatiosSetIDTextBox.Text = "gearRatios1";

            // One Wheel Transmission
            oneWheelTransmissionIDTextBox.Text = "oneWheelTransmission1";
            oneWheelTransmissionTypeComboBox.Text = "2WD";
            oneWheelPrimaryRatioTextBox.Text = "2.111";
            oneWheelFinalRatioTextBox.Text = "3.7143";
            oneWheelGearShiftTimeTextBox.Text = "0.2";
            oneWheelTransmissionEfficiencyTextBox.Text = "87.5";

            // Two Wheel Transmission
            twoWheelTransmissionIDTextBox.Text = "twoWheelTransmission1";
            twoWheelPrimaryRatioTextBox.Text = "2.111";
            twoWheelFinalRatioTextBox.Text = "3.7143";
            twoWheelGearShiftTimeTextBox.Text = "0.2";
            twoWheelTransmissionEfficiencyTextBox.Text = "87.5";
            twoWheelTransmissionTorqueBiasTextBox.Text = "50";

            // One Wheel Car and setup fields
            oneWheelCarIDTextBox.Text = "oneWheelCar1";
            oneWheelSetupIDTextBox.Text = "oneWheelSetup1";

            // Two Wheel Car and setup fields
            twoWheelCarIDTextBox.Text = "twoWheelCar1";
            twoWheelSetupIDTextBox.Text = "twoWheelSetup1";

            // Path Sectors
            tabularPathSectorsStartDistancesListBox.Items.Add(new PathSector(1, 0));
            tabularPathSectorsStartDistancesListBox.Items.Add(new PathSector(2, 50));
            tabularPathSectorsStartDistancesListBox.Items.Add(new PathSector(3, 207.075));
            tabularPathSectorsStartDistancesListBox.Items.Add(new PathSector(4, 257.075));
            _ReorderAndResetListboxSectorsIndexes();
            tabularPathSectorsSetIDTextBox.Text = "sectorSet1";

            // Path Sections
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Straight, 50, 0, 0));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 62.83, 20, 20));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 31.415, 10, 10));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 62.83, 20, 20));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Straight, 50, 0, 0));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 157.08, 50, 50));
            tabularPathSectionsSetIDTextBox.Text = "sections1";

            // Path
            tabularPathIDTextBox.Text = "path1";
            tabularPathResolutionTextBox.Text = "100";

            // GGV Diagram
            ggvDiagramIDTextBox.Text = "ggv1";
            ggvDiagramAmountOfPointsPerSpeedTextBox.Text = "40";
            ggvDiagramAmountOfSpeedsTextBox.Text = "20";
            ggvDiagramLowestSpeedTextBox.Text = "10";
            ggvDiagramHighestSpeedTextBox.Text = "120";

            // Lap Time Simulation
            lapTimeSimulationIDTextBox.Text = "lapTime1";
        }
        /// <summary>
        /// Populates the UI fields with sample values.
        /// </summary>
        private void _PopulateFieldsWithLMP1()
        {
            // Two Wheel Aerodynamic Map

            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(49.0,70.0,0.593,1.996,0.464));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(49.0,67.5,0.591,2.010,0.457));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(49.0,65.0,0.590,2.023,0.449));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(49.0,62.5,0.589,2.033,0.442));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(49.0,60.0,0.587,2.038,0.436));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(49.0,57.5,0.586,2.044,0.431));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(49.0,55.0,0.585,2.051,0.425));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(49.0,52.5,0.584,2.052,0.420));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(49.0,50.0,0.583,2.048,0.417));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(49.0,47.5,0.583,2.046,0.414));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(49.0,45.0,0.582,2.045,0.410));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(46.5,70.0,0.590,2.026,0.476));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(46.5,67.5,0.588,2.035,0.469));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(46.5,65.0,0.587,2.048,0.461));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(46.5,62.5,0.585,2.055,0.455));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(46.5,60.0,0.584,2.060,0.449));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(46.5,57.5,0.583,2.068,0.443));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(46.5,55.0,0.582,2.069,0.438));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(46.5,52.5,0.581,2.066,0.434));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(46.5,50.0,0.580,2.065,0.430));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(46.5,47.5,0.579,2.064,0.426));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(46.5,45.0,0.578,2.057,0.423));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(44.0,70.0,0.586,2.055,0.488));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(44.0,67.5,0.585,2.064,0.480));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(44.0,65.0,0.583,2.073,0.473));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(44.0,62.5,0.582,2.079,0.467));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(44.0,60.0,0.581,2.086,0.461));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(44.0,57.5,0.579,2.088,0.455));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(44.0,55.0,0.578,2.086,0.451));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(44.0,52.5,0.577,2.085,0.447));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(44.0,50.0,0.576,2.085,0.442));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(44.0,47.5,0.575,2.079,0.438));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(44.0,45.0,0.575,2.067,0.436));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(41.5,70.0,0.583,2.088,0.498));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(41.5,67.5,0.581,2.092,0.491));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(41.5,65.0,0.580,2.100,0.484));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(41.5,62.5,0.578,2.102,0.478));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(41.5,60.0,0.577,2.108,0.472));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(41.5,57.5,0.576,2.108,0.467));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(41.5,55.0,0.575,2.107,0.463));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(41.5,52.5,0.574,2.107,0.458));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(41.5,50.0,0.573,2.102,0.454));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(41.5,47.5,0.572,2.091,0.451));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(41.5,45.0,0.571,2.083,0.449));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(39.0,70.0,0.580,2.121,0.507));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(39.0,67.5,0.578,2.125,0.501));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(39.0,65.0,0.576,2.128,0.494));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(39.0,62.5,0.575,2.130,0.489));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(39.0,60.0,0.574,2.131,0.483));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(39.0,57.5,0.572,2.131,0.478));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(39.0,55.0,0.571,2.130,0.473));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(39.0,52.5,0.570,2.126,0.468));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(39.0,50.0,0.569,2.117,0.465));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(39.0,47.5,0.568,2.109,0.463));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(39.0,45.0,0.567,2.101,0.460));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(36.5,70.0,0.576,2.152,0.517));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(36.5,67.5,0.574,2.156,0.510));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(36.5,65.0,0.573,2.159,0.503));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(36.5,62.5,0.571,2.156,0.498));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(36.5,60.0,0.570,2.158,0.492));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(36.5,57.5,0.569,2.152,0.488));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(36.5,55.0,0.567,2.152,0.483));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(36.5,52.5,0.566,2.144,0.479));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(36.5,50.0,0.565,2.136,0.476));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(36.5,47.5,0.564,2.129,0.473));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(36.5,45.0,0.564,2.116,0.470));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(34.0,70.0,0.572,2.185,0.524));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(34.0,67.5,0.571,2.190,0.518));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(34.0,65.0,0.569,2.191,0.512));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(34.0,62.5,0.568,2.187,0.507));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(34.0,60.0,0.566,2.184,0.501));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(34.0,57.5,0.565,2.178,0.497));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(34.0,55.0,0.564,2.173,0.493));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(34.0,52.5,0.563,2.165,0.489));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(34.0,50.0,0.562,2.158,0.485));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(34.0,47.5,0.560,2.146,0.482));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(34.0,45.0,0.560,2.130,0.480));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(31.5,70.0,0.569,2.222,0.531));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(31.5,67.5,0.567,2.224,0.525));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(31.5,65.0,0.565,2.220,0.520));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(31.5,62.5,0.564,2.217,0.514));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(31.5,60.0,0.562,2.213,0.509));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(31.5,57.5,0.561,2.203,0.505));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(31.5,55.0,0.560,2.198,0.501));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(31.5,52.5,0.559,2.185,0.498));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(31.5,50.0,0.557,2.177,0.494));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(31.5,47.5,0.557,2.163,0.492));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(31.5,45.0,0.556,2.147,0.490));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(29.0,70.0,0.565,2.258,0.539));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(29.0,67.5,0.563,2.254,0.533));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(29.0,65.0,0.561,2.251,0.527));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(29.0,62.5,0.560,2.249,0.521));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(29.0,60.0,0.558,2.243,0.517));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(29.0,57.5,0.557,2.233,0.513));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(29.0,55.0,0.556,2.222,0.509));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(29.0,52.5,0.555,2.210,0.506));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(29.0,50.0,0.553,2.197,0.503));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(29.0,47.5,0.552,2.182,0.500));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(29.0,45.0,0.552,2.168,0.498));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(26.5,70.0,0.561,2.291,0.545));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(26.5,67.5,0.559,2.288,0.539));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(26.5,65.0,0.557,2.287,0.533));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(26.5,62.5,0.556,2.282,0.528));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(26.5,60.0,0.554,2.270,0.524));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(26.5,57.5,0.553,2.261,0.519));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(26.5,55.0,0.552,2.250,0.516));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(26.5,52.5,0.550,2.233,0.513));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(26.5,50.0,0.549,2.220,0.510));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(26.5,47.5,0.548,2.200,0.508));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(26.5,45.0,0.547,2.186,0.506));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(24.0,70.0,0.557,2.326,0.550));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(24.0,67.5,0.555,2.326,0.544));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(24.0,65.0,0.553,2.321,0.539));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(24.0,62.5,0.552,2.311,0.534));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(24.0,60.0,0.550,2.301,0.529));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(24.0,57.5,0.549,2.292,0.525));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(24.0,55.0,0.547,2.278,0.522));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(24.0,52.5,0.546,2.261,0.520));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(24.0,50.0,0.545,2.243,0.517));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(24.0,47.5,0.544,2.223,0.515));
            twoWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.TwoWheelAerodynamicMapPoint(24.0,45.0,0.543,2.204,0.513));
            twoWheelAerodynamicMapIDTextBox.Text = "twoWheelLMP1AeroMap1";

            // Aerodynamics
            aerodynamicsIDTextBox.Text = "aero1";
            frontalAreaTextBox.Text = "1.75";
            airDensityTextBox.Text = "1.225";
            
            // Two Wheel Brakes
            twoWheelBrakesIDTextBox.Text = "twoWheelLMP1Brakes1";
            twoWheelBrakesBrakeBiasTextBox.Text = "54";
            twoWheelBrakesFrontMaximumTorqueTextBox.Text = "1e5";
            twoWheelBrakesRearMaximumTorqueTextBox.Text = "1e5";

            // Engine Curves
            engineCurvesPointsListBox.Items.Add(new Vehicle.EngineCurvesPoint(4000,324.30,0,0));
            engineCurvesPointsListBox.Items.Add(new Vehicle.EngineCurvesPoint(4500,338.60,0,0));
            engineCurvesPointsListBox.Items.Add(new Vehicle.EngineCurvesPoint(5000,356.00,0,0));
            engineCurvesPointsListBox.Items.Add(new Vehicle.EngineCurvesPoint(5500,370.00,0,0));
            engineCurvesPointsListBox.Items.Add(new Vehicle.EngineCurvesPoint(6000,383.00,0,0));
            engineCurvesPointsListBox.Items.Add(new Vehicle.EngineCurvesPoint(6500,396.60,0,0));
            engineCurvesPointsListBox.Items.Add(new Vehicle.EngineCurvesPoint(7000,403.80,0,0));
            engineCurvesPointsListBox.Items.Add(new Vehicle.EngineCurvesPoint(7500,414.00,0,0));
            engineCurvesPointsListBox.Items.Add(new Vehicle.EngineCurvesPoint(8000,406.80,0,0));
            engineCurvesPointsListBox.Items.Add(new Vehicle.EngineCurvesPoint(8500,414.70,0,0));
            engineCurvesPointsListBox.Items.Add(new Vehicle.EngineCurvesPoint(9000,417.00,0,0));
            engineCurvesPointsListBox.Items.Add(new Vehicle.EngineCurvesPoint(9250,414.20,0,0));
            engineCurvesPointsListBox.Items.Add(new Vehicle.EngineCurvesPoint(9500,405.50,0,0));
            engineCurvesPointsListBox.Items.Add(new Vehicle.EngineCurvesPoint(9750,399.50,0,0));
            engineCurvesPointsListBox.Items.Add(new Vehicle.EngineCurvesPoint(10000,391.60,0,0));
            engineCurvesPointsListBox.Items.Add(new Vehicle.EngineCurvesPoint(10250,383.50,0,0));
            engineCurvesPointsListBox.Items.Add(new Vehicle.EngineCurvesPoint(10500,370.30,0,0));
            engineCurvesPointsListBox.Items.Add(new Vehicle.EngineCurvesPoint(10750,355.00,0,0));
            engineCurvesPointsListBox.Items.Add(new Vehicle.EngineCurvesPoint(11000,325.00,0,0));

            engineCurvesIDTextBox.Text = "engineCurvesLMP1";

            // Engine
            engineIDTextBox.Text = "engine1";
            maxThrottleTextBox.Text = "100";
            fuelDensityTextBox.Text = "870";
            
            // Inertia And Dimensions
            twoWheelInertiaAndDimensionsIDTextBox.Text = "inertiaAndDimensions1";
            twoWheelTotalMassTextBox.Text = "995";
            twoWheelTotalMassDistributionTextBox.Text = "47.58";
            twoWheelTotalMassCGHeightTextBox.Text = "270.4";
            twoWheelFrontUnsprungMassTextBox.Text = "74";
            twoWheelFrontUnsprungMassCGHeightTextBox.Text = "340";
            twoWheelRearUnsprungMassTextBox.Text = "84";
            twoWheelRearUnsprungMassCGHeightTextBox.Text = "355";
            twoWheelWheelbaseTextBox.Text = "2980";
            twoWheelRotPartsMITextBox.Text = "0";
            twoWheelGravityAccelTextBox.Text = "9.81";

            // Simplified Suspension
            simplifiedSuspensionIDTextBox.Text = "frontSuspension24";
            simplifiedSuspensionHeaveStiffnessTextBox.Text = "300";
            simplifiedSuspensionRideHeightTextBox.Text = "24";
            _AddSimplifiedSuspensionToListBox_Click(null, new RoutedEventArgs());
            simplifiedSuspensionIDTextBox.Text = "frontSuspension30o25";
            simplifiedSuspensionRideHeightTextBox.Text = "30.25";
            _AddSimplifiedSuspensionToListBox_Click(null, new RoutedEventArgs());
            simplifiedSuspensionIDTextBox.Text = "frontSuspension36o5";
            simplifiedSuspensionRideHeightTextBox.Text = "36.5";
            _AddSimplifiedSuspensionToListBox_Click(null, new RoutedEventArgs());
            simplifiedSuspensionIDTextBox.Text = "frontSuspension42o75";
            simplifiedSuspensionRideHeightTextBox.Text = "42.75";
            _AddSimplifiedSuspensionToListBox_Click(null, new RoutedEventArgs());
            simplifiedSuspensionIDTextBox.Text = "frontSuspension49";
            simplifiedSuspensionRideHeightTextBox.Text = "49";
            _AddSimplifiedSuspensionToListBox_Click(null, new RoutedEventArgs());

            simplifiedSuspensionIDTextBox.Text = "rearSuspension45";
            simplifiedSuspensionHeaveStiffnessTextBox.Text = "333";
            simplifiedSuspensionRideHeightTextBox.Text = "45";
            _AddSimplifiedSuspensionToListBox_Click(null, new RoutedEventArgs());
            simplifiedSuspensionIDTextBox.Text = "rearSuspension51o25";
            simplifiedSuspensionRideHeightTextBox.Text = "51.25";
            _AddSimplifiedSuspensionToListBox_Click(null, new RoutedEventArgs());
            simplifiedSuspensionIDTextBox.Text = "rearSuspension57o5";
            simplifiedSuspensionRideHeightTextBox.Text = "57.5";
            _AddSimplifiedSuspensionToListBox_Click(null, new RoutedEventArgs());
            simplifiedSuspensionIDTextBox.Text = "rearSuspension63o75";
            simplifiedSuspensionRideHeightTextBox.Text = "63.75";
            _AddSimplifiedSuspensionToListBox_Click(null, new RoutedEventArgs());
            simplifiedSuspensionIDTextBox.Text = "rearSuspension70";
            simplifiedSuspensionRideHeightTextBox.Text = "70";
            _AddSimplifiedSuspensionToListBox_Click(null, new RoutedEventArgs());
            // Steering
            steeringSystemIDTextBox.Text = "steering1";
            steeringSystemFrontSteeringRatioTextBox.Text = ".5";
            steeringSystemRearSteeringRatioTextBox.Text = "0";
            steeringSystemMaximumSteeringWheelAngleTextBox.Text = "120";
            
            // Gear Ratios
            gearRatiosListBox.Items.Add(new Vehicle.GearRatio(37/12));
            gearRatiosListBox.Items.Add(new Vehicle.GearRatio(31/14));
            gearRatiosListBox.Items.Add(new Vehicle.GearRatio(29/16));
            gearRatiosListBox.Items.Add(new Vehicle.GearRatio(28/18));
            gearRatiosListBox.Items.Add(new Vehicle.GearRatio(26/19));
            gearRatiosListBox.Items.Add(new Vehicle.GearRatio(27/22));
            gearRatiosSetIDTextBox.Text = "gearRatios1";

            // Two Wheel Transmission
            twoWheelTransmissionIDTextBox.Text = "twoWheelTransmission1";
            twoWheelPrimaryRatioTextBox.Text = "2.744";
            twoWheelFinalRatioTextBox.Text = "1.105";
            twoWheelGearShiftTimeTextBox.Text = "0.2";
            twoWheelTransmissionEfficiencyTextBox.Text = "100";
            twoWheelTransmissionTorqueBiasTextBox.Text = "0";
            
            // Two Wheel Car and setup fields
            twoWheelCarIDTextBox.Text = "twoWheelCar1";
            twoWheelSetupIDTextBox.Text = "twoWheelSetup1";

            // Path Sectors
            tabularPathSectorsStartDistancesListBox.Items.Add(new PathSector(1, 0));
            _ReorderAndResetListboxSectorsIndexes();
            tabularPathSectorsSetIDTextBox.Text = "sectorSet1";

            // Path Sections
            #region Spa-Francorchamps
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Straight, 141.39, 0, 1));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 15.447, 1985.618, 1986.618));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Straight, 29.192, 0, 1));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 13.857, 1449.88, 1450.88));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 12.939, 1264.128, 1265.128));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 11.001, 850.86, 851.86));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 8.564, 36.073, 37.073));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 7.024, 15.25, 16.25));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 6.597, 22.267, 23.267));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 6.324, 17.291, 18.291));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 7.206, 18.505, 19.505));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 8.424, 23.892, 24.892));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 10, 57.51, 58.51));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 11.864, 78.834, 79.834));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 13.281, 69.889, 70.889));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 14.782, 158.251, 159.251));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 16.375, 400.489, 401.489));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 17.626, 1362.181, 1363.181));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 18.899, 822.768, 823.768));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 20.118, 657.49, 658.49));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 21.156, 712.853, 713.853));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 21.994, 1153.509, 1154.509));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Straight, 22.951, 0, 1));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 23.875, 1066.157, 1067.157));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 24.756, 476.876, 477.876));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 25.519, 442.57, 443.57));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 26.168, 502.18, 503.18));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 26.801, 413.104, 414.104));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 27.404, 368.488, 369.488));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 27.999, 480.665, 481.665));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Straight, 178.76, 0, 1));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 29.661, 1784.719, 1785.719));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 29.113, 928.981, 929.981));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 28.49, 335.666, 336.666));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 27.775, 513.143, 514.143));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 27.032, 782.025, 783.025));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 26.119, 173.318, 174.318));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 24.925, 114.882, 115.882));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 24.333, 242.889, 243.889));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 24.283, 277.966, 278.966));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 24.365, 350.313, 351.313));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 24.722, 1993.682, 1994.682));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 25.096, 177.716, 178.716));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 25.413, 251.261, 252.261));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 25.971, 725.644, 726.644));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 26.528, 896.691, 897.691));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 26.989, 948.892, 949.892));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 27.369, 1715.943, 1716.943));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Straight, 27.774, 0, 1));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 28.161, 1456.591, 1457.591));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 28.532, 1152.554, 1153.554));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 28.893, 1215.682, 1216.682));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 29.249, 979.832, 980.832));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 29.59, 674.891, 675.891));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 29.913, 565.636, 566.636));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 30.21, 869.442, 870.442));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 30.494, 881.788, 882.788));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 30.775, 1394.143, 1395.143));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Straight, 655.429, 0, 1));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 14.596, 417.624, 418.624));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 14.042, 66.278, 67.278));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 13.46, 57.263, 58.263));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 13.51, 62.171, 63.171));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 14.313, 63.663, 64.663));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 14.947, 88.018, 89.018));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 15.647, 364.347, 365.347));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 15.904, 80.324, 81.324));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 16.06, 79.489, 80.489));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 16.858, 76.897, 77.897));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 17.314, 109.624, 110.624));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 17.606, 110.186, 111.186));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 18.078, 105.925, 106.925));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 17.733, 175.65, 176.65));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 16.585, 108.991, 109.991));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 15.979, 68.54, 69.54));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 15.4, 68.778, 69.778));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 15.861, 98.256, 99.256));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 16.517, 77.731, 78.731));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 17.254, 105.281, 106.281));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 18.183, 174.857, 175.857));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 19.106, 148.54, 149.54));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 20.113, 442.195, 443.195));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 21.203, 964.768, 965.768));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 22.075, 1540.291, 1541.291));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Straight, 166.918, 0, 1));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 17.49, 712.765, 713.765));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 15.201, 130.503, 131.503));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 14.315, 49.384, 50.384));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 13.872, 46.734, 47.734));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 13.736, 49.159, 50.159));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 14.228, 51.267, 52.267));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 14.653, 51.256, 52.256));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 15.086, 54.047, 55.047));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 15.524, 56.052, 57.052));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 15.692, 53.661, 54.661));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 15.793, 56.848, 57.848));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 16.192, 63.291, 64.291));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 16.957, 100.293, 101.293));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 17.846, 117.517, 118.517));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 19.036, 803.026, 804.026));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 19.525, 658.66, 659.66));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 17.557, 553.685, 554.685));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 16.263, 153.832, 154.832));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 15.903, 92.816, 93.816));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 15.542, 72.793, 73.793));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 15.758, 70.315, 71.315));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 16.4, 75.373, 76.373));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 17.326, 88.19, 89.19));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 18.381, 111.996, 112.996));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 19.46, 127.82, 128.82));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 20.588, 803.822, 804.822));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 21.761, 398.118, 399.118));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 22.671, 673.854, 674.854));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 23.624, 1025.016, 1026.016));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 24.549, 1175.706, 1176.706));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 25.439, 1127.642, 1128.642));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 26.288, 1169.156, 1170.156));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 26.926, 1356.097, 1357.097));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 27.606, 1237.97, 1238.97));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 28.263, 1819.53, 1820.53));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Straight, 109.867, 0, 1));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 22.336, 1312.425, 1313.425));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 21.613, 174.733, 175.733));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 21.543, 135.459, 136.459));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 21.956, 142.533, 143.533));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 22.483, 148.179, 149.179));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 23.021, 147.602, 148.602));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 23.465, 131.141, 132.141));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 23.378, 114.749, 115.749));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 22.725, 118.365, 119.365));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 22.06, 114.629, 115.629));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 22.35, 384.3, 385.3));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 22.924, 244.041, 245.041));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 23.514, 262.45, 263.45));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 24.025, 173.563, 174.563));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 24.444, 158.828, 159.828));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 24.851, 172.835, 173.835));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 25.246, 215.309, 216.309));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 25.646, 212.503, 213.503));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 25.982, 195.493, 196.493));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 26.306, 268.718, 269.718));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Straight, 26.906, 0, 1));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 27.407, 1196.389, 1197.389));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 27.939, 837.579, 838.579));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 28.438, 1345.884, 1346.884));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Straight, 83.054, 0, 1));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 21.553, 1821.227, 1822.227));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 17.901, 854.03, 855.03));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 15.099, 279.139, 280.139));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 14.693, 95.577, 96.577));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 14.985, 85.248, 86.248));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 15.792, 79.689, 80.689));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 16.501, 78.41, 79.41));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 17.103, 89.541, 90.541));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 17.008, 73.173, 74.173));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 17.075, 81.481, 82.481));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 17.626, 139.365, 140.365));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 18.447, 313.22, 314.22));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 19.169, 130.631, 131.631));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 19.574, 99.629, 100.629));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 19.611, 89.304, 90.304));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 19.771, 89.742, 90.742));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 20.04, 91.229, 92.229));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 20.332, 98.919, 99.919));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 20.729, 117.985, 118.985));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 21.279, 161.955, 162.955));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 22.051, 436.724, 437.724));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 22.919, 863.669, 864.669));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 23.7, 555.892, 556.892));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 23.339, 584.477, 585.477));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 22.421, 761.978, 762.978));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 20.222, 307.645, 308.645));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 17.682, 98.594, 99.594));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 16.993, 80.261, 81.261));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 16.593, 68.959, 69.959));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 16.947, 71.191, 72.191));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 17.369, 70.577, 71.577));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 17.892, 81.782, 82.782));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 18.458, 93.867, 94.867));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 19.129, 291.416, 292.416));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 20.171, 987.482, 988.482));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 21.263, 1018.453, 1019.453));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 22.083, 234.214, 235.214));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 22.893, 195.157, 196.157));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 23.603, 144.041, 145.041));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 24.113, 129.83, 130.83));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 24.538, 134.964, 135.964));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 24.325, 115.438, 116.438));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 23.558, 118.109, 119.109));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 23.817, 155.227, 156.227));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 24.332, 285.348, 286.348));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 24.996, 682.995, 683.995));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 25.604, 1086.619, 1087.619));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 26.054, 617.828, 618.828));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 26.524, 562.453, 563.453));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 26.979, 613.201, 614.201));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 27.418, 619.24, 620.24));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 27.84, 633.341, 634.341));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 28.251, 594.954, 595.954));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 28.651, 635.143, 636.143));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 29.039, 527.355, 528.355));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 29.41, 520.167, 521.167));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 29.768, 468.638, 469.638));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 30.118, 672.484, 673.484));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Straight, 61.032, 0, 1));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 30.89, 1818.113, 1819.113));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 31.131, 940.839, 941.839));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 31.357, 622.548, 623.548));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 31.568, 487.8, 488.8));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 31.747, 356.429, 357.429));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 31.91, 366.12, 367.12));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 32.033, 280.431, 281.431));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 32.124, 300.546, 301.546));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 32.235, 515.427, 516.427));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 31.585, 696.518, 697.518));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Straight, 62.154, 0, 1));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 30.918, 425.985, 426.985));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 30.769, 244.793, 245.793));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 30.547, 206.896, 207.896));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 30.29, 195.459, 196.459));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 30.226, 230.527, 231.527));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 30.394, 294.746, 295.746));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 30.604, 395.754, 396.754));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 30.865, 1773.729, 1774.729));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 31.131, 846.151, 847.151));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 31.361, 566.423, 567.423));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 31.569, 555.914, 556.914));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 31.774, 977.782, 978.782));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 31.974, 1957.018, 1958.018));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 32.158, 1297.462, 1298.462));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 32.321, 794.678, 795.678));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 31.839, 1057.134, 1058.134));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Straight, 143.006, 0, 1));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 11.988, 284.434, 285.434));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 11.436, 34.946, 35.946));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 10.764, 29.198, 30.198));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 10.425, 39.851, 40.851));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 10.765, 38.387, 39.387));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 10.486, 28.907, 29.907));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 9.996, 50.297, 51.297));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 9.571, 33.289, 34.289));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 9.494, 27.74, 28.74));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 10.486, 30.563, 31.563));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 10.831, 26.469, 27.469));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 10.846, 35.955, 36.955));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 11.556, 46.063, 47.063));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 12.407, 52.392, 53.392));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 13.788, 270.072, 271.072));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 15.253, 279.827, 280.827));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 16.41, 419.075, 420.075));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Straight, 190.557, 0, 1));
            tabularPathSectionsSetIDTextBox.Text = "Spa-Francorchamps";
            _TabularPathAddSectionsSetToListBox_Click(null, new RoutedEventArgs());
            tabularPathSectionsListBox.Items.Clear();
            #endregion
            #region Monaco
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 25.583, 847.217, 848.217));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 26.114, 718.494, 719.494));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 26.654, 618.978, 619.978));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 26.965, 383.549, 384.549));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 26.324, 448.478, 449.478));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Straight, 46.667, 0, 1));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 17.878, 1533.201, 1534.201));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 14.25, 422.44, 423.44));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 10.668, 329.111, 330.111));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 9.474, 99.443, 100.443));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 9.55, 32.535, 33.535));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 10.192, 28.273, 29.273));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 11.108, 60.839, 61.839));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 12.204, 79.076, 80.076));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 13.353, 94.416, 95.416));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 14.551, 216.929, 217.929));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 15.8, 1725.254, 1726.254));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 16.742, 621.113, 622.113));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 17.671, 404.198, 405.198));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 18.569, 436.649, 437.649));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 19.438, 1252.468, 1253.468));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Straight, 106.278, 0, 1));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 20.974, 769.806, 770.806));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 20.386, 248.471, 249.471));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 20.557, 946.753, 947.753));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Straight, 40.696, 0, 1));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 19.624, 179.449, 180.449));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 19.24, 533.368, 534.368));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 18.864, 636.383, 637.383));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 18.364, 1580.524, 1581.524));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 17.371, 1864.182, 1865.182));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 16.9, 420.421, 421.421));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 16.408, 91.942, 92.942));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 15.86, 90.124, 91.124));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 15.1, 94.1, 95.1));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 14.581, 108.9, 109.9));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 13.603, 79.586, 80.586));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 13.029, 51.656, 52.656));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 12.488, 45.191, 46.191));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 12.028, 72.735, 73.735));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 11.733, 132.082, 133.082));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 12.047, 85.766, 86.766));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 12.646, 75.909, 76.909));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 12.882, 90.945, 91.945));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 12.521, 409.763, 410.763));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 12.261, 136.219, 137.219));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 11.944, 43.608, 44.608));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 11.886, 48.695, 49.695));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 12.465, 57.078, 58.078));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 13.306, 75.906, 76.906));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 14.324, 113.201, 114.201));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 15.518, 164.472, 165.472));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 16.76, 124.626, 125.626));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 18.018, 719.431, 720.431));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Straight, 18.943, 0, 1));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 18.968, 1222.518, 1223.518));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Straight, 70.363, 0, 1));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 8.363, 125.614, 126.614));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 7.986, 43.854, 44.854));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 7.682, 28.308, 29.308));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 7.371, 20.801, 21.801));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 7.44, 21.174, 22.174));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 7.871, 22.818, 23.818));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 8.317, 24.146, 25.146));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 8.921, 39.864, 40.864));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 10.079, 445.407, 446.407));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 11.267, 761.155, 762.155));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 12.013, 184.445, 185.445));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 11.04, 162.416, 163.416));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 8.689, 570.067, 571.067));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Straight, 15.944, 0, 1));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 7.164, 139.507, 140.507));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 6.601, 20.782, 21.782));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 6.4, 14.324, 15.324));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 6.161, 11.516, 12.516));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 5.89, 10.534, 11.534));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 5.604, 10.361, 11.361));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 5.538, 16.365, 17.365));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 6.19, 16.91, 17.91));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 6.99, 28.709, 29.709));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 7.978, 40.868, 41.868));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 9.104, 456.709, 457.709));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 9.119, 75.692, 76.692));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 8.685, 23.281, 24.281));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 8.332, 28.026, 29.026));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 8.047, 25.365, 26.365));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 8.122, 28.345, 29.345));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 8.892, 32.53, 33.53));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 9.775, 43.973, 44.973));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 10.658, 55.012, 56.012));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 11.543, 100.983, 101.983));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 11.443, 731.395, 732.395));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 8.854, 217.455, 218.455));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 6.222, 48.874, 49.874));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 5.867, 31.048, 32.048));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 5.528, 20.031, 21.031));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 5.507, 21.357, 22.357));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 6.093, 22.798, 23.798));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 6.885, 28.339, 29.339));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 7.869, 30.423, 31.423));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 8.947, 42.836, 43.836));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 10.079, 87.39, 88.39));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 11.633, 681.262, 682.262));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 13.286, 402.1, 403.1));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 14.713, 501.477, 502.477));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 15.853, 742.545, 743.545));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 15.774, 1127.225, 1128.225));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 15.728, 475.762, 476.762));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 15.9, 454.108, 455.108));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 16.375, 256.652, 257.652));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 16.817, 374.386, 375.386));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 17.124, 370.151, 371.151));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 16.972, 295.111, 296.111));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 17.131, 623.207, 624.207));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 17.415, 314.673, 315.673));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 17.689, 231.128, 232.128));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 17.953, 223.88, 224.88));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 18.208, 184.177, 185.177));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 18.435, 139.827, 140.827));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 18.657, 168.562, 169.562));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 18.9, 253.748, 254.748));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 19.158, 538.346, 539.346));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 19.443, 763.078, 764.078));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 19.878, 988.414, 989.414));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 20.313, 955.884, 956.884));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 20.742, 1002.398, 1003.398));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 20.492, 1025.248, 1026.248));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 20.01, 555.298, 556.298));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 19.564, 573.763, 574.763));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 19.165, 418.348, 419.348));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 18.786, 519.499, 520.499));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 18.419, 596.288, 597.288));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 17.843, 358.609, 359.609));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 16.597, 613.777, 614.777));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 15.219, 1330.241, 1331.241));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Straight, 12.036, 0, 1));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 8.65, 386.186, 387.186));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 7.368, 280.201, 281.201));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 7.301, 64.502, 65.502));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 7.164, 22.795, 23.795));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 6.875, 22.076, 23.076));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 6.657, 34.95, 35.95));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 6.542, 82.306, 83.306));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 6.16, 16.049, 17.049));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 6.243, 19.357, 20.357));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 6.633, 21.283, 22.283));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 7.136, 33.329, 34.329));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 8.058, 59.102, 60.102));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 9.06, 153.52, 154.52));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 10.149, 97.664, 98.664));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 11.243, 86.625, 87.625));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 12.286, 86.445, 87.445));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 13.172, 456.433, 457.433));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 14.038, 1317.167, 1318.167));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Straight, 14.964, 0, 1));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 15.826, 1791.76, 1792.76));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 16.611, 1562.629, 1563.629));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Straight, 65.096, 0, 1));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 15.306, 333.982, 334.982));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 14.833, 74.701, 75.701));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 14.299, 55.135, 56.135));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 13.763, 56.331, 57.331));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 13.924, 104.7, 105.7));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 14.589, 169.168, 170.168));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 15.251, 220.567, 221.567));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 15.893, 203.141, 204.141));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 15.964, 161.105, 162.105));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 15.488, 135.838, 136.838));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 15.025, 122.081, 123.081));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 14.556, 89.986, 90.986));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 14.179, 70.731, 71.731));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 14.486, 87.849, 88.849));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 15.047, 269.946, 270.946));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 15.633, 213.392, 214.392));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 16.179, 176.712, 177.712));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 16.329, 131.785, 132.785));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 15.404, 422.505, 423.505));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 13.124, 575.623, 576.623));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 11.733, 328.275, 329.275));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 11.388, 341.127, 342.127));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 10.94, 147.441, 148.441));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 10.486, 50.949, 51.949));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 9.95, 26.384, 27.384));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 9.607, 37.974, 38.974));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 9.931, 1914.78, 1915.78));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 10.276, 39.288, 40.288));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 10.225, 43.768, 44.768));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 11.315, 55.421, 56.421));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 12.296, 54.943, 55.943));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 13.244, 125.263, 126.263));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 14.192, 123.491, 124.491));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 14.706, 124.018, 125.018));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 14.218, 114.482, 115.482));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 13.736, 165.095, 166.095));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 13.294, 167.596, 168.596));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 12.856, 102.101, 103.101));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 12.344, 137.163, 138.163));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 11.458, 500.323, 501.323));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 10.969, 402.161, 403.161));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 10.472, 425.872, 426.872));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 9.94, 37.937, 38.937));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 9.372, 24.616, 25.616));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 8.735, 18.171, 19.171));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 8.154, 18.595, 19.595));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 7.81, 22.344, 23.344));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 7.844, 26.439, 27.439));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 8.347, 54.952, 55.952));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 9.139, 244.998, 245.998));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 9.768, 845.764, 846.764));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 9.347, 131.945, 132.945));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 8.906, 31.035, 32.035));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 8.481, 30.515, 31.515));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 8.7, 32.453, 33.453));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 9.543, 30.893, 31.893));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 9.693, 41.823, 42.823));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 10.843, 160.333, 161.333));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 11.975, 132.588, 133.588));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 12.897, 116.938, 117.938));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Left, 14.314, 480.128, 481.128));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 15.775, 607.594, 608.594));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 17.088, 377.953, 378.953));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 18.172, 472.457, 473.457));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 19.269, 464.421, 465.421));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 20.332, 606.323, 607.323));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 21.354, 693.779, 694.779));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 22.115, 513.977, 514.977));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 22.917, 679.803, 680.803));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 23.693, 597.633, 598.633));
            tabularPathSectionsListBox.Items.Add(new TabularPathSection(TabularPathSection.SectionType.Right, 24.444, 619.953, 620.953));
            tabularPathSectionsSetIDTextBox.Text = "Monaco";
            _TabularPathAddSectionsSetToListBox_Click(null, new RoutedEventArgs());
            tabularPathSectionsListBox.Items.Clear();
            #endregion
            // Path
            tabularPathIDTextBox.Text = "path1";
            tabularPathResolutionTextBox.Text = "100";

            // GGV Diagram
            ggvDiagramIDTextBox.Text = "ggv1";
            ggvDiagramAmountOfPointsPerSpeedTextBox.Text = "40";
            ggvDiagramAmountOfSpeedsTextBox.Text = "20";
            ggvDiagramLowestSpeedTextBox.Text = "10";
            ggvDiagramHighestSpeedTextBox.Text = "120";

            // Lap Time Simulation
            lapTimeSimulationIDTextBox.Text = "lapTime1";
        }
        #endregion

        #region Main Menu Methods
        #region Main Navigation Controls Click Methods

        /// <summary>
        /// Displays the file options buttons (new/save/load) and hides all of the other Mainmenu buttons options.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _ProjectEnvironmentButton_Click(object sender, RoutedEventArgs e)
        {
            _HideOptionsButtons();
            projectButtonOptionsGrid.Visibility = Visibility.Visible;
            _ResetMainNavigationsButtonsBackground();
            projectEnvironmentButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF3B4554"));
        }

        /// <summary>
        /// Displays the vehicle options buttons (models) and hides all of the other Mainmenu buttons options.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _VehicleInputEnvironmentButton_Click(object sender, RoutedEventArgs e)
        {
            _HideOptionsButtons();
            vehicleButtonOptionsGrid.Visibility = Visibility.Visible;
            _ResetMainNavigationsButtonsBackground();
            vehicleInputEnvironmentButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF3B4554"));
        }

        /// <summary>
        /// Displays the path options buttons (input methods) and hides all of the other Mainmenu buttons options.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _PathInputEnvironmentButton_Click(object sender, RoutedEventArgs e)
        {
            _HideOptionsButtons();
            pathButtonOptionsGrid.Visibility = Visibility.Visible;
            _ResetMainNavigationsButtonsBackground();
            pathInputEnvironmentButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF3B4554"));
        }

        /// <summary>
        /// Displays the simulation options buttons (types) and hides all of the other Mainmenu buttons options.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _SimulationInputEnvironmentButton_Click(object sender, RoutedEventArgs e)
        {
            _HideOptionsButtons();
            simulationButtonOptionsGrid.Visibility = Visibility.Visible;
            _ResetMainNavigationsButtonsBackground();
            simulationInputEnvironmentButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF3B4554"));
        }

        /// <summary>
        /// Displays the results analysis options buttons (types) and hides all of the other Mainmenu buttons options.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _ResultsAnalysisEnvironmentButton_Click(object sender, RoutedEventArgs e)
        {
            _HideOptionsButtons();
            resultsButtonOptionsGrid.Visibility = Visibility.Visible;
            _ResetMainNavigationsButtonsBackground();
            resultsAnalysisEnvironmentButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF3B4554"));
        }

        /// <summary>
        /// Hides all the main navigation controls options and displays the help file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _HelpEnvironmentButton_Click(object sender, RoutedEventArgs e)
        {
            _HideOptionsButtons();
        }

        /// <summary>
        /// Collapses the UI's options menu.
        /// </summary>
        private void _HideOptionsButtons()
        {
            projectButtonOptionsGrid.Visibility = Visibility.Hidden;
            vehicleButtonOptionsGrid.Visibility = Visibility.Hidden;
            pathButtonOptionsGrid.Visibility = Visibility.Hidden;
            simulationButtonOptionsGrid.Visibility = Visibility.Hidden;
            resultsButtonOptionsGrid.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Makes th main navigation buttons background transparent.
        /// </summary>
        private void _ResetMainNavigationsButtonsBackground()
        {
            projectEnvironmentButton.Background = Brushes.Transparent;
            vehicleInputEnvironmentButton.Background = Brushes.Transparent;
            pathInputEnvironmentButton.Background = Brushes.Transparent;
            simulationInputEnvironmentButton.Background = Brushes.Transparent;
            resultsAnalysisEnvironmentButton.Background = Brushes.Transparent;
            helpEnvironmentButton.Background = Brushes.Transparent;
        }

        private void _ResetOptionsButtonsBackground()
        {
            aerodynamicsButton.Background = Brushes.Transparent;
            brakesButton.Background = Brushes.Transparent;
            engineButton.Background = Brushes.Transparent;
            inertiaAndDimensionsButton.Background = Brushes.Transparent;
            suspensionAndSteeringButton.Background = Brushes.Transparent;
            tireButton.Background = Brushes.Transparent;
            transmissionButton.Background = Brushes.Transparent;
            oneWheelVehicleButton.Background = Brushes.Transparent;
            twoWheelVehicleButton.Background = Brushes.Transparent;
            tabularPathButton.Background = Brushes.Transparent;
            ggvDiagramButton.Background = Brushes.Transparent;
            lapTimeSimulationButton.Background = Brushes.Transparent;
            resultsAnalysis2DChartButton.Background = Brushes.Transparent;
            resultsAnalysisTrackMapButton.Background = Brushes.Transparent;
        }

        #endregion
        #region Main Controls Options Methods

        /// <summary>
        /// Collapses all of the work environments of the main work environment 
        /// </summary>
        /// <param name="dockingManager"></param>
        private void _CollapseMainDockingManagerContent()
        {
            aerodynamicsInputDockingManager.Visibility = Visibility.Collapsed;
            brakesInputDockingManager.Visibility = Visibility.Collapsed;
            engineInputDockingManager.Visibility = Visibility.Collapsed;
            inertiaAndDimensionsInputDockingManager.Visibility = Visibility.Collapsed;
            suspensionAndSteeringInputDockingManager.Visibility = Visibility.Collapsed;
            tireInputDockingManager.Visibility = Visibility.Collapsed;
            transmissionInputDockingManager.Visibility = Visibility.Collapsed;
            oneWheelVehicleInputDockingManager.Visibility = Visibility.Collapsed;
            twoWheelVehicleInputDockingManager.Visibility = Visibility.Collapsed;
            fourWheelVehicleInputDockingManager.Visibility = Visibility.Collapsed;
            tabularPathInputDockingManager.Visibility = Visibility.Collapsed;
            drawPathInputDockingManager.Visibility = Visibility.Collapsed;
            optimizePathInputDockingManager.Visibility = Visibility.Collapsed;
            ggvDiagramDockingManager.Visibility = Visibility.Collapsed;
            lapTimeSimulationDockingManager.Visibility = Visibility.Collapsed;
            resultsAnalysis2DChartsGrid.Visibility = Visibility.Collapsed;
            resultsAnalysisTrackMapsGrid.Visibility = Visibility.Collapsed;
        }

        #region Project Button Options Methods

        /// <summary>
        /// Clears the UI, so that a new project can be defined.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _NewProjectButton_Click(object sender, RoutedEventArgs e)
        {
            _ClearUILists();
            _ResetOptionsButtonsBackground();
        }

        /// <summary>
        /// Opens a dialog box where the user will choose a file (or create a new one) to save the
        /// project's objects which are contained in the UI lists.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _SaveProjectButton_Click(object sender, RoutedEventArgs e)
        {
            // Initializes the save dialog box
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                Title = "Save Project",
                RestoreDirectory = true,
                DefaultExt = "xml",
                AddExtension = true,
                Filter = "xml files (*.xml)|*.xml"
            };
            // Displays the save file dialog box
            saveFileDialog.ShowDialog();
            // Checks if the file name is not an empty string
            if (saveFileDialog.FileName != "")
            {
                // Creates the project object
                Project project = new Project();
                // Adds the lists objects to the project
                // Aerodynamics
                foreach (Vehicle.OneWheelAerodynamics oneWheelAerodynamics in oneWheelAerodynamicsListBox.Items)
                    project.OneWheelAerodynamics.Add(oneWheelAerodynamics);
                foreach (Vehicle.OneWheelAerodynamicMap oneWheelAerodynamicMaps in oneWheelAerodynamicMapsListBox.Items)
                    project.OneWheelAerodynamicMaps.Add(oneWheelAerodynamicMaps);
                foreach (Vehicle.OneWheelAerodynamicMapPoint oneWheelAerodynamicMapPoint in oneWheelAerodynamicMapPointsListBox.Items)
                    project.OneWheelAerodynamicMapPoints.Add(oneWheelAerodynamicMapPoint);
                foreach (Vehicle.TwoWheelAerodynamics twoWheelAerodynamics in twoWheelAerodynamicsListBox.Items)
                    project.TwoWheelAerodynamics.Add(twoWheelAerodynamics);
                foreach (Vehicle.TwoWheelAerodynamicMap twoWheelAerodynamicMaps in twoWheelAerodynamicMapsListBox.Items)
                    project.TwoWheelAerodynamicMaps.Add(twoWheelAerodynamicMaps);
                foreach (Vehicle.TwoWheelAerodynamicMapPoint twoWheelAerodynamicMapPoints in twoWheelAerodynamicMapPointsListBox.Items)
                    project.TwoWheelAerodynamicMapPoints.Add(twoWheelAerodynamicMapPoints);
                // Brakes
                foreach (Vehicle.OneWheelBrakes oneWheelBrakes in oneWheelBrakesListBox.Items)
                    project.OneWheelBrakes.Add(oneWheelBrakes);
                foreach (Vehicle.TwoWheelBrakes twoWheelBrakes in twoWheelBrakesListBox.Items)
                    project.TwoWheelBrakes.Add(twoWheelBrakes);
                // Engine
                foreach (Vehicle.Engine engine in engineListBox.Items)
                    project.Engines.Add(engine);
                foreach (Vehicle.EngineCurves engineCurve in engineCurvesListBox.Items)
                    project.EngineCurves.Add(engineCurve);
                foreach (Vehicle.EngineCurvesPoint engineCurvePoint in engineCurvesPointsListBox.Items)
                    project.EngineCurvesPoints.Add(engineCurvePoint);
                // Inertia
                foreach (Vehicle.Inertia inertia in oneWheelInertiaListBox.Items)
                    project.Inertias.Add(inertia);
                foreach (Vehicle.TwoWheelInertiaAndDimensions twoWheelInertiaAndDimensions in twoWheelInertiaAndDimensionsListBox.Items)
                    project.TwoWheelInertiaAndDimensions.Add(twoWheelInertiaAndDimensions);
                // Suspension And Tires
                foreach (Vehicle.SimplifiedSuspension simplifiedSuspension in simplifiedSuspensionListBox.Items)
                    project.SimplifiedSuspensions.Add(simplifiedSuspension);
                foreach (Vehicle.SteeringSystem steeringSystem in steeringSystemListBox.Items)
                    project.SteeringSystems.Add(steeringSystem);
                // Tires
                foreach (Vehicle.Tire tire in tireListBox.Items)
                    project.Tires.Add(tire);
                foreach (Vehicle.TireModelMF52 tireModel in tireModelListBox.Items)
                    project.TireModelMF52s.Add(tireModel);
                foreach (Vehicle.TireModelMF52Point tireModelPoint in tireModelDisplayParameterSetsCheckListBox.Items)
                    project.TireModelMF52Points.Add(tireModelPoint);
                // Transmission
                foreach (Vehicle.OneWheelTransmission oneWheelTransmission in oneWheelTransmissionListBox.Items)
                    project.OneWheelTransmissions.Add(oneWheelTransmission);
                foreach (Vehicle.TwoWheelTransmission twoWheelTransmission in twoWheelTransmissionListBox.Items)
                    project.TwoWheelTransmissions.Add(twoWheelTransmission);
                foreach (Vehicle.GearRatiosSet gearRatiosSet in gearRatiosSetsListBox.Items)
                    project.GearRatiosSets.Add(gearRatiosSet);
                foreach (Vehicle.GearRatio gearRatio in gearRatiosListBox.Items)
                    project.GearRatios.Add(gearRatio);
                // One Wheel Car
                foreach (Vehicle.OneWheelCar oneWheelCar in oneWheelCarAndSetupListBox.Items)
                    project.OneWheelCars.Add(oneWheelCar);
                // Two Wheel Car
                foreach (Vehicle.TwoWheelCar twoWheelCar in twoWheelCarAndSetupListBox.Items)
                    project.TwoWheelCars.Add(twoWheelCar);
                // Tabular Path
                foreach (Path path in tabularPathsListBox.Items)
                    project.TabularPaths.Add(path);
                foreach (PathSectorsSet pathSectorSet in tabularPathSectorsSetsListBox.Items)
                    project.TabularPathSectorsSets.Add(pathSectorSet);
                foreach (PathSector pathSector in tabularPathSectorsStartDistancesListBox.Items)
                    project.TabularPathSectors.Add(pathSector);
                foreach (TabularPathSectionsSet pathSectionsSet in tabularPathSectionsSetsListBox.Items)
                    project.TabularPathSectionsSets.Add(pathSectionsSet);
                foreach (TabularPathSection pathSection in tabularPathSectionsListBox.Items)
                    project.TabularPathSections.Add(pathSection);
                // GGV Diagrams
                foreach (Simulation.GGVDiagram ggvDiagram in simulationGGVDiagramListBox.Items)
                    project.GGVDiagrams.Add(ggvDiagram);
                // Lap Time Simulations
                foreach (Simulation.LapTimeSimulation lapTimeSimulation in lapTimeSimulationListBox.Items)
                    project.LapTimeSimulations.Add(lapTimeSimulation);
                // Lap Time Simulation Results
                foreach (Results.LapTimeSimulationResults lapTimeSimulationResults in lapTimeSimulationResultsAnalysisResultsListBox.Items)
                    project.LapTimeSimulationResults.Add(lapTimeSimulationResults);
                // Saves the project to the file
                project.Save(saveFileDialog.FileName);
            }

        }

        /// <summary>
        /// Opens a dialog box where the user chooses the project's file to be loaded. Then,
        /// the UI's listboxes get cleared and filled by the file's objects.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _LoadProjectButton_Click(object sender, RoutedEventArgs e)
        {
            // Initializes the open file dialog box
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Title = "Load Project",
                RestoreDirectory = true
            };
            // Displays the open file dialog box
            openFileDialog.ShowDialog();
            // Initializes and loads the project object
            Project project = new Project();
            project = project.Load(openFileDialog.FileName);
            // Clears the UI lists and loads the file's objects
            _ClearUILists();
            // Aerodynamics
            foreach (Vehicle.OneWheelAerodynamics oneWheelAerodynamics in project.OneWheelAerodynamics)
                oneWheelAerodynamicsListBox.Items.Add(oneWheelAerodynamics);
            foreach (Vehicle.OneWheelAerodynamicMap oneWheelAerodynamicMaps in project.OneWheelAerodynamicMaps)
                oneWheelAerodynamicMapsListBox.Items.Add(oneWheelAerodynamicMaps);
            foreach (Vehicle.OneWheelAerodynamicMapPoint oneWheelAerodynamicMapPoint in project.OneWheelAerodynamicMapPoints)
                oneWheelAerodynamicMapPointsListBox.Items.Add(oneWheelAerodynamicMapPoint);
            foreach (Vehicle.TwoWheelAerodynamics twoWheelAerodynamics in project.TwoWheelAerodynamics)
                twoWheelAerodynamicsListBox.Items.Add(twoWheelAerodynamics);
            foreach (Vehicle.TwoWheelAerodynamicMap twoWheelAerodynamicMaps in project.TwoWheelAerodynamicMaps)
                twoWheelAerodynamicMapsListBox.Items.Add(twoWheelAerodynamicMaps);
            foreach (Vehicle.TwoWheelAerodynamicMapPoint twoWheelAerodynamicMapPoints in project.TwoWheelAerodynamicMapPoints)
                twoWheelAerodynamicMapPointsListBox.Items.Add(twoWheelAerodynamicMapPoints);
            // Brakes
            foreach (Vehicle.OneWheelBrakes oneWheelBrakes in project.OneWheelBrakes)
                oneWheelBrakesListBox.Items.Add(oneWheelBrakes);
            foreach (Vehicle.TwoWheelBrakes twoWheelBrakes in project.TwoWheelBrakes)
                twoWheelBrakesListBox.Items.Add(twoWheelBrakes);
            // Engine
            foreach (Vehicle.Engine engine in project.Engines)
                engineListBox.Items.Add(engine);
            foreach (Vehicle.EngineCurves engineCurve in project.EngineCurves)
                engineCurvesListBox.Items.Add(engineCurve);
            foreach (Vehicle.EngineCurvesPoint engineCurvePoint in project.EngineCurvesPoints)
                engineCurvesPointsListBox.Items.Add(engineCurvePoint);
            // Inertia
            foreach (Vehicle.Inertia inertia in project.Inertias)
                oneWheelInertiaListBox.Items.Add(inertia);
            foreach (Vehicle.TwoWheelInertiaAndDimensions twoWheelInertiaAndDimensions in project.TwoWheelInertiaAndDimensions)
                twoWheelInertiaAndDimensionsListBox.Items.Add(twoWheelInertiaAndDimensions);
            // Suspension And Tires
            foreach (Vehicle.SimplifiedSuspension simplifiedSuspension in project.SimplifiedSuspensions)
                simplifiedSuspensionListBox.Items.Add(simplifiedSuspension);
            foreach (Vehicle.SteeringSystem steeringSystem in project.SteeringSystems)
                steeringSystemListBox.Items.Add(steeringSystem);
            // Tires
            foreach (Vehicle.Tire tire in project.Tires)
                tireListBox.Items.Add(tire);
            foreach (Vehicle.TireModelMF52 tireModel in project.TireModelMF52s)
            {
                tireModelListBox.Items.Add(tireModel);
                tireModelDisplayCheckListBox.Items.Add(tireModel);
            }
            foreach (Vehicle.TireModelMF52Point tireModelPoint in project.TireModelMF52Points)
                tireModelDisplayParameterSetsCheckListBox.Items.Add(tireModelPoint);
            // Transmission
            foreach (Vehicle.OneWheelTransmission oneWheelTransmission in project.OneWheelTransmissions)
                oneWheelTransmissionListBox.Items.Add(oneWheelTransmission);
            foreach (Vehicle.TwoWheelTransmission twoWheelTransmission in project.TwoWheelTransmissions)
                twoWheelTransmissionListBox.Items.Add(twoWheelTransmission);
            foreach (Vehicle.GearRatiosSet gearRatiosSet in project.GearRatiosSets)
                gearRatiosSetsListBox.Items.Add(gearRatiosSet);
            foreach (Vehicle.GearRatio gearRatio in project.GearRatios)
                gearRatiosListBox.Items.Add(gearRatio);
            // Path
            foreach (Path path in project.TabularPaths)
                tabularPathsListBox.Items.Add(path);
            foreach (PathSectorsSet pathSectorSet in project.TabularPathSectorsSets)
                tabularPathSectorsSetsListBox.Items.Add(pathSectorSet);
            foreach (PathSector pathSector in project.TabularPathSectors)
                tabularPathSectorsStartDistancesListBox.Items.Add(pathSector);
            foreach (TabularPathSectionsSet pathSectionsSet in project.TabularPathSectionsSets)
                tabularPathSectionsSetsListBox.Items.Add(pathSectionsSet);
            foreach (TabularPathSection pathSection in project.TabularPathSections)
                tabularPathSectionsListBox.Items.Add(pathSection);
            // One Wheel Car
            foreach (Vehicle.OneWheelCar oneWheelCar in project.OneWheelCars)
                oneWheelCarAndSetupListBox.Items.Add(oneWheelCar);
            // Two Wheel Car
            foreach (Vehicle.TwoWheelCar twoWheelCar in project.TwoWheelCars)
                twoWheelCarAndSetupListBox.Items.Add(twoWheelCar);
            // GGV Diagrams
            foreach (Simulation.GGVDiagram ggvDiagram in project.GGVDiagrams)
                simulationGGVDiagramListBox.Items.Add(ggvDiagram);
            // Lap Time Simulations
            foreach (Simulation.LapTimeSimulation lapTimeSimulation in project.LapTimeSimulations)
                lapTimeSimulationListBox.Items.Add(lapTimeSimulation);
            // Lap Time Simulation Results
            foreach (Results.LapTimeSimulationResults lapTimeSimulationResults in project.LapTimeSimulationResults)
            {
                lapTimeSimulationResultsAnalysisResultsListBox.Items.Add(lapTimeSimulationResults);
                lapTimeSimulationResultsAnalysisResultsComboBox.Items.Add(lapTimeSimulationResults);
            }
        }

        /// <summary>
        /// Clears all the lists of the UI.
        /// </summary>
        private void _ClearUILists()
        {
            // Clears the UI lists
            // Aerodynamics
            oneWheelAerodynamicsListBox.Items.Clear();
            twoWheelAerodynamicsListBox.Items.Clear();
            oneWheelAerodynamicMapsListBox.Items.Clear();
            oneWheelAerodynamicMapPointsListBox.Items.Clear();
            twoWheelAerodynamicMapsListBox.Items.Clear();
            twoWheelAerodynamicMapPointsListBox.Items.Clear();
            // Brakes
            oneWheelBrakesListBox.Items.Clear();
            twoWheelBrakesListBox.Items.Clear();
            // Engine
            engineListBox.Items.Clear();
            engineCurvesListBox.Items.Clear();
            engineCurvesPointsListBox.Items.Clear();
            // Inertia
            oneWheelInertiaListBox.Items.Clear();
            twoWheelInertiaAndDimensionsListBox.Items.Clear();
            // Suspension
            simplifiedSuspensionListBox.Items.Clear();
            steeringSystemListBox.Items.Clear();
            // Tire
            tireListBox.Items.Clear();
            tireModelListBox.Items.Clear();
            tireModelDisplayCheckListBox.Items.Clear();
            tireModelDisplayParameterSetsCheckListBox.Items.Clear();
            // Transmission
            oneWheelTransmissionListBox.Items.Clear();
            twoWheelTransmissionListBox.Items.Clear();
            gearRatiosSetsListBox.Items.Clear();
            gearRatiosListBox.Items.Clear();
            // One Wheel Model Cars
            oneWheelCarAndSetupListBox.Items.Clear();
            // Two Wheel Model Cars
            twoWheelCarAndSetupListBox.Items.Clear();
            // Four Wheel Model Cars
            // Tabular Paths
            tabularPathsListBox.Items.Clear();
            tabularPathSectorsSetsListBox.Items.Clear();
            tabularPathSectorsStartDistancesListBox.Items.Clear();
            tabularPathSectionsSetsListBox.Items.Clear();
            tabularPathSectionsListBox.Items.Clear();
            // Drawn Paths
            // Optimization Paths
            // GGV Diagrams
            simulationGGVDiagramListBox.Items.Clear();
            // Lap Time Simulations
            lapTimeSimulationListBox.Items.Clear();
            lapTimeSimulationGGVDiagramPerSectorListBox.Items.Clear();
        }
        
        #endregion
        #region Vehicle Button Options Methods

        /// <summary>
        /// Changes the work environment to the aerodynamics input environment.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _AerodynamicsButton_Click(object sender, RoutedEventArgs e)
        {
            _CollapseMainDockingManagerContent();
            aerodynamicsInputDockingManager.Visibility = Visibility.Visible;
            _ResetOptionsButtonsBackground();
            aerodynamicsButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF3B4554"));
        }

        /// <summary>
        /// Changes the work environment to the brakes input environment.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _BrakesButton_Click(object sender, RoutedEventArgs e)
        {
            _CollapseMainDockingManagerContent();
            brakesInputDockingManager.Visibility = Visibility.Visible;
            _ResetOptionsButtonsBackground();
            brakesButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF3B4554"));
        }

        /// <summary>
        /// Changes the work environment to the engine input environment.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _EngineButton_Click(object sender, RoutedEventArgs e)
        {
            _CollapseMainDockingManagerContent();
            engineInputDockingManager.Visibility = Visibility.Visible;
            _ResetOptionsButtonsBackground();
            engineButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF3B4554"));
        }

        /// <summary>
        /// Changes the work environment to the inertia model input environment.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _InertiaAndDimensionsButton_Click(object sender, RoutedEventArgs e)
        {
            _CollapseMainDockingManagerContent();
            inertiaAndDimensionsInputDockingManager.Visibility = Visibility.Visible;
            _ResetOptionsButtonsBackground();
            inertiaAndDimensionsButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF3B4554"));
        }

        /// <summary>
        /// Changes the work environment to the suspension and steering input environment.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _SuspensionAndSteeringButton_Click(object sender, RoutedEventArgs e)
        {
            _CollapseMainDockingManagerContent();
            suspensionAndSteeringInputDockingManager.Visibility = Visibility.Visible;
            _ResetOptionsButtonsBackground();
            suspensionAndSteeringButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF3B4554"));
        }

        /// <summary>
        /// Changes the work environment to the tire model input environment.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _TireButton_Click(object sender, RoutedEventArgs e)
        {
            _CollapseMainDockingManagerContent();
            tireInputDockingManager.Visibility = Visibility.Visible;
            _ResetOptionsButtonsBackground();
            tireButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF3B4554"));
        }

        /// <summary>
        /// Changes the work environment to the transmission model input environment.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _TransmissionButton_Click(object sender, RoutedEventArgs e)
        {
            _CollapseMainDockingManagerContent();
            transmissionInputDockingManager.Visibility = Visibility.Visible;
            _ResetOptionsButtonsBackground();
            transmissionButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF3B4554"));
        }
        /// <summary>
        /// Changes the work environment to the One Wheel vehicle model input environment.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _OneWheelVehicleButton_Click(object sender, RoutedEventArgs e)
        {
            _CollapseMainDockingManagerContent();
            oneWheelVehicleInputDockingManager.Visibility = Visibility.Visible;
            _ResetOptionsButtonsBackground();
            oneWheelVehicleButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF3B4554"));
        }
        /// <summary>
        /// Changes the work environment to the Two Wheel vehicle model input environment.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _TwoWheelVehicleButton_Click(object sender, RoutedEventArgs e)
        {
            _CollapseMainDockingManagerContent();
            twoWheelVehicleInputDockingManager.Visibility = Visibility.Visible;
            _ResetOptionsButtonsBackground();
            twoWheelVehicleButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF3B4554"));
        }
        /// <summary>
        /// Changes the work environment to the Four Wheel vehicle model input environment.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _FourWheelVehicleButton_Click(object sender, RoutedEventArgs e)
        {
            _CollapseMainDockingManagerContent();
            fourWheelVehicleInputDockingManager.Visibility = Visibility.Visible;
        }
        #endregion
        #region Path Button Options Methods
        /// <summary>
        /// Changes the work environment to the Tabular Path input environment.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _TabularPathButton_Click(object sender, RoutedEventArgs e)
        {
            _CollapseMainDockingManagerContent();
            tabularPathInputDockingManager.Visibility = Visibility.Visible;
            _ResetOptionsButtonsBackground();
            tabularPathButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF3B4554"));
        }
        /// <summary>
        /// Changes the work environment to the Path Drawing environment.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _DrawPathButton_Click(object sender, RoutedEventArgs e)
        {
            _CollapseMainDockingManagerContent();
            drawPathInputDockingManager.Visibility = Visibility.Visible;
        }
        /// <summary>
        /// Changes the work environment to the Path Optimization environment.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _OptimizePathButton_Click(object sender, RoutedEventArgs e)
        {
            _CollapseMainDockingManagerContent();
            optimizePathInputDockingManager.Visibility = Visibility.Visible;
        }
        #endregion
        #region Simulation Button Options Methods
        /// <summary>
        /// Changes the work environment to the GGV Diagram environment.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _GGVDiagramButton_Click(object sender, RoutedEventArgs e)
        {
            _CollapseMainDockingManagerContent();
            ggvDiagramDockingManager.Visibility = Visibility.Visible;
            _ResetOptionsButtonsBackground();
            ggvDiagramButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF3B4554"));
        }
        /// <summary>
        /// Changes the work environment to the Lap Time Simulation environment.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _LapTimeSimulationButton_Click(object sender, RoutedEventArgs e)
        {
            _CollapseMainDockingManagerContent();
            lapTimeSimulationDockingManager.Visibility = Visibility.Visible;
            _ResetOptionsButtonsBackground();
            lapTimeSimulationButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF3B4554"));
        }
        #endregion
        #region Results Analysis Button Methods
        /// <summary>
        /// Changes the work environment to the 2D Chart results analysis environment.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _ResultsAnalysis2DChartButton_Click(object sender, RoutedEventArgs e)
        {
            _CollapseMainDockingManagerContent();
            resultsAnalysis2DChartsGrid.Visibility = Visibility.Visible;
            _ResetOptionsButtonsBackground();
            resultsAnalysis2DChartButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF3B4554"));
        }
        /// <summary>
        /// Changes the work environment to thetrack map results analysis environment.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _ResultsAnalysisTrackMapButton_Click(object sender, RoutedEventArgs e)
        {
            _CollapseMainDockingManagerContent();
            resultsAnalysisTrackMapsGrid.Visibility = Visibility.Visible;
            _ResetOptionsButtonsBackground();
            resultsAnalysisTrackMapButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF3B4554"));
        }
        #endregion
        #endregion
        #endregion

        #region Vehicle Input Methods

        #region Aerodynamics
        #region Aerodynamics

        /// <summary>
        /// Creates an aerodynamics object and adds it to the aerodynamics listbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _AddAerodynamicsToListBox_Click(object sender, RoutedEventArgs e)
        {
            if (aerodynamicsVehicleModelComboBox.SelectedItem != null)
            {
                switch (aerodynamicsVehicleModelComboBox.SelectedValue.ToString())
                {
                    case "One Wheel":
                        _AddOneWheelAerodynamicsToListBox();
                        break;
                    case "Two Wheel":
                        _AddTwoWheelAerodynamicsToListBox();
                        break;
                    default:
                        break;
                }
            }
            else System.Windows.MessageBox.Show(
               "Please, select a vehicle model.",
               "Error",
               MessageBoxButton.OK,
               MessageBoxImage.Error);
        }

        /// <summary>
        /// Deletes an aerodynamics from the aerodynamics listbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _DeleteAerodynamicsOfListBox_Click(object sender, RoutedEventArgs e)
        {
            if (aerodynamicsVehicleModelComboBox.SelectedItem != null)
            {
                switch (aerodynamicsVehicleModelComboBox.SelectedValue.ToString())
                {
                    case "One Wheel":
                        _DeleteOneWheelAerodynamicsOfListBox();
                        break;
                    case "Two Wheel":
                        _DeleteTwoWheelAerodynamicsOfListBox();
                        break;
                    default:
                        break;
                }
            }
            else System.Windows.MessageBox.Show(
               "Please, select a vehicle model.",
               "Error",
               MessageBoxButton.OK,
               MessageBoxImage.Error);
        }

        /// <summary>
        /// Updates the aerodynamic maps ComboBox items an displays changes the aerodynamics listbox to match the selection in the vehicle model selction ComboBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _AerodynamicsVehicleModelComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            switch (aerodynamicsVehicleModelComboBox.SelectedValue.ToString())
            {
                case "One Wheel":
                    _CollapseAerodynamicListBoxes();
                    oneWheelAerodynamicsListBox.Visibility = Visibility.Visible;
                    aerodynamicMapComboBox.ItemsSource = oneWheelAerodynamicMapsListBox.Items;
                    break;
                case "Two Wheel":
                    _CollapseAerodynamicListBoxes();
                    twoWheelAerodynamicsListBox.Visibility = Visibility.Visible;
                    aerodynamicMapComboBox.ItemsSource = twoWheelAerodynamicMapsListBox.Items;
                    break;
                default:
                    _CollapseAerodynamicListBoxes();
                    aerodynamicMapComboBox.ItemsSource = null;
                    break;
            }
        }

        /// <summary>
        /// Collapses the aerodynamics listboxes.
        /// </summary>
        private void _CollapseAerodynamicListBoxes()
        {
            oneWheelAerodynamicsListBox.Visibility = Visibility.Collapsed;
            twoWheelAerodynamicsListBox.Visibility = Visibility.Collapsed;
        }

        #region One Wheel Model
        /// <summary>
        /// Creates an aerodynamics object and adds it to the one wheel model aerodynamics listbox.
        /// </summary>
        private void _AddOneWheelAerodynamicsToListBox()
        {
            if (aerodynamicsIDTextBox.Text != "" &&
                aerodynamicMapComboBox.SelectedItem != null)
            {
                // Gets the object's data
                string aerodynamicID = aerodynamicsIDTextBox.Text;
                string description = aerodynamicsDescriptionTextBox.Text;
                Vehicle.OneWheelAerodynamicMap aerodynamicMap = aerodynamicMapComboBox.SelectedItem as Vehicle.OneWheelAerodynamicMap;
                double frontalArea = double.Parse(frontalAreaTextBox.Text);
                double airDensity = double.Parse(airDensityTextBox.Text);
                // Initializes a new object
                Vehicle.OneWheelAerodynamics aerodynamics = new Vehicle.OneWheelAerodynamics(aerodynamicID, description, aerodynamicMap, frontalArea, airDensity);
                aerodynamics.GetAerodynamicMapParameters();
                // Adds the object to the listbox and the ComboBox
                oneWheelAerodynamicsListBox.Items.Add(aerodynamics);
            }
            else System.Windows.MessageBox.Show(
                "Could not create Aerodynamics. \n " +
                "   It should have an ID. \n" +
                "   An aerodynamic map must be selected.",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }

        /// <summary>
        /// Deletes an aerodynamics from the one wheel model aerodynamics listbox.
        /// </summary>
        private void _DeleteOneWheelAerodynamicsOfListBox()
        {
            // Checks if there's a listbox item selected and then removes it
            if (oneWheelAerodynamicsListBox.SelectedItems.Count == 1)
            {
                oneWheelAerodynamicsListBox.Items.RemoveAt(oneWheelAerodynamicsListBox.Items.IndexOf(oneWheelAerodynamicsListBox.SelectedItem));
            }
        }

        /// <summary>
        /// Loads the properties of the one wheel model aerodynamics listbox's aerodynamics and displays it in the UI fields.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _OneWheelAerodynamicsListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // Checks if there's a listbox item selected
            if (oneWheelAerodynamicsListBox.SelectedItems.Count == 1)
            {
                // Gets the selected object
                Vehicle.OneWheelAerodynamics aerodynamics = oneWheelAerodynamicsListBox.SelectedItem as Vehicle.OneWheelAerodynamics;
                // Writes the properties in the UI
                aerodynamicsIDTextBox.Text = aerodynamics.ID;
                aerodynamicsDescriptionTextBox.Text = aerodynamics.Description;
                aerodynamicMapComboBox.Text = aerodynamics.AerodynamicMap.ToString();
                frontalAreaTextBox.Text = aerodynamics.FrontalArea.ToString("F3");
                airDensityTextBox.Text = aerodynamics.AirDensity.ToString("F3");
            }
        }
        #endregion

        #region Two Wheel Model
        /// <summary>
        /// Creates an aerodynamics object and adds it to the two wheel model aerodynamics listbox.
        /// </summary>
        private void _AddTwoWheelAerodynamicsToListBox()
        {
            if (aerodynamicsIDTextBox.Text != "" &&
                aerodynamicMapComboBox.SelectedItem != null)
            {
                // Gets the object's data
                string aerodynamicID = aerodynamicsIDTextBox.Text;
                string description = aerodynamicsDescriptionTextBox.Text;
                Vehicle.TwoWheelAerodynamicMap aerodynamicMap = aerodynamicMapComboBox.SelectedItem as Vehicle.TwoWheelAerodynamicMap;
                double frontalArea = double.Parse(frontalAreaTextBox.Text);
                double airDensity = double.Parse(airDensityTextBox.Text);
                // Initializes a new object
                Vehicle.TwoWheelAerodynamics aerodynamics = new Vehicle.TwoWheelAerodynamics(aerodynamicID, description, aerodynamicMap, frontalArea, airDensity);
                aerodynamics.GetAerodynamicMapParameters();
                // Adds the object to the listbox and the ComboBox
                twoWheelAerodynamicsListBox.Items.Add(aerodynamics);
            }
            else System.Windows.MessageBox.Show(
                "Could not create Aerodynamics. \n " +
                "   It should have an ID. \n" +
                "   An aerodynamic map must be selected.",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }

        /// <summary>
        /// Deletes an aerodynamics from the two wheel model aerodynamics listbox.
        /// </summary>
        private void _DeleteTwoWheelAerodynamicsOfListBox()
        {
            // Checks if there's a listbox item selected and then removes it
            if (twoWheelAerodynamicsListBox.SelectedItems.Count == 1)
            {
                twoWheelAerodynamicsListBox.Items.RemoveAt(twoWheelAerodynamicsListBox.Items.IndexOf(twoWheelAerodynamicsListBox.SelectedItem));
            }
        }

        /// <summary>
        /// Loads the properties of the two wheel model aerodynamics listbox's aerodynamics and displays it in the UI fields.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _TwoWheelAerodynamicsListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // Checks if there's a listbox item selected
            if (twoWheelAerodynamicsListBox.SelectedItems.Count == 1)
            {
                // Gets the selected object
                Vehicle.TwoWheelAerodynamics aerodynamics = twoWheelAerodynamicsListBox.SelectedItem as Vehicle.TwoWheelAerodynamics;
                // Writes the properties in the UI
                aerodynamicsIDTextBox.Text = aerodynamics.ID;
                aerodynamicsDescriptionTextBox.Text = aerodynamics.Description;
                aerodynamicMapComboBox.Text = aerodynamics.AerodynamicMap.ToString();
                frontalAreaTextBox.Text = aerodynamics.FrontalArea.ToString("F3");
                airDensityTextBox.Text = aerodynamics.AirDensity.ToString("F3");
            }
        }

        #endregion
        #endregion
        #region Aerodynamic Map

        #region One Wheel Model

        /// <summary>
        /// Creates an aerodynamic map object and adds it to the one wheel model aerodynamic maps listbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _AddOneWheelAerodynamicMapToListBox_Click(object sender, RoutedEventArgs e)
        {
            if (oneWheelAerodynamicMapIDTextBox.Text != "" &&
                oneWheelAerodynamicMapPointsListBox.Items.Count != 0)
            {
                // Gets the object's properties values
                string mapID = oneWheelAerodynamicMapIDTextBox.Text;
                string description = oneWheelAerodynamicMapDescriptionTextBox.Text;
                List<Vehicle.OneWheelAerodynamicMapPoint> aerodynamicMapPoints = new List<Vehicle.OneWheelAerodynamicMapPoint>();
                foreach (var aerodynamicMapPointItem in oneWheelAerodynamicMapPointsListBox.Items)
                {
                    Vehicle.OneWheelAerodynamicMapPoint aerodynamicMapPoint = aerodynamicMapPointItem as Vehicle.OneWheelAerodynamicMapPoint;
                    aerodynamicMapPoints.Add(aerodynamicMapPoint);
                }
                // Initializes a new object
                Vehicle.OneWheelAerodynamicMap aerodynamicMap = new Vehicle.OneWheelAerodynamicMap(mapID, description, aerodynamicMapPoints);
                // Adds the object to the listbox and the ComboBox
                oneWheelAerodynamicMapsListBox.Items.Add(aerodynamicMap);
            }
            else System.Windows.MessageBox.Show(
                "Could not create Aerodynamic Map. \n " +
                "   It should have an ID. \n" +
                "   The aerodynamic map points list can't be empty.",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }

        /// <summary>
        /// Deletes an aerodynamic map from the one wheel model aerodynamic maps listbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _DeleteOneWheelAerodynamicMapOfListBox_Click(object sender, RoutedEventArgs e)
        {
            // Checks if there's a listbox item selected and then removes it
            if (oneWheelAerodynamicMapsListBox.SelectedItems.Count == 1)
            {
                oneWheelAerodynamicMapsListBox.Items.RemoveAt(oneWheelAerodynamicMapsListBox.Items.IndexOf(oneWheelAerodynamicMapsListBox.SelectedItem));
            }
        }

        /// <summary>
        /// Loads the properties of the one wheel model aerodynamic maps listbox's aerodynamic map and displays it in the UI fields.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _OneWheelAerodynamicMapsListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // Checks if there's a listbox item selected
            if (oneWheelAerodynamicMapsListBox.SelectedItems.Count == 1)
            {
                // Gets the selected object
                Vehicle.OneWheelAerodynamicMap aerodynamicMap = oneWheelAerodynamicMapsListBox.SelectedItem as Vehicle.OneWheelAerodynamicMap;
                // Writes the properties in the UI
                oneWheelAerodynamicMapIDTextBox.Text = aerodynamicMap.ID;
                oneWheelAerodynamicMapDescriptionTextBox.Text = aerodynamicMap.Description;
                // Clears and writes the list in the UI
                oneWheelAerodynamicMapPointsListBox.Items.Clear();
                foreach (Vehicle.OneWheelAerodynamicMapPoint aerodynamicMapPoint in aerodynamicMap.MapPoints)
                {
                    oneWheelAerodynamicMapPointsListBox.Items.Add(aerodynamicMapPoint);
                }
            }
        }

        /// <summary>
        /// Creates an aerodynamic map point object and adds it to the one wheel model aerodynamic map points listbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _AddOneWheelAerodynamicMapPointToListBox_Click(object sender, RoutedEventArgs e)
        {
            // Gets the object's properties values
            double rideHeight = double.Parse(newOneWheelAerodynamicMapPointRideHeightTextBox.Text);
            double dragCoefficient = double.Parse(newOneWheelAerodynamicMapPointDragCoefficientTextBox.Text);
            double liftCoefficient = double.Parse(newOneWheelAerodynamicMapPointLiftCoefficientTextBox.Text);
            // Initializes a new object
            Vehicle.OneWheelAerodynamicMapPoint aerodynamicMapPoint = new Vehicle.OneWheelAerodynamicMapPoint(rideHeight, dragCoefficient, liftCoefficient);
            // Adds the object to the listbox and the ComboBox
            oneWheelAerodynamicMapPointsListBox.Items.Add(aerodynamicMapPoint);
            // Reorders the aerodynamic map points listbox items in ascending order of car height and speed
            List<Vehicle.OneWheelAerodynamicMapPoint> aerodynamicMapPoints = new List<Vehicle.OneWheelAerodynamicMapPoint>();
            foreach (var aerodynamicMapPointItem in oneWheelAerodynamicMapPointsListBox.Items)
            {
                Vehicle.OneWheelAerodynamicMapPoint currentAerodynamicMapPoint = aerodynamicMapPointItem as Vehicle.OneWheelAerodynamicMapPoint;
                aerodynamicMapPoints.Add(currentAerodynamicMapPoint);
            }
            aerodynamicMapPoints = aerodynamicMapPoints.OrderBy(currentAerodynamicMapPoint => currentAerodynamicMapPoint.RideHeight).ToList();
            oneWheelAerodynamicMapPointsListBox.Items.Clear();
            foreach (Vehicle.OneWheelAerodynamicMapPoint currentAerodynamicMapPoint in aerodynamicMapPoints)
            {
                oneWheelAerodynamicMapPointsListBox.Items.Add(currentAerodynamicMapPoint);
            }
        }

        /// <summary>
        /// Deletes a aerodynamic map point from the one wheel model aerodynamic map points listbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _DeleteOneWheelAerodynamicMapPointOfListBox_Click(object sender, RoutedEventArgs e)
        {
            // Checks if there's a listbox item selected and then removes it
            if (oneWheelAerodynamicMapPointsListBox.SelectedItems.Count == 1)
            {
                oneWheelAerodynamicMapPointsListBox.Items.RemoveAt(oneWheelAerodynamicMapPointsListBox.Items.IndexOf(oneWheelAerodynamicMapPointsListBox.SelectedItem));
            }
        }
        #endregion

        #region Two Wheel Model

        /// <summary>
        /// Creates an aerodynamic map object and adds it to the two wheel model aerodynamic maps listbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _TwoWheelAddAerodynamicMapToListBox_Click(object sender, RoutedEventArgs e)
        {
            if (twoWheelAerodynamicMapIDTextBox.Text != "" &&
                twoWheelAerodynamicMapPointsListBox.Items.Count != 0)
            {
                // Gets the object's properties values
                string mapID = twoWheelAerodynamicMapIDTextBox.Text;
                string description = twoWheelAerodynamicMapDescriptionTextBox.Text;
                List<Vehicle.TwoWheelAerodynamicMapPoint> aerodynamicMapPoints = new List<Vehicle.TwoWheelAerodynamicMapPoint>();
                foreach (var aerodynamicMapPointItem in twoWheelAerodynamicMapPointsListBox.Items)
                {
                    Vehicle.TwoWheelAerodynamicMapPoint aerodynamicMapPoint = aerodynamicMapPointItem as Vehicle.TwoWheelAerodynamicMapPoint;
                    aerodynamicMapPoints.Add(aerodynamicMapPoint);
                }
                // Initializes a new object
                Vehicle.TwoWheelAerodynamicMap aerodynamicMap = new Vehicle.TwoWheelAerodynamicMap(mapID, description, aerodynamicMapPoints);
                // Adds the object to the listbox and the ComboBox
                twoWheelAerodynamicMapsListBox.Items.Add(aerodynamicMap);
            }
            else System.Windows.MessageBox.Show(
                "Could not create Aerodynamic Map. \n " +
                "   It should have an ID. \n" +
                "   The aerodynamic map points list can't be empty.",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }

        /// <summary>
        /// Deletes an aerodynamic map from the two wheel model aerodynamic maps listbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _TwoWheelDeleteAerodynamicMapOfListBox_Click(object sender, RoutedEventArgs e)
        {
            // Checks if there's a listbox item selected and then removes it
            if (twoWheelAerodynamicMapsListBox.SelectedItems.Count == 1)
            {
                twoWheelAerodynamicMapsListBox.Items.RemoveAt(twoWheelAerodynamicMapsListBox.Items.IndexOf(twoWheelAerodynamicMapsListBox.SelectedItem));
            }
        }

        /// <summary>
        /// Loads the properties of the two wheel model aerodynamic maps listbox's aerodynamic map and displays it in the UI fields.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _TwoWheelAerodynamicMapsListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // Checks if there's a listbox item selected
            if (twoWheelAerodynamicMapsListBox.SelectedItems.Count == 1)
            {
                // Gets the selected object
                Vehicle.TwoWheelAerodynamicMap aerodynamicMap = twoWheelAerodynamicMapsListBox.SelectedItem as Vehicle.TwoWheelAerodynamicMap;
                // Writes the properties in the UI
                twoWheelAerodynamicMapIDTextBox.Text = aerodynamicMap.ID;
                twoWheelAerodynamicMapDescriptionTextBox.Text = aerodynamicMap.Description;
                // Clears and writes the list in the UI
                twoWheelAerodynamicMapPointsListBox.Items.Clear();
                foreach (Vehicle.TwoWheelAerodynamicMapPoint aerodynamicMapPoint in aerodynamicMap.MapPoints)
                {
                    twoWheelAerodynamicMapPointsListBox.Items.Add(aerodynamicMapPoint);
                }
            }
        }

        /// <summary>
        /// Creates an aerodynamic map point object and adds it to the two wheel model aerodynamic map points listbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _TwoWheelAddAerodynamicMapPointToListBox_Click(object sender, RoutedEventArgs e)
        {
            if (double.Parse(newTwoWheelAerodynamicMapPointDownforceDistributionTextBox.Text) > 0 && double.Parse(newTwoWheelAerodynamicMapPointDownforceDistributionTextBox.Text) <= 100)
            {
                // Gets the object's properties values
                double frontRideHeight = double.Parse(newTwoWheelAerodynamicMapPointFrontRideHeightTextBox.Text);
                double rearRideHeight = double.Parse(newTwoWheelAerodynamicMapPointRearRideHeightTextBox.Text);
                double dragCoefficient = double.Parse(newTwoWheelAerodynamicMapPointDragCoefficientTextBox.Text);
                double liftCoefficient = double.Parse(newTwoWheelAerodynamicMapPointLiftCoefficientTextBox.Text);
                double downforceDistribution = double.Parse(newTwoWheelAerodynamicMapPointDownforceDistributionTextBox.Text);
                // Initializes a new object
                Vehicle.TwoWheelAerodynamicMapPoint aerodynamicMapPoint = new Vehicle.TwoWheelAerodynamicMapPoint(frontRideHeight, rearRideHeight, dragCoefficient, liftCoefficient, downforceDistribution);
                // Adds the object to the listbox and the ComboBox
                twoWheelAerodynamicMapPointsListBox.Items.Add(aerodynamicMapPoint);
                // Reorders the aerodynamic map points listbox items in ascending order of car height and speed
                List<Vehicle.TwoWheelAerodynamicMapPoint> aerodynamicMapPoints = new List<Vehicle.TwoWheelAerodynamicMapPoint>();
                foreach (var aerodynamicMapPointItem in twoWheelAerodynamicMapPointsListBox.Items)
                {
                    Vehicle.TwoWheelAerodynamicMapPoint currentAerodynamicMapPoint = aerodynamicMapPointItem as Vehicle.TwoWheelAerodynamicMapPoint;
                    aerodynamicMapPoints.Add(currentAerodynamicMapPoint);
                }
                aerodynamicMapPoints = aerodynamicMapPoints.OrderBy(currentAerodynamicMapPoint => currentAerodynamicMapPoint.RearRideHeight).ToList();
                aerodynamicMapPoints = aerodynamicMapPoints.OrderBy(currentAerodynamicMapPoint => currentAerodynamicMapPoint.FrontRideHeight).ToList();
                twoWheelAerodynamicMapPointsListBox.Items.Clear();
                foreach (Vehicle.TwoWheelAerodynamicMapPoint currentAerodynamicMapPoint in aerodynamicMapPoints)
                {
                    twoWheelAerodynamicMapPointsListBox.Items.Add(currentAerodynamicMapPoint);
                }
            }
            else System.Windows.MessageBox.Show(
                "Could not create Aerodynamic Map Point. \n " +
                "   The downforce distribution's value must be between 0 and 100.",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }

        /// <summary>
        /// Deletes a aerodynamic map point from the two wheel model aerodynamic map points listbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _TwoWheelDeleteAerodynamicMapPointOfListBox_Click(object sender, RoutedEventArgs e)
        {
            // Checks if there's a listbox item selected and then removes it
            if (twoWheelAerodynamicMapPointsListBox.SelectedItems.Count == 1)
            {
                twoWheelAerodynamicMapPointsListBox.Items.RemoveAt(twoWheelAerodynamicMapPointsListBox.Items.IndexOf(twoWheelAerodynamicMapPointsListBox.SelectedItem));
            }
        }

        #endregion
        #endregion
        #endregion

        #region Brakes

        #region One Wheel Model
        /// <summary>
        /// Creates a one wheel model brakes object and adds it to the one wheel model brakes listbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _AddOneWheelBrakesToListBox_Click(object sender, RoutedEventArgs e)
        {
            if (oneWheelBrakesIDTextBox.Text != "" &&
                double.Parse(oneWheelBrakesMaximumTorqueTextBox.Text) != 0)
            {
                // Gets the object's properties values
                string brakesID = oneWheelBrakesIDTextBox.Text;
                string description = oneWheelBrakesDescriptionTextBox.Text;
                double maxTorque = double.Parse(oneWheelBrakesMaximumTorqueTextBox.Text);
                // Initializes a new object
                Vehicle.OneWheelBrakes brakes = new Vehicle.OneWheelBrakes(brakesID, description, maxTorque);
                // Adds the object to the listbox and the ComboBox
                oneWheelBrakesListBox.Items.Add(brakes);
            }
            else System.Windows.MessageBox.Show(
                "Could not create Brakes. \n " +
                "   It should have an ID. \n" +
                "   The maximum torque can't be zero. \n" +
                "   Note: Negative values are corrected to positive values.",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }

        /// <summary>
        /// Deletes a one wheel model brakes from the one wheel model brakes listbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _DeleteOneWheelBrakesOfListBox_Click(object sender, RoutedEventArgs e)
        {
            // Checks if there's a listbox item selected and then removes it
            if (oneWheelBrakesListBox.SelectedItems.Count == 1)
            {
                oneWheelBrakesListBox.Items.RemoveAt(oneWheelBrakesListBox.Items.IndexOf(oneWheelBrakesListBox.SelectedItem));
            }
        }

        /// <summary>
        /// Loads the properties of a listbox's one wheel model brakes and displays it in the UI fields.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _OneWheelBrakesListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (oneWheelBrakesListBox.SelectedItems.Count == 1)
            {
                // Gets the selected object
                Vehicle.OneWheelBrakes brakes = oneWheelBrakesListBox.SelectedItem as Vehicle.OneWheelBrakes;
                // Writes the properties in the UI
                oneWheelBrakesIDTextBox.Text = brakes.ID;
                oneWheelBrakesDescriptionTextBox.Text = brakes.Description;
                oneWheelBrakesMaximumTorqueTextBox.Text = brakes.MaximumTorque.ToString("F3");
            }
        }
        #endregion

        #region Two Wheel Model
        /// <summary>
        /// Creates a two wheel model brakes object and adds it to the two wheel model brakes listbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _TwoWheelAddBrakesToListBox_Click(object sender, RoutedEventArgs e)
        {
            if (twoWheelBrakesIDTextBox.Text != "" &&
                double.Parse(twoWheelBrakesBrakeBiasTextBox.Text) >= 0 &&
                double.Parse(twoWheelBrakesBrakeBiasTextBox.Text) <= 100 &&
                double.Parse(twoWheelBrakesFrontMaximumTorqueTextBox.Text) != 0 &&
                double.Parse(twoWheelBrakesRearMaximumTorqueTextBox.Text) != 0)
            {
                // Gets the object's properties values
                string brakesID = twoWheelBrakesIDTextBox.Text;
                string description = twoWheelBrakesDescriptionTextBox.Text;
                double brakeBias = double.Parse(twoWheelBrakesBrakeBiasTextBox.Text) / 100;
                double frontMaxTorque = double.Parse(twoWheelBrakesFrontMaximumTorqueTextBox.Text);
                double rearMaxTorque = double.Parse(twoWheelBrakesRearMaximumTorqueTextBox.Text);
                // Initializes a new object
                Vehicle.TwoWheelBrakes brakes = new Vehicle.TwoWheelBrakes(brakesID, description, brakeBias, frontMaxTorque, rearMaxTorque);
                brakes.GetBrakesAuxiliarParameters();
                // Adds the object to the listbox and the ComboBox
                twoWheelBrakesListBox.Items.Add(brakes);
            }
            else System.Windows.MessageBox.Show(
                "Could not create Brakes. \n " +
                "   It should have an ID. \n" +
                "   The brake bias must be between 0 and 100. \n" +
                "   The front/rear maximum torque can't be zero. \n" +
                "   Note: Negative values are corrected to positive values.",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }

        /// <summary>
        /// Deletes a two wheel model brakes from the two wheel model brakes listbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _TwoWheelDeleteBrakesOfListBox_Click(object sender, RoutedEventArgs e)
        {
            // Checks if there's a listbox item selected and then removes it
            if (twoWheelBrakesListBox.SelectedItems.Count == 1)
            {
                twoWheelBrakesListBox.Items.RemoveAt(twoWheelBrakesListBox.Items.IndexOf(twoWheelBrakesListBox.SelectedItem));
            }
        }

        /// <summary>
        /// Loads the properties of a listbox's two wheel model brakes and displays it in the UI fields.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _TwoWheelBrakesListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (twoWheelBrakesListBox.SelectedItems.Count == 1)
            {
                // Gets the selected object
                Vehicle.TwoWheelBrakes brakes = twoWheelBrakesListBox.SelectedItem as Vehicle.TwoWheelBrakes;
                // Writes the properties in the UI
                twoWheelBrakesIDTextBox.Text = brakes.ID;
                twoWheelBrakesDescriptionTextBox.Text = brakes.Description;
                twoWheelBrakesBrakeBiasTextBox.Text = (brakes.BrakeBias * 100).ToString("F3");
                twoWheelBrakesFrontMaximumTorqueTextBox.Text = (Math.Abs(brakes.FrontMaximumTorque)).ToString("F3");
                twoWheelBrakesRearMaximumTorqueTextBox.Text = (Math.Abs(brakes.RearMaximumTorque)).ToString("F3");
            }
        }

        #endregion

        #endregion

        #region Engine
        #region Engine

        /// <summary>
        /// Creates an engine object and adds it to the one wheel model engines listbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _AddEngineToListBox_Click(object sender, RoutedEventArgs e)
        {
            if (engineIDTextBox.Text != "" &&
                engineCurvesComboBox.SelectedItem != null &&
                double.Parse(maxThrottleTextBox.Text) != 0 &&
                double.Parse(fuelDensityTextBox.Text) != 0)
            {
                // Gets the object's data
                string engineID = engineIDTextBox.Text;
                string description = engineDescriptionTextBox.Text;
                Vehicle.EngineCurves engineCurves = engineCurvesComboBox.SelectedItem as Vehicle.EngineCurves;
                double maxThrottle = double.Parse(maxThrottleTextBox.Text) / 100;
                double fuelDensity = double.Parse(fuelDensityTextBox.Text);
                // Initializes a new object
                Vehicle.Engine engine = new Vehicle.Engine(engineID, description, engineCurves, maxThrottle, fuelDensity);
                // Adds the object to the listbox and the ComboBox
                engineListBox.Items.Add(engine);
            }
            else System.Windows.MessageBox.Show(
                "Could not create Engine. \n " +
                "   It should have an ID. \n" +
                "   An engine curves set must be selected. \n" +
                "   The maximum throttle can't be zero. \n" +
                "   The fuel density can't be zero.",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }

        /// <summary>
        /// Deletes an engine from the one wheel model engines listbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _DeleteEngineOfListBox_Click(object sender, RoutedEventArgs e)
        {
            // Checks if there's a listbox item selected and then removes it
            if (engineListBox.SelectedItems.Count == 1)
            {
                engineListBox.Items.RemoveAt(engineListBox.Items.IndexOf(engineListBox.SelectedItem));
            }
        }

        /// <summary>
        /// Loads the properties of the one wheel model engines listbox's engine and displays it in the UI fields.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _EngineListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // Checks if there's a listbox item selected
            if (engineListBox.SelectedItems.Count == 1)
            {
                // Gets the selected object
                Vehicle.Engine engine = engineListBox.SelectedItem as Vehicle.Engine;
                // Writes the properties in the UI
                engineIDTextBox.Text = engine.ID;
                engineDescriptionTextBox.Text = engine.Description;
                engineCurvesComboBox.Text = engine.EngineCurves.ToString();
                maxThrottleTextBox.Text = (engine.MaxThrottle * 100).ToString("F3");
                fuelDensityTextBox.Text = engine.FuelDensity.ToString("F3");
            }
        }

        #endregion
        #region Engine Curves

        /// <summary>
        /// Creates an engine curves object and adds it to the one wheel model engine curves listbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _AddEngineCurvesToListBox_Click(object sender, RoutedEventArgs e)
        {
            if (engineCurvesIDTextBox.Text != "" &&
                engineCurvesPointsListBox.Items.Count != 0)
            {
                // Gets the object's properties values
                string curvesID = engineCurvesIDTextBox.Text;
                string description = engineCurvesDescriptionTextBox.Text;
                List<Vehicle.EngineCurvesPoint> engineCurvesPoints = new List<Vehicle.EngineCurvesPoint>();
                foreach (var engineCurvesPointItem in engineCurvesPointsListBox.Items)
                {
                    Vehicle.EngineCurvesPoint engineCurvesPoint = engineCurvesPointItem as Vehicle.EngineCurvesPoint;
                    engineCurvesPoints.Add(engineCurvesPoint);
                }
                // Initializes a new object
                Vehicle.EngineCurves engineCurves = new Vehicle.EngineCurves(curvesID, description, engineCurvesPoints);
                // Adds the object to the listbox and the ComboBox
                engineCurvesListBox.Items.Add(engineCurves);
            }
            else System.Windows.MessageBox.Show(
                "Could not create Engine Curves. \n " +
                "   It should have an ID. \n" +
                "   The engines curves points list can't be empty.",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }

        /// <summary>
        /// Deletes an engine curves from the one wheel model engine curves listbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _DeleteEngineCurvesOfListBox_Click(object sender, RoutedEventArgs e)
        {
            // Checks if there's a listbox item selected and then removes it
            if (engineCurvesListBox.SelectedItems.Count == 1)
            {
                engineCurvesListBox.Items.RemoveAt(engineCurvesListBox.Items.IndexOf(engineCurvesListBox.SelectedItem));
            }
        }

        /// <summary>
        /// Loads the properties of the one wheel model engine curves listbox's engine curves and displays it in the UI fields.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _EngineCurvesListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // Checks if there's a listbox item selected
            if (engineCurvesListBox.SelectedItems.Count == 1)
            {
                // Gets the selected object
                Vehicle.EngineCurves engineCurves = engineCurvesListBox.SelectedItem as Vehicle.EngineCurves;
                // Writes the properties in the UI
                engineCurvesIDTextBox.Text = engineCurves.ID;
                engineCurvesDescriptionTextBox.Text = engineCurves.Description;
                // Clears and writes the list in the UI
                engineCurvesPointsListBox.Items.Clear();
                foreach (Vehicle.EngineCurvesPoint engineCurvesPoint in engineCurves.CurvesPoints)
                {
                    gearRatiosListBox.Items.Add(engineCurvesPoint);
                }
            }
        }

        /// <summary>
        /// Creates an engine curves point object and adds it to the one wheel model engine curves points listbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _AddEngineCurvesPointToListBox_Click(object sender, RoutedEventArgs e)
        {
            if (double.Parse(newEngineCurvesPointTorqueTextBox.Text) != 0)
            {
                // Gets the object's properties values
                double rpm = double.Parse(newEngineCurvesPointRPMTextBox.Text);
                if (rpm < 0) rpm = -rpm;
                double torque = double.Parse(newEngineCurvesPointTorqueTextBox.Text);
                double brakingTorque = double.Parse(newEngineCurvesPointBrakingTorqueTextBox.Text);
                double specFuelCons = double.Parse(newEngineCurvesPointSpecFuelConsTextBox.Text);
                // Initializes a new object
                Vehicle.EngineCurvesPoint engineCurvesPoint = new Vehicle.EngineCurvesPoint(rpm, torque, brakingTorque, specFuelCons);
                // Adds the object to the listbox and the ComboBox
                engineCurvesPointsListBox.Items.Add(engineCurvesPoint);
                // Reorders the engine curves points listbox items in ascending order of rpm
                List<Vehicle.EngineCurvesPoint> engineCurvesPoints = new List<Vehicle.EngineCurvesPoint>();
                foreach (var engineCurvesPointItem in engineCurvesPointsListBox.Items)
                {
                    Vehicle.EngineCurvesPoint currentEngineCurvesPoint = engineCurvesPointItem as Vehicle.EngineCurvesPoint;
                    engineCurvesPoints.Add(currentEngineCurvesPoint);
                }
                engineCurvesPoints = engineCurvesPoints.OrderBy(currentCurvePoint => currentCurvePoint.RotationalSpeed).ToList();
                engineCurvesPointsListBox.Items.Clear();
                foreach (Vehicle.EngineCurvesPoint currentEngineCurvesPoint in engineCurvesPoints)
                {
                    engineCurvesPointsListBox.Items.Add(currentEngineCurvesPoint);
                }
            }
            else System.Windows.MessageBox.Show(
                "Could not create Engine Curves Point. \n " +
                "   The torque can't be zero. \n" +
                "   Note: Negative values are corrected to positive values.",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }

        /// <summary>
        /// Deletes an engine curves points from the one wheel model engine curves points listbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _DeleteEngineCurvesPointOfListBox_Click(object sender, RoutedEventArgs e)
        {
            // Checks if there's a listbox item selected and then removes it
            if (engineCurvesPointsListBox.SelectedItems.Count == 1)
            {
                engineCurvesPointsListBox.Items.RemoveAt(engineCurvesPointsListBox.Items.IndexOf(engineCurvesPointsListBox.SelectedItem));
            }
        }

        #endregion
        #endregion

        #region Inertia And Dimensions

        #region One Wheel Inertia
        /// <summary>
        /// Creates a one wheel model inertia object and adds it to the one wheel model inertia listbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _AddOneWheelInertiaToListBox_Click(object sender, RoutedEventArgs e)
        {
            if (oneWheelInertiaIDTextBox.Text != "" &&
                double.Parse(oneWheelTotalMassTextBox.Text) != 0 &&
                double.Parse(oneWheelGravityAccelTextBox.Text) != 0)
            {
                // Gets the object's properties values
                string inertiaID = oneWheelInertiaIDTextBox.Text;
                string description = oneWheelInertiaDescriptionTextBox.Text;
                double totalMass = double.Parse(oneWheelTotalMassTextBox.Text);
                double unsprungMass = double.Parse(oneWheelUnsprungMassTextBox.Text);
                double rotPartsMI = double.Parse(oneWheelRotPartsMITextBox.Text);
                double gravity = double.Parse(oneWheelGravityAccelTextBox.Text);
                // Initializes a new object
                Vehicle.OneWheelInertia inertia = new Vehicle.OneWheelInertia(inertiaID, description, totalMass, unsprungMass, rotPartsMI, gravity);
                // Adds the object to the listbox
                oneWheelInertiaListBox.Items.Add(inertia);
            }
            else System.Windows.MessageBox.Show(
                "Could not create Inertia. \n" +
                "   It should have an ID. \n" +
                "   The total mass can't be zero. \n" +
                "   Gravity can't be zero. \n" +
                "   Note: Negative values are corrected to positive values.",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }

        /// <summary>
        /// Deletes a one wheel model inertia from the one wheel model inertia listbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _DeleteOneWheelInertiaOfListBox_Click(object sender, RoutedEventArgs e)
        {
            // Checks if there's a listbox item selected and then removes it
            if (oneWheelInertiaListBox.SelectedItems.Count == 1)
            {
                oneWheelInertiaListBox.Items.RemoveAt(oneWheelInertiaListBox.Items.IndexOf(oneWheelInertiaListBox.SelectedItem));
            }
        }

        /// <summary>
        /// Loads the properties of a listbox's one wheel model inertia and displays it in the UI fields.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _OneWheelInertiaListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // Checks if there's a listbox item selected
            if (oneWheelInertiaListBox.SelectedItems.Count == 1)
            {
                // Gets the selected object
                Vehicle.OneWheelInertia inertia = oneWheelInertiaListBox.SelectedItem as Vehicle.OneWheelInertia;
                // Writes the properties in the UI
                oneWheelInertiaIDTextBox.Text = inertia.ID;
                oneWheelInertiaDescriptionTextBox.Text = inertia.Description;
                oneWheelTotalMassTextBox.Text = inertia.TotalMass.ToString("F3");
                oneWheelUnsprungMassTextBox.Text = inertia.UnsprungMass.ToString("F3");
                oneWheelRotPartsMITextBox.Text = inertia.RotPartsMI.ToString("F3");
                oneWheelGravityAccelTextBox.Text = inertia.Gravity.ToString("F3");
            }
        }
        #endregion

        #region Two Wheel Inertia And Dimensions
        /// <summary>
        /// Creates a two wheel model inertia and dimensions object and adds it to the two wheel model inertia and dimensions listbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _AddTwoWheelInertiaAndDimensionsToListBox_Click(object sender, RoutedEventArgs e)
        {
            if (twoWheelInertiaAndDimensionsIDTextBox.Text != "" &&
                double.Parse(twoWheelTotalMassDistributionTextBox.Text) > 0 &&
                double.Parse(twoWheelTotalMassDistributionTextBox.Text) < 100 &&
                double.Parse(twoWheelTotalMassTextBox.Text) != 0 &&
                double.Parse(twoWheelWheelbaseTextBox.Text) != 0 &&
                double.Parse(twoWheelGravityAccelTextBox.Text) != 0)
            {
                // Gets the object's properties values
                string inertiaID = twoWheelInertiaAndDimensionsIDTextBox.Text;
                string description = twoWheelInertiaAndDimensionsDescriptionTextBox.Text;
                double totalMass = double.Parse(twoWheelTotalMassTextBox.Text);
                double totalMassDistribution = double.Parse(twoWheelTotalMassDistributionTextBox.Text) / 100;
                double totalMassCGHeight = double.Parse(twoWheelTotalMassCGHeightTextBox.Text) / 1000;
                double frontUnsprungMass = double.Parse(twoWheelFrontUnsprungMassTextBox.Text);
                double frontUnsprungMassCGHeight = double.Parse(twoWheelFrontUnsprungMassCGHeightTextBox.Text) / 1000;
                double rearUnsprungMass = double.Parse(twoWheelRearUnsprungMassTextBox.Text);
                double rearUnsprungMassCGHeight = double.Parse(twoWheelRearUnsprungMassCGHeightTextBox.Text) / 1000;
                double wheelbase = double.Parse(twoWheelWheelbaseTextBox.Text) / 1000;
                double rotPartsMI = double.Parse(twoWheelRotPartsMITextBox.Text);
                double gravity = double.Parse(twoWheelGravityAccelTextBox.Text);
                // Initializes a new object
                Vehicle.TwoWheelInertiaAndDimensions inertiaAndDimensions = new Vehicle.TwoWheelInertiaAndDimensions(inertiaID, description, totalMass, totalMassDistribution, totalMassCGHeight, frontUnsprungMass, frontUnsprungMassCGHeight, rearUnsprungMass, rearUnsprungMassCGHeight, wheelbase, rotPartsMI, gravity);
                inertiaAndDimensions.GetExtraInertiaParameters();
                // Adds the object to the listbox
                twoWheelInertiaAndDimensionsListBox.Items.Add(inertiaAndDimensions);
            }
            else System.Windows.MessageBox.Show(
                "Could not create Inertia. \n" +
                "   It should have an ID. \n" +
                "   The total mass can't be zero. \n" +
                "   The total mass distribution must be between 0% and 100%. \n" +
                "   The wheelbase can't be zero. \n" +
                "   Gravity can't be zero. \n" +
                "   Note: Negative values are corrected to positive values.",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }

        /// <summary>
        /// Deletes a two wheel model inertia and dimensions from the two wheel model inertia and dimensions listbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _DeleteTwoWheelInertiaAndDimensionsOfListBox_Click(object sender, RoutedEventArgs e)
        {
            // Checks if there's a listbox item selected and then removes it
            if (twoWheelInertiaAndDimensionsListBox.SelectedItems.Count == 1)
            {
                twoWheelInertiaAndDimensionsListBox.Items.RemoveAt(twoWheelInertiaAndDimensionsListBox.Items.IndexOf(twoWheelInertiaAndDimensionsListBox.SelectedItem));
            }
        }

        /// <summary>
        /// Loads the properties of a listbox's two wheel model inertia and dimensions and displays it in the UI fields.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _TwoWheelInertiaAndDimensionsListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // Checks if there's a listbox item selected
            if (twoWheelInertiaAndDimensionsListBox.SelectedItems.Count == 1)
            {
                // Gets the selected object
                Vehicle.TwoWheelInertiaAndDimensions inertiaAndDimensions = twoWheelInertiaAndDimensionsListBox.SelectedItem as Vehicle.TwoWheelInertiaAndDimensions;
                // Writes the properties in the UI
                twoWheelInertiaAndDimensionsIDTextBox.Text = inertiaAndDimensions.ID;
                twoWheelInertiaAndDimensionsDescriptionTextBox.Text = inertiaAndDimensions.Description;
                twoWheelTotalMassTextBox.Text = inertiaAndDimensions.TotalMass.ToString("F3");
                twoWheelTotalMassDistributionTextBox.Text = (inertiaAndDimensions.TotalMassDistribution * 100).ToString("F3");
                twoWheelTotalMassCGHeightTextBox.Text = (inertiaAndDimensions.TotalMassCGHeight * 1000).ToString("F3");
                twoWheelFrontUnsprungMassTextBox.Text = inertiaAndDimensions.FrontUnsprungMass.ToString("F3");
                twoWheelFrontUnsprungMassCGHeightTextBox.Text = (inertiaAndDimensions.FrontUnsprungMassCGHeight * 1000).ToString("F3");
                twoWheelRearUnsprungMassTextBox.Text = inertiaAndDimensions.RearUnsprungMass.ToString("F3");
                twoWheelRearUnsprungMassCGHeightTextBox.Text = (inertiaAndDimensions.RearUnsprungMassCGHeight * 1000).ToString("F3");
                twoWheelWheelbaseTextBox.Text = (inertiaAndDimensions.Wheelbase * 1000).ToString("F3");
                twoWheelRotPartsMITextBox.Text = inertiaAndDimensions.RotPartsMI.ToString("F3");
                twoWheelGravityAccelTextBox.Text = inertiaAndDimensions.Gravity.ToString("F3");
            }
        }

        #endregion

        #endregion

        #region Suspension And Steering

        #region Simplified Suspension
        /// <summary>
        /// Creates a simplified suspension object and adds it to the simplified suspension listbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _AddSimplifiedSuspensionToListBox_Click(object sender, RoutedEventArgs e)
        {
            if (simplifiedSuspensionIDTextBox.Text != "" &&
                double.Parse(simplifiedSuspensionHeaveStiffnessTextBox.Text) != 0)
            {
                // Gets the object's properties values
                string suspensionID = simplifiedSuspensionIDTextBox.Text;
                string description = simplifiedSuspensionDescriptionTextBox.Text;
                double heaveStiffness = double.Parse(simplifiedSuspensionHeaveStiffnessTextBox.Text) * 1000;
                double rideHeight = double.Parse(simplifiedSuspensionRideHeightTextBox.Text) / 1000;
                // Initializes a new object
                Vehicle.SimplifiedSuspension suspension = new Vehicle.SimplifiedSuspension(suspensionID, description, heaveStiffness, rideHeight);
                // Adds the object to the listbox
                simplifiedSuspensionListBox.Items.Add(suspension);
            }
            else System.Windows.MessageBox.Show(
                "Could not create Suspension. \n " +
                "   It should have an ID. \n" +
                "   The heave stiffness can't be zero. \n" +
                "   Note: Negative values are corrected to positive values.",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }

        /// <summary>
        /// Deletes a simplified suspension from the simplified suspension listbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _DeleteSimplifiedSuspensionOfListBox_Click(object sender, RoutedEventArgs e)
        {
            // Checks if there's a listbox item selected and then removes it
            if (simplifiedSuspensionListBox.SelectedItems.Count == 1)
            {
                simplifiedSuspensionListBox.Items.RemoveAt(simplifiedSuspensionListBox.Items.IndexOf(simplifiedSuspensionListBox.SelectedItem));
            }
        }

        /// <summary>
        /// Loads the properties of a listbox's simplified suspension and displays it in the UI fields.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _SimplifiedSuspensionListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (simplifiedSuspensionListBox.SelectedItems.Count == 1)
            {
                // Gets the selected object
                Vehicle.SimplifiedSuspension suspension = simplifiedSuspensionListBox.SelectedItem as Vehicle.SimplifiedSuspension;
                // Writes the properties in the UI
                simplifiedSuspensionIDTextBox.Text = suspension.ID;
                simplifiedSuspensionDescriptionTextBox.Text = suspension.Description;
                simplifiedSuspensionHeaveStiffnessTextBox.Text = (suspension.HeaveStiffness / 1000).ToString("F3");
                simplifiedSuspensionRideHeightTextBox.Text = (suspension.RideHeight * 1000).ToString("F3");
            }
        }
        #endregion

        #region Steering System

        /// <summary>
        /// Creates a steering system object and adds it to the steering system listbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _AddSteeringSystemToListBox_Click(object sender, RoutedEventArgs e)
        {
            if (steeringSystemIDTextBox.Text != "" &&
                double.Parse(steeringSystemMaximumSteeringWheelAngleTextBox.Text) != 0)
            {
                // Gets the object's properties values
                string id = steeringSystemIDTextBox.Text;
                string description = steeringSystemDescriptionTextBox.Text;
                double frontSteeringRatio = double.Parse(steeringSystemFrontSteeringRatioTextBox.Text);
                double rearSteeringRatio = double.Parse(steeringSystemRearSteeringRatioTextBox.Text);
                double maximumSteeringWheelAngle = double.Parse(steeringSystemMaximumSteeringWheelAngleTextBox.Text) * Math.PI / 180;
                // Initializes a new object
                Vehicle.SteeringSystem steeringSystem = new Vehicle.SteeringSystem(id, description, frontSteeringRatio, rearSteeringRatio, maximumSteeringWheelAngle);
                // Adds the object to the listbox
                steeringSystemListBox.Items.Add(steeringSystem);
            }
            else System.Windows.MessageBox.Show(
                "Could not create Steering System. \n " +
                "   It should have an ID. \n" +
                "   The maximum steering wheel angle can't be zero. \n" +
                "   Note: Negative values are corrected to positive values.",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }

        /// <summary>
        /// Deletes a steering system from the steering system listbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _DeleteSteeringSystemOfListBox_Click(object sender, RoutedEventArgs e)
        {
            // Checks if there's a listbox item selected and then removes it
            if (steeringSystemListBox.SelectedItems.Count == 1)
            {
                steeringSystemListBox.Items.RemoveAt(steeringSystemListBox.Items.IndexOf(steeringSystemListBox.SelectedItem));
            }
        }

        /// <summary>
        /// Loads the properties of a listbox's steering system and displays it in the UI fields.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _SteeringSystemListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (steeringSystemListBox.SelectedItems.Count == 1)
            {
                // Gets the selected object
                Vehicle.SteeringSystem steeringSystem = steeringSystemListBox.SelectedItem as Vehicle.SteeringSystem;
                // Writes the properties in the UI
                steeringSystemIDTextBox.Text = steeringSystem.ID;
                steeringSystemDescriptionTextBox.Text = steeringSystem.Description;
                steeringSystemFrontSteeringRatioTextBox.Text = (steeringSystem.FrontSteeringRatio).ToString("F3");
                steeringSystemRearSteeringRatioTextBox.Text = (steeringSystem.RearSteeringRatio).ToString("F3");
                steeringSystemMaximumSteeringWheelAngleTextBox.Text = (steeringSystem.MaximumSteeringWheelAngle * 180 / Math.PI).ToString("F3");
            }
        }
        #endregion

        #endregion

        #region Tires
        #region Tire

        /// <summary>
        /// Creates a tire object and adds it to the tires listbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _AddTireToListBox_Click(object sender, RoutedEventArgs e)
        {
            if (tireIDTextBox.Text != "" &&
                double.Parse(tireStiffnessTextBox.Text) != 0 &&
                tireModelComboBox.SelectedItem != null)
            {
                // Inputs assigning and convertion.
                string tireID = tireIDTextBox.Text;
                string description = tireDescriptionTextBox.Text;
                double verticalStiffness = double.Parse(tireStiffnessTextBox.Text) * 1000;
                Vehicle.TireModelMF52 tireModelMF52 = tireModelComboBox.SelectedItem as Vehicle.TireModelMF52;
                // Initializes a new object
                Vehicle.Tire tire = new Vehicle.Tire(tireID, description, tireModelMF52, verticalStiffness);
                // Adds the object to the listbox and the ComboBox
                tireListBox.Items.Add(tire);
            }
            else System.Windows.MessageBox.Show(
                "Could not create Tire. \n " +
                "   It should have an ID. \n" +
                "   A tire model must be selected. \n" +
                "   The tire stiffness can't be zero.",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }

        /// <summary>
        /// Deletes a tire from the tires listbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _DeleteTireOfListBox_Click(object sender, RoutedEventArgs e)
        {
            // Checks if there's a listbox item selected and then removes it
            if (tireListBox.SelectedItems.Count == 1)
            {
                tireListBox.Items.RemoveAt(tireListBox.Items.IndexOf(tireListBox.SelectedItem));
            }
        }

        /// <summary>
        /// Loads the properties of a listbox's tire and displays it in the UI fields.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _TireListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (tireListBox.SelectedItems.Count == 1)
            {
                // Gets the selected object
                Vehicle.Tire tire = tireListBox.SelectedItem as Vehicle.Tire;
                // Writes the properties in the UI
                tireIDTextBox.Text = tire.ID;
                tireDescriptionTextBox.Text = tire.Description;
                tireStiffnessTextBox.Text = (tire.VerticalStiffness / 1000).ToString("F3");
            }
        }

        #endregion
        #region Tire Model

        /// <summary>
        /// Creates a tire model object and adds it to the tire model listbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _AddTireModelToListBox_Click(object sender, RoutedEventArgs e)
        {
            if (tireModelIDTextBox.Text != "" &&
                File.Exists(tireModelTextBox.Text))
            {
                // Inputs assigning and convertion.
                string modelID = tireModelIDTextBox.Text;
                string description = tireModelDescriptionTextBox.Text;
                string tireModelFile = tireModelTextBox.Text;
                double alphaMin = double.Parse(tireModelMinSlipAngleTextBox.Text) * Math.PI / 180;
                double alphaMax = double.Parse(tireModelMaxSlipAngleTextBox.Text) * Math.PI / 180;
                double kappaMin = double.Parse(tireModelMinLongitudinalSlipTextBox.Text);
                double kappaMax = double.Parse(tireModelMaxLongitudinalSlipTextBox.Text);
                double lambdaFzO = double.Parse(lambdaFzOTextBox.Text);
                double lambdaMux = double.Parse(lambdaMuxTextBox.Text);
                double lambdaMuy = double.Parse(lambdaMuyTextBox.Text);
                double lambdaMuV = double.Parse(lambdaMuVTextBox.Text);
                double lambdaKxk = double.Parse(lambdaKxkTextBox.Text);
                double lambdaKya = double.Parse(lambdaKyaTextBox.Text);
                double lambdaCx = double.Parse(lambdaCxTextBox.Text);
                double lambdaCy = double.Parse(lambdaCyTextBox.Text);
                double lambdaEx = double.Parse(lambdaExTextBox.Text);
                double lambdaEy = double.Parse(lambdaEyTextBox.Text);
                double lambdaHx = double.Parse(lambdaHxTextBox.Text);
                double lambdaHy = double.Parse(lambdaHyTextBox.Text);
                double lambdaVx = double.Parse(lambdaVxTextBox.Text);
                double lambdaVy = double.Parse(lambdaVyTextBox.Text);
                double lambdaKyg = double.Parse(lambdaKygTextBox.Text);
                double lambdaKzg = double.Parse(lambdaKzgTextBox.Text);
                double lambdat = double.Parse(lambdatTextBox.Text);
                double lambdaMr = double.Parse(lambdaMrTextBox.Text);
                double lambdaxa = double.Parse(lambdaxaTextBox.Text);
                double lambdayk = double.Parse(lambdaykTextBox.Text);
                double lambdaVyk = double.Parse(lambdaVykTextBox.Text);
                double lambdas = double.Parse(lambdasTextBox.Text);
                double lambdaCz = double.Parse(lambdaCzTextBox.Text);
                double lambdaMx = double.Parse(lambdaMxTextBox.Text);
                double lambdaVMx = double.Parse(lambdaVMxTextBox.Text);
                double lambdaMy = double.Parse(lambdaMyTextBox.Text);
                // Initializes the scalling factors list
                List<double> lambdaList = new List<double>
                {
                    // Assigns the scalling factors to the list
                    lambdaFzO,  lambdaMux,  lambdaMuy,
                    lambdaMuV,  lambdaKxk,  lambdaKya,
                    lambdaCx,   lambdaCy,   lambdaEx,
                    lambdaEy,   lambdaHx,   lambdaHy,
                    lambdaVx,   lambdaVy,   lambdaKyg,
                    lambdaKzg,  lambdat,    lambdaMr,
                    lambdaxa,   lambdayk,   lambdaVyk,
                    lambdas,    lambdaCz,   lambdaMx,
                    lambdaVMx,  lambdaMy
                };
                // Creates the Tire Model object
                Vehicle.TireModelMF52 tireModelMF52 = new Vehicle.TireModelMF52(modelID, description, tireModelFile, lambdaList, alphaMin, alphaMax, kappaMin, kappaMax);
                // Adds the object to the listbox and the ComboBox
                tireModelListBox.Items.Add(tireModelMF52);
                tireModelDisplayCheckListBox.Items.Add(tireModelMF52);
            }
            else System.Windows.MessageBox.Show(
                "Could not create Tire Model. \n " +
                "   It should have an ID. \n" +
                "   Check if the coefficients file exists. \n" +
                "   Note: Negative values are corrected to positive values.",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }

        /// <summary>
        /// Deletes a tire model from the tire models listbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _DeleteTireModelOfListBox_Click(object sender, RoutedEventArgs e)
        {
            // Checks if there's a listbox item selected and then removes it
            if (tireModelListBox.SelectedItems.Count == 1)
            {
                tireModelListBox.Items.RemoveAt(tireModelListBox.Items.IndexOf(tireModelListBox.SelectedItem));
                tireModelDisplayCheckListBox.Items.RemoveAt(tireModelDisplayCheckListBox.Items.IndexOf(tireModelDisplayCheckListBox.SelectedItem));
            }
        }

        /// <summary>
        /// Loads the properties of the tire models listbox's tire model and displays it in the UI fields.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _TireModelListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (tireModelListBox.SelectedItems.Count == 1)
            {
                // Gets the selected object
                Vehicle.TireModelMF52 tireModel = tireModelListBox.SelectedItem as Vehicle.TireModelMF52;
                // Writes the properties in the UI
                tireModelIDTextBox.Text = tireModel.ID;
                tireModelDescriptionTextBox.Text = tireModel.Description;
                tireModelTextBox.Text = tireModel.FileLocation;
                tireModelMinSlipAngleTextBox.Text = (tireModel.AlphaMin * Math.PI / 180).ToString("F3");
                tireModelMaxSlipAngleTextBox.Text = (tireModel.AlphaMax * Math.PI / 180).ToString("F3");
                tireModelMinLongitudinalSlipTextBox.Text = tireModel.KappaMin.ToString("F3");
                tireModelMinLongitudinalSlipTextBox.Text = tireModel.KappaMax.ToString("F3");
                lambdaFzOTextBox.Text = tireModel.lambdaFzO.ToString("F3");
                lambdaMuxTextBox.Text = tireModel.lambdaMux.ToString("F3");
                lambdaMuyTextBox.Text = tireModel.lambdaMuy.ToString("F3");
                lambdaMuVTextBox.Text = tireModel.lambdaMuV.ToString("F3");
                lambdaKxkTextBox.Text = tireModel.lambdaKxk.ToString("F3");
                lambdaKyaTextBox.Text = tireModel.lambdaKya.ToString("F3");
                lambdaCxTextBox.Text = tireModel.lambdaCx.ToString("F3");
                lambdaCyTextBox.Text = tireModel.lambdaCy.ToString("F3");
                lambdaExTextBox.Text = tireModel.lambdaEx.ToString("F3");
                lambdaEyTextBox.Text = tireModel.lambdaEy.ToString("F3");
                lambdaHxTextBox.Text = tireModel.lambdaHx.ToString("F3");
                lambdaHyTextBox.Text = tireModel.lambdaHy.ToString("F3");
                lambdaVxTextBox.Text = tireModel.lambdaVx.ToString("F3");
                lambdaVyTextBox.Text = tireModel.lambdaVy.ToString("F3");
                lambdaKygTextBox.Text = tireModel.lambdaKyg.ToString("F3");
                lambdaKzgTextBox.Text = tireModel.lambdaKzg.ToString("F3");
                lambdatTextBox.Text = tireModel.lambdat.ToString("F3");
                lambdaMrTextBox.Text = tireModel.lambdaMr.ToString("F3");
                lambdaxaTextBox.Text = tireModel.lambdaxa.ToString("F3");
                lambdaykTextBox.Text = tireModel.lambdayk.ToString("F3");
                lambdaVykTextBox.Text = tireModel.lambdaVyk.ToString("F3");
                lambdasTextBox.Text = tireModel.lambdas.ToString("F3");
                lambdaCzTextBox.Text = tireModel.lambdaCz.ToString("F3");
                lambdaMxTextBox.Text = tireModel.lambdaMx.ToString("F3");
                lambdaVMxTextBox.Text = tireModel.lambdaVMx.ToString("F3");
                lambdaMyTextBox.Text = tireModel.lambdaMy.ToString("F3");
            }
        }

        /// <summary>
        /// Opens a dialog box to select the tire model coefficients file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _TireModelButton_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog
            Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog
            {
                // Set filter for file extension and default file extension  
                DefaultExt = ".txt",
                Filter = "Text documents (.txt)|*.txt"
            };
            // Launch OpenFileDialog by calling ShowDialog method
            Nullable<bool> result = openFileDlg.ShowDialog();
            // Get the selected file name and display in a TextBox.
            // Load content of file in a TextBlock
            if (result == true)
            {
                tireModelTextBox.Text = openFileDlg.FileName;
            }
        }

        #region Tire Model Display

        /// <summary>
        /// Creates a tire model point object and adds it to the tire model points checklistbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _AddTireModelParameterSetToCheckListBox_Click(object sender, RoutedEventArgs e)
        {
            // Gets the parameters from the UI
            double longitudinalSlip = double.Parse(tireModelDisplayParametersLongitudinalSlipTextBox.Text);
            double slipAngle = double.Parse(tireModelDisplayParametersSlipAngleTextBox.Text);
            double verticalLoad = double.Parse(tireModelDisplayParametersVerticalLoadTextBox.Text);
            double inclinationAngle = double.Parse(tireModelDisplayParametersInclinationAngleTextBox.Text);
            double speed = double.Parse(tireModelDisplayParametersSpeedTextBox.Text);
            // Initializes the new tire model point object
            Vehicle.TireModelMF52Point tireModelPoint = new Vehicle.TireModelMF52Point(longitudinalSlip, slipAngle, verticalLoad, inclinationAngle, speed);
            // Adds the object to the checklistbox
            tireModelDisplayParameterSetsCheckListBox.Items.Add(tireModelPoint);
        }

        /// <summary>
        /// Deletes a tire model point from the tire model points checklistbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _DeleteTireModelParameterSetOfCheckListBox_Click(object sender, RoutedEventArgs e)
        {
            // Checks if there's a listbox item selected and then removes it
            if (tireModelDisplayParameterSetsCheckListBox.SelectedItems.Count == 1)
            {
                tireModelDisplayParameterSetsCheckListBox.Items.RemoveAt(tireModelDisplayParameterSetsCheckListBox.Items.IndexOf(tireModelDisplayParameterSetsCheckListBox.SelectedItem));
            }
            else System.Windows.MessageBox.Show(
                "Could not delete parameters set. Please, select only one parameters set. ",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }

        /// <summary>
        /// Adds a new chart tab to the Tire Model analysis environment.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _TireModelDisplayChartTabControl_NewButtonClick(object sender, EventArgs e)
        {
            TabItemExt item = new TabItemExt
            {

                Header = "Chart" + (tireModelDisplayChartTabControl.Items.Count + 1),

            };
            tireModelDisplayChartTabControl.Items.Add(item);
        }

        /// <summary>
        /// Adds a new chart to the Tire Model analysis environment when the last chart is closed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _TireModelDisplayChartTabControl_OnCloseButtonClick(object sender, CloseTabEventArgs e)
        {
            if (tireModelDisplayChartTabControl.Items.Count == 1)
            {
                TabItemExt item = new TabItemExt
                {

                    Header = "Chart" + (tireModelDisplayChartTabControl.Items.Count),

                };
                tireModelDisplayChartTabControl.Items.Add(item);
            }
        }

        private void _TireModelDisplayCheckListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            _UpdateCurrentTireModelDisplayChart();
        }

        private void _TireModelParameterSetsCheckListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            _UpdateCurrentTireModelDisplayChart();
        }

        private void _TireModelDisplayChartYAxisDataComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            _UpdateCurrentTireModelDisplayChart();
        }

        private void _TireModelDisplayChartXAxisDataComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            _UpdateCurrentTireModelDisplayChart();
        }

        private void _TireModelDisplayXAxisRangeMinTextBox_ValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            _UpdateCurrentTireModelDisplayChart();
        }

        private void _TireModelDisplayXAxisRangeMaxTextBox_ValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            _UpdateCurrentTireModelDisplayChart();
        }

        private void _TireModelDisplayYAxisAmountOfPointsTextBox_ValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            _UpdateCurrentTireModelDisplayChart();
        }

        /// <summary>
        /// Updates the current tab's tire model display chart.
        /// </summary>
        private void _UpdateCurrentTireModelDisplayChart()
        {
            // Checks if all of the information needed to generate the chart is ok.
            if (tireModelDisplayCheckListBox.SelectedItems.Count == 0 || tireModelDisplayParameterSetsCheckListBox.SelectedItems.Count == 0 || tireModelDisplayChartYAxisDataComboBox.SelectedValue == null || tireModelDisplayChartXAxisDataComboBox.SelectedValue == null || !double.TryParse(tireModelDisplayXAxisRangeMinTextBox.Text, out double rangeMin) || !double.TryParse(tireModelDisplayXAxisRangeMaxTextBox.Text, out double rangeMax) || rangeMin >= rangeMax || !int.TryParse(tireModelDisplayDataAmountOfPointsTextBox.Text, out int amountOfPoints) || amountOfPoints <= 0)
            {
                TabItemExt currentTab = tireModelDisplayChartTabControl.SelectedItem as TabItemExt;
                if (currentTab != null) currentTab.Content = new Grid();
                return;
            }
            // Initializes the new chart
            SfChart chart = new SfChart
            {
                Header = "Tire Model Analysis",
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(10),
                Legend = new ChartLegend()
                {
                    ItemsPanel = mainWorkGrid.Resources["chartsLegendsPanelTemplate"] as ItemsPanelTemplate,
                    DockPosition = ChartDock.Right,
                    MaxWidth = 300
                },
                PrimaryAxis = new NumericalAxis(),
                SecondaryAxis = new NumericalAxis()
            };
            // Adds zoom/panning behaviour to the chart
            /*ChartZoomPanBehavior zoomingAndPanning = new ChartZoomPanBehavior()
            {
                EnableZoomingToolBar = true,
                EnableMouseWheelZooming = true,
                EnablePanning = true,
                ZoomRelativeToCursor = true,
            };
            chart.Behaviors.Add(zoomingAndPanning);*/
            // Adds a trackball to the chart
            ChartTrackBallBehavior trackBall = new ChartTrackBallBehavior();
            chart.Behaviors.Add(trackBall);
            // X axis data generation
            double[] xAxisData = Generate.LinearSpaced(int.Parse(tireModelDisplayDataAmountOfPointsTextBox.Text), double.Parse(tireModelDisplayXAxisRangeMinTextBox.Text), double.Parse(tireModelDisplayXAxisRangeMaxTextBox.Text));
            // Chart's data generation (current tire model -> current tire model parameter set -> current curve point).
            foreach (Vehicle.TireModelMF52 tireModel in tireModelDisplayCheckListBox.SelectedItems)
            {
                foreach (Vehicle.TireModelMF52Point tireModelMF52ParametersSet in tireModelDisplayParameterSetsCheckListBox.SelectedItems)
                {
                    // Current data set list
                    Vehicle.TireModelMF52ViewModel tireModelViewModel = new Vehicle.TireModelMF52ViewModel();
                    FastLineSeries fastLineSeries = new FastLineSeries();
                    for (int iPoint = 0; iPoint < int.Parse(tireModelDisplayDataAmountOfPointsTextBox.Text); iPoint++)
                    {
                        // Current point's parameters set
                        Vehicle.TireModelMF52Point currentPoint = new Vehicle.TireModelMF52Point(tireModelMF52ParametersSet.LongitudinalSlip, tireModelMF52ParametersSet.SlipAngle, tireModelMF52ParametersSet.VerticalLoad, tireModelMF52ParametersSet.InclinationAngle, tireModelMF52ParametersSet.Speed);
                        switch (tireModelDisplayChartXAxisDataComboBox.SelectedValue.ToString())
                        {
                            case "Longitudinal Slip":
                                currentPoint.LongitudinalSlip = xAxisData[iPoint];
                                chart.PrimaryAxis.Header = "Longitudinal Slip";
                                chart.PrimaryAxis.LabelFormat = "N2";
                                fastLineSeries.XBindingPath = "LongitudinalSlip";
                                break;
                            case "Slip Angle":
                                currentPoint.SlipAngle = xAxisData[iPoint];
                                chart.PrimaryAxis.Header = "Slip Angle [deg]";
                                chart.PrimaryAxis.LabelFormat = "N2";
                                fastLineSeries.XBindingPath = "SlipAngle";
                                break;
                            case "Vertical Load":
                                currentPoint.VerticalLoad = xAxisData[iPoint];
                                chart.PrimaryAxis.Header = "Vertical Load [N]";
                                chart.PrimaryAxis.LabelFormat = "N0";
                                fastLineSeries.XBindingPath = "VerticalLoad";
                                break;
                            case "Inclination Angle":
                                currentPoint.InclinationAngle = xAxisData[iPoint];
                                break;
                            case "Speed":
                                currentPoint.Speed = xAxisData[iPoint];
                                chart.PrimaryAxis.Header = "Speed [km/h]";
                                chart.PrimaryAxis.LabelFormat = "N1";
                                fastLineSeries.XBindingPath = "Speed";
                                break;
                            default:
                                break;
                        }
                        // Gets the y axis data
                        switch (tireModelDisplayChartYAxisDataComboBox.SelectedValue.ToString())
                        {
                            case "Longitudinal Force":
                                currentPoint.GetLongitudinalForce(tireModel);
                                chart.SecondaryAxis.Header = "Longitudinal Force [N]";
                                chart.SecondaryAxis.LabelFormat = "N0";
                                fastLineSeries.YBindingPath = "LongitudinalForce";
                                break;
                            case "Lateral Force":
                                currentPoint.GetLateralForce(tireModel);
                                chart.SecondaryAxis.Header = "Lateral Force [N]";
                                chart.SecondaryAxis.LabelFormat = "N0";
                                fastLineSeries.YBindingPath = "LateralForce";
                                break;
                            case "Overturning Moment":
                                currentPoint.GetOverturningMoment(tireModel);
                                chart.SecondaryAxis.Header = "Overturning Moment [Nm]";
                                chart.SecondaryAxis.LabelFormat = "N1";
                                fastLineSeries.YBindingPath = "OverturningMoment";
                                break;
                            case "Rolling Moment":
                                currentPoint.GetRollingMoment(tireModel);
                                chart.SecondaryAxis.Header = "Rolling Moment [Nm]";
                                chart.SecondaryAxis.LabelFormat = "N1";
                                fastLineSeries.YBindingPath = "RollingMoment";
                                break;
                            case "Self-Aligning Torque":
                                currentPoint.GetSelfAligningTorque(tireModel);
                                chart.SecondaryAxis.Header = "Self-Aligning Torque [Nm]";
                                chart.SecondaryAxis.LabelFormat = "N1";
                                fastLineSeries.YBindingPath = "SelfAligningTorque";
                                break;
                            default:
                                break;
                        }
                        // Adds the current point to the current list
                        tireModelViewModel.TireModelMF52Points.Add(currentPoint);
                    }
                    string seriesLabel = "TM: " + tireModel.ID + "\n - Long. Slip: " + tireModelMF52ParametersSet.LongitudinalSlip.ToString("F2") + "\n - Slip Angle: " + tireModelMF52ParametersSet.SlipAngle.ToString("F2") + "\n - Vert. Load: " + tireModelMF52ParametersSet.VerticalLoad.ToString("F0") + "\n - Incl. Angle: " + tireModelMF52ParametersSet.InclinationAngle.ToString("F2") + "\n - Speed:" + tireModelMF52ParametersSet.Speed.ToString("F1");
                    fastLineSeries.ItemsSource = tireModelViewModel.TireModelMF52Points;
                    fastLineSeries.Label = seriesLabel;
                    // Adds the current series to the chart.
                    chart.Series.Add(fastLineSeries);
                }
            }
            // Adds the chart to the current chart tab
            Grid grid = new Grid();
            grid.Children.Add(chart);
            TabItemExt currentChartTab = tireModelDisplayChartTabControl.SelectedItem as TabItemExt;
            currentChartTab.Content = grid;
        }

        /// <summary>
        /// Adds a new chart to the Tire model display chart analysis environment when the last chart is closed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _LapTimeSimulationResultsAnalysisChartTabControl_OnCloseButtonClick(object sender, CloseTabEventArgs e)
        {
            if (tireModelDisplayChartTabControl.Items.Count == 1)
            {
                TabItemExt item = new TabItemExt
                {

                    Header = "Chart" + (tireModelDisplayChartTabControl.Items.Count),

                };
                tireModelDisplayChartTabControl.Items.Add(item);
            }
        }

        #endregion
        #endregion
        #endregion

        #region Transmission
        #region One Wheel Transmission

        /// <summary>
        /// Creates a one wheel model's transmission object and adds it to the one wheel model's transmissions listbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _AddOneWheelTransmissionToListBox_Click(object sender, RoutedEventArgs e)
        {
            if (oneWheelTransmissionIDTextBox.Text != "" &&
                oneWheelTransmissionGearRatiosSetComboBox.SelectedItem != null)
            {
                // Gets the object's data
                string transmissionID = oneWheelTransmissionIDTextBox.Text;
                string description = oneWheelTransmissionDescriptionTextBox.Text;
                string type = oneWheelTransmissionTypeComboBox.Text;
                Vehicle.GearRatiosSet gearRatiosSet = oneWheelTransmissionGearRatiosSetComboBox.SelectedItem as Vehicle.GearRatiosSet;
                double primaryRatio = double.Parse(oneWheelPrimaryRatioTextBox.Text);
                double finalRatio = double.Parse(oneWheelFinalRatioTextBox.Text);
                double gearShiftTime = double.Parse(oneWheelGearShiftTimeTextBox.Text);
                double efficiency = double.Parse(oneWheelTransmissionEfficiencyTextBox.Text) / 100;
                // Initializes a new object
                Vehicle.OneWheelTransmission transmission = new Vehicle.OneWheelTransmission(
                    transmissionID, description, type, primaryRatio, finalRatio, gearShiftTime, efficiency, gearRatiosSet);
                // Adds the object to the listbox and the ComboBox
                oneWheelTransmissionListBox.Items.Add(transmission);
            }
            else System.Windows.MessageBox.Show(
                "Could not create Transmission. \n " +
                "   It should have an ID. \n" +
                "   A gear ratios set must be selected.",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }

        /// <summary>
        /// Deletes a one wheel model's transmission from the one wheel model's transmissions listbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _DeleteOneWheelTransmissionOfListBox_Click(object sender, RoutedEventArgs e)
        {
            // Checks if there's a listbox item selected and then removes it
            if (oneWheelTransmissionListBox.SelectedItems.Count == 1)
            {
                oneWheelTransmissionListBox.Items.RemoveAt(oneWheelTransmissionListBox.Items.IndexOf(oneWheelTransmissionListBox.SelectedItem));
            }
        }

        /// <summary>
        /// Loads the properties of the one wheel model's transmissions listbox's transmission and displays it in the UI fields.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _OneWheelTransmissionListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // Checks if there's a listbox item selected
            if (oneWheelTransmissionListBox.SelectedItems.Count == 1)
            {
                // Gets the selected object
                Vehicle.OneWheelTransmission transmission = oneWheelTransmissionListBox.SelectedItem as Vehicle.OneWheelTransmission;
                // Writes the properties in the UI
                oneWheelTransmissionIDTextBox.Text = transmission.ID;
                oneWheelTransmissionDescriptionTextBox.Text = transmission.Description;
                oneWheelTransmissionTypeComboBox.Text = transmission.Type;
                oneWheelTransmissionGearRatiosSetComboBox.Text = transmission.GearRatiosSet.ToString();
                oneWheelPrimaryRatioTextBox.Text = transmission.PrimaryRatio.ToString("F3");
                oneWheelFinalRatioTextBox.Text = transmission.FinalRatio.ToString("F3");
                oneWheelGearShiftTimeTextBox.Text = transmission.GearShiftTime.ToString("F3");
                oneWheelTransmissionEfficiencyTextBox.Text = (transmission.Efficiency * 100).ToString("F3");
            }
        }

        #endregion

        #region Two Wheel Transmission

        /// <summary>
        /// Creates a two wheel model's transmission object and adds it to the two wheel model's transmissions listbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _TwoWheelAddTransmissionToListBox_Click(object sender, RoutedEventArgs e)
        {
            if (twoWheelTransmissionIDTextBox.Text != "" &&
                double.Parse(twoWheelTransmissionTorqueBiasTextBox.Text) >= 0 &&
                double.Parse(twoWheelTransmissionTorqueBiasTextBox.Text) <= 100 &&
                twoWheelTransmissionGearRatiosSetComboBox.SelectedItem != null)
            {
                // Gets the object's data
                string transmissionID = twoWheelTransmissionIDTextBox.Text;
                string description = twoWheelTransmissionDescriptionTextBox.Text;
                Vehicle.GearRatiosSet gearRatiosSet = twoWheelTransmissionGearRatiosSetComboBox.SelectedItem as Vehicle.GearRatiosSet;
                double primaryRatio = double.Parse(twoWheelPrimaryRatioTextBox.Text);
                double finalRatio = double.Parse(twoWheelFinalRatioTextBox.Text);
                double gearShiftTime = double.Parse(twoWheelGearShiftTimeTextBox.Text);
                double efficiency = double.Parse(twoWheelTransmissionEfficiencyTextBox.Text) / 100;
                double torqueBias = double.Parse(twoWheelTransmissionTorqueBiasTextBox.Text) / 100;
                // Initializes a new object
                Vehicle.TwoWheelTransmission transmission = new Vehicle.TwoWheelTransmission(
                    transmissionID, description, torqueBias, primaryRatio, finalRatio, gearShiftTime, efficiency, gearRatiosSet);
                // Adds the object to the listbox and the ComboBox
                twoWheelTransmissionListBox.Items.Add(transmission);
            }
            else System.Windows.MessageBox.Show(
                "Could not create Transmission. \n " +
                "   It should have an ID. \n" +
                "   A gear ratios set must be selected. \n" +
                "   The torque bias must be between 0 and 100.",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }

        /// <summary>
        /// Deletes a two wheel model's transmission from the two wheel model's transmissions listbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _TwoWheelDeleteTransmissionOfListBox_Click(object sender, RoutedEventArgs e)
        {
            // Checks if there's a listbox item selected and then removes it
            if (twoWheelTransmissionListBox.SelectedItems.Count == 1)
            {
                twoWheelTransmissionListBox.Items.RemoveAt(twoWheelTransmissionListBox.Items.IndexOf(twoWheelTransmissionListBox.SelectedItem));
            }
        }

        /// <summary>
        /// Loads the properties of the two wheel model's transmissions listbox's transmission and displays it in the UI fields.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _TwoWheelTransmissionListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // Checks if there's a listbox item selected
            if (twoWheelTransmissionListBox.SelectedItems.Count == 1)
            {
                // Gets the selected object
                Vehicle.TwoWheelTransmission transmission = twoWheelTransmissionListBox.SelectedItem as Vehicle.TwoWheelTransmission;
                // Writes the properties in the UI
                twoWheelTransmissionIDTextBox.Text = transmission.ID;
                twoWheelTransmissionDescriptionTextBox.Text = transmission.Description;
                twoWheelTransmissionGearRatiosSetComboBox.Text = transmission.GearRatiosSet.ToString();
                twoWheelPrimaryRatioTextBox.Text = transmission.PrimaryRatio.ToString("F3");
                twoWheelFinalRatioTextBox.Text = transmission.FinalRatio.ToString("F3");
                twoWheelGearShiftTimeTextBox.Text = transmission.GearShiftTime.ToString("F3");
                twoWheelTransmissionEfficiencyTextBox.Text = (transmission.Efficiency * 100).ToString("F3");
                twoWheelTransmissionTorqueBiasTextBox.Text = (transmission.TorqueBias * 100).ToString("F3");
            }
        }

        #endregion

        #region Gear Ratios

        /// <summary>
        /// Creates a gear ratios set object and adds it to the one wheel model gear ratios sets listbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _AddGearRatioSetToListBox_Click(object sender, RoutedEventArgs e)
        {
            if (gearRatiosSetIDTextBox.Text != "" &&
                gearRatiosListBox.Items.Count != 0)
            {
                // Gets the object's properties values
                string setID = gearRatiosSetIDTextBox.Text;
                string description = gearRatiosSetDescriptionTextBox.Text;
                List<Vehicle.GearRatio> gearRatios = new List<Vehicle.GearRatio>();
                foreach (var gearRatioItem in gearRatiosListBox.Items)
                {
                    Vehicle.GearRatio gearRatio = gearRatioItem as Vehicle.GearRatio;
                    gearRatios.Add(gearRatio);
                }
                // Initializes a new object
                Vehicle.GearRatiosSet gearRatiosSet = new Vehicle.GearRatiosSet(setID, description, gearRatios);
                // Adds the object to the listbox and the ComboBox
                gearRatiosSetsListBox.Items.Add(gearRatiosSet);
            }
            else System.Windows.MessageBox.Show(
                "Could not create Gear Ratios Set. \n " +
                "   It should have an ID. \n" +
                "   The gear ratios list can't be empty.",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }

        /// <summary>
        /// Deletes a gear ratios set from the one wheel model gear ratios sets listbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _DeleteGearRatioSetOfListBox_Click(object sender, RoutedEventArgs e)
        {
            // Checks if there's a listbox item selected and then removes it
            if (gearRatiosSetsListBox.SelectedItems.Count == 1)
            {
                gearRatiosSetsListBox.Items.RemoveAt(gearRatiosSetsListBox.Items.IndexOf(gearRatiosSetsListBox.SelectedItem));
            }
        }

        /// <summary>
        /// Loads the properties of the one wheel model gear ratio sets listbox's gear ratio set and displays it in the UI fields.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _GearRatioSetListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // Checks if there's a listbox item selected
            if (gearRatiosSetsListBox.SelectedItems.Count == 1)
            {
                // Gets the selected object
                Vehicle.GearRatiosSet gearRatioSet = gearRatiosSetsListBox.SelectedItem as Vehicle.GearRatiosSet;
                // Writes the properties in the UI
                gearRatiosSetIDTextBox.Text = gearRatioSet.ID;
                gearRatiosSetDescriptionTextBox.Text = gearRatioSet.Description;
                // Clears and writes the list in the UI
                gearRatiosListBox.Items.Clear();
                foreach (Vehicle.GearRatio gearRatio in gearRatioSet.GearRatios)
                {
                    gearRatiosListBox.Items.Add(gearRatio);
                }
            }
        }

        /// <summary>
        /// Creates a gear ratio object and adds it to the one wheel model gear ratios listbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _AddGearRatioToListBox_Click(object sender, RoutedEventArgs e)
        {
            if (double.Parse(newGearRatioTextBox.Text) != 0)
            {
                // Gets the object's properties values
                double ratio = double.Parse(newGearRatioTextBox.Text);
                if (ratio < 0) ratio = -ratio;
                // Initializes a new object
                Vehicle.GearRatio gearRatio = new Vehicle.GearRatio(ratio);
                // Adds the object to the listbox and the ComboBox
                gearRatiosListBox.Items.Add(gearRatio);
                // Reorders the gear ratios listbox items in descending order
                List<Vehicle.GearRatio> gearRatios = new List<Vehicle.GearRatio>();
                foreach (var gearRatioItem in gearRatiosListBox.Items)
                {
                    Vehicle.GearRatio currentGearRatio = gearRatioItem as Vehicle.GearRatio;
                    gearRatios.Add(currentGearRatio);
                }
                gearRatios = gearRatios.OrderByDescending(currentGearRatio => currentGearRatio.Ratio).ToList();
                gearRatiosListBox.Items.Clear();
                foreach (Vehicle.GearRatio currentGearRatio in gearRatios)
                {
                    gearRatiosListBox.Items.Add(currentGearRatio);
                }
            }
            else System.Windows.MessageBox.Show(
                "Could not create Gear Ratio. \n " +
                "   The ratio can't be zero. \n" +
                "   Note: Negative values are corrected to positive values.",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }

        /// <summary>
        /// Deletes a gear ratio from the one wheel model gear ratios listbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _DeleteGearRatioOfListBox_Click(object sender, RoutedEventArgs e)
        {
            // Checks if there's a listbox item selected and then removes it
            if (gearRatiosListBox.SelectedItems.Count == 1)
            {
                gearRatiosListBox.Items.RemoveAt(gearRatiosListBox.Items.IndexOf(gearRatiosListBox.SelectedItem));
            }
        }

        #endregion
        #endregion

        #region One Wheel Car Input Methods

        #region Single Car And Setup

        /// <summary>
        /// Creates a one wheel model car/setup object and adds it to the one wheel model car/setup listbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _OneWheelAddCarAndSetupToListBox_Click(object sender, RoutedEventArgs e)
        {
            if (oneWheelCarIDTextBox.Text != "" &&
                oneWheelSetupIDTextBox.Text != "" &&
                oneWheelAerodynamicsComboBox.Text != "" &&
                oneWheelBrakesComboBox.Text != "" &&
                oneWheelEngineComboBox.Text != "" &&
                oneWheelInertiaComboBox.Text != "" &&
                oneWheelSuspensionComboBox.Text != "" &&
                oneWheelTireComboBox.Text != "" &&
                oneWheelTransmissionComboBox.Text != "")
            {
                // Gets the object's properties values
                string carID = oneWheelCarIDTextBox.Text;
                string setupID = oneWheelSetupIDTextBox.Text;
                string description = oneWheelCarAndSetupDescriptionTextBox.Text;
                Vehicle.OneWheelAerodynamics aerodynamics = oneWheelAerodynamicsComboBox.SelectedItem as Vehicle.OneWheelAerodynamics;
                Vehicle.OneWheelBrakes brakes = oneWheelBrakesComboBox.SelectedItem as Vehicle.OneWheelBrakes;
                Vehicle.Engine engine = oneWheelEngineComboBox.SelectedItem as Vehicle.Engine;
                Vehicle.OneWheelInertia inertia = oneWheelInertiaComboBox.SelectedItem as Vehicle.OneWheelInertia;
                Vehicle.SimplifiedSuspension suspension = oneWheelSuspensionComboBox.SelectedItem as Vehicle.SimplifiedSuspension;
                Vehicle.Tire tire = oneWheelTireComboBox.SelectedItem as Vehicle.Tire;
                Vehicle.OneWheelTransmission transmission = oneWheelTransmissionComboBox.SelectedItem as Vehicle.OneWheelTransmission;
                // Initializes a new object
                Vehicle.OneWheelCar car = new Vehicle.OneWheelCar(carID, setupID, description, aerodynamics, brakes, engine, inertia, suspension, tire, transmission);
                // Gets additional parameters
                car.GetLinearAccelerationParameters();
                car.GetCarOperationSpeedRange();
                // Adds the object to the listbox
                oneWheelCarAndSetupListBox.Items.Add(car);
            }
            else System.Windows.MessageBox.Show(
                "Could not create Car/Setup. \n" +
                "   It should have a car ID. \n" +
                "   It should have a setup ID. \n" +
                "   All of the components boxes must be filled.",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }

        /// <summary>
        /// Deletes a one wheel model car/setup from the one wheel model car/setup listbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _OneWheelDeleteCarAndSetupOfListBox_Click(object sender, RoutedEventArgs e)
        {
            // Checks if there's a listbox item selected and then removes it
            if (oneWheelCarAndSetupListBox.SelectedItems.Count == 1)
            {
                oneWheelCarAndSetupListBox.Items.RemoveAt(oneWheelCarAndSetupListBox.Items.IndexOf(oneWheelCarAndSetupListBox.SelectedItem));
            }
        }

        /// <summary>
        /// Loads the properties of a listbox's one wheel model car/setup and displays it in the UI fields.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _OneWheelCarAndSetupListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // Checks if there's a listbox item selected
            if (oneWheelCarAndSetupListBox.SelectedItems.Count == 1)
            {
                // Gets the selected object
                Vehicle.OneWheelCar car = oneWheelCarAndSetupListBox.SelectedItem as Vehicle.OneWheelCar;
                // Writes the properties in the UI
                oneWheelCarIDTextBox.Text = car.ID;
                oneWheelSetupIDTextBox.Text = car.SetupID;
                oneWheelCarAndSetupDescriptionTextBox.Text = car.Description;
                oneWheelAerodynamicsComboBox.Text = car.Aerodynamics.ID;
                oneWheelBrakesComboBox.Text = car.Brakes.ID;
                oneWheelEngineComboBox.Text = car.Engine.ID;
                oneWheelInertiaComboBox.Text = car.Inertia.ID;
                oneWheelSuspensionComboBox.Text = car.Suspension.ID;
                oneWheelTireComboBox.Text = car.Tire.ID;
                oneWheelTransmissionComboBox.Text = car.Transmission.ID;
            }
        }

        #region Powertrain Diagram
        /// <summary>
        /// Updates the powertrain diagram when the transmission combobox item changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _OneWheelTransmissionComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (oneWheelTransmissionComboBox.SelectedValue != null && oneWheelEngineComboBox.SelectedValue != null) _GenerateOneWheelPowerTrainDiagram();
            else _ClearOneWheelPowertrainDiagram();
        }
        /// <summary>
        /// Updates the powertrain diagram when the engine combobox item changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _OneWheelEngineComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (oneWheelTransmissionComboBox.SelectedValue != null && oneWheelEngineComboBox.SelectedValue != null) _GenerateOneWheelPowerTrainDiagram();
            else _ClearOneWheelPowertrainDiagram();
        }

        private void _GenerateOneWheelPowerTrainDiagram()
        {
            // Gets the engine and transmission objects
            Vehicle.Engine engine = oneWheelEngineComboBox.SelectedItem as Vehicle.Engine;
            Vehicle.Transmission transmission = oneWheelTransmissionComboBox.SelectedItem as Vehicle.Transmission;
            // Gets the view model from the engine and the transmission
            Vehicle.PowertrainViewModel powertrainViewModel = new Vehicle.PowertrainViewModel();
            powertrainViewModel.GetPowertrainCurvePoints(engine, transmission);
            // Initializes a new chart
            SfChart chart = new SfChart()
            {
                Header = "Powertrain Diagram - Engine: " + engine.ID + " - Transmission: " + transmission.ID,
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(10),
                Width = oneWheelCarAndSetupDockingWindow.ActualHeight,
                Legend = new ChartLegend()
            };
            // Adds the axis to the chart
            chart.PrimaryAxis = new NumericalAxis()
            {
                Header = "Wheel Angular Speed (rpm)",
                LabelFormat = "N0"
            };
            chart.SecondaryAxis = new NumericalAxis()
            {
                Header = "Wheel Torque (Nm)",
                LabelFormat = "N0"
            };
            // Adds zoom/panning behaviour to the chart
            /*ChartZoomPanBehavior zoomingAndPanning = new ChartZoomPanBehavior()
            {
                EnableZoomingToolBar = true,
                EnableMouseWheelZooming = true,
                EnablePanning = true,
                ZoomRelativeToCursor = true,
            };
            chart.Behaviors.Add(zoomingAndPanning);*/
            // Adds the curves data of each gear
            for (int iGear = 0; iGear < transmission.GearRatiosSet.GearRatios.Count; iGear++)
            {
                FastLineSeries fastLineSeries = new FastLineSeries()
                {
                    Label = "Gear: " + (iGear + 1).ToString(),
                    ItemsSource = powertrainViewModel.PowertrainDiagramCurvePoints[iGear],
                    XBindingPath = "WheelAngularSpeed",
                    YBindingPath = "Torque",
                    StrokeThickness = 5
                };
                chart.Series.Add(fastLineSeries);
            }
            // Clears the preview grid and displays the new chart
            oneWheelModelPowertrainDiagramDisplayGrid.Children.Clear();
            oneWheelModelPowertrainDiagramDisplayGrid.Children.Add(chart);
            DockingManager.SetDesiredWidthInDockedMode(oneWheelCarAndSetupDockingWindow, 250 + oneWheelCarAndSetupDockingWindow.ActualHeight);
        }
        /// <summary>
        /// Clears the one wheel powertrain diagram.
        /// </summary>
        private void _ClearOneWheelPowertrainDiagram()
        {
            oneWheelModelPowertrainDiagramDisplayGrid.Children.Clear();
        }
        #endregion
        #endregion

        #endregion

        #region Two Wheel Car Input Methods

        #region Single Car And Setup

        /// <summary>
        /// Creates a two wheel model car/setup object and adds it to the two wheel model car/setup listbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _TwoWheelAddCarAndSetupToListBox_Click(object sender, RoutedEventArgs e)
        {
            if (twoWheelCarIDTextBox.Text != "" &&
                   twoWheelSetupIDTextBox.Text != "" &&
                   twoWheelAerodynamicsComboBox.Text != "" &&
                   twoWheelBrakesComboBox.Text != "" &&
                   twoWheelEngineComboBox.Text != "" &&
                   twoWheelInertiaAndDimensionsComboBox.Text != "" &&
                   twoWheelFrontSuspensionComboBox.Text != "" &&
                   twoWheelRearSuspensionComboBox.Text != "" &&
                   twoWheelSteeringSystemComboBox.Text != "" &&
                   twoWheelFrontTireComboBox.Text != "" &&
                   twoWheelRearTireComboBox.Text != "" &&
                   twoWheelTransmissionComboBox.Text != "")
            {
                // Gets the object's properties values
                string carID = twoWheelCarIDTextBox.Text;
                string setupID = twoWheelSetupIDTextBox.Text;
                string description = twoWheelCarAndSetupDescriptionTextBox.Text;
                Vehicle.TwoWheelAerodynamics aerodynamics = twoWheelAerodynamicsComboBox.SelectedItem as Vehicle.TwoWheelAerodynamics;
                Vehicle.TwoWheelBrakes brakes = twoWheelBrakesComboBox.SelectedItem as Vehicle.TwoWheelBrakes;
                Vehicle.Engine engine = twoWheelEngineComboBox.SelectedItem as Vehicle.Engine;
                Vehicle.TwoWheelInertiaAndDimensions inertiaAndDimensions = twoWheelInertiaAndDimensionsComboBox.SelectedItem as Vehicle.TwoWheelInertiaAndDimensions;
                Vehicle.SimplifiedSuspension frontSuspension = twoWheelFrontSuspensionComboBox.SelectedItem as Vehicle.SimplifiedSuspension;
                Vehicle.SimplifiedSuspension rearSuspension = twoWheelRearSuspensionComboBox.SelectedItem as Vehicle.SimplifiedSuspension;
                Vehicle.SteeringSystem steering = twoWheelSteeringSystemComboBox.SelectedItem as Vehicle.SteeringSystem;
                Vehicle.Tire frontTire = twoWheelFrontTireComboBox.SelectedItem as Vehicle.Tire;
                Vehicle.Tire rearTire = twoWheelRearTireComboBox.SelectedItem as Vehicle.Tire;
                Vehicle.TwoWheelTransmission transmission = twoWheelTransmissionComboBox.SelectedItem as Vehicle.TwoWheelTransmission;
                // Initializes a new object
                Vehicle.TwoWheelCar car = new Vehicle.TwoWheelCar(carID, setupID, description, aerodynamics, brakes, engine, inertiaAndDimensions, frontSuspension, rearSuspension, steering, frontTire, rearTire, transmission);
                // Gets additional parameters
                car.GetLinearAccelerationParameters();
                car.GetCarOperationSpeedRange();
                // Adds the object to the listbox
                twoWheelCarAndSetupListBox.Items.Add(car);
            }
            else System.Windows.MessageBox.Show(
                "Could not create Car/Setup. \n" +
                "   It should have a car ID. \n" +
                "   It should have a setup ID. \n" +
                "   All of the components boxes must be filled.",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }

        /// <summary>
        /// Deletes a two wheel model car/setup from the two wheel model car/setup listbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _TwoWheelDeleteCarAndSetupOfListBox_Click(object sender, RoutedEventArgs e)
        {
            // Checks if there's a listbox item selected and then removes it
            if (twoWheelCarAndSetupListBox.SelectedItems.Count == 1)
            {
                twoWheelCarAndSetupListBox.Items.RemoveAt(twoWheelCarAndSetupListBox.Items.IndexOf(twoWheelCarAndSetupListBox.SelectedItem));
            }
        }

        /// <summary>
        /// Loads the properties of a listbox's two wheel model car/setup and displays it in the UI fields.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _TwoWheelCarAndSetupListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // Checks if there's a listbox item selected
            if (twoWheelCarAndSetupListBox.SelectedItems.Count == 1)
            {
                // Gets the selected object
                Vehicle.TwoWheelCar car = twoWheelCarAndSetupListBox.SelectedItem as Vehicle.TwoWheelCar;
                // Writes the properties in the UI
                twoWheelCarIDTextBox.Text = car.ID;
                twoWheelSetupIDTextBox.Text = car.SetupID;
                twoWheelCarAndSetupDescriptionTextBox.Text = car.Description;
                twoWheelAerodynamicsComboBox.Text = car.Aerodynamics.ID;
                twoWheelBrakesComboBox.Text = car.Brakes.ID;
                twoWheelEngineComboBox.Text = car.Engine.ID;
                twoWheelInertiaAndDimensionsComboBox.Text = car.InertiaAndDimensions.ID;
                twoWheelFrontSuspensionComboBox.Text = car.FrontSuspension.ID;
                twoWheelRearSuspensionComboBox.Text = car.RearSuspension.ID;
                twoWheelSteeringSystemComboBox.Text = car.Steering.ID;
                twoWheelFrontTireComboBox.Text = car.FrontTire.ID;
                twoWheelRearTireComboBox.Text = car.RearTire.ID;
                twoWheelTransmissionComboBox.Text = car.Transmission.ID;
            }
        }

        #region Powertrain Diagram
        /// <summary>
        /// Updates the powertrain diagram when the transmission combobox item changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _TwoWheelTransmissionComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (twoWheelTransmissionComboBox.SelectedValue != null && twoWheelEngineComboBox.SelectedValue != null) _GenerateTwoWheelPowerTrainDiagram();
            else _ClearTwoWheelPowertrainDiagram();
        }
        /// <summary>
        /// Updates the powertrain diagram when the engine combobox item changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _TwoWheelEngineComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (twoWheelTransmissionComboBox.SelectedValue != null && twoWheelEngineComboBox.SelectedValue != null) _GenerateTwoWheelPowerTrainDiagram();
            else _ClearTwoWheelPowertrainDiagram();
        }

        private void _GenerateTwoWheelPowerTrainDiagram()
        {
            // Gets the engine and transmission objects
            Vehicle.Engine engine = twoWheelEngineComboBox.SelectedItem as Vehicle.Engine;
            Vehicle.Transmission transmission = twoWheelTransmissionComboBox.SelectedItem as Vehicle.Transmission;
            // Gets the view model from the engine and the transmission
            Vehicle.PowertrainViewModel powertrainViewModel = new Vehicle.PowertrainViewModel();
            powertrainViewModel.GetPowertrainCurvePoints(engine, transmission);
            // Initializes a new chart
            SfChart chart = new SfChart()
            {
                Header = "Powertrain Diagram - Engine: " + engine.ID + " - Transmission: " + transmission.ID,
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(10),
                Width = twoWheelCarAndSetupDockingWindow.ActualHeight,
                Legend = new ChartLegend()
            };
            // Adds the axis to the chart
            chart.PrimaryAxis = new NumericalAxis()
            {
                Header = "Wheel Angular Speed (rpm)",
                LabelFormat = "N0"
            };
            chart.SecondaryAxis = new NumericalAxis()
            {
                Header = "Wheel Torque (Nm)",
                LabelFormat = "N0"
            };
            // Adds zoom/panning behaviour to the chart
            /*ChartZoomPanBehavior zoomingAndPanning = new ChartZoomPanBehavior()
            {
                EnableZoomingToolBar = true,
                EnableMouseWheelZooming = true,
                EnablePanning = true,
                ZoomRelativeToCursor = true,
            };
            chart.Behaviors.Add(zoomingAndPanning);*/
            // Adds the curves data of each gear
            for (int iGear = 0; iGear < transmission.GearRatiosSet.GearRatios.Count; iGear++)
            {
                FastLineSeries fastLineSeries = new FastLineSeries()
                {
                    Label = "Gear: " + (iGear + 1).ToString(),
                    ItemsSource = powertrainViewModel.PowertrainDiagramCurvePoints[iGear],
                    XBindingPath = "WheelAngularSpeed",
                    YBindingPath = "Torque",
                    StrokeThickness = 5
                };
                chart.Series.Add(fastLineSeries);
            }
            // Clears the preview grid and displays the new chart
            twoWheelModelPowertrainDiagramDisplayGrid.Children.Clear();
            twoWheelModelPowertrainDiagramDisplayGrid.Children.Add(chart);
            DockingManager.SetDesiredWidthInDockedMode(twoWheelCarAndSetupDockingWindow, 250 + twoWheelCarAndSetupDockingWindow.ActualHeight);
        }

        /// <summary>
        /// Clears the two wheel powertrain diagram.
        /// </summary>
        private void _ClearTwoWheelPowertrainDiagram()
        {
            twoWheelModelPowertrainDiagramDisplayGrid.Children.Clear();
        }
        #endregion

        #endregion

        #endregion

        #endregion

        #region Path Input Methods

        /// <summary>
        /// Gets the x and y axis limits so that the path is centralized in the chart area.
        /// </summary>
        /// <param name="path"> Path to be displayed </param>
        /// <returns></returns>
        private double[] _GetPathChartAxisRange(Path path)
        {
            // Average coordinates
            double averageX = path.CoordinatesX.Average();
            double averageY = path.CoordinatesY.Average();
            // Path's minimum and maximum coordinates values
            double[] minCandidates = new double[2] { path.CoordinatesX.Min(), path.CoordinatesY.Min() };
            double[] maxCandidates = new double[2] { path.CoordinatesX.Max(), path.CoordinatesY.Max() };
            // Final axis range
            double dataRange = maxCandidates.Max() - minCandidates.Min();
            // Axis limits array
            double[] axisLimits = new double[4]
            {
                averageX - dataRange * 0.6,
                averageX + dataRange * 0.6,
                averageY - dataRange * 0.6,
                averageY + dataRange * 0.6
            };

            return axisLimits;
        }

        #region Tabular

        #region Main Tabular Path

        /// <summary>
        /// Creates a tabular path object and adds it to the tabular paths listbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _TabularPathAddPathToListBox_Click(object sender, RoutedEventArgs e)
        {
            if (tabularPathIDTextBox.Text != "" &&
                tabularPathSectorsSetComboBox.SelectedItem != null &&
                tabularPathSectionsSetComboBox.SelectedItem != null &&
                int.Parse(tabularPathResolutionTextBox.Text) != 0)
            {
                // Gets the object's data
                string pathID = tabularPathIDTextBox.Text;
                string description = tabularPathDescriptionTextBox.Text;
                PathSectorsSet pathSectorsSet = tabularPathSectorsSetComboBox.SelectedItem as PathSectorsSet;
                TabularPathSectionsSet pathSectionsSet = tabularPathSectionsSetComboBox.SelectedItem as TabularPathSectionsSet;
                double resolution = double.Parse(tabularPathResolutionTextBox.Text) / 1000;
                // Initializes a new object
                Path path = new Path(pathID, description, pathSectorsSet, pathSectionsSet, resolution);
                path.GeneratePathPointsParametersFromTabular();
                // Adds the object to the listbox and the ComboBox
                tabularPathsListBox.Items.Add(path);
            }
            else System.Windows.MessageBox.Show(
               "Could not create Tabular Path. \n " +
               "    It should have an ID. \n" +
               "    A sectors set must be selected. \n" +
               "    A sections set must be selected. \n" +
               "    The resolution can't be zero. \n" +
               "    Note: Negative values are corrected to positive values.",
               "Error",
               MessageBoxButton.OK,
               MessageBoxImage.Error);
        }

        /// <summary>
        /// Deletes a tabular path from the tabular paths listbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _TabularPathDeletePathOfListBox_Click(object sender, RoutedEventArgs e)
        {
            // Checks if there's a listbox item selected and then removes it
            if (tabularPathsListBox.SelectedItems.Count == 1)
            {
                tabularPathsListBox.Items.RemoveAt(tabularPathsListBox.Items.IndexOf(tabularPathsListBox.SelectedItem));
                if (tabularPathsListBox.Items.Count == 0) _ClearTabularPathDisplayChart();
            }
        }

        /// <summary>
        /// Loads the properties of the tabular paths listbox's tabular path and displays it in the UI fields.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _TabularPathsListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // Checks if there's a listbox item selected
            if (tabularPathsListBox.SelectedItems.Count == 1)
            {
                // Gets the selected object
                Path path = tabularPathsListBox.SelectedItem as Path;
                // Writes the properties in the UI
                tabularPathIDTextBox.Text = path.ID;
                tabularPathDescriptionTextBox.Text = path.Description;
                tabularPathSectorsSetComboBox.Text = path.SectorsSet.ToString();
                tabularPathSectionsSetComboBox.Text = path.TabularSectionsSet.ToString();
                tabularPathResolutionTextBox.Text = path.Resolution.ToString("F0");
                // Updates the path display chart if the checkbox is selected
                if ((bool)tabularPathAllowPathDisplayCheckBox.IsChecked) _UpdateTabularPathDisplayChart();
            }
        }

        /// <summary>
        /// Displays the path chart if the checkbox becomes checked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _TabularPathAllowPathDisplayCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (tabularPathsListBox.SelectedItems.Count == 1) _UpdateTabularPathDisplayChart();
        }

        /// <summary>
        /// Clears the tabular path display chart area if the checkbox is unchecked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _TabularPathAllowPathDisplayCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            _ClearTabularPathDisplayChart();
        }

        /// <summary>
        /// Updates the tabular path display chart.
        /// </summary>
        private void _UpdateTabularPathDisplayChart()
        {
            // Gets the path object from the path listbox
            Path path = tabularPathsListBox.SelectedItem as Path;
            // Gets the coordinates of the points of each path sector
            List<PathViewModel> sectorsPointsViewModels = new List<PathViewModel>();
            for (int iSector = 0; iSector < path.SectorsSet.Sectors.Count; iSector++)
            {
                PathViewModel currentSectorViewModel = new PathViewModel();
                for (int iPoint = 0; iPoint < path.LocalSectorIndex.Count; iPoint++)
                {
                    if (path.LocalSectorIndex[iPoint] == iSector + 1)
                    {
                        PathPoint pathPoint = new PathPoint(path.CoordinatesX[iPoint], path.CoordinatesY[iPoint]);
                        currentSectorViewModel.PathPoints.Add(pathPoint);
                    }
                }
                sectorsPointsViewModels.Add(currentSectorViewModel);
            }
            // Initializes a new chart
            SfChart chart = new SfChart()
            {
                Header = "Path Display: " + path.ID,
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(10),
                Width = tabularPathDockingWindow.ActualHeight,
                Legend = new ChartLegend()
            };
            // Gets the axis minimum and maximum values
            double[] axisLimits = _GetPathChartAxisRange(path);
            // Adds the axis to the chart
            chart.PrimaryAxis = new NumericalAxis()
            {
                Header = "X Coordinates (m)",
                Minimum = axisLimits[0],
                Maximum = axisLimits[1],
                LabelFormat = "N0"
            };
            chart.SecondaryAxis = new NumericalAxis()
            {
                Header = "Y Coordinates (m)",
                Minimum = axisLimits[2],
                Maximum = axisLimits[3],
                LabelFormat = "N0"
            };
            // Adds zoom/panning behaviour to the chart
            /*ChartZoomPanBehavior zoomingAndPanning = new ChartZoomPanBehavior()
            {
                EnableZoomingToolBar = true,
                EnableMouseWheelZooming = true,
                EnablePanning = true,
                ZoomRelativeToCursor = true,
            };
            chart.Behaviors.Add(zoomingAndPanning);*/
            // Generates and adds the sectors data series to be added to the chart
            for (int iSector = 0; iSector < sectorsPointsViewModels.Count; iSector++)
            {
                FastLineSeries fastLineSeries = new FastLineSeries()
                {
                    Label = "Sector " + (iSector + 1).ToString("N0"),
                    ItemsSource = sectorsPointsViewModels[iSector].PathPoints,
                    XBindingPath = "CoordinateX",
                    YBindingPath = "CoordinateY",
                    StrokeThickness = 10
                };
                chart.Series.Add(fastLineSeries);
            }
            // Generates and adds the data series to be added to the chart
            PathViewModel pathViewModel = new PathViewModel(path);
            FastLineSeries series = new FastLineSeries()
            {
                Label = path.ID,
                ItemsSource = pathViewModel.PathPoints,
                XBindingPath = "CoordinateX",
                YBindingPath = "CoordinateY",
                StrokeThickness = 5
            };
            chart.Series.Add(series);
            // Clears the preview grid and displays the new chart
            tabularPathDisplayGrid.Children.Clear();
            tabularPathDisplayGrid.Children.Add(chart);
            DockingManager.SetDesiredWidthInDockedMode(tabularPathDockingWindow, 250 + tabularPathDockingWindow.ActualHeight);
        }

        /// <summary>
        /// Clears the tabular path display chart area.
        /// </summary>
        private void _ClearTabularPathDisplayChart()
        {
            // Clears the path sections preview grid
            tabularPathDisplayGrid.Children.Clear();
            // Resets the docking window width
            DockingManager.SetDesiredWidthInDockedMode(tabularPathDockingWindow, 250);
        }

        #endregion

        #region Path Sectors

        /// <summary>
        /// Creates a tabular path sectors set object and adds it to the tabular path sectors sets listbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _TabularPathAddSectorsSetToListBox_Click(object sender, RoutedEventArgs e)
        {
            if (tabularPathSectorsSetIDTextBox.Text != "" &&
                tabularPathSectorsStartDistancesListBox.Items.Count != 0)
            {
                // Gets the object's properties values
                string id = tabularPathSectorsSetIDTextBox.Text;
                string description = tabularPathSectorsSetDescriptionTextBox.Text;
                List<PathSector> pathSectorsStartDistances = new List<PathSector>();
                foreach (var sectorStartDistanceItem in tabularPathSectorsStartDistancesListBox.Items)
                {
                    PathSector pathSector = sectorStartDistanceItem as PathSector;
                    pathSectorsStartDistances.Add(pathSector);
                }
                // Initializes a new object
                PathSectorsSet pathSectorsSet = new PathSectorsSet(id, description, pathSectorsStartDistances);
                // Adds the object to the listbox and the ComboBox
                tabularPathSectorsSetsListBox.Items.Add(pathSectorsSet);
            }
            else System.Windows.MessageBox.Show(
                "Could not create Path Sectors Set. \n " +
                "   It should have an ID. \n" +
                "   The sectors list can't be empty.",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }

        /// <summary>
        /// Deletes a tabular path sectors set from the one wheel model tabular path sectors sets listbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _TabularPathDeleteSectorsSetOfListBox_Click(object sender, RoutedEventArgs e)
        {
            // Checks if there's a listbox item selected and then removes it
            if (tabularPathSectorsSetsListBox.SelectedItems.Count == 1)
            {
                tabularPathSectorsSetsListBox.Items.RemoveAt(tabularPathSectorsSetsListBox.Items.IndexOf(tabularPathSectorsSetsListBox.SelectedItem));
            }
        }

        /// <summary>
        /// Loads the properties of the tabular path sectors sets listbox's path sectors set and displays it in the UI fields.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _TabularPathSectorsListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // Checks if there's a listbox item selected
            if (tabularPathSectorsSetsListBox.SelectedItems.Count == 1)
            {
                // Gets the selected object
                PathSectorsSet sectorsSet = tabularPathSectorsSetsListBox.SelectedItem as PathSectorsSet;
                // Writes the properties in the UI
                tabularPathSectorsSetIDTextBox.Text = sectorsSet.ID;
                tabularPathSectorsSetDescriptionTextBox.Text = sectorsSet.Description;
                // Clears and writes the list in the UI
                tabularPathSectorsStartDistancesListBox.Items.Clear();
                foreach (PathSector pathSector in sectorsSet.Sectors)
                {
                    tabularPathSectorsStartDistancesListBox.Items.Add(pathSector);
                }
            }
        }

        /// <summary>
        /// Creates a tabular path sector object and adds it to the tabular path sector start distances listbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _TabularPathAddSectorStartDistanceToListBox_Click(object sender, RoutedEventArgs e)
        {
            if (double.Parse(tabularPathNewSectorStartDistanceTextBox.Text) != 0)
            {
                // Gets the object's properties values
                double startDistance = double.Parse(tabularPathNewSectorStartDistanceTextBox.Text);
                if (startDistance < 0) startDistance = -startDistance;
                // Initializes a new object
                PathSector pathSector = new PathSector(0, startDistance);
                // Adds the object to the listbox and the ComboBox
                tabularPathSectorsStartDistancesListBox.Items.Add(pathSector);
                _ReorderAndResetListboxSectorsIndexes();
            }
            else System.Windows.MessageBox.Show(
                "Could not create Sector Start Distance. \n " +
                "   The start distance can't be zero. \n" +
                "   Note: Negative values are corrected to positive values.",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }

        /// <summary>
        /// Deletes a tabular path sector start distances from the tabular path sector start distances listbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _TabularPathDeleteSectorStartDistanceOfListBox_Click(object sender, RoutedEventArgs e)
        {
            if (tabularPathSectorsStartDistancesListBox.SelectedItems.Count == 1)
            {
                PathSector selectedStartDistance = tabularPathSectorsStartDistancesListBox.SelectedItem as PathSector;
                if (selectedStartDistance.StartDistance != 0)
                {
                    tabularPathSectorsStartDistancesListBox.Items.RemoveAt(tabularPathSectorsStartDistancesListBox.Items.IndexOf(tabularPathSectorsStartDistancesListBox.SelectedItem));
                    _ReorderAndResetListboxSectorsIndexes();
                }
            }
        }

        /// <summary>
        /// Reorders and resets the indexes of the path sectors listbox's sectors.
        /// </summary>
        private void _ReorderAndResetListboxSectorsIndexes()
        {
            // Initializes a new path sectors list and fills it with the listbox's objects
            List<PathSector> pathSectors = new List<PathSector>();
            foreach (PathSector pathSector in tabularPathSectorsStartDistancesListBox.Items)
            {
                pathSectors.Add(pathSector);
            }
            // Reorders the path sectors cording to their starting distances
            pathSectors = pathSectors.OrderBy(pathSector => pathSector.StartDistance).ToList();
            // Clears the listbox
            tabularPathSectorsStartDistancesListBox.Items.Clear();
            // Assigns the new indexes and adds the path sectors to the listbox
            for (int iSector = 0; iSector < pathSectors.Count; iSector++)
            {
                pathSectors[iSector].Index = iSector + 1;
                tabularPathSectorsStartDistancesListBox.Items.Add(pathSectors[iSector]);
            }
        }

        #endregion

        #region Path Sections

        /// <summary>
        /// Creates a tabular path sections set object and adds it to the tabular path sections sets listbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _TabularPathAddSectionsSetToListBox_Click(object sender, RoutedEventArgs e)
        {
            if (tabularPathSectionsSetIDTextBox.Text != "" &&
                tabularPathSectionsListBox.Items.Count != 0)
            {
                // Gets the object's properties values
                string id = tabularPathSectionsSetIDTextBox.Text;
                string description = tabularPathSectionsSetDescriptionTextBox.Text;
                List<TabularPathSection> pathSections = new List<TabularPathSection>();
                foreach (var pathSectionItem in tabularPathSectionsListBox.Items)
                {
                    TabularPathSection pathSection = pathSectionItem as TabularPathSection;
                    pathSections.Add(pathSection);
                }
                // Initializes a new object
                TabularPathSectionsSet pathSectionsSet = new TabularPathSectionsSet(id, description, pathSections);
                // Adds the object to the listbox and the ComboBox
                tabularPathSectionsSetsListBox.Items.Add(pathSectionsSet);
            }
            else System.Windows.MessageBox.Show(
                "Could not create Path Sections Set. \n " +
                "   It should have an ID. \n" +
                "   The sections list can't be empty.",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }

        /// <summary>
        /// Deletes a tabular path sections set from the one wheel model tabular path sections sets listbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _TabularPathDeleteSectionsSetOfListBox_Click(object sender, RoutedEventArgs e)
        {
            // Checks if there's a listbox item selected and then removes it
            if (tabularPathSectionsSetsListBox.SelectedItems.Count == 1)
            {
                tabularPathSectionsSetsListBox.Items.RemoveAt(tabularPathSectionsSetsListBox.Items.IndexOf(tabularPathSectionsSetsListBox.SelectedItem));
            }
        }

        /// <summary>
        /// Loads the properties of the tabular path sections sets listbox's path section and displays it in the UI fields.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _TabularPathSectionsSetListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // Checks if there's a listbox item selected
            if (tabularPathSectionsSetsListBox.SelectedItems.Count == 1)
            {
                // Gets the selected object
                TabularPathSectionsSet sectionsSet = tabularPathSectionsSetsListBox.SelectedItem as TabularPathSectionsSet;
                // Writes the properties in the UI
                tabularPathSectionsSetIDTextBox.Text = sectionsSet.ID;
                tabularPathSectionsSetDescriptionTextBox.Text = sectionsSet.Description;
                // Clears and writes the list in the UI
                tabularPathSectionsListBox.Items.Clear();
                foreach (TabularPathSection pathSection in sectionsSet.Sections)
                {
                    tabularPathSectionsListBox.Items.Add(pathSection);
                }
                // Updates the preview chart if the checkbox is selected
                if ((bool)tabularPathSectionsAllowPathPreviewCheckBox.IsChecked) _UpdateTabularPathSectionsPreviewChart();
            }
        }
        /// <summary>
        /// Creates a tabular path section object and adds it to the tabular path sections listbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _TabularPathAddSectionToListBox_Click(object sender, RoutedEventArgs e)
        {
            if (tabularPathNewSectionTypeComboBox.SelectedItem != null &&
                double.Parse(tabularPathNewSectionLengthTextBox.Text) != 0)
            {
                // Gets the object's properties values
                double length = double.Parse(tabularPathNewSectionLengthTextBox.Text);
                if (length < 0) length = -length;
                TabularPathSection.SectionType sectionType;
                double initialRadius;
                double finalRadius;
                switch (tabularPathNewSectionTypeComboBox.Text)
                {
                    case ("Left"):
                        sectionType = TabularPathSection.SectionType.Left;
                        initialRadius = double.Parse(tabularPathNewSectionInitialRadiusTextBox.Text);
                        finalRadius = double.Parse(tabularPathNewSectionFinalRadiusTextBox.Text);
                        break;
                    case ("Right"):
                        sectionType = TabularPathSection.SectionType.Right;
                        initialRadius = double.Parse(tabularPathNewSectionInitialRadiusTextBox.Text);
                        finalRadius = double.Parse(tabularPathNewSectionFinalRadiusTextBox.Text);
                        break;
                    default:
                        sectionType = TabularPathSection.SectionType.Straight;
                        initialRadius = 0;
                        finalRadius = 0;
                        break;
                }
                if (initialRadius < 0) initialRadius = -initialRadius;
                if (finalRadius < 0) finalRadius = -finalRadius;
                // Initializes a new object
                TabularPathSection pathSection = new TabularPathSection(sectionType, length, initialRadius, finalRadius);
                // Adds the object to the listbox and the ComboBox
                tabularPathSectionsListBox.Items.Add(pathSection);
                // Updates the path preview chart
                if ((bool)tabularPathSectionsAllowPathPreviewCheckBox.IsChecked) _UpdateTabularPathSectionsPreviewChart();
            }
            else System.Windows.MessageBox.Show(
                "Could not create Section. \n " +
                "   The length can't be zero. \n" +
                "   Note: Negative values are corrected to positive values. \n" +
                "   Note: Radius equal to zero means straight section. \n" +
                "   Note: Straight sections radiuses are corrected to zero.",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }

        /// <summary>
        /// Deletes a tabular path section from the tabular path section listbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _TabularPathDeleteSectionOfListBox_Click(object sender, RoutedEventArgs e)
        {
            // Checks if there's a listbox item selected and then removes it
            if (tabularPathSectionsListBox.SelectedItems.Count == 1)
            {
                tabularPathSectionsListBox.Items.RemoveAt(tabularPathSectionsListBox.Items.IndexOf(tabularPathSectionsListBox.SelectedItem));
                // Updates the path preview chart
                if ((bool)tabularPathSectionsAllowPathPreviewCheckBox.IsChecked && tabularPathSectionsListBox.Items.Count != 0) _UpdateTabularPathSectionsPreviewChart();
                else _ClearTabularPathSectionsPreviewChart();
            }
        }

        /// <summary>
        /// Updates the tabular path sections preview chart when the checkbox is checked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _TabularPathSectionsAllowPathPreviewCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (tabularPathSectionsListBox.Items.Count > 0) _UpdateTabularPathSectionsPreviewChart();
        }

        /// <summary>
        /// Clears the tabular path sections preview chart when the checkbox is checked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _TabularPathSectionsAllowPathPreviewCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            _ClearTabularPathSectionsPreviewChart();
        }

        /// <summary>
        /// Updates the sections preview chart.
        /// </summary>
        private void _UpdateTabularPathSectionsPreviewChart()
        {
            // Gets a new path sections set from the UI
            List<TabularPathSection> pathSections = new List<TabularPathSection>();
            foreach (var pathSectionItem in tabularPathSectionsListBox.Items)
            {
                TabularPathSection pathSection = pathSectionItem as TabularPathSection;
                pathSections.Add(pathSection);
            }
            TabularPathSectionsSet sectionsSet = new TabularPathSectionsSet("", "", pathSections);
            // Initializes a new path with the path sections information
            PathSectorsSet sectorsSet = new PathSectorsSet("", "", new List<PathSector>() { new PathSector(0, 0) });
            Path path = new Path("", "", sectorsSet, sectionsSet, 0.1);
            path.GeneratePathPointsParametersFromTabular();
            PathViewModel pathViewModel = new PathViewModel(path);
            // Initializes a new chart
            SfChart chart = new SfChart()
            {
                Header = "Path Preview",
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(5),
                Width = tabularPathSectionsDockingWindow.ActualHeight
            };
            // Gets the axis minimum and maximum values
            double[] axisLimits = _GetPathChartAxisRange(path);
            // Adds the axis to the chart
            chart.PrimaryAxis = new NumericalAxis()
            {
                Header = "X Coordinates (m)",
                Minimum = axisLimits[0],
                Maximum = axisLimits[1],
                LabelFormat = "N0"
            };
            chart.SecondaryAxis = new NumericalAxis()
            {
                Header = "Y Coordinates (m)",
                Minimum = axisLimits[2],
                Maximum = axisLimits[3],
                LabelFormat = "N0"
            };
            // Adds zoom/panning behaviour to the chart
            /*ChartZoomPanBehavior zoomingAndPanning = new ChartZoomPanBehavior()
            {
                EnableZoomingToolBar = true,
                EnableMouseWheelZooming = true,
                EnablePanning = true,
                ZoomRelativeToCursor = true,
            };
            chart.Behaviors.Add(zoomingAndPanning);*/
            // Generates and adds the data series to be added to the chart
            FastLineSeries series = new FastLineSeries()
            {
                ItemsSource = pathViewModel.PathPoints,
                XBindingPath = "CoordinateX",
                YBindingPath = "CoordinateY",
                StrokeThickness = 10
            };
            chart.Series.Add(series);
            // Clears the preview grid and displays the new chart
            tabularPathSectionsPreviewGrid.Children.Clear();
            tabularPathSectionsPreviewGrid.Children.Add(chart);
            DockingManager.SetDesiredWidthInDockedMode(tabularPathSectionsDockingWindow, 250 + tabularPathSectionsDockingWindow.ActualHeight);
        }

        /// <summary>
        /// Clears the path sections preview chart area.
        /// </summary>
        private void _ClearTabularPathSectionsPreviewChart()
        {
            // Clears the path sections preview grid
            tabularPathSectionsPreviewGrid.Children.Clear();
            // Resets the docking window width
            DockingManager.SetDesiredWidthInDockedMode(tabularPathSectionsDockingWindow, 250);
        }

        #endregion

        #endregion

        #endregion

        #region Simulation Methods

        #region GGV Diagram Input Methods
        /// <summary>
        /// Creates a ggv diagram object, generates it and adds it to the simulation ggv diagrams listbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _SimulationAddGGVDiagramToListBox_Click(object sender, RoutedEventArgs e)
        {
            if (ggvDiagramIDTextBox.Text != "" &&
                ggvDiagramVehicleTypeSelectionComboBox.SelectedItem != null &&
                ggvDiagramVehicleSelectionComboBox.SelectedItem != null &&
                int.Parse(ggvDiagramAmountOfPointsPerSpeedTextBox.Text) >= 4 &&
                int.Parse(ggvDiagramAmountOfSpeedsTextBox.Text) != 0 &&
                Math.Abs(double.Parse(ggvDiagramLowestSpeedTextBox.Text)) <= Math.Abs(double.Parse(ggvDiagramHighestSpeedTextBox.Text)))
            {
                // Gets the object's data
                string id = ggvDiagramIDTextBox.Text;
                string description = ggvDiagramDescriptionTextBox.Text;
                int amountOfPointsPerSpeed = int.Parse(ggvDiagramAmountOfPointsPerSpeedTextBox.Text);
                int amountOfSpeeds = int.Parse(ggvDiagramAmountOfSpeedsTextBox.Text);
                double lowestSpeed = double.Parse(ggvDiagramLowestSpeedTextBox.Text) / 3.6;
                double highestSpeed = double.Parse(ggvDiagramHighestSpeedTextBox.Text) / 3.6;
                // Progress bar window
                UIClasses.ProgressBars.TaskProgressBarWindow progressBarWindow = new UIClasses.ProgressBars.TaskProgressBarWindow();
                progressBarWindow.taskID.Text = "Generating GGV Diagram. ID: " + id;
                progressBarWindow.taskProgressBar.Value = 0;
                progressBarWindow.Show();
                // Background Worker definition
                BackgroundWorker ggvGenerationWorker = new BackgroundWorker();
                ggvGenerationWorker.WorkerReportsProgress = true;
                // Checks for the car model
                switch (ggvDiagramVehicleTypeSelectionComboBox.SelectedValue.ToString())
                {
                    case "One Wheel":
                        // Gets the car object
                        Vehicle.OneWheelCar oneWheelCar = ggvDiagramVehicleSelectionComboBox.SelectedItem as Vehicle.OneWheelCar;
                        // Initializes and generates the GGV diagram
                        Simulation.GGVDiagram oneWheelGGVDiagram = new Simulation.GGVDiagram(id, description, amountOfPointsPerSpeed, amountOfSpeeds, lowestSpeed, highestSpeed, oneWheelCar);
                        // Worker set up
                        ggvGenerationWorker.DoWork += oneWheelGGVDiagram.GenerateGGVDiagramForTheOneWheelModel;
                        ggvGenerationWorker.ProgressChanged += ReportProgressToProgressBar;
                        ggvGenerationWorker.RunWorkerCompleted += GenerateOneWheelGGVDiagramCompleted;
                        // Generates the ggv diagram
                        ggvGenerationWorker.RunWorkerAsync(oneWheelGGVDiagram);
                        void GenerateOneWheelGGVDiagramCompleted(object internalSender, RunWorkerCompletedEventArgs internalE)
                        {
                            oneWheelGGVDiagram.GGDiagrams = (internalE.Result as Simulation.GGVDiagram).GGDiagrams;
                            progressBarWindow.Close();
                            // Adds the GGV diagram to the listbox
                            simulationGGVDiagramListBox.Items.Add(oneWheelGGVDiagram);
                        }
                        break;
                    case "Two Wheel":
                        // Gets the car object
                        Vehicle.TwoWheelCar twoWheelCar = ggvDiagramVehicleSelectionComboBox.SelectedItem as Vehicle.TwoWheelCar;
                        // Initializes and generates the GGV diagram
                        Simulation.GGVDiagram twoWheelGGVDiagram = new Simulation.GGVDiagram(id, description, amountOfPointsPerSpeed, amountOfSpeeds, lowestSpeed, highestSpeed, twoWheelCar);
                        // Worker set up
                        ggvGenerationWorker.DoWork += twoWheelGGVDiagram.GenerateGGVDiagramForTheTwoWheelModel;
                        ggvGenerationWorker.ProgressChanged += ReportProgressToProgressBar;
                        ggvGenerationWorker.RunWorkerCompleted += GenerateTwoWheelGGVDiagramCompleted;
                        // Generates the ggv diagram
                        ggvGenerationWorker.RunWorkerAsync(twoWheelGGVDiagram);
                        void GenerateTwoWheelGGVDiagramCompleted(object internalSender, RunWorkerCompletedEventArgs internalE)
                        {
                            twoWheelGGVDiagram.GGDiagrams = (internalE.Result as Simulation.GGVDiagram).GGDiagrams;
                            progressBarWindow.Close();
                            // Adds the GGV diagram to the listbox
                            simulationGGVDiagramListBox.Items.Add(twoWheelGGVDiagram);
                        }
                        break;
                    default:
                        break;
                }

                void ReportProgressToProgressBar(object internalSender, ProgressChangedEventArgs internalE)
                {
                    progressBarWindow.taskProgressBar.Value = internalE.ProgressPercentage;
                }
            }
            else System.Windows.MessageBox.Show(
               "Could not create GGV Diagram. \n " +
               "    It should have an ID. \n" +
               "    A vehicle model must be selected. \n" +
               "    A vehicle must be selected. \n" +
               "    The amount of points per speed should be at least 8. \n" +
               "    The amount of directions should be at least 4. \n" +
               "    The amount of speeds can't be zero. \n" +
               "    The lowest speed should be smaller or equal to the highest speed. \n" +
               "    Note: Negative values are corrected to positive values.",
               "Error",
               MessageBoxButton.OK,
               MessageBoxImage.Error);
        }
        /// <summary>
        /// Deletes a ggv diagram from the ggv diagrams listbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _SimulationDeleteGGVDiagramOfListBox_Click(object sender, RoutedEventArgs e)
        {
            // Checks if there's a listbox item selected and then removes it
            if (simulationGGVDiagramListBox.SelectedItems.Count == 1)
            {
                simulationGGVDiagramListBox.Items.RemoveAt(simulationGGVDiagramListBox.Items.IndexOf(simulationGGVDiagramListBox.SelectedItem));
                if (simulationGGVDiagramListBox.Items.Count == 0) _ClearSimulationGGVDiagramDisplayChart();
            }
        }
        /// <summary>
        /// Changes the GGV diagram input environment accordingly to the selected vehicle model.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _GGVDiagramVehicleTypeSelectionComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            switch (ggvDiagramVehicleTypeSelectionComboBox.SelectedValue.ToString())
            {
                case "One Wheel":
                    ggvDiagramVehicleSelectionComboBox.ItemsSource = oneWheelCarAndSetupListBox.Items;
                    break;
                case "Two Wheel":
                    ggvDiagramVehicleSelectionComboBox.ItemsSource = twoWheelCarAndSetupListBox.Items;
                    break;
                default:
                    ggvDiagramVehicleSelectionComboBox.ItemsSource = null;
                    break;
            }
        }
        /// <summary>
        /// Loads the properties of the simulation ggv diagrams listbox's ggv diagram and displays it in the UI fields.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _SimulationGGVDiagramListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // Checks if there's a listbox item selected
            if (simulationGGVDiagramListBox.SelectedItems.Count == 1)
            {
                // Gets the selected object
                Simulation.GGVDiagram ggvDiagram = simulationGGVDiagramListBox.SelectedItem as Simulation.GGVDiagram;
                // Writes the properties in the UI
                ggvDiagramIDTextBox.Text = ggvDiagram.ID;
                ggvDiagramDescriptionTextBox.Text = ggvDiagram.Description;
                ggvDiagramAmountOfPointsPerSpeedTextBox.Text = ggvDiagram.AmountOfPointsPerSpeed.ToString();
                ggvDiagramAmountOfSpeedsTextBox.Text = ggvDiagram.AmountOfSpeeds.ToString("F0");
                ggvDiagramLowestSpeedTextBox.Text = (ggvDiagram.LowestSpeed * 3.6).ToString("F2");
                ggvDiagramHighestSpeedTextBox.Text = (ggvDiagram.HighestSpeed * 3.6).ToString("F2");
                switch (ggvDiagram.CarModelType)
                {
                    case CarModelType.OneWheel:
                        ggvDiagramVehicleSelectionComboBox.Text = ggvDiagram.OneWheelCar.ToString();
                        break;
                    case CarModelType.TwoWheel:
                        ggvDiagramVehicleSelectionComboBox.Text = ggvDiagram.TwoWheelCar.ToString();
                        break;
                    default:
                        break;
                }
                // Updates the ggv diagram display chart
                if ((bool)simulationGGVDiagramAllowPathDisplayCheckBox.IsChecked) _UpdateSimulationGGVDiagramDisplayChart();
            }
        }

        #region GGV Diagram Display
        /// <summary>
        /// Displays the path chart if the checkbox becomes checked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _SimulationGGVDiagramAllowPathDisplayCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (simulationGGVDiagramListBox.SelectedItems.Count == 1) _UpdateSimulationGGVDiagramDisplayChart();
        }

        /// <summary>
        /// Clears the GGV Diagram display chart area if the checkbox is unchecked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _SimulationGGVDiagramAllowPathDisplayCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            _ClearSimulationGGVDiagramDisplayChart();
        }

        /// <summary>
        /// Updates the GGV Diagram display chart.
        /// </summary>
        private void _UpdateSimulationGGVDiagramDisplayChart()
        {
            // Loads the GGV Diagram object and geneates its view model
            Simulation.GGVDiagram ggvDiagram = simulationGGVDiagramListBox.SelectedItem as Simulation.GGVDiagram;
            if (ggvDiagram.GGDiagrams.Count < ggvDiagram.AmountOfSpeeds)
            {
                return;
            }
            Simulation.GGVDiagramViewModel ggvDiagramViewModel = new Simulation.GGVDiagramViewModel(ggvDiagram);
            // Initializes the surface chart
            SfSurfaceChart surface = new SfSurfaceChart()
            {
                Header = "GGV Diagram: " + ggvDiagram.ID,
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(10),
                Width = ggvDiagramDefinitionDockingWindow.ActualHeight,
                RowSize = ggvDiagramViewModel.RowSize,
                ColumnSize = ggvDiagramViewModel.ColumnSize,
                ColorBar = new ChartColorBar() { DockPosition = ChartDock.Right },
                Type = SurfaceType.WireframeSurface,
                EnableRotation = true,
                EnableZooming = true
            };
            // Adds the data to the chart
            for (int i = 0; i < ggvDiagramViewModel.GGVDiagramPoints.Count; i++)
            {
                surface.Data.AddPoints(
                    ggvDiagramViewModel.GGVDiagramPoints[i].LongitudinalAcceleration,
                    ggvDiagramViewModel.GGVDiagramPoints[i].Speed,
                    ggvDiagramViewModel.GGVDiagramPoints[i].LateralAcceleration);
            }
            // Initializes the surface axis and adds them to the chart
            SurfaceAxis xAxis = new SurfaceAxis()
            {
                Header = "Longitudinal Acceleration [m/s²]",
                Minimum = surface.Data.XValues.Min(),
                Maximum = surface.Data.XValues.Max(),
                LabelFormat = "0.00",
                FontSize = 15
            };
            SurfaceAxis yAxis = new SurfaceAxis()
            {
                Header = "Speed [km/h]",
                Minimum = surface.Data.YValues.Min(),
                Maximum = surface.Data.YValues.Max(),
                LabelFormat = "0.00",
                FontSize = 15
            };
            SurfaceAxis zAxis = new SurfaceAxis()
            {
                Header = "Lateral Acceleration [m/s²]",
                Minimum = surface.Data.ZValues.Min(),
                Maximum = surface.Data.ZValues.Max(),
                LabelFormat = "0.00",
                FontSize = 15
            };
            surface.XAxis = xAxis;
            surface.YAxis = yAxis;
            surface.ZAxis = zAxis;
            // Clears the preview grid and displays the new chart
            ggvDiagramDisplayGrid.Children.Clear();
            ggvDiagramDisplayGrid.Children.Add(surface);
            DockingManager.SetDesiredWidthInDockedMode(ggvDiagramDefinitionDockingWindow, 250 + ggvDiagramDefinitionDockingWindow.ActualHeight);
        }

        /// <summary>
        /// Clears the GGV Diagram display chart area.
        /// </summary>
        private void _ClearSimulationGGVDiagramDisplayChart()
        {
            // Clears the path sections preview grid
            ggvDiagramDisplayGrid.Children.Clear();
            // Resets the docking window width
            DockingManager.SetDesiredWidthInDockedMode(ggvDiagramDefinitionDockingWindow, 250);
        }

        #endregion
        #endregion
        #region Lap Time Simulation Inputs Methods

        /// <summary>
        /// Creates a lap time simulation object, runs the simulation, adds it to the lap time simulation listbox and 
        /// adds the results object to the results analysis lap time simulation listbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _LapTimeSimulationAddToListBox_Click(object sender, RoutedEventArgs e)
        {
            if (lapTimeSimulationIDTextBox.Text != "" &&
                lapTimeSimulationModeComboBox.SelectedItem != null &&
                lapTimeSimulationPathComboBox.SelectedItem != null &&
                lapTimeSimulationGGVDiagramComboBox.SelectedItem != null)
            {
                // Gets the object's data
                string id = lapTimeSimulationIDTextBox.Text;
                string description = lapTimeSimulationDescriptionTextBox.Text;
                string mode = lapTimeSimulationModeComboBox.Text;
                bool isFirstLap = false;
                if (mode == "First Lap") isFirstLap = true;
                Path path = lapTimeSimulationPathComboBox.SelectedItem as Path;
                Simulation.GGVDiagram ggvDiagram = lapTimeSimulationGGVDiagramComboBox.SelectedItem as Simulation.GGVDiagram;
                // Generates the GGV diagrams list
                List<Simulation.LapTimeSimulationSectorSetup> ggvDiagrams = new List<Simulation.LapTimeSimulationSectorSetup>();
                for (int iSector = 1; iSector <= path.SectorsSet.Sectors.Count; iSector++)
                {
                    bool isGGVDiagramSpecific = false;
                    // Sweeps through the sectors setup listbox and adds the specific GGV diagram requested by the user
                    foreach (Simulation.LapTimeSimulationSectorSetup sectorSetup in lapTimeSimulationGGVDiagramPerSectorListBox.Items)
                    {
                        if (sectorSetup.SectorIndex == iSector)
                        {
                            isGGVDiagramSpecific = true;
                            ggvDiagrams.Add(sectorSetup);
                        }
                    }
                    // Adds the default GGV diagram if the sector has no specific GGV diagram.
                    if (!isGGVDiagramSpecific) ggvDiagrams.Add(new Simulation.LapTimeSimulationSectorSetup(iSector, ggvDiagram));
                }
                // Progress bar window
                UIClasses.ProgressBars.TaskProgressBarWindow progressBarWindow = new UIClasses.ProgressBars.TaskProgressBarWindow();
                progressBarWindow.taskID.Text = "Running Lap Time Simulation. ID: " + id;
                progressBarWindow.taskProgressBar.Value = 0;
                progressBarWindow.Show();
                // Initializes a new object
                Simulation.LapTimeSimulation lapTimeSimulation = new Simulation.LapTimeSimulation(id, description, path, ggvDiagrams, isFirstLap);
                Results.LapTimeSimulationResults results;
                // Background Worker definition
                BackgroundWorker lapTimeSimulationWorker = new BackgroundWorker();
                lapTimeSimulationWorker.WorkerReportsProgress = true;
                lapTimeSimulationWorker.DoWork += lapTimeSimulation.RunLapTimeSimulation;
                lapTimeSimulationWorker.ProgressChanged += _ReportProgressToProgressBar;
                lapTimeSimulationWorker.RunWorkerCompleted += _LapTimeSimulationWorkCompleted;
                // Runs the lap time simulation
                lapTimeSimulationWorker.RunWorkerAsync(lapTimeSimulation);
                void _ReportProgressToProgressBar(object internalSender, ProgressChangedEventArgs internalE)
                {
                    progressBarWindow.taskProgressBar.Value = internalE.ProgressPercentage;
                }
                void _LapTimeSimulationWorkCompleted(object internalSender, RunWorkerCompletedEventArgs internalE)
                {
                    lapTimeSimulation = internalE.Result as Simulation.LapTimeSimulation;
                    results = lapTimeSimulation.Results;
                    progressBarWindow.Close();
                    _AddResultsToListboxes();
                }
                void _AddResultsToListboxes()
                {
                    // Adds the object to the listbox and the ComboBox
                    lapTimeSimulationListBox.Items.Add(lapTimeSimulation);
                    lapTimeSimulationResultsAnalysisResultsListBox.Items.Add(results);
                    lapTimeSimulationResultsAnalysisResultsComboBox.Items.Add(results);
                }
            }
            else System.Windows.MessageBox.Show(
               "Could not create Lap Time Simulation. \n " +
               "    It should have an ID. \n" +
               "    A mode must be selected. \n" +
               "    A path must be selected. \n" +
               "    A GGV Diagram must be selected.",
               "Error",
               MessageBoxButton.OK,
               MessageBoxImage.Error);
        }
        /// <summary>
        /// Updates the progress bar's value.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <summary>
        /// Deletes a lap time simulation from the lap time simulation listbox and its results object from the results analysis listbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _LapTimeSimulationDeleteOfListBox_Click(object sender, RoutedEventArgs e)
        {
            // Checks if there's a listbox item selected and then removes it
            if (lapTimeSimulationListBox.SelectedItems.Count == 1)
            {
                lapTimeSimulationResultsAnalysisResultsListBox.Items.RemoveAt(lapTimeSimulationListBox.Items.IndexOf(lapTimeSimulationListBox.SelectedItem));
                lapTimeSimulationResultsAnalysisResultsComboBox.Items.RemoveAt(lapTimeSimulationListBox.Items.IndexOf(lapTimeSimulationListBox.SelectedItem));
                lapTimeSimulationListBox.Items.RemoveAt(lapTimeSimulationListBox.Items.IndexOf(lapTimeSimulationListBox.SelectedItem));
            }
        }

        /// <summary>
        /// Loads the properties of the lap time simulations listbox's lap time simulation and displays it in the UI fields.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _LapTimeSimulationListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // Checks if there's a listbox item selected
            if (lapTimeSimulationListBox.SelectedItems.Count == 1)
            {
                // Gets the selected object
                Simulation.LapTimeSimulation lapTimeSimulation = lapTimeSimulationListBox.SelectedItem as Simulation.LapTimeSimulation;
                // Writes the properties in the UI
                lapTimeSimulationIDTextBox.Text = lapTimeSimulation.ID;
                lapTimeSimulationDescriptionTextBox.Text = lapTimeSimulation.Description;
                if (lapTimeSimulation.IsFirstLap) lapTimeSimulationModeComboBox.Text = "First Lap";
                else lapTimeSimulationModeComboBox.Text = "Normal Lap";
                lapTimeSimulationPathComboBox.Text = lapTimeSimulation.Path.ToString();
                lapTimeSimulationGGVDiagramPerSectorListBox.Items.Clear();
                foreach (Simulation.LapTimeSimulationSectorSetup sectorSetup in lapTimeSimulation.GGVDiagramsPerSector)
                {
                    lapTimeSimulationGGVDiagramPerSectorListBox.Items.Add(sectorSetup);
                }
            }
        }

        /// <summary>
        /// Creates a sector setup object and adds it to the sectors specific GGV Diagrams listbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _LapTimeSimulationAddGGVDiagramPerSectorToListBox_Click(object sender, RoutedEventArgs e)
        {
            if (lapTimeSimulationNewSectorIndexComboBox.SelectedItem != null &&
                lapTimeSimulationNewSectorGGVDiagramComboBox.SelectedItem != null)
            {
                int sectorIndex = lapTimeSimulationNewSectorIndexComboBox.Items.IndexOf(lapTimeSimulationNewSectorIndexComboBox.SelectedItem) + 1;
                Simulation.GGVDiagram ggvDiagram = lapTimeSimulationNewSectorGGVDiagramComboBox.SelectedItem as Simulation.GGVDiagram;
                Simulation.LapTimeSimulationSectorSetup sectorSetup = new Simulation.LapTimeSimulationSectorSetup(sectorIndex, ggvDiagram);
                lapTimeSimulationGGVDiagramPerSectorListBox.Items.Add(sectorSetup);
            }
            else System.Windows.MessageBox.Show(
               "Could not create Sector's Specific GGV Diagram. \n " +
               "    A sector index must be selected. \n" +
               "    A GGV Diagram must be selected.",
               "Error",
               MessageBoxButton.OK,
               MessageBoxImage.Error);
        }

        /// <summary>
        /// Deletes a sector setup from the sectors specific GGV Diagrams listbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _LapTimeSimulationDeleteGGVDiagramPerSectorOfListBox_Click(object sender, RoutedEventArgs e)
        {
            // Checks if there's a listbox item selected and then removes it
            if (lapTimeSimulationGGVDiagramPerSectorListBox.SelectedItems.Count == 1)
            {
                lapTimeSimulationGGVDiagramPerSectorListBox.Items.RemoveAt(lapTimeSimulationGGVDiagramPerSectorListBox.Items.IndexOf(lapTimeSimulationGGVDiagramPerSectorListBox.SelectedItem));
            }
        }

        /// <summary>
        /// Changes the items of the new sector index ComboBox to match the selected path.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _LapTimeSimulationPathSelectionComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (lapTimeSimulationPathComboBox.SelectedItem != null)
            {
                Path path = lapTimeSimulationPathComboBox.SelectedItem as Path;
                lapTimeSimulationNewSectorIndexComboBox.DataContext = path;
                lapTimeSimulationNewSectorIndexComboBox.ItemsSource = path.SectorsSet.Sectors;
            }
        }

        #endregion

        #endregion

        #region Results Analysis Methods
        #region Lap Time Simulation

        #region 2D Chart

        /// <summary>
        /// Updates the current lap time simulation analysis 2D chart when the x axis ComboBox item gets changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _LapTimeSimulationResultsAnalysis2DChartXAxisDataComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (lapTimeSimulationResultsAnalysis2DChartXAxisDataComboBox.SelectedItem != null && lapTimeSimulationResultsAnalysis2DChartYAxisDataComboBox.SelectedItem != null && lapTimeSimulationResultsAnalysis2DChartCurvesTypeDataComboBox.SelectedItem != null && lapTimeSimulationResultsAnalysisResultsListBox.SelectedItems.Count != 0)
            {
                _UpdateCurrentLapTimeSimulationAnalysis2DChart();
            }
        }

        /// <summary>
        /// Updates the current lap time simulation analysis 2D chart when the y axis ComboBox item gets changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _LapTimeSimulationResultsAnalysis2DChartYAxisDataComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (lapTimeSimulationResultsAnalysis2DChartXAxisDataComboBox.SelectedItem != null && lapTimeSimulationResultsAnalysis2DChartYAxisDataComboBox.SelectedItem != null && lapTimeSimulationResultsAnalysis2DChartCurvesTypeDataComboBox.SelectedItem != null && lapTimeSimulationResultsAnalysisResultsListBox.SelectedItems.Count != 0)
            {
                _UpdateCurrentLapTimeSimulationAnalysis2DChart();
            }
        }

        /// <summary>
        /// Updates the current lap time simulation analysis 2D chart when the curve type ComboBox item gets changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _LapTimeSimulationResultsAnalysis2DChartCurvesTypeDataComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (lapTimeSimulationResultsAnalysis2DChartXAxisDataComboBox.SelectedItem != null && lapTimeSimulationResultsAnalysis2DChartYAxisDataComboBox.SelectedItem != null && lapTimeSimulationResultsAnalysis2DChartCurvesTypeDataComboBox.SelectedItem != null && lapTimeSimulationResultsAnalysisResultsListBox.SelectedItems.Count != 0)
            {
                _UpdateCurrentLapTimeSimulationAnalysis2DChart();
            }
        }
        /// <summary>
        /// Updates the current lap time simulation analysis 2D chart when the simulation results checklistbox item gets changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _LapTimeSimulationResultsAnalysisResultsListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (lapTimeSimulationResultsAnalysis2DChartXAxisDataComboBox.SelectedItem != null && lapTimeSimulationResultsAnalysis2DChartYAxisDataComboBox.SelectedItem != null && lapTimeSimulationResultsAnalysis2DChartCurvesTypeDataComboBox.SelectedItem != null && lapTimeSimulationResultsAnalysisResultsListBox.SelectedItems.Count != 0)
            {
                _UpdateCurrentLapTimeSimulationAnalysis2DChart();
            }
        }
        /// <summary>
        /// Updates the current lap time simulation analysis 2D chart
        /// </summary>
        private void _UpdateCurrentLapTimeSimulationAnalysis2DChart()
        {
            if (lapTimeSimulationResultsAnalysis2DChartXAxisDataComboBox.SelectedValue == null || lapTimeSimulationResultsAnalysis2DChartYAxisDataComboBox.SelectedValue == null || lapTimeSimulationResultsAnalysis2DChartCurvesTypeDataComboBox.SelectedValue == null)
            {
                return;
            }
            // Gets the chart's parameters
            string xDataType = lapTimeSimulationResultsAnalysis2DChartXAxisDataComboBox.SelectedValue.ToString();
            string yDataType = lapTimeSimulationResultsAnalysis2DChartYAxisDataComboBox.SelectedValue.ToString();
            string lineType = lapTimeSimulationResultsAnalysis2DChartCurvesTypeDataComboBox.SelectedValue.ToString();
            // Initializes the new chart parameters object
            UIClasses.ResultsAnalysis.LapTimeSimulation2DChartParameters chartParameters = new UIClasses.ResultsAnalysis.LapTimeSimulation2DChartParameters(xDataType, yDataType, lineType);
            SfChart chart = (new UIClasses.ResultsAnalysis.LapTimeSimulation2DChartParameters()).InitializeChart();
            // Sweeps the lap time simulation results and adds the data to the chart.
            foreach (Results.LapTimeSimulationResults results in lapTimeSimulationResultsAnalysisResultsListBox.SelectedItems)
            {
                // Initializes the chart's data series
                Results.LapTimeSimulationResultsViewModel resultsViewModel = new Results.LapTimeSimulationResultsViewModel(results);
                chart = chartParameters.AddDataToChart(chart, results.ID, resultsViewModel);
            }
            // Adds the chart to the current chart tab
            resultsAnalysis2DChartGrid.Children.Clear();
            resultsAnalysis2DChartGrid.Children.Add(chart);
        }

        #endregion

        #region Track Map
        /// <summary>
        /// Updates the track map when the results set is changed in the combobox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _LapTimeSimulationResultsAnalysisResultsComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            _UpdateCurrentLapTimeSimulationAnalysisTrackMap();
        }
        /// <summary>
        /// Updates the track map when the results type is changed in the combobox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _ResultsAnalysisTrackMapDataComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            _UpdateCurrentLapTimeSimulationAnalysisTrackMap();
        }
        /// <summary>
        /// Updates th track map.
        /// </summary>
        private void _UpdateCurrentLapTimeSimulationAnalysisTrackMap()
        {
            if (lapTimeSimulationResultsAnalysisResultsComboBox.SelectedItem == null || resultsAnalysisTrackMapDataComboBox.SelectedValue == null)
            {
                return;
            }
            // Gets the track map's parameters
            string resultType = resultsAnalysisTrackMapDataComboBox.SelectedValue.ToString();
            // Initializes the new track map parameters object
            UIClasses.ResultsAnalysis.LapTimeSimulationTrackMapParameters trackMapParameters = new UIClasses.ResultsAnalysis.LapTimeSimulationTrackMapParameters(resultType);
            // Gets the results view model
            Results.LapTimeSimulationResults lapTimeSimulationResults = lapTimeSimulationResultsAnalysisResultsComboBox.SelectedItem as Results.LapTimeSimulationResults;
            Results.LapTimeSimulationResultsViewModel lapTimeSimulationResultsViewModel = new Results.LapTimeSimulationResultsViewModel(lapTimeSimulationResults);
            // Creates the track map
            double size = mainWorkEnvironment.ActualHeight;
            SfSurfaceChart trackMap = trackMapParameters.AddDataToTrackMap(lapTimeSimulationResultsViewModel, size);
            // Updates the track map's grid
            resultsAnalysisTrackMapGrid.Children.Clear();
            resultsAnalysisTrackMapGrid.Children.Add(trackMap);
        }
        #endregion

        #endregion

        #endregion


    }
}
