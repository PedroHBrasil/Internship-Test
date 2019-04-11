using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternshipTest
{
    /// <summary>
    /// Used to create the paths charts.
    /// </summary>
    public class PathViewModel
    {
        #region Properties
        /// <summary>
        /// Collection used to display.
        /// </summary>
        public ObservableCollection<PathPoint> PathPoints { get; set; }
        #endregion
        #region Contructors
        public PathViewModel() { PathPoints = new ObservableCollection<PathPoint>(); }
        public PathViewModel(Path path)
        {
            PathPoints = new ObservableCollection<PathPoint>();
            for (int iPoint = 0; iPoint < path.AmountOfPointsInPath; iPoint++)
            {
                PathPoints.Add(new PathPoint(path.CoordinatesX[iPoint], path.CoordinatesY[iPoint]));
            }
        }
        #endregion
    }
    /// <summary>
    /// Contains the cordinates of a path point.
    /// </summary>
    public class PathPoint
    {
        #region Properties
        /// <summary>
        /// X axis coordinate [m].
        /// </summary>
        public double CoordinateX { get; set; }
        /// <summary>
        /// Y axis coordinate [m].
        /// </summary>
        public double CoordinateY { get; set; }
        #endregion
        #region Constructors
        public PathPoint(double coordinateX, double coordinateY)
        {
            CoordinateX = coordinateX;
            CoordinateY = coordinateY;
        }
        #endregion
    }
}
