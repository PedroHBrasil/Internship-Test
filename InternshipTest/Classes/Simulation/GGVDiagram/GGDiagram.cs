using MathNet.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternshipTest.Simulation
{
    class GGDiagram
    {
        // Fields
        private Vehicle.OneWheel.AerodynamicMapPoint interpolatedAerodynamicMapPoint;
        private double tireFz;
        private double kappa;
        private double alpha;
        private double referenceFx;
        
        // Properties
        public double Speed { get; set; }
        public Vehicle.OneWheel.Car Car { get; set; }
        public int AmountOfPoints { get; set; }
        public List<double> LongitudinalAccelerations { get; set; }
        public List<double> LateralAccelerations { get; set; }
        public List<double> Curvatures { get; set; }

        // Constructors
        public GGDiagram(Vehicle.OneWheel.Car car)
        {
            Car = car;
            LongitudinalAccelerations = new List<double>();
            LateralAccelerations = new List<double>();
            Curvatures = new List<double>();
        }

        public GGDiagram(double speed, Vehicle.OneWheel.Car car, int amountOfPoints)
        {
            Speed = speed;
            Car = car;
            AmountOfPoints = amountOfPoints;

            LongitudinalAccelerations = new List<double>();
            LateralAccelerations = new List<double>();
            Curvatures = new List<double>();

            GenerateGGDiagram();
            GetAssociatedCurvatures();
        }

        // Methods
        private void GenerateGGDiagram()
        {
            // Longitudinal Slip and Slip Angle initial values
            kappa = 0; alpha = 0;
            // Gets the aerodynamic coefficients
            interpolatedAerodynamicMapPoint = Car.GetAerodynamicCoefficients(Speed);
            // Gets the lift force
            double liftForce = -interpolatedAerodynamicMapPoint.LiftCoefficient * Car.Aerodynamics.FrontalArea * Car.Aerodynamics.AirDensity * Math.Pow(Speed / 3.6, 2) / 2;
            // Tire resultant Fz
            tireFz = (liftForce + Car.Inertia.TotalMass * Car.Inertia.Gravity) / 4;
            // Slip angle optimization for minimum tire Fy in pureslip
            double alphaMin = SlipAngleForFYMinimizationInPureSlip();
            // Longitudinal Slip optimization for minimum tire Fx in pureslip
            double kappaBrake = LongitudinalSlipForFXMinimizationInPureSlip();
            // Longitudinal Slip optimization for maximum tire Fx in pureslip
            double kappaAccel = LongitudinalSlipForFXMaximizationInPureSlip();
            // Generates the vectors of longitudinal slip and slip angle
            double[] kappas = Generate.LinearSpaced(AmountOfPoints / 4, kappaBrake, kappaAccel);
            double[] alphas = Generate.LinearSpaced(AmountOfPoints / 8, 0, alphaMin);
            // Generates the GG diagram
            GetGGDiagramAccelerations(kappas, alphas);
        }
        #region GGDiagramAccelerations
        private void GetGGDiagramAccelerations(double[] kappas, double[] alphas)
        {
            // Extreme left cornering + braking region
            foreach (double currentAlpha in alphas)
            {
                // Slip angle update
                alpha = currentAlpha;
                // Longitudinal Slip for minimum longitudinal force
                kappa = LongitudinalSlipForFXMinimizationInPureSlip();
                // Accelerations determination
                GetAccelerationsForMinimumLongitudinalAndFixedSlipAngle();
            }
            // Extreme left cornering region
            foreach (double currentKappa in kappas)
            {
                // Longitudinal Slip update
                kappa = currentKappa;
                // Slip Angle for minimum lateral force
                alpha = SlipAngleForFYMinimizationInPureSlip();
                // Which is the sign of the longitudinal slip?
                if (kappa >= 0) GetAccelerationsForMaximumLongitudinalAndFixedSlipAngle();
                else GetAccelerationsForMinimumLongitudinalAndFixedSlipAngle();
            }
            // Extreme left cornering + accelerating region
            foreach (double currentAlpha in alphas.Reverse())
            {
                // Slip angle update
                alpha = currentAlpha;
                // Longitudinal Slip for maximum longitudinal force
                kappa = LongitudinalSlipForFXMaximizationInPureSlip();
                // Accelerations determination
                GetAccelerationsForMaximumLongitudinalAndFixedSlipAngle();
            }
            // Mirroring to get the right cornering side
            for (int iAcceleration = LongitudinalAccelerations.Count - 2; iAcceleration > 0; iAcceleration--)
            {
                LongitudinalAccelerations.Add(LongitudinalAccelerations[iAcceleration]);
                LateralAccelerations.Add(-LateralAccelerations[iAcceleration]);
            }
        }
        private void GetAccelerationsForMinimumLongitudinalAndFixedSlipAngle()
        {
            // Calculates the wheel radius [mm]
            double wheelRadius = Car.Tire.TireModel.RO * 1000 - tireFz / Car.Tire.VerticalStiffness;
            // Maximum appliable torque due to tire grip [Nm]
            double limitTorqueDueToGrip = Car.Tire.TireModel.GetTireFx(kappa, alpha, tireFz, 0, Speed / 3.6) * (wheelRadius / 1000) * 4;
            // Rolling resistance moment [Nm]
            double tireMy = Car.Tire.TireModel.GetTireMy(0, alpha, tireFz, 0, Speed / 3.6);
            // Wheel center angular speed [rpm]
            double wheelCenterAngularSpeed = Speed * 60 / (3.6 * 2 * Math.PI * wheelRadius / 1000);
            // Wheel braking torque curve interpolation
            alglib.spline1dbuildlinear(Car.WheelRPMCurve.ToArray(), Car.WheelBrakingTorqueCurve.ToArray(), out alglib.spline1dinterpolant wheelBrakingTorqueInterp);
            double powertrainBrakingTorque = alglib.spline1dcalc(wheelBrakingTorqueInterp, wheelCenterAngularSpeed) + tireMy * Car.Transmission.AmountOfDrivenWheels;
            // Limit torque due to brakes 
            double limitTorqueDueToBrakes = -Car.Brakes.MaxTorque + powertrainBrakingTorque + tireMy * 4;
            // Is the torque limited by the brakes or by the tire grip?
            if (limitTorqueDueToGrip < limitTorqueDueToBrakes)
            {
                referenceFx = limitTorqueDueToBrakes / (wheelRadius / 1000) / 4;
                kappa = GetLongitudinalSlipForGivenWheelTorque();
            }
            // Aerodynamic drag force [N]
            double dragForce = -interpolatedAerodynamicMapPoint.DragCoefficient * Car.Aerodynamics.FrontalArea * Car.Aerodynamics.AirDensity * Math.Pow(Speed / 3.6, 2) / 2;
            // Car's resultant longitudinal force [N]
            double carLongitudinalForce = Car.Tire.TireModel.GetTireFx(kappa, alpha, tireFz, 0, Speed / 3.6) * 4 + dragForce;
            // Car's resultant lateral force [N]
            double carLateralForce = Car.Tire.TireModel.GetTireFy(kappa, alpha, tireFz, 0, Speed / 3.6) * 4;
            // Inertia efficiency (due to rotational parts moment of inertia)
            double inertiaEfficiency = Math.Pow(wheelRadius / 1000, 2) * Car.Inertia.TotalMass /
                (Math.Pow(wheelRadius / 1000, 2) * Car.Inertia.TotalMass + Car.Inertia.RotPartsMI);
            // Car's longitudinal acceleration [G]
            double longitudinalAcceleration = (carLongitudinalForce / Car.Inertia.TotalMass) * inertiaEfficiency / Car.Inertia.Gravity;
            // Car's lateral acceleration [G]
            double lateralAcceleration = carLateralForce / Car.Inertia.TotalMass / Car.Inertia.Gravity;
            // Result
            LongitudinalAccelerations.Add(longitudinalAcceleration);
            LateralAccelerations.Add(lateralAcceleration);
        }
        private void GetAccelerationsForMaximumLongitudinalAndFixedSlipAngle()
        {
            // Calculates the wheel radius [mm]
            double wheelRadius = Car.Tire.TireModel.RO * 1000 - tireFz / Car.Tire.VerticalStiffness;
            // Maximum appliable torque due to tire grip [Nm]
            double limitTorqueDueToGrip = Car.Tire.TireModel.GetTireFx(kappa, alpha, tireFz, 0, Speed / 3.6) * (wheelRadius / 1000) * Car.Transmission.AmountOfDrivenWheels;
            // Rolling resistance moment [Nm]
            double tireMy = Car.Tire.TireModel.GetTireMy(0, alpha, tireFz, 0, Speed / 3.6);
            // Wheel center angular speed [rpm]
            double wheelCenterAngularSpeed = Speed * 60 / (3.6 * 2 * Math.PI * wheelRadius / 1000);
            // Wheel torque curve interpolation
            alglib.spline1dbuildlinear(Car.WheelRPMCurve.ToArray(), Car.WheelTorqueCurve.ToArray(), out alglib.spline1dinterpolant wheelTorqueInterp);
            double limitTorqueDueToPowertrain= alglib.spline1dcalc(wheelTorqueInterp,wheelCenterAngularSpeed) + tireMy * Car.Transmission.AmountOfDrivenWheels;
            // Is the torque limited by the engine or by the tire grip?
            if (limitTorqueDueToGrip > limitTorqueDueToPowertrain)
            {
                referenceFx = limitTorqueDueToPowertrain / (wheelRadius / 1000) / Car.Transmission.AmountOfDrivenWheels;
                kappa = GetLongitudinalSlipForGivenWheelTorque();
            }
            // Aerodynamic drag force [N]
            double dragForce = -interpolatedAerodynamicMapPoint.DragCoefficient * Car.Aerodynamics.FrontalArea * Car.Aerodynamics.AirDensity * Math.Pow(Speed / 3.6, 2) / 2;
            // Car's resultant longitudinal force [N]
            double carLongitudinalForce = Car.Tire.TireModel.GetTireFx(kappa, alpha, tireFz, 0, Speed / 3.6) * Car.Transmission.AmountOfDrivenWheels + dragForce +
                tireMy / (wheelRadius / 1000) * (4 - Car.Transmission.AmountOfDrivenWheels);
            // Car's resultant lateral force [N]
            double carLateralForce;
            if (Car.Transmission.AmountOfDrivenWheels == 2)
                carLateralForce = (Car.Tire.TireModel.GetTireFy(0, alpha, tireFz, 0, Speed / 3.6) + Car.Tire.TireModel.GetTireFy(kappa, alpha, tireFz, 0, Speed / 3.6)) * 2;
            else
                carLateralForce = Car.Tire.TireModel.GetTireFy(kappa, alpha, tireFz, 0, Speed / 3.6) * 4;
            // Inertia efficiency (due to rotational parts moment of inertia)
            double inertiaEfficiency = Math.Pow(wheelRadius / 1000, 2) * Car.Inertia.TotalMass /
                (Math.Pow(wheelRadius / 1000, 2) * Car.Inertia.TotalMass + Car.Inertia.RotPartsMI);
            // Car's longitudinal acceleration [G]
            double longitudinalAcceleration = (carLongitudinalForce / Car.Inertia.TotalMass) * inertiaEfficiency / Car.Inertia.Gravity;
            // Car's lateral acceleration [G]
            double lateralAcceleration = carLateralForce / Car.Inertia.TotalMass / Car.Inertia.Gravity;
            // Result
            LongitudinalAccelerations.Add(longitudinalAcceleration);
            LateralAccelerations.Add(lateralAcceleration);
        }

        private double GetLongitudinalSlipForGivenWheelTorque()
        {
            // Optimization parameters
            double epsg = 1e-10;
            double epsf = 0;
            double epsx = 0;
            double diffstep = 1.0e-6;
            int maxits = 100;
            double[] bndl = new double[] { -.2 };
            double[] bndu = new double[] { .2 };

            double[] kappaAccel = new double[] { 0 };

            alglib.minbleiccreatef(kappaAccel, diffstep, out alglib.minbleicstate state);
            alglib.minbleicsetbc(state, bndl, bndu);
            alglib.minbleicsetcond(state, epsg, epsf, epsx, maxits);
            alglib.minbleicoptimize(state, LongitudinalSlipForGivenWheelTorqueOptimization, null, null);
            alglib.minbleicresults(state, out kappaAccel, out alglib.minbleicreport rep);
            /*
            alglib.minlbfgscreatef(1, kappaAccel, diffstep, out alglib.minlbfgsstate state);
            alglib.minlbfgssetcond(state, epsg, epsf, epsx, maxits);
            alglib.minlbfgsoptimize(state, LongitudinalSlipForGivenWheelTorqueOptimization, null, null);
            alglib.minlbfgsresults(state, out kappaAccel, out alglib.minlbfgsreport rep);
            */
            return kappaAccel[0];
        }

        private void LongitudinalSlipForGivenWheelTorqueOptimization(double[] kappa, ref double tireFx, object obj)
        {
            tireFx = Math.Abs(Car.Tire.TireModel.GetTireFx(kappa[0], alpha, tireFz, 0, Speed / 3.6) - referenceFx);
        }

        #endregion
        #region TireForcesOptimization
        private double SlipAngleForFYMinimizationInPureSlip()
        {
            // Optimization parameters
            double epsg = 1e-10;
            double epsf = 0;
            double epsx = 0;
            double diffstep = 1.0e-6;
            int maxits = 100;
            double[] bndl = new double[] { -1.5 };
            double[] bndu = new double[] { 1.5 };

            double[] alpha = new double[] { 0 };

            alglib.minbleiccreatef(alpha, diffstep, out alglib.minbleicstate state);
            alglib.minbleicsetbc(state, bndl, bndu);
            alglib.minbleicsetcond(state, epsg, epsf, epsx, maxits);
            alglib.minbleicoptimize(state, SlipAngleForMinimumTireFY, null, null);
            alglib.minbleicresults(state, out alpha, out alglib.minbleicreport rep);

            /*
            alglib.mincgcreatef(alpha, diffstep, out alglib.mincgstate state);
            alglib.mincgsetcond(state, epsg, epsf, epsx, maxits);
            alglib.mincgoptimize(state, SlipAngleForMinimumTireFY, null, null);
            alglib.mincgresults(state, out alpha, out alglib.mincgreport rep);
            */
            /*
            alglib.minlbfgscreatef(1, alpha, diffstep, out alglib.minlbfgsstate state);
            alglib.minlbfgssetcond(state, epsg, epsf, epsx, maxits);
            alglib.minlbfgsoptimize(state, SlipAngleForMinimumTireFY, null, null);
            alglib.minlbfgsresults(state, out alpha, out alglib.minlbfgsreport rep);
            */
            return alpha[0];
        }
        private double LongitudinalSlipForFXMinimizationInPureSlip()
        {
            // Optimization parameters
            double epsg = 1e-10;
            double epsf = 0;
            double epsx = 0;
            double diffstep = 1.0e-6;
            int maxits = 100;
            double[] bndl = new double[] { -.2 };
            double[] bndu = new double[] { 0 };

            double[] kappaBrake = new double[] { 0 };

            alglib.minbleiccreatef(kappaBrake, diffstep, out alglib.minbleicstate state);
            alglib.minbleicsetbc(state, bndl, bndu);
            alglib.minbleicsetcond(state, epsg, epsf, epsx, maxits);
            alglib.minbleicoptimize(state, LongitudinalSlipForMinimumTireFX, null, null);
            alglib.minbleicresults(state, out kappaBrake, out alglib.minbleicreport rep);

            /*
            alglib.minlbfgscreatef(1, kappaBrake, diffstep, out alglib.minlbfgsstate state);
            alglib.minlbfgssetcond(state, epsg, epsf, epsx, maxits);
            alglib.minlbfgsoptimize(state, LongitudinalSlipForMinimumTireFX, null, null);
            alglib.minlbfgsresults(state, out kappaBrake, out alglib.minlbfgsreport rep);
            */
            return kappaBrake[0];
        }
        private double LongitudinalSlipForFXMaximizationInPureSlip()
        {
            // Optimization parameters
            double epsg = 1e-10;
            double epsf = 0;
            double epsx = 0;
            double diffstep = 1.0e-6;
            int maxits = 100;
            double[] bndl = new double[] { 0 };
            double[] bndu = new double[] { .2 };

            double[] kappaAccel = new double[] { 0 };

            alglib.minbleiccreatef(kappaAccel, diffstep, out alglib.minbleicstate state);
            alglib.minbleicsetbc(state, bndl, bndu);
            alglib.minbleicsetcond(state, epsg, epsf, epsx, maxits);
            alglib.minbleicoptimize(state, LongitudinalSlipForMaximumTireFX, null, null);
            alglib.minbleicresults(state, out kappaAccel, out alglib.minbleicreport rep);
            /*
            alglib.minlbfgscreatef(1, kappaAccel, diffstep, out alglib.minlbfgsstate state);
            alglib.minlbfgssetcond(state, epsg, epsf, epsx, maxits);
            alglib.minlbfgsoptimize(state, LongitudinalSlipForMaximumTireFX, null, null);
            alglib.minlbfgsresults(state, out kappaAccel, out alglib.minlbfgsreport rep);
            */
            return kappaAccel[0];
        }

        private void SlipAngleForMinimumTireFY(double[] alpha, ref double tireFy, object obj)
        {
            tireFy = Car.Tire.TireModel.GetTireFy(kappa, alpha[0], tireFz, 0, Speed / 3.6);
        }
        private void LongitudinalSlipForMinimumTireFX(double[] kappa, ref double tireFx, object obj)
        {
            tireFx = Car.Tire.TireModel.GetTireFx(kappa[0], alpha, tireFz, 0, Speed / 3.6);
        }
        private void LongitudinalSlipForMaximumTireFX(double[] kappa, ref double tireFx, object obj)
        {
            tireFx = -Car.Tire.TireModel.GetTireFx(kappa[0], alpha, tireFz, 0, Speed / 3.6);
        }
        #endregion
        public void GetAssociatedCurvatures()
        {
            // Gets and registers the curvatures associated with each point of the GG diagram
            for (int iLateralAcceleration = 0; iLateralAcceleration < LateralAccelerations.Count; iLateralAcceleration++)
            {
                // Corrects the speed if it is equal to zero
                double currentSpeed = Speed;
                if (currentSpeed == 0) currentSpeed = 0.1;
                Curvatures.Add(LateralAccelerations[iLateralAcceleration] * Car.Inertia.Gravity / Math.Pow(currentSpeed / 3.6, 2));
            }
        }
        public double GetLongitudinalAccelerationViaInterpolationBasedOnLateralAcceleration(double lateralAcceleration, string longitudinalAccelerationMode)
        {
            // Gets the lateral acceleration interval index
            int iAcceleration = GetLateralAccelerationIndex(lateralAcceleration, longitudinalAccelerationMode);
            // Gets the index of the next lateral acceleration
            int iNextAcceleration;
            if (iAcceleration == LateralAccelerations.Count - 1) iNextAcceleration = 0;
            else iNextAcceleration = iAcceleration + 1;
            // Calculates the interpolation ratio
            double interpolationRatio = (lateralAcceleration - LateralAccelerations[iAcceleration]) -
                (LateralAccelerations[iNextAcceleration] - LateralAccelerations[iAcceleration]);
            // Adjusts the interpolation ratio if necessary
            if (interpolationRatio < 0) interpolationRatio = 0;
            else if (interpolationRatio > 1) interpolationRatio = 1;
            // Gets the longitudinal acceleration
            double longitudinalAcceleration = LongitudinalAccelerations[iAcceleration] +
                interpolationRatio * (LongitudinalAccelerations[iNextAcceleration] - LongitudinalAccelerations[iAcceleration]);
            // Returns the longitudinal acceleration
            return longitudinalAcceleration;
        }
        private int GetLateralAccelerationIndex(double lateralAcceleration, string longitudinalAccelerationMode)
        {
            // Initializes the index and range indicator variables
            int iAcceleration;
            bool isInRange = false;
            // Checks if the lateral acceleration is in range
            if (lateralAcceleration >= LateralAccelerations.Min() &&
                lateralAcceleration <= LateralAccelerations.Max())
                isInRange = true;
            // If in range, finds the interval. Else, uses the maximum or minimum lateral acceleration index.
            if (isInRange)
            {
                for (iAcceleration = 0; iAcceleration < LateralAccelerations.Count; iAcceleration++)
                {
                    // Next acceleration index
                    int iNextAcceleration;
                    if (iAcceleration == LateralAccelerations.Count) iNextAcceleration = 0;
                    else iNextAcceleration = iAcceleration + 1;
                    // Checks if the current interval contains the current lateral acceleration ad if it corresponds to the current acceleration mode
                    if (longitudinalAccelerationMode == "Braking" &&
                        LateralAccelerations[iAcceleration] >= lateralAcceleration &&
                        LateralAccelerations[iNextAcceleration] <= lateralAcceleration)
                        break;
                    else if (longitudinalAccelerationMode == "Accelerating" &&
                        LateralAccelerations[iAcceleration] <= lateralAcceleration &&
                        LateralAccelerations[iNextAcceleration] >= lateralAcceleration)
                        break;
                }
            }
            else
            {
                // Gets the index of the maximum or minimum value, depending on the acceleration sign
                if (lateralAcceleration > 0) iAcceleration = LateralAccelerations.IndexOf(LateralAccelerations.Max());
                else iAcceleration = LateralAccelerations.IndexOf(LateralAccelerations.Min());
            }
            // Returns the index of the lateral acceleration interval
            return iAcceleration;
        }
    }
}
