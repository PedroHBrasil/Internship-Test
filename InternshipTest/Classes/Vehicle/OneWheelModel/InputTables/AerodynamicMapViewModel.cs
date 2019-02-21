using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternshipTest.Vehicle.OneWheel
{
    public class AerodynamicMapViewModel
    {
        public ObservableCollection<AerodynamicMapPoint> AerodynamicMap { get; set; }

        public AerodynamicMapViewModel()
        {
            AerodynamicMap = new ObservableCollection<AerodynamicMapPoint>();
            GenerateAerodynamicMapPoints();
        }
        private void GenerateAerodynamicMapPoints()
        {
            AerodynamicMap.Add(new AerodynamicMapPoint(40, 50, 1.1, -3.0));
            AerodynamicMap.Add(new AerodynamicMapPoint(40, 40, 1.0, -3.1));
            AerodynamicMap.Add(new AerodynamicMapPoint(40, 30, 0.9, -3.2));
            AerodynamicMap.Add(new AerodynamicMapPoint(50, 50, 1.15, -3.1));
            AerodynamicMap.Add(new AerodynamicMapPoint(50, 40, 1.05, -3.2));
            AerodynamicMap.Add(new AerodynamicMapPoint(50, 30, 0.95, -3.3));
            AerodynamicMap.Add(new AerodynamicMapPoint(60, 50, 1.2, -3.15));
            AerodynamicMap.Add(new AerodynamicMapPoint(60, 40, 1.3, -3.25));
            AerodynamicMap.Add(new AerodynamicMapPoint(60, 30, 1.4, -3.35));
        }
    }
}
