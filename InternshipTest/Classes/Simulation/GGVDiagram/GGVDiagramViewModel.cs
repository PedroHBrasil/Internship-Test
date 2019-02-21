using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternshipTest.Simulation
{
    class GGVDiagramViewModel
    {
        // Properties
        public ObservableCollection<GGVDiagramPoint> GGVDiagramPoints { get; set; }
        public int RowSize { get; set; }
        public int ColumnSize { get; set; }
        // Construtor
        public GGVDiagramViewModel(GGVDiagram diagram)
        {
            RowSize = diagram.Speeds.Length;
            ColumnSize = diagram.GGDiagrams[0].LongitudinalAccelerations.Count + 1;
            GGVDiagramPoints = new ObservableCollection<GGVDiagramPoint>();
            // Sweeps the ggv diagram
            for (int iSpeed = 0; iSpeed < RowSize; iSpeed++)
            {
                // Gets the current speed
                double currentSpeed = diagram.Speeds[iSpeed];
                // Sweeps the current gg diagram
                for (int iPoint = 0; iPoint < ColumnSize-1; iPoint++)
                {
                    // Gets the current accelerations
                    double currentLongitudinalAcceleration = diagram.GGDiagrams[iSpeed].LongitudinalAccelerations[iPoint];
                    double currentLateralAcceleration = diagram.GGDiagrams[iSpeed].LateralAccelerations[iPoint];
                    // Adds the current point to the view model
                    GGVDiagramPoints.Add(new GGVDiagramPoint(currentLongitudinalAcceleration, currentSpeed, currentLateralAcceleration));
                }
                GGVDiagramPoints.Add(new GGVDiagramPoint(diagram.GGDiagrams[iSpeed].LongitudinalAccelerations[0],
                    currentSpeed, diagram.GGDiagrams[iSpeed].LateralAccelerations[0]));
            }

        }
    }
    class GGVDiagramPoint
    {
        // Properties
        public double LongitudinalAcceleration { get; set; }
        public double Speed { get; set; }
        public double LateralAcceleration { get; set; }
        // Constructor
        public GGVDiagramPoint(double longitudinalAcceleration, double speed, double lateralAcceleration)
        {
            LongitudinalAcceleration = longitudinalAcceleration;
            Speed = speed;
            LateralAcceleration = lateralAcceleration;
        }
    }
}
