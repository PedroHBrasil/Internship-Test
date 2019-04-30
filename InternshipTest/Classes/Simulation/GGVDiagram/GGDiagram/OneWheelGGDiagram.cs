using MathNet.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternshipTest.Simulation
{
    /// <summary>
    /// Contains the maximum possible accelerations of a car's setup for a given speed.
    /// </summary>
    public class OneWheelGGDiagram : GGDiagram
    {
        #region Fields
        /// <summary>
        /// Aerodynamic properties obtained via interpolation of the car's aerodynamic map.
        /// </summary>
        private Vehicle.OneWheelAerodynamicMapPoint interpolatedAerodynamicMapPoint;
        /// <summary>
        /// Vertical load at the tire [N].
        /// </summary>
        private double tireFz;
        /// <summary>
        /// Tire's longitudnal slip.
        /// </summary>
        private double kappa;
        /// <summary>
        /// Tire's slip angle [rad].
        /// </summary>
        private double alpha;
        /// <summary>
        /// Reference longitudinal force used in the optimization algorithms to find the longitudinal slip associated with a given force [N].
        /// </summary>
        private double referenceFx;
        #endregion
        #region Properties
        /// <summary>
        /// Car and setup for which the GG diagram is generated.
        /// </summary>
        public Vehicle.OneWheelCar Car { get; set; }
        #endregion
        #region Constructors
        public OneWheelGGDiagram() { }

        public OneWheelGGDiagram(Vehicle.OneWheelCar car)
        {
            Car = car;
            LongitudinalAccelerations = new List<double>();
            LateralAccelerations = new List<double>();
            Curvatures = new List<double>();
        }

        public OneWheelGGDiagram(double speed, Vehicle.OneWheelCar car, int amountOfPoints, int amountOfDirections)
        {
            Speed = speed;
            Car = car;
            AmountOfPoints = amountOfPoints;
            AmountOfDirections = amountOfDirections;

            LongitudinalAccelerations = new List<double>();
            LateralAccelerations = new List<double>();
            Curvatures = new List<double>();
        }
        #endregion
        #region Methods
        /// <summary>
        /// Generates the GG Diagram.
        /// </summary>
        public void GenerateGGDiagram()
        {
            // Longitudinal Slip and Slip Angle initial values
            kappa = 0; alpha = 0;
            // Gets the aerodynamic coefficients
            interpolatedAerodynamicMapPoint = Car.GetAerodynamicCoefficients(Speed);
            // Gets the lift force
            double liftForce = -interpolatedAerodynamicMapPoint.LiftCoefficient * Car.Aerodynamics.FrontalArea * Car.Aerodynamics.AirDensity * Math.Pow(Speed, 2) / 2;
            // Tire resultant Fz
            tireFz = (liftForce + Car.Inertia.TotalMass * Car.Inertia.Gravity) / 4;
            // Slip angle optimization for minimum tire Fy in pureslip
            double alphaMin = _SlipAngleForFYMinimizationInPureSlip();
            // Longitudinal Slip optimization for minimum tire Fx in pureslip
            double kappaBrake = _LongitudinalSlipForFXMinimizationInPureSlip();
            // Longitudinal Slip optimization for maximum tire Fx in pureslip
            double kappaAccel = _LongitudinalSlipForFXMaximizationInPureSlip();
            // Generates the vectors of longitudinal slip and slip angle
            double[] kappas = Generate.LinearSpaced(AmountOfPoints / 4, kappaBrake, kappaAccel);
            double[] alphas = Generate.LinearSpaced(AmountOfPoints / 8, 0, alphaMin);
            // Generates the GG diagram
            _GetGGDiagramAccelerations(kappas, alphas);
        }
        #region GGDiagramAccelerations
        /// <summary>
        /// Gets the GG diagram's accelerations.
        /// </summary>
        /// <param name="kappas"> Longitudinal slips </param>
        /// <param name="alphas"> Slip angles [rad] </param>
        private void _GetGGDiagramAccelerations(double[] kappas, double[] alphas)
        {
            // Extreme left cornering + braking region
            foreach (double currentAlpha in alphas)
            {
                // Slip angle update
                alpha = currentAlpha;
                // Longitudinal Slip for minimum longitudinal force
                kappa = _LongitudinalSlipForFXMinimizationInPureSlip();
                // Accelerations determination
                _GetAccelerationsForMinimumLongitudinalAndFixedSlipAngle();
            }
            // Extreme left cornering region
            foreach (double currentKappa in kappas)
            {
                // Longitudinal Slip update
                kappa = currentKappa;
                // Slip Angle for minimum lateral force
                alpha = _SlipAngleForFYMinimizationInPureSlip();
                // Which is the sign of the longitudinal slip?
                if (kappa >= 0) _GetAccelerationsForMaximumLongitudinalAndFixedSlipAngle();
                else _GetAccelerationsForMinimumLongitudinalAndFixedSlipAngle();
            }
            // Extreme left cornering + accelerating region
            foreach (double currentAlpha in alphas.Reverse())
            {
                // Slip angle update
                alpha = currentAlpha;
                // Longitudinal Slip for maximum longitudinal force
                kappa = _LongitudinalSlipForFXMaximizationInPureSlip();
                // Accelerations determination
                _GetAccelerationsForMaximumLongitudinalAndFixedSlipAngle();
            }
            // Mirroring to get the right cornering side
            for (int iAcceleration = LongitudinalAccelerations.Count - 2; iAcceleration > 0; iAcceleration--)
            {
                LongitudinalAccelerations.Add(LongitudinalAccelerations[iAcceleration]);
                LateralAccelerations.Add(-LateralAccelerations[iAcceleration]);
            }
            // Filter the points by the directions
            _FilterGGDiagramByDirections();
        }
        /// <summary>
        /// Filters the GG diagram so that there is one acceleration per direction.
        /// </summary>
        private void _FilterGGDiagramByDirections()
        {
            // Current accelerations arrays
            double[] currentLongitudinalAccelerations = LongitudinalAccelerations.ToArray();
            double[] currentLateralAccelerations = LateralAccelerations.ToArray();
            // Current accelerations directions array
            double[] currentAccelerationsDirections = new double[LongitudinalAccelerations.Count];
            for (int i = 0; i < LongitudinalAccelerations.Count; i++)
            {
                currentAccelerationsDirections[i] = Math.Atan2(LongitudinalAccelerations[i], LateralAccelerations[i]);
            }
            // Target directions array
            double[] targetDirections = Generate.LinearSpaced(LongitudinalAccelerations.Count + 1, -Math.PI, Math.PI);
            // New accelerations arrays
            double[] newLongitudinalAccelerations = new double[targetDirections.Length - 1];
            double[] newLateralAccelerations = new double[targetDirections.Length - 1];
            // New accelerations arrays determination
            for (int iTargetDirection = 0; iTargetDirection < targetDirections.Length - 1; iTargetDirection++)
            {
                // Indexes of the current accelerations in range
                List<int> indexesOfTheCurrentAccelerationsInRange = new List<int>();
                List<double> magnitudesOfTheCurrentAccelerationsInRange = new List<double>();
                for (int iCurrentAcceleration = 0; iCurrentAcceleration < currentAccelerationsDirections.Length; iCurrentAcceleration++)
                {
                    // Checks if te current acceleration's direction is in the current target direction interval
                    if (currentAccelerationsDirections[iCurrentAcceleration] >= targetDirections[iTargetDirection] && currentAccelerationsDirections[iCurrentAcceleration] < targetDirections[iTargetDirection + 1])
                    {
                        indexesOfTheCurrentAccelerationsInRange.Add(iCurrentAcceleration);
                        magnitudesOfTheCurrentAccelerationsInRange.Add(Math.Sqrt(Math.Pow(LongitudinalAccelerations[iCurrentAcceleration], 2) + Math.Pow(LateralAccelerations[iCurrentAcceleration], 2)));
                    }
                }
                // Determination of the new acceleration
                if (indexesOfTheCurrentAccelerationsInRange.Count > 0)
                {
                    int newAccelerationIndexInIndexesArray = magnitudesOfTheCurrentAccelerationsInRange.IndexOf(magnitudesOfTheCurrentAccelerationsInRange.Max());
                    int newAccelerationIndex = indexesOfTheCurrentAccelerationsInRange[newAccelerationIndexInIndexesArray];
                    newLongitudinalAccelerations[iTargetDirection] = LongitudinalAccelerations[newAccelerationIndex];
                    newLateralAccelerations[iTargetDirection] = LateralAccelerations[newAccelerationIndex];
                }
            }
            // Checks if there are any non assigned values in the accelerations array and fills them by interpolation
            for (int iDirection = 0; iDirection < targetDirections.Length - 1; iDirection++)
            {
                if (newLongitudinalAccelerations[iDirection] == 0)
                {
                    // Previous acceleration index
                    int iPreviousAccel;
                    if (iDirection > 0) iPreviousAccel = iDirection - 1;
                    else iPreviousAccel = targetDirections.Length - 1;
                    // Next acceleration index
                    int iNextAccel = iDirection;
                    do
                    {
                        iNextAccel++;
                        if (iNextAccel == targetDirections.Length)
                        {
                            iNextAccel = 0;
                        }
                    } while (newLongitudinalAccelerations[iNextAccel] == 0 && newLateralAccelerations[iNextAccel] == 0 && iNextAccel != targetDirections.Length);
                    // Interpolates to get the accelerations in this direction
                    double interpolationRatio = (iDirection - iPreviousAccel) / (double)(iNextAccel - iPreviousAccel);
                    newLongitudinalAccelerations[iDirection] = newLongitudinalAccelerations[iPreviousAccel] + interpolationRatio * (newLongitudinalAccelerations[iNextAccel] - newLongitudinalAccelerations[iPreviousAccel]);
                    newLateralAccelerations[iDirection] = newLateralAccelerations[iPreviousAccel] + interpolationRatio * (newLateralAccelerations[iNextAccel] - newLateralAccelerations[iPreviousAccel]);
                }
            }
            // Writes the interpolated values to the lists
            LongitudinalAccelerations = newLongitudinalAccelerations.ToList();
            LateralAccelerations = newLateralAccelerations.ToList();
        }

        /// <summary>
        /// Gets the longitudinal and lateral accelerations for a fixed and slip angle and minimum longitudinal force.
        /// </summary>
        private void _GetAccelerationsForMinimumLongitudinalAndFixedSlipAngle()
        {
            // Calculates the wheel radius [mm]
            double wheelRadius = Car.Tire.TireModel.RO - tireFz / Car.Tire.VerticalStiffness;
            // Maximum appliable torque due to tire grip [Nm]
            double limitTorqueDueToGrip = Car.Tire.TireModel.GetTireFx(kappa, alpha, tireFz, 0, Speed) * wheelRadius * 4;
            // Rolling resistance moment [Nm]
            double tireMy = Car.Tire.TireModel.GetTireMy(0, alpha, tireFz, 0, Speed);
            // Wheel center angular speed [rad/s]
            double wheelCenterAngularSpeed = Speed / wheelRadius;
            // Wheel braking torque curve interpolation
            alglib.spline1dbuildlinear(Car.WheelRotationalSpeedCurve.ToArray(), Car.WheelBrakingTorqueCurve.ToArray(), out alglib.spline1dinterpolant wheelBrakingTorqueInterp);
            double powertrainBrakingTorque = alglib.spline1dcalc(wheelBrakingTorqueInterp, wheelCenterAngularSpeed) + tireMy * Car.Transmission.AmountOfDrivenWheels;
            // Limit torque due to brakes 
            double limitTorqueDueToBrakes = -Car.Brakes.MaximumTorque + powertrainBrakingTorque + tireMy * 4;
            // Is the torque limited by the brakes or by the tire grip?
            if (limitTorqueDueToGrip < limitTorqueDueToBrakes)
            {
                referenceFx = limitTorqueDueToBrakes / wheelRadius / 4;
                kappa = _GetLongitudinalSlipForGivenWheelTorque();
            }
            // Aerodynamic drag force [N]
            double dragForce = -interpolatedAerodynamicMapPoint.DragCoefficient * Car.Aerodynamics.FrontalArea * Car.Aerodynamics.AirDensity * Math.Pow(Speed, 2) / 2;
            // Car's resultant longitudinal force [N]
            double carLongitudinalForce = Car.Tire.TireModel.GetTireFx(kappa, alpha, tireFz, 0, Speed) * 4 + dragForce;
            // Car's resultant lateral force [N]
            double carLateralForce = Car.Tire.TireModel.GetTireFy(kappa, alpha, tireFz, 0, Speed) * 4;
            // Inertia efficiency (due to rotational parts moment of inertia)
            double inertiaEfficiency = Math.Pow(wheelRadius, 2) * Car.Inertia.TotalMass /
                (Math.Pow(wheelRadius, 2) * Car.Inertia.TotalMass + Car.Inertia.RotPartsMI);
            // Car's longitudinal acceleration [G]
            double longitudinalAcceleration = (carLongitudinalForce / Car.Inertia.TotalMass) * inertiaEfficiency;
            // Car's lateral acceleration [G]
            double lateralAcceleration = carLateralForce / Car.Inertia.TotalMass;
            // Result
            LongitudinalAccelerations.Add(longitudinalAcceleration);
            LateralAccelerations.Add(lateralAcceleration);
        }
        /// <summary>
        /// Gets the longitudinal and lateral accelerations for a fixed and slip angle and maximum longitudinal force.
        /// </summary>
        private void _GetAccelerationsForMaximumLongitudinalAndFixedSlipAngle()
        {
            // Calculates the wheel radius [mm]
            double wheelRadius = Car.Tire.TireModel.RO - tireFz / Car.Tire.VerticalStiffness;
            // Maximum appliable torque due to tire grip [Nm]
            double limitTorqueDueToGrip = Car.Tire.TireModel.GetTireFx(kappa, alpha, tireFz, 0, Speed) * wheelRadius * Car.Transmission.AmountOfDrivenWheels;
            // Rolling resistance moment [Nm]
            double tireMy = Car.Tire.TireModel.GetTireMy(0, alpha, tireFz, 0, Speed);
            // Wheel center angular speed [rpm]
            double wheelCenterAngularSpeed = Speed / wheelRadius;
            // Wheel torque curve interpolation
            alglib.spline1dbuildlinear(Car.WheelRotationalSpeedCurve.ToArray(), Car.WheelTorqueCurve.ToArray(), out alglib.spline1dinterpolant wheelTorqueInterp);
            double limitTorqueDueToPowertrain = alglib.spline1dcalc(wheelTorqueInterp, wheelCenterAngularSpeed) + tireMy * Car.Transmission.AmountOfDrivenWheels;
            // Is the torque limited by the engine or by the tire grip?
            if (limitTorqueDueToGrip > limitTorqueDueToPowertrain)
            {
                referenceFx = limitTorqueDueToPowertrain / wheelRadius / Car.Transmission.AmountOfDrivenWheels;
                kappa = _GetLongitudinalSlipForGivenWheelTorque();
            }
            // Aerodynamic drag force [N]
            double dragForce = -interpolatedAerodynamicMapPoint.DragCoefficient * Car.Aerodynamics.FrontalArea * Car.Aerodynamics.AirDensity * Math.Pow(Speed, 2) / 2;
            // Car's resultant longitudinal force [N]
            double carLongitudinalForce = Car.Tire.TireModel.GetTireFx(kappa, alpha, tireFz, 0, Speed) * Car.Transmission.AmountOfDrivenWheels + dragForce +
                tireMy / wheelRadius * (4 - Car.Transmission.AmountOfDrivenWheels);
            // Car's resultant lateral force [N]
            double carLateralForce;
            if (Car.Transmission.AmountOfDrivenWheels == 2)
                carLateralForce = (Car.Tire.TireModel.GetTireFy(0, alpha, tireFz, 0, Speed) + Car.Tire.TireModel.GetTireFy(kappa, alpha, tireFz, 0, Speed)) * 2;
            else
                carLateralForce = Car.Tire.TireModel.GetTireFy(kappa, alpha, tireFz, 0, Speed) * 4;
            // Inertia efficiency (due to rotational parts moment of inertia)
            double inertiaEfficiency = Math.Pow(wheelRadius, 2) * Car.Inertia.TotalMass /
                (Math.Pow(wheelRadius, 2) * Car.Inertia.TotalMass + Car.Inertia.RotPartsMI);
            // Car's longitudinal acceleration [G]
            double longitudinalAcceleration = (carLongitudinalForce / Car.Inertia.TotalMass) * inertiaEfficiency;
            // Car's lateral acceleration [G]
            double lateralAcceleration = carLateralForce / Car.Inertia.TotalMass;
            // Result
            LongitudinalAccelerations.Add(longitudinalAcceleration);
            LateralAccelerations.Add(lateralAcceleration);
        }
        #endregion
        #region TireForcesOptimization
        /// <summary>
        /// Gets the longitudinal slip for fixed slip angle and a given wheel torque.
        /// </summary>
        /// <returns> Longitudinal slip </returns>
        private double _GetLongitudinalSlipForGivenWheelTorque()
        {
            // Optimization parameters
            double epsg = 1e-10;
            double epsf = 0;
            double epsx = 0;
            double diffstep = 1.0e-6;
            int maxits = 100;
            
            double[] bndl = new double[] { Car.Tire.TireModel.KappaMin };
            double[] bndu = new double[] { Car.Tire.TireModel.KappaMax };
            
            double[] kappaAccel = new double[] { 0 };
            
            alglib.minbleiccreatef(kappaAccel, diffstep, out alglib.minbleicstate state);
            alglib.minbleicsetbc(state, bndl, bndu);
            alglib.minbleicsetcond(state, epsg, epsf, epsx, maxits);
            alglib.minbleicoptimize(state, _LongitudinalSlipForGivenWheelTorqueOptimization, null, null);
            alglib.minbleicresults(state, out kappaAccel, out alglib.minbleicreport rep);
            
            return kappaAccel[0];
        }
        /// <summary>
        /// Gets the slip angle which corresponds to the minimum tire lateral force for zero longitudinal slip.
        /// </summary>
        /// <returns> Slip angle [rad] </returns>
        private double _SlipAngleForFYMinimizationInPureSlip()
        {
            // Optimization parameters
            double epsg = 1e-10;
            double epsf = 0;
            double epsx = 0;
            double diffstep = 1.0e-6;
            int maxits = 100;

            double[] bndl = new double[] { Car.Tire.TireModel.AlphaMin * Math.PI / 180 };
            double[] bndu = new double[] { Car.Tire.TireModel.AlphaMax * Math.PI / 180 };
            
            double[] alpha = new double[] { 0 };
            
            alglib.minbleiccreatef(alpha, diffstep, out alglib.minbleicstate state);
            alglib.minbleicsetbc(state, bndl, bndu);
            alglib.minbleicsetcond(state, epsg, epsf, epsx, maxits);
            alglib.minbleicoptimize(state, _SlipAngleForMinimumTireFY, null, null);
            alglib.minbleicresults(state, out alpha, out alglib.minbleicreport rep);
            
            return alpha[0];
        }
        /// <summary>
        /// Gets the longitudinal slip which corresponds to the minimum tire longitudinal force and zero slip angle.
        /// </summary>
        /// <returns> Longitudinal slip </returns>
        private double _LongitudinalSlipForFXMinimizationInPureSlip()
        {
            // Optimization parameters
            double epsg = 1e-10;
            double epsf = 0;
            double epsx = 0;
            double diffstep = 1.0e-6;
            int maxits = 100;

            double[] bndl = new double[] { Car.Tire.TireModel.KappaMin };
            double[] bndu = new double[] { Car.Tire.TireModel.KappaMax };

            double[] kappaBrake = new double[] { 0 };

            alglib.minbleiccreatef(kappaBrake, diffstep, out alglib.minbleicstate state);
            alglib.minbleicsetbc(state, bndl, bndu);
            alglib.minbleicsetcond(state, epsg, epsf, epsx, maxits);
            alglib.minbleicoptimize(state, _LongitudinalSlipForMinimumTireFX, null, null);
            alglib.minbleicresults(state, out kappaBrake, out alglib.minbleicreport rep);

            return kappaBrake[0];
        }
        /// <summary>
        /// Gets the longitudinal slip which corresponds to the maximum tire longitudinal force and zero slip angle.
        /// </summary>
        /// <returns> Longitudinal slip </returns>
        private double _LongitudinalSlipForFXMaximizationInPureSlip()
        {
            // Optimization parameters
            double epsg = 1e-10;
            double epsf = 0;
            double epsx = 0;
            double diffstep = 1.0e-6;
            int maxits = 100;

            double[] bndl = new double[] { Car.Tire.TireModel.KappaMin };
            double[] bndu = new double[] { Car.Tire.TireModel.KappaMax };

            double[] kappaAccel = new double[] { 0 };

            alglib.minbleiccreatef(kappaAccel, diffstep, out alglib.minbleicstate state);
            alglib.minbleicsetbc(state, bndl, bndu);
            alglib.minbleicsetcond(state, epsg, epsf, epsx, maxits);
            alglib.minbleicoptimize(state, _LongitudinalSlipForMaximumTireFX, null, null);
            alglib.minbleicresults(state, out kappaAccel, out alglib.minbleicreport rep);

            return kappaAccel[0];
        }
        /// <summary>
        /// Optimization function to get the longitudinal slip for a given torque.
        /// </summary>
        /// <param name="kappa"> Longitudinal Slip </param>
        /// <param name="tireFx"> Longitudinal Force </param>
        /// <param name="obj"></param>
        private void _LongitudinalSlipForGivenWheelTorqueOptimization(double[] kappa, ref double tireFx, object obj)
        {
            tireFx = Math.Abs(Car.Tire.TireModel.GetTireFx(kappa[0], alpha, tireFz, 0, Speed) - referenceFx);
        }
        /// <summary>
        /// Optimization function to get the slip angle for minimum lateral force and zero longitudinal slip.
        /// </summary>
        /// <param name="alpha"> Slip angle [rad] </param>
        /// <param name="tireFy"> Lateral force [N] </param>
        /// <param name="obj"></param>
        private void _SlipAngleForMinimumTireFY(double[] alpha, ref double tireFy, object obj)
        {
            tireFy = Car.Tire.TireModel.GetTireFy(kappa, alpha[0], tireFz, 0, Speed);
        }
        /// <summary>
        /// Optimization function to get the longitudinal slip for minimum lonitudinal force and zero slip angle.
        /// </summary>
        /// <param name="kappa"> Longitudinal Slip </param>
        /// <param name="tireFx"> Longitudinal Force </param>
        /// <param name="obj"></param>
        private void _LongitudinalSlipForMinimumTireFX(double[] kappa, ref double tireFx, object obj)
        {
            tireFx = Car.Tire.TireModel.GetTireFx(kappa[0], alpha, tireFz, 0, Speed);
        }
        /// <summary>
        /// Optimization function to get the longitudinal slip for maximum lonitudinal force and zero slip angle.
        /// </summary>
        /// <param name="kappa"> Longitudinal Slip </param>
        /// <param name="tireFx"> Longitudinal Force </param>
        /// <param name="obj"></param>
        private void _LongitudinalSlipForMaximumTireFX(double[] kappa, ref double tireFx, object obj)
        {
            tireFx = -Car.Tire.TireModel.GetTireFx(kappa[0], alpha, tireFz, 0, Speed);
        }
        #endregion
        /// <summary>
        /// Gets the longitudinal acceleration based on the interpolation of the GG diagram by a lateral acceleration.
        /// </summary>
        /// <param name="lateralAcceleration"> Lateral acceleration [m/s²] </param>
        /// <param name="longitudinalAccelerationMode"> "Accelerating" or "Braking" </param>
        /// <returns></returns>
        public double GetLongitudinalAccelerationViaInterpolationBasedOnLateralAcceleration(double lateralAcceleration, string longitudinalAccelerationMode)
        {
            // Gets the lateral acceleration interval index
            int iAcceleration = _GetLateralAccelerationIndex(lateralAcceleration, longitudinalAccelerationMode);
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
        /// <summary>
        /// Gets the index of the laterala acceleration to be used in the interpolation.
        /// </summary>
        /// <param name="lateralAcceleration"> Lateral acceleration [m/s²] </param>
        /// <param name="longitudinalAccelerationMode"> "Accelerating" or "Braking" </param>
        /// <returns></returns>
        private int _GetLateralAccelerationIndex(double lateralAcceleration, string longitudinalAccelerationMode)
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
                    if (iAcceleration == LateralAccelerations.Count - 1) iNextAcceleration = 0;
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
        #endregion
    }
}
