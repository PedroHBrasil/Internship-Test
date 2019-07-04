using GeneticSharp.Domain;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Terminations;
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
        }
        #endregion
        #region Enums
        private enum CorneringDirection { Left, Right }
        #endregion
        #region Fields
        private DynamicStateParameters pureBrakingParameters;
        private DynamicStateParameters pureAcceleratingParameters;
        private DynamicStateParameters pureLeftCorneringParameters;
        private DynamicStateParameters pureRightCorneringParameters;
        private double currentSteeringWheelAngle;
        private double currentFrontWheelSteeringAngle;
        private double currentRearWheelSteeringAngle;
        private double currentLateralAcceleration;
        private double currentYawMoment;
        private double currentCarSlipAngle;
        private bool isLateralAccelerationPositive;

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

        public TwoWheelGGDiagram(double speed, Vehicle.TwoWheelCar car, int amountOfPoints)
        {
            Speed = speed;
            Car = car;
            AmountOfPoints = amountOfPoints;

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
                // Left
            isLateralAccelerationPositive = false;
            _GetCorneringAccelerationSteadyState();
            pureLeftCorneringParameters.longitudinalAcceleration = _GetLongitudinalAccelerationForPureCorneringSteadyState(pureLeftCorneringParameters.carSlipAngle, pureLeftCorneringParameters.lateralAcceleration, pureLeftCorneringParameters.steeringWheelAngle);
                // Right
            isLateralAccelerationPositive = true;
            _GetCorneringAccelerationSteadyState();
            pureRightCorneringParameters.longitudinalAcceleration = _GetLongitudinalAccelerationForPureCorneringSteadyState(pureRightCorneringParameters.carSlipAngle, pureRightCorneringParameters.lateralAcceleration, pureRightCorneringParameters.steeringWheelAngle);
            // Combined accelerations
            _GetCombinedOperationAccelerations();
        }
        #region Accelerations Determination By Use Of Genetic Algorithm

        #endregion
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
                double referenceWheelSpeedRatio = wheelsAngularSpeeds[1] * Car.Transmission.TorqueBias / (wheelsAngularSpeeds[0] * Car.Transmission.TorqueBias + wheelsAngularSpeeds[1] * (1 - Car.Transmission.TorqueBias));
                double referenceWheelAngularSpeed = wheelsAngularSpeeds[0] + referenceWheelSpeedRatio * (wheelsAngularSpeeds[1] - wheelsAngularSpeeds[0]);
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
                Vehicle.TwoWheelAerodynamicMapPoint currentAerodynamicParameters = Car.GetAerodynamicCoefficients(Speed, longitudinalAcceleration);
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
                double frontLongitudinalSlipForMaximumBrake = Car.FrontTire.TireModel.GetLongitudinalSlipForMinimumTireFx(0, wheelsLoads[0] / 2, 0, Speed);
                double rearLongitudinalSlipForMaximumBrake = Car.RearTire.TireModel.GetLongitudinalSlipForMinimumTireFx(0, wheelsLoads[1] / 2, 0, Speed);
                // Maximum braking wheels torques according to tire grip [Nm]
                double frontGripTorque = Car.FrontTire.TireModel.GetTireFx(frontLongitudinalSlipForMaximumBrake, 0, wheelsLoads[0] / 2, 0, Speed) * wheelsRadiuses[0] * 2;
                double rearGripTorque = Car.RearTire.TireModel.GetTireFx(rearLongitudinalSlipForMaximumBrake, 0, wheelsLoads[1] / 2, 0, Speed) * wheelsRadiuses[1] * 2;
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
                double referenceWheelSpeedRatio = wheelsAngularSpeeds[1] * Car.Transmission.TorqueBias / (wheelsAngularSpeeds[0] * Car.Transmission.TorqueBias + wheelsAngularSpeeds[1] * (1 - Car.Transmission.TorqueBias));
                double referenceWheelAngularSpeed = wheelsAngularSpeeds[0] + referenceWheelSpeedRatio * (wheelsAngularSpeeds[1] - wheelsAngularSpeeds[0]);
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
                Vehicle.TwoWheelAerodynamicMapPoint currentAerodynamicParameters = Car.GetAerodynamicCoefficients(Speed, longitudinalAcceleration);
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
                double frontGripTorque = Car.FrontTire.TireModel.GetTireFx(frontLongitudinalSlipForMaximumAccelerating, 0, wheelsLoads[0] / 2, 0, Speed) * wheelsRadiuses[0] * 2;
                double rearGripTorque = Car.RearTire.TireModel.GetTireFx(rearLongitudinalSlipForMaximumAccelerating, 0, wheelsLoads[1] / 2, 0, Speed) * wheelsRadiuses[1] * 2;
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
        #region Steady-State Trials
        /// <summary>
        /// Gets the maximum/minimum lateral acceleration in pure slip steady state
        /// </summary>
        private void _GetCorneringAccelerationSteadyState()
        {
            // Optimization parameters
            double epsg = 1e-6;
            double epsf = 1e-4;
            double epsx = 0;
            double diffstep = 1.0e-4;
            int maxits = 100;
            double[] bndl;
            double[] bndu;
            double[] steeringWheelAngle;
            if (isLateralAccelerationPositive)
            {
                bndl = new double[] { 0 };
                bndu = new double[] { Car.Steering.MaximumSteeringWheelAngle };
                steeringWheelAngle = new double[] { -pureLeftCorneringParameters.steeringWheelAngle };
            }
            else
            {
                bndl = new double[] { -Car.Steering.MaximumSteeringWheelAngle };
                bndu = new double[] { 0 };
                steeringWheelAngle = new double[] { -Car.Steering.MaximumSteeringWheelAngle / 2 };
            }
            // Optimization setup
            alglib.minbleiccreatef(steeringWheelAngle, diffstep, out alglib.minbleicstate state);
            alglib.minbleicsetbc(state, bndl, bndu);
            alglib.minbleicsetcond(state, epsg, epsf, epsx, maxits);
            // Optimization execution
            alglib.minbleicoptimize(state, _SteeringWheelAngleOptimizationForCorneringSteadyState, null, null);
            alglib.minbleicresults(state, out steeringWheelAngle, out alglib.minbleicreport rep);

            if (isLateralAccelerationPositive)
            {
                pureRightCorneringParameters.steeringWheelAngle = steeringWheelAngle[0];
                pureRightCorneringParameters.carSlipAngle = currentCarSlipAngle;
                pureRightCorneringParameters.frontWheelParameters.steeringAngle = currentFrontWheelSteeringAngle;
                pureRightCorneringParameters.rearWheelParameters.steeringAngle = currentRearWheelSteeringAngle;
            }
            else
            {
                pureLeftCorneringParameters.steeringWheelAngle = steeringWheelAngle[0];
                pureLeftCorneringParameters.carSlipAngle = currentCarSlipAngle;
                pureLeftCorneringParameters.frontWheelParameters.steeringAngle = currentFrontWheelSteeringAngle;
                pureLeftCorneringParameters.rearWheelParameters.steeringAngle = currentRearWheelSteeringAngle;
            }
            //_UpdateCarSlipAngleVsYawMomentArrays();
        }
        /// <summary>
        /// Method for optimization of the steering wheel angle for maximum/minimum lateral acceleration in pure slip steady state
        /// </summary>
        /// <param name="steeringWheelAngle"></param>
        /// <param name="lateralAcceleration"></param>
        /// <param name="obj"></param>
        private void _SteeringWheelAngleOptimizationForCorneringSteadyState(double[] steeringWheelAngle, ref double lateralAcceleration, object obj)
        {
            currentSteeringWheelAngle = steeringWheelAngle[0];
            if (isLateralAccelerationPositive)
            {
                pureRightCorneringParameters.lateralAcceleration = _GetLateralAccelerationForSteeringAngleSteadyState(steeringWheelAngle[0]);
                lateralAcceleration = -pureRightCorneringParameters.lateralAcceleration;
            }
            else
            {
                pureLeftCorneringParameters.lateralAcceleration = _GetLateralAccelerationForSteeringAngleSteadyState(steeringWheelAngle[0]);
                lateralAcceleration = pureLeftCorneringParameters.lateralAcceleration;
            }
        }
        /// <summary>
        /// Gets the lateral acceleration for a given steering wheel angle in steady state
        /// </summary>
        /// <param name="steeringAngle"></param>
        /// <returns></returns>
        private double _GetLateralAccelerationForSteeringAngleSteadyState(double steeringAngle)
        {
            // Wheels steering angles [rad]
            currentFrontWheelSteeringAngle = steeringAngle * Car.Steering.FrontSteeringRatio;
            currentRearWheelSteeringAngle = steeringAngle * Car.Steering.RearSteeringRatio;
            // Current lateral acceleration optimization.
            double[] lateralAccelerationForOptimization = new double[] { 0 };
            double epsg = 1e-6;
            double epsf = 1e-3;
            double epsx = 0;
            double diffstep = 1.0e-3;
            int maxits = 100;

            alglib.minlbfgscreatef(1, lateralAccelerationForOptimization, diffstep, out alglib.minlbfgsstate state);
            alglib.minlbfgssetcond(state, epsg, epsf, epsx, maxits);
            alglib.minlbfgsoptimize(state, _LateralAccelerationForGivenSteeringAngleSteadyStateOptimization, null, null);
            alglib.minlbfgsresults(state, out lateralAccelerationForOptimization, out alglib.minlbfgsreport rep);

            return lateralAccelerationForOptimization[0];
        }
        /// <summary>
        /// Methodsfor optimization of the lateral acceleration in steady state
        /// </summary>
        /// <param name="lateralAccelerationGuess"></param>
        /// <param name="lateralAccelerationError"></param>
        /// <param name="obj"></param>
        private void _LateralAccelerationForGivenSteeringAngleSteadyStateOptimization(double[] lateralAccelerationGuess, ref double lateralAccelerationError, object obj)
        {
            currentLateralAcceleration = lateralAccelerationGuess[0];
            // Lateral acceleration update [m/s²]
            double lateralAcceleration = _GetLateralAccelerationSteadyState(currentLateralAcceleration, currentFrontWheelSteeringAngle, currentRearWheelSteeringAngle);
            // Error evaluation [m/s²]
            lateralAccelerationError = Math.Abs(currentLateralAcceleration - lateralAcceleration);
        }
        /// <summary>
        /// Gets the lateral acceleration in steady state
        /// </summary>
        /// <param name="lateralAccelerationGuess"></param>
        /// <param name="frontWheelSteeringAngle"></param>
        /// <param name="rearWheelSteeringAngle"></param>
        /// <returns></returns>
        private double _GetLateralAccelerationSteadyState(double lateralAccelerationGuess, double frontWheelSteeringAngle, double rearWheelSteeringAngle)
        {
            // Get current Car Slip Angle for zero yaw moment
            currentCarSlipAngle = _GetCarSlipAngleForZeroYawMoment(-Math.PI / 3, Math.PI / 3, 50);
            // Wheels slip angles [rad]
            double[] wheelsSlipAngles = _GetWheelsSlipAngles(currentCarSlipAngle, lateralAccelerationGuess, frontWheelSteeringAngle, rearWheelSteeringAngle);
            // Wheels loads [N]
            double[] wheelsLoads = Car.GetWheelsLoads(Speed, currentCarSlipAngle, 0);
            // Wheels longitudinal forces [N]
            double frontWheelLateralForce = 2 * Car.FrontTire.TireModel.GetTireFy(0, wheelsSlipAngles[0], wheelsLoads[0] / 2, 0, Speed) * Math.Cos(frontWheelSteeringAngle);
            double rearWheelLateralForce = 2 * Car.RearTire.TireModel.GetTireFy(0, wheelsSlipAngles[1], wheelsLoads[1] / 2, 0, Speed) * Math.Cos(rearWheelSteeringAngle);
            // Lateral acceleration [m/s²]
            double lateralAcceleration = (frontWheelLateralForce + rearWheelLateralForce) / Car.InertiaAndDimensions.TotalMass;

            return lateralAcceleration;
        }
        /// <summary>
        /// Gets the car slip angle which gives zero yaw moment
        /// </summary>
        /// <param name="lowerBoundary"></param>
        /// <param name="upperBoundary"></param>
        /// <param name="amountOfSearchIntervals"></param>
        /// <returns></returns>
        private double _GetCarSlipAngleForZeroYawMoment(double lowerBoundary, double upperBoundary, int amountOfSearchIntervals)
        {
            // Inner boundaries array generation
            double[] innerBoundaries = Generate.LinearSpaced(amountOfSearchIntervals, lowerBoundary, upperBoundary);
            // Reference yaw moment [Nm] (Lower boundary)
            double referenceYawMoment = _GetYawMoment(innerBoundaries[0], currentLateralAcceleration, currentSteeringWheelAngle);
            // Inner boundary determination loop
            bool isSignEqual = true;
            int iBoundary = 0;
            do
            {
                iBoundary++;
                // Gets the yaw moment for the current boundary index
                double currentYawMoment = _GetYawMoment(innerBoundaries[iBoundary], currentLateralAcceleration, currentSteeringWheelAngle);
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
        /// Method used for gradient based optimization of the car slip angle for zero yaw moment
        /// </summary>
        /// <param name="carSlipAngleOptimization"></param>
        /// <param name="yawMomentSquared"></param>
        /// <param name="obj"></param>
        private void _CarSlipAngleOptimization(double[] carSlipAngleOptimization, ref double yawMomentSquared, object obj)
        {
            currentYawMoment = _GetYawMoment(carSlipAngleOptimization[0], currentLateralAcceleration, currentSteeringWheelAngle);
            yawMomentSquared = Math.Pow(currentYawMoment, 2);
        }
        /// <summary>
        /// Gets the longitudinal acceleration associated with the pure slip cornering situation
        /// </summary>
        /// <param name="carSlipAngle"></param>
        /// <param name="lateralAcceleration"></param>
        /// <param name="steeringWheelAngle"></param>
        /// <returns></returns>
        private double _GetLongitudinalAccelerationForPureCorneringSteadyState(double carSlipAngle, double lateralAcceleration, double steeringWheelAngle)
        {
            // Wheels steering angles [rad]
            double frontWheelSteeringAngle = steeringWheelAngle * Car.Steering.FrontSteeringRatio;
            double rearWheelSteeringAngle = steeringWheelAngle * Car.Steering.RearSteeringRatio;
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
            Vehicle.TwoWheelAerodynamicMapPoint currentAerodynamicParameters = Car.GetAerodynamicCoefficients(Speed, 0);
            double dragForce = -currentAerodynamicParameters.DragCoefficient * Car.Aerodynamics.FrontalArea * Car.Aerodynamics.AirDensity * Math.Pow(Speed, 2) / 2;
            // Vehicle accelerations [m/s²]
            double longitudinalAcceleration = (frontWheelLongitudinalForce + rearWheelLongitudinalForce + dragForce) * inertiaEfficiency / Car.InertiaAndDimensions.TotalMass;

            return longitudinalAcceleration;
        }
        #endregion
        private double _GetLateralAccelerationForSteeringAngleAndCarSlipAngle(double steeringAngle, double carSlipAngle)
        {
            // Current car slip angle [rad]
            currentCarSlipAngle = carSlipAngle;
            // Wheels steering angles [rad]
            currentFrontWheelSteeringAngle = steeringAngle * Car.Steering.FrontSteeringRatio;
            currentRearWheelSteeringAngle = steeringAngle * Car.Steering.RearSteeringRatio;
            // Current lateral acceleration optimization.
            double[] lateralAccelerationForOptimization = new double[] { 0 };
            double epsg = 1e-6;
            double epsf = 1e-3;
            double epsx = 0;
            double diffstep = 1.0e-3;
            int maxits = 100;

            alglib.minlbfgscreatef(1, lateralAccelerationForOptimization, diffstep, out alglib.minlbfgsstate state);
            alglib.minlbfgssetcond(state, epsg, epsf, epsx, maxits);
            alglib.minlbfgsoptimize(state, _LateralAccelerationForGivenSteeringAngleAndCarSlipAngleOptimization, null, null);
            alglib.minlbfgsresults(state, out lateralAccelerationForOptimization, out alglib.minlbfgsreport rep);

            return lateralAccelerationForOptimization[0];
        }
        /// <summary>
        /// Method to find the lateral acceleration associated with the given steering wheel angle.
        /// </summary>
        /// <param name="lateralAccelerationGuess"> Optimization parameter: lateral acceleration guess [m/s²]. </param>
        /// <param name="lateralAccelerationError"> Optimization target: difference between the guess and the calculated lateral acceleration [m/s²]. </param>
        /// <param name="obj"></param>
        private void _LateralAccelerationForGivenSteeringAngleAndCarSlipAngleOptimization(double[] lateralAccelerationGuess, ref double lateralAccelerationError, object obj)
        {
            currentLateralAcceleration = lateralAccelerationGuess[0];
            // Lateral acceleration update [m/s²]
            double lateralAcceleration = _GetLateralAcceleration(currentCarSlipAngle, currentLateralAcceleration, currentFrontWheelSteeringAngle, currentRearWheelSteeringAngle);
            // Error evaluation [m/s²]
            lateralAccelerationError = Math.Abs(currentLateralAcceleration - lateralAcceleration);
        }
        /// <summary>
        /// Gets the car's yaw moment.
        /// </summary>
        /// <param name="carSlipAngle"> Car's slip angle [rad]. </param>
        /// <param name="lateralAcceleration"> Car's lateral acceleration [m/s²] </param>
        /// <param name="frontWheelSteeringAngle"> Front wheel steering angle [rad] </param>
        /// <param name="rearWheelSteeringAngle"> Rear wheel steering angle [rad] </param>
        /// <returns> Yaw moment [Nm] </returns>
        private double _GetYawMoment(double carSlipAngle, double lateralAcceleration, double steeringAngle)
        {
            // Wheels steering angles [rad]
            double frontWheelSteeringAngle = steeringAngle * Car.Steering.FrontSteeringRatio;
            double rearWheelSteeringAngle = steeringAngle * Car.Steering.RearSteeringRatio;
            // Wheels loads [N]
            double[] wheelsLoads = Car.GetWheelsLoads(Speed, carSlipAngle, 0);
            // Wheels slip angles [rad]
            double[] wheelsSlipAngles = _GetWheelsSlipAngles(carSlipAngle, lateralAcceleration, frontWheelSteeringAngle, rearWheelSteeringAngle);
            // Front and rear lateral forces [N]
            double frontWheelLateralForce = 2 * Car.FrontTire.TireModel.GetTireFy(0, wheelsSlipAngles[0], wheelsLoads[0], 0, Speed) * Math.Cos(currentFrontWheelSteeringAngle);
            double rearWheelLateralForce = 2 * Car.RearTire.TireModel.GetTireFy(0, wheelsSlipAngles[1], wheelsLoads[1], 0, Speed) * Math.Cos(currentRearWheelSteeringAngle);
            // Yaw moment (squared) [Nm]
            currentYawMoment = frontWheelLateralForce * Car.InertiaAndDimensions.DistanceBetweenFrontAxisAndCG - rearWheelLateralForce * Car.InertiaAndDimensions.DistanceBetweenRearAxisAndCG;
            return currentYawMoment;
        }
        private void _UpdateCarSlipAngleVsYawMomentArrays()
        {
            int amountOfPoints = 500;
            testCarSlipAngle = Generate.LinearSpaced(amountOfPoints, -Math.PI/3, Math.PI/3);
            testYawMoment = new double[amountOfPoints];
            for (int i = 0; i < amountOfPoints; i++)
            {
                // Input separation
                double carSlipAngle = testCarSlipAngle[i];
                // Yaw moment (squared) [Nm]
                testYawMoment[i] = _GetYawMoment(carSlipAngle, pureLeftCorneringParameters.lateralAcceleration, pureLeftCorneringParameters.steeringWheelAngle);
            }
        }
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
            double frontWheelLateralForce = 2 * Car.FrontTire.TireModel.GetTireFy(0, wheelsSlipAngles[0], wheelsLoads[0] / 2, 0, Speed) * Math.Cos(frontWheelSteeringAngle);
            double rearWheelLateralForce = 2 * Car.RearTire.TireModel.GetTireFy(0, wheelsSlipAngles[1], wheelsLoads[1] / 2, 0, Speed) * Math.Cos(rearWheelSteeringAngle);
            // Lateral acceleration [m/s²]
            double lateralAcceleration = (frontWheelLateralForce + rearWheelLateralForce) / Car.InertiaAndDimensions.TotalMass;

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
        #endregion
        #region Ellipsoid Interpolation
        /// <summary>
        /// Gets the combined operation accelerations by "ellipsoid interpolation".
        /// </summary>
        private void _GetCombinedOperationAccelerations()
        {
            // Amount of directions
            AmountOfDirections = AmountOfPoints * 4;
            // Directions array [rad]
            double[] directions = Generate.LinearSpaced(AmountOfDirections, -Math.PI, Math.PI * (AmountOfDirections - 2) / AmountOfDirections);
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
        #endregion
    }
}
