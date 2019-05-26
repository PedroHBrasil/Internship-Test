using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Domain.Populations;
using MathNet.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternshipTest.Simulation
{
    public class TwoWheelGGDiagram : GGDiagram
    {
        #region Structs
        /// <summary>
        /// Contains the dynamic parameters of the car.
        /// </summary>
        private struct DynamicStateParameters
        {
            public double steeringWheelAngle;
            public double pedalActuation;
            public double longitudinalAcceleration;
            public double lateralAcceleration;
            public double carSlipAngle;
            public double yawMoment;
            public WheelParameters frontWheelParameters;
            public WheelParameters rearWheelParameters;
        }
        /// <summary>
        /// Contains the dynamic parameters which are specific to each car wheel.
        /// </summary>
        private struct WheelParameters
        {
            public double steeringAngle;
            public double slipAngle;
            public double longitudinalSlip;
        }
        #endregion
        #region Enums
        private enum CorneringDirection { Left, Right}
        #endregion
        #region Fields
        private DynamicStateParameters pureBrakingParameters;
        private DynamicStateParameters pureAcceleratingParameters;
        private DynamicStateParameters pureLeftCorneringParameters;
        private DynamicStateParameters pureRightCorneringParameters;
        private double currentFrontWheelSteeringAngle;
        private double currentRearWheelSteeringAngle;
        private double currentLateralAcceleration;
        private double currentYawMoment;
        private double[] testCarSlipAngle;
        private double[] testYawMoment;
        #endregion
        #region Properties
        /// <summary>
        /// Car and setup for which the GG diagram is generated.
        /// </summary>
        public Vehicle.TwoWheelCar Car { get; set; }
        #endregion
        #region Constructors
        public TwoWheelGGDiagram() { }

        public TwoWheelGGDiagram(Vehicle.TwoWheelCar car)
        {
            Car = car;
            LongitudinalAccelerations = new List<double>();
            LateralAccelerations = new List<double>();
            Curvatures = new List<double>();
        }

        public TwoWheelGGDiagram(double speed, Vehicle.TwoWheelCar car, int amountOfPoints, int amountOfDirections)
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
        public void GenerateGGDiagram()
        {
            // Dynamic state variables initialization
            pureBrakingParameters = new DynamicStateParameters();
            pureAcceleratingParameters = new DynamicStateParameters();
            pureLeftCorneringParameters = new DynamicStateParameters();
            pureRightCorneringParameters = new DynamicStateParameters();
            // Limit longitudinal accelerations due to grip
            pureBrakingParameters.longitudinalAcceleration = _GetBrakingAccelerationDueToTires();
            pureBrakingParameters.lateralAcceleration = 0;
            pureAcceleratingParameters.longitudinalAcceleration = _GetAcceleratingAccelerationDueToTires();
            pureAcceleratingParameters.lateralAcceleration = 0;
            // Limit cornering accelerations
            _GetMinimumCorneringAcceleration();
            pureLeftCorneringParameters.longitudinalAcceleration = _GetLongitudinalAccelerationForPureCornering(pureLeftCorneringParameters.carSlipAngle, pureLeftCorneringParameters.lateralAcceleration, pureLeftCorneringParameters.frontWheelParameters.steeringAngle, pureLeftCorneringParameters.rearWheelParameters.steeringAngle);
            _GetMaximumCorneringAcceleration();
            pureRightCorneringParameters.longitudinalAcceleration = _GetLongitudinalAccelerationForPureCornering(pureRightCorneringParameters.carSlipAngle, pureRightCorneringParameters.lateralAcceleration, pureRightCorneringParameters.frontWheelParameters.steeringAngle, pureRightCorneringParameters.rearWheelParameters.steeringAngle);
            // Combined accelerations
            _GetCombinedOperationAccelerations();
        }

        #region Accelerations by Optimization in Pureslip and Ellipsoid Interpolation
        #region Pure Longitudinal
        /// <summary>
        /// Adjusts the tires torques so that a torque bias is followed.
        /// </summary>
        /// <param name="torqueBias"> Desired torque bias. </param>
        /// <param name="frontGripTorque"> Initial front torque [Nm] </param>
        /// <param name="rearGripTorque"> Initial rear torque [Nm] </param>
        /// <returns> The updated front and rear torques [Nm] </returns>
        private double[] _AdjustWheelsTorquesForGivenTorqueBias(double torqueBias, double frontGripTorque, double rearGripTorque)
        {
            double[] finalTorques = new double[2];
            // Checks if the torque bias is equal to zero or one.
            if (torqueBias == 0)
            {
                finalTorques[0] = 0;
                finalTorques[1] = rearGripTorque;
            }
            else if (torqueBias == 1)
            {
                finalTorques[0] = frontGripTorque;
                finalTorques[1] = 0;
            }
            else
            {
                // Gets the front and rear brake to grip ratio
                double currentTorqueBias = frontGripTorque / (frontGripTorque + rearGripTorque);
                // Checks which ratio is higher
                if (currentTorqueBias > torqueBias)
                {
                    // Adjusts the torques based on the rear grip.
                    finalTorques[0] = rearGripTorque * torqueBias / (1 - torqueBias);
                    finalTorques[1] = rearGripTorque;
                }
                else
                {
                    // Adjusts the torques based on the front grip.
                    finalTorques[0] = frontGripTorque;
                    finalTorques[1] = frontGripTorque * (1 - torqueBias) / torqueBias;
                }
            }

            return finalTorques;
        }
        #region Braking
        /// <summary>
        /// Gets the maximum braking acceleration due to the brakes system.
        /// </summary>
        private double _GetBrakingAccelerationDueToBrakes()
        {
            // Longitudinal Acceleration vs. Longitudinal Load Transfer Optimization loop
            double longitudinalAcceleration = 0;
            double oldLongitudinalAcceleration;
            double tol = 1e-3;
            double error;
            int maxIter = 100;
            int iter = 0;
            do
            {
                // Iteration limit
                iter++;
                // Wheels loads [N]
                double[] wheelsLoads = Car.GetWheelsLoads(Speed, 0, longitudinalAcceleration);
                // Wheels radiuses [m]
                double[] wheelsRadiuses = Car.GetWheelsRadiuses(wheelsLoads);
                // Wheels angular speeds [rad/s]
                double[] wheelsAngularSpeeds = Car.GetWheelsAngularSpeeds(wheelsRadiuses, Speed);
                double referenceWheelAngularSpeed = wheelsAngularSpeeds[0] * Car.Transmission.TorqueBias / (wheelsAngularSpeeds[0] * Car.Transmission.TorqueBias + wheelsAngularSpeeds[1] * (1 - Car.Transmission.TorqueBias));
                // Engine braking torque curve determinaton by interpolation
                alglib.spline1dbuildlinear(Car.WheelRotationalSpeedCurve.ToArray(), Car.WheelBrakingTorqueCurve.ToArray(), out alglib.spline1dinterpolant wheelBrakingTorqueInterp);
                double powertrainBrakingTorque = alglib.spline1dcalc(wheelBrakingTorqueInterp, referenceWheelAngularSpeed);
                // Brakes torques from pedals actuation [Nm]
                double frontBrakeTorque = Car.Brakes.ActualMaximumFrontTorque + powertrainBrakingTorque * Car.Transmission.TorqueBias;
                double rearBrakeTorque = Car.Brakes.ActualMaximumRearTorque + powertrainBrakingTorque * (1 - Car.Transmission.TorqueBias);
                double[] finalBrakeTorques = new double[2] { frontBrakeTorque, rearBrakeTorque };
                // Determines the longitudinal forces [N]
                double frontLongitudinalForce = finalBrakeTorques[0] / wheelsRadiuses[0];
                double rearLongitudinalForce = finalBrakeTorques[1] / wheelsRadiuses[1];
                // Gets the inertia efficiency [ratio]
                double meanWheelRadius = wheelsRadiuses.Average();
                double inertiaEfficiency = Car.GetInertiaEfficiency(meanWheelRadius);
                // Aerodynamic drag [N]
                Vehicle.TwoWheelAerodynamicMapPoint currentAerodynamicParameters = Car.GetAerodynamicCoefficients(Speed, 0, longitudinalAcceleration);
                double dragForce = -currentAerodynamicParameters.DragCoefficient * Car.Aerodynamics.FrontalArea * Car.Aerodynamics.AirDensity * Math.Pow(Speed, 2) / 2;
                // Longitudinal acceleration
                oldLongitudinalAcceleration = longitudinalAcceleration;
                longitudinalAcceleration = (frontLongitudinalForce + rearLongitudinalForce + dragForce) / Car.InertiaAndDimensions.TotalMass * inertiaEfficiency;
                // Error update
                error = Math.Abs(longitudinalAcceleration - oldLongitudinalAcceleration);
            } while (error > tol && iter < maxIter);
            return longitudinalAcceleration;
        }
        /// <summary>
        ///  Gets the maximum braking acceleration due to the tires grips.
        /// </summary>
        /// <returns> Braking maximum acceleration due to grip limitation [m/s²] </returns>
        private double _GetBrakingAccelerationDueToTires()
        {
            // Longitudinal Acceleration vs. Longitudinal Load Transfer Optimization loop
            double longitudinalAcceleration = 0;
            double oldLongitudinalAcceleration;
            double tol = 1e-3;
            double error;
            int maxIter = 100;
            int iter = 0;
            do
            {
                // Iteration limit
                iter++;
                // Wheels loads [N]
                double[] wheelsLoads = Car.GetWheelsLoads(Speed, 0, longitudinalAcceleration);
                // Wheels radiuses [m]
                double[] wheelsRadiuses = Car.GetWheelsRadiuses(wheelsLoads);
                // Longitudinal slips for maximum brake torque due to grip
                double frontLongitudinalSlipForMaximumBrake = Car.FrontTire.TireModel.GetLongitudinalSlipForMinimumTireFx(0, wheelsLoads[0], 0, Speed);
                double rearLongitudinalSlipForMaximumBrake = Car.RearTire.TireModel.GetLongitudinalSlipForMinimumTireFx(0, wheelsLoads[1], 0, Speed);
                // Maximum braking wheels torques according to tire grip [Nm]
                double frontGripTorque = Car.FrontTire.TireModel.GetTireFx(frontLongitudinalSlipForMaximumBrake, 0, wheelsLoads[0], 0, Speed) * wheelsRadiuses[0] * 2;
                double rearGripTorque = Car.RearTire.TireModel.GetTireFx(rearLongitudinalSlipForMaximumBrake, 0, wheelsLoads[1], 0, Speed) * wheelsRadiuses[1] * 2;
                // Adjust the braking torques accordingly to the tires grips
                double[] finalBrakeTorques = _AdjustWheelsTorquesForGivenTorqueBias(Car.Brakes.BrakeBias, frontGripTorque, rearGripTorque);
                // Determines the longitudinal forces [N]
                double frontLongitudinalForce = finalBrakeTorques[0] / wheelsRadiuses[0];
                double rearLongitudinalForce = finalBrakeTorques[1] / wheelsRadiuses[1];
                // Gets the inertia efficiency [ratio]
                double meanWheelRadius = wheelsRadiuses.Average();
                double inertiaEfficiency = Car.GetInertiaEfficiency(meanWheelRadius);
                // Longitudinal acceleration
                oldLongitudinalAcceleration = longitudinalAcceleration;
                longitudinalAcceleration = (frontLongitudinalForce + rearLongitudinalForce) / Car.InertiaAndDimensions.TotalMass * inertiaEfficiency;
                // Error update
                error = Math.Abs(longitudinalAcceleration - oldLongitudinalAcceleration);
            } while (error > tol && iter < maxIter);
            return longitudinalAcceleration;
        }
        #endregion
        #region Accelerating
        /// <summary>
        /// Gets the maximum accelerating acceleration due to the powertrain system.
        /// </summary>
        /// <returns> Accelerating maximum acceleration due to brakes limitation [m/s²] </returns>
        private double _GetAcceleratingAccelerationDueToPowertrain()
        {
            // Longitudinal Acceleration vs. Longitudinal Load Transfer Optimization loop
            double longitudinalAcceleration = 0;
            double oldLongitudinalAcceleration;
            double tol = 1e-3;
            double error;
            int maxIter = 100;
            int iter = 0;
            do
            {
                // Iteration limit
                iter++;
                // Wheels loads [N]
                double[] wheelsLoads = Car.GetWheelsLoads(Speed, 0, longitudinalAcceleration);
                // Wheels radiuses [m]
                double[] wheelsRadiuses = Car.GetWheelsRadiuses(wheelsLoads);
                // Wheels angular speeds [rad/s]
                double[] wheelsAngularSpeeds = Car.GetWheelsAngularSpeeds(wheelsRadiuses, Speed);
                double referenceWheelAngularSpeed = wheelsAngularSpeeds[0] * Car.Transmission.TorqueBias / (wheelsAngularSpeeds[0] * Car.Transmission.TorqueBias + wheelsAngularSpeeds[1] * (1 - Car.Transmission.TorqueBias));
                // Engine braking torque curve determinaton by interpolation
                alglib.spline1dbuildlinear(Car.WheelRotationalSpeedCurve.ToArray(), Car.WheelTorqueCurve.ToArray(), out alglib.spline1dinterpolant wheelTorqueInterp);
                double powertrainTorque = alglib.spline1dcalc(wheelTorqueInterp, referenceWheelAngularSpeed);
                // Brakes torques from pedals actuation [Nm]
                double frontPowertrainTorque = powertrainTorque * Car.Transmission.TorqueBias;
                double rearPowertrainTorque = powertrainTorque * (1 - Car.Transmission.TorqueBias);
                double[] finalPowertrainTorques = new double[2] { frontPowertrainTorque, rearPowertrainTorque };
                // Determines the longitudinal forces [N]
                double frontLongitudinalForce = finalPowertrainTorques[0] / wheelsRadiuses[0];
                double rearLongitudinalForce = finalPowertrainTorques[1] / wheelsRadiuses[1];
                // Gets the inertia efficiency [ratio]
                double meanWheelRadius = wheelsRadiuses.Average();
                double inertiaEfficiency = Car.GetInertiaEfficiency(meanWheelRadius);
                // Aerodynamic drag [N]
                Vehicle.TwoWheelAerodynamicMapPoint currentAerodynamicParameters = Car.GetAerodynamicCoefficients(Speed, 0, longitudinalAcceleration);
                double dragForce = -currentAerodynamicParameters.DragCoefficient * Car.Aerodynamics.FrontalArea * Car.Aerodynamics.AirDensity * Math.Pow(Speed, 2) / 2;
                // Longitudinal acceleration
                oldLongitudinalAcceleration = longitudinalAcceleration;
                longitudinalAcceleration = (frontLongitudinalForce + rearLongitudinalForce + dragForce) / Car.InertiaAndDimensions.TotalMass * inertiaEfficiency;
                // Error update
                error = Math.Abs(longitudinalAcceleration - oldLongitudinalAcceleration);
            } while (error > tol && iter < maxIter);
            return longitudinalAcceleration;
        }
        /// <summary>
        ///  Gets the maximum accelerating acceleration due to the tires grips.
        /// </summary>
        /// <returns> Accelerating maximum acceleration due to grip limitation [m/s²] </returns>
        private double _GetAcceleratingAccelerationDueToTires()
        {
            // Longitudinal Acceleration vs. Longitudinal Load Transfer Optimization loop
            double longitudinalAcceleration = 0;
            double oldLongitudinalAcceleration;
            double tol = 1e-3;
            double error;
            int maxIter = 100;
            int iter = 0;
            do
            {
                // Iteration limit
                iter++;
                // Wheels loads [N]
                double[] wheelsLoads = Car.GetWheelsLoads(Speed, 0, longitudinalAcceleration);
                // Wheels radiuses [m]
                double[] wheelsRadiuses = Car.GetWheelsRadiuses(wheelsLoads);
                // Longitudinal slips for maximum brake torque due to grip
                double frontLongitudinalSlipForMaximumAccelerating = Car.FrontTire.TireModel.GetLongitudinalSlipForMaximumTireFx(0, wheelsLoads[0], 0, Speed);
                double rearLongitudinalSlipForMaximumAccelerating = Car.RearTire.TireModel.GetLongitudinalSlipForMaximumTireFx(0, wheelsLoads[1], 0, Speed);
                // Maximum braking wheels torques according to tire grip [Nm]
                double frontGripTorque = Car.FrontTire.TireModel.GetTireFx(frontLongitudinalSlipForMaximumAccelerating, 0, wheelsLoads[0], 0, Speed) * wheelsRadiuses[0] * 2;
                double rearGripTorque = Car.RearTire.TireModel.GetTireFx(rearLongitudinalSlipForMaximumAccelerating, 0, wheelsLoads[1], 0, Speed) * wheelsRadiuses[1] * 2;
                // Adjust the braking torques accordingly to the tires grips
                double[] finalPowertrainTorques = _AdjustWheelsTorquesForGivenTorqueBias(Car.Transmission.TorqueBias, frontGripTorque, rearGripTorque);
                // Determines the longitudinal forces [N]
                double frontLongitudinalForce = finalPowertrainTorques[0] / wheelsRadiuses[0];
                double rearLongitudinalForce = finalPowertrainTorques[1] / wheelsRadiuses[1];
                // Gets the inertia efficiency [ratio]
                double meanWheelRadius = wheelsRadiuses.Average();
                double inertiaEfficiency = Car.GetInertiaEfficiency(meanWheelRadius);
                // Longitudinal acceleration
                oldLongitudinalAcceleration = longitudinalAcceleration;
                longitudinalAcceleration = (frontLongitudinalForce + rearLongitudinalForce) / Car.InertiaAndDimensions.TotalMass * inertiaEfficiency;
                // Error update
                error = Math.Abs(longitudinalAcceleration - oldLongitudinalAcceleration);
            } while (error > tol && iter < maxIter);
            return longitudinalAcceleration;
        }
        #endregion
        #endregion
        #region Pure Cornering
        #region Left (Negative)
        /// <summary>
        /// Gets the car's minimum cornering acceleration by optimization.
        /// </summary>
        /// <returns> Minimum cornering lateral acceleration [m/s²] </returns>
        private void _GetMinimumCorneringAcceleration()
        {
            // Optimization parameters
            double epsg = 1e-6;
            double epsf = 0;
            double epsx = 0;
            double diffstep = 1.0e-3;
            int maxits = 100;

            double[] bndl = new double[] { -Car.Steering.MaximumSteeringWheelAngle };
            double[] bndu = new double[] { 0 };

            double[] delta = new double[] { 0 };

            alglib.minbleiccreatef(delta, diffstep, out alglib.minbleicstate state);
            alglib.minbleicsetbc(state, bndl, bndu);
            alglib.minbleicsetcond(state, epsg, epsf, epsx, maxits);
            alglib.minbleicoptimize(state, _SteeringWheelAngleOptimizationForMinimumCornering, null, null);
            alglib.minbleicresults(state, out delta, out alglib.minbleicreport rep);

            pureLeftCorneringParameters.steeringWheelAngle = delta[0];
            pureLeftCorneringParameters.frontWheelParameters.steeringAngle = currentFrontWheelSteeringAngle;
            pureLeftCorneringParameters.rearWheelParameters.steeringAngle = currentRearWheelSteeringAngle;
        }
        /// <summary>
        /// Method for optimization of the steering wheel angle for minimum lateral acceleration.
        /// </summary>
        /// <param name="delta"> Steering wheel angle [rad] </param>
        /// <param name="lateralAcceleration"> Lateral acceleration [m/s²] </param>
        /// <param name="obj"></param>
        private void _SteeringWheelAngleOptimizationForMinimumCornering(double[] delta, ref double lateralAcceleration, object obj)
        {
            // Wheels steering angles
            currentFrontWheelSteeringAngle = delta[0] * Car.Steering.FrontSteeringRatio;
            currentRearWheelSteeringAngle = delta[0] * Car.Steering.RearSteeringRatio;
            // Current lateral acceleration optimization.
            double[] lateralAccelerationForOptimization = new double[] { 0 };
            double epsg = 1e-6;
            double epsf = 0;
            double epsx = 0;
            double diffstep = 1.0e-2;
            int maxits = 100;

            alglib.minlbfgscreatef(1, lateralAccelerationForOptimization, diffstep, out alglib.minlbfgsstate state);
            alglib.minlbfgssetcond(state, epsg, epsf, epsx, maxits);
            alglib.minlbfgsoptimize(state, _LateralAccelerationForGivenSteeringAngleOptimization, null, null);
            alglib.minlbfgsresults(state, out lateralAccelerationForOptimization, out alglib.minlbfgsreport rep);

            pureLeftCorneringParameters.lateralAcceleration = lateralAccelerationForOptimization[0];
            lateralAcceleration = lateralAccelerationForOptimization[0];
        }
        #endregion
        #region Right (Positive)
        /// <summary>
        /// Gets the car's maximum cornering acceleration by optimization.
        /// </summary>
        /// <returns> Maximum cornering lateral acceleration [m/s²] </returns>
        private void _GetMaximumCorneringAcceleration()
        {
            // Optimization parameters
            double epsg = 1e-6;
            double epsf = 0;
            double epsx = 0;
            double diffstep = 1.0e-3;
            int maxits = 100;

            double[] bndl = new double[] { 0 };
            double[] bndu = new double[] { Car.Steering.MaximumSteeringWheelAngle };

            double[] delta = new double[] { 0 };

            alglib.minbleiccreatef(delta, diffstep, out alglib.minbleicstate state);
            alglib.minbleicsetbc(state, bndl, bndu);
            alglib.minbleicsetcond(state, epsg, epsf, epsx, maxits);
            alglib.minbleicoptimize(state, _SteeringWheelAngleOptimizationForMaximumCornering, null, null);
            alglib.minbleicresults(state, out delta, out alglib.minbleicreport rep);

            pureRightCorneringParameters.steeringWheelAngle = delta[0];
            pureRightCorneringParameters.frontWheelParameters.steeringAngle = currentFrontWheelSteeringAngle;
            pureRightCorneringParameters.rearWheelParameters.steeringAngle = currentRearWheelSteeringAngle;
        }
        /// <summary>
        /// Method for optimization of the steering wheel angle for maximum lateral acceleration.
        /// </summary>
        /// <param name="delta"> Steering wheel angle [rad] </param>
        /// <param name="lateralAcceleration"> Lateral acceleration [m/s²] </param>
        /// <param name="obj"></param>
        private void _SteeringWheelAngleOptimizationForMaximumCornering(double[] delta, ref double lateralAcceleration, object obj)
        {
            // Wheels steering angles
            currentFrontWheelSteeringAngle = delta[0] * Car.Steering.FrontSteeringRatio;
            currentRearWheelSteeringAngle = delta[0] * Car.Steering.RearSteeringRatio;
            // Current lateral acceleration optimization.
            double[] lateralAccelerationForOptimization = new double[] { 0 };
            double epsg = 1e-6;
            double epsf = 0;
            double epsx = 0;
            double diffstep = 1.0e-2;
            int maxits = 100;

            alglib.minlbfgscreatef(1, lateralAccelerationForOptimization, diffstep, out alglib.minlbfgsstate state);
            alglib.minlbfgssetcond(state, epsg, epsf, epsx, maxits);
            alglib.minlbfgsoptimize(state, _LateralAccelerationForGivenSteeringAngleOptimization, null, null);
            alglib.minlbfgsresults(state, out lateralAccelerationForOptimization, out alglib.minlbfgsreport rep);

            pureRightCorneringParameters.lateralAcceleration = lateralAccelerationForOptimization[0];

            lateralAcceleration = -lateralAccelerationForOptimization[0];
        }
        #endregion
        /// <summary>
        /// Method to find the lateral acceleration associated with the given steering wheel angle.
        /// </summary>
        /// <param name="lateralAccelerationGuess"> Optimization parameter: lateral acceleration guess [m/s²]. </param>
        /// <param name="lateralAccelerationError"> Optimization target: difference between the guess and the calculated lateral acceleration [m/s²]. </param>
        /// <param name="obj"></param>
        private void _LateralAccelerationForGivenSteeringAngleOptimization(double[] lateralAccelerationGuess, ref double lateralAccelerationError, object obj)
        {
            currentLateralAcceleration = lateralAccelerationGuess[0];
            // Car slip angle vs. yaw moment optimization
            pureRightCorneringParameters.carSlipAngle = _GetCarSlipAngleForZeroYawMoment(-Math.PI / 3, Math.PI / 3, 20);
            // Lateral acceleration update [m/s²]
            double lateralAcceleration = _GetLateralAcceleration(pureRightCorneringParameters.carSlipAngle, currentLateralAcceleration, currentFrontWheelSteeringAngle, currentRearWheelSteeringAngle);
            // Error evaluation [m/s²]
            lateralAccelerationError = Math.Abs(currentLateralAcceleration - lateralAcceleration);
        }
        /// <summary>
        /// Gets the car slip angle associated with zero yaw moment, if there is one.
        /// </summary>
        /// <param name="lowerBoundary"> Lower boundary of the search interval. </param>
        /// <param name="upperBoundary"> Upper boundary of the search interval. </param>
        /// <param name="amountOfSearchIntervals"> Amount of inner intervals to generate (refines the search). </param>
        /// <returns> Car slip angle [rad] </returns>
        private double _GetCarSlipAngleForZeroYawMoment(double lowerBoundary, double upperBoundary, int amountOfSearchIntervals)
        {
            // Inner boundaries array generation
            double[] innerBoundaries = Generate.LinearSpaced(amountOfSearchIntervals, lowerBoundary, upperBoundary);
            // Reference yaw moment [Nm] (Lower boundary)
            double referenceYawMoment = _GetYawMomentForCarSlipAngle(innerBoundaries[0], currentLateralAcceleration, currentFrontWheelSteeringAngle, currentRearWheelSteeringAngle);
            // Inner boundary determination loop
            bool isSignEqual = true;
            int iBoundary = 0;
            do
            {
                iBoundary++;
                // Gets the yaw moment for the current boundary index
                double currentYawMoment = _GetYawMomentForCarSlipAngle(innerBoundaries[iBoundary], currentLateralAcceleration, currentFrontWheelSteeringAngle, currentRearWheelSteeringAngle);
                // Checks if the yaw moment has changed its sign and updates the boolean "isSignEqual" if it is
                if (Math.Sign(referenceYawMoment) != Math.Sign(currentYawMoment))
                {
                    isSignEqual = false;
                }
            } while (isSignEqual && iBoundary < amountOfSearchIntervals - 2);
            // Checks if an inner interval was found and runs the optimization if it was.
            if (!isSignEqual)
            {
                // Optimization parameters
                double epsg = 1e-10;
                double epsf = 0;
                double epsx = 0;
                double diffstep = 1.0e-6;
                int maxits = 100;
                double[] minCarSlipAngle = new double[] { innerBoundaries[iBoundary - 1] };
                double[] maxCarSlipAngle = new double[] { innerBoundaries[iBoundary] };

                double[] carSlipAngle = new double[] { pureLeftCorneringParameters.carSlipAngle };
                alglib.minbleiccreatef(carSlipAngle, diffstep, out alglib.minbleicstate state);
                alglib.minbleicsetbc(state, minCarSlipAngle, maxCarSlipAngle);
                alglib.minbleicsetcond(state, epsg, epsf, epsx, maxits);
                alglib.minbleicoptimize(state, _CarSlipAngleOptimization, null, null);
                alglib.minbleicresults(state, out carSlipAngle, out alglib.minbleicreport rep);

                return carSlipAngle[0];
            }
            else
            {
                return 0;
            }
        }
        /// <summary>
        /// Gets the car's yaw moment.
        /// </summary>
        /// <param name="carSlipAngle"> Car's slip angle [rad]. </param>
        /// <param name="lateralAcceleration"> Car's lateral acceleration [m/s²] </param>
        /// <param name="frontWheelSteeringAngle"> Front wheel steering angle [rad] </param>
        /// <param name="rearWheelSteeringAngle"> Rear wheel steering angle [rad] </param>
        /// <returns> Yaw moment [Nm] </returns>
        private double _GetYawMomentForCarSlipAngle(double carSlipAngle, double lateralAcceleration, double frontWheelSteeringAngle, double rearWheelSteeringAngle)
        {
            // Wheels loads [N]
            double[] wheelsLoads = Car.GetWheelsLoads(Speed, carSlipAngle, 0);
            // Wheels slip angles [rad]
            double[] wheelsSlipAngles = _GetWheelsSlipAngles(carSlipAngle, lateralAcceleration, frontWheelSteeringAngle, rearWheelSteeringAngle);
            // Front and rear lateral forces [N]
            double frontWheelLateralForce = 2 * Car.FrontTire.TireModel.GetTireFy(0, wheelsSlipAngles[0], wheelsLoads[0], 0, Speed) * Math.Cos(currentFrontWheelSteeringAngle);
            double rearWheelLateralForce = 2 * Car.RearTire.TireModel.GetTireFy(0, wheelsSlipAngles[1], wheelsLoads[1], 0, Speed) * Math.Cos(currentRearWheelSteeringAngle);
            // Aerodynamic yaw moment [Nm]
            Vehicle.TwoWheelAerodynamicMapPoint currentAerodynamicParameters = Car.GetAerodynamicCoefficients(Speed, carSlipAngle, 0);
            double aerodynamicYawMoment = -currentAerodynamicParameters.YawMomentCoefficient * Car.Aerodynamics.FrontalArea * Car.Aerodynamics.AirDensity * Math.Pow(Speed, 2) / 2;
            // Yaw moment (squared) [Nm]
            currentYawMoment = frontWheelLateralForce * Car.InertiaAndDimensions.DistanceBetweenFrontAxisAndCG - rearWheelLateralForce * Car.InertiaAndDimensions.DistanceBetweenRearAxisAndCG + aerodynamicYawMoment;
            return currentYawMoment;
        }
        private void _UpdateCarSlipAngleVsYawMomentArrays()
        {
            int amountOfPoints = 500;
            testCarSlipAngle = Generate.LinearSpaced(amountOfPoints, -Math.PI, Math.PI);
            testYawMoment = new double[amountOfPoints];
            for (int i = 0; i < amountOfPoints; i++)
            {
                // Input separation
                double carSlipAngle = testCarSlipAngle[i];
                // Wheels loads [N]
                double[] wheelsLoads = Car.GetWheelsLoads(Speed, carSlipAngle, 0);
                // Wheels slip angles [rad]
                double[] wheelsSlipAngles = _GetWheelsSlipAngles(carSlipAngle, currentLateralAcceleration, currentFrontWheelSteeringAngle, currentRearWheelSteeringAngle);
                // Front and rear lateral forces [N]
                double frontWheelLateralForce = 2 * Car.FrontTire.TireModel.GetTireFy(0, wheelsSlipAngles[0], wheelsLoads[0], 0, Speed) * Math.Cos(currentFrontWheelSteeringAngle);
                double rearWheelLateralForce = 2 * Car.RearTire.TireModel.GetTireFy(0, wheelsSlipAngles[1], wheelsLoads[1], 0, Speed) * Math.Cos(currentRearWheelSteeringAngle);
                // Aerodynamic yaw moment [Nm]
                Vehicle.TwoWheelAerodynamicMapPoint currentAerodynamicParameters = Car.GetAerodynamicCoefficients(Speed, carSlipAngle, 0);
                double aerodynamicYawMoment = -currentAerodynamicParameters.YawMomentCoefficient * Car.Aerodynamics.FrontalArea * Car.Aerodynamics.AirDensity * Math.Pow(Speed, 2) / 2;
                // Yaw moment (squared) [Nm]
                testYawMoment[i] = frontWheelLateralForce * Car.InertiaAndDimensions.DistanceBetweenFrontAxisAndCG - rearWheelLateralForce * Car.InertiaAndDimensions.DistanceBetweenRearAxisAndCG + aerodynamicYawMoment;
            }
        }
        /// <summary>
        /// Finds the car slip angle and the lateral acceleration associated with the current steering wheel angle for steady-state operation.
        /// </summary>
        /// <param name="carSlipAngleOptimization"> Optimization parameters: Car slip angle. </param>
        /// <param name="yawMomentSquared"> Optimization reference: Yaw moment (squared) </param>
        /// <param name="obj"></param>
        private void _CarSlipAngleOptimization(double[] carSlipAngleOptimization, ref double yawMomentSquared, object obj)
        {
            currentYawMoment = _GetYawMomentForCarSlipAngle(carSlipAngleOptimization[0], currentLateralAcceleration, currentFrontWheelSteeringAngle, currentRearWheelSteeringAngle);
            yawMomentSquared = Math.Pow(currentYawMoment, 2);
        }
        /// <summary>
        /// Gets the car's lateral acceleration based on a guess of lateral acceleration.
        /// </summary>
        /// <param name="carSlipAngle"> Car's slip angle [rad]. </param>
        /// <param name="lateralAccelerationGuess"> Car's lateral acceleration guess [m/s²] </param>
        /// <param name="frontWheelSteeringAngle"> Front wheel steering angle [rad] </param>
        /// <param name="rearWheelSteeringAngle"> Rear wheel steering angle [rad] </param>
        /// <returns> Car's lateral acceleration [m/s²] </returns>
        private double _GetLateralAcceleration(double carSlipAngle, double lateralAccelerationGuess, double frontWheelSteeringAngle, double rearWheelSteeringAngle)
        {
            // Wheels slip angles [rad]
            double[] wheelsSlipAngles = _GetWheelsSlipAngles(carSlipAngle, lateralAccelerationGuess, frontWheelSteeringAngle, rearWheelSteeringAngle);
            // Wheels loads [N]
            double[] wheelsLoads = Car.GetWheelsLoads(Speed, carSlipAngle, 0);
            // Wheels longitudinal forces [N]
            double frontWheelLateralForce = 2 * Car.FrontTire.TireModel.GetTireFy(0, wheelsSlipAngles[0], wheelsLoads[0], 0, Speed) * Math.Cos(frontWheelSteeringAngle);
            double rearWheelLateralForce = 2 * Car.RearTire.TireModel.GetTireFy(0, wheelsSlipAngles[1], wheelsLoads[1], 0, Speed) * Math.Cos(rearWheelSteeringAngle);
            // Aerodynamic side force [N]
            Vehicle.TwoWheelAerodynamicMapPoint currentAerodynamicParameters = Car.GetAerodynamicCoefficients(Speed, carSlipAngle, 0);
            double sideForce = -currentAerodynamicParameters.SideForceCoefficient * Car.Aerodynamics.FrontalArea * Car.Aerodynamics.AirDensity * Math.Pow(Speed, 2) / 2;
            // Lateral acceleration [m/s²]
            double lateralAcceleration = (frontWheelLateralForce + rearWheelLateralForce + sideForce) / Car.InertiaAndDimensions.TotalMass;

            return lateralAcceleration;
        }
        /// <summary>
        /// Gets the wheels slip angles.
        /// </summary>
        /// <param name="carSlipAngle"> Car's slip angle [rad]. </param>
        /// <param name="lateralAcceleration"> Car's lateral acceleration [m/s²] </param>
        /// <param name="frontWheelSteeringAngle"> Front wheel steering angle [rad] </param>
        /// <param name="rearWheelSteeringAngle"> Rear wheel steering angle [rad] </param>
        /// <returns> Wheels slip angles (0: front - 1: rear) </returns>
        private double[] _GetWheelsSlipAngles(double carSlipAngle, double lateralAcceleration, double frontWheelSteeringAngle, double rearWheelSteeringAngle)
        {
            // Yaw rate [rad/s]
            double yawRate = lateralAcceleration / Speed;
            // Wheels longitudinal speeds [m/s]
            double frontWheelLongitudinalSpeed = (Speed * Math.Sin(-carSlipAngle) + Car.InertiaAndDimensions.DistanceBetweenFrontAxisAndCG * yawRate) * Math.Sin(frontWheelSteeringAngle) + Speed * Math.Cos(-carSlipAngle) * Math.Cos(frontWheelSteeringAngle);
            double rearWheelLongitudinalSpeed = (Speed * Math.Sin(-carSlipAngle) - Car.InertiaAndDimensions.DistanceBetweenFrontAxisAndCG * yawRate) * Math.Sin(rearWheelSteeringAngle) + Speed * Math.Cos(-carSlipAngle) * Math.Cos(rearWheelSteeringAngle);
            // Wheels lateral speeds [m/s]
            double frontWheelLateralSpeed = (Speed * Math.Sin(-carSlipAngle) + Car.InertiaAndDimensions.DistanceBetweenFrontAxisAndCG * yawRate) * Math.Cos(frontWheelSteeringAngle) - Speed * Math.Cos(-carSlipAngle) * Math.Sin(frontWheelSteeringAngle);
            double rearWheelLateralSpeed = (Speed * Math.Sin(-carSlipAngle) - Car.InertiaAndDimensions.DistanceBetweenRearAxisAndCG * yawRate) * Math.Cos(rearWheelSteeringAngle) - Speed * Math.Cos(-carSlipAngle) * Math.Sin(rearWheelSteeringAngle);
            // Slip Angles [rad]
            double frontSlipAngle = Math.Atan(frontWheelLateralSpeed / frontWheelLongitudinalSpeed);
            double rearSlipAngle = Math.Atan(rearWheelLateralSpeed / rearWheelLongitudinalSpeed);
            return new double[] { frontSlipAngle, rearSlipAngle };
        }
        /// <summary>
        /// Gets the car's longitudinal acceleration in pure cornering situation (zero longitudinal slip).
        /// </summary>
        /// <param name="carSlipAngle"> Car's slip angle [rad]. </param>
        /// <param name="lateralAcceleration"> Car's lateral acceleration [m/s²] </param>
        /// <param name="frontWheelSteeringAngle"> Front wheel steering angle [rad] </param>
        /// <param name="rearWheelSteeringAngle"> Rear wheel steering angle [rad] </param>
        /// <returns> Car's longitudinal acceleration [m/s²] </returns>
        private double _GetLongitudinalAccelerationForPureCornering(double carSlipAngle, double lateralAcceleration, double frontWheelSteeringAngle, double rearWheelSteeringAngle)
        {
            // Wheels loads [N]
            double[] wheelsLoads = Car.GetWheelsLoads(Speed, carSlipAngle, 0);
            // Wheels slip angles [rad]
            double[] wheelsSlipAngles = _GetWheelsSlipAngles(carSlipAngle, lateralAcceleration, frontWheelSteeringAngle, rearWheelSteeringAngle);
            // Wheels longitudinal forces [N]
            double frontWheelLongitudinalForce = 2 * Car.FrontTire.TireModel.GetTireFx(0, wheelsSlipAngles[0], wheelsLoads[0], 0, Speed) * Math.Cos(frontWheelSteeringAngle);
            double rearWheelLongitudinalForce = 2 * Car.RearTire.TireModel.GetTireFx(0, wheelsSlipAngles[1], wheelsLoads[1], 0, Speed) * Math.Cos(rearWheelSteeringAngle);
            // Wheels radiuses [m]
            double[] wheelsRadiuses = Car.GetWheelsRadiuses(wheelsLoads);
            // Gets the inertia efficiency [ratio]
            double meanWheelRadius = wheelsRadiuses.Average();
            double inertiaEfficiency = Car.GetInertiaEfficiency(meanWheelRadius);
            // Aerodynamic drag force [N]
            Vehicle.TwoWheelAerodynamicMapPoint currentAerodynamicParameters = Car.GetAerodynamicCoefficients(Speed, carSlipAngle, 0);
            double dragForce = -currentAerodynamicParameters.DragCoefficient * Car.Aerodynamics.FrontalArea * Car.Aerodynamics.AirDensity * Math.Pow(Speed, 2) / 2;
            // Vehicle accelerations [m/s²]
            double longitudinalAcceleration = (frontWheelLongitudinalForce + rearWheelLongitudinalForce + dragForce) * inertiaEfficiency / Car.InertiaAndDimensions.TotalMass;

            return longitudinalAcceleration;
        }
        #endregion
        #region Ellipsoid Interpolation
        /// <summary>
        /// Gets the combined operation accelerations by "ellipsoid interpolation".
        /// </summary>
        private void _GetCombinedOperationAccelerations()
        {
            // Amount of directions
            int amountOfDirections = AmountOfPoints * 4;
            // Directions array [rad]
            double[] directions = Generate.LinearSpaced(amountOfDirections, -Math.PI, Math.PI * (amountOfDirections - 2) / amountOfDirections);
            // Gets the accelerations for each direction
            _GetAccelerationsForEachDirection(directions);
            // Gets the longitudinal acceleration limits given the powertrain and brakes capabilities.
            double minimumAcceleration = _GetBrakingAccelerationDueToBrakes();
            double maximumAcceleration = _GetAcceleratingAccelerationDueToPowertrain();
            // Adjusts the accelerations based on the limits
            _AdjustLongitudinalAccelerations(minimumAcceleration, maximumAcceleration);
        }
        /// <summary>
        /// Gets the combined operation accelerations for the given directions.
        /// </summary>
        /// <param name="directions"> Accelerations directions [rad] </param>
        private void _GetAccelerationsForEachDirection(double[] directions)
        {
            for (int iDirection = 0; iDirection < directions.Length; iDirection++)
            {
                // Initializes the current accelerations array (long. and lat.) and determines them based on the direction.
                double[] currentAccelerations = new double[2];
                if (directions[iDirection] >= -Math.PI && directions[iDirection] < -Math.PI / 2)
                {
                    currentAccelerations[0] = pureLeftCorneringParameters.longitudinalAcceleration + (pureBrakingParameters.longitudinalAcceleration - pureLeftCorneringParameters.longitudinalAcceleration) * (-Math.Sin(directions[iDirection]));
                    currentAccelerations[1] = pureLeftCorneringParameters.lateralAcceleration * (-Math.Cos(directions[iDirection]));
                }
                else if (directions[iDirection] >= -Math.PI / 2 && directions[iDirection] < 0)
                {
                    currentAccelerations[0] = pureRightCorneringParameters.longitudinalAcceleration + (pureBrakingParameters.longitudinalAcceleration - pureRightCorneringParameters.longitudinalAcceleration) * (-Math.Sin(directions[iDirection]));
                    currentAccelerations[1] = pureRightCorneringParameters.lateralAcceleration * Math.Cos(directions[iDirection]);
                }
                else if (directions[iDirection] >= 0 && directions[iDirection] < Math.PI / 2)
                {
                    currentAccelerations[0] = pureRightCorneringParameters.longitudinalAcceleration + (pureAcceleratingParameters.longitudinalAcceleration - pureRightCorneringParameters.longitudinalAcceleration) * Math.Sin(directions[iDirection]);
                    currentAccelerations[1] = pureRightCorneringParameters.lateralAcceleration * Math.Cos(directions[iDirection]);
                }
                else if (directions[iDirection] >= Math.PI / 2 && directions[iDirection] < Math.PI)
                {
                    currentAccelerations[0] = pureLeftCorneringParameters.longitudinalAcceleration + (pureAcceleratingParameters.longitudinalAcceleration - pureLeftCorneringParameters.longitudinalAcceleration) * Math.Sin(directions[iDirection]);
                    currentAccelerations[1] = pureLeftCorneringParameters.lateralAcceleration * (-Math.Cos(directions[iDirection]));
                }
                // Adds the results to the lists
                LongitudinalAccelerations.Add(currentAccelerations[0]);
                LateralAccelerations.Add(currentAccelerations[1]);
            }
        }
        /// <summary>
        /// Adjusts the accelrations based on the brakes and powertrain limitations.
        /// </summary>
        /// <param name="minimumAcceleration"> Maximum braking acceleration [m/s²] </param>
        /// <param name="maximumAcceleration"> Maximum accelerating acceleration [m/s²] </param>
        private void _AdjustLongitudinalAccelerations(double minimumAcceleration, double maximumAcceleration)
        {
            for (int iDirection = 0; iDirection < LongitudinalAccelerations.Count; iDirection++)
            {
                if (LongitudinalAccelerations[iDirection] < minimumAcceleration)
                {
                    LongitudinalAccelerations[iDirection] = minimumAcceleration;
                }
                else if (LongitudinalAccelerations[iDirection] > maximumAcceleration)
                {
                    LongitudinalAccelerations[iDirection] = maximumAcceleration;
                }
            }
        }
        #endregion
        #endregion
        /*
        #region Single And Multivariate Optimization

        /// <summary>
        /// Gets the acceleration for maximum braking.
        /// </summary>
        /// <returns> Maximum braking acceleration [m/s²]. </returns>
        private double _GetLongitudinalAccelerationForMaximumBraking()
        {
            // Longitudinal Acceleration vs. Longitudinal Load Transfer Optimization loop
            double longitudinalAcceleration = 0;
            double oldLongitudinalAcceleration;
            double tol = 1e-3;
            double error;
            int maxIter = 100;
            int iter = 0;
            do
            {
                // Iteration limit
                iter++;
                // Wheels loads [N]
                double[] wheelsLoads = Car.GetWheelsLoads(Speed, 0, longitudinalAcceleration);
                // Wheels radiuses [m]
                double[] wheelsRadiuses = Car.GetWheelsRadiuses(wheelsLoads);
                // Wheels angular speeds [rad/s]
                double[] wheelsAngularSpeeds = Car.GetWheelsAngularSpeeds(wheelsRadiuses, Speed);
                double referenceWheelAngularSpeed = wheelsAngularSpeeds[0] * Car.Transmission.TorqueBias / (wheelsAngularSpeeds[0] * Car.Transmission.TorqueBias + wheelsAngularSpeeds[1] * (1 - Car.Transmission.TorqueBias));
                // Engine braking torque curve determinaton by interpolation
                alglib.spline1dbuildlinear(Car.WheelRotationalSpeedCurve.ToArray(), Car.WheelBrakingTorqueCurve.ToArray(), out alglib.spline1dinterpolant wheelBrakingTorqueInterp);
                double powertrainBrakingTorque = alglib.spline1dcalc(wheelBrakingTorqueInterp, referenceWheelAngularSpeed);
                // Brakes torques from pedals actuation [Nm]
                double frontBrakeTorque = Car.Brakes.ActualMaximumFrontTorque + powertrainBrakingTorque * Car.Transmission.TorqueBias;
                double rearBrakeTorque = Car.Brakes.ActualMaximumRearTorque + powertrainBrakingTorque * (1 - Car.Transmission.TorqueBias);
                double[] finalBrakeTorques = new double[2] { frontBrakeTorque, rearBrakeTorque };
                // Longitudinal slips for maximum brake torque due to grip
                double frontLongitudinalSlipForMaximumBrake = Car.FrontTire.TireModel.GetLongitudinalSlipForMinimumTireFx(0, wheelsLoads[0], 0, Speed);
                double rearLongitudinalSlipForMaximumBrake = Car.RearTire.TireModel.GetLongitudinalSlipForMinimumTireFx(0, wheelsLoads[1], 0, Speed);
                // Maximum braking wheels torques according to tire grip [Nm]
                double frontGripTorque = Car.FrontTire.TireModel.GetTireFx(frontLongitudinalSlipForMaximumBrake, 0, wheelsLoads[0], 0, Speed) * wheelsRadiuses[0] * 2;
                double rearGripTorque = Car.RearTire.TireModel.GetTireFx(rearLongitudinalSlipForMaximumBrake, 0, wheelsLoads[1], 0, Speed) * wheelsRadiuses[1] * 2;
                // Checks if the tire grip is being exceeded
                if (frontBrakeTorque < frontGripTorque || rearBrakeTorque < rearGripTorque)
                {
                    // Adjust the braking torques accordingly to the tires grips
                    finalBrakeTorques = _AdjustBrakingTorquesForTiresGrips(frontBrakeTorque, rearBrakeTorque, frontGripTorque, rearGripTorque);
                }
                // Determines the longitudinal forces [N]
                double frontLongitudinalForce = finalBrakeTorques[0] / wheelsRadiuses[0];
                double rearLongitudinalForce = finalBrakeTorques[1] / wheelsRadiuses[1];
                // Gets the inertia efficiency [ratio]
                double meanWheelRadius = wheelsRadiuses.Average();
                double inertiaEfficiency = Car.GetInertiaEfficiency(meanWheelRadius);
                // Longitudinal acceleration
                oldLongitudinalAcceleration = longitudinalAcceleration;
                longitudinalAcceleration = (frontLongitudinalForce + rearLongitudinalForce) / Car.InertiaAndDimensions.TotalMass * inertiaEfficiency;
                // Error update
                error = Math.Abs(longitudinalAcceleration - oldLongitudinalAcceleration);
            } while (error > tol && iter < maxIter);
            return longitudinalAcceleration;
        }
        /// <summary>
        /// Function for adjust of the braking torques based on the available tire grips.
        /// </summary>
        /// <param name="frontBrakeTorque"> Initial front brake torque [Nm] </param>
        /// <param name="rearBrakeTorque"> Initial rear brake torque [Nm] </param>
        /// <param name="frontGripTorque"> Available front brake torque (grip) [Nm] </param>
        /// <param name="rearGripTorque"> Available rear brake torque (grip) [Nm] </param>
        /// <returns> Adjusted front and rear brake torques [Nm]. </returns>
        private double[] _AdjustBrakingTorquesForTiresGrips(double frontBrakeTorque, double rearBrakeTorque, double frontGripTorque, double rearGripTorque)
        {
            double[] finalBrakeTorques = new double[2];
            // Gets the front and rear brake to grip ratio
            double frontBrakeToGripRatio = frontBrakeTorque / frontGripTorque;
            double rearBrakeToGripRatio = rearBrakeTorque / rearGripTorque;
            // Checks which ratio is higher
            if (frontBrakeToGripRatio > rearBrakeToGripRatio)
            {
                // Adjusts the torques based on the front grip.
                finalBrakeTorques[0] = frontGripTorque;
                finalBrakeTorques[1] = frontGripTorque * (1 - Car.Brakes.BrakeBias) / Car.Brakes.BrakeBias;
            }
            else
            {
                // Adjusts the torques based on the rear grip.
                finalBrakeTorques[0] = rearGripTorque * Car.Brakes.BrakeBias / (1 - Car.Brakes.BrakeBias);
                finalBrakeTorques[1] = rearGripTorque;
            }

            return finalBrakeTorques;
        }

        /// <summary>
        /// Gets the acceleration for maximum accelerating.
        /// </summary>
        /// <returns> Maximum accelerating acceleration [m/s²]. </returns>
        private double _GetLongitudinalAccelerationForMaximumAccelerating()
        {
            // Longitudinal Acceleration vs. Longitudinal Load Transfer Optimization loop
            double longitudinalAcceleration = 0;
            double oldLongitudinalAcceleration;
            double tol = 1e-3;
            double error;
            int maxIter = 100;
            int iter = 0;
            do
            {
                // Iteration limit
                iter++;
                // Wheels loads [N]
                double[] wheelsLoads = Car.GetWheelsLoads(Speed, 0, longitudinalAcceleration);
                // Wheels radiuses [m]
                double[] wheelsRadiuses = Car.GetWheelsRadiuses(wheelsLoads);
                // Wheels angular speeds [rad/s]
                double[] wheelsAngularSpeeds = Car.GetWheelsAngularSpeeds(wheelsRadiuses, Speed);
                double referenceWheelAngularSpeed = wheelsAngularSpeeds[0] * Car.Transmission.TorqueBias / (wheelsAngularSpeeds[0] * Car.Transmission.TorqueBias + wheelsAngularSpeeds[1] * (1 - Car.Transmission.TorqueBias));
                // Engine braking torque curve determinaton by interpolation
                alglib.spline1dbuildlinear(Car.WheelRotationalSpeedCurve.ToArray(), Car.WheelTorqueCurve.ToArray(), out alglib.spline1dinterpolant wheelTorqueInterp);
                double powertrainTorque = alglib.spline1dcalc(wheelTorqueInterp, referenceWheelAngularSpeed);
                // Brakes torques from pedals actuation [Nm]
                double frontPowertrainTorque = powertrainTorque * Car.Transmission.TorqueBias;
                double rearPowertrainTorque = powertrainTorque * (1 - Car.Transmission.TorqueBias);
                double[] finalPowertainTorques = new double[2] { frontPowertrainTorque, rearPowertrainTorque };
                // Longitudinal slips for maximum brake torque due to grip
                double frontLongitudinalSlipForMaximumAccelerating = Car.FrontTire.TireModel.GetLongitudinalSlipForMaximumTireFx(0, wheelsLoads[0], 0, Speed);
                double rearLongitudinalSlipForMaximumAccelerating = Car.RearTire.TireModel.GetLongitudinalSlipForMaximumTireFx(0, wheelsLoads[1], 0, Speed);
                // Maximum braking wheels torques according to tire grip [Nm]
                double frontGripTorque = Car.FrontTire.TireModel.GetTireFx(frontLongitudinalSlipForMaximumAccelerating, 0, wheelsLoads[0], 0, Speed) * wheelsRadiuses[0] * 2;
                double rearGripTorque = Car.RearTire.TireModel.GetTireFx(rearLongitudinalSlipForMaximumAccelerating, 0, wheelsLoads[1], 0, Speed) * wheelsRadiuses[1] * 2;
                // Checks if the tire grip is being exceeded
                if (frontPowertrainTorque > frontGripTorque || rearPowertrainTorque > rearGripTorque)
                {
                    // Adjust the braking torques accordingly to the tires grips
                    finalPowertainTorques = _AdjustAcceleratingTorquesForTiresGrips(frontPowertrainTorque, rearPowertrainTorque, frontGripTorque, rearGripTorque);
                }
                // Determines the longitudinal forces [N]
                double frontLongitudinalForce = finalPowertainTorques[0] / wheelsRadiuses[0];
                double rearLongitudinalForce = finalPowertainTorques[1] / wheelsRadiuses[1];
                // Gets the inertia efficiency [ratio]
                double meanWheelRadius = wheelsRadiuses.Average();
                double inertiaEfficiency = Car.GetInertiaEfficiency(meanWheelRadius);
                // Longitudinal acceleration
                oldLongitudinalAcceleration = longitudinalAcceleration;
                longitudinalAcceleration = (frontLongitudinalForce + rearLongitudinalForce) / Car.InertiaAndDimensions.TotalMass * inertiaEfficiency;
                // Error update
                error = Math.Abs(longitudinalAcceleration - oldLongitudinalAcceleration);
            } while (error > tol && iter < maxIter);
            return longitudinalAcceleration;
        }
        /// <summary>
        /// Function for adjust of the accelerating torques based on the available tire grips.
        /// </summary>
        /// <param name="frontPowertrainTorque"> Initial front brake torque [Nm] </param>
        /// <param name="rearPowertrainTorque"> Initial rear brake torque [Nm] </param>
        /// <param name="frontGripTorque"> Available front brake torque (grip) [Nm] </param>
        /// <param name="rearGripTorque"> Available rear brake torque (grip) [Nm] </param>
        /// <returns> Adjusted front and rear accelerating torques [Nm]. </returns>
        private double[] _AdjustAcceleratingTorquesForTiresGrips(double frontPowertrainTorque, double rearPowertrainTorque, double frontGripTorque, double rearGripTorque)
        {
            double[] finalBrakeTorques = new double[2];
            // Gets the front and rear brake to grip ratio
            double frontBrakeToGripRatio = frontPowertrainTorque / frontGripTorque;
            double rearBrakeToGripRatio = rearPowertrainTorque / rearGripTorque;
            // Checks which ratio is higher
            if (frontBrakeToGripRatio > rearBrakeToGripRatio)
            {
                // Adjusts the torques based on the front grip.
                finalBrakeTorques[0] = frontGripTorque;
                finalBrakeTorques[1] = frontGripTorque * (1 - Car.Transmission.TorqueBias) / Car.Transmission.TorqueBias;
            }
            else
            {
                // Adjusts the torques based on the rear grip.
                finalBrakeTorques[0] = rearGripTorque * Car.Transmission.TorqueBias / (1 - Car.Transmission.TorqueBias);
                finalBrakeTorques[1] = rearGripTorque;
            }

            return finalBrakeTorques;
        }


        private double _GetLateralAccelerationForMaximumCornering()
        {
            // Optimization parameters
            double epsg = 1e-10;
            double epsf = 0;
            double epsx = 0;
            double diffstep = 1.0e-6;
            int maxits = 100;

            double[] bndl = new double[] { -Car.Steering.MaximumSteeringWheelAngle };
            double[] bndu = new double[] { Car.Steering.MaximumSteeringWheelAngle };

            double[] delta = new double[] { 0 };

            alglib.minbleiccreatef(delta, diffstep, out alglib.minbleicstate state);
            alglib.minbleicsetbc(state, bndl, bndu);
            alglib.minbleicsetcond(state, epsg, epsf, epsx, maxits);
            alglib.minbleicoptimize(state, _SteeringWheelAngleOptimizationForMaximumCornering, null, null);
            alglib.minbleicresults(state, out delta, out alglib.minbleicreport rep);

            return delta[0];
        }
        */
        #endregion
        /* Algorithms which did not work.
        /// <summary>
        /// Generates the GG Diagram.
        /// </summary>
        public void GenerateGGDiagram2()
        {
            longitudinalAccelerationErrors = new List<double>();
            lateralAccelerationErrors = new List<double>();
            // Steering wheel angle and pedals actuation vectors
            double[] pedalActuations = Generate.LinearSpaced(AmountOfPoints * 2, -1, 1);
            double[] steeringWheelAngles = Generate.LinearSpaced(AmountOfPoints, 0, Car.Steering.MaximumSteeringWheelAngle);
            for (int iPedalActuation = 0; iPedalActuation < pedalActuations.Length; iPedalActuation++)
            {
                optimizationInitialGuess = new double[] { 0, 0, 0, 0, 0, 0, 0 };
                for (int iSteeringWheelAngle = 0; iSteeringWheelAngle < steeringWheelAngles.Length; iSteeringWheelAngle++)
                {
                    double[] accelerationsAndCarSlipAngle = _GetAccelerationsWithMultiVariableOptimization(steeringWheelAngles[iSteeringWheelAngle], pedalActuations[iPedalActuation]);
                    LongitudinalAccelerations.Add(accelerationsAndCarSlipAngle[4]);
                    LateralAccelerations.Add(accelerationsAndCarSlipAngle[5]);
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
        #region MultiVariate Optimization 2
        private double[] _GetAccelerationsWithMultiVariableOptimization2(double steeringWheelAngle, double pedalsActuation)
        {
            currentPedalsActuation = pedalsActuation;
            // Wheels steer angles
            currentFrontWheelSteeringAngle = steeringWheelAngle * Car.Steering.FrontSteeringRatio;
            currentRearWheelSteeringAngle = steeringWheelAngle * Car.Steering.RearSteeringRatio;
            // Optimization parameters
            double[] optimizationParameters = optimizationInitialGuess;
            double[] inputScales = new double[] { 1, 1, 1, 1, 100, 100, 1 };
            double stepSizeStopCriteria = 1e-10;
            int maxIter = 100;
            // Optimization setup
            alglib.minlmcreatev(7, optimizationParameters, 1e-3, out alglib.minlmstate state);
            alglib.minlmsetstpmax(state, 5);
            alglib.minlmsetcond(state, stepSizeStopCriteria, maxIter);
            alglib.minlmsetscale(state, inputScales);
            // Runs the optimization
            alglib.minlmoptimize(state, _AccelerationsAndCarSlipAngleMultiVariableOptimizationMethod2, null, null);
            // Optimization results extraction
            alglib.minlmresults(state, out optimizationParameters, out alglib.minlmreport report);
            optimizationInitialGuess = optimizationParameters;
            return optimizationParameters;
        }

        private void _AccelerationsAndCarSlipAngleMultiVariableOptimizationMethod2(double[] optimizationParameters, double[] optimizationOutputs, object obj)
        {
            // Accelerations and car slip angle split
            double frontLongitudinalSlip = optimizationParameters[0];
            double rearLongitudinalSlip = optimizationParameters[1];
            double frontSlipAngle = optimizationParameters[2];
            double rearSlipAngle = optimizationParameters[3];
            double longitudinalAcceleration = optimizationParameters[4];
            double lateralAcceleration = optimizationParameters[5];
            double carSlipAngle = optimizationParameters[6];
            // Yaw rate [rad/s]
            double yawRate = lateralAcceleration / Speed;
            // Longitudinal load transfer [N]
            double longitudinalLoadTransfer = longitudinalAcceleration * Car.InertiaAndDimensions.TotalMass * Car.InertiaAndDimensions.TotalMassCGHeight / Car.InertiaAndDimensions.Wheelbase;
            // Current aerodynamic parameters
            Vehicle.TwoWheelAerodynamicMapPoint currentAerodynamicParameters = Car.GetAerodynamicCoefficients(Speed, carSlipAngle, longitudinalLoadTransfer);
            // Aerodynamic lift force [N]
            double liftForce = -currentAerodynamicParameters.LiftCoefficient * Car.Aerodynamics.FrontalArea * Car.Aerodynamics.AirDensity * Math.Pow(Speed, 2) / 2;
            // Aerodynamic pitch moment [N]
            double aerodynamicPitchMoment = -currentAerodynamicParameters.PitchMomentCoefficient * Car.Aerodynamics.FrontalArea * Car.Aerodynamics.AirDensity * Math.Pow(Speed, 2) / 2;
            // Aerodynamic resultant vertical forces [N]
            double frontAerodynamicVerticalForce = liftForce / 2 - aerodynamicPitchMoment / Car.InertiaAndDimensions.Wheelbase;
            double rearAerodynamicVerticalForce = liftForce / 2 + aerodynamicPitchMoment / Car.InertiaAndDimensions.Wheelbase;
            // Wheels vertical loads [N]
            double frontWheelVerticalLoad = (Car.InertiaAndDimensions.FrontWeight - longitudinalLoadTransfer + frontAerodynamicVerticalForce) / 2;
            double rearWheelVerticalLoad = (Car.InertiaAndDimensions.RearWeight + longitudinalLoadTransfer + rearAerodynamicVerticalForce) / 2;
            if (frontWheelVerticalLoad < 0)
            {
                rearWheelVerticalLoad = rearWheelVerticalLoad + frontWheelVerticalLoad;
                frontWheelVerticalLoad = 0;
            }
            else if (rearWheelVerticalLoad < 0)
            {
                frontWheelVerticalLoad = frontWheelVerticalLoad + rearWheelVerticalLoad;
                rearWheelVerticalLoad = 0;
            }
            // Wheels radiuses [m]
            currentFrontWheelRadius = Car.FrontTire.TireModel.RO - frontWheelVerticalLoad / Car.FrontTire.VerticalStiffness;
            currentRearWheelRadius = Car.RearTire.TireModel.RO - rearWheelVerticalLoad / Car.RearTire.VerticalStiffness;
            // Wheels rotational speeds [rad/s]
            double frontWheelRotationalSpeed = Speed / currentFrontWheelRadius;
            double rearWheelRotationalSpeed = Speed / currentRearWheelRadius;
            // Transmission rotational speed [rad/s] (reference for the engine)
            double transmissionRotationalSpeed = frontWheelRotationalSpeed * Car.Transmission.TorqueBias + rearWheelRotationalSpeed * (1 - Car.Transmission.TorqueBias);
            // Wheels torques limits according to brake/powertrain
            double[] wheelsLimitTorques = _GetWheelsTorquesAccordingToPedalsActuation(transmissionRotationalSpeed);
            double frontWheelTorqueLimit = wheelsLimitTorques[0];
            double rearWheelTorqueLimit = wheelsLimitTorques[1];
            // Wheels torques limits according to tires grips
            double newFrontLongitudinalSlip;
            double newRearLongitudinalSlip;
            if (currentPedalsActuation < 0)
            {
                newFrontLongitudinalSlip = Car.FrontTire.TireModel.GetLongitudinalSlipForMinimumTireFx(frontSlipAngle, frontWheelVerticalLoad, 0, Speed);
                newRearLongitudinalSlip = Car.RearTire.TireModel.GetLongitudinalSlipForMinimumTireFx(rearSlipAngle, rearWheelVerticalLoad, 0, Speed);
            }
            else
            {
                newFrontLongitudinalSlip = Car.FrontTire.TireModel.GetLongitudinalSlipForMaximumTireFx(frontSlipAngle, frontWheelVerticalLoad, 0, Speed);
                newRearLongitudinalSlip = Car.RearTire.TireModel.GetLongitudinalSlipForMaximumTireFx(rearSlipAngle, rearWheelVerticalLoad, 0, Speed);
            }
            double frontWheelGripTorqueLimit = Car.FrontTire.TireModel.GetTireFx(newFrontLongitudinalSlip, frontSlipAngle, frontWheelVerticalLoad, 0, Speed) * currentFrontWheelRadius * 2;
            double rearWheelGripTorqueLimit = Car.RearTire.TireModel.GetTireFx(newRearLongitudinalSlip, rearSlipAngle, rearWheelVerticalLoad, 0, Speed) * currentRearWheelRadius * 2;
            // Gets the final torques at each wheel [Nm]
            double[] wheelsFinalTorques = _GetWheelsFinalTorques(frontWheelTorqueLimit, rearWheelTorqueLimit, frontWheelGripTorqueLimit, rearWheelGripTorqueLimit);
            // Wheels longitudinal forces [N]
            double frontWheelLongitudinalForce = wheelsFinalTorques[0] / currentFrontWheelRadius;
            double rearWheelLongitudinalForce = wheelsFinalTorques[1] / currentRearWheelRadius;
            // Wheels longitudinal slips
            double[] frontKappas;
            double[] rearKappas;
            if (currentPedalsActuation < 0)
            {
                frontKappas = Generate.LinearSpaced(AmountOfPoints, Car.FrontTire.TireModel.KappaMin, 0);
                rearKappas = Generate.LinearSpaced(AmountOfPoints, Car.RearTire.TireModel.KappaMin, 0);
            }
            else
            {
                frontKappas = Generate.LinearSpaced(AmountOfPoints, 0, Car.FrontTire.TireModel.KappaMax);
                rearKappas = Generate.LinearSpaced(AmountOfPoints, 0, Car.RearTire.TireModel.KappaMax);
            }
            newFrontLongitudinalSlip = Car.FrontTire.TireModel.GetLongitudinalSlipForGivenLongitudinalForce(frontWheelLongitudinalForce / 2, frontKappas, frontSlipAngle, frontWheelVerticalLoad, 0, Speed);
            newRearLongitudinalSlip = Car.RearTire.TireModel.GetLongitudinalSlipForGivenLongitudinalForce(rearWheelLongitudinalForce / 2, rearKappas, rearSlipAngle, rearWheelVerticalLoad, 0, Speed);
            // Wheels longitudinal (correction based on longitudinl slip) and lateral forces [N].
            frontWheelLongitudinalForce = 2 * Car.FrontTire.TireModel.GetTireFx(newFrontLongitudinalSlip, frontSlipAngle, frontWheelVerticalLoad, 0, Speed);
            rearWheelLongitudinalForce = 2 * Car.RearTire.TireModel.GetTireFx(newRearLongitudinalSlip, rearSlipAngle, rearWheelVerticalLoad, 0, Speed);
            double frontWheelLateralForce = 2 * Car.FrontTire.TireModel.GetTireFy(newFrontLongitudinalSlip, frontSlipAngle, frontWheelVerticalLoad, 0, Speed);
            double rearWheelLateralForce = 2 * Car.RearTire.TireModel.GetTireFy(newRearLongitudinalSlip, rearSlipAngle, rearWheelVerticalLoad, 0, Speed);
            // Inertia efficiency (due to rotational parts moment of inertia)
            double referenceWheelRadius = (currentFrontWheelRadius + currentRearWheelRadius) / 2;
            double inertiaEfficiency = Math.Pow(referenceWheelRadius, 2) * Car.InertiaAndDimensions.TotalMass /
                (Math.Pow(referenceWheelRadius, 2) * Car.InertiaAndDimensions.TotalMass + Car.InertiaAndDimensions.RotPartsMI);
            // Aerodynamic drag force [N], side force [N] and yaw moment [Nm].
            double dragForce = -currentAerodynamicParameters.DragCoefficient * Car.Aerodynamics.FrontalArea * Car.Aerodynamics.AirDensity * Math.Pow(Speed, 2) / 2;
            double sideForce = -currentAerodynamicParameters.SideForceCoefficient * Car.Aerodynamics.FrontalArea * Car.Aerodynamics.AirDensity * Math.Pow(Speed, 2) / 2;
            double aerodynamicYawMoment = -currentAerodynamicParameters.YawMomentCoefficient * Car.Aerodynamics.FrontalArea * Car.Aerodynamics.AirDensity * Math.Pow(Speed, 2) / 2;
            // Vehicle accelerations [m/s²]
            double newLongitudinalAcceleration = (frontWheelLongitudinalForce + rearWheelLongitudinalForce + dragForce) * inertiaEfficiency / Car.InertiaAndDimensions.TotalMass;
            double newLateralAcceleration = (frontWheelLateralForce + rearWheelLateralForce + sideForce) / Car.InertiaAndDimensions.TotalMass;
            // Wheels traveling speeds [m/s]
            double frontWheelLateralSpeed = (Speed * Math.Sin(-carSlipAngle) + Car.InertiaAndDimensions.DistanceBetweenFrontAxisAndCG * yawRate) * Math.Cos(currentFrontWheelSteeringAngle) - Speed * Math.Cos(-carSlipAngle) * Math.Sin(currentFrontWheelSteeringAngle);
            double frontWheelLongitudinalSpeed = (Speed * Math.Sin(-carSlipAngle) + Car.InertiaAndDimensions.DistanceBetweenFrontAxisAndCG * yawRate) * Math.Sin(currentFrontWheelSteeringAngle) + Speed * Math.Cos(-carSlipAngle) * Math.Cos(currentFrontWheelSteeringAngle);
            double rearWheelLateralSpeed = (Speed * Math.Sin(-carSlipAngle) - Car.InertiaAndDimensions.DistanceBetweenRearAxisAndCG * yawRate) * Math.Cos(currentRearWheelSteeringAngle) - Speed * Math.Cos(-carSlipAngle) * Math.Sin(currentRearWheelSteeringAngle);
            double rearWheelLongitudinalSpeed = (Speed * Math.Sin(-carSlipAngle) - Car.InertiaAndDimensions.DistanceBetweenFrontAxisAndCG * yawRate) * Math.Sin(currentRearWheelSteeringAngle) + Speed * Math.Cos(-carSlipAngle) * Math.Cos(currentRearWheelSteeringAngle);
            // Calculated Slip Angles [rad]
            double newFrontSlipAngle = Math.Atan(frontWheelLateralSpeed / frontWheelLongitudinalSpeed);
            double newRearSlipAngle = Math.Atan(rearWheelLateralSpeed / rearWheelLongitudinalSpeed);
            // Convergence criteria update
            optimizationOutputs[0] = frontLongitudinalSlip - newFrontLongitudinalSlip;
            optimizationOutputs[1] = rearLongitudinalSlip - newRearLongitudinalSlip;
            optimizationOutputs[2] = frontSlipAngle - newFrontSlipAngle;
            optimizationOutputs[3] = rearSlipAngle - newRearSlipAngle;
            optimizationOutputs[4] = longitudinalAcceleration - newLongitudinalAcceleration;
            optimizationOutputs[5] = lateralAcceleration - newLateralAcceleration;
            optimizationOutputs[6] = frontWheelLateralForce * Car.InertiaAndDimensions.DistanceBetweenFrontAxisAndCG - rearWheelLateralForce * Car.InertiaAndDimensions.DistanceBetweenRearAxisAndCG + aerodynamicYawMoment;
        }
        #endregion
        #region Direct Iteration Optimization
        private double[] _GetAccelerationsBasedOnDirectIteration(double steeringWheelAngle, double pedalsActuation)
        {
            currentPedalsActuation = pedalsActuation;
            // Wheels steer angles
            currentFrontWheelSteeringAngle = steeringWheelAngle * Car.Steering.FrontSteeringRatio;
            currentRearWheelSteeringAngle = steeringWheelAngle * Car.Steering.RearSteeringRatio;
            // Optimization parameters
            double lateralAccelerationTol = 1e-3;
            int lateralAccelerationMaxIter = 100;
            double longitudinalAccelerationTol = 1e-4;
            int longitudinalAccelerationMaxIter = 100;
            double yawMomentTol = 1e-3;
            int carSlipAngleMaxIter = 100;
            int differentialMaxIter = 4;
            // Optimization parameters
            double[] accelerationDifferencesAndYawMoment;
            double currentCarSlipAngle = 0;
            double currentLateralAcceleration = 0;
            double currentLongitudinalAcceleration = 0;
            // Optimization loops
            double lateralAccelerationError = 1e10;
            double lateralAccelerationIter = 0;
            do
            {
                // Lateral acceleration iteration number increment
                lateralAccelerationIter++;
                // Longitudinal acceleration loop
                double longitudinalAccelerationError = 1e10;
                double longitudinalAccelerationIter = 0;
                do
                {
                    // Longitudinal acceleration iteration number increment
                    longitudinalAccelerationIter++;
                    // Car slip angle loop (Secant method)
                    double currentYawMoment;
                    double carSlipAngleIter = 0;
                    double differentialStep = 1e-3;
                    do
                    {
                        // Car slip angle iteration number increment
                        carSlipAngleIter++;

                        // Optimization parameters array
                        double[] accelerationsAndCarSlipAngle = new double[3] { currentLongitudinalAcceleration, currentLateralAcceleration, currentCarSlipAngle };
                        // Parameters optimization
                        accelerationDifferencesAndYawMoment = _AccelerationsAndCarSlipAngleOptimizationMethod(accelerationsAndCarSlipAngle);
                        // Yaw moment update
                        currentYawMoment = accelerationDifferencesAndYawMoment[2];
                        // Differential calculation loop
                        double differential;
                        int differentialIter = 0;
                        do
                        {
                            // Values for the reference of the differential
                            accelerationsAndCarSlipAngle[2] = currentCarSlipAngle + differentialStep * Math.Pow(10, differentialIter);
                            double[] accelerationDifferencesAndYawMomentForDifferentialUpdate = _AccelerationsAndCarSlipAngleOptimizationMethod(accelerationsAndCarSlipAngle);
                            // Differential calculation
                            differential = (accelerationDifferencesAndYawMomentForDifferentialUpdate[2] - accelerationDifferencesAndYawMoment[2]) / (differentialStep * Math.Pow(10, differentialIter));
                            // Iteration number update
                            differentialIter++;
                        } while (differential == 0 && differentialIter < differentialMaxIter);
                        // Car slip angle update
                        currentCarSlipAngle -= currentYawMoment / differential;
                        if (double.IsInfinity(currentCarSlipAngle))
                        {
                            bool isInf = true;
                        }
                    } while (Math.Abs(currentYawMoment) > yawMomentTol && carSlipAngleIter < carSlipAngleMaxIter);
                    // Longitudinal acceleration error criteria and current value update
                    longitudinalAccelerationError = accelerationDifferencesAndYawMoment[0];
                    //longitudinalAccelerationErrors.Add(longitudinalAccelerationError);
                    currentLongitudinalAcceleration += accelerationDifferencesAndYawMoment[0];
                } while (Math.Abs(longitudinalAccelerationError) > longitudinalAccelerationTol && longitudinalAccelerationIter < longitudinalAccelerationMaxIter);
                // Lateral acceleration error criteria and current value update
                lateralAccelerationError = accelerationDifferencesAndYawMoment[1];
                currentLateralAcceleration += accelerationDifferencesAndYawMoment[1];
            } while (Math.Abs(lateralAccelerationError) > lateralAccelerationTol && lateralAccelerationIter < lateralAccelerationMaxIter);

            return new double[] { currentLongitudinalAcceleration, currentLateralAcceleration};
        }

        private double[] _AccelerationsAndCarSlipAngleOptimizationMethod(double[] accelerationsAndCarSlipAngle)
        {
            // Accelerations and car slip angle split
            double longitudinalAcceleration = accelerationsAndCarSlipAngle[0];
            double lateralAcceleration = accelerationsAndCarSlipAngle[1];
            double carSlipAngle = accelerationsAndCarSlipAngle[2];
            // Yaw rate [rad/s]
            double yawRate = lateralAcceleration / Speed;
            // Slip Angles [rad]
            double frontWheelLateralSpeed = (Speed * Math.Sin(-carSlipAngle) + Car.InertiaAndDimensions.DistanceBetweenFrontAxisAndCG * yawRate) * Math.Cos(currentFrontWheelSteeringAngle) - Speed * Math.Cos(-carSlipAngle) * Math.Sin(currentFrontWheelSteeringAngle);
            double frontWheelLongitudinalSpeed = (Speed * Math.Sin(-carSlipAngle) + Car.InertiaAndDimensions.DistanceBetweenFrontAxisAndCG * yawRate) * Math.Sin(currentFrontWheelSteeringAngle) + Speed * Math.Cos(-carSlipAngle) * Math.Cos(currentFrontWheelSteeringAngle);
            double frontSlipAngle = Math.Atan(frontWheelLateralSpeed / frontWheelLongitudinalSpeed);
            double rearWheelLateralSpeed = (Speed * Math.Sin(-carSlipAngle) - Car.InertiaAndDimensions.DistanceBetweenRearAxisAndCG * yawRate) * Math.Cos(currentRearWheelSteeringAngle) - Speed * Math.Cos(-carSlipAngle) * Math.Sin(currentRearWheelSteeringAngle);
            double rearWheelLongitudinalSpeed = (Speed * Math.Sin(-carSlipAngle) - Car.InertiaAndDimensions.DistanceBetweenFrontAxisAndCG * yawRate) * Math.Sin(currentRearWheelSteeringAngle) + Speed * Math.Cos(-carSlipAngle) * Math.Cos(currentRearWheelSteeringAngle);
            double rearSlipAngle = Math.Atan(rearWheelLateralSpeed / rearWheelLongitudinalSpeed);
            // Longitudinal load transfer [N]
            double longitudinalLoadTransfer = longitudinalAcceleration * Car.InertiaAndDimensions.TotalMass * Car.InertiaAndDimensions.TotalMassCGHeight / Car.InertiaAndDimensions.Wheelbase;
            // Current aerodynamic parameters
            Vehicle.TwoWheelAerodynamicMapPoint currentAerodynamicParameters = Car.GetAerodynamicCoefficients(Speed, carSlipAngle, longitudinalLoadTransfer);
            // Aerodynamic lift force [N]
            double liftForce = -currentAerodynamicParameters.LiftCoefficient * Car.Aerodynamics.FrontalArea * Car.Aerodynamics.AirDensity * Math.Pow(Speed, 2) / 2;
            // Aerodynamic pitch moment [N]
            double aerodynamicPitchMoment = -currentAerodynamicParameters.PitchMomentCoefficient * Car.Aerodynamics.FrontalArea * Car.Aerodynamics.AirDensity * Math.Pow(Speed, 2) / 2;
            // Aerodynamic resultant vertical forces [N]
            double frontAerodynamicVerticalForce = liftForce / 2 - aerodynamicPitchMoment / Car.InertiaAndDimensions.Wheelbase;
            double rearAerodynamicVerticalForce = liftForce / 2 + aerodynamicPitchMoment / Car.InertiaAndDimensions.Wheelbase;
            // Wheels vertical loads [N]
            double frontWheelVerticalLoad = (Car.InertiaAndDimensions.FrontWeight - longitudinalLoadTransfer + frontAerodynamicVerticalForce) / 2;
            double rearWheelVerticalLoad = (Car.InertiaAndDimensions.RearWeight + longitudinalLoadTransfer + rearAerodynamicVerticalForce) / 2;
            if (frontWheelVerticalLoad < 0)
            {
                rearWheelVerticalLoad = rearWheelVerticalLoad + frontWheelVerticalLoad;
                frontWheelVerticalLoad = 0;
            }
            else if (rearWheelVerticalLoad < 0)
            {
                frontWheelVerticalLoad = frontWheelVerticalLoad + rearWheelVerticalLoad;
                rearWheelVerticalLoad = 0;
            }
            // Wheels radiuses [m]
            currentFrontWheelRadius = Car.FrontTire.TireModel.RO - frontWheelVerticalLoad / Car.FrontTire.VerticalStiffness;
            currentRearWheelRadius = Car.RearTire.TireModel.RO - rearWheelVerticalLoad / Car.RearTire.VerticalStiffness;
            // Wheels rotational speeds [rad/s]
            double frontWheelRotationalSpeed = Speed / currentFrontWheelRadius;
            double rearWheelRotationalSpeed = Speed / currentRearWheelRadius;
            // Transmission rotational speed [rad/s] (reference for the engine)
            double transmissionRotationalSpeed = frontWheelRotationalSpeed * Car.Transmission.TorqueBias + rearWheelRotationalSpeed * (1 - Car.Transmission.TorqueBias);
            // Wheels torques limits according to brake/powertrain
            double[] wheelsLimitTorques = _GetWheelsTorquesAccordingToPedalsActuation(transmissionRotationalSpeed);
            double frontWheelTorqueLimit = wheelsLimitTorques[0];
            double rearWheelTorqueLimit = wheelsLimitTorques[1];
            // Wheels torques limits according to tires grips
            double frontWheelSlipAngle;
            double rearWheelSlipAngle;
            if (currentPedalsActuation < 0)
            {
                frontWheelSlipAngle = Car.FrontTire.TireModel.GetLongitudinalSlipForMinimumTireFx(frontSlipAngle, frontWheelVerticalLoad, 0, Speed);
                rearWheelSlipAngle = Car.RearTire.TireModel.GetLongitudinalSlipForMinimumTireFx(rearSlipAngle, rearWheelVerticalLoad, 0, Speed);
            }
            else
            {
                frontWheelSlipAngle = Car.FrontTire.TireModel.GetLongitudinalSlipForMaximumTireFx(frontSlipAngle, frontWheelVerticalLoad, 0, Speed);
                rearWheelSlipAngle = Car.RearTire.TireModel.GetLongitudinalSlipForMaximumTireFx(rearSlipAngle, rearWheelVerticalLoad, 0, Speed);
            }
            double frontWheelGripTorqueLimit = Car.FrontTire.TireModel.GetTireFx(frontWheelSlipAngle, frontSlipAngle, frontWheelVerticalLoad, 0, Speed) * currentFrontWheelRadius * 2;
            double rearWheelGripTorqueLimit = Car.RearTire.TireModel.GetTireFx(rearWheelSlipAngle, rearSlipAngle, rearWheelVerticalLoad, 0, Speed) * currentRearWheelRadius * 2;
            // Gets the final torques at each wheel [Nm]
            double[] wheelsFinalTorques = _GetWheelsFinalTorques(frontWheelTorqueLimit, rearWheelTorqueLimit, frontWheelGripTorqueLimit, rearWheelGripTorqueLimit);
            // Wheels longitudinal forces [N]
            double frontWheelLongitudinalForce = wheelsFinalTorques[0] / currentFrontWheelRadius;
            double rearWheelLongitudinalForce = wheelsFinalTorques[1] / currentRearWheelRadius;
            // Wheels longitudinal slips
            double[] frontKappas;
            double[] rearKappas;
            if (currentPedalsActuation < 0)
            {
                frontKappas = Generate.LinearSpaced(50, Car.FrontTire.TireModel.KappaMin, 0);
                rearKappas = Generate.LinearSpaced(50, Car.RearTire.TireModel.KappaMin, 0);
            }
            else
            {
                frontKappas = Generate.LinearSpaced(50, 0, Car.FrontTire.TireModel.KappaMax);
                rearKappas = Generate.LinearSpaced(50, 0, Car.RearTire.TireModel.KappaMax);
            }
            double frontWheelLongitudinalSlip = Car.FrontTire.TireModel.GetLongitudinalSlipForGivenLongitudinalForce(frontWheelLongitudinalForce / 2, frontKappas, frontWheelSlipAngle, frontWheelVerticalLoad, 0, Speed);
            double rearWheelLongitudinalSlip = Car.RearTire.TireModel.GetLongitudinalSlipForGivenLongitudinalForce(rearWheelLongitudinalForce / 2, rearKappas, rearWheelSlipAngle, rearWheelVerticalLoad, 0, Speed);
            // Wheels longitudinal (correction based on longitudinl slip) and lateral forces [N].
            frontWheelLongitudinalForce = 2 * Car.FrontTire.TireModel.GetTireFx(frontWheelLongitudinalSlip, frontWheelSlipAngle, frontWheelVerticalLoad, 0, Speed);
            rearWheelLongitudinalForce = 2 * Car.RearTire.TireModel.GetTireFx(rearWheelLongitudinalSlip, rearWheelSlipAngle, rearWheelVerticalLoad, 0, Speed);
            double frontWheelLateralForce = 2 * Car.FrontTire.TireModel.GetTireFy(frontWheelLongitudinalSlip, frontWheelSlipAngle, frontWheelVerticalLoad, 0, Speed);
            double rearWheelLateralForce = 2 * Car.RearTire.TireModel.GetTireFy(rearWheelLongitudinalSlip, rearWheelSlipAngle, rearWheelVerticalLoad, 0, Speed);
            // Front and rear axis tire forces
            double frontAxisLongitudinalForceDueToTire = frontWheelLongitudinalForce * Math.Cos(currentFrontWheelSteeringAngle) - frontWheelLateralForce * Math.Sin(currentFrontWheelSteeringAngle);
            double rearAxisLongitudinalForceDueToTire = rearWheelLongitudinalForce * Math.Cos(currentRearWheelSteeringAngle) - rearWheelLongitudinalForce * Math.Sin(currentRearWheelSteeringAngle);
            double frontAxisLateralForceDueToTire = frontWheelLateralForce * Math.Cos(currentFrontWheelSteeringAngle) + frontWheelLongitudinalForce * Math.Sin(currentFrontWheelSteeringAngle);
            double rearAxisLateralForceDueToTire = rearWheelLateralForce * Math.Cos(currentRearWheelSteeringAngle) + rearWheelLongitudinalForce * Math.Sin(currentRearWheelSteeringAngle);
            // Inertia efficiency (due to rotational parts moment of inertia)
            double referenceWheelRadius = (currentFrontWheelRadius + currentRearWheelRadius) / 2;
            double inertiaEfficiency = Math.Pow(referenceWheelRadius, 2) * Car.InertiaAndDimensions.TotalMass /
                (Math.Pow(referenceWheelRadius, 2) * Car.InertiaAndDimensions.TotalMass + Car.InertiaAndDimensions.RotPartsMI);
            // Aerodynamic drag force [N], side force [N] and yaw moment [Nm].
            double dragForce = -currentAerodynamicParameters.DragCoefficient * Car.Aerodynamics.FrontalArea * Car.Aerodynamics.AirDensity * Math.Pow(Speed, 2) / 2;
            double sideForce = -currentAerodynamicParameters.SideForceCoefficient * Car.Aerodynamics.FrontalArea * Car.Aerodynamics.AirDensity * Math.Pow(Speed, 2) / 2;
            double aerodynamicYawMoment = -currentAerodynamicParameters.YawMomentCoefficient * Car.Aerodynamics.FrontalArea * Car.Aerodynamics.AirDensity * Math.Pow(Speed, 2) / 2;
            // Vehicle accelerations [m/s²]
            double newLongitudinalAcceleration = (frontAxisLongitudinalForceDueToTire + rearAxisLongitudinalForceDueToTire + dragForce) * inertiaEfficiency / Car.InertiaAndDimensions.TotalMass;
            double newLateralAcceleration = (frontAxisLateralForceDueToTire + rearAxisLateralForceDueToTire + sideForce) / Car.InertiaAndDimensions.TotalMass;
            // Convergence criteria update
            double[] accelerationDifferencesAndYawMoment = new double[3];
            accelerationDifferencesAndYawMoment[0] = newLongitudinalAcceleration - longitudinalAcceleration;
            accelerationDifferencesAndYawMoment[1] = newLateralAcceleration - lateralAcceleration;
            accelerationDifferencesAndYawMoment[2] = frontAxisLateralForceDueToTire * Car.InertiaAndDimensions.DistanceBetweenFrontAxisAndCG - rearAxisLateralForceDueToTire * Car.InertiaAndDimensions.DistanceBetweenRearAxisAndCG + aerodynamicYawMoment;
            return accelerationDifferencesAndYawMoment;
        }
        #endregion
        #region Multivariate Optimization 1
        /// <summary>
        /// Gets the accelerations via multivariable optimization for a combination of steering wheel angle and pedals (throttle/brake) actuation.
        /// </summary>
        /// <param name="steeringWheelAngle"> Angle of the steering wheel [rad]. </param>
        /// <param name="pedalsActuation"> Actuation of the throttle/brake pedals [-1 <= ratio <= 1] </param>
        private double[] _GetAccelerationsWithMultiVariableOptimization(double steeringWheelAngle, double pedalsActuation)
        {
            currentPedalsActuation = pedalsActuation;
            // Wheels steer angles
            currentFrontWheelSteeringAngle = steeringWheelAngle * Car.Steering.FrontSteeringRatio;
            currentRearWheelSteeringAngle = steeringWheelAngle * Car.Steering.RearSteeringRatio;
            // Optimization parameters
            double[] accelerationsAndCarSlipAngle = new double[] { 0, 0, 0 };
            double[] inputScales = new double[] { 100, 100, 1 };
            double stepSizeStopCriteria = 1e-10;
            int maxIter = 1000;
            // Optimization setup
            alglib.minlmcreatev(3, accelerationsAndCarSlipAngle, 0.0001, out alglib.minlmstate state);
            alglib.minlmsetstpmax(state, 5);
            alglib.minlmsetcond(state, stepSizeStopCriteria, maxIter);
            alglib.minlmsetscale(state, inputScales);
            // Runs the optimization
            alglib.minlmoptimize(state, _AccelerationsAndCarSlipAngleMultiVariableOptimizationMethod, null, null);
            // Optimization results extraction
            alglib.minlmresults(state, out accelerationsAndCarSlipAngle, out alglib.minlmreport report);
            return accelerationsAndCarSlipAngle;
        }
        /// <summary>
        /// Used in to find the vehicle accelerations for given steering wheel angle and brake/throttle actuation by optimization.
        /// </summary>
        /// <param name="accelerationsAndCarSlipAngle"> [0]: Longitudinal Acceleration [m/s²] - [1] Lateral Acceleration [m/s²] - [2] Car Slip Angle [deg] (to have a better scalling of the problem) </param>
        /// <param name="accelerationDifferencesAndYawMoment"> [0]: Longitudinal Acceleration Difference [m/s²] - [1] Lateral Acceleration Difference [m/s²] - [2] Yaw Moment [Nm] </param>
        /// <param name="obj"></param>
        private void _AccelerationsAndCarSlipAngleMultiVariableOptimizationMethod(double[] accelerationsAndCarSlipAngle, double[] accelerationDifferencesAndYawMoment, object obj)
        {
            // Accelerations and car slip angle split
            double longitudinalAcceleration = accelerationsAndCarSlipAngle[0];
            double lateralAcceleration = accelerationsAndCarSlipAngle[1];
            double carSlipAngle = accelerationsAndCarSlipAngle[2] * Math.PI / 180;
            // Yaw rate [rad/s]
            double yawRate = lateralAcceleration / Speed;
            // Slip Angles [rad]
            double frontWheelLateralSpeed = (Speed * Math.Sin(-carSlipAngle) + Car.InertiaAndDimensions.DistanceBetweenFrontAxisAndCG * yawRate) * Math.Cos(currentFrontWheelSteeringAngle) - Speed * Math.Cos(-carSlipAngle) * Math.Sin(currentFrontWheelSteeringAngle);
            double frontWheelLongitudinalSpeed = (Speed * Math.Sin(-carSlipAngle) + Car.InertiaAndDimensions.DistanceBetweenFrontAxisAndCG * yawRate) * Math.Sin(currentFrontWheelSteeringAngle) + Speed * Math.Cos(-carSlipAngle) * Math.Cos(currentFrontWheelSteeringAngle);
            double frontSlipAngle = Math.Atan(frontWheelLateralSpeed / frontWheelLongitudinalSpeed);
            double rearWheelLateralSpeed = (Speed * Math.Sin(-carSlipAngle) - Car.InertiaAndDimensions.DistanceBetweenRearAxisAndCG * yawRate) * Math.Cos(currentRearWheelSteeringAngle) - Speed * Math.Cos(-carSlipAngle) * Math.Sin(currentRearWheelSteeringAngle);
            double rearWheelLongitudinalSpeed = (Speed * Math.Sin(-carSlipAngle) - Car.InertiaAndDimensions.DistanceBetweenFrontAxisAndCG * yawRate) * Math.Sin(currentRearWheelSteeringAngle) + Speed * Math.Cos(-carSlipAngle) * Math.Cos(currentRearWheelSteeringAngle);
            double rearSlipAngle = Math.Atan(rearWheelLateralSpeed / rearWheelLongitudinalSpeed);
            // Longitudinal load transfer [N]
            double longitudinalLoadTransfer = longitudinalAcceleration * Car.InertiaAndDimensions.TotalMass * Car.InertiaAndDimensions.TotalMassCGHeight / Car.InertiaAndDimensions.Wheelbase;
            // Current aerodynamic parameters
            Vehicle.TwoWheelAerodynamicMapPoint currentAerodynamicParameters = Car.GetAerodynamicCoefficients(Speed, carSlipAngle, longitudinalLoadTransfer);
            // Aerodynamic lift force [N]
            double liftForce = -currentAerodynamicParameters.LiftCoefficient * Car.Aerodynamics.FrontalArea * Car.Aerodynamics.AirDensity * Math.Pow(Speed, 2) / 2;
            // Aerodynamic pitch moment [N]
            double aerodynamicPitchMoment = -currentAerodynamicParameters.PitchMomentCoefficient * Car.Aerodynamics.FrontalArea * Car.Aerodynamics.AirDensity * Math.Pow(Speed, 2) / 2;
            // Aerodynamic resultant vertical forces [N]
            double frontAerodynamicVerticalForce = liftForce / 2 - aerodynamicPitchMoment / Car.InertiaAndDimensions.Wheelbase;
            double rearAerodynamicVerticalForce = liftForce / 2 + aerodynamicPitchMoment / Car.InertiaAndDimensions.Wheelbase;
            // Wheels vertical loads [N]
            double frontWheelVerticalLoad = (Car.InertiaAndDimensions.FrontWeight - longitudinalLoadTransfer + frontAerodynamicVerticalForce) / 2;
            double rearWheelVerticalLoad = (Car.InertiaAndDimensions.RearWeight + longitudinalLoadTransfer + rearAerodynamicVerticalForce) / 2;
            if (frontWheelVerticalLoad < 0)
            {
                rearWheelVerticalLoad = rearWheelVerticalLoad + frontWheelVerticalLoad;
                frontWheelVerticalLoad = 0;
            }
            else if (rearWheelVerticalLoad < 0)
            {
                frontWheelVerticalLoad = frontWheelVerticalLoad + rearWheelVerticalLoad;
                rearWheelVerticalLoad = 0;
            }
            // Wheels radiuses [m]
            currentFrontWheelRadius = Car.FrontTire.TireModel.RO - frontWheelVerticalLoad / Car.FrontTire.VerticalStiffness;
            currentRearWheelRadius = Car.RearTire.TireModel.RO - rearWheelVerticalLoad / Car.RearTire.VerticalStiffness;
            // Wheels rotational speeds [rad/s]
            double frontWheelRotationalSpeed = Speed / currentFrontWheelRadius;
            double rearWheelRotationalSpeed = Speed / currentRearWheelRadius;
            // Transmission rotational speed [rad/s] (reference for the engine)
            double transmissionRotationalSpeed = frontWheelRotationalSpeed * Car.Transmission.TorqueBias + rearWheelRotationalSpeed * (1 - Car.Transmission.TorqueBias);
            // Wheels torques limits according to brake/powertrain
            double[] wheelsLimitTorques = _GetWheelsTorquesAccordingToPedalsActuation(transmissionRotationalSpeed);
            double frontWheelTorqueLimit = wheelsLimitTorques[0];
            double rearWheelTorqueLimit = wheelsLimitTorques[1];
            // Wheels torques limits according to tires grips
            double frontWheelLongitudinalSlip;
            double rearWheelLongitudinalSlip;
            if (currentPedalsActuation < 0)
            {
                frontWheelLongitudinalSlip = Car.FrontTire.TireModel.GetLongitudinalSlipForMinimumTireFx(frontSlipAngle, frontWheelVerticalLoad, 0, Speed);
                rearWheelLongitudinalSlip = Car.RearTire.TireModel.GetLongitudinalSlipForMinimumTireFx(rearSlipAngle, rearWheelVerticalLoad, 0, Speed);
            }
            else
            {
                frontWheelLongitudinalSlip = Car.FrontTire.TireModel.GetLongitudinalSlipForMaximumTireFx(frontSlipAngle, frontWheelVerticalLoad, 0, Speed);
                rearWheelLongitudinalSlip = Car.RearTire.TireModel.GetLongitudinalSlipForMaximumTireFx(rearSlipAngle, rearWheelVerticalLoad, 0, Speed);
            }
            double frontWheelGripTorqueLimit = Car.FrontTire.TireModel.GetTireFx(frontWheelLongitudinalSlip, frontSlipAngle, frontWheelVerticalLoad, 0, Speed) * currentFrontWheelRadius * 2;
            double rearWheelGripTorqueLimit = Car.RearTire.TireModel.GetTireFx(rearWheelLongitudinalSlip, rearSlipAngle, rearWheelVerticalLoad, 0, Speed) * currentRearWheelRadius * 2;
            // Gets the final torques at each wheel [Nm]
            double[] wheelsFinalTorques = _GetWheelsFinalTorques(frontWheelTorqueLimit, rearWheelTorqueLimit, frontWheelGripTorqueLimit, rearWheelGripTorqueLimit);
            // Wheels longitudinal forces [N]
            double frontWheelLongitudinalForce = wheelsFinalTorques[0] / currentFrontWheelRadius;
            double rearWheelLongitudinalForce = wheelsFinalTorques[1] / currentRearWheelRadius;
            // Wheels longitudinal slips
            double[] frontKappas;
            double[] rearKappas;
            if (currentPedalsActuation < 0)
            {
                frontKappas = Generate.LinearSpaced(AmountOfPoints, Car.FrontTire.TireModel.KappaMin, 0);
                rearKappas = Generate.LinearSpaced(AmountOfPoints, Car.RearTire.TireModel.KappaMin, 0);
            }
            else
            {
                frontKappas = Generate.LinearSpaced(AmountOfPoints, 0, Car.FrontTire.TireModel.KappaMax);
                rearKappas = Generate.LinearSpaced(AmountOfPoints, 0, Car.RearTire.TireModel.KappaMax);
            }
            frontWheelLongitudinalSlip = Car.FrontTire.TireModel.GetLongitudinalSlipForGivenLongitudinalForce(frontWheelLongitudinalForce / 2, frontKappas, frontSlipAngle, frontWheelVerticalLoad, 0, Speed);
            rearWheelLongitudinalSlip = Car.RearTire.TireModel.GetLongitudinalSlipForGivenLongitudinalForce(rearWheelLongitudinalForce / 2, rearKappas, rearSlipAngle, rearWheelVerticalLoad, 0, Speed);
            // Wheels longitudinal (correction based on longitudinl slip) and lateral forces [N].
            frontWheelLongitudinalForce = 2 * Car.FrontTire.TireModel.GetTireFx(frontWheelLongitudinalSlip, frontSlipAngle, frontWheelVerticalLoad, 0, Speed);
            rearWheelLongitudinalForce = 2 * Car.RearTire.TireModel.GetTireFx(rearWheelLongitudinalSlip, rearSlipAngle, rearWheelVerticalLoad, 0, Speed);
            double frontWheelLateralForce = 2 * Car.FrontTire.TireModel.GetTireFy(frontWheelLongitudinalSlip, frontSlipAngle, frontWheelVerticalLoad, 0, Speed);
            double rearWheelLateralForce = 2 * Car.RearTire.TireModel.GetTireFy(rearWheelLongitudinalSlip, rearSlipAngle, rearWheelVerticalLoad, 0, Speed);
            // Inertia efficiency (due to rotational parts moment of inertia)
            double referenceWheelRadius = (currentFrontWheelRadius + currentRearWheelRadius) / 2;
            double inertiaEfficiency = Math.Pow(referenceWheelRadius, 2) * Car.InertiaAndDimensions.TotalMass /
                (Math.Pow(referenceWheelRadius, 2) * Car.InertiaAndDimensions.TotalMass + Car.InertiaAndDimensions.RotPartsMI);
            // Aerodynamic drag force [N], side force [N] and yaw moment [Nm].
            double dragForce = -currentAerodynamicParameters.DragCoefficient * Car.Aerodynamics.FrontalArea * Car.Aerodynamics.AirDensity * Math.Pow(Speed, 2) / 2;
            double sideForce = -currentAerodynamicParameters.SideForceCoefficient * Car.Aerodynamics.FrontalArea * Car.Aerodynamics.AirDensity * Math.Pow(Speed, 2) / 2;
            double aerodynamicYawMoment = -currentAerodynamicParameters.YawMomentCoefficient * Car.Aerodynamics.FrontalArea * Car.Aerodynamics.AirDensity * Math.Pow(Speed, 2) / 2;
            // Vehicle accelerations [m/s²]
            double newLongitudinalAcceleration = (frontWheelLongitudinalForce + rearWheelLongitudinalForce + dragForce) * inertiaEfficiency / Car.InertiaAndDimensions.TotalMass;
            double newLateralAcceleration = (frontWheelLateralForce + rearWheelLateralForce + sideForce) / Car.InertiaAndDimensions.TotalMass;
            // Convergence criteria update
            accelerationDifferencesAndYawMoment[0] = longitudinalAcceleration - newLongitudinalAcceleration;
            accelerationDifferencesAndYawMoment[1] = lateralAcceleration - newLateralAcceleration;
            accelerationDifferencesAndYawMoment[2] = frontWheelLateralForce * Car.InertiaAndDimensions.DistanceBetweenFrontAxisAndCG - rearWheelLateralForce * Car.InertiaAndDimensions.DistanceBetweenRearAxisAndCG + aerodynamicYawMoment;
        }
        /// <summary>
        /// Gets the torques at the front and rear wheels based on the pedals actuation and wheels rotational speeds
        /// </summary>
        /// <param name="transmissionRotationalSpeed"> Reference rotational speed for the engine [rad/s]. </param>
        /// <returns> Torques at the front and rear wheels [Nm] </returns>
        private double[] _GetWheelsTorquesAccordingToPedalsActuation(double transmissionRotationalSpeed)
        {
            double[] wheelsLimitTorques = new double[2];
            // Checks if the car is braking or accelerating.
            if (currentPedalsActuation < 0) // Braking
            {
                // Engine braking torque [Nm]
                alglib.spline1dbuildlinear(Car.WheelRotationalSpeedCurve.ToArray(), Car.WheelBrakingTorqueCurve.ToArray(), out alglib.spline1dinterpolant wheelBrakingTorqueInterp);
                double powertrainBrakingTorque = alglib.spline1dcalc(wheelBrakingTorqueInterp, transmissionRotationalSpeed);
                // Front and rear powertrain braking torques [Nm]
                double frontPowertrainBrakingTorque = powertrainBrakingTorque * Car.Transmission.TorqueBias;
                double rearPowertrainBrakingTorque = powertrainBrakingTorque * (1 - Car.Transmission.TorqueBias);
                // Front and rear limit torques [Nm]
                wheelsLimitTorques[0] = frontPowertrainBrakingTorque - currentPedalsActuation * Car.Brakes.FrontMaximumTorque;
                wheelsLimitTorques[1] = rearPowertrainBrakingTorque - currentPedalsActuation * Car.Brakes.RearMaximumTorque;
            }
            else // Accelerating
            {
                // Engine torque [Nm]
                alglib.spline1dbuildlinear(Car.WheelRotationalSpeedCurve.ToArray(), Car.WheelTorqueCurve.ToArray(), out alglib.spline1dinterpolant wheelTorqueInterp);
                double powertrainTorque = alglib.spline1dcalc(wheelTorqueInterp, transmissionRotationalSpeed);
                // Front and rear limit torques [Nm]
                wheelsLimitTorques[0] = currentPedalsActuation * powertrainTorque * Car.Transmission.TorqueBias;
                wheelsLimitTorques[1] = currentPedalsActuation * powertrainTorque * (1 - Car.Transmission.TorqueBias);
            }
            return wheelsLimitTorques;
        }
        /// <summary>
        /// Gets the torques at each wheel.
        /// </summary>
        /// <param name="frontWheelTorqueLimit"> Torque limit due to brakes/powertrain at the front wheel [Nm]. </param>
        /// <param name="rearWheelTorqueLimit"> Torque limit due to brakes/powertrain at the rear wheel [Nm]. </param>
        /// <param name="frontWheelGripTorqueLimit"> Torque limit due to tire grip at the front wheel [Nm]. </param>
        /// <param name="rearWheelGripTorqueLimit"> Torque limit due to tire grip at the front wheel [Nm]. </param>
        /// <returns> [0]: Front wheel torque [Nm] - [1]: Rear wheel torque [Nm] </returns>
        private double[] _GetWheelsFinalTorques(double frontWheelTorqueLimit, double rearWheelTorqueLimit, double frontWheelGripTorqueLimit, double rearWheelGripTorqueLimit)
        {
            // Final torques initial values
            double frontFinalTorque = frontWheelGripTorqueLimit;
            double rearFinalTorque = rearWheelGripTorqueLimit;
            // Checks the value of the pedal actuation to determine if the brakes or the powertrain is being used.
            if (currentPedalsActuation < 0)
            {
                // Correction due to limit torque values
                if (frontFinalTorque < frontWheelTorqueLimit) frontFinalTorque = frontWheelTorqueLimit;
                if (rearFinalTorque < rearWheelTorqueLimit) rearFinalTorque = rearWheelTorqueLimit;
                // Current bias ratio
                double biasRatio = frontWheelGripTorqueLimit / (frontWheelGripTorqueLimit + rearWheelGripTorqueLimit);
                // Correction due to bias ratio
                if (Car.Brakes.BrakeBias == 0) frontFinalTorque = 0;
                else if (Car.Brakes.BrakeBias == 1) rearFinalTorque = 0;
                else if (biasRatio > Car.Brakes.BrakeBias) frontFinalTorque = rearFinalTorque * Car.Brakes.BrakeBias / (1 - Car.Brakes.BrakeBias);
                else if (biasRatio < Car.Brakes.BrakeBias) rearFinalTorque = frontFinalTorque * (1 - Car.Brakes.BrakeBias) / Car.Brakes.BrakeBias;
            }
            else
            {
                // Correction due to limit torque values
                if (frontFinalTorque > frontWheelTorqueLimit) frontFinalTorque = frontWheelTorqueLimit;
                if (rearFinalTorque > rearWheelTorqueLimit) rearFinalTorque = rearWheelTorqueLimit;
                // Current bias ratio
                double biasRatio = frontWheelGripTorqueLimit / (frontWheelGripTorqueLimit + rearWheelGripTorqueLimit);
                // Correction due to bias ratio
                if (Car.Transmission.TorqueBias == 0) frontFinalTorque = 0;
                else if (Car.Transmission.TorqueBias == 1) rearFinalTorque = 0;
                else if (biasRatio > Car.Transmission.TorqueBias) frontFinalTorque = rearFinalTorque * Car.Transmission.TorqueBias / (1 - Car.Transmission.TorqueBias);
                else if (biasRatio < Car.Transmission.TorqueBias) rearFinalTorque = frontFinalTorque * (1 - Car.Transmission.TorqueBias) / Car.Transmission.TorqueBias;
            }
            return new double[] { frontFinalTorque, rearFinalTorque };
        }
        #endregion
        #endregion
        */
    }
}
