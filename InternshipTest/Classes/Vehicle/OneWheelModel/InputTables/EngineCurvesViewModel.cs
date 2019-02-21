using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternshipTest.Vehicle.OneWheel
{
    public class EngineCurvesViewModel
    {
        public ObservableCollection<EngineCurvesPoint> EngineCurves { get; set; }

        public EngineCurvesViewModel()
        {
            EngineCurves = new ObservableCollection<EngineCurvesPoint>();
            GenerateEngineCurvesPoints();
        }

        private void GenerateEngineCurvesPoints()
        {
            EngineCurves.Add(new EngineCurvesPoint(3000, 55.77, 3.0, 390.45));
            EngineCurves.Add(new EngineCurvesPoint(3500, 52.50, 3.5, 381.50));
            EngineCurves.Add(new EngineCurvesPoint(4000, 54.71, 4.0, 381.50));
            EngineCurves.Add(new EngineCurvesPoint(4500, 57.77, 4.5, 379.02));
            EngineCurves.Add(new EngineCurvesPoint(5000, 70.67, 5.0, 367.21));
            EngineCurves.Add(new EngineCurvesPoint(5500, 79.63, 5.5, 366.58));
            EngineCurves.Add(new EngineCurvesPoint(6000, 83.18, 6.0, 361.44));
            EngineCurves.Add(new EngineCurvesPoint(6500, 83.82, 6.5, 362.35));
            EngineCurves.Add(new EngineCurvesPoint(7000, 83.86, 7.0, 364.99));
            EngineCurves.Add(new EngineCurvesPoint(7500, 84.50, 7.5, 369.16));
            EngineCurves.Add(new EngineCurvesPoint(8000, 82.56, 8.0, 372.97));
            EngineCurves.Add(new EngineCurvesPoint(8500, 77.88, 8.5, 375.73));
            EngineCurves.Add(new EngineCurvesPoint(9000, 72.16, 9.0, 378.86));
            EngineCurves.Add(new EngineCurvesPoint(9500, 66.65, 9.5, 383.92));
            EngineCurves.Add(new EngineCurvesPoint(10000, 61.16, 10.0, 391.00));
            EngineCurves.Add(new EngineCurvesPoint(10500, 55.94, 10.5, 398.92));
            EngineCurves.Add(new EngineCurvesPoint(11000, 50.23, 11.0, 407.68));
        }
    }
}
