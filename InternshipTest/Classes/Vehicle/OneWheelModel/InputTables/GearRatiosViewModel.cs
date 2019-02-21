using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternshipTest.Vehicle.OneWheel
{
    class GearRatiosViewModel
    {
        public ObservableCollection<GearRatio> GearRatios { get; set; }

        public GearRatiosViewModel()
        {
            GearRatios = new ObservableCollection<GearRatio>();
            GenerateGearRatios();
        }
        private void GenerateGearRatios()
        {
            GearRatios.Add(new GearRatio(2.75));
            GearRatios.Add(new GearRatio(1.938));
            GearRatios.Add(new GearRatio(1.556));
            GearRatios.Add(new GearRatio(1.348));
            GearRatios.Add(new GearRatio(1.208));
            GearRatios.Add(new GearRatio(1.095));
        }
    }
}
