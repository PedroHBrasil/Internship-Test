using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternshipTest.Vehicle
{
    /// <summary>
    /// Contains the aerodynamic map of a two wheel vehicle model.
    /// </summary>
    [Serializable]
    public class TwoWheelAerodynamicMap : GenericInfo
    {
        #region Properties
        /// <summary>
        /// Contains the aerodynamic map points.
        /// </summary>
        public List<TwoWheelAerodynamicMapPoint> MapPoints { get; set; }
        #endregion
        #region Constructors
        public TwoWheelAerodynamicMap() { }
        public TwoWheelAerodynamicMap(string mapID, string description, List<TwoWheelAerodynamicMapPoint> aerodynamicMapPoints)
        {
            ID = mapID;
            Description = description;
            MapPoints = aerodynamicMapPoints;
        }
        #endregion
    }
}
