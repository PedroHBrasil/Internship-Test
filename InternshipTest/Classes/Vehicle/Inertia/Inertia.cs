using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternshipTest.Vehicle
{
    /// <summary>
    /// Contains the basic information about the inertia and dimensions objects.
    /// </summary>
    public class Inertia : GenericInfo
    {
        #region Properties
        /// <summary>
        /// Car total mass [kg].
        /// </summary>
        public double TotalMass { get; set; }
        /// <summary>
        /// Sum of the car's rotational parts moment of inertia [kg*m²].
        /// </summary>
        public double RotPartsMI { get; set; }
        /// <summary>
        /// Gravity acceleration [m/s²].
        /// </summary>
        public double Gravity { get; set; }
        #endregion
    }
}
