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

namespace InternshipTest
{
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
            // Sets the application theme
            CurrentVisualStyle = "Blend";
            // this.Loaded += OnLoaded;

            _PopulateFields();
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
        private void _PopulateFields()
        {
            // Inertia
            oneWheelInertiaIDTextBox.Text = "inertia1";
            oneWheelTotalMassTextBox.Text = "300";
            oneWheelUnsprungMassTextbox.Text = "50";
            oneWheelRotPartsMITextbox.Text = "5";
            oneWheelGravityAccelTextbox.Text = "9.81";

            // Suspension
            oneWheelSuspensionIDTextBox.Text = "susp1";
            oneWheelHeaveStiffnessTextBox.Text = "100";
            oneWheelRideHeightTextbox.Text = "50";

            // Brakes
            oneWheelBrakesIDTextBox.Text = "brk1";
            oneWheelBrakesMaxTorqueTextBox.Text = "2000";

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
            tireModelDisplayParameterSetsCheckListBox.Items.Add(new Vehicle.TireModelMF52Point(0, -.1, 1000, 0, 40));
            tireModelDisplayParameterSetsCheckListBox.Items.Add(new Vehicle.TireModelMF52Point(0, .1, 1000, 0, 40));
            tireModelDisplayParameterSetsCheckListBox.Items.Add(new Vehicle.TireModelMF52Point(-6, 0, 1000, 0, 40));
            tireModelDisplayParameterSetsCheckListBox.Items.Add(new Vehicle.TireModelMF52Point(6, 0, 1000, 0, 40));
            tireModelDisplayDataAmountOfPointsTextBox.Text = "100";

            // Gear Ratios
            oneWheelGearRatiosListBox.Items.Add(new Vehicle.GearRatio(2.75));
            oneWheelGearRatiosListBox.Items.Add(new Vehicle.GearRatio(1.938));
            oneWheelGearRatiosListBox.Items.Add(new Vehicle.GearRatio(1.556));
            oneWheelGearRatiosListBox.Items.Add(new Vehicle.GearRatio(1.348));
            oneWheelGearRatiosListBox.Items.Add(new Vehicle.GearRatio(1.208));
            oneWheelGearRatiosListBox.Items.Add(new Vehicle.GearRatio(1.095));
            oneWheelGearRatiosSetIDTextBox.Text = "gearRatios1";

            // Transmission
            oneWheelTransmissionIDTextBox.Text = "trans1";
            oneWheelTransmissionTypeComboBox.Text = "2WD";
            oneWheelPrimaryRatioTextBox.Text = "2.111";
            oneWheelFinalRatioTextBox.Text = "3.7143";
            oneWheelGearShiftTimeTextBox.Text = "0.2";
            oneWheelTransmissionEfficiencyTextBox.Text = "87.5";

            // Aerodynamic Map
            oneWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.AerodynamicMapPoint(40, 30, 1, -2));
            oneWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.AerodynamicMapPoint(40, 40, 1.1, -1.9));
            oneWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.AerodynamicMapPoint(40, 50, 1.2, -1.8));
            oneWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.AerodynamicMapPoint(70, 30, 1.1, -2.1));
            oneWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.AerodynamicMapPoint(70, 40, 1.2, -2.2));
            oneWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.AerodynamicMapPoint(70, 50, 1.3, -2.3));
            oneWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.AerodynamicMapPoint(100, 30, 1.1, -2.3));
            oneWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.AerodynamicMapPoint(100, 40, 1, -2.2));
            oneWheelAerodynamicMapPointsListBox.Items.Add(new Vehicle.AerodynamicMapPoint(100, 50, 0.9, -2.1));
            oneWheelAerodynamicMapIDTextBox.Text = "aeroMap1";

            // Aerodynamics
            oneWheelAerodynamicsIDTextBox.Text = "aero1";
            oneWheelFrontalAreaTextBox.Text = "1.2";
            oneWheelAirDensityTextBox.Text = "1.3";

            // Engine Curves
            oneWheelEngineCurvesPointsListBox.Items.Add(new Vehicle.EngineCurvesPoint(3000, 55.77, 3.0, 390.45));
            oneWheelEngineCurvesPointsListBox.Items.Add(new Vehicle.EngineCurvesPoint(3500, 52.50, 3.5, 381.50));
            oneWheelEngineCurvesPointsListBox.Items.Add(new Vehicle.EngineCurvesPoint(4000, 54.71, 4.0, 381.80));
            oneWheelEngineCurvesPointsListBox.Items.Add(new Vehicle.EngineCurvesPoint(4500, 57.77, 4.5, 379.02));
            oneWheelEngineCurvesPointsListBox.Items.Add(new Vehicle.EngineCurvesPoint(5000, 70.67, 5.0, 367.21));
            oneWheelEngineCurvesPointsListBox.Items.Add(new Vehicle.EngineCurvesPoint(5500, 79.63, 5.5, 366.58));
            oneWheelEngineCurvesPointsListBox.Items.Add(new Vehicle.EngineCurvesPoint(6000, 83.18, 6.0, 361.44));
            oneWheelEngineCurvesPointsListBox.Items.Add(new Vehicle.EngineCurvesPoint(6500, 83.82, 6.5, 362.35));
            oneWheelEngineCurvesPointsListBox.Items.Add(new Vehicle.EngineCurvesPoint(7000, 83.86, 7.0, 364.99));
            oneWheelEngineCurvesPointsListBox.Items.Add(new Vehicle.EngineCurvesPoint(7500, 84.50, 7.5, 369.16));
            oneWheelEngineCurvesPointsListBox.Items.Add(new Vehicle.EngineCurvesPoint(8000, 82.56, 8.0, 372.97));
            oneWheelEngineCurvesPointsListBox.Items.Add(new Vehicle.EngineCurvesPoint(8500, 77.88, 8.5, 375.73));
            oneWheelEngineCurvesPointsListBox.Items.Add(new Vehicle.EngineCurvesPoint(9000, 72.16, 9.0, 378.86));
            oneWheelEngineCurvesPointsListBox.Items.Add(new Vehicle.EngineCurvesPoint(9500, 66.65, 9.5, 383.92));
            oneWheelEngineCurvesPointsListBox.Items.Add(new Vehicle.EngineCurvesPoint(10000, 61.16, 10.0, 391.00));
            oneWheelEngineCurvesPointsListBox.Items.Add(new Vehicle.EngineCurvesPoint(10500, 55.94, 10.5, 398.92));
            oneWheelEngineCurvesPointsListBox.Items.Add(new Vehicle.EngineCurvesPoint(11000, 50.23, 11.0, 407.68));
            oneWheelEngineCurvesIDTextBox.Text = "engineCurves1";

            // Engine
            oneWheelEngineIDTextBox.Text = "engine1";
            oneWheelMaxThrottleTextBox.Text = "100";
            oneWheelFuelDensityTextBox.Text = "870";

            // Car and setup fields
            oneWheelCarIDTextBox.Text = "car1";
            oneWheelSetupIDTextBox.Text = "setup1";

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
            tabularPathIDTextBox.Text = "testPath";
            tabularPathResolutionTextBox.Text = "100";

            // GGV Diagram
            ggvDiagramAmountOfPointsPerSpeedTextBox.Text = "40";
            ggvDiagramAmountOfSpeedsTextBox.Text = "20";
            ggvDiagramLowestSpeedTextBox.Text = "10";
            ggvDiagramHighestSpeedTextBox.Text = "120";
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
            _CollapseOptionsButtons();
            projectButtonOptionsGrid.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Displays the vehicle options buttons (models) and hides all of the other Mainmenu buttons options.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _VehicleInputEnvironmentButton_Click(object sender, RoutedEventArgs e)
        {
            _CollapseOptionsButtons();
            vehicleButtonOptionsGrid.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Displays the path options buttons (input methods) and hides all of the other Mainmenu buttons options.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _PathInputEnvironmentButton_Click(object sender, RoutedEventArgs e)
        {
            _CollapseOptionsButtons();
            pathButtonOptionsGrid.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Displays the simulation options buttons (types) and hides all of the other Mainmenu buttons options.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _SimulationInputEnvironmentButton_Click(object sender, RoutedEventArgs e)
        {
            _CollapseOptionsButtons();
            simulationButtonOptionsGrid.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Displays the results analysis options buttons (types) and hides all of the other Mainmenu buttons options.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _ResultsAnalysisEnvironmentButton_Click(object sender, RoutedEventArgs e)
        {
            _CollapseOptionsButtons();
            resultsButtonOptionsGrid.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Hides all the main navigation controls options and displays the help file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _HelpEnvironmentButton_Click(object sender, RoutedEventArgs e)
        {
            _CollapseOptionsButtons();
        }

        /// <summary>
        /// Changes the application theme according o the combobox selection.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _ThemeSelectionCombobox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ComboBoxItem themeItem = themeSelectionCombobox.SelectedItem as ComboBoxItem;
            CurrentVisualStyle = themeItem.Content.ToString();
        }

        /// <summary>
        /// Collapses the UI's options menu.
        /// </summary>
        private void _CollapseOptionsButtons()
        {
            projectButtonOptionsGrid.Visibility = Visibility.Collapsed;
            vehicleButtonOptionsGrid.Visibility = Visibility.Collapsed;
            pathButtonOptionsGrid.Visibility = Visibility.Collapsed;
            simulationButtonOptionsGrid.Visibility = Visibility.Collapsed;
            resultsButtonOptionsGrid.Visibility = Visibility.Collapsed;
        }

        #endregion
        #region Main Controls Options Methods

        /// <summary>
        /// Collapses all of the work environments of the main work environment 
        /// </summary>
        /// <param name="dockingManager"></param>
        private void _CollapseMainDockingManagerContent()
        {
            tireInputDockingManager.Visibility = Visibility.Collapsed;
            oneWheelVehicleInputDockingManager.Visibility = Visibility.Collapsed;
            twoWheelVehicleInputDockingManager.Visibility = Visibility.Collapsed;
            fourWheelVehicleInputDockingManager.Visibility = Visibility.Collapsed;
            tabularPathInputDockingManager.Visibility = Visibility.Collapsed;
            drawPathInputDockingManager.Visibility = Visibility.Collapsed;
            optimizePathInputDockingManager.Visibility = Visibility.Collapsed;
            ggvDiagramDockingManager.Visibility = Visibility.Collapsed;
            lapTimeSimulationDockingManager.Visibility = Visibility.Collapsed;
            ggvDiagramResultsAnalysisGrid.Visibility = Visibility.Collapsed;
            lapTimeSimulationResultsAnalysisGrid.Visibility = Visibility.Collapsed;
            _CollapseOptionsButtons();
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
            _CollapseOptionsButtons();
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
                // Initializes the project object's lists
                // One Wheel Model Cars
                List<Vehicle.Car> oneWheelCars = new List<Vehicle.Car>();
                List<Vehicle.Inertia> oneWheelInertias = new List<Vehicle.Inertia>();
                List<Vehicle.Suspension> oneWheelSuspensions = new List<Vehicle.Suspension>();
                List<Vehicle.Brakes> oneWheelBrakes = new List<Vehicle.Brakes>();
                List<Vehicle.Tire> oneWheelTires = new List<Vehicle.Tire>();
                List<Vehicle.Transmission> oneWheelTransmissions = new List<Vehicle.Transmission>();
                List<Vehicle.Aerodynamics> oneWheelAerodynamics = new List<Vehicle.Aerodynamics>();
                List<Vehicle.Engine> oneWheelEngines = new List<Vehicle.Engine>();
                // Two Wheel Model Cars
                // Four Wheel Model Cars
                // Tabular Paths
                List<Path> tabularPaths = new List<Path>();
                // Drawn Paths
                // Optimization Paths
                // GGV Diagrams
                List<Simulation.GGVDiagram> ggvDiagrams = new List<Simulation.GGVDiagram>();
                // Lap Time Simulations
                List<Simulation.LapTimeSimulation> lapTimeSimulations = new List<Simulation.LapTimeSimulation>();

                // Fills the lists with the UI elements
                // One Wheel Model Cars
                foreach (Vehicle.Car car in oneWheelCarAndSetupListBox.Items)
                    oneWheelCars.Add(car);
                foreach (Vehicle.Inertia inertia in oneWheelInertiaListBox.Items)
                    oneWheelInertias.Add(inertia);
                foreach (Vehicle.Suspension suspension in oneWheelSuspensionListBox.Items)
                    oneWheelSuspensions.Add(suspension);
                foreach (Vehicle.Brakes brakes in oneWheelBrakesListBox.Items)
                    oneWheelBrakes.Add(brakes);
                foreach (Vehicle.Tire tires in tireListBox.Items)
                    oneWheelTires.Add(tires);
                foreach (Vehicle.Transmission transmission in oneWheelTransmissionListBox.Items)
                    oneWheelTransmissions.Add(transmission);
                foreach (Vehicle.Aerodynamics aerodynamics in oneWheelAerodynamicsListBox.Items)
                    oneWheelAerodynamics.Add(aerodynamics);
                foreach (Vehicle.Engine engine in oneWheelEngineListBox.Items)
                    oneWheelEngines.Add(engine);
                // Two Wheel Model Cars
                // Four Wheel Model Cars
                // Tabular Paths
                foreach (Path path in tabularPathsListBox.Items)
                    tabularPaths.Add(path);
                // Drawn Paths
                // Optimization Paths
                // GGV Diagrams
                foreach (Simulation.GGVDiagram ggvDiagram in simulationGGVDiagramListBox.Items)
                    ggvDiagrams.Add(ggvDiagram);
                // Lap Time Simulations
                foreach (Simulation.LapTimeSimulation lapTimeSimulation in lapTimeSimulationListBox.Items)
                    lapTimeSimulations.Add(lapTimeSimulation);

                // Creates the project object
                Project project = new Project(oneWheelCars, oneWheelInertias, oneWheelSuspensions, oneWheelBrakes, oneWheelTires,
                    oneWheelTransmissions, oneWheelAerodynamics, oneWheelEngines, tabularPaths, ggvDiagrams, lapTimeSimulations);
                // Saves the project to the file
                project.Save(saveFileDialog.FileName);
                _CollapseOptionsButtons();
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
            // One Wheel Model Cars
            foreach (Vehicle.Car car in project.OneWheelCars)
                oneWheelCarAndSetupListBox.Items.Add(car);
            foreach (Vehicle.Inertia inertia in project.OneWheelInertias)
                oneWheelInertiaListBox.Items.Add(inertia);
            foreach (Vehicle.Suspension suspension in project.OneWheelSuspensions)
                oneWheelSuspensionListBox.Items.Add(suspension);
            foreach (Vehicle.Brakes brakes in project.OneWheelBrakes)
                oneWheelBrakesListBox.Items.Add(brakes);
            foreach (Vehicle.Tire tires in project.OneWheelTires)
                tireListBox.Items.Add(tires);
            foreach (Vehicle.Transmission transmission in project.OneWheelTransmissions)
                oneWheelTransmissionListBox.Items.Add(transmission);
            foreach (Vehicle.Aerodynamics aerodynamics in project.OneWheelAerodynamics)
                oneWheelAerodynamicsListBox.Items.Add(aerodynamics);
            foreach (Vehicle.Engine engine in project.OneWheelEngines)
                oneWheelEngineListBox.Items.Add(engine);
            // Two Wheel Model Cars
            // Four Wheel Model Cars
            // Tabular Paths
            foreach (Path path in project.TabularPaths)
                tabularPathsListBox.Items.Add(path);
            // Drawn Paths
            // Optimization Paths
            // GGV Diagrams
            foreach (Simulation.GGVDiagram ggvDiagram in project.GGVDiagrams)
                simulationGGVDiagramListBox.Items.Add(ggvDiagram);
            // Lap Time Simulations
            foreach (Simulation.LapTimeSimulation lapTimeSimulation in project.LapTimeSimulations)
                lapTimeSimulationListBox.Items.Add(lapTimeSimulation);
            _CollapseOptionsButtons();
        }

        /// <summary>
        /// Clears all the lists of the UI.
        /// </summary>
        private void _ClearUILists()
        {
            // Clears the UI lists
            // One Wheel Model Cars
            oneWheelCarAndSetupListBox.Items.Clear();
            oneWheelInertiaListBox.Items.Clear();
            oneWheelSuspensionListBox.Items.Clear();
            oneWheelBrakesListBox.Items.Clear();
            tireListBox.Items.Clear();
            oneWheelTransmissionListBox.Items.Clear();
            oneWheelAerodynamicsListBox.Items.Clear();
            oneWheelEngineListBox.Items.Clear();
            // Two Wheel Model Cars
            // Four Wheel Model Cars
            // Tabular Paths
            tabularPathsListBox.Items.Clear();
            // Drawn Paths
            // Optimization Paths
            // GGV Diagrams
            simulationGGVDiagramListBox.Items.Clear();
            // Lap Time Simulations
            lapTimeSimulationListBox.Items.Clear();
        }

        #endregion
        #region Vehicle Button Options Methods
        /// <summary>
        /// Changes the work environment to the tire model input environment.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _TireButton_Click(object sender, RoutedEventArgs e)
        {
            _CollapseMainDockingManagerContent();
            tireInputDockingManager.Visibility = Visibility.Visible;
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
        }
        #endregion
        #region Results Analysis Button Methods
        /// <summary>
        /// Changes the work environment to the GGV Diagram results analysis environment.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _GGVDiagramResultsAnalysisButton_Click(object sender, RoutedEventArgs e)
        {
            _CollapseMainDockingManagerContent();
            ggvDiagramResultsAnalysisGrid.Visibility = Visibility.Visible;
        }
        /// <summary>
        /// Changes the work environment to the lap time simulation results analysis environment.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _LapTimeSimulationResultsAnalysisButton_Click(object sender, RoutedEventArgs e)
        {
            _CollapseMainDockingManagerContent();
            lapTimeSimulationResultsAnalysisGrid.Visibility = Visibility.Visible;
        }
        #endregion
        #endregion
        #endregion

        #region Vehicle Input Methods

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
                // Adds the object to the listbox and the combobox
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
                double alphaMin = double.Parse(tireModelMinSlipAngleTextBox.Text);
                double alphaMax = double.Parse(tireModelMaxSlipAngleTextBox.Text);
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
                // Adds the object to the listbox and the combobox
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
                tireModelMinSlipAngleTextBox.Text = tireModel.AlphaMin.ToString("F3");
                tireModelMaxSlipAngleTextBox.Text = tireModel.AlphaMax.ToString("F3");
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
            if (tireModelListBox.SelectedItems.Count == 1)
            {
                tireModelListBox.Items.RemoveAt(tireModelListBox.Items.IndexOf(tireModelListBox.SelectedItem));
                tireModelDisplayCheckListBox.Items.RemoveAt(tireModelDisplayCheckListBox.Items.IndexOf(tireModelDisplayCheckListBox.SelectedItem));
            }
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

        private void _UpdateCurrentTireModelDisplayChart()
        {
            // Checks if all of the information needed to generate the chart is ok.
            if (tireModelDisplayCheckListBox.SelectedItems.Count==0 || tireModelDisplayParameterSetsCheckListBox.SelectedItems.Count==0 || tireModelDisplayChartYAxisDataComboBox.SelectedValue == null || tireModelDisplayChartXAxisDataComboBox.SelectedValue == null || !double.TryParse(tireModelDisplayXAxisRangeMinTextBox.Text, out double rangeMin) || !double.TryParse(tireModelDisplayXAxisRangeMaxTextBox.Text, out double rangeMax) || rangeMin >= rangeMax || !int.TryParse(tireModelDisplayDataAmountOfPointsTextBox.Text, out int amountOfPoints) || amountOfPoints <= 0)
            {
                TabItemExt currentTab = tireModelDisplayChartTabControl.SelectedItem as TabItemExt;
                if (currentTab!=null) currentTab.Content = new Grid();
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
                    MaxWidth = 200
                },
                PrimaryAxis = new NumericalAxis(),
                SecondaryAxis = new NumericalAxis()
            };
            // Adds zoom/panning behaviour to the chart
            ChartZoomPanBehavior zoomingAndPanning = new ChartZoomPanBehavior()
            {
                EnableZoomingToolBar = true,
                EnableMouseWheelZooming = true,
                EnablePanning = true,
                ZoomRelativeToCursor = true,
            };
            chart.Behaviors.Add(zoomingAndPanning);
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
                    string seriesLabel = "TM: " + tireModel.ID + " - LS: " + tireModelMF52ParametersSet.LongitudinalSlip.ToString("F2") + " - SA: " + tireModelMF52ParametersSet.SlipAngle.ToString("F2") + " - VL: " + tireModelMF52ParametersSet.VerticalLoad.ToString("F0") + " - IA: " + tireModelMF52ParametersSet.InclinationAngle.ToString("F2") + " - S:" + tireModelMF52ParametersSet.Speed.ToString("F1");
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

        #endregion
        #endregion

        #endregion

        #region One Wheel Car Input Methods

        #region CarAndSetup

        /// <summary>
        /// Creates a one wheel model car/setup object and adds it to the one wheel mmodel car/setup listbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _OneWheelAddCarAndSetupToListBox_Click(object sender, RoutedEventArgs e)
        {
            if (oneWheelCarIDTextBox.Text != "" &&
                oneWheelSetupIDTextBox.Text != "" &&
                oneWheelInertiaCombobox.Text != "" &&
                oneWheelEngineCombobox.Text != "" &&
                oneWheelTransmissionCombobox.Text != "" &&
                oneWheelAerodynamicsCombobox.Text != "" &&
                oneWheelSuspensionCombobox.Text != "" &&
                oneWheelBrakesCombobox.Text != "")
            {
                // Gets the object's properties values
                string carID = oneWheelCarIDTextBox.Text;
                string setupID = oneWheelSetupIDTextBox.Text;
                string description = oneWheelCarAndSetupDescriptionTextBox.Text;
                Vehicle.Inertia inertia = oneWheelInertiaCombobox.SelectedItem as Vehicle.Inertia;
                Vehicle.Tire tire = oneWheelTireCombobox.SelectedItem as Vehicle.Tire;
                Vehicle.Engine engine = oneWheelEngineCombobox.SelectedItem as Vehicle.Engine;
                Vehicle.Transmission transmission = oneWheelTransmissionCombobox.SelectedItem as Vehicle.Transmission;
                Vehicle.Aerodynamics aerodynamics = oneWheelAerodynamicsCombobox.SelectedItem as Vehicle.Aerodynamics;
                Vehicle.Suspension suspension = oneWheelSuspensionCombobox.SelectedItem as Vehicle.Suspension;
                Vehicle.Brakes brakes = oneWheelBrakesCombobox.SelectedItem as Vehicle.Brakes;
                // Initializes a new object
                Vehicle.Car car = new Vehicle.Car(carID, setupID, description, inertia, tire, engine, transmission, aerodynamics, suspension, brakes);
                // Gets additional parameters
                car.GetEquivalentHeaveStiffness();
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
                Vehicle.Car car = oneWheelCarAndSetupListBox.SelectedItem as Vehicle.Car;
                // Writes the properties in the UI
                oneWheelCarIDTextBox.Text = car.ID;
                oneWheelSetupIDTextBox.Text = car.SetupID;
                oneWheelCarAndSetupDescriptionTextBox.Text = car.Description;
                oneWheelInertiaCombobox.Text = car.Inertia.ID;
                oneWheelTireCombobox.Text = car.Tire.ID;
                oneWheelEngineCombobox.Text = car.Engine.ID;
                oneWheelTransmissionCombobox.Text = car.Transmission.ID;
                oneWheelAerodynamicsCombobox.Text = car.Aerodynamics.ID;
                oneWheelSuspensionCombobox.Text = car.Suspension.ID;
                oneWheelBrakesCombobox.Text = car.Brakes.ID;
            }
        }

        #endregion

        #region Inertia

        /// <summary>
        /// Creates a one wheel model inertia object and adds it to the one wheel model inertia listbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _OneWheelAddInertiaToListBox_Click(object sender, RoutedEventArgs e)
        {
            if (oneWheelInertiaIDTextBox.Text != "" &&
                double.Parse(oneWheelTotalMassTextBox.Text) != 0 &&
                double.Parse(oneWheelGravityAccelTextbox.Text) != 0)
            {
                // Gets the object's properties values
                string inertiaID = oneWheelInertiaIDTextBox.Text;
                string description = oneWheelInertiaDescriptionTextBox.Text;
                double totalMass = double.Parse(oneWheelTotalMassTextBox.Text);
                double unsprungMass = double.Parse(oneWheelUnsprungMassTextbox.Text);
                double rotPartsMI = double.Parse(oneWheelRotPartsMITextbox.Text);
                double gravity = double.Parse(oneWheelGravityAccelTextbox.Text);
                // Initializes a new object
                Vehicle.Inertia inertia = new Vehicle.Inertia(inertiaID, description, totalMass, unsprungMass, rotPartsMI, gravity);
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
        private void _OneWheelDeleteInertiaOfListBox_Click(object sender, RoutedEventArgs e)
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
                Vehicle.Inertia inertia = oneWheelInertiaListBox.SelectedItem as Vehicle.Inertia;
                // Writes the properties in the UI
                oneWheelInertiaIDTextBox.Text = inertia.ID;
                oneWheelInertiaDescriptionTextBox.Text = inertia.Description;
                oneWheelTotalMassTextBox.Text = inertia.TotalMass.ToString("F3");
                oneWheelUnsprungMassTextbox.Text = inertia.UnsprungMass.ToString("F3");
                oneWheelRotPartsMITextbox.Text = inertia.RotPartsMI.ToString("F3");
                oneWheelGravityAccelTextbox.Text = inertia.Gravity.ToString("F3");
            }
        }

        #endregion

        #region Suspension

        /// <summary>
        /// Creates a one wheel model suspension object and adds it to the one wheel model suspension listbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _OneWheelAddSuspensionToListBox_Click(object sender, RoutedEventArgs e)
        {
            if (oneWheelSuspensionIDTextBox.Text != "" &&
                double.Parse(oneWheelHeaveStiffnessTextBox.Text) != 0)
            {
                // Gets the object's properties values
                string suspensionID = oneWheelSuspensionIDTextBox.Text;
                string description = oneWheelSuspensionDescriptionTextBox.Text;
                double heaveStiffness = double.Parse(oneWheelHeaveStiffnessTextBox.Text) * 1000;
                double rideHeight = double.Parse(oneWheelRideHeightTextbox.Text) / 1000;
                // Initializes a new object
                Vehicle.Suspension suspension = new Vehicle.Suspension(suspensionID, description, heaveStiffness, rideHeight);
                // Adds the object to the listbox
                oneWheelSuspensionListBox.Items.Add(suspension);
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
        /// Deletes a one wheel model suspension from the one wheel model suspension listbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _OneWheelDeleteSuspensionOfListBox_Click(object sender, RoutedEventArgs e)
        {
            // Checks if there's a listbox item selected and then removes it
            if (oneWheelInertiaListBox.SelectedItems.Count == 1)
            {
                oneWheelSuspensionListBox.Items.RemoveAt(oneWheelSuspensionListBox.Items.IndexOf(oneWheelSuspensionListBox.SelectedItem));
            }
        }

        /// <summary>
        /// Loads the properties of a listbox's one wheel model suspension and displays it in the UI fields.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _OneWheelSuspensionListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (oneWheelInertiaListBox.SelectedItems.Count == 1)
            {
                // Gets the selected object
                Vehicle.Suspension suspension = oneWheelSuspensionListBox.SelectedItem as Vehicle.Suspension;
                // Writes the properties in the UI
                oneWheelSuspensionIDTextBox.Text = suspension.ID;
                oneWheelSuspensionDescriptionTextBox.Text = suspension.Description;
                oneWheelHeaveStiffnessTextBox.Text = (suspension.HeaveStiffness / 1000).ToString("F3");
                oneWheelRideHeightTextbox.Text = (suspension.RideHeight * 1000).ToString("F3");
            }
        }

        #endregion

        #region Brakes

        /// <summary>
        /// Creates a one wheel model brakes object and adds it to the one wheel model brakes listbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _OneWheelAddBrakesToListBox_Click(object sender, RoutedEventArgs e)
        {
            if (oneWheelBrakesIDTextBox.Text != "" &&
                double.Parse(oneWheelBrakesMaxTorqueTextBox.Text) != 0)
            {
                // Gets the object's properties values
                string brakesID = oneWheelBrakesIDTextBox.Text;
                string description = oneWheelBrakesDescriptionTextBox.Text;
                double maxTorque = double.Parse(oneWheelBrakesMaxTorqueTextBox.Text);
                // Initializes a new object
                Vehicle.Brakes brakes = new Vehicle.Brakes(brakesID, description, maxTorque);
                // Adds the object to the listbox and the combobox
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
        private void _OneWheelDeleteBrakesOfListBox_Click(object sender, RoutedEventArgs e)
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
                Vehicle.Brakes brakes = oneWheelBrakesListBox.SelectedItem as Vehicle.Brakes;
                // Writes the properties in the UI
                oneWheelBrakesIDTextBox.Text = brakes.ID;
                oneWheelBrakesDescriptionTextBox.Text = brakes.Description;
                oneWheelBrakesMaxTorqueTextBox.Text = brakes.MaxTorque.ToString("F3");
            }
        }

        #endregion

        #region Transmission

        /// <summary>
        /// Creates a transmission object and adds it to the one wheel model transmissions listbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _OneWheelAddTransmissionToListBox_Click(object sender, RoutedEventArgs e)
        {
            if (oneWheelTransmissionIDTextBox.Text != "" &&
                oneWheelTransmissionGearRatiosSetComboBox.SelectedItem != null)
            {
                // Gets the object's data
                string transmissionID = oneWheelTransmissionIDTextBox.Text;
                string description = oneWheelTransmissionDescriptionTextBox.Text;
                string transmissionType = oneWheelTransmissionTypeComboBox.Text;
                int amountOfDrivenWheels = 2;
                if (transmissionType == "4WD") amountOfDrivenWheels = 4;
                Vehicle.GearRatiosSet gearRatiosSet = oneWheelTransmissionGearRatiosSetComboBox.SelectedItem as Vehicle.GearRatiosSet;
                double primaryRatio = double.Parse(oneWheelPrimaryRatioTextBox.Text);
                double finalRatio = double.Parse(oneWheelFinalRatioTextBox.Text);
                double gearShiftTime = double.Parse(oneWheelGearShiftTimeTextBox.Text);
                double efficiency = double.Parse(oneWheelTransmissionEfficiencyTextBox.Text) / 100;
                // Initializes a new object
                Vehicle.Transmission transmission = new Vehicle.Transmission(
                    transmissionID, description, amountOfDrivenWheels, primaryRatio, finalRatio, gearShiftTime, efficiency, gearRatiosSet);
                // Adds the object to the listbox and the combobox
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
        /// Deletes a transmission from the one wheel model transmissions listbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _OneWheelDeleteTransmissionOfListBox_Click(object sender, RoutedEventArgs e)
        {
            // Checks if there's a listbox item selected and then removes it
            if (oneWheelTransmissionListBox.SelectedItems.Count == 1)
            {
                oneWheelTransmissionListBox.Items.RemoveAt(oneWheelTransmissionListBox.Items.IndexOf(oneWheelTransmissionListBox.SelectedItem));
            }
        }

        /// <summary>
        /// Loads the properties of the one wheel model transmissions listbox's transmission and displays it in the UI fields.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _OneWheelTransmissionListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // Checks if there's a listbox item selected
            if (oneWheelTransmissionListBox.SelectedItems.Count == 1)
            {
                // Gets the selected object
                Vehicle.Transmission transmission = oneWheelTransmissionListBox.SelectedItem as Vehicle.Transmission;
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

        #region Gear Ratios

        /// <summary>
        /// Creates a gear ratios set object and adds it to the one wheel model gear ratios sets listbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _OneWheelAddGearRatioSetToListBox_Click(object sender, RoutedEventArgs e)
        {
            if (oneWheelGearRatiosSetIDTextBox.Text != "" &&
                oneWheelGearRatiosListBox.Items.Count != 0)
            {
                // Gets the object's properties values
                string setID = oneWheelGearRatiosSetIDTextBox.Text;
                string description = oneWheelGearRatiosSetDescriptionTextBox.Text;
                List<Vehicle.GearRatio> gearRatios = new List<Vehicle.GearRatio>();
                foreach (var gearRatioItem in oneWheelGearRatiosListBox.Items)
                {
                    Vehicle.GearRatio gearRatio = gearRatioItem as Vehicle.GearRatio;
                    gearRatios.Add(gearRatio);
                }
                // Initializes a new object
                Vehicle.GearRatiosSet gearRatiosSet = new Vehicle.GearRatiosSet(setID, description, gearRatios);
                // Adds the object to the listbox and the combobox
                oneWheelGearRatiosSetsListBox.Items.Add(gearRatiosSet);
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
        private void _OneWheelDeleteGearRatioSetOfListBox_Click(object sender, RoutedEventArgs e)
        {
            // Checks if there's a listbox item selected and then removes it
            if (oneWheelGearRatiosSetsListBox.SelectedItems.Count == 1)
            {
                oneWheelGearRatiosSetsListBox.Items.RemoveAt(oneWheelGearRatiosSetsListBox.Items.IndexOf(oneWheelGearRatiosSetsListBox.SelectedItem));
            }
        }

        /// <summary>
        /// Loads the properties of the one wheel model gear ratio sets listbox's gear ratio set and displays it in the UI fields.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _OneWheelGearRatioSetListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // Checks if there's a listbox item selected
            if (oneWheelGearRatiosSetsListBox.SelectedItems.Count == 1)
            {
                // Gets the selected object
                Vehicle.GearRatiosSet gearRatioSet = oneWheelGearRatiosSetsListBox.SelectedItem as Vehicle.GearRatiosSet;
                // Writes the properties in the UI
                oneWheelGearRatiosSetIDTextBox.Text = gearRatioSet.ID;
                oneWheelGearRatiosSetDescriptionTextBox.Text = gearRatioSet.Description;
                // Clears and writes the list in the UI
                oneWheelGearRatiosListBox.Items.Clear();
                foreach (Vehicle.GearRatio gearRatio in gearRatioSet.GearRatios)
                {
                    oneWheelGearRatiosListBox.Items.Add(gearRatio);
                }
            }
        }

        /// <summary>
        /// Creates a gear ratio object and adds it to the one wheel model gear ratios listbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _OneWheelAddGearRatioToListBox_Click(object sender, RoutedEventArgs e)
        {
            if (double.Parse(oneWheelNewGearRatioTextBox.Text) != 0)
            {
                // Gets the object's properties values
                double ratio = double.Parse(oneWheelNewGearRatioTextBox.Text);
                if (ratio < 0) ratio = -ratio;
                // Initializes a new object
                Vehicle.GearRatio gearRatio = new Vehicle.GearRatio(ratio);
                // Adds the object to the listbox and the combobox
                oneWheelGearRatiosListBox.Items.Add(gearRatio);
                // Reorders the gear ratios listbox items in descending order
                List<Vehicle.GearRatio> gearRatios = new List<Vehicle.GearRatio>();
                foreach (var gearRatioItem in oneWheelGearRatiosListBox.Items)
                {
                    Vehicle.GearRatio currentGearRatio = gearRatioItem as Vehicle.GearRatio;
                    gearRatios.Add(currentGearRatio);
                }
                gearRatios = gearRatios.OrderByDescending(currentGearRatio => currentGearRatio.Ratio).ToList();
                oneWheelGearRatiosListBox.Items.Clear();
                foreach (Vehicle.GearRatio currentGearRatio in gearRatios)
                {
                    oneWheelGearRatiosListBox.Items.Add(currentGearRatio);
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
        private void _OneWheelDeleteGearRatioOfListBox_Click(object sender, RoutedEventArgs e)
        {
            // Checks if there's a listbox item selected and then removes it
            if (oneWheelGearRatiosListBox.SelectedItems.Count == 1)
            {
                oneWheelGearRatiosListBox.Items.RemoveAt(oneWheelGearRatiosListBox.Items.IndexOf(oneWheelGearRatiosListBox.SelectedItem));
            }
        }

        #endregion

        #region Aerodynamics

        /// <summary>
        /// Creates an aerodynamics object and adds it to the one wheel model aerodynamics listbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _OneWheelAddAerodynamicsToListBox_Click(object sender, RoutedEventArgs e)
        {
            if (oneWheelAerodynamicsIDTextBox.Text != "" &&
                oneWheelAerodynamicMapComboBox.SelectedItem != null)
            {
                // Gets the object's data
                string aerodynamicID = oneWheelAerodynamicsIDTextBox.Text;
                string description = oneWheelAerodynamicsDescriptionTextBox.Text;
                Vehicle.AerodynamicMap aerodynamicMap = oneWheelAerodynamicMapComboBox.SelectedItem as Vehicle.AerodynamicMap;
                double frontalArea = double.Parse(oneWheelFrontalAreaTextBox.Text);
                double airDensity = double.Parse(oneWheelAirDensityTextBox.Text);
                // Initializes a new object
                Vehicle.Aerodynamics aerodynamics = new Vehicle.Aerodynamics(aerodynamicID, description, aerodynamicMap, frontalArea, airDensity);
                aerodynamics.GetAerodynamicMapParameters();
                // Adds the object to the listbox and the combobox
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
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _OneWheelDeleteAerodynamicsOfListBox_Click(object sender, RoutedEventArgs e)
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
                Vehicle.Aerodynamics aerodynamics = oneWheelAerodynamicsListBox.SelectedItem as Vehicle.Aerodynamics;
                // Writes the properties in the UI
                oneWheelAerodynamicsIDTextBox.Text = aerodynamics.ID;
                oneWheelAerodynamicsDescriptionTextBox.Text = aerodynamics.Description;
                oneWheelAerodynamicMapComboBox.Text = aerodynamics.AerodynamicMap.ToString();
                oneWheelFrontalAreaTextBox.Text = aerodynamics.FrontalArea.ToString("F3");
                oneWheelAirDensityTextBox.Text = aerodynamics.AirDensity.ToString("F3");
            }
        }

        #endregion

        #region Aerodynamic Map

        /// <summary>
        /// Creates an aerodynamic map object and adds it to the one wheel model aerodynamic maps listbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _OneWheelAddAerodynamicMapToListBox_Click(object sender, RoutedEventArgs e)
        {
            if (oneWheelAerodynamicMapIDTextBox.Text != "" &&
                oneWheelAerodynamicMapPointsListBox.Items.Count != 0)
            {
                // Gets the object's properties values
                string mapID = oneWheelAerodynamicMapIDTextBox.Text;
                string description = oneWheelAerodynamicMapDescriptionTextBox.Text;
                List<Vehicle.AerodynamicMapPoint> aerodynamicMapPoints = new List<Vehicle.AerodynamicMapPoint>();
                foreach (var aerodynamicMapPointItem in oneWheelAerodynamicMapPointsListBox.Items)
                {
                    Vehicle.AerodynamicMapPoint aerodynamicMapPoint = aerodynamicMapPointItem as Vehicle.AerodynamicMapPoint;
                    aerodynamicMapPoints.Add(aerodynamicMapPoint);
                }
                // Initializes a new object
                Vehicle.AerodynamicMap aerodynamicMap = new Vehicle.AerodynamicMap(mapID, description, aerodynamicMapPoints);
                // Adds the object to the listbox and the combobox
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
        private void _OneWheelDeleteAerodynamicMapOfListBox_Click(object sender, RoutedEventArgs e)
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
                Vehicle.AerodynamicMap aerodynamicMap = oneWheelAerodynamicMapsListBox.SelectedItem as Vehicle.AerodynamicMap;
                // Writes the properties in the UI
                oneWheelAerodynamicMapIDTextBox.Text = aerodynamicMap.ID;
                oneWheelAerodynamicMapDescriptionTextBox.Text = aerodynamicMap.Description;
                // Clears and writes the list in the UI
                oneWheelAerodynamicMapPointsListBox.Items.Clear();
                foreach (Vehicle.AerodynamicMapPoint aerodynamicMapPoint in aerodynamicMap.MapPoints)
                {
                    oneWheelGearRatiosListBox.Items.Add(aerodynamicMapPoint);
                }
            }
        }

        /// <summary>
        /// Creates an aerodynamic map point object and adds it to the one wheel model aerodynamic map points listbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _OneWheelAddAerodynamicMapPointToListBox_Click(object sender, RoutedEventArgs e)
        {
            // Gets the object's properties values
            double speed = double.Parse(oneWheelNewAerodynamicMapPointSpeedTextBox.Text);
            double rideHeight = double.Parse(oneWheelNewAerodynamicMapPointRideHeightTextBox.Text);
            double dragCoefficient = double.Parse(oneWheelNewAerodynamicMapPointDragCoefficientTextBox.Text);
            double liftCoefficient = double.Parse(oneWheelNewAerodynamicMapPointLiftCoefficientTextBox.Text);
            // Initializes a new object
            Vehicle.AerodynamicMapPoint aerodynamicMapPoint = new Vehicle.AerodynamicMapPoint(speed, rideHeight, dragCoefficient, liftCoefficient);
            // Adds the object to the listbox and the combobox
            oneWheelAerodynamicMapPointsListBox.Items.Add(aerodynamicMapPoint);
            // Reorders the aerodynamic map points listbox items in ascending order of car height and speed
            List<Vehicle.AerodynamicMapPoint> aerodynamicMapPoints = new List<Vehicle.AerodynamicMapPoint>();
            foreach (var aerodynamicMapPointItem in oneWheelAerodynamicMapPointsListBox.Items)
            {
                Vehicle.AerodynamicMapPoint currentAerodynamicMapPoint = aerodynamicMapPointItem as Vehicle.AerodynamicMapPoint;
                aerodynamicMapPoints.Add(currentAerodynamicMapPoint);
            }
            aerodynamicMapPoints = aerodynamicMapPoints.OrderBy(currentGearRatio => currentGearRatio.RideHeight).ToList();
            aerodynamicMapPoints = aerodynamicMapPoints.OrderBy(currentGearRatio => currentGearRatio.WindRelativeSpeed).ToList();
            oneWheelAerodynamicMapPointsListBox.Items.Clear();
            foreach (Vehicle.AerodynamicMapPoint currentAerodynamicMapPoint in aerodynamicMapPoints)
            {
                oneWheelAerodynamicMapPointsListBox.Items.Add(currentAerodynamicMapPoint);
            }
        }

        /// <summary>
        /// Deletes a aerodynamic map point from the one wheel model aerodynamic map points listbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _OneWheelDeleteAerodynamicMapPointOfListBox_Click(object sender, RoutedEventArgs e)
        {
            // Checks if there's a listbox item selected and then removes it
            if (oneWheelAerodynamicMapPointsListBox.SelectedItems.Count == 1)
            {
                oneWheelAerodynamicMapPointsListBox.Items.RemoveAt(oneWheelAerodynamicMapPointsListBox.Items.IndexOf(oneWheelAerodynamicMapPointsListBox.SelectedItem));
            }
        }

        #endregion

        #region Engine

        /// <summary>
        /// Creates an engine object and adds it to the one wheel model engines listbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _OneWheelAddEngineToListBox_Click(object sender, RoutedEventArgs e)
        {
            if (oneWheelEngineIDTextBox.Text != "" &&
                oneWheelEngineCurvesComboBox.SelectedItem != null &&
                double.Parse(oneWheelMaxThrottleTextBox.Text) != 0 &&
                double.Parse(oneWheelFuelDensityTextBox.Text) != 0)
            {
                // Gets the object's data
                string engineID = oneWheelEngineIDTextBox.Text;
                string description = oneWheelEngineDescriptionTextBox.Text;
                Vehicle.EngineCurves engineCurves = oneWheelEngineCurvesComboBox.SelectedItem as Vehicle.EngineCurves;
                double maxThrottle = double.Parse(oneWheelMaxThrottleTextBox.Text) / 100;
                double fuelDensity = double.Parse(oneWheelFuelDensityTextBox.Text);
                // Initializes a new object
                Vehicle.Engine engine = new Vehicle.Engine(engineID, description, engineCurves, maxThrottle, fuelDensity);
                // Adds the object to the listbox and the combobox
                oneWheelEngineListBox.Items.Add(engine);
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
        private void _OneWheelDeleteEngineOfListBox_Click(object sender, RoutedEventArgs e)
        {
            // Checks if there's a listbox item selected and then removes it
            if (oneWheelEngineListBox.SelectedItems.Count == 1)
            {
                oneWheelEngineListBox.Items.RemoveAt(oneWheelEngineListBox.Items.IndexOf(oneWheelEngineListBox.SelectedItem));
            }
        }

        /// <summary>
        /// Loads the properties of the one wheel model engines listbox's engine and displays it in the UI fields.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _OneWheelEngineListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // Checks if there's a listbox item selected
            if (oneWheelEngineListBox.SelectedItems.Count == 1)
            {
                // Gets the selected object
                Vehicle.Engine engine = oneWheelEngineListBox.SelectedItem as Vehicle.Engine;
                // Writes the properties in the UI
                oneWheelEngineIDTextBox.Text = engine.ID;
                oneWheelEngineDescriptionTextBox.Text = engine.Description;
                oneWheelEngineCurvesComboBox.Text = engine.EngineCurves.ToString();
                oneWheelMaxThrottleTextBox.Text = (engine.MaxThrottle * 100).ToString("F3");
                oneWheelFuelDensityTextBox.Text = engine.FuelDensity.ToString("F3");
            }
        }

        #endregion

        #region Engine Curves

        /// <summary>
        /// Creates an engine curves object and adds it to the one wheel model engine curves listbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _OneWheelAddEngineCurvesToListBox_Click(object sender, RoutedEventArgs e)
        {
            if (oneWheelEngineCurvesIDTextBox.Text != "" &&
                oneWheelEngineCurvesPointsListBox.Items.Count != 0)
            {
                // Gets the object's properties values
                string curvesID = oneWheelEngineCurvesIDTextBox.Text;
                string description = oneWheelEngineCurvesDescriptionTextBox.Text;
                List<Vehicle.EngineCurvesPoint> engineCurvesPoints = new List<Vehicle.EngineCurvesPoint>();
                foreach (var engineCurvesPointItem in oneWheelEngineCurvesPointsListBox.Items)
                {
                    Vehicle.EngineCurvesPoint engineCurvesPoint = engineCurvesPointItem as Vehicle.EngineCurvesPoint;
                    engineCurvesPoints.Add(engineCurvesPoint);
                }
                // Initializes a new object
                Vehicle.EngineCurves engineCurves = new Vehicle.EngineCurves(curvesID, description, engineCurvesPoints);
                // Adds the object to the listbox and the combobox
                oneWheelEngineCurvesListBox.Items.Add(engineCurves);
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
        private void _OneWheelDeleteEngineCurvesOfListBox_Click(object sender, RoutedEventArgs e)
        {
            // Checks if there's a listbox item selected and then removes it
            if (oneWheelEngineCurvesListBox.SelectedItems.Count == 1)
            {
                oneWheelEngineCurvesListBox.Items.RemoveAt(oneWheelEngineCurvesListBox.Items.IndexOf(oneWheelEngineCurvesListBox.SelectedItem));
            }
        }

        /// <summary>
        /// Loads the properties of the one wheel model engine curves listbox's engine curves and displays it in the UI fields.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _OneWheelEngineCurvesListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // Checks if there's a listbox item selected
            if (oneWheelEngineCurvesListBox.SelectedItems.Count == 1)
            {
                // Gets the selected object
                Vehicle.EngineCurves engineCurves = oneWheelEngineCurvesListBox.SelectedItem as Vehicle.EngineCurves;
                // Writes the properties in the UI
                oneWheelEngineCurvesIDTextBox.Text = engineCurves.ID;
                oneWheelEngineCurvesDescriptionTextBox.Text = engineCurves.Description;
                // Clears and writes the list in the UI
                oneWheelEngineCurvesPointsListBox.Items.Clear();
                foreach (Vehicle.EngineCurvesPoint engineCurvesPoint in engineCurves.CurvesPoints)
                {
                    oneWheelGearRatiosListBox.Items.Add(engineCurvesPoint);
                }
            }
        }

        /// <summary>
        /// Creates an engine curves point object and adds it to the one wheel model engine curves points listbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _OneWheelAddEngineCurvesPointToListBox_Click(object sender, RoutedEventArgs e)
        {
            if (double.Parse(oneWheelNewEngineCurvesPointTorqueTextBox.Text) != 0)
            {
                // Gets the object's properties values
                double rpm = double.Parse(oneWheelNewEngineCurvesPointRPMTextBox.Text);
                if (rpm < 0) rpm = -rpm;
                double torque = double.Parse(oneWheelNewEngineCurvesPointTorqueTextBox.Text);
                double brakingTorque = double.Parse(oneWheelNewEngineCurvesPointBrakingTorqueTextBox.Text);
                double specFuelCons = double.Parse(oneWheelNewEngineCurvesPointSpecFuelConsTextBox.Text);
                // Initializes a new object
                Vehicle.EngineCurvesPoint engineCurvesPoint = new Vehicle.EngineCurvesPoint(rpm, torque, brakingTorque, specFuelCons);
                // Adds the object to the listbox and the combobox
                oneWheelEngineCurvesPointsListBox.Items.Add(engineCurvesPoint);
                // Reorders the engine curves points listbox items in ascending order of rpm
                List<Vehicle.EngineCurvesPoint> engineCurvesPoints = new List<Vehicle.EngineCurvesPoint>();
                foreach (var engineCurvesPointItem in oneWheelEngineCurvesPointsListBox.Items)
                {
                    Vehicle.EngineCurvesPoint currentEngineCurvesPoint = engineCurvesPointItem as Vehicle.EngineCurvesPoint;
                    engineCurvesPoints.Add(currentEngineCurvesPoint);
                }
                engineCurvesPoints = engineCurvesPoints.OrderBy(currentCurvePoint => currentCurvePoint.RotationalSpeed).ToList();
                oneWheelEngineCurvesPointsListBox.Items.Clear();
                foreach (Vehicle.EngineCurvesPoint currentEngineCurvesPoint in engineCurvesPoints)
                {
                    oneWheelEngineCurvesPointsListBox.Items.Add(currentEngineCurvesPoint);
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
        private void _OneWheelDeleteEngineCurvesPointOfListBox_Click(object sender, RoutedEventArgs e)
        {
            // Checks if there's a listbox item selected and then removes it
            if (oneWheelEngineCurvesPointsListBox.SelectedItems.Count == 1)
            {
                oneWheelEngineCurvesPointsListBox.Items.RemoveAt(oneWheelEngineCurvesPointsListBox.Items.IndexOf(oneWheelEngineCurvesPointsListBox.SelectedItem));
            }
        }

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
                // Adds the object to the listbox and the combobox
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
            ChartZoomPanBehavior zoomingAndPanning = new ChartZoomPanBehavior()
            {
                EnableZoomingToolBar = true,
                EnableMouseWheelZooming = true,
                EnablePanning = true,
                ZoomRelativeToCursor = true,
            };
            chart.Behaviors.Add(zoomingAndPanning);
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
                // Adds the object to the listbox and the combobox
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
                // Adds the object to the listbox and the combobox
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
                // Adds the object to the listbox and the combobox
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
                // Adds the object to the listbox and the combobox
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
            _UpdateTabularPathSectionsPreviewChart();
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
            Path path = new Path("", "", sectorsSet, sectionsSet, 100);
            path.GeneratePathPointsParametersFromTabular();
            PathViewModel pathViewModel = new PathViewModel(path);
            // Initializes a new chart
            SfChart chart = new SfChart()
            {
                Header = "Path Preview",
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(10),
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
            ChartZoomPanBehavior zoomingAndPanning = new ChartZoomPanBehavior()
            {
                EnableZoomingToolBar = true,
                EnableMouseWheelZooming = true,
                EnablePanning = true,
                ZoomRelativeToCursor = true,
            };
            chart.Behaviors.Add(zoomingAndPanning);
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
                ggvDiagramVehicleSelectionComboBox.SelectedItem != null &&
                int.Parse(ggvDiagramAmountOfPointsPerSpeedTextBox.Text) >= 8 &&
                int.Parse(ggvDiagramAmountOfDirectionsTextBox.Text) >= 4 &&
                int.Parse(ggvDiagramAmountOfSpeedsTextBox.Text) != 0 &&
                Math.Abs(double.Parse(ggvDiagramLowestSpeedTextBox.Text)) <= Math.Abs(double.Parse(ggvDiagramHighestSpeedTextBox.Text)))
            {
                // Gets the object's data
                string id = ggvDiagramIDTextBox.Text;
                string description = ggvDiagramDescriptionTextBox.Text;
                Vehicle.Car car = ggvDiagramVehicleSelectionComboBox.SelectedItem as Vehicle.Car;
                int amountOfPointsPerSpeed = int.Parse(ggvDiagramAmountOfPointsPerSpeedTextBox.Text);
                int amountOfDirections = int.Parse(ggvDiagramAmountOfDirectionsTextBox.Text);
                int amountOfSpeeds = int.Parse(ggvDiagramAmountOfSpeedsTextBox.Text);
                double lowestSpeed = double.Parse(ggvDiagramLowestSpeedTextBox.Text) / 3.6;
                double highestSpeed = double.Parse(ggvDiagramHighestSpeedTextBox.Text) / 3.6;
                // Initializes a new object
                Simulation.GGVDiagram ggvDiagram = new Simulation.GGVDiagram(id, description, car, amountOfPointsPerSpeed, amountOfDirections, amountOfSpeeds, lowestSpeed, highestSpeed);
                ggvDiagram.GenerateGGVDiagram();
                // Adds the object to the listbox and the combobox
                simulationGGVDiagramListBox.Items.Add(ggvDiagram);
            }
            else System.Windows.MessageBox.Show(
               "Could not create GGV Diagram. \n " +
               "    It should have an ID. \n" +
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
                ggvDiagramVehicleSelectionComboBox.Text = ggvDiagram.Car.ToString();
                ggvDiagramAmountOfPointsPerSpeedTextBox.Text = ggvDiagram.AmountOfPointsPerSpeed.ToString();
                ggvDiagramAmountOfDirectionsTextBox.Text = ggvDiagram.AmountOfDirections.ToString();
                ggvDiagramAmountOfSpeedsTextBox.Text = ggvDiagram.AmountOfSpeeds.ToString("F0");
                ggvDiagramLowestSpeedTextBox.Text = (ggvDiagram.LowestSpeed * 3.6).ToString("F2");
                ggvDiagramHighestSpeedTextBox.Text = (ggvDiagram.HighestSpeed * 3.6).ToString("F2");
                // Updates the ggv diagram display chart
                if ((bool)simulationGGVDiagramAllowPathDisplayCheckBox.IsChecked) _UpdateSimulationGGVDiagramDisplayChart();
            }
        }

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
                Header = "Longitudinal Acceleration [G]",
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
                Header = "Lateral Acceleration [G]",
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
                // Initializes a new object
                Simulation.LapTimeSimulation lapTimeSimulation = new Simulation.LapTimeSimulation(id, description, path, ggvDiagrams, isFirstLap);
                Results.LapTimeSimulationResults results = lapTimeSimulation.RunLapTimeSimulation();
                results.GetElapsedTimeAndLapTime();
                // Adds the object to the listbox and the combobox
                lapTimeSimulationListBox.Items.Add(lapTimeSimulation);
                lapTimeSimulationResultsAnalysisResultsListBox.Items.Add(results);
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
        /// Deletes a lap time simulation from the lap time simulation listbox and its results object from the results analysis listbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _LapTimeSimulationDeleteOfListBox_Click(object sender, RoutedEventArgs e)
        {
            // Checks if there's a listbox item selected and then removes it
            if (lapTimeSimulationListBox.SelectedItems.Count == 1)
            {
                lapTimeSimulationListBox.Items.RemoveAt(lapTimeSimulationListBox.Items.IndexOf(lapTimeSimulationListBox.SelectedItem));
                lapTimeSimulationResultsAnalysisResultsListBox.Items.RemoveAt(lapTimeSimulationListBox.Items.IndexOf(lapTimeSimulationListBox.SelectedItem));
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
                int sectorIndex = int.Parse(lapTimeSimulationNewSectorIndexComboBox.Text);
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
        /// Changes the items of the new sector index combobox to match the selected path.
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


        private void AddLapTimeSimulationToListboxButton_Click(object sender, RoutedEventArgs e)
        {/*
            if (LapTimeSimulationModeCombobox.Text!=null && 
                LapTimeSimulationPathCombobox.Text!=null)
            {
                // Gets the sfdatagrid data
                SfDataGrid sfDataGrid = lapTimeSimulationPathSectorsSetupGrid.Children[0] as SfDataGrid;
                Simulation.LapTimeSimulationSectorsSetupViewModel lapTimeSimulationSectorsSetupViewModel = new Simulation.LapTimeSimulationSectorsSetupViewModel();
                lapTimeSimulationSectorsSetupViewModel.LapTimeSimulationSectorsSetups = sfDataGrid.ItemsSource as NoteervableCollection<Simulation.LapTimeSimulationSectorsSetup>;
                List<Simulation.LapTimeSimulationSectorsSetup> sectorsSetup = lapTimeSimulationSectorsSetupViewModel.LapTimeSimulationSectorsSetups.ToList();
                // Gets the object's properties values
                Path path = LapTimeSimulationPathCombobox.SelectedItem as Path;
                string simulationMode = LapTimeSimulationModeCombobox.Text;
                bool isFirstLap = false;
                if (simulationMode == "First Lap") isFirstLap = true;
                // Initializes a new object
                Simulation.LapTimeSimulation lapTimeSimulation = new Simulation.LapTimeSimulation(path, sectorsSetup, isFirstLap);
                // Runs the lap time simulation
                lapTimeSimulation.RunLapTimeSimulation();
                // Adds the object to the listbox
                lapTimeSimulationListBox.Items.Add(lapTimeSimulation);
            }*/
        }

        private void DeleteLapTimeSimulationOfListboxButton_Click(object sender, RoutedEventArgs e)
        {/*
            // Checks if there's a listbox item selected and then removes it
            if (lapTimeSimulationListBox.SelectedItems.Count == 1)
            {
                lapTimeSimulationListBox.Items.RemoveAt(lapTimeSimulationListBox.Items.IndexOf(lapTimeSimulationListBox.SelectedItem));
            }*/
        }

        private void LapTimeSimulationListBox_SelectionChanged_(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            /*
            if (lapTimeSimulationListBox.SelectedItems.Count == 1)
            {
                // Gets the selected object
                Simulation.LapTimeSimulation lapTimeSimulation = lapTimeSimulationListBox.SelectedItem as Simulation.LapTimeSimulation;
                // Get and writes the tabular data in the UI
                Simulation.LapTimeSimulationSectorsSetupViewModel lapTimeSimulationSectorsSetupViewModel = new Simulation.LapTimeSimulationSectorsSetupViewModel();
                lapTimeSimulationSectorsSetupViewModel.LapTimeSimulationSectorsSetups.Clear();
                for (int iSector = 0; iSector < lapTimeSimulation.GGVDiagramsPerSector.Count; iSector++)
                {
                    lapTimeSimulationSectorsSetupViewModel.LapTimeSimulationSectorsSetups.Add(new Simulation.LapTimeSimulationSectorsSetup(lapTimeSimulation.Path.Sectors[iSector], lapTimeSimulation.GGVDiagramsPerSector[iSector].SectorGGVDiagram));
                }
                SetLapTimeSimulationSectorsSetupSfDataGrid(lapTimeSimulationSectorsSetupViewModel);
                // Writes the properties in the UI
                LapTimeSimulationPathCombobox.Text = lapTimeSimulation.Path.ToString();
                if (lapTimeSimulation.IsFirstLap) LapTimeSimulationModeCombobox.Text = "First Lap";
                else LapTimeSimulationModeCombobox.Text = "Normal Lap";
            }*/
        }

        private void LapTimeSimulationPathCombobox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {/*
            if (LapTimeSimulationPathCombobox.Text != null &&
                ggvDiagramListBox.HasItems)
            {
                // Gets the selected object
                Path path = LapTimeSimulationPathCombobox.SelectedItem as Path;
                // Initializes a new sectors setup object
                Simulation.LapTimeSimulationSectorsSetupViewModel lapTimeSimulationSectorsSetupViewModel = new Simulation.LapTimeSimulationSectorsSetupViewModel();
                // Fills the object with data
                foreach (PathSector sector in path.Sectors)
                {
                    lapTimeSimulationSectorsSetupViewModel.LapTimeSimulationSectorsSetups.Add(new Simulation.LapTimeSimulationSectorsSetup(sector, ggvDiagramListBox.Items[0] as Simulation.GGVDiagram));
                }
                // Clears the data grid content
                lapTimeSimulationPathSectorsSetupGrid.Children.Clear();
                // Sets up the datagrid
                SetLapTimeSimulationSectorsSetupSfDataGrid(lapTimeSimulationSectorsSetupViewModel);
            }*/
        }

        private void SetLapTimeSimulationSectorsSetupSfDataGrid()
        {/*
            // Clears the input grid
            lapTimeSimulationPathSectorsSetupGrid.Children.Clear();
            // Initializes the input data grid
            SfDataGrid sfDataGrid = new SfDataGrid()
            {
                DataContext = viewModel,
                HeaderRowHeight = 30,
                LiveDataUpdateMode = Syncfusion.Data.LiveDataUpdateMode.AllowDataShaping,
                AllowEditing = true,
                AllowSorting = true
            };
            // Adds the data grid's columns
            GridNumericColumn sectorColumn = new GridNumericColumn()
            {
                HeaderText = "Sector",
                MappingName = "Sector.Index",
                NumberDecimalDigits = 0
            };
            GridComboBoxColumn ggvColumn = new GridComboBoxColumn()
            {
                HeaderText = "Sector",
                MappingName = "SectorGGVDiagram"
            };
            sfDataGrid.Columns.Add(sectorColumn);
            sfDataGrid.Columns.Add(ggvColumn);
            // Adds the SfDataGrid to the grid
            tabularPathSectorsGrid.Children.Add(sfDataGrid);*/
        }

        #endregion

        #endregion

        #region Results Analysis Methods

        #region GGV Diagram

        /// <summary>
        /// Adds a new chart tab to the GGV Diagram results analysis environment.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _GGVDiagramResultsAnalysisChartTabControl_NewButtonClick(object sender, EventArgs e)
        {
            TabItemExt item = new TabItemExt
            {

                Header = "Chart" + (ggvDiagramResultsAnalysisChartTabControl.Items.Count + 1),

            };
            ggvDiagramResultsAnalysisChartTabControl.Items.Add(item);
        }

        /// <summary>
        /// Adds a new chart to the GGV Diagram results analysis environment when the last chart is closed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _GGVDiagramResultsAnalysisChartTabControl_OnCloseButtonClick(object sender, CloseTabEventArgs e)
        {
            if (ggvDiagramResultsAnalysisChartTabControl.Items.Count == 1)
            {
                TabItemExt item = new TabItemExt
                {

                    Header = "Chart" + (ggvDiagramResultsAnalysisChartTabControl.Items.Count),

                };
                ggvDiagramResultsAnalysisChartTabControl.Items.Add(item);
            }
        }

        #endregion

        #region Lap Time Simulation

        /// <summary>
        /// Adds a new chart tab to the Lap Time Simulation results analysis environment.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _LapTimeSimulationResultsAnalysisChartTabControl_NewButtonClick(object sender, EventArgs e)
        {
            TabItemExt item = new TabItemExt
            {

                Header = "Chart" + (lapTimeSimulationResultsAnalysisChartTabControl.Items.Count + 1),

            };
            lapTimeSimulationResultsAnalysisChartTabControl.Items.Add(item);
        }

        /// <summary>
        /// Adds a new chart to the Lap Time Simulation results analysis environment when the last chart is closed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _LapTimeSimulationResultsAnalysisChartTabControl_OnCloseButtonClick(object sender, CloseTabEventArgs e)
        {
            if (lapTimeSimulationResultsAnalysisChartTabControl.Items.Count == 1)
            {
                TabItemExt item = new TabItemExt
                {

                    Header = "Chart" + (lapTimeSimulationResultsAnalysisChartTabControl.Items.Count),

                };
                lapTimeSimulationResultsAnalysisChartTabControl.Items.Add(item);
            }
        }

        /// <summary>
        /// Updates the current lap time simulation analysis chart accordingly to the specified type in the combobox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _LapTimeSimulationResultsAnalysisChartTypeCombobox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            switch (lapTimeSimulationResultsAnalysisChartTypeCombobox.SelectedItem.ToString())
            {
                case "2D Chart":
                    lapTimeSimulationResultsAnalysis2DChartMenuGrid.Visibility = Visibility.Visible;
                    lapTimeSimulationResultsAnalysisTrackMapMenuGrid.Visibility = Visibility.Collapsed;
                    break;
                case "Track Map":
                    lapTimeSimulationResultsAnalysis2DChartMenuGrid.Visibility = Visibility.Collapsed;
                    lapTimeSimulationResultsAnalysisTrackMapMenuGrid.Visibility = Visibility.Visible;
                    break;
                default:
                    break;
            }
            _ClearCurrentLapTimeSimulationAnalysisChart();
        }

        /// <summary>
        /// Clears the current lap time simulation analysis chart
        /// </summary>
        private void _ClearCurrentLapTimeSimulationAnalysisChart()
        {
            TabItemExt currentChartTab = lapTimeSimulationResultsAnalysisChartTabControl.SelectedItem as TabItemExt;
            currentChartTab.Content = new Grid();
        }

        #region 2D Chart

        /// <summary>
        /// Updates the current lap time simulation analysis 2D chart when the x axis combobox item gets changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _LapTimeSimulationResultsAnalysis2DChartXAxisDataCombobox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (lapTimeSimulationResultsAnalysis2DChartXAxisDataCombobox.SelectedItem != null && lapTimeSimulationResultsAnalysis2DChartYAxisDataCombobox.SelectedItem != null && lapTimeSimulationResultsAnalysis2DChartCurvesTypeDataCombobox.SelectedItem != null && lapTimeSimulationResultsAnalysisResultsListBox.SelectedItems.Count != 0)
            {
                _UpdateCurrentLapTimeSimulationAnalysis2DChart();
            }
            else _ClearCurrentLapTimeSimulationAnalysisChart();
        }

        /// <summary>
        /// Updates the current lap time simulation analysis 2D chart when the y axis combobox item gets changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _LapTimeSimulationResultsAnalysis2DChartYAxisDataCombobox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (lapTimeSimulationResultsAnalysis2DChartXAxisDataCombobox.SelectedItem != null && lapTimeSimulationResultsAnalysis2DChartYAxisDataCombobox.SelectedItem != null && lapTimeSimulationResultsAnalysis2DChartCurvesTypeDataCombobox.SelectedItem != null && lapTimeSimulationResultsAnalysisResultsListBox.SelectedItems.Count != 0)
            {
                _UpdateCurrentLapTimeSimulationAnalysis2DChart();
            }
            else _ClearCurrentLapTimeSimulationAnalysisChart();
        }

        /// <summary>
        /// Updates the current lap time simulation analysis 2D chart when the curve type combobox item gets changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _LapTimeSimulationResultsAnalysis2DChartCurvesTypeDataCombobox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (lapTimeSimulationResultsAnalysis2DChartXAxisDataCombobox.SelectedItem != null && lapTimeSimulationResultsAnalysis2DChartYAxisDataCombobox.SelectedItem != null && lapTimeSimulationResultsAnalysis2DChartCurvesTypeDataCombobox.SelectedItem != null && lapTimeSimulationResultsAnalysisResultsListBox.SelectedItems.Count != 0)
            {
                _UpdateCurrentLapTimeSimulationAnalysis2DChart();
            }
            else _ClearCurrentLapTimeSimulationAnalysisChart();
        }

        /// <summary>
        /// Updates the current lap time simulation analysis 2D chart
        /// </summary>
        private void _UpdateCurrentLapTimeSimulationAnalysis2DChart()
        {
            // Initializes the new chart
            SfChart chart = new SfChart
            {
                Header = "Lap Time Simulation Analysis: 2D Chart",
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(10),
                Width = tabularPathDockingWindow.ActualHeight,
                Legend = new ChartLegend(),
                PrimaryAxis = new NumericalAxis(),
                SecondaryAxis = new NumericalAxis()
            };
            // Adds zoom/panning behaviour to the chart
            ChartZoomPanBehavior zoomingAndPanning = new ChartZoomPanBehavior()
            {
                EnableZoomingToolBar = true,
                EnableMouseWheelZooming = true,
                EnablePanning = true,
                ZoomRelativeToCursor = true,
            };
            chart.Behaviors.Add(zoomingAndPanning);
            // Initializes the data series
            FastLineSeries fastLineSeries;
            FastScatterBitmapSeries fastScatterSeries;
            // Sweeps the lap time simulation results and adds the data to the chart.
            foreach (Results.LapTimeSimulationResults results in lapTimeSimulationResultsAnalysisResultsListBox.SelectedItems)
            {
                // Initializes the chart's data series
                Results.LapTimeSimulationResultsViewModel resultsViewModel = new Results.LapTimeSimulationResultsViewModel(results);
                fastLineSeries = new FastLineSeries() { ItemsSource = resultsViewModel.ResultsDisplayCollection };
                fastScatterSeries = new FastScatterBitmapSeries() { ItemsSource = resultsViewModel.ResultsDisplayCollection };
                // Adds the x axis to the chart with the x axis combobox selected item's type of data.
                switch (lapTimeSimulationResultsAnalysis2DChartXAxisDataCombobox.SelectedValue.ToString())
                {
                    case "Time":
                        chart.PrimaryAxis.Header = "Time [s]";
                        chart.PrimaryAxis.LabelFormat = "N2";
                        fastLineSeries.XBindingPath = "ElapsedTime";
                        fastScatterSeries.XBindingPath = "ElapsedTime";
                        break;
                    case "Distance":
                        chart.PrimaryAxis.Header = "Distance [m]";
                        chart.PrimaryAxis.LabelFormat = "N2";
                        fastLineSeries.XBindingPath = "ElapsedDistance";
                        fastScatterSeries.XBindingPath = "ElapsedDistance";
                        break;
                    case "Speed":
                        chart.PrimaryAxis.Header = "Speed [km/h]";
                        chart.PrimaryAxis.LabelFormat = "N2";
                        fastLineSeries.XBindingPath = "Speed";
                        fastScatterSeries.XBindingPath = "Speed";
                        break;
                    case "Longitudinal Acceleration":
                        chart.PrimaryAxis.Header = "Longitudinal Acceleration [G]";
                        chart.PrimaryAxis.LabelFormat = "N2";
                        fastLineSeries.XBindingPath = "LongitudinalAcceleration";
                        fastScatterSeries.XBindingPath = "LongitudinalAcceleration";
                        break;
                    case "Lateral Acceleration":
                        chart.PrimaryAxis.Header = "Lateral Acceleration [G]";
                        chart.PrimaryAxis.LabelFormat = "N2";
                        fastLineSeries.XBindingPath = "LateralAcceleration";
                        fastScatterSeries.XBindingPath = "LateralAcceleration";
                        break;
                    case "Gear":
                        chart.PrimaryAxis.Header = "Gear Number";
                        chart.PrimaryAxis.LabelFormat = "N0";
                        fastLineSeries.XBindingPath = "GearNumber";
                        fastScatterSeries.XBindingPath = "GearNumber";
                        break;
                    default:
                        break;
                }
                // Adds the y axis to the chart with the y axis combobox selected item's type of data.
                switch (lapTimeSimulationResultsAnalysis2DChartYAxisDataCombobox.SelectedValue.ToString())
                {
                    case "Time":
                        chart.SecondaryAxis.Header = "Time [s]";
                        chart.SecondaryAxis.LabelFormat = "N2";
                        fastLineSeries.YBindingPath = "ElapsedTime";
                        fastScatterSeries.YBindingPath = "ElapsedTime";
                        break;
                    case "Distance":
                        chart.SecondaryAxis.Header = "Distance [m]";
                        chart.SecondaryAxis.LabelFormat = "N2";
                        fastLineSeries.YBindingPath = "ElapsedDistance";
                        fastScatterSeries.YBindingPath = "ElapsedDistance";
                        break;
                    case "Speed":
                        chart.SecondaryAxis.Header = "Speed [km/h]";
                        chart.SecondaryAxis.LabelFormat = "N2";
                        fastLineSeries.YBindingPath = "Speed";
                        fastScatterSeries.YBindingPath = "Speed";
                        break;
                    case "Longitudinal Acceleration":
                        chart.SecondaryAxis.Header = "Longitudinal Acceleration [G]";
                        chart.SecondaryAxis.LabelFormat = "N2";
                        fastLineSeries.YBindingPath = "LongitudinalAcceleration";
                        fastScatterSeries.YBindingPath = "LongitudinalAcceleration";
                        break;
                    case "Lateral Acceleration":
                        chart.SecondaryAxis.Header = "Lateral Acceleration [G]";
                        chart.SecondaryAxis.LabelFormat = "N2";
                        fastLineSeries.YBindingPath = "LateralAcceleration";
                        fastScatterSeries.YBindingPath = "LateralAcceleration";
                        break;
                    case "Gear":
                        chart.SecondaryAxis.Header = "Gear Number";
                        chart.SecondaryAxis.LabelFormat = "N0";
                        fastLineSeries.YBindingPath = "GearNumber";
                        fastScatterSeries.YBindingPath = "GearNumber";
                        break;
                    default:
                        break;
                }
                // Adds the series to the chart
                switch (lapTimeSimulationResultsAnalysis2DChartCurvesTypeDataCombobox.SelectedValue.ToString())
                {
                    case "Line":
                        chart.Series.Add(fastLineSeries);
                        // Adds a trackball to the chart
                        ChartTrackBallBehavior trackBall = new ChartTrackBallBehavior();
                        chart.Behaviors.Add(trackBall);
                        break;
                    case "Scatter":
                        chart.Series.Add(fastScatterSeries);
                        break;
                    default:
                        break;
                }
            }
            // Adds the chart to the current chart tab
            Grid grid = new Grid();
            grid.Children.Add(chart);
            TabItemExt currentChartTab = lapTimeSimulationResultsAnalysisChartTabControl.SelectedItem as TabItemExt;
            currentChartTab.Content = grid;
        }

        #endregion

        #endregion

        #endregion

    }
}
