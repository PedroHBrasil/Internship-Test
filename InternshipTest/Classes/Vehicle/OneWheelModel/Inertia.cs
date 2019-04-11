using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternshipTest.Vehicle.OneWheel
{
    /// <summary>
    /// Contains the information about a one wheel model's vehicle's inertia.
    /// </summary>
    public class Inertia : GenericInfo
    {
        #region Properties
        /// <summary>
        /// Car total mass [kg].
        /// </summary>
        public double TotalMass { get; set; }
        /// <summary>
        /// Car's total unsprung mass [kg]. 
        /// </summary>
        public double UnsprungMass { get; set; }
        /// <summary>
        /// Sum of the car's rotational parts moment of inertia [kg*m²].
        /// </summary>
        public double RotPartsMI { get; set; }
        /// <summary>
        /// Gravity acceleration [m/s²].
        /// </summary>
        public double Gravity { get; set; }
        #endregion
        #region Constructors
        public Inertia() { }
        public Inertia(string inertiaID, string description, double totalMass, double unsprungMass, double rotPartsMI, double gravity)
        {
            ID = inertiaID;
            Description = description;
            TotalMass = Math.Abs(totalMass);
            UnsprungMass = Math.Abs(unsprungMass);
            RotPartsMI = Math.Abs(rotPartsMI);
            Gravity = Math.Abs(gravity);
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
