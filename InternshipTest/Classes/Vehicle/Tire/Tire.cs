using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternshipTest.Vehicle
{
    /// <summary>
    /// Contains the information about a tire subsystem.
    /// </summary>
    [Serializable]
    public class Tire : GenericInfo
    {
        #region Properties
        /// <summary>
        /// Pacejka's Magic Formula 5.2 data.
        /// </summary>
        public TireModelMF52 TireModel { get; set; }
        /// <summary>
        /// Tire's vertical stiffness [N/m]
        /// </summary>
        public double VerticalStiffness { get; set; }
        #endregion
        #region Constructors
        public Tire() { }

        public Tire(string tireID, string description, TireModelMF52 tireModel, double verticalStiffness)
        {
            ID = tireID;
            Description = description;
            TireModel = tireModel;
            VerticalStiffness = Math.Abs(verticalStiffness);
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
