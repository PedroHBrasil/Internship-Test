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

        public OneWheelGGDiagram(double speed, Vehicle.OneWheelCar car, int amountOfPoints)
        {
            Speed = speed;
            Car = car;
            AmountOfPoints = amountOfPoints;
            AmountOfDirections = amountOfPoints;

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
                    double tireMy = 0;
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
                            kappa = Car.Tire.TireModel.GetLongitudinalSlipForGivenLongitudinalForce(referenceFx, kappas, alpha, tireFz, 0, Speed);
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
                            kappa = Car.Tire.TireModel.GetLongitudinalSlipForGivenLongitudinalForce(referenceFx, kappas, alpha, tireFz, 0, Speed);
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
        #endregion
        #endregion
    }
}
