using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace InternshipTest
{
    [Serializable]
    public class Project
    {
        #region Properties
        // One Wheel Model Cars
        public List<Vehicle.OneWheel.Car> OneWheelCars { get; set; }
        public List<Vehicle.OneWheel.Inertia> OneWheelInertias { get; set; }
        public List<Vehicle.OneWheel.Suspension> OneWheelSuspensions { get; set; }
        public List<Vehicle.OneWheel.Brakes> OneWheelBrakes { get; set; }
        public List<Vehicle.OneWheel.Tire> OneWheelTires { get; set; }
        public List<Vehicle.OneWheel.Transmission> OneWheelTransmissions { get; set; }
        public List<Vehicle.OneWheel.Aerodynamics> OneWheelAerodynamics { get; set; }
        public List<Vehicle.OneWheel.Engine> OneWheelEngines { get; set; }
        // Two Wheel Model Cars
        // Four Wheel Model Cars
        // Tabular Paths
        public List<Path> TabularPaths { get; set; }
        // Drawn Paths
        // Optimization Paths
        // GGV Diagrams
        public List<Simulation.GGVDiagram> GGVDiagrams { get; set; }
        // Lap Time Simulations
        public List<Simulation.LapTimeSimulation> LapTimeSimulations { get; set; }
        #endregion
        #region Constructors
        public Project() { }
        
        public Project(List<Vehicle.OneWheel.Car> oneWheelCars, List<Vehicle.OneWheel.Inertia> oneWheelInertias,
            List<Vehicle.OneWheel.Suspension> oneWheelSuspensions, List<Vehicle.OneWheel.Brakes> oneWheelBrakes,
            List<Vehicle.OneWheel.Tire> oneWheelTires, List<Vehicle.OneWheel.Transmission> oneWheelTransmissions,
            List<Vehicle.OneWheel.Aerodynamics> oneWheelAerodynamics, List<Vehicle.OneWheel.Engine> oneWheelEngines,
            List<Path> tabularPaths, List<Simulation.GGVDiagram> ggvDiagrams,
            List<Simulation.LapTimeSimulation> lapTimeSimulations)
        {
            OneWheelCars = oneWheelCars;
            OneWheelInertias = oneWheelInertias;
            OneWheelSuspensions = oneWheelSuspensions;
            OneWheelBrakes = oneWheelBrakes;
            OneWheelTires = oneWheelTires;
            OneWheelTransmissions = oneWheelTransmissions;
            OneWheelAerodynamics = oneWheelAerodynamics;
            OneWheelEngines = oneWheelEngines;
            TabularPaths = tabularPaths;
            GGVDiagrams = ggvDiagrams;
            LapTimeSimulations = lapTimeSimulations;
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
                // Initializes the reader
                XmlSerializer reader = new XmlSerializer(GetType());
                // Initializes the file stream reader
                StreamReader file = new StreamReader(filePath);
                // Gets the loaded project object
                Project project = (Project)reader.Deserialize(file);
                file.Close();

                return project;
            }
            else return new Project();
        }

        #endregion
    }
}
