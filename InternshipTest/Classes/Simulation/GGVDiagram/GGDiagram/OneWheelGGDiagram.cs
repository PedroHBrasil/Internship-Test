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
        private double referenceMy;
        private List<double> errorsFx;
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
            double alphaMin = Car.Tire.TireModel.GetSlipAngleForMinimumTireFy(0, tireFz, 0, Speed);
            // Longitudinal Slip optimization for minimum tire Fx in pureslip
            double kappaBrake = Car.Tire.TireModel.GetLongitudinalSlipForMinimumTireFx(0, tireFz, 0, Speed);
            // Longitudinal Slip optimization for maximum tire Fx in pureslip
            double kappaAccel = Car.Tire.TireModel.GetLongitudinalSlipForMaximumTireFx(0, tireFz, 0, Speed);
            // Generates the vectors of longitudinal slip and slip angle
            //double[] kappas = Generate.LinearSpaced(AmountOfPoints / 4, kappaBrake, kappaAccel);
            //double[] alphas = Generate.LinearSpaced(AmountOfPoints / 8, 0, alphaMin);
            double[] kappasBraking = Generate.LinearSpaced(AmountOfPoints, kappaBrake, 0);
            double[] kappasAccelerating = Generate.LinearSpaced(AmountOfPoints, 0, kappaAccel);
            double[] alphas = Generate.LinearSpaced(AmountOfPoints, 0, alphaMin);
            // Generates the GG diagram
            errorsFx = new List<double>();
            _GetGGDiagramAccelerationsTest(kappasBraking, kappasAccelerating, alphas);
        }
        #region GGDiagramAccelerations
        /// <summary>
        /// Gets the GG diagram's accelerations.
        /// </summary>
        /// <param name="kappasBraking"> Longitudinal slips for the braking case </param>
        /// <param name="kappasAccelerating"> Longitudinal slips for the accelerating case </param>
        /// <param name="alphas"> Slip angles [rad] </param>
        private void _GetGGDiagramAccelerationsTest(double[] kappasBraking, double[] kappasAccelerating, double[] alphas)
        {
            // Merges the longitudinal slip arrays into one.
            double[] kappas = new double[kappasBraking.Length + kappasAccelerating.Length];
            kappasBraking.CopyTo(kappas, 0);
            kappasAccelerating.CopyTo(kappas, kappasBraking.Length);
            // Calculates the wheel radius [mm]
            double wheelRadius = Car.Tire.TireModel.RO - tireFz / Car.Tire.VerticalStiffness;
            // Wheel center angular speed [rad/s]
            double wheelCenterAngularSpeed = Speed / wheelRadius;
            for (int iKappa = 0; iKappa < kappas.Length; iKappa++)
            {
                kappa = kappas[iKappa];
                for (int iAlpha = 0; iAlpha < alphas.Length; iAlpha++)
                {
                    alpha = alphas[iAlpha];
                    // Rolling resistance moment [Nm]
                    double tireMy = Car.Tire.TireModel.GetTireMy(0, alpha, tireFz, 0, Speed);
                    double carLongitudinalForce;
                    double carLateralForce;
                    if (kappa < 0)
                    {
                        // Maximum appliable torque due to tire grip [Nm]
                        double limitTorqueDueToGrip = Car.Tire.TireModel.GetTireFx(kappa, alpha, tireFz, 0, Speed) * wheelRadius * 4;
                        // Wheel braking torque curve interpolation
                        alglib.spline1dbuildlinear(Car.WheelRotationalSpeedCurve.ToArray(), Car.WheelBrakingTorqueCurve.ToArray(), out alglib.spline1dinterpolant wheelBrakingTorqueInterp);
                        double powertrainBrakingTorque = alglib.spline1dcalc(wheelBrakingTorqueInterp, wheelCenterAngularSpeed) + tireMy * Car.Transmission.AmountOfDrivenWheels;
                        // Limit torque due to brakes
                        double limitTorqueDueToBrakes = -Car.Brakes.MaximumTorque + powertrainBrakingTorque;
                        // Is the torque limited by the brakes or by the tire grip?
                        if (limitTorqueDueToGrip < limitTorqueDueToBrakes)
                        {
                            double referenceFx = limitTorqueDueToBrakes / 4 / wheelRadius;
                            // Finds the longitudinal slip for the current Fx
                            double errorFx = 1e10;
                            double oldFx = Car.Tire.TireModel.GetTireFx(kappas[0], alpha, tireFz, 0, Speed);
                            int iOptimizeKappa;
                            for (iOptimizeKappa = 1; iOptimizeKappa < kappas.Length; iOptimizeKappa++)
                            {
                                double currentFx = Car.Tire.TireModel.GetTireFx(kappas[iOptimizeKappa], alpha, tireFz, 0, Speed);
                                double currentErrorFx = Math.Abs(currentFx - oldFx);
                                if (currentErrorFx > errorFx)
                                {
                                    break;
                                }
                                else
                                {
                                    oldFx = currentFx;
                                    errorFx = currentErrorFx;
                                }
                            }
                            kappa = kappas[iOptimizeKappa];
                            errorsFx.Add(Math.Abs(Car.Tire.TireModel.GetTireFx(kappa, alpha, tireFz, 0, Speed) - referenceFx));
                        }
                        // Aerodynamic drag force [N]
                        double dragForce = -interpolatedAerodynamicMapPoint.DragCoefficient * Car.Aerodynamics.FrontalArea * Car.Aerodynamics.AirDensity * Math.Pow(Speed, 2) / 2;
                        // Car's resultant longitudinal force [N]
                        carLongitudinalForce = (Car.Tire.TireModel.GetTireFx(kappa, alpha, tireFz, 0, Speed) + tireMy) * 4 + dragForce;
                        // Car's resultant lateral force [N]
                        carLateralForce = Car.Tire.TireModel.GetTireFy(kappa, alpha, tireFz, 0, Speed) * 4;
                    }
                    else
                    {
                        // Wheel torque curve interpolation
                        alglib.spline1dbuildlinear(Car.WheelRotationalSpeedCurve.ToArray(), Car.WheelTorqueCurve.ToArray(), out alglib.spline1dinterpolant wheelTorqueInterp);
                        double limitTorqueDueToPowertrain = alglib.spline1dcalc(wheelTorqueInterp, wheelCenterAngularSpeed);
                        // Maximum appliable torque due to tire grip [Nm]
                        double limitTorqueDueToGrip = Car.Tire.TireModel.GetTireFx(kappa, alpha, tireFz, 0, Speed) * wheelRadius * Car.Transmission.AmountOfDrivenWheels;
                        // Is the torque limited by the engine or by the tire grip?
                        if (limitTorqueDueToGrip > limitTorqueDueToPowertrain)
                        {
                            double referenceFx = limitTorqueDueToPowertrain / Car.Transmission.AmountOfDrivenWheels / wheelRadius;
                            // Finds the longitudinal slip for the current Fx
                            double errorFx = 1e10;
                            double oldFx = Car.Tire.TireModel.GetTireFx(kappas[0], alpha, tireFz, 0, Speed);
                            int iOptimizeKappa;
                            for (iOptimizeKappa = 1; iOptimizeKappa < kappas.Length; iOptimizeKappa++)
                            {
                                double currentFx = Car.Tire.TireModel.GetTireFx(kappas[iOptimizeKappa], alpha, tireFz, 0, Speed);
                                double currentErrorFx = Math.Abs(currentFx - oldFx);
                                if (currentErrorFx > errorFx)
                                {
                                    break;
                                }
                                else
                                {
                                    oldFx = currentFx;
                                    errorFx = currentErrorFx;
                                }
                            }
                            kappa = kappas[iOptimizeKappa];
                            errorsFx.Add(Math.Abs(Car.Tire.TireModel.GetTireFx(kappa, alpha, tireFz, 0, Speed) - referenceFx));
                        }
                        // Aerodynamic drag force [N]
                        double dragForce = -interpolatedAerodynamicMapPoint.DragCoefficient * Car.Aerodynamics.FrontalArea * Car.Aerodynamics.AirDensity * Math.Pow(Speed, 2) / 2;
                        // Car's resultant longitudinal force [N]
                        carLongitudinalForce = Car.Tire.TireModel.GetTireFx(kappa, alpha, tireFz, 0, Speed) * Car.Transmission.AmountOfDrivenWheels + dragForce +
                            tireMy / wheelRadius * 4;
                        // Car's resultant lateral force [N]
                        if (Car.Transmission.AmountOfDrivenWheels == 2)
                            carLateralForce = (Car.Tire.TireModel.GetTireFy(0, alpha, tireFz, 0, Speed) + Car.Tire.TireModel.GetTireFy(kappa, alpha, tireFz, 0, Speed)) * 2;
                        else
                            carLateralForce = Car.Tire.TireModel.GetTireFy(kappa, alpha, tireFz, 0, Speed) * 4;
                    }
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
            // Current GG Diagram's mean accelerations
            double meanLongitudinalAcceleration = currentLongitudinalAccelerations.Average();
            double meanLateralAcceleration = currentLateralAccelerations.Average();
            // Current accelerations directions array
            double[] currentAccelerationsDirections = new double[LongitudinalAccelerations.Count];
            for (int i = 0; i < LongitudinalAccelerations.Count; i++)
            {
                currentAccelerationsDirections[i] = Math.Atan2(LongitudinalAccelerations[i] - meanLongitudinalAcceleration, LateralAccelerations[i] - meanLateralAcceleration);
            }
            // Target directions array
            double[] targetDirections = Generate.LinearSpaced(AmountOfDirections + 1, -Math.PI, Math.PI);           
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
                    // Checks if the current acceleration's direction is in the current target direction interval
                    if (currentAccelerationsDirections[iCurrentAcceleration] >= targetDirections[iTargetDirection] && currentAccelerationsDirections[iCurrentAcceleration] < targetDirections[iTargetDirection + 1])
                    {
                        indexesOfTheCurrentAccelerationsInRange.Add(iCurrentAcceleration);
                        magnitudesOfTheCurrentAccelerationsInRange.Add(Math.Sqrt(Math.Pow(LongitudinalAccelerations[iCurrentAcceleration] - meanLongitudinalAcceleration, 2) + Math.Pow(LateralAccelerations[iCurrentAcceleration] - meanLateralAcceleration, 2)));
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
            // Gets the indexes of the directions which have accelerations
            List<int> nonZeroAccelerationsIndexes = new List<int>();
            for (int iDirection = 0; iDirection < newLongitudinalAccelerations.Length; iDirection++)
            {
                if (newLongitudinalAccelerations[iDirection] != 0 || newLateralAccelerations[iDirection] != 0)
                {
                    nonZeroAccelerationsIndexes.Add(iDirection);
                }
            }
            // Gets the remaining accelerations by interpolation
            for (int iNonZeroAcceleration = 0; iNonZeroAcceleration < nonZeroAccelerationsIndexes.Count; iNonZeroAcceleration++)
            {
                // Gets the current and next non-zero accelerations indexes
                int currentAccelerationIndex = nonZeroAccelerationsIndexes[iNonZeroAcceleration];
                int nextAccelerationIndex;
                if (iNonZeroAcceleration == nonZeroAccelerationsIndexes.Count - 1) nextAccelerationIndex = nonZeroAccelerationsIndexes[0];
                else nextAccelerationIndex = nonZeroAccelerationsIndexes[iNonZeroAcceleration + 1];
                // Checks if there are zero accelerations between these points. 
                if (nextAccelerationIndex - currentAccelerationIndex > 1)
                {
                    // Gets the accelerations in the points by interpolation
                    for (int iDirection = currentAccelerationIndex + 1; iDirection < nextAccelerationIndex; iDirection++)
                    {
                        double interpolationRatio = (iDirection - currentAccelerationIndex) / (nextAccelerationIndex - currentAccelerationIndex);
                        newLongitudinalAccelerations[iDirection] = newLongitudinalAccelerations[currentAccelerationIndex] + interpolationRatio * (newLongitudinalAccelerations[nextAccelerationIndex] - newLongitudinalAccelerations[currentAccelerationIndex]);
                        newLateralAccelerations[iDirection] = newLateralAccelerations[currentAccelerationIndex] + interpolationRatio * (newLateralAccelerations[nextAccelerationIndex] - newLateralAccelerations[currentAccelerationIndex]);
                    }
                }
                else if (iNonZeroAcceleration == nonZeroAccelerationsIndexes.Count() - 1 && (currentAccelerationIndex != newLongitudinalAccelerations.Length - 1 || nextAccelerationIndex != 0))
                {
                    // Gets the accelerations in the points by interpolation
                    for (int iDirection = currentAccelerationIndex + 1; iDirection < nextAccelerationIndex + newLongitudinalAccelerations.Length; iDirection++)
                    {
                        double interpolationRatio = (iDirection - currentAccelerationIndex) / (nextAccelerationIndex + (newLongitudinalAccelerations.Length - 1) - currentAccelerationIndex);
                        if (iDirection < newLongitudinalAccelerations.Length)
                        {
                            newLongitudinalAccelerations[iDirection] = newLongitudinalAccelerations[currentAccelerationIndex] + interpolationRatio * (newLongitudinalAccelerations[nextAccelerationIndex] - newLongitudinalAccelerations[currentAccelerationIndex]);
                            newLateralAccelerations[iDirection] = newLateralAccelerations[currentAccelerationIndex] + interpolationRatio * (newLateralAccelerations[nextAccelerationIndex] - newLateralAccelerations[currentAccelerationIndex]);
                        }
                        else
                        {
                            newLongitudinalAccelerations[iDirection - newLongitudinalAccelerations.Length] = newLongitudinalAccelerations[currentAccelerationIndex] + interpolationRatio * (newLongitudinalAccelerations[nextAccelerationIndex] - newLongitudinalAccelerations[currentAccelerationIndex]);
                            newLateralAccelerations[iDirection - newLongitudinalAccelerations.Length] = newLateralAccelerations[currentAccelerationIndex] + interpolationRatio * (newLateralAccelerations[nextAccelerationIndex] - newLateralAccelerations[currentAccelerationIndex]);
                        }
                    }
                }
            }
            // Writes the interpolated values to the lists
            LongitudinalAccelerations = newLongitudinalAccelerations.ToList();
            LateralAccelerations = newLateralAccelerations.ToList();
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
                        LateralAccelerations[iAcceleration] <= lateralAcceleration &&
                        LateralAccelerations[iNextAcceleration] >= lateralAcceleration)
                        break;
                    else if (longitudinalAccelerationMode == "Accelerating" &&
                        LateralAccelerations[iAcceleration] >= lateralAcceleration &&
                        LateralAccelerations[iNextAcceleration] <= lateralAcceleration)
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
