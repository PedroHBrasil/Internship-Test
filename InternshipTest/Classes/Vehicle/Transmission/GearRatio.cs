using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternshipTest.Vehicle
{
    /// <summary>
    /// Contains a gear ratio.
    /// </summary>
    [Serializable]
    public class GearRatio
    {
        #region Properties
        /// <summary>
        /// Gear ratio [(speed in)/(speed out)]
        /// </summary>
        public double Ratio { get; set; }
        #endregion
        #region Constructors
        public GearRatio() { }
        public GearRatio(double ratio)
        {
            Ratio = Math.Abs(ratio);
        }
        #endregion
    }
}
