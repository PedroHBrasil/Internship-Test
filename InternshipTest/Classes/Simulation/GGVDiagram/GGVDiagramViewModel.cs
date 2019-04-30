using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternshipTest.Simulation
{
    /// <summary>
    /// Used to display a GGV diagram.
    /// </summary>
    public class GGVDiagramViewModel
    {
        #region Properties
        /// <summary>
        /// Points of the GGV diagram [longitudinal acceleration, speed, lateral acceleration].
        /// </summary>
        public ObservableCollection<GGVDiagramPoint> GGVDiagramPoints { get; set; }
        /// <summary>
        /// Size of the matrix's row to generate the surface (amount of speeds).
        /// </summary>
        public int RowSize { get; set; }
        /// <summary>
        /// Size of the matrix's column to generate the surface (amount of longitudinal accelerations + 1 to close the surface)
        /// </summary>
        public int ColumnSize { get; set; }
        #endregion
        #region Construtor
        public GGVDiagramViewModel(OneWheelGGVDiagram diagram)
        {
            RowSize = diagram.Speeds.Length;
            ColumnSize = diagram.GGDiagrams[0].LongitudinalAccelerations.Count + 1;
            GGVDiagramPoints = new ObservableCollection<GGVDiagramPoint>();
            // Sweeps the ggv diagram
            for (int iSpeed = 0; iSpeed < RowSize; iSpeed++)
            {
                // Gets the current speed
                double currentSpeed = diagram.Speeds[iSpeed] * 3.6;
                // Sweeps the current gg diagram
                for (int iPoint = 0; iPoint < ColumnSize-1; iPoint++)
                {
                    // Gets the current accelerations
                    double currentLongitudinalAcceleration = diagram.GGDiagrams[iSpeed].LongitudinalAccelerations[iPoint] / diagram.Car.Inertia.Gravity;
                    double currentLateralAcceleration = diagram.GGDiagrams[iSpeed].LateralAccelerations[iPoint] / diagram.Car.Inertia.Gravity;
                    // Adds the current point to the view model
                    GGVDiagramPoints.Add(new GGVDiagramPoint(currentLongitudinalAcceleration, currentSpeed, currentLateralAcceleration));
                }
                GGVDiagramPoints.Add(new GGVDiagramPoint(diagram.GGDiagrams[iSpeed].LongitudinalAccelerations[0] / diagram.Car.Inertia.Gravity,
                    currentSpeed, diagram.GGDiagrams[iSpeed].LateralAccelerations[0] / diagram.Car.Inertia.Gravity));
            }
        }
        #endregion
    }
    public class GGVDiagramPoint
    {
        #region Properties
        /// <summary>
        /// Longitudinal acceleration [G].
        /// </summary>
        public double LongitudinalAcceleration { get; set; }
        /// <summary>
        /// Car speed [km/h].
        /// </summary>
        public double Speed { get; set; }
        /// <summary>
        /// Lateral acceleration [G].
        /// </summary>
        public double LateralAcceleration { get; set; }
        #endregion
        #region Constructor
        public GGVDiagramPoint(double longitudinalAcceleration, double speed, double lateralAcceleration)
        {
            LongitudinalAcceleration = longitudinalAcceleration;
            Speed = speed;
            LateralAcceleration = lateralAcceleration;
        }
        #endregion
    }
}
