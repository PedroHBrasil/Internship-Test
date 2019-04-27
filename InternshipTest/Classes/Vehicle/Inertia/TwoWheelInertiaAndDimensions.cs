using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternshipTest.Vehicle
{
    /// <summary>
    /// Contains the inertia and dimensions parameters of a two wheel vehicle model's.
    /// </summary>
    class TwoWheelInertiaAndDimensions : Inertia
    {
        #region Properties
        /// <summary>
        /// Total mass distribution front-to-rear [ratio] (% front).
        /// </summary>
        public double TotalMassDistribution { get; set; }
        /// <summary>
        /// Total mass CG height [m].
        /// </summary>
        public double TotalMassCGHeight { get; set; }
        /// <summary>
        /// Car's front unsprung mass [kg]. 
        /// </summary>
        public double FrontUnsprungMass { get; set; }
        /// <summary>
        /// Front unsprung mass CG height [m].
        /// </summary>
        public double FrontUnsprungMassCGHeight { get; set; }
        /// <summary>
        /// Car's rear unsprung mass [kg]. 
        /// </summary>
        public double RearUnsprungMass { get; set; }
        /// <summary>
        /// Rear unsprung mass CG height [m].
        /// </summary>
        public double RearUnsprungMassCGHeight { get; set; }
        /// <summary>
        /// Vehicle's wheelbase [m].
        /// </summary>
        public double Wheelbase { get; set; }
        #endregion
        #region Constructors
        public TwoWheelInertiaAndDimensions(string id, string desciption, double totalMass,double totalMassDistribution, double totalMassCGHeight, double frontUnsprungMass, double frontUnsprungMassCGHeight, double rearUnsprungMass, double rearUnsprungMassCGHeight, double wheelbase, double rotPartsMI, double gravity)
        {
            ID = id;
            Description = desciption;
            TotalMass = Math.Abs(totalMass);
            TotalMassDistribution = Math.Abs(totalMassDistribution);
            TotalMassCGHeight = Math.Abs(totalMassCGHeight);
            FrontUnsprungMass = Math.Abs(frontUnsprungMass);
            FrontUnsprungMassCGHeight = Math.Abs(frontUnsprungMassCGHeight);
            RearUnsprungMass = Math.Abs(rearUnsprungMass);
            RearUnsprungMassCGHeight = Math.Abs(rearUnsprungMassCGHeight);
            Wheelbase = Math.Abs(wheelbase);
            RotPartsMI = Math.Abs(rotPartsMI);
            Gravity = Math.Abs(gravity);
        }
        #endregion
    }
}
