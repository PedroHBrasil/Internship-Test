using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternshipTest.Vehicle
{
    /// <summary>
    /// Contains the gear ratios of a gearbox.
    /// </summary>
    public class GearRatiosSet : GenericInfo
    {
        #region Properties
        /// <summary>
        /// Gearbox's ratios [(speed in)/(speed out)].
        /// </summary>
        public List<GearRatio> GearRatios { get; set; }
        #endregion

        #region Constructors
        public GearRatiosSet() { }
        public GearRatiosSet(string gearRatiosSetID, string description, List<GearRatio> gearRatios)
        {
            ID = gearRatiosSetID;
            Description = description;
            GearRatios = gearRatios;
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
