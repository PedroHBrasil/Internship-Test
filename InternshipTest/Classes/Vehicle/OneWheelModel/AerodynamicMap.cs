using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternshipTest.Vehicle.OneWheel
{
    /// <summary>
    /// Contains the information of a one wheel model vehicle aerodynamic map.
    /// </summary>
    public class AerodynamicMap : GenericInfo
    {
        #region Properties
        /// <summary>
        /// Contains the aerodynamic map points.
        /// </summary>
        public List<AerodynamicMapPoint> MapPoints { get; set; }
        #endregion
        #region Constructors
        public AerodynamicMap() { }
        public AerodynamicMap(string mapID, string description, List<AerodynamicMapPoint> aerodynamicMapPoints)
        {
            ID = mapID;
            Description = description;
            MapPoints = aerodynamicMapPoints;
        }
        #endregion
        #region Methods
        public override string ToString()
        {
            return ID;
        }
        #endregion
    }
}
