using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternshipTest.Vehicle
{
    public class PowertrainViewModel
    {
        #region Properties
        public List<ObservableCollection<PowertrainViewModelPoint>> PowertrainDiagramCurvePoints { get; set; }
        #endregion
        #region Constructors
        public PowertrainViewModel() { }
        public void GetPowertrainCurvePoints(Engine engine, Transmission transmission)
        {
            PowertrainDiagramCurvePoints = new List<ObservableCollection<PowertrainViewModelPoint>>();
            // Gets the engine torque curve
            List<double> torques = new List<double>();
            List<double> engineSpeeds = new List<double>();
            foreach (EngineCurvesPoint engineCurvesPoint in engine.EngineCurves.CurvesPoints)
            {
                torques.Add(engineCurvesPoint.Torque);
                engineSpeeds.Add(engineCurvesPoint.RotationalSpeed);
            }
            // Gets the gear ratios
            List<double> gearRatios = new List<double>();
            foreach (GearRatio gearRatio in transmission.GearRatiosSet.GearRatios)
            {
                double resultingRatio = transmission.PrimaryRatio * transmission.FinalRatio * gearRatio.Ratio;
                gearRatios.Add(resultingRatio);
            }
            // Gets the resulting wheel torque curve foreach gear
            foreach (double resultingRatio in gearRatios)
            {
                ObservableCollection<PowertrainViewModelPoint> currentCollection = new ObservableCollection<PowertrainViewModelPoint>();
                for (int iPoint = 0; iPoint < torques.Count; iPoint++)
                {
                    double currentTorque = torques[iPoint] * resultingRatio * transmission.Efficiency;
                    double currentSpeed = engineSpeeds[iPoint] / resultingRatio;
                    PowertrainViewModelPoint currentPoint = new PowertrainViewModelPoint(currentTorque, currentSpeed);
                    currentCollection.Add(currentPoint);
                }
                PowertrainDiagramCurvePoints.Add(currentCollection);
            }
        }
        #endregion
    }
    public class PowertrainViewModelPoint
    {
        #region Properties
        /// <summary>
        /// Wheel Torque [Nm].
        /// </summary>
        public double Torque { get; set; }
        /// <summary>
        /// Wheel Angular Speed [rpm].
        /// </summary>
        public double WheelAngularSpeed { get; set; }
        #endregion
        #region Constructors
        public PowertrainViewModelPoint() { }
        public PowertrainViewModelPoint(double torque, double wheelAngularSpeed)
        {
            Torque = torque;
            WheelAngularSpeed = wheelAngularSpeed * 60 / (2 * Math.PI);
        }
        #endregion
    }
}
