using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternshipTest.Vehicle
{
    public class Aerodynamics : GenericInfo
    {
        #region Properties
        /// <summary>
        /// Area of the vehicle's projection over the front plane projection [m²].
        /// </summary>
        public double FrontalArea { get; set; }
        /// <summary>
        /// Density of the environment air [kg/m³].
        /// </summary>
        public double AirDensity { get; set; }
        #endregion
    }
}
