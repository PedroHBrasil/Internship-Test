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
    public class GGDiagram
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
        /// Speed at which the accelrations are calculated [m/s].
        /// </summary>
        public double Speed { get; set; }
        /// <summary>
        /// Car and setup for which the GG diagram is generated.
        /// </summary>
        public Vehicle.OneWheelCar Car { get; set; }
        /// <summary>
        /// Amount of points of the GG diagram.
        /// </summary>
        public int AmountOfPoints { get; set; }
        /// <summary>
        /// Amount of directions of the GG diagram accelerations.
        /// </summary>
        public int AmountOfDirections { get; set; }
        /// <summary>
        /// Longitudinal accelerations [m/s²].
        /// </summary>
        public List<double> LongitudinalAccelerations { get; set; }
        /// <summary>
        /// Lateral accelerations [m/s²].
        /// </summary>
        public List<double> LateralAccelerations { get; set; }
        /// <summary>
        /// Curvatures [1/m].
        /// </summary>
        public List<double> Curvatures { get; set; }
        #endregion
        #region Constructors
        public GGDiagram() { }

        public GGDiagram(Vehicle.OneWheelCar car)
        {
            Car = car;
            LongitudinalAccelerations = new List<double>();
            LateralAccelerations = new List<double>();
            Curvatures = new List<double>();
        }

        public GGDiagram(double speed, Vehicle.OneWheelCar car, int amountOfPoints, int amountOfDirections)
        {
            Speed = speed;
            Car = car;
            AmountOfPoints = amountOfPoints;
            AmountOfDirections = amountOfDirections;

            LongitudinalAccelerations = new List<double>();
            LateralAccelerations = new List<double>();
            Curvatures = new List<double>();

            _GenerateGGDiagram();
            GetAssociatedCurvatures();
        }
        #endregion
        #region Methods
        /// <summary>
        /// Generates the GG Diagram.
        /// </summary>
        private void _GenerateGGDiagram()
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
            double limitTorqueDueToBrakes = -Car.Brakes.MaxTorque + powertrainBrakingTorque + tireMy * 4;
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
        /// Gets the curvatures associated with each point of the GG diagram.
        /// </summary>
        public void GetAssociatedCurvatures()
        {
            // Gets and registers the curvatures associated with each point of the GG diagram
            for (int iLateralAcceleration = 0; iLateralAcceleration < LateralAccelerations.Count; iLateralAcceleration++)
            {
                // Corrects the speed if it is equal to zero
                double currentSpeed = Speed;
                if (currentSpeed == 0) currentSpeed = 0.1;
                Curvatures.Add(LateralAccelerations[iLateralAcceleration] * Car.Inertia.Gravity / Math.Pow(currentSpeed, 2));
            }
        }
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
/*
public class GGDiagram
{
    #region Fields
    private double currentDirection;
    private double inertiaEfficiency;
    private double liftForce;
    private double dragForce;
    private double tireVerticalLoad;
    private double maximumThrottleActuation;
    private double maximumBrakePedalActuation;
    private double targetLongitudinalForce;
    private double currentAlpha;
    private double currentPedalActuation;
    #endregion
    #region Properties
    /// <summary>
    /// Speed at which the accelrations are calculated [m/s].
    /// </summary>
    public double Speed { get; set; }
    /// <summary>
    /// Car and setup for which the GG diagram is generated.
    /// </summary>
    public Vehicle.Car Car { get; set; }
    /// <summary>
    /// Amount of points of the GG diagram.
    /// </summary>
    public int AmountOfPoints { get; set; }
    /// <summary>
    /// Longitudinal accelerations [m/s²].
    /// </summary>
    public List<double> LongitudinalAccelerations { get; set; }
    /// <summary>
    /// Lateral accelerations [m/s²].
    /// </summary>
    public List<double> LateralAccelerations { get; set; }
    public List<double> Curvatures { get; set; }
    #endregion
    #region Constructors
    public GGDiagram() { }

    public GGDiagram(Vehicle.Car car)
    {
        Car = car;
    }

    public GGDiagram(double speed, Vehicle.Car car, int amountOfPoints)
    {
        Speed = speed;
        Car = car;
        AmountOfPoints = amountOfPoints / 4 * 4;
        _GenerateGGDiagram();
    }
    #endregion
    #region Methods
    /// <summary>
    /// Gets the GG diagram's points.
    /// </summary>
    private void _GenerateGGDiagram()
    {
        // Initializes the accelerations lists
        LongitudinalAccelerations = new List<double>();
        LateralAccelerations = new List<double>();
        Curvatures = new List<double>();
        // Generates the directions vector
        double[] directions = Generate.LinearSpaced(AmountOfPoints, -Math.PI, Math.PI * (2 * Math.PI / AmountOfPoints));
        // Gets the aerodynamic forces and the tire's vertical load
        Vehicle.AerodynamicMapPoint aerodynamicMapPoint = Car.GetAerodynamicCoefficients(Speed);
        dragForce = aerodynamicMapPoint.DragCoefficient * Car.Aerodynamics.AirDensity * Car.Aerodynamics.FrontalArea * Math.Pow(Speed, 2) / 2;
        liftForce = aerodynamicMapPoint.LiftCoefficient * Car.Aerodynamics.AirDensity * Car.Aerodynamics.FrontalArea * Math.Pow(Speed, 2) / 2;
        tireVerticalLoad = (Car.Inertia.TotalMass * Car.Inertia.Gravity - liftForce) / 4;
        // Gets the maximum throttle and brake pedal actuations
        _GetMaximumThrottleActuation();
        _GetMaximumBrakePedalActuation();
        // Gets the inertia efficiency (share of energy which actually accelerates the car in longitudinal motion)
        _GetInertiaEfficiency();
        // Generates the GG diagram points for each direction
        for (int iDirection = 0; iDirection < AmountOfPoints; iDirection++)
        {
            currentDirection = directions[iDirection];
            double[] slipAngleAndPedalActuation = _GetSlipAngleAndPedalActuation_Test();
            double[] accelerations = _GetAccelerationsFromSlipAngleAndPedalActuation(slipAngleAndPedalActuation);
            LongitudinalAccelerations.Add(accelerations[0]);
            LateralAccelerations.Add(accelerations[1]);
        }
    }
    public void GetAssociatedCurvatures()
    {

    }

    public double GetLongitudinalAccelerationViaInterpolationBasedOnLateralAcceleration(double dddd, string mode)
    {
        return 0;
    }

    /// <summary>
    /// Calculates the share of energy which is spent in longitudinal movement of the car. The remainder is spent in the rotation of the rotational parts.
    /// </summary>
    private void _GetInertiaEfficiency()
    {
        // Wheel radius [m]
        double wheelRadius = Car.Tire.TireModel.RO - tireVerticalLoad / Car.Tire.VerticalStiffness;
        // Inertia Efficiency (ratio)
        inertiaEfficiency = Math.Pow(wheelRadius, 2) * Car.Inertia.TotalMass /
            (Math.Pow(wheelRadius, 2) * Car.Inertia.TotalMass + Car.Inertia.RotPartsMI);
    }

    /// <summary>
    /// Gets the accelerations from the slip angle and pedal actuation.
    /// </summary>
    /// <param name="slipAngleAndPedalActuation"> [0]: Slip angle [rad] - [1]: Pedal Actuation [ratio] </param>
    /// <returns> Accelerations: [0]: Longitudinal - [1]: Lateral </returns>
    private double[] _GetAccelerationsFromSlipAngleAndPedalActuation(double[] slipAngleAndPedalActuation)
    {
        // Splits the inputs
        double alpha = slipAngleAndPedalActuation[0];
        double longitudinalForceSourceActuation = slipAngleAndPedalActuation[1];
        // Longitudinal force [N]
        double longitudinalForce;
        if (longitudinalForceSourceActuation >= 0) longitudinalForce = _GetLongitudinalForceFromThrottleActuation(longitudinalForceSourceActuation);
        else longitudinalForce = _GetLongitudinalForceFromBrakePedalActuation(longitudinalForceSourceActuation);
        // Determination of the tire's longitudinal slip
        double kappa = _GetLongitudinalSlipFromLongitudinalForce(alpha, longitudinalForce / 4);
        // Gets the lateral force [N]
        double lateralForce = Car.Tire.TireModel.GetTireFy(kappa, alpha, tireVerticalLoad, 0, Speed);
        // Accelerations
        double[] accelerations = new double[2]
        {
                (longitudinalForce - dragForce) / Car.Inertia.TotalMass * inertiaEfficiency,
                lateralForce / Car.Inertia.TotalMass
        };
        return accelerations;
    }
    #region MaximumThrottleActuationDetermination
    /// <summary>
    /// Gets the maximum throttle actuation by comparing the tire and powertrain maximum longitudinal forces.
    /// </summary>
    private void _GetMaximumThrottleActuation()
    {
        // Gets the maximum longitudinal forces that can be given by the tire and the powertrain.
        double maximumLongitudinalForceFromPowertrain = _GetMaximumLongitudinalForceFromPowertrain();
        double maximumLongitudinalForceFromTire = _GetMaximumLongitudinalForceFromTire();
        // Finds the tire to powertrain maximum longitudinal force ratio.
        maximumThrottleActuation = maximumLongitudinalForceFromTire / maximumLongitudinalForceFromPowertrain;
        // If the maximum of the tire is higher than the maximum of the powertrain, the maximum throttle actuation is corrected to be 1. Otherwise, the maximum throttle actuation is equal to the maximum longitudinal force ratio. 
        if (maximumThrottleActuation > 1) maximumThrottleActuation = 1;
    }
    /// <summary>
    /// Determinates the powertrain's maximum longitudinal force by interpolation of the car's equivalent wheel torque curve.
    /// </summary>
    /// <returns> The maximum longitudinal force that the powertrain can provide [N] </returns>
    private double _GetMaximumLongitudinalForceFromPowertrain()
    {
        // Wheel radius [m]
        double wheelRadius = Car.Tire.TireModel.RO - tireVerticalLoad / Car.Tire.VerticalStiffness;
        // Wheel rotational speed [rad/s]
        double wheelRotationalSpeed = Speed / wheelRadius;
        // Powertrain torque curve interpolation object
        alglib.spline1dbuildlinear(Car.WheelRotationalSpeedCurve.ToArray(), Car.WheelTorqueCurve.ToArray(), out alglib.spline1dinterpolant powertrainTorqueInterpolationObject);
        // Powertrain torque [N*m]
        double powertrainTorque = alglib.spline1dcalc(powertrainTorqueInterpolationObject, wheelRotationalSpeed);
        // Longitudinal Force [N]
        double longitudinalForce = powertrainTorque / wheelRadius * Car.Transmission.AmountOfDrivenWheels;

        return longitudinalForce;
    }
    /// <summary>
    /// Runs the an optimization to find the maximum longitudinal force that the tire supports.
    /// </summary>
    /// <returns> Tire's longitudinal force [N] </returns>
    private double _GetMaximumLongitudinalForceFromTire()
    {
        // Optimization parameters
        double epsg = 1e-10;
        double epsf = 0;
        double epsx = 0;
        double diffstep = 1.0e-6;
        int maxits = 100;
        double[] bndl = new double[] { 0 };
        double[] bndu = new double[] { .2 };

        double[] kappa = new double[] { 0 };

        alglib.minbleiccreatef(kappa, diffstep, out alglib.minbleicstate state);
        alglib.minbleicsetbc(state, bndl, bndu);
        alglib.minbleicsetcond(state, epsg, epsf, epsx, maxits);
        alglib.minbleicoptimize(state, _LongitudinalSlipForMaximumTireLongitudinalForce, null, null);
        alglib.minbleicresults(state, out kappa, out alglib.minbleicreport rep);

        // Tire's longitudinal force [N]
        double longitudinalForce = Car.Tire.TireModel.GetTireFx(kappa[0], 0, tireVerticalLoad, 0, Speed) * Car.Transmission.AmountOfDrivenWheels;

        return longitudinalForce;
    }
    /// <summary>
    /// Used to find the longitudinal slip for maximum longitudinal force in pure slip.
    /// </summary>
    /// <param name="kappa"> Longitudinal slip </param>
    /// <param name="longitudinalForce"> Tire's longitudinal force [N] </param>
    /// <param name="obj"></param>
    private void _LongitudinalSlipForMaximumTireLongitudinalForce(double[] kappa, ref double longitudinalForce, object obj)
    {
        longitudinalForce = -Car.Tire.TireModel.GetTireFx(kappa[0], 0, tireVerticalLoad, 0, Speed);
    }
    #endregion
    #region MaximumBrakePedalActuationDetermination
    /// <summary>
    /// Gets the maximum throttle actuation by comparing the tire and powertrain maximum longitudinal forces.
    /// </summary>
    private void _GetMaximumBrakePedalActuation()
    {
        // Gets the minimum longitudinal forces that can be given by the tire and the brakes.
        double minimumLongitudinalForceFromBrakes = _GetMinimumLongitudinalForceFromBrakes();
        double minimumLongitudinalForceFromTire = _GetMinimumLongitudinalForceFromTire();
        // Finds the tire to brakes maximum longitudinal force ratio.
        maximumBrakePedalActuation = minimumLongitudinalForceFromTire / minimumLongitudinalForceFromBrakes;
        // If the minimum of the tire is lower than the minimum of the brakes, the maximum brake pedal actuation is corrected to be 1. Otherwise, the maximum brake pedal actuation is equal to the minimum longitudinal force ratio. 
        if (maximumBrakePedalActuation > 1) maximumBrakePedalActuation = 1;
    }
    /// <summary>
    /// Determinates the brake's minimum longitudinal force by interpolation of the car's equivalent wheel braking torque curve and the maximum torque appliable by the brake system.
    /// </summary>
    /// <returns> The maximum longitudinal force that the powertrain can provide [N] </returns>
    private double _GetMinimumLongitudinalForceFromBrakes()
    {
        // Wheel radius [m]
        double wheelRadius = Car.Tire.TireModel.RO - tireVerticalLoad / Car.Tire.VerticalStiffness;
        // Wheel rotational speed [rad/s]
        double wheelRotationalSpeed = Speed / wheelRadius;
        // Powertrain torque curve interpolation object
        alglib.spline1dbuildlinear(Car.WheelRotationalSpeedCurve.ToArray(), Car.WheelBrakingTorqueCurve.ToArray(), out alglib.spline1dinterpolant powertrainBrakeTorqueInterpolationObject);
        // Powertrain torque [N*m]
        double brakeTorque = alglib.spline1dcalc(powertrainBrakeTorqueInterpolationObject, wheelRotationalSpeed) * Car.Transmission.AmountOfDrivenWheels - Car.Brakes.MaxTorque;
        // Longitudinal Force [N]
        double longitudinalForce = brakeTorque / wheelRadius;

        return longitudinalForce;
    }
    /// <summary>
    /// Runs the an optimization to find the minimum longitudinal force that the tire supports.
    /// </summary>
    /// <returns> Tire's longitudinal force [N] </returns>
    private double _GetMinimumLongitudinalForceFromTire()
    {
        // Optimization parameters
        double epsg = 1e-10;
        double epsf = 0;
        double epsx = 0;
        double diffstep = 1.0e-6;
        int maxits = 100;
        double[] bndl = new double[] { -.2 };
        double[] bndu = new double[] { 0 };

        double[] kappa = new double[] { 0 };

        alglib.minbleiccreatef(kappa, diffstep, out alglib.minbleicstate state);
        alglib.minbleicsetbc(state, bndl, bndu);
        alglib.minbleicsetcond(state, epsg, epsf, epsx, maxits);
        alglib.minbleicoptimize(state, _LongitudinalSlipForMinimumTireLongitudinalForce, null, null);
        alglib.minbleicresults(state, out kappa, out alglib.minbleicreport rep);

        // Tire's longitudinal force [N]
        double longitudinalForce = Car.Tire.TireModel.GetTireFx(kappa[0], 0, tireVerticalLoad, 0, Speed) * 4;

        return longitudinalForce;
    }
    /// <summary>
    /// Used to find the longitudinal slip for minimum longitudinal force in pure slip.
    /// </summary>
    /// <param name="kappa"> Longitudinal slip </param>
    /// <param name="longitudinalForce"> Tire's longitudinal force [N] </param>
    /// <param name="obj"></param>
    private void _LongitudinalSlipForMinimumTireLongitudinalForce(double[] kappa, ref double longitudinalForce, object obj)
    {
        longitudinalForce = Car.Tire.TireModel.GetTireFx(kappa[0], 0, tireVerticalLoad, 0, Speed);
    }
    #endregion
    #region GG Diagram Point Determination

    private double[] _GetSlipAngleAndPedalActuation_Test()
    {
        // Optimization parameters
        double epsg = 1e-10;
        double epsf = 0;
        double epsx = 0;
        double diffstep = 1.0e-6;
        int maxits = 100;
        double[] bndl = new double[] { -Math.PI / 4 };
        double[] bndu = new double[] { Math.PI / 4 };

        double[] alpha = new double[] { 0 };

        alglib.minbleiccreatef(alpha, diffstep, out alglib.minbleicstate state);
        alglib.minbleicsetbc(state, bndl, bndu);
        alglib.minbleicsetcond(state, epsg, epsf, epsx, maxits);
        alglib.minbleicoptimize(state, _OptimizeSlipAngleForDirectionAndAcceleration, null, null);
        alglib.minbleicresults(state, out alpha, out alglib.minbleicreport rep);

        return new double[2] { alpha[0], currentPedalActuation };
    }

    private void _OptimizeSlipAngleForDirectionAndAcceleration(double[] alpha, ref double negativeSquaredResultantAcceleration, object obj)
    {
        currentAlpha = alpha[0];

        // Optimization parameters
        double epsg = 1e-10;
        double epsf = 0;
        double epsx = 0;
        double diffstep = 1.0e-6;
        int maxits = 100;
        double[] bndl = new double[] { -maximumBrakePedalActuation };
        double[] bndu = new double[] { maximumThrottleActuation };

        double[] pedalActuation = new double[] { 0 };

        alglib.minbleiccreatef(pedalActuation, diffstep, out alglib.minbleicstate state);
        alglib.minbleicsetbc(state, bndl, bndu);
        alglib.minbleicsetcond(state, epsg, epsf, epsx, maxits);
        alglib.minbleicoptimize(state, _OptimizePedalActuationForAccelerationDirection, null, null);
        alglib.minbleicresults(state, out pedalActuation, out alglib.minbleicreport rep);

        currentPedalActuation = pedalActuation[0];

        // Vehicle accelerations
        double[] accelerations = _GetAccelerationsFromSlipAngleAndPedalActuation(new double[2] { alpha[0], pedalActuation[0] });
        // Negative squared resultant acceleration 
        negativeSquaredResultantAcceleration = -(Math.Pow(accelerations[0], 2) + Math.Pow(accelerations[1], 2));
    }

    private void _OptimizePedalActuationForAccelerationDirection(double[] pedalActuation, ref double absoluteDirectionDifference, object obj)
    {
        // Longitudinal force [N]
        double longitudinalForce;
        if (pedalActuation[0] >= 0) longitudinalForce = _GetLongitudinalForceFromThrottleActuation(pedalActuation[0]);
        else longitudinalForce = _GetLongitudinalForceFromBrakePedalActuation(pedalActuation[0]);
        // Determination of the tire's longitudinal slip
        double kappa = _GetLongitudinalSlipFromLongitudinalForce(currentAlpha, longitudinalForce / 4);
        // Gets the lateral force [N]
        double lateralForce = Car.Tire.TireModel.GetTireFy(kappa, currentAlpha, tireVerticalLoad, 0, Speed);
        // Accelerations
        double longitudinalAcceleration = (longitudinalForce - dragForce) / Car.Inertia.TotalMass * inertiaEfficiency;
        double lateralAcceleration = lateralForce / Car.Inertia.TotalMass;
        // Absolute direction difference
        absoluteDirectionDifference = Math.Abs(Math.Atan2(longitudinalAcceleration, lateralAcceleration) - currentDirection);
    }

    /// <summary>
    /// Optimizes the slip angle and the pedal position (throttle/brake) to maximize the acceleration in the current direction.
    /// </summary>
    /// <returns> The slip angle [rad] and the pedal actuation [ratio] </returns>
    private double[] _GetSlipAngleAndPedalActuation()
    {
        // Optimization parameters
        double[] slipAngleAndPedalActuation;
        if (currentDirection > 0) slipAngleAndPedalActuation = new double[] { -Math.PI / 4, 0 };
        else slipAngleAndPedalActuation = new double[] { Math.PI / 4, 0 };
        double[] inputsScales = new double[] { 1, 1 };
        double[] lowerBoundaries = new double[] { -Math.PI / 4, -maximumBrakePedalActuation };
        double[] upperBoundaries = new double[] { Math.PI / 4, maximumThrottleActuation };
        double minInputStepSize = 0.000001;
        int iterationLimit = 1000;
        // Optimization setup
        alglib.minlmcreatev(2, slipAngleAndPedalActuation, 0.0001, out alglib.minlmstate state);
        alglib.minlmsetbc(state, lowerBoundaries, upperBoundaries);
        alglib.minlmsetcond(state, minInputStepSize, iterationLimit);
        alglib.minlmsetscale(state, inputsScales);
        // Optimization
        alglib.minlmoptimize(state, _SlipAngleAndPedalActuationOptimization, null, null);
        // Results extrction
        alglib.minlmresults(state, out slipAngleAndPedalActuation, out alglib.minlmreport report);

        return slipAngleAndPedalActuation;
    }

    /// <summary>
    /// Used to optimize the slip angle and pedal actuation to generate the GG Diagram's point.
    /// </summary>
    /// <param name="slipAngleAndPedalActuation"> [0]: Slip angle [rad] - [1]: Pedal Actuation [ratio] </param>
    /// <param name="optimizationTargets"> [0]: Negative of the resultant acceleration - [1]: Error in direction [rad] - [2]: Error in longitudinal slip optimization for longitudinal force [N] </param>
    /// <param name="obj"></param>
    private void _SlipAngleAndPedalActuationOptimization(double[] slipAngleAndPedalActuation, double[] optimizationTargets, object obj)
    {
        // Splits the inputs
        double alpha = slipAngleAndPedalActuation[0];
        double longitudinalForceSourceActuation = slipAngleAndPedalActuation[1];
        // Longitudinal force [N]
        double longitudinalForce;
        if (longitudinalForceSourceActuation >= 0) longitudinalForce = _GetLongitudinalForceFromThrottleActuation(longitudinalForceSourceActuation);
        else longitudinalForce = _GetLongitudinalForceFromBrakePedalActuation(longitudinalForceSourceActuation);
        // Determination of the tire's longitudinal slip
        double kappa = _GetLongitudinalSlipFromLongitudinalForce(alpha, longitudinalForce / 4);
        // Gets the error associated with kappa's optimization
        double errorKappa = Math.Abs(targetLongitudinalForce - Car.Tire.TireModel.GetTireFx(kappa, alpha, tireVerticalLoad, 0, Speed));
        // Gets the lateral force [N]
        double lateralForce = Car.Tire.TireModel.GetTireFy(kappa, alpha, tireVerticalLoad, 0, Speed);
        // Accelerations
        double longitudinalAcceleration = (longitudinalForce - dragForce) / Car.Inertia.TotalMass * inertiaEfficiency;
        double lateralAcceleration = lateralForce / Car.Inertia.TotalMass;
        // Gets the optimization targets values - [0] = Negative of resultant acceleration magnitude, [1] = resultant acceleration direction and [2] = kappa optimization error.
        optimizationTargets[0] = -Math.Sqrt(Math.Pow(longitudinalAcceleration, 2) + Math.Pow(lateralAcceleration, 2));
        optimizationTargets[1] = Math.Abs(Math.Atan2(longitudinalAcceleration, lateralAcceleration) - currentDirection);
        //optimizationTargets[2] = errorKappa;
    }
    /// <summary>
    /// Gets the longitudinal force for a given throttle pedal position.
    /// </summary>
    /// <param name="throttleActuation"> Throttle position (ratio) </param>
    /// <returns> Car's longitudinal force [N] </returns>
    private double _GetLongitudinalForceFromThrottleActuation(double throttleActuation)
    {
        // Wheel radius [m]
        double wheelRadius = Car.Tire.TireModel.RO - tireVerticalLoad / Car.Tire.VerticalStiffness;
        // Wheel rotational speed [rad/s]
        double wheelRotationalSpeed = Speed / wheelRadius;
        // Powertrain torque curve interpolation object
        alglib.spline1dbuildlinear(Car.WheelRotationalSpeedCurve.ToArray(), Car.WheelTorqueCurve.ToArray(), out alglib.spline1dinterpolant powertrainTorqueInterpolationObject);
        // Powertrain torque [N*m]
        double powertrainTorque = alglib.spline1dcalc(powertrainTorqueInterpolationObject, wheelRotationalSpeed);
        // Longitudinal Force [N]
        double longitudinalForce = powertrainTorque / wheelRadius * Car.Transmission.AmountOfDrivenWheels * throttleActuation;

        return longitudinalForce;
    }
    /// <summary>
    /// Gets the longitudinal force for a given brake pedal position.
    /// </summary>
    /// <param name="brakePedalActuation"> Brake pedal position (ratio) </param>
    /// <returns> Car's longitudinal force [N] </returns>
    private double _GetLongitudinalForceFromBrakePedalActuation(double brakePedalActuation)
    {
        // Wheel radius [m]
        double wheelRadius = Car.Tire.TireModel.RO - tireVerticalLoad / Car.Tire.VerticalStiffness;
        // Wheel rotational speed [rad/s]
        double wheelRotationalSpeed = Speed / wheelRadius;
        // Powertrain torque curve interpolation object
        alglib.spline1dbuildlinear(Car.WheelRotationalSpeedCurve.ToArray(), Car.WheelBrakingTorqueCurve.ToArray(), out alglib.spline1dinterpolant powertrainBrakeTorqueInterpolationObject);
        // Powertrain torque [N*m]
        double brakeTorque = alglib.spline1dcalc(powertrainBrakeTorqueInterpolationObject, wheelRotationalSpeed) * Car.Transmission.AmountOfDrivenWheels - Car.Brakes.MaxTorque;
        // Longitudinal Force [N]
        double longitudinalForce = brakeTorque / wheelRadius * brakePedalActuation;

        return longitudinalForce;
    }
    /// <summary>
    /// Optimizes the longitudinal slip to match a given longitudinal force and slip angle.
    /// </summary>
    /// <param name="alpha"></param>
    /// <param name="longitudinalForce"></param>
    /// <returns> Longitudinal slip </returns>
    private double _GetLongitudinalSlipFromLongitudinalForce(double alpha, double longitudinalForce)
    {
        currentAlpha = alpha;
        targetLongitudinalForce = longitudinalForce;

        // Optimization parameters
        double epsg = 1e-10;
        double epsf = 0;
        double epsx = 0;
        double diffstep = 1.0e-6;
        int maxits = 100;
        double[] bndl = new double[] { -.2 };
        double[] bndu = new double[] { .2 };

        double[] kappa = new double[] { 0 };

        alglib.minbleiccreatef(kappa, diffstep, out alglib.minbleicstate state);
        alglib.minbleicsetbc(state, bndl, bndu);
        alglib.minbleicsetcond(state, epsg, epsf, epsx, maxits);
        alglib.minbleicoptimize(state, _LongitudinalSlipForGivenLongitudinalForceOptimization, null, null);
        alglib.minbleicresults(state, out kappa, out alglib.minbleicreport rep);

        return kappa[0];
    }
    /// <summary>
    /// Used to optimize the longitudinal slip for a target longitudinal force.
    /// </summary>
    /// <param name="kappa"> Longitudinal slip </param>
    /// <param name="longitudinalForceDifference"> Difference between the target force and the current calculated force [N] </param>
    /// <param name="obj"></param>
    private void _LongitudinalSlipForGivenLongitudinalForceOptimization(double[] kappa, ref double longitudinalForceDifference, object obj)
    {
        longitudinalForceDifference = Math.Abs(Car.Tire.TireModel.GetTireFx(kappa[0], currentAlpha, tireVerticalLoad, 0, Speed) - targetLongitudinalForce);
    }
    #endregion
    #endregion
}*/
