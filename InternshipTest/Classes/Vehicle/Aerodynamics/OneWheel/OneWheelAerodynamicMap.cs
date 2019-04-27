using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternshipTest.Vehicle
{
    /// <summary>
    /// Contains the information of a one wheel model vehicle aerodynamic map.
    /// </summary>
    public class OneWheelAerodynamicMap : GenericInfo
    {
        #region Properties
        /// <summary>
        /// Contains the aerodynamic map points.
        /// </summary>
        public List<OneWheelAerodynamicMapPoint> MapPoints { get; set; }
        #endregion
        #region Constructors
        public OneWheelAerodynamicMap() { }
        public OneWheelAerodynamicMap(string mapID, string description, List<OneWheelAerodynamicMapPoint> aerodynamicMapPoints)
        {
            ID = mapID;
            Description = description;
            MapPoints = aerodynamicMapPoints;
        }
        #endregion
    }
}
