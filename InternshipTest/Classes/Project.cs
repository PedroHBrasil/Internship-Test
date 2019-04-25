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
        public List<Vehicle.Car> OneWheelCars { get; set; }
        public List<Vehicle.Inertia> OneWheelInertias { get; set; }
        public List<Vehicle.Suspension> OneWheelSuspensions { get; set; }
        public List<Vehicle.Brakes> OneWheelBrakes { get; set; }
        public List<Vehicle.Tire> OneWheelTires { get; set; }
        public List<Vehicle.Transmission> OneWheelTransmissions { get; set; }
        public List<Vehicle.Aerodynamics> OneWheelAerodynamics { get; set; }
        public List<Vehicle.Engine> OneWheelEngines { get; set; }
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
        
        public Project(List<Vehicle.Car> oneWheelCars, List<Vehicle.Inertia> oneWheelInertias,
            List<Vehicle.Suspension> oneWheelSuspensions, List<Vehicle.Brakes> oneWheelBrakes,
            List<Vehicle.Tire> oneWheelTires, List<Vehicle.Transmission> oneWheelTransmissions,
            List<Vehicle.Aerodynamics> oneWheelAerodynamics, List<Vehicle.Engine> oneWheelEngines,
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
