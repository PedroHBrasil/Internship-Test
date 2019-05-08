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
        #region Fields
        private double currentPedalsActuation;
        private double currentFrontWheelSteeringAngle;
        private double currentRearWheelSteeringAngle;
        private double currentFrontWheelRadius;
        private double currentRearWheelRadius;
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
        /// <summary>
        /// Generates the GG Diagram.
        /// </summary>
        public void GenerateGGDiagram()
        {
            // Steering wheel angle and pedals actuation vectors
            double[] pedalActuations = Generate.LinearSpaced(AmountOfPoints * 2, -1, 1);
            double[] steeringWheelAngles = Generate.LinearSpaced(AmountOfPoints, 0, Car.Steering.MaximumSteeringWheelAngle);
            for (int iPedalActuation = 0; iPedalActuation < pedalActuations.Length; iPedalActuation++)
            {
                for (int iSteeringWheelAngle = 0; iSteeringWheelAngle < steeringWheelAngles.Length; iSteeringWheelAngle++)
                {
                    double[] accelerationsAndCarSlipAngle = _GetAccelerationsWithMultiVariableOptimization(steeringWheelAngles[iSteeringWheelAngle], pedalActuations[iPedalActuation]);
                    LongitudinalAccelerations.Add(accelerationsAndCarSlipAngle[0]);
                    LateralAccelerations.Add(accelerationsAndCarSlipAngle[1]);
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
            double[] inputScales = new double[] { 1, 1, 1 };
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
                frontKappas = Generate.LinearSpaced(AmountOfPoints, Car.FrontTire.TireModel.KappaMin, 0);
                rearKappas = Generate.LinearSpaced(AmountOfPoints, Car.RearTire.TireModel.KappaMin, 0);
            }
            else
            {
                frontKappas = Generate.LinearSpaced(AmountOfPoints, 0, Car.FrontTire.TireModel.KappaMax);
                rearKappas = Generate.LinearSpaced(AmountOfPoints, 0, Car.RearTire.TireModel.KappaMax);
            }
            double frontWheelLongitudinalSlip = Car.FrontTire.TireModel.GetLongitudinalSlipForGivenLongitudinalForce(frontWheelLongitudinalForce / 2, frontKappas, frontWheelSlipAngle, frontWheelVerticalLoad, 0, Speed);
            double rearWheelLongitudinalSlip = Car.RearTire.TireModel.GetLongitudinalSlipForGivenLongitudinalForce(rearWheelLongitudinalForce / 2, rearKappas, rearWheelSlipAngle, rearWheelVerticalLoad, 0, Speed);
            // Wheels longitudinal (correction based on longitudinl slip) and lateral forces [N].
            frontWheelLongitudinalForce = 2 * Car.FrontTire.TireModel.GetTireFx(frontWheelLongitudinalSlip, frontWheelSlipAngle, frontWheelVerticalLoad, 0, Speed);
            rearWheelLongitudinalForce = 2 * Car.RearTire.TireModel.GetTireFx(rearWheelLongitudinalSlip, rearWheelSlipAngle, rearWheelVerticalLoad, 0, Speed);
            double frontWheelLateralForce = 2 * Car.FrontTire.TireModel.GetTireFy(frontWheelLongitudinalSlip, frontWheelSlipAngle, frontWheelVerticalLoad, 0, Speed);
            double rearWheelLateralForce = 2 * Car.RearTire.TireModel.GetTireFy(rearWheelLongitudinalSlip, rearWheelSlipAngle, rearWheelVerticalLoad, 0, Speed);
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
    }
}
