using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternshipTest
{
    public class PathSectorViewModel
    {
        public ObservableCollection<PathSector> PathSectors { get; set; }

        public PathSectorViewModel()
        {
            PathSectors = new ObservableCollection<PathSector>()
            {
                new PathSector(1,0,true),
                new PathSector(2,50,false),
                new PathSector(3,128.52,false),
                new PathSector(4,178.52,false)
            };
        }
    }
}
