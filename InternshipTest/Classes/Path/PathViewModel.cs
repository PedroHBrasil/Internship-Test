using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternshipTest
{
    class PathViewModel
    {
        // Properties
        public ObservableCollection<PathPoint> PathPoints { get; set; }
        // Constructor
        public PathViewModel(Path path)
        {
            PathPoints = new ObservableCollection<PathPoint>();
            for (int iPoint = 0; iPoint < path.AmountOfPointsInPath; iPoint++)
            {
                PathPoints.Add(new PathPoint(path.CoordinatesX[iPoint], path.CoordinatesY[iPoint]));
            }
        }
    }
    class PathPoint
    {
        // Properties
        public double CoordinateX { get; set; }
        public double CoordinateY { get; set; }
        // Constructor
        public PathPoint(double coordinateX, double coordinateY)
        {
            CoordinateX = coordinateX;
            CoordinateY = coordinateY;
        }
    }
}
