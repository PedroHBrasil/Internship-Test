using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternshipTest
{
    public class PathSector
    {
        // Properties ---------------------------------------------------
        public int SectorID { get; set; }
        public double SectorStartDistance { get; set; }
        public bool IsDRSEnabled { get; set; }
        // Constructors -------------------------------------------------
        public PathSector()
        {
            SectorID = 0;
            SectorStartDistance = 0;
            IsDRSEnabled = false;
        }
        public PathSector(int sectorID, double sectorStartDistance, bool isDRSEnabled)
        {
            SectorID = sectorID;
            SectorStartDistance = sectorStartDistance;
            IsDRSEnabled = isDRSEnabled;
        }
    }
}
