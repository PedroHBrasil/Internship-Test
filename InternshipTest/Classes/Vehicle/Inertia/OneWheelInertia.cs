using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternshipTest.Vehicle
{
    /// <summary>
    /// Contains the information about a one wheel model's vehicle's inertia.
    /// </summary>
    [Serializable]
    public class OneWheelInertia : Inertia
    {
        #region Properties
        /// <summary>
        /// Car's total unsprung mass [kg]. 
        /// </summary>
        public double UnsprungMass { get; set; }
        #endregion
        #region Constructors
        public OneWheelInertia() { }
        public OneWheelInertia(string inertiaID, string description, double totalMass, double unsprungMass, double rotPartsMI, double gravity)
        {
            ID = inertiaID;
            Description = description;
            TotalMass = Math.Abs(totalMass);
            UnsprungMass = Math.Abs(unsprungMass);
            RotPartsMI = Math.Abs(rotPartsMI);
            Gravity = Math.Abs(gravity);
        }
        #endregion
    }
}
