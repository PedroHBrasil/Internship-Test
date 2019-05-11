using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternshipTest.Vehicle
{
    [Serializable]
    public class TireModelMF52Point
    {
        #region Properties
        #region Input Parameters
        /// <summary>
        /// Tire's longitudinal slip.
        /// </summary>
        public double LongitudinalSlip { get; set; }
        /// <summary>
        /// Tire's slip angle [deg].
        /// </summary>
        public double SlipAngle { get; set; }
        /// <summary>
        /// Tire's vertical load [N].
        /// </summary>
        public double VerticalLoad { get; set; }
        /// <summary>
        /// Tire's inclination angle [deg].
        /// </summary>
        public double InclinationAngle { get; set; }
        /// <summary>
        /// Tire's speed [km/h].
        /// </summary>
        public double Speed { get; set; }
        #endregion
        #region Output Parameters
        /// <summary>
        /// Tire's longitudinal force [N].
        /// </summary>
        public double LongitudinalForce { get; set; }
        /// <summary>
        /// Tire's lateral force [N].
        /// </summary>
        public double LateralForce { get; set; }
        /// <summary>
        /// Tire's overturning moment [Nm].
        /// </summary>
        public double OverturningMoment { get; set; }
        /// <summary>
        /// Tire's rolling moment [Nm].
        /// </summary>
        public double RollingMoment { get; set; }
        /// <summary>
        /// Tire's self-aligning torque [Nm].
        /// </summary>
        public double SelfAligningTorque { get; set; }
        #endregion
        #endregion
        #region Constructors
        public TireModelMF52Point() { }
        public TireModelMF52Point(double longitudinalSlip, double slipAngle, double verticalLoad, double inclinationAngle, double speed)
        {
            LongitudinalSlip = longitudinalSlip;
            SlipAngle = slipAngle;
            VerticalLoad = verticalLoad;
            InclinationAngle = inclinationAngle;
            Speed = speed;
        }
        #endregion
        #region Methods
        /// <summary>
        /// Gets the tire's longitudinal force for the current point's input parameters.
        /// </summary>
        /// <param name="tireModel"> Tire model to be used. </param>
        public void GetLongitudinalForce(TireModelMF52 tireModel)
        {
            double alpha = SlipAngle / 180 * Math.PI;
            double inclination = InclinationAngle / 180 * Math.PI;
            double speed = Speed / 3.6;
            LongitudinalForce = tireModel.GetTireFx(LongitudinalSlip, alpha, VerticalLoad, inclination, speed);
        }
        /// <summary>
        /// Gets the tire's lateral force for the current point's input parameters.
        /// </summary>
        /// <param name="tireModel"> Tire model to be used. </param>
        public void GetLateralForce(TireModelMF52 tireModel)
        {
            double alpha = SlipAngle / 180 * Math.PI;
            double inclination = InclinationAngle / 180 * Math.PI;
            double speed = Speed / 3.6;
            LateralForce = tireModel.GetTireFy(LongitudinalSlip, alpha, VerticalLoad, inclination, speed);
        }
        /// <summary>
        /// Gets the tire's overturning moment for the current point's input parameters.
        /// </summary>
        /// <param name="tireModel"> Tire model to be used. </param>
        public void GetOverturningMoment(TireModelMF52 tireModel)
        {
            double alpha = SlipAngle / 180 * Math.PI;
            double inclination = InclinationAngle / 180 * Math.PI;
            double speed = Speed / 3.6;
            OverturningMoment = tireModel.GetTireMx(LongitudinalSlip, alpha, VerticalLoad, inclination, speed);
        }
        /// <summary>
        /// Gets the tire's rolling moment for the current point's input parameters.
        /// </summary>
        /// <param name="tireModel"> Tire model to be used. </param>
        public void GetRollingMoment(TireModelMF52 tireModel)
        {
            double alpha = SlipAngle / 180 * Math.PI;
            double inclination = InclinationAngle / 180 * Math.PI;
            double speed = Speed / 3.6;
            RollingMoment = tireModel.GetTireMy(LongitudinalSlip, alpha, VerticalLoad, inclination, speed);
        }
        /// <summary>
        /// Gets the tire's self-aligning torque for the current point's input parameters.
        /// </summary>
        /// <param name="tireModel"> Tire model to be used. </param>
        public void GetSelfAligningTorque(TireModelMF52 tireModel)
        {
            double alpha = SlipAngle / 180 * Math.PI;
            double inclination = InclinationAngle / 180 * Math.PI;
            double speed = Speed / 3.6;
            SelfAligningTorque = tireModel.GetTireMz(LongitudinalSlip, alpha, VerticalLoad, inclination, speed);
        }
        #endregion
    }
}
