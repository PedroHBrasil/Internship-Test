﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternshipTest.Vehicle
{
    /// <summary>
    /// Contains the information about a two wheel model brakes subsystem.
    /// </summary>
    [Serializable]
    public class TwoWheelBrakes : GenericInfo
    {
        #region Properties
        /// <summary>
        /// Share of total brake pressure that gets to the front wheel [ratio] (% front).
        /// </summary>
        public double BrakeBias { get; set; }
        /// <summary>
        /// Maximum appliable torque by the brakes subsystem at the front wheel [N*m].
        /// </summary>
        public double FrontMaximumTorque { get; set; }
        /// <summary>
        /// Maximum appliable torque by the brakes subsystem at the rear wheel [N*m].
        /// </summary>
        public double RearMaximumTorque { get; set; }
        /// <summary>
        /// Maximum front torque considering the brake bias [N*m].
        /// </summary>
        public double ActualMaximumFrontTorque { get; set; }
        /// <summary>
        /// Maximum rear torque considering the brake bias [N*m].
        /// </summary>
        public double ActualMaximumRearTorque { get; set; }
        #endregion
        #region Constructors
        public TwoWheelBrakes() { }
        public TwoWheelBrakes(string id, string description, double brakeBias, double frontMaximumTorque, double rearMaximumTorque)
        {
            ID = id;
            Description = description;
            BrakeBias = Math.Abs(brakeBias);
            FrontMaximumTorque = -Math.Abs(frontMaximumTorque);
            RearMaximumTorque = -Math.Abs(rearMaximumTorque);
        }
        #endregion
        #region Methods
        public void GetBrakesAuxiliarParameters()
        {
            // Brake bias according to maximum appliable torques
            double brakeBiasForMaximumTorques = FrontMaximumTorque / (FrontMaximumTorque + RearMaximumTorque);
            // Checks if this brake bias is higher or lower than the standard brake bias. Determines the actual maximum torques based on this.
            if (brakeBiasForMaximumTorques > BrakeBias)
            {
                ActualMaximumFrontTorque = RearMaximumTorque * BrakeBias / (1 - BrakeBias);
                ActualMaximumRearTorque = RearMaximumTorque;
            }
            else
            {
                ActualMaximumFrontTorque = FrontMaximumTorque;
                ActualMaximumRearTorque = FrontMaximumTorque * (1 - BrakeBias) / BrakeBias;
            }
        }
        #endregion
    }
}
