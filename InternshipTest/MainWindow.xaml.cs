#region Copyright Syncfusion Inc. 2001-2019.
// Copyright Syncfusion Inc. 2001-2019. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Syncfusion.SfSkinManager;
using Syncfusion.UI.Xaml.Charts;
using Syncfusion.Windows.Tools.Controls;

namespace InternshipTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : RibbonWindow
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
        public string CurrentVisualStyle
        {
            get
            {
                return currentVisualStyle;
            }
            set
            {
                currentVisualStyle = value;
                OnVisualStyleChanged();
            }
        }
        #endregion
        public MainWindow()
        {
            InitializeComponent();
			this.Loaded += OnLoaded;

            PopulateFields();
        }
		/// <summary>
        /// Called when [loaded].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            CurrentVisualStyle = "Blend";
        }
		/// <summary>
        /// On Visual Style Changed.
        /// </summary>
        /// <remarks></remarks>
        private void OnVisualStyleChanged()
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
        private void MainRibbon_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (vehicleInputTab.IsChecked)
            {
                vehicleInputEnvironment.Visibility = Visibility.Visible;
                pathInputEnvironment.Visibility = Visibility.Hidden;
                simulationInputEnvironment.Visibility = Visibility.Hidden;
                resultsAnalysisEnvironment.Visibility = Visibility.Hidden;
            }
            else if (pathInputTab.IsChecked)
            {
                vehicleInputEnvironment.Visibility = Visibility.Hidden;
                pathInputEnvironment.Visibility = Visibility.Visible;
                simulationInputEnvironment.Visibility = Visibility.Hidden;
                resultsAnalysisEnvironment.Visibility = Visibility.Hidden;
            }
            else if (simulationInputTab.IsChecked)
            {
                vehicleInputEnvironment.Visibility = Visibility.Hidden;
                pathInputEnvironment.Visibility = Visibility.Hidden;
                simulationInputEnvironment.Visibility = Visibility.Visible;
                resultsAnalysisEnvironment.Visibility = Visibility.Hidden;
            }
            else if (resultsTab.IsChecked)
            {
                vehicleInputEnvironment.Visibility = Visibility.Hidden;
                pathInputEnvironment.Visibility = Visibility.Hidden;
                simulationInputEnvironment.Visibility = Visibility.Hidden;
                resultsAnalysisEnvironment.Visibility = Visibility.Visible;
            }
        }

        #region Sample Preparation
        private void PopulateFields()
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
            oneWheelCarHeightTextbox.Text = "50";

            // Brakes
            oneWheelBrakesIDTextBox.Text = "brk1";
            oneWheelBrakesMaxTorqueTextBox.Text = "2000";

            // Tire
            oneWheelTireIDTextBox.Text = "tire1";
            oneWheelTireStiffnessTextBox.Text = "120";
            oneWheelTireModelTextBox.Text = @"D:\Google Drive\Work\OptimumG\Internship Test\Programs\Auxiliar Files\TireModelMF52.txt";
            oneWheelLambdaMuxTextBox.Text = "0.66";
            oneWheelLambdaMuyTextBox.Text = "0.66";

            // Transmission
            oneWheelTransmissionIDTextBox.Text = "trans1";
            oneWheelTransmissionTypeComboBox.Text = "2WD";
            oneWheelPrimaryRatioTextBox.Text = "2.111";
            oneWheelFinalRatioTextBox.Text = "3.7143";
            oneWheelGearShiftTimeTextBox.Text = "0.2";
            oneWheelTransmissionEfficiencyTextBox.Text = "87.5";

            // Aerodynamics
            oneWheelAerodynamicsIDTextBox.Text = "aero1";
            oneWheelFrontalAreaTextBox.Text = "1.2";
            oneWheelAirDensityTextBox.Text = "1.3";

            // Engine
            oneWheelEngineIDTextBox.Text = "engine1";
            oneWheelMaxThrottleTextBox.Text = "100";
            oneWheelFuelDensityTextBox.Text = "870";

            // Car and setup fields
            oneWheelCarIDTextBox.Text = "car1";
            oneWheelSetupIDTextBox.Text = "setup1";
            oneWheelInertiaCombobox.Text = "inertia1";
            oneWheelTireCombobox.Text = "tire1";
            oneWheelEngineCombobox.Text = "engine1";
            oneWheelTransmissionCombobox.Text = "trans1";
            oneWheelAerodynamicsCombobox.Text = "aero1";
            oneWheelDRSCombobox.Text = "aero1";
            oneWheelSuspensionCombobox.Text = "susp1";
            oneWheelBrakesCombobox.Text = "brk1";

            // Path
            tabularPathIDTextBox.Text = "testPath";
            tabularPathResolutionTextBox.Text = "100";

            // GGV Diagram
            GGVDiagramAmountOfPointsPerSpeedTextBox.Text = "40";
            GGVDiagramAmountOfSpeedsTextBox.Text = "20";
            GGVDiagramLowestSpeedTextBox.Text = "10";
            GGVDiagramHighestSpeedTextBox.Text = "100";
        }
        #endregion

        #region One Wheel Car Input Methods
        #region CarAndSetup
        private void OneWheelAddCarAndSetupToListBox_Click(object sender, RoutedEventArgs e)
        {
            if (oneWheelCarIDTextBox.Text!=null &&
                oneWheelSetupIDTextBox.Text!=null &&
                oneWheelInertiaCombobox.Text!=null &&
                oneWheelEngineCombobox!=null &&
                oneWheelTransmissionCombobox.Text!=null &&
                oneWheelAerodynamicsCombobox.Text!=null &&
                oneWheelDRSCombobox!=null &&
                oneWheelSuspensionCombobox!=null &&
                oneWheelBrakesCombobox!=null)
            {
                // Gets the object's properties values
                string carID = oneWheelCarIDTextBox.Text;
                string setupID = oneWheelSetupIDTextBox.Text;
                Vehicle.OneWheel.Inertia inertia = oneWheelInertiaCombobox.SelectedItem as Vehicle.OneWheel.Inertia;
                Vehicle.OneWheel.Tire tire = oneWheelTireCombobox.SelectedItem as Vehicle.OneWheel.Tire;
                Vehicle.OneWheel.Engine engine = oneWheelEngineCombobox.SelectedItem as Vehicle.OneWheel.Engine;
                Vehicle.OneWheel.Transmission transmission = oneWheelTransmissionCombobox.SelectedItem as Vehicle.OneWheel.Transmission;
                Vehicle.OneWheel.Aerodynamics aerodynamics = oneWheelAerodynamicsCombobox.SelectedItem as Vehicle.OneWheel.Aerodynamics;
                Vehicle.OneWheel.Aerodynamics drs = oneWheelDRSCombobox.SelectedItem as Vehicle.OneWheel.Aerodynamics;
                Vehicle.OneWheel.Suspension suspension = oneWheelSuspensionCombobox.SelectedItem as Vehicle.OneWheel.Suspension;
                Vehicle.OneWheel.Brakes brakes = oneWheelBrakesCombobox.SelectedItem as Vehicle.OneWheel.Brakes;
                // Initializes a new object
                Vehicle.OneWheel.Car car = new Vehicle.OneWheel.Car(carID, setupID, inertia, tire, engine, transmission, aerodynamics, drs, suspension, brakes);
                // Adds the object to the listbox
                oneWheelCarAndSetupListBox.Items.Add(car);
            }
        }

        private void OneWheelDeleteCarAndSetupOfListBox_Click(object sender, RoutedEventArgs e)
        {
            // Checks if there's a listbox item selected and then removes it
            if (oneWheelCarAndSetupListBox.SelectedItems.Count == 1)
            {
                oneWheelCarAndSetupListBox.Items.RemoveAt(oneWheelCarAndSetupListBox.Items.IndexOf(oneWheelCarAndSetupListBox.SelectedItem));
            }
        }

        private void OneWheelCarAndSetupListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Checks if there's a listbox item selected
            if (oneWheelCarAndSetupListBox.SelectedItems.Count == 1)
            {
                // Gets the selected object
                Vehicle.OneWheel.Car car = oneWheelCarAndSetupListBox.SelectedItem as Vehicle.OneWheel.Car;
                // Writes the properties in the UI
                oneWheelCarIDTextBox.Text = car.CarID;
                oneWheelSetupIDTextBox.Text = car.SetupID;
                oneWheelInertiaCombobox.Text = car.Inertia.InertiaID;
                oneWheelTireCombobox.Text = car.Tire.TireID;
                oneWheelEngineCombobox.Text = car.Engine.EngineID;
                oneWheelTransmissionCombobox.Text = car.Transmission.TransmissionID;
                oneWheelAerodynamicsCombobox.Text = car.Aerodynamics.AeroID;
                oneWheelDRSCombobox.Text = car.DRS.AeroID;
                oneWheelSuspensionCombobox.Text = car.Suspension.SuspensionID;
                oneWheelBrakesCombobox.Text = car.Brakes.BrakesID;
            }
        }
        #endregion
        #region Inertia
        private void OneWheelAddInertiaToListBox_Click(object sender, RoutedEventArgs e)
        {
            // Gets the object's properties values
            string inertiaID = oneWheelInertiaIDTextBox.Text;
            double totalMass = double.Parse(oneWheelTotalMassTextBox.Text);
            double unsprungMass = double.Parse(oneWheelUnsprungMassTextbox.Text);
            double rotPartsMI = double.Parse(oneWheelRotPartsMITextbox.Text);
            double gravity = double.Parse(oneWheelGravityAccelTextbox.Text);
            // Initializes a new object
            Vehicle.OneWheel.Inertia inertia = new Vehicle.OneWheel.Inertia(inertiaID, totalMass, unsprungMass, rotPartsMI, gravity);
            // Adds the object to the listbox
            oneWheelInertiaListBox.Items.Add(inertia);
        }

        private void OneWheelDeleteInertiaOfListBox_Click(object sender, RoutedEventArgs e)
        {
            // Checks if there's a listbox item selected and then removes it
            if (oneWheelInertiaListBox.SelectedItems.Count == 1)
            {
                oneWheelInertiaListBox.Items.RemoveAt(oneWheelInertiaListBox.Items.IndexOf(oneWheelInertiaListBox.SelectedItem));
            }
        }

        private void OneWheelInertiaListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Checks if there's a listbox item selected
            if (oneWheelInertiaListBox.SelectedItems.Count == 1)
            {
                // Gets the selected object
                Vehicle.OneWheel.Inertia inertia = oneWheelInertiaListBox.SelectedItem as Vehicle.OneWheel.Inertia;
                // Writes the properties in the UI
                oneWheelInertiaIDTextBox.Text = inertia.InertiaID;
                oneWheelTotalMassTextBox.Text = inertia.TotalMass.ToString("F3");
                oneWheelUnsprungMassTextbox.Text = inertia.UnsprungMass.ToString("F3");
                oneWheelRotPartsMITextbox.Text = inertia.RotPartsMI.ToString("F3");
                oneWheelGravityAccelTextbox.Text = inertia.Gravity.ToString("F3");
            }
        }
        #endregion
        #region Suspension
        private void OneWheelAddSuspensionToListBox_Click(object sender, RoutedEventArgs e)
        {
            // Gets the object's properties values
            string suspensionID = oneWheelSuspensionIDTextBox.Text;
            double heaveStiffness = double.Parse(oneWheelHeaveStiffnessTextBox.Text);
            double carHeight = double.Parse(oneWheelCarHeightTextbox.Text);
            // Initializes a new object
            Vehicle.OneWheel.Suspension suspension = new Vehicle.OneWheel.Suspension(suspensionID, heaveStiffness, carHeight);
            // Adds the object to the listbox
            oneWheelSuspensionListBox.Items.Add(suspension);
        }

        private void OneWheelDeleteSuspensionOfListBox_Click(object sender, RoutedEventArgs e)
        {
            // Checks if there's a listbox item selected and then removes it
            if (oneWheelInertiaListBox.SelectedItems.Count == 1)
            {
                oneWheelSuspensionListBox.Items.RemoveAt(oneWheelSuspensionListBox.Items.IndexOf(oneWheelSuspensionListBox.SelectedItem));
            }
        }

        private void OneWheelSuspensionListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (oneWheelInertiaListBox.SelectedItems.Count == 1)
            {
                // Gets the selected object
                Vehicle.OneWheel.Suspension suspension = oneWheelSuspensionListBox.SelectedItem as Vehicle.OneWheel.Suspension;
                // Writes the properties in the UI
                oneWheelSuspensionIDTextBox.Text = suspension.SuspensionID;
                oneWheelHeaveStiffnessTextBox.Text = suspension.HeaveStiffness.ToString("F3");
                oneWheelCarHeightTextbox.Text = suspension.CarHeight.ToString("F3");
            }
        }
        #endregion
        #region Brakes
        private void OneWheelAddBrakesToListBox_Click(object sender, RoutedEventArgs e)
        {
            // Gets the object's properties values
            string brakesID = oneWheelBrakesIDTextBox.Text;
            double maxTorque = double.Parse(oneWheelBrakesMaxTorqueTextBox.Text);
            // Initializes a new object
            Vehicle.OneWheel.Brakes brakes = new Vehicle.OneWheel.Brakes(brakesID, maxTorque);
            // Adds the object to the listbox and the combobox
            oneWheelBrakesListBox.Items.Add(brakes);
        }

        private void OneWheelDeleteBrakesOfListBox_Click(object sender, RoutedEventArgs e)
        {
            // Checks if there's a listbox item selected and then removes it
            if (oneWheelBrakesListBox.SelectedItems.Count == 1)
            {
                oneWheelBrakesListBox.Items.RemoveAt(oneWheelBrakesListBox.Items.IndexOf(oneWheelBrakesListBox.SelectedItem));
            }
        }

        private void OneWheelBrakesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (oneWheelBrakesListBox.SelectedItems.Count == 1)
            {
                // Gets the selected object
                Vehicle.OneWheel.Brakes brakes = oneWheelBrakesListBox.SelectedItem as Vehicle.OneWheel.Brakes;
                // Writes the properties in the UI
                oneWheelBrakesIDTextBox.Text = brakes.BrakesID;
                oneWheelBrakesMaxTorqueTextBox.Text = brakes.MaxTorque.ToString("F3");
            }
        }
        #endregion
        #region Tire
        private void OneWheelAddTireToListBox_Click(object sender, RoutedEventArgs e)
        {
            // Inputs assigning and convertion.
            string tireID = oneWheelTireIDTextBox.Text;
            string tireModelFile = oneWheelTireModelTextBox.Text;
            double verticalStiffness = double.Parse(oneWheelTireStiffnessTextBox.Text);
            double lambdaFzO = double.Parse(oneWheelLambdaFzOTextBox.Text);
            double lambdaMux = double.Parse(oneWheelLambdaMuxTextBox.Text);
            double lambdaMuy = double.Parse(oneWheelLambdaMuyTextBox.Text);
            double lambdaMuV = double.Parse(oneWheelLambdaMuVTextBox.Text);
            double lambdaKxk = double.Parse(oneWheelLambdaKxkTextBox.Text);
            double lambdaKya = double.Parse(oneWheellambdaKyaTextBox.Text);
            double lambdaCx = double.Parse(oneWheelLambdaCxTextBox.Text);
            double lambdaCy = double.Parse(oneWheelLambdaCyTextBox.Text);
            double lambdaEx = double.Parse(oneWheelLambdaExTextBox.Text);
            double lambdaEy = double.Parse(oneWheelLambdaEyTextBox.Text);
            double lambdaHx = double.Parse(oneWheelLambdaHxTextBox.Text);
            double lambdaHy = double.Parse(oneWheelLambdaHyTextBox.Text);
            double lambdaVx = double.Parse(oneWheelLambdaVxTextBox.Text);
            double lambdaVy = double.Parse(oneWheelLambdaVyTextBox.Text);
            double lambdaKyg = double.Parse(oneWheelLambdaKygTextBox.Text);
            double lambdaKzg = double.Parse(oneWheelLambdaKzgTextBox.Text);
            double lambdat = double.Parse(oneWheelLambdatTextBox.Text);
            double lambdaMr = double.Parse(oneWheelLambdaMrTextBox.Text);
            double lambdaxa = double.Parse(oneWheelLambdaxaTextBox.Text);
            double lambdayk = double.Parse(oneWheelLambdaykTextBox.Text);
            double lambdaVyk = double.Parse(oneWheelLambdaVykTextBox.Text);
            double lambdas = double.Parse(oneWheelLambdasTextBox.Text);
            double lambdaCz = double.Parse(oneWheelLambdaCzTextBox.Text);
            double lambdaMx = double.Parse(oneWheelLambdaMxTextBox.Text);
            double lambdaVMx = double.Parse(oneWheelLambdaVMxTextBox.Text);
            double lambdaMy = double.Parse(oneWheelLambdaMyTextBox.Text);
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
            Vehicle.TireModelMF52 TireModelMF52 = new Vehicle.TireModelMF52(tireModelFile, lambdaList);
            // Initializes a new object
            Vehicle.OneWheel.Tire tireSubSystem = new Vehicle.OneWheel.Tire(tireID, TireModelMF52, verticalStiffness);
            // Adds the object to the listbox and the combobox
            oneWheelTireListBox.Items.Add(tireSubSystem);
        }

        private void OneWheelDeleteTireOfListBox_Click(object sender, RoutedEventArgs e)
        {
            // Checks if there's a listbox item selected and then removes it
            if (oneWheelTireListBox.SelectedItems.Count == 1)
            {
                oneWheelTireListBox.Items.RemoveAt(oneWheelTireListBox.Items.IndexOf(oneWheelTireListBox.SelectedItem));
            }
        }

        private void OneWheelTireListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (oneWheelTireListBox.SelectedItems.Count == 1)
            {
                // Gets the selected object
                Vehicle.OneWheel.Tire tireSubsystem = oneWheelTireListBox.SelectedItem as Vehicle.OneWheel.Tire;
                // Writes the properties in the UI
                oneWheelTireIDTextBox.Text = tireSubsystem.TireID;
                oneWheelTireModelTextBox.Text = tireSubsystem.TireModel.FileLocation;
                oneWheelTireStiffnessTextBox.Text = tireSubsystem.VerticalStiffness.ToString("F3");
                oneWheelLambdaFzOTextBox.Text = tireSubsystem.TireModel.lambdaFzO.ToString("F3");
                oneWheelLambdaMuxTextBox.Text = tireSubsystem.TireModel.lambdaMux.ToString("F3");
                oneWheelLambdaMuyTextBox.Text = tireSubsystem.TireModel.lambdaMuy.ToString("F3");
                oneWheelLambdaMuVTextBox.Text = tireSubsystem.TireModel.lambdaMuV.ToString("F3");
                oneWheelLambdaKxkTextBox.Text = tireSubsystem.TireModel.lambdaKxk.ToString("F3");
                oneWheellambdaKyaTextBox.Text = tireSubsystem.TireModel.lambdaKya.ToString("F3");
                oneWheelLambdaCxTextBox.Text = tireSubsystem.TireModel.lambdaCx.ToString("F3");
                oneWheelLambdaCyTextBox.Text = tireSubsystem.TireModel.lambdaCy.ToString("F3");
                oneWheelLambdaExTextBox.Text = tireSubsystem.TireModel.lambdaEx.ToString("F3");
                oneWheelLambdaEyTextBox.Text = tireSubsystem.TireModel.lambdaEy.ToString("F3");
                oneWheelLambdaHxTextBox.Text = tireSubsystem.TireModel.lambdaHx.ToString("F3");
                oneWheelLambdaHyTextBox.Text = tireSubsystem.TireModel.lambdaHy.ToString("F3");
                oneWheelLambdaVxTextBox.Text = tireSubsystem.TireModel.lambdaVx.ToString("F3");
                oneWheelLambdaVyTextBox.Text = tireSubsystem.TireModel.lambdaVy.ToString("F3");
                oneWheelLambdaKygTextBox.Text = tireSubsystem.TireModel.lambdaKyg.ToString("F3");
                oneWheelLambdaKzgTextBox.Text = tireSubsystem.TireModel.lambdaKzg.ToString("F3");
                oneWheelLambdatTextBox.Text = tireSubsystem.TireModel.lambdat.ToString("F3");
                oneWheelLambdaMrTextBox.Text = tireSubsystem.TireModel.lambdaMr.ToString("F3");
                oneWheelLambdaxaTextBox.Text = tireSubsystem.TireModel.lambdaxa.ToString("F3");
                oneWheelLambdaykTextBox.Text = tireSubsystem.TireModel.lambdayk.ToString("F3");
                oneWheelLambdaVykTextBox.Text = tireSubsystem.TireModel.lambdaVyk.ToString("F3");
                oneWheelLambdasTextBox.Text = tireSubsystem.TireModel.lambdas.ToString("F3");
                oneWheelLambdaCzTextBox.Text = tireSubsystem.TireModel.lambdaCz.ToString("F3");
                oneWheelLambdaMxTextBox.Text = tireSubsystem.TireModel.lambdaMx.ToString("F3");
                oneWheelLambdaVMxTextBox.Text = tireSubsystem.TireModel.lambdaVMx.ToString("F3");
                oneWheelLambdaMyTextBox.Text = tireSubsystem.TireModel.lambdaMy.ToString("F3");
            }
        }

        private void OneWheelTireModelButton_Click(object sender, RoutedEventArgs e)
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
                oneWheelTireModelTextBox.Text = openFileDlg.FileName;
            }
        }
        #endregion
        #region Transmission
        private void OneWheelAddTransmissionToListBox_Click(object sender, RoutedEventArgs e)
        {
            // Gets the sfdatagrid data
            List<Vehicle.OneWheel.GearRatio> gearRatios = new List<Vehicle.OneWheel.GearRatio>();
            Vehicle.OneWheel.GearRatiosViewModel gearRatiosViewModel = oneWheelGearRatiosSfDataGrid.DataContext as Vehicle.OneWheel.GearRatiosViewModel;
            foreach (Vehicle.OneWheel.GearRatio gearRatio in gearRatiosViewModel.GearRatios)
            {
                gearRatios.Add(gearRatio);
            }
            gearRatios.OrderByDescending(gearRatio => gearRatio.Ratio);
            // Gets the rest of the object's data
            string transmissionID = oneWheelTransmissionIDTextBox.Text;
            string transmissionType = oneWheelTransmissionTypeComboBox.Text;
            double primaryRatio = double.Parse(oneWheelPrimaryRatioTextBox.Text);
            double finalRatio = double.Parse(oneWheelFinalRatioTextBox.Text);
            double gearShiftTime = double.Parse(oneWheelGearShiftTimeTextBox.Text);
            double efficiency = double.Parse(oneWheelTransmissionEfficiencyTextBox.Text);
            // Initializes a new object
            Vehicle.OneWheel.Transmission transmission = new Vehicle.OneWheel.Transmission(
                transmissionID, transmissionType, primaryRatio, finalRatio, gearShiftTime, efficiency, gearRatios);
            // Adds the object to the listbox and the combobox
            oneWheelTransmissionListBox.Items.Add(transmission);
        }

        private void OneWheelDeleteTransmissionOfListBox_Click(object sender, RoutedEventArgs e)
        {
            // Checks if there's a listbox item selected and then removes it
            if (oneWheelTransmissionListBox.SelectedItems.Count == 1)
            {
                oneWheelTransmissionListBox.Items.RemoveAt(oneWheelTransmissionListBox.Items.IndexOf(oneWheelTransmissionListBox.SelectedItem));
            }
        }

        private void OneWheelTransmissionListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Checks if there's a listbox item selected and then removes it
            if (oneWheelTransmissionListBox.SelectedItems.Count == 1)
            {
                // Gets the selected object
                Vehicle.OneWheel.Transmission transmission = oneWheelTransmissionListBox.SelectedItem as Vehicle.OneWheel.Transmission;
                // Get and writes the tabular data in the UI
                Vehicle.OneWheel.GearRatiosViewModel gearRatiosViewModel = new Vehicle.OneWheel.GearRatiosViewModel();
                gearRatiosViewModel.GearRatios.Clear();
                foreach (Vehicle.OneWheel.GearRatio gearRatio in transmission.GearRatios)
                {
                    gearRatiosViewModel.GearRatios.Add(gearRatio);
                }
                oneWheelGearRatiosSfDataGrid.DataContext = gearRatiosViewModel;
                // Writes the properties in the UI
                oneWheelTransmissionIDTextBox.Text = transmission.TransmissionID;
                oneWheelTransmissionTypeComboBox.Text = transmission.Type;
                oneWheelPrimaryRatioTextBox.Text = transmission.PrimaryRatio.ToString("F3");
                oneWheelFinalRatioTextBox.Text = transmission.FinalRatio.ToString("F3");
                oneWheelGearShiftTimeTextBox.Text = transmission.GearShiftTime.ToString("F3");
                oneWheelTransmissionEfficiencyTextBox.Text = transmission.Efficiency.ToString("F3");
            }
        }
        #endregion
        #region Aerodynamics
        private void OneWheelAddAerodynamicsToListBox_Click(object sender, RoutedEventArgs e)
        {
            // Gets the sfdatagrid data
            List<Vehicle.OneWheel.AerodynamicMapPoint> aerodynamicMapPoints = new List<Vehicle.OneWheel.AerodynamicMapPoint>();
            Vehicle.OneWheel.AerodynamicMapViewModel aerodynamicMapViewModel = oneWheelAerodynamicMapSfDataGrid.DataContext as Vehicle.OneWheel.AerodynamicMapViewModel;
            foreach (Vehicle.OneWheel.AerodynamicMapPoint aerodynamicMapPoint in aerodynamicMapViewModel.AerodynamicMap)
            {
                aerodynamicMapPoints.Add(aerodynamicMapPoint);
            }
            // Gets the rest of the object's data
            string aerodynamicID = oneWheelAerodynamicsIDTextBox.Text;
            double frontalArea = double.Parse(oneWheelFrontalAreaTextBox.Text);
            double airDensity = double.Parse(oneWheelAirDensityTextBox.Text);
            // Initializes a new object
            Vehicle.OneWheel.Aerodynamics aerodynamics = new Vehicle.OneWheel.Aerodynamics(aerodynamicID, aerodynamicMapPoints, frontalArea, airDensity);
            // Adds the object to the listbox and the combobox
            oneWheelAerodynamicsListBox.Items.Add(aerodynamics);
        }

        private void OneWheelDeleteAerodynamicsOfListBox_Click(object sender, RoutedEventArgs e)
        {
            // Checks if there's a listbox item selected and then removes it
            if (oneWheelAerodynamicsListBox.SelectedItems.Count == 1)
            {
                oneWheelAerodynamicsListBox.Items.RemoveAt(oneWheelAerodynamicsListBox.Items.IndexOf(oneWheelAerodynamicsListBox.SelectedItem));
            }
        }

        private void OneWheelAerodynamicsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Checks if there's a listbox item selected
            if (oneWheelAerodynamicsListBox.SelectedItems.Count == 1)
            {
                // Gets the selected object
                Vehicle.OneWheel.Aerodynamics aerodynamics = oneWheelAerodynamicsListBox.SelectedItem as Vehicle.OneWheel.Aerodynamics;
                // Get and writes the tabular data in the UI
                Vehicle.OneWheel.AerodynamicMapViewModel aerodynamicMapViewModel = new Vehicle.OneWheel.AerodynamicMapViewModel();
                aerodynamicMapViewModel.AerodynamicMap.Clear();
                foreach (Vehicle.OneWheel.AerodynamicMapPoint aerodynamicMapPoint in aerodynamics.AerodynamicMapPoints)
                {
                    aerodynamicMapViewModel.AerodynamicMap.Add(aerodynamicMapPoint);
                }
                oneWheelGearRatiosSfDataGrid.DataContext = aerodynamicMapViewModel;
                // Writes the properties in the UI
                oneWheelAerodynamicsIDTextBox.Text = aerodynamics.AeroID;
                oneWheelFrontalAreaTextBox.Text = aerodynamics.FrontalArea.ToString("F3");
                oneWheelAirDensityTextBox.Text = aerodynamics.AirDensity.ToString("F3");
            }
        }
        #endregion
        #region Engine
        private void OneWheelAddEngineToListBox_Click(object sender, RoutedEventArgs e)
        {
            // Gets the sfdatagrid data
            List<Vehicle.OneWheel.EngineCurvesPoint> engineCurvesPoints = new List<Vehicle.OneWheel.EngineCurvesPoint>();
            Vehicle.OneWheel.EngineCurvesViewModel engineCurvesViewModel = oneWheelEngineCurvesSfDataGrid.DataContext as Vehicle.OneWheel.EngineCurvesViewModel;
            foreach (Vehicle.OneWheel.EngineCurvesPoint engineCurvesPoint in engineCurvesViewModel.EngineCurves)
            {
                engineCurvesPoints.Add(engineCurvesPoint);
            }
            engineCurvesPoints.OrderBy(engineCurvesPoint => engineCurvesPoint.RPM);
            // Gets the rest of the object's data
            string engineID = oneWheelEngineIDTextBox.Text;
            double maxThrottle = double.Parse(oneWheelMaxThrottleTextBox.Text);
            double fuelDensity = double.Parse(oneWheelFuelDensityTextBox.Text);
            // Initializes a new object
            Vehicle.OneWheel.Engine engine = new Vehicle.OneWheel.Engine(engineID, engineCurvesPoints, maxThrottle, fuelDensity);
            // Adds the object to the listbox and the combobox
            oneWheelEngineListBox.Items.Add(engine);
        }

        private void OneWheelDeleteEngineOfListBox_Click(object sender, RoutedEventArgs e)
        {
            // Checks if there's a listbox item selected and then removes it
            if (oneWheelEngineListBox.SelectedItems.Count == 1)
            {
                oneWheelEngineListBox.Items.RemoveAt(oneWheelEngineListBox.Items.IndexOf(oneWheelEngineListBox.SelectedItem));
            }
        }

        private void OneWheelEngineListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Checks if there's a listbox item selected
            if (oneWheelEngineListBox.SelectedItems.Count == 1)
            {
                // Gets the selected object
                Vehicle.OneWheel.Engine engine = oneWheelEngineListBox.SelectedItem as Vehicle.OneWheel.Engine;
                // Get and writes the tabular data in the UI
                Vehicle.OneWheel.EngineCurvesViewModel engineCurvesViewModel = new Vehicle.OneWheel.EngineCurvesViewModel();
                engineCurvesViewModel.EngineCurves.Clear();
                foreach (Vehicle.OneWheel.EngineCurvesPoint engineCurvesPoint in engine.EngineCurves)
                {
                    engineCurvesViewModel.EngineCurves.Add(engineCurvesPoint);
                }
                oneWheelEngineCurvesSfDataGrid.DataContext = engineCurvesViewModel;
                // Writes the properties in the UI
                oneWheelEngineIDTextBox.Text = engine.EngineID;
                oneWheelMaxThrottleTextBox.Text = engine.MaxThrottle.ToString("F3");
                oneWheelFuelDensityTextBox.Text = engine.FuelDensity.ToString("F3");
            }
        }
        #endregion
        #endregion

        #region Path Input Methods

        #region Tabular
        private void AddTabularPathToListboxButton_Click(object sender, RoutedEventArgs e)
        {
            // Gets the sfdatagrids data
            List<TabularPathSection> tabularPathSections = new List<TabularPathSection>();
            TabularPathSectionViewModel tabularPathSectionsViewModel = tabularPathSectionsSfDataGrid.DataContext as TabularPathSectionViewModel;
            foreach (TabularPathSection tabularPathSection in tabularPathSectionsViewModel.TabularPathSections)
            {
                tabularPathSections.Add(tabularPathSection);
            }
            List<PathSector> pathSectors = new List<PathSector>();
            PathSectorViewModel pathSectorViewModel = tabularPathSectorsSfDataGrid.DataContext as PathSectorViewModel;
            foreach (PathSector pathSector in pathSectorViewModel.PathSectors)
            {
                pathSectors.Add(pathSector);
            }
            // Gets the rest of the object's data
            string pathID = tabularPathIDTextBox.Text;
            int resolution = int.Parse(tabularPathResolutionTextBox.Text);
            // Initializes a new object
            Path path = new Path(pathID, resolution, pathSectors, tabularPathSections);
            // Adds the object to the listbox and the combobox
            tabularPathListBox.Items.Add(path);
        }

        private void DeleteTabularPathOfListboxButton_Click(object sender, RoutedEventArgs e)
        {
            // Checks if there's a listbox item selected and then removes it
            if (tabularPathListBox.SelectedItems.Count == 1)
            {
                tabularPathListBox.Items.RemoveAt(tabularPathListBox.Items.IndexOf(tabularPathListBox.SelectedItem));
            }
        }

        private void TabularPathListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Checks if there's a listbox item selected
            if (tabularPathListBox.SelectedItems.Count == 1)
            {
                // Gets the selected object
                Path path = tabularPathListBox.SelectedItem as Path;
                // Get and writes the tabular data in the UI
                TabularPathSectionViewModel tabularPathSectionViewModel = new TabularPathSectionViewModel();
                tabularPathSectionViewModel.TabularPathSections.Clear();
                foreach (TabularPathSection tabularPathSection in path.TabularSections)
                {
                    tabularPathSectionViewModel.TabularPathSections.Add(tabularPathSection);
                }
                tabularPathSectionsSfDataGrid.DataContext = tabularPathSectionViewModel;
                PathSectorViewModel pathSectorViewModel = new PathSectorViewModel();
                pathSectorViewModel.PathSectors.Clear();
                foreach (PathSector pathSector in path.Sectors)
                {
                    pathSectorViewModel.PathSectors.Add(pathSector);
                }
                tabularPathSectorsSfDataGrid.DataContext = pathSectorViewModel;
                // Writes the properties in the UI
                tabularPathIDTextBox.Text = path.PathID;
                tabularPathResolutionTextBox.Text = path.Resolution.ToString();

                // Removes the current chart
                tabularPathVisualizationGrid.Children.Clear();
                // Initializes the chart
                SfChart chart = new SfChart() { IsManipulationEnabled = true, Header = path.PathID, FontSize = 30 };
                // Add zooming/panning feature
                ChartZoomPanBehavior zooming = new ChartZoomPanBehavior()
                {
                    EnableZoomingToolBar = true,
                    ZoomRelativeToCursor = true
                };
                chart.Behaviors.Add(zooming);
                // Initializes the chart's axis
                NumericalAxis xAxis = new NumericalAxis() { Header = "X Coordinates [m]" };
                NumericalAxis yAxis = new NumericalAxis() { Header = "Y Coordinates [m]" };
                // Adds the axis to the chart
                chart.PrimaryAxis = xAxis;
                chart.SecondaryAxis = yAxis;
                // Initializes the path view model
                PathViewModel viewModel = new PathViewModel(path);
                // Initializes the chart data
                FastLineSeries series = new FastLineSeries()
                {
                    ItemsSource = viewModel.PathPoints,
                    XBindingPath = "CoordinateX",
                    YBindingPath = "CoordinateY",
                    StrokeThickness = 10
                };
                // Adds the series to the chart
                chart.Series.Add(series);
                // Adds the chart to the UI
                tabularPathVisualizationGrid.Children.Add(chart);
            }
        }

        #endregion
        #endregion

        #region GGV Diagram Input Methods
        private void AddGGVDiagramToListboxButton_Click(object sender, RoutedEventArgs e)
        {
            if (GGVDiagramCarAndSetupCombobox.Text!=null &&
                GGVDiagramAmountOfPointsPerSpeedTextBox.Text!="0" &&
                GGVDiagramAmountOfSpeedsTextBox.Text!="0" &&
                double.Parse(GGVDiagramLowestSpeedTextBox.Text)<= double.Parse(GGVDiagramHighestSpeedTextBox.Text))
            {
                // Gets the object's properties values
                Vehicle.OneWheel.Car car = GGVDiagramCarAndSetupCombobox.SelectedItem as Vehicle.OneWheel.Car;
                int amountOfPointsPerSpeed = int.Parse(GGVDiagramAmountOfPointsPerSpeedTextBox.Text);
                int amountOfSpeeds = int.Parse(GGVDiagramAmountOfSpeedsTextBox.Text);
                double lowestSpeed = double.Parse(GGVDiagramLowestSpeedTextBox.Text);
                double highestSpeed = double.Parse(GGVDiagramHighestSpeedTextBox.Text);
                // Initializes a new object
                Simulation.GGVDiagram diagram = new Simulation.GGVDiagram(car, amountOfPointsPerSpeed, amountOfSpeeds, lowestSpeed, highestSpeed);
                // Adds the object to the listbox
                GGVDiagramListBox.Items.Add(diagram);
            }
        }

        private void DeleteGGVDiagramOfListboxButton_Click(object sender, RoutedEventArgs e)
        {
            // Checks if there's a listbox item selected and then removes it
            if (GGVDiagramListBox.SelectedItems.Count == 1)
            {
                GGVDiagramListBox.Items.RemoveAt(GGVDiagramListBox.Items.IndexOf(GGVDiagramListBox.SelectedItem));
            }
        }

        private void GGVDiagramListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GGVDiagramListBox.SelectedItems.Count == 1)
            {
                // Gets the selected object
                Simulation.GGVDiagram diagram = GGVDiagramListBox.SelectedItem as Simulation.GGVDiagram;
                // Writes the properties in the UI
                GGVDiagramCarAndSetupCombobox.Text = diagram.Car.ToString();
                GGVDiagramAmountOfPointsPerSpeedTextBox.Text = diagram.AmountOfPointsPerSpeed.ToString();
                GGVDiagramAmountOfSpeedsTextBox.Text = diagram.AmountOfSpeeds.ToString();
                GGVDiagramLowestSpeedTextBox.Text = diagram.LowestSpeed.ToString();
                GGVDiagramHighestSpeedTextBox.Text = diagram.HighestSpeed.ToString();
                // Removes the current surface
                GGVDiagramVisualizationGrid.Children.Clear();
                // Sets up a new surface for GGV visualization
                SfSurfaceChart surface = new SfSurfaceChart()
                {
                    EnableRotation = true,
                    EnableZooming = true,
                    Header = "GGV Diagram",
                    FontSize = 30,
                    Type = SurfaceType.WireframeSurface
                };
                // Sets up the surface data
                Simulation.GGVDiagramViewModel viewModel = new Simulation.GGVDiagramViewModel(diagram);
                foreach (Simulation.GGVDiagramPoint point in viewModel.GGVDiagramPoints)
                {
                    surface.Data.AddPoints(point.LongitudinalAcceleration, point.Speed, point.LateralAcceleration);
                }
                surface.RowSize = viewModel.RowSize;
                surface.ColumnSize = viewModel.ColumnSize;
                
                // Sets up the x axis 
                SurfaceAxis xAxis = new SurfaceAxis
                {
                    Header = "Longitudinal Acceleration [G]",
                    Minimum = surface.Data.XValues.Min(),
                    Maximum = surface.Data.XValues.Max(),
                    LabelFormat = "0.00",
                    FontSize = 15
                };
                surface.XAxis = xAxis;

                // Sets up the y axis
                SurfaceAxis yAxis = new SurfaceAxis
                {
                    Header = "Speed [km/h]",
                    Minimum = surface.Data.YValues.Min(),
                    Maximum = surface.Data.YValues.Max(),
                    LabelFormat = "0.00",
                    FontSize = 15
                };
                surface.YAxis = yAxis;

                // Sets up the z axis
                SurfaceAxis zAxis = new SurfaceAxis
                {
                    Header = "Lateral Acceleration [G]",
                    Minimum = surface.Data.ZValues.Min(),
                    Maximum = surface.Data.ZValues.Max(),
                    LabelFormat = "0.00",
                    FontSize = 15
                };
                surface.ZAxis = zAxis;
                // Adds a colorbar
                surface.ColorBar = new ChartColorBar() { DockPosition = ChartDock.Right };
                // Adds the surface to the UI
                GGVDiagramVisualizationGrid.Children.Add(surface);
            }
        }
        #endregion

        #region Lap Time Simulation Inputs Methods
        private void AddLapTimeSimulationToListboxButton_Click(object sender, RoutedEventArgs e)
        {
            if (LapTimeSimulationModeCombobox.Text!=null && 
                LapTimeSimulationPathCombobox.Text!=null &&
                LapTimeSimulationGGVDiagramCombobox.SelectedItem!=null)
            {
                // Gets the object's properties values
                Simulation.GGVDiagram ggvDiagram = LapTimeSimulationGGVDiagramCombobox.SelectedItem as Simulation.GGVDiagram;
                Path path = LapTimeSimulationPathCombobox.SelectedItem as Path;
                string simulationMode = LapTimeSimulationModeCombobox.Text;
                bool isFirstLap = false;
                if (simulationMode == "First Lap") isFirstLap = true;
                // Initializes a new object
                Simulation.LapTimeSimulation lapTimeSimulation = new Simulation.LapTimeSimulation(ggvDiagram, path, isFirstLap);
                // Adds the object to the listbox
                lapTimeSimulationListBox.Items.Add(lapTimeSimulation);
            }
        }

        private void DeleteLapTimeSimulationOfListboxButton_Click(object sender, RoutedEventArgs e)
        {
            // Checks if there's a listbox item selected and then removes it
            if (lapTimeSimulationListBox.SelectedItems.Count == 1)
            {
                lapTimeSimulationListBox.Items.RemoveAt(lapTimeSimulationListBox.Items.IndexOf(lapTimeSimulationListBox.SelectedItem));
            }
        }

        private void LapTimeSimulationListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (lapTimeSimulationListBox.SelectedItems.Count == 1)
            {
                // Gets the selected object
                Simulation.LapTimeSimulation lapTimeSimulation = lapTimeSimulationListBox.SelectedItem as Simulation.LapTimeSimulation;
                // Writes the properties in the UI
                LapTimeSimulationGGVDiagramCombobox.Text = lapTimeSimulation.SimulationGGVDiagram.ToString();
                LapTimeSimulationPathCombobox.Text = lapTimeSimulation.SimulationPath.ToString();
                if (lapTimeSimulation.IsFirstLap) LapTimeSimulationModeCombobox.Text = "First Lap";
                else LapTimeSimulationModeCombobox.Text = "Normal Lap";
            }
        }
        #endregion

        #region Result Analysis Methods
        #region Lap Time Simulation
        private void ResultsAnalysisLapTimeSimulationXAxisDataCombobox_DropDownClosed(object sender, EventArgs e)
        {
            // Checks if there are items are selected
            if (resultsAnalysisLapTimeSimulationChecklistbox.SelectedItems.Count > 0 &&
                resultsAnalysisLapTimeSimulationXAxisDataCombobox.SelectedItem != null &&
                resultsAnalysisLapTimeSimulationYAxisDataCombobox.SelectedItem != null)
            {
                // Initializes a new chart
                SfChart chart = new SfChart() { Header = "Lap Time Simulation Analysis" };
                // Add zooming/panning feature
                ChartZoomPanBehavior zooming = new ChartZoomPanBehavior()
                {
                    EnableZoomingToolBar = true,
                    ZoomRelativeToCursor = true
                };
                chart.Behaviors.Add(zooming);
                // Initializes the chart's axis
                NumericalAxis xAxis = new NumericalAxis();
                NumericalAxis yAxis = new NumericalAxis();
                // Adds the axis to the chart
                chart.PrimaryAxis = xAxis;
                chart.SecondaryAxis = yAxis;
                Results.LapTimeSimulation viewModel;
                // Sweeps through the simulation check list box to add the series to the chart
                foreach (Simulation.LapTimeSimulation lapTimeSimulation in resultsAnalysisLapTimeSimulationChecklistbox.SelectedItems)
                {
                    // Initializes the lap tim simulation view model
                    viewModel = new Results.LapTimeSimulation(lapTimeSimulation);
                    // Fast line data series initialization
                    FastLineSeries series = new FastLineSeries()
                    {
                        ItemsSource = viewModel.Results
                    };
                    // Selects the axis binding sources based on the axis comboboxes
                    switch (resultsAnalysisLapTimeSimulationXAxisDataCombobox.Text)
                    {
                        case "Time":
                            series.XBindingPath = "ElapsedTime";
                            chart.PrimaryAxis.Header = "Time [s]";
                            break;
                        case "Distance":
                            series.XBindingPath = "ElapsedDistance";
                            chart.PrimaryAxis.Header = "Distance [m]";
                            break;
                        case "Speed":
                            series.XBindingPath = "Speed";
                            chart.PrimaryAxis.Header = "Speed [km/h]";
                            break;
                        case "Longitudinal Acceleration":
                            series.XBindingPath = "LongitudinalAcceleration";
                            chart.PrimaryAxis.Header = "Longitudinal Acceleration [G]";
                            break;
                        case "Lateral Acceleration":
                            series.XBindingPath = "LateralAcceleration";
                            chart.PrimaryAxis.Header = "Lateral Acceleration [G]";
                            break;
                        case "Gear":
                            series.XBindingPath = "GearNumber";
                            chart.PrimaryAxis.Header = "Gear Number";
                            break;
                        default:
                            break;
                    }
                    switch (resultsAnalysisLapTimeSimulationYAxisDataCombobox.Text)
                    {
                        case "Time":
                            series.YBindingPath = "ElapsedTime";
                            chart.SecondaryAxis.Header = "Time [s]";
                            break;
                        case "Distance":
                            series.YBindingPath = "ElapsedDistance";
                            chart.SecondaryAxis.Header = "Distance [m]";
                            break;
                        case "Speed":
                            series.YBindingPath = "Speed";
                            chart.SecondaryAxis.Header = "Speed [km/h]";
                            break;
                        case "Longitudinal Acceleration":
                            series.YBindingPath = "LongitudinalAcceleration";
                            chart.SecondaryAxis.Header = "Longitudinal Acceleration [G]";
                            break;
                        case "Lateral Acceleration":
                            series.YBindingPath = "LateralAcceleration";
                            chart.SecondaryAxis.Header = "Lateral Acceleration [G]";
                            break;
                        case "Gear":
                            series.YBindingPath = "GearNumber";
                            chart.SecondaryAxis.Header = "Gear Number";
                            break;
                        default:
                            break;
                    }
                    // Adds the series to the chart
                    chart.Series.Add(series);
                }
                // Adds legend to the chart
                chart.Legend = new ChartLegend();
                // Clears grid
                resultsAnalysisLapTimeSimulationChartGrid.Children.Clear();
                // Adds the chart to the grid
                resultsAnalysisLapTimeSimulationChartGrid.Children.Add(chart);
            }

        }
        #endregion

        #endregion


    }
}
