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
        private double currentFrontWheelSteeringAngle;
        private double currentRearWheelSteeringAngle;
        private double currentLateralAcceleration;
        private double currentYawMoment;
        private double currentCarSlipAngle;
        /*
        private double[] testCarSlipAngle;
        private double[] testYawMoment;
        */
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
            int[] amountOfPointsPerOptimizationParameter = new int[] { 30, 30 };
            double[] steeringAngleLimits = new double[] { -Car.Steering.MaximumSteeringWheelAngle, 0 };
            double[] carSlipAngleLimits = new double[] { -Math.PI / 3, Math.PI / 3 };
            _GetPureCorneringAccelerationByGeneticAlgorithm(-1, amountOfPointsPerOptimizationParameter, steeringAngleLimits, carSlipAngleLimits);
            pureLeftCorneringParameters.steeringWheelAngle = -Car.Steering.MaximumSteeringWheelAngle / 2;
            pureLeftCorneringParameters.carSlipAngle = 0;
            _GetMinimumCorneringAcceleration();
            pureLeftCorneringParameters.longitudinalAcceleration = _GetLongitudinalAccelerationForPureCornering(pureLeftCorneringParameters.carSlipAngle, pureLeftCorneringParameters.lateralAcceleration, pureLeftCorneringParameters.frontWheelParameters.steeringAngle, pureLeftCorneringParameters.rearWheelParameters.steeringAngle);
            _GetMaximumCorneringAcceleration();
            pureRightCorneringParameters.longitudinalAcceleration = _GetLongitudinalAccelerationForPureCornering(pureRightCorneringParameters.carSlipAngle, pureRightCorneringParameters.lateralAcceleration, pureRightCorneringParameters.frontWheelParameters.steeringAngle, pureRightCorneringParameters.rearWheelParameters.steeringAngle);
            if (pureLeftCorneringParameters.lateralAcceleration>-pureRightCorneringParameters.lateralAcceleration*.5)
            {
                pureLeftCorneringParameters.steeringWheelAngle = -pureRightCorneringParameters.steeringWheelAngle;
                pureLeftCorneringParameters.carSlipAngle = -pureRightCorneringParameters.carSlipAngle;
                _GetMinimumCorneringAcceleration();
                pureLeftCorneringParameters.longitudinalAcceleration = _GetLongitudinalAccelerationForPureCornering(pureLeftCorneringParameters.carSlipAngle, pureLeftCorneringParameters.lateralAcceleration, pureLeftCorneringParameters.frontWheelParameters.steeringAngle, pureLeftCorneringParameters.rearWheelParameters.steeringAngle);
            }
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
        #region Initial Guess by Genetic Algorithm
        /// <summary>
        /// Gets the initial guess of the cornering situation by use of genetic algorithm.
        /// </summary>
        /// <param name="lateralAccelerationSign"> Corner orientation: -1 is left and 1 is right. </param>
        /// <param name="amountOfPointsPerOptimizationParameter"> Contains the amount of points for each optimization parameter. </param>
        /// <param name="steeringAngleLimits"> Limits of the steering wheel angle possibilities [rad]. </param>
        /// <param name="carSlipAngleLimits"> Limits of the car slip angle possibilities [rad]. </param>
        private void _GetPureCorneringAccelerationByGeneticAlgorithm(int lateralAccelerationSign, int[] amountOfPointsPerOptimizationParameter, double[] steeringAngleLimits, double[] carSlipAngleLimits)
        {
            // Optimization parameters arrays
            double[] steeringAngles = Generate.LinearSpaced(amountOfPointsPerOptimizationParameter[0], steeringAngleLimits[0], steeringAngleLimits[1]);
            double[] carSlipAngles = Generate.LinearSpaced(amountOfPointsPerOptimizationParameter[1], carSlipAngleLimits[0], carSlipAngleLimits[1]);
            // Genetic algorithm setup
            var chromossome = new FloatingPointChromosome(
                new double[] { 0, 0 },
                new double[] { amountOfPointsPerOptimizationParameter[0] - 1, amountOfPointsPerOptimizationParameter[1] - 1 },
                new int[] { 5, 5 },
                new int[] { 0, 0 });
            var population = new Population(10, 20, chromossome);
            var fitness = new FuncFitness((c) =>
            {
                var fc = c as FloatingPointChromosome;
                var values = fc.ToFloatingPoints();
                var steeringWheelAngle = steeringAngles[Convert.ToInt32(values[0])];
                var carSlipAngle = carSlipAngles[Convert.ToInt32(values[1])];
                // Evaluates the fitness based on: calculated lateral acceleration, the lateral acceleration difference and the yaw moment.
                return _GetOptimizationReferenceParametersValues(lateralAccelerationSign, steeringWheelAngle, carSlipAngle);
            });
            var selection = new EliteSelection();
            var crossover = new UniformCrossover();
            var mutation = new FlipBitMutation();
            var termination = new FitnessStagnationTermination(20);
            var ga = new GeneticAlgorithm(population, fitness, selection, crossover, mutation);
            ga.Termination = termination;
            // Optimization tracking
            System.Diagnostics.Debug.WriteLine("Generation: Steering Angle - Car Slip Angle - Lat. Acceleration => fitness (Speed = " + Speed.ToString("F2") + " Corner Orientation: " + lateralAccelerationSign.ToString("F0") + ")");
            var latestFitness = 0.0;
            ga.GenerationRan += (sender, e) =>
            {
                var bestChromosome = ga.BestChromosome as FloatingPointChromosome;
                var bestFitness = bestChromosome.Fitness.Value;
                if (latestFitness != bestFitness)
                {
                    latestFitness = bestFitness;
                    var phenotype = bestChromosome.ToFloatingPoints();
                    System.Diagnostics.Debug.WriteLine(
                        (ga.GenerationsNumber.ToString(),
                            steeringAngles[Convert.ToInt32(phenotype[0])].ToString("F4"),
                            carSlipAngles[Convert.ToInt32(phenotype[1])].ToString("F4"),
                            latestFitness.ToString("F6")));
                };
            };
            // Genetic algorithm optimization execution
            ga.Start();
            var result = ((FloatingPointChromosome)ga.BestChromosome).ToFloatingPoints();

            if (lateralAccelerationSign == -1)
            {
                pureLeftCorneringParameters.steeringWheelAngle = steeringAngles[Convert.ToInt32(result[0])];
                pureLeftCorneringParameters.carSlipAngle = carSlipAngles[Convert.ToInt32(result[1])];
                pureLeftCorneringParameters.frontWheelParameters.steeringAngle = pureLeftCorneringParameters.steeringWheelAngle * Car.Steering.FrontSteeringRatio;
                pureLeftCorneringParameters.rearWheelParameters.steeringAngle = pureLeftCorneringParameters.steeringWheelAngle * Car.Steering.RearSteeringRatio;
                pureLeftCorneringParameters.yawMoment = _GetYawMoment(pureLeftCorneringParameters.carSlipAngle, pureLeftCorneringParameters.lateralAcceleration, pureLeftCorneringParameters.frontWheelParameters.steeringAngle, pureLeftCorneringParameters.rearWheelParameters.steeringAngle);
                currentFrontWheelSteeringAngle = pureLeftCorneringParameters.frontWheelParameters.steeringAngle;
                currentRearWheelSteeringAngle = pureLeftCorneringParameters.rearWheelParameters.steeringAngle;
                _GetOptimizationReferenceParametersValues(lateralAccelerationSign, pureLeftCorneringParameters.steeringWheelAngle, pureLeftCorneringParameters.carSlipAngle);
            }
            else if (lateralAccelerationSign == 1)
            {
                pureRightCorneringParameters.steeringWheelAngle = steeringAngles[Convert.ToInt32(result[0])];
                pureRightCorneringParameters.carSlipAngle = carSlipAngles[Convert.ToInt32(result[1])];
                pureRightCorneringParameters.frontWheelParameters.steeringAngle = pureLeftCorneringParameters.steeringWheelAngle * Car.Steering.FrontSteeringRatio;
                pureRightCorneringParameters.rearWheelParameters.steeringAngle = pureLeftCorneringParameters.steeringWheelAngle * Car.Steering.RearSteeringRatio;
                pureRightCorneringParameters.yawMoment = _GetYawMoment(pureRightCorneringParameters.carSlipAngle, pureRightCorneringParameters.lateralAcceleration, pureRightCorneringParameters.frontWheelParameters.steeringAngle, pureRightCorneringParameters.rearWheelParameters.steeringAngle);
                currentFrontWheelSteeringAngle = pureRightCorneringParameters.frontWheelParameters.steeringAngle;
                currentRearWheelSteeringAngle = pureRightCorneringParameters.rearWheelParameters.steeringAngle;
                _GetOptimizationReferenceParametersValues(lateralAccelerationSign, pureRightCorneringParameters.steeringWheelAngle, pureRightCorneringParameters.carSlipAngle);
            }
        }
        /// <summary>
        /// Gets the fitness based on thelateral acceleration value.
        /// </summary>
        /// <param name="lateralAccelerationSign"> Corner orientation: -1 is left and 1 is right. </param>
        /// <param name="steeringWheelAngle"> Current steering wheel angle [rad]. </param>
        /// <param name="carSlipAngle"> Current car slip angle [rad]. </param>
        /// <returns> Fitness score based on the lateral acceleration. </returns>
        private double _GetOptimizationReferenceParametersValues(int lateralAccelerationSign, double steeringWheelAngle, double carSlipAngle)
        {
            double lateralAcceleration = _GetLateralAccelerationForSteeringAngleAndCarSlipAngle(steeringWheelAngle, carSlipAngle);
            currentLateralAcceleration = lateralAcceleration;
            // Evaluates the fitness
            double lateralAccelerationFitness;
            if (Math.Sign(lateralAcceleration) != lateralAccelerationSign || lateralAcceleration == 0) lateralAccelerationFitness = 0;
            else lateralAccelerationFitness = 1 - 1 / Math.Abs(lateralAcceleration);

            return lateralAccelerationFitness;
        }
        #endregion
        #region Left (Negative)
        /// <summary>
        /// Gets the car's minimum cornering acceleration by optimization.
        /// </summary>
        /// <returns> Minimum cornering lateral acceleration [m/s²] </returns>
        private void _GetMinimumCorneringAcceleration()
        {
            // Optimization parameters
            double epsg = 1e-6;
            double epsf = 1e-4;
            double epsx = 0;
            double diffstep = 1.0e-4;
            int maxits = 100;
            double[] bndl = new double[] { -Car.Steering.MaximumSteeringWheelAngle, -Math.PI / 3 };
            double[] bndu = new double[] { 0, Math.PI / 3 };
            double[] deltaAndCarSlipAngle = new double[] { pureLeftCorneringParameters.steeringWheelAngle, pureLeftCorneringParameters.carSlipAngle };
            // Optimization setup
            alglib.minbleiccreatef(deltaAndCarSlipAngle, diffstep, out alglib.minbleicstate state);
            alglib.minbleicsetbc(state, bndl, bndu);
            alglib.minbleicsetcond(state, epsg, epsf, epsx, maxits);
            // Optimization execution
            alglib.minbleicoptimize(state, _SteeringWheelAngleOptimizationForMinimumCornering, null, null);
            alglib.minbleicresults(state, out deltaAndCarSlipAngle, out alglib.minbleicreport rep);

            pureLeftCorneringParameters.steeringWheelAngle = deltaAndCarSlipAngle[0];
            pureLeftCorneringParameters.carSlipAngle = deltaAndCarSlipAngle[1];
            pureLeftCorneringParameters.frontWheelParameters.steeringAngle = currentFrontWheelSteeringAngle;
            pureLeftCorneringParameters.rearWheelParameters.steeringAngle = currentRearWheelSteeringAngle;
        }
        /// <summary>
        /// Method for optimization of the steering wheel angle for minimum lateral acceleration.
        /// </summary>
        /// <param name="deltaAndCarSlipAngle"> Steering wheel angle [rad] </param>
        /// <param name="lateralAcceleration"> Lateral acceleration [m/s²] </param>
        /// <param name="obj"></param>
        private void _SteeringWheelAngleOptimizationForMinimumCornering(double[] deltaAndCarSlipAngle, ref double lateralAcceleration, object obj)
        {
            pureLeftCorneringParameters.lateralAcceleration = _GetLateralAccelerationForSteeringAngleAndCarSlipAngle(deltaAndCarSlipAngle[0], deltaAndCarSlipAngle[1]);
            lateralAcceleration = pureLeftCorneringParameters.lateralAcceleration;
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
            double epsf = 1e-4;
            double epsx = 0;
            double diffstep = 1.0e-4;
            int maxits = 100;

            double[] bndl = new double[] { 0, -Math.PI / 3 };
            double[] bndu = new double[] { Car.Steering.MaximumSteeringWheelAngle, Math.PI / 3 };

            double[] deltaAndCarSlipAngle = new double[] { -pureLeftCorneringParameters.steeringWheelAngle, -pureLeftCorneringParameters.carSlipAngle };

            alglib.minbleiccreatef(deltaAndCarSlipAngle, diffstep, out alglib.minbleicstate state);
            alglib.minbleicsetbc(state, bndl, bndu);
            alglib.minbleicsetcond(state, epsg, epsf, epsx, maxits);
            alglib.minbleicoptimize(state, _SteeringWheelAngleOptimizationForMaximumCornering, null, null);
            alglib.minbleicresults(state, out deltaAndCarSlipAngle, out alglib.minbleicreport rep);

            pureRightCorneringParameters.steeringWheelAngle = deltaAndCarSlipAngle[0];
            pureRightCorneringParameters.carSlipAngle = deltaAndCarSlipAngle[1];
            pureRightCorneringParameters.frontWheelParameters.steeringAngle = currentFrontWheelSteeringAngle;
            pureRightCorneringParameters.rearWheelParameters.steeringAngle = currentRearWheelSteeringAngle;
        }
        /// <summary>
        /// Method for optimization of the steering wheel angle for maximum lateral acceleration.
        /// </summary>
        /// <param name="deltaAndCarSlipAngle"> Steering wheel angle [rad] </param>
        /// <param name="lateralAcceleration"> Lateral acceleration [m/s²] </param>
        /// <param name="obj"></param>
        private void _SteeringWheelAngleOptimizationForMaximumCornering(double[] deltaAndCarSlipAngle, ref double lateralAcceleration, object obj)
        {
            // Current car slip angle [rad]
            currentCarSlipAngle = deltaAndCarSlipAngle[1];
            // Wheels steering angles
            currentFrontWheelSteeringAngle = deltaAndCarSlipAngle[0] * Car.Steering.FrontSteeringRatio;
            currentRearWheelSteeringAngle = deltaAndCarSlipAngle[0] * Car.Steering.RearSteeringRatio;
            // Current lateral acceleration optimization.
            double[] lateralAccelerationForOptimization = new double[] { 0 };
            double epsg = 1e-6;
            double epsf = 0;
            double epsx = 0;
            double diffstep = 1.0e-2;
            int maxits = 100;

            alglib.minlbfgscreatef(1, lateralAccelerationForOptimization, diffstep, out alglib.minlbfgsstate state);
            alglib.minlbfgssetcond(state, epsg, epsf, epsx, maxits);
            alglib.minlbfgsoptimize(state, _LateralAccelerationForGivenSteeringAngleAndCarSlipAngleOptimization, null, null);
            alglib.minlbfgsresults(state, out lateralAccelerationForOptimization, out alglib.minlbfgsreport rep);

            pureRightCorneringParameters.lateralAcceleration = _GetLateralAccelerationForSteeringAngleAndCarSlipAngle(deltaAndCarSlipAngle[0], deltaAndCarSlipAngle[1]);

            lateralAcceleration = -pureRightCorneringParameters.lateralAcceleration;
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
        private double _GetYawMoment(double carSlipAngle, double lateralAcceleration, double frontWheelSteeringAngle, double rearWheelSteeringAngle)
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
        /*
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
        }*/
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
        #endregion
    }
}
