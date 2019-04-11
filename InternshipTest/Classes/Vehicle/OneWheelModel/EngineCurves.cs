using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternshipTest.Vehicle.OneWheel
{
    /// <summary>
    /// Contains the engine curves of a one wheel model's engine subsystem (torque, braking torque and specific fuel consumption as functions of the rotational speed).
    /// </summary>
    public class EngineCurves : GenericInfo
    {
        #region Properties
        /// <summary>
        /// Points of the engine's curves.
        /// </summary>
        public List<EngineCurvesPoint> CurvesPoints { get; set; }
        #endregion
        #region Constructors
        public EngineCurves() { }
        public EngineCurves(string curvesID, string description, List<EngineCurvesPoint> curvesPoints)
        {
            ID = curvesID;
            Description = description;
            CurvesPoints = curvesPoints;
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
