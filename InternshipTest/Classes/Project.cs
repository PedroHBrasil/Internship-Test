using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace InternshipTest
{
    [Serializable]
    public class Project
    {
        #region Properties
        // Aerodynamics
        public List<Vehicle.OneWheelAerodynamics> OneWheelAerodynamics { get; set; }
        public List<Vehicle.OneWheelAerodynamicMap> OneWheelAerodynamicMaps { get; set; }
        public List<Vehicle.OneWheelAerodynamicMapPoint> OneWheelAerodynamicMapPoints { get; set; }
        public List<Vehicle.TwoWheelAerodynamics> TwoWheelAerodynamics { get; set; }
        public List<Vehicle.TwoWheelAerodynamicMap> TwoWheelAerodynamicMaps { get; set; }
        public List<Vehicle.TwoWheelAerodynamicMapPoint> TwoWheelAerodynamicMapPoints { get; set; }
        // Brakes
        public List<Vehicle.OneWheelBrakes> OneWheelBrakes { get; set; }
        public List<Vehicle.TwoWheelBrakes> TwoWheelBrakes { get; set; }
        // Engine
        public List<Vehicle.Engine> Engines { get; set; }
        public List<Vehicle.EngineCurves> EngineCurves { get; set; }
        public List<Vehicle.EngineCurvesPoint> EngineCurvesPoints { get; set; }
        // Inertia
        public List<Vehicle.Inertia> Inertias { get; set; }
        public List<Vehicle.TwoWheelInertiaAndDimensions> TwoWheelInertiaAndDimensions { get; set; }
        // Suspension And Steering
        public List<Vehicle.SimplifiedSuspension> SimplifiedSuspensions { get; set; }
        public List<Vehicle.SteeringSystem> SteeringSystems { get; set; }
        // Tires
        public List<Vehicle.Tire> Tires { get; set; }
        public List<Vehicle.TireModelMF52> TireModelMF52s { get; set; }
        public List<Vehicle.TireModelMF52Point> TireModelMF52Points { get; set; }
        // Transmission
        public List<Vehicle.OneWheelTransmission> OneWheelTransmissions { get; set; }
        public List<Vehicle.TwoWheelTransmission> TwoWheelTransmissions { get; set; }
        public List<Vehicle.GearRatiosSet> GearRatiosSets { get; set; }
        public List<Vehicle.GearRatio> GearRatios { get; set; }
        // One Wheel Model Cars
        public List<Vehicle.OneWheelCar> OneWheelCars { get; set; }
        // Two Wheel Model Cars
        public List<Vehicle.TwoWheelCar> TwoWheelCars { get; set; }
        // Four Wheel Model Cars
        // Tabular Paths
        public List<Path> TabularPaths { get; set; }
        public List<TabularPathSectionsSet> TabularPathSectionsSets { get; set; }
        public List<TabularPathSection> TabularPathSections { get; set; }
        public List<PathSectorsSet> TabularPathSectorsSets { get; set; }
        public List<PathSector> TabularPathSectors { get; set; }
        // Drawn Paths
        // Optimization Paths
        // GGV Diagrams
        public List<Simulation.GGVDiagram> GGVDiagrams { get; set; }
        // Lap Time Simulations
        public List<Simulation.LapTimeSimulation> LapTimeSimulations { get; set; }
        // Lap Time Simulation Results
        public List<Results.LapTimeSimulationResults> LapTimeSimulationResults { get; set; }
        #endregion
        #region Constructors
        public Project()
        {
            // Aerodynamics
            OneWheelAerodynamics = new List<Vehicle.OneWheelAerodynamics>();
            OneWheelAerodynamicMaps = new List<Vehicle.OneWheelAerodynamicMap>();
            OneWheelAerodynamicMapPoints = new List<Vehicle.OneWheelAerodynamicMapPoint>();
            TwoWheelAerodynamics = new List<Vehicle.TwoWheelAerodynamics>();
            TwoWheelAerodynamicMaps = new List<Vehicle.TwoWheelAerodynamicMap>();
            TwoWheelAerodynamicMapPoints = new List<Vehicle.TwoWheelAerodynamicMapPoint>();
            // Brakes
            OneWheelBrakes = new List<Vehicle.OneWheelBrakes>();
            TwoWheelBrakes = new List<Vehicle.TwoWheelBrakes>();
            // Engine
            Engines = new List<Vehicle.Engine>();
            EngineCurves = new List<Vehicle.EngineCurves>();
            EngineCurvesPoints = new List<Vehicle.EngineCurvesPoint>();
            // Inertia
            Inertias = new List<Vehicle.Inertia>();
            TwoWheelInertiaAndDimensions = new List<Vehicle.TwoWheelInertiaAndDimensions>();
            // Suspension And Steering
            SimplifiedSuspensions = new List<Vehicle.SimplifiedSuspension>();
            SteeringSystems = new List<Vehicle.SteeringSystem>();
            // Tires
            Tires = new List<Vehicle.Tire>();
            TireModelMF52s = new List<Vehicle.TireModelMF52>();
            TireModelMF52Points = new List<Vehicle.TireModelMF52Point>();
            // Transmission
            OneWheelTransmissions = new List<Vehicle.OneWheelTransmission>();
            TwoWheelTransmissions = new List<Vehicle.TwoWheelTransmission>();
            GearRatiosSets = new List<Vehicle.GearRatiosSet>();
            GearRatios = new List<Vehicle.GearRatio>();
            // One Wheel Car
            OneWheelCars = new List<Vehicle.OneWheelCar>();
            // Two Wheel Car
            TwoWheelCars = new List<Vehicle.TwoWheelCar>();
            // Four Wheel Car
            // Tabular Path
            TabularPaths = new List<Path>();
            TabularPathSectionsSets = new List<TabularPathSectionsSet>();
            TabularPathSections = new List<TabularPathSection>();
            TabularPathSectorsSets = new List<PathSectorsSet>();
            TabularPathSectors = new List<PathSector>();
            // Drawn Paths
            // Optimization Paths
            // GGV Diagrams
            GGVDiagrams = new List<Simulation.GGVDiagram>();
            // Lap Time Simulations
            LapTimeSimulations = new List<Simulation.LapTimeSimulation>();
            // Lap Time Simulation results
            LapTimeSimulationResults = new List<Results.LapTimeSimulationResults>();
        }

        #endregion
        #region Methods
        public void Save(string filePath)
        {
            // Initializes the writer
            XmlSerializer writer = new XmlSerializer(GetType());
            // Initializes the file stream writer
            StreamWriter writingFile = new StreamWriter(filePath);
            // Writes to the file
            writer.Serialize(writingFile, this);
            // Closes the stream writer
            writingFile.Close();
        }

        public Project Load(string filePath)
        {
            if (File.Exists(filePath))
            {
                try
                {
                    // Initializes the reader
                    XmlSerializer reader = new XmlSerializer(GetType());
                    // Initializes the file stream reader
                    StreamReader file = new StreamReader(filePath);
                    // Gets the loaded project object
                    Project project = (Project)reader.Deserialize(file);
                    file.Close();

                    return project;
                }
                catch (Exception)
                {
                    System.Windows.MessageBox.Show(
                   "Could not load project file. Plase, check if the chosen file is a project file.",
                   "Error",
                   MessageBoxButton.OK,
                   MessageBoxImage.Error);
                   return new Project();
                }
            }
            else return new Project();
        }

        #endregion
    }
}
