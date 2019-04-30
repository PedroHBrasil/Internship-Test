using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternshipTest.Vehicle
{
    /// <summary>
    /// Contains the information of a two wheel vehicle model object.
    /// </summary>
    [Serializable]
    public class TwoWheelCar : Car
    {
        #region Properties
        /// <summary>
        /// Car's aerodynamics parameters.
        /// </summary>
        public TwoWheelAerodynamics Aerodynamics { get; set; }
        /// <summary>
        /// Car's brakes subsystem parameters.
        /// </summary>
        public TwoWheelBrakes Brakes { get; set; }
        /// <summary>
        /// Car's inertia and dimensions parameters.
        /// </summary>
        public TwoWheelInertiaAndDimensions InertiaAndDimensions { get; set; }
        /// <summary>
        /// Car's front suspension parameters.
        /// </summary>
        public SimplifiedSuspension FrontSuspension { get; set; }
        /// <summary>
        /// Car's rear suspension parameters.
        /// </summary>
        public SimplifiedSuspension RearSuspension { get; set; }
        /// <summary>
        /// Car's steering system parameters.
        /// </summary>
        public SteeringSystem Steering { get; set; }
        /// <summary>
        /// Car's front tire parameters.
        /// </summary>
        public Tire FrontTire { get; set; }
        /// <summary>
        /// Car's rear tire parameters.
        /// </summary>
        public Tire RearTire { get; set; }
        /// <summary>
        /// Car's transmission parameters.
        /// </summary>
        public TwoWheelTransmission Transmission { get; set; }
        #endregion

        #region Constructors
        public TwoWheelCar() { }
        public TwoWheelCar(string carID, string setupID, string description, TwoWheelAerodynamics aerodynamics, TwoWheelBrakes brakes, Engine engine, TwoWheelInertiaAndDimensions inertiaAndDimensions, SimplifiedSuspension frontSuspension, SimplifiedSuspension rearSuspension, SteeringSystem steering, Tire frontTire, Tire rearTire, TwoWheelTransmission transmission)
        {
            ID = carID;
            SetupID = setupID;
            Description = description;
            Aerodynamics = aerodynamics;
            Brakes = brakes;
            Engine = engine;
            InertiaAndDimensions = inertiaAndDimensions;
            FrontSuspension = frontSuspension;
            RearSuspension = rearSuspension;
            Steering = steering;
            FrontTire = frontTire;
            RearTire = rearTire;
            Transmission = transmission;
        }
        #endregion

        #region Methods
        #region General Methods
        /// Gets the current gear number based on the car's speed.
        /// </summary>
        /// <param name="speed"> Car's speed [m/s] </param>
        /// <returns> Gear number </returns>
        public int GetGearNumberFromCarSpeed(double speed)
        {
            // Gets the reference wheel radius [m]
            double referenceWheelRadius = _GetMeanWheelRadius(speed, 0);
            // Wheel rotational speed [rad/s]
            double wheelCenterAngularSpeed = speed / referenceWheelRadius;
            // Gear interpolation object
            alglib.spline1dbuildlinear(WheelRotationalSpeedCurve.ToArray(), WheelGearCurve.ToArray(), out alglib.spline1dinterpolant wheelGearCurveInterp);
            // Current gear number
            return (int)Math.Ceiling(alglib.spline1dcalc(wheelGearCurveInterp, wheelCenterAngularSpeed));
        }
        /// <summary>
        /// Gets the aerodynamic coefficients by interpolation of the aerodynamic map.
        /// </summary>
        /// <param name="speed"> Car's speed [m/s] </param>
        /// <param name="carSlipAngle"> Car's slip angle [rad] </param>
        /// <returns> Interpolated aerodynamic map point </returns>
        public TwoWheelAerodynamicMapPoint GetAerodynamicCoefficients(double speed, double carSlipAngle, double longitudinalLoadTransfer)
        {
            // Equivalent heave stiffnesses
            double equivalentFrontHeaveStiffness = (FrontSuspension.HeaveStiffness * FrontTire.VerticalStiffness * 2) / (FrontSuspension.HeaveStiffness + FrontTire.VerticalStiffness * 2);
            double equivalentRearHeaveStiffness = (RearSuspension.HeaveStiffness * RearTire.VerticalStiffness * 2) / (RearSuspension.HeaveStiffness + RearTire.VerticalStiffness * 2);
            // Optimization parameters
            double tol = 1e-6;
            double errorFront;
            double errorRear;
            double frontRideHeight = FrontSuspension.RideHeight;
            double rearRideHeight = RearSuspension.RideHeight;
            TwoWheelAerodynamicMapPoint interpolatedAerodynamicMapPoint;
            int iter = 0;
            do
            {
                iter++;
                // Aerodynamic map interpolation
                interpolatedAerodynamicMapPoint = Aerodynamics.GetAerodynamicMapPointFromParameters(speed, frontRideHeight, rearRideHeight, carSlipAngle);
                // Calculates the lift force
                double liftForce = -interpolatedAerodynamicMapPoint.LiftCoefficient * Aerodynamics.FrontalArea * Aerodynamics.AirDensity * Math.Pow(speed, 2) / 2;
                // Calculates the aerodynamic pitch moment
                double aerodynamicPitchMoment = -interpolatedAerodynamicMapPoint.PitchMomentCoefficient * Aerodynamics.FrontalArea * Aerodynamics.AirDensity * Math.Pow(speed, 2) / 2;
                // Resultant front and rear forces
                double frontAerodynamicVerticalForce = (liftForce - aerodynamicPitchMoment / InertiaAndDimensions.Wheelbase - longitudinalLoadTransfer) / 2;
                double rearAerodynamicVerticalForce = (liftForce + aerodynamicPitchMoment / InertiaAndDimensions.Wheelbase + longitudinalLoadTransfer) / 2;
                // New car height [m]
                double oldFrontRideHeight = frontRideHeight;
                frontRideHeight = FrontSuspension.RideHeight - frontAerodynamicVerticalForce / equivalentFrontHeaveStiffness;
                double oldRearRideHeight = rearRideHeight;
                rearRideHeight = RearSuspension.RideHeight - rearAerodynamicVerticalForce / equivalentRearHeaveStiffness;
                // Error update
                errorFront = Math.Abs(frontRideHeight - oldFrontRideHeight) * 1000;
                errorRear = Math.Abs(rearRideHeight - oldRearRideHeight) * 1000;
            } while ((errorFront > tol || errorRear > tol) && iter < 100);
            return interpolatedAerodynamicMapPoint;
        }
        /// <summary>
        /// Gets the reference wheel radius for the transmission system calculations.
        /// </summary>
        /// <param name="speed"> Car's speed [m/s]. </param>
        /// <param name="carSlipAngle"> Car's slip angle [rad] </param>
        /// <returns> The reference radius for the current speed and car slip angle. </returns>
        private double _GetMeanWheelRadius(double speed, double carSlipAngle)
        {
            // Gets the aerodynamic coefficients
            TwoWheelAerodynamicMapPoint interpolatedAerodynamicMapPoint = GetAerodynamicCoefficients(speed, carSlipAngle);
            // Calculates the lift force
            double liftForce = -interpolatedAerodynamicMapPoint.LiftCoefficient * Aerodynamics.FrontalArea * Aerodynamics.AirDensity * Math.Pow(speed, 2) / 2;
            // Calculates the aerodynamic pitch moment
            double aerodynamicPitchMoment = -interpolatedAerodynamicMapPoint.PitchMomentCoefficient * Aerodynamics.FrontalArea * Aerodynamics.AirDensity * Math.Pow(speed, 2) / 2;
            // Tire resultant Fz [N]
            double frontTireFz = (liftForce - aerodynamicPitchMoment / InertiaAndDimensions.Wheelbase + InertiaAndDimensions.TotalMass * InertiaAndDimensions.Gravity * InertiaAndDimensions.TotalMassDistribution) / 2;
            double rearTireFz = (liftForce + aerodynamicPitchMoment / InertiaAndDimensions.Wheelbase + InertiaAndDimensions.TotalMass * InertiaAndDimensions.Gravity * (1 - InertiaAndDimensions.TotalMassDistribution)) / 2;
            // Calculates the mean wheel radius [m]
            double frontWheelRadius = FrontTire.TireModel.RO - frontTireFz / FrontTire.VerticalStiffness;
            double rearWheelRadius = RearTire.TireModel.RO - rearTireFz / RearTire.VerticalStiffness;
            double meanWheelRadius = (frontWheelRadius + rearWheelRadius) / 2;

            return meanWheelRadius;
        }
        #endregion
        #region Get Linear Acceleration Parameters Methods
        /// <summary>
        /// Gets the car's linear acceleration parameters (equivalent torques, braking torques and specific fuel consumptions)
        /// as function of the wheel's rotational speeds. For this, the engine's and the transmission's parameters are used.
        /// </summary>
        public void GetLinearAccelerationParameters()
        {
            // Linear acceleration lists initialization
            WheelRotationalSpeedCurve = new List<double>();
            WheelTorqueCurve = new List<double>();
            WheelGearCurve = new List<double>();
            WheelBrakingTorqueCurve = new List<double>();
            WheelSpecFuelConsCurve = new List<double>();
            // Converts the engine curves into a 2 dimensional array
            double[,] engineCurvesArray = _GetEngineCurvesArray();
            // Adjust the engine curves to start at zero RPM, if they are not.
            if (engineCurvesArray[0, 0] != 0)
            {
                engineCurvesArray = _AddZeroRotationalSpeedPointToCurves(engineCurvesArray);
            }
            // Get interpolated engine curves (one point per engine rpm)
            double[,] engineInterpolatedCurvesArray = _GetEngineInterpolatedCurves(engineCurvesArray);
            // Resultant transmission gear ratios at the wheel
            double[] resultantGearRatiosAtWheel = _GetResultantGearRatiosAtWheel();
            // Gets the resultant engine curves for wheel rpms for each gear
            double[,,] resultantEngineCurvesAtWheelsPerGearArray =
                _GetResultantEngineCurvesAtWheelPerGear(engineInterpolatedCurvesArray, resultantGearRatiosAtWheel);
            // Gets the gears rpm ranges (first and last rpms indexes based on the gear shifting)
            int[,] gearsRPMRanges = _GetGearsRPMRanges(resultantEngineCurvesAtWheelsPerGearArray);
            // Resultant engine curves at wheel
            _GetResultantEngineCurves(resultantEngineCurvesAtWheelsPerGearArray, gearsRPMRanges);
        }
        /// <summary>
        /// Converts the engine curves object information to an array for easier manipulation.
        /// </summary>
        /// <param name="engine"> Engine object which contains the curves </param>
        /// <returns> A two dimensional array where:
        ///     Each column (dimension zero) corresonds to: rpm, torque, braking torque and specific fuel consumption.
        ///     Each row corresponds to a data point.
        /// </returns>
        private double[,] _GetEngineCurvesArray()
        {
            int arrayDim1 = Engine.EngineCurves.CurvesPoints.Count();
            double[,] engineCurvesArray = new double[4, arrayDim1];
            for (int iPoint = 0; iPoint < arrayDim1; iPoint++)
            {
                engineCurvesArray[0, iPoint] = Engine.EngineCurves.CurvesPoints[iPoint].RotationalSpeed;
                engineCurvesArray[1, iPoint] = Engine.EngineCurves.CurvesPoints[iPoint].Torque * Engine.MaxThrottle;
                engineCurvesArray[2, iPoint] = Engine.EngineCurves.CurvesPoints[iPoint].BrakingTorque;
                engineCurvesArray[3, iPoint] = Engine.EngineCurves.CurvesPoints[iPoint].SpecFuelCons;
            }
            return engineCurvesArray;
        }
        /// <summary>
        /// Modifies the engnie curves array so that the first element corresponds to rpm equal to zero
        /// </summary>
        /// <param name="engineCurvesArray"> Two dimensional double a which contains the original engine curves data. </param>
        /// <returns> Two dimensional double array with added first element which rpm is equal to zero. </returns>
        private double[,] _AddZeroRotationalSpeedPointToCurves(double[,] engineCurvesArray)
        {
            double[,] newEngineCurvesArray = new double[engineCurvesArray.GetLength(0), engineCurvesArray.GetLength(1) + 1];
            // Adds one element to the end of the array and shifts all of the points one position forward.
            for (int iRPM = engineCurvesArray.GetLength(1); iRPM > 0; iRPM--)
            {
                for (int iCurve = 0; iCurve < 4; iCurve++)
                {
                    newEngineCurvesArray[iCurve, iRPM] = engineCurvesArray[iCurve, iRPM - 1];
                }
            }
            // Adds rpm equal to zero to the curves  
            newEngineCurvesArray[0, 0] = 0;
            newEngineCurvesArray[1, 0] = newEngineCurvesArray[1, 1];
            newEngineCurvesArray[2, 0] = newEngineCurvesArray[2, 1];
            newEngineCurvesArray[3, 0] = newEngineCurvesArray[3, 1];

            return newEngineCurvesArray;
        }
        /// <summary>
        /// Interpolates the engine curves so that there is one point per engine RPM.
        /// </summary>
        /// <param name="engineCurvesArray"> Array which contains the engine curves information (_GetEngineCurvesArray() output)</param>
        /// <returns> An array which contains the interpolated engine curves (one point per engine rpm) </returns>
        private double[,] _GetEngineInterpolatedCurves(double[,] engineCurvesArray)
        {
            // Array initialization
            int interpolatedArrayDim1 = (int)ArrayManipulation.GetLineFromTwoDimensionalDoubleArray(engineCurvesArray, 0).Max() + 1;
            double[,] engineInterpolatedCurvesArray = new double[4, interpolatedArrayDim1];
            // Engine curves interpolation objects initialization
            alglib.spline1dbuildlinear(ArrayManipulation.GetLineFromTwoDimensionalDoubleArray(engineCurvesArray, 0),
                ArrayManipulation.GetLineFromTwoDimensionalDoubleArray(engineCurvesArray, 1),
                out alglib.spline1dinterpolant engineTorqueInterp);
            alglib.spline1dbuildlinear(ArrayManipulation.GetLineFromTwoDimensionalDoubleArray(engineCurvesArray, 0),
                ArrayManipulation.GetLineFromTwoDimensionalDoubleArray(engineCurvesArray, 2),
                out alglib.spline1dinterpolant engineBrakingTorqueInterp);
            alglib.spline1dbuildlinear(ArrayManipulation.GetLineFromTwoDimensionalDoubleArray(engineCurvesArray, 0),
                ArrayManipulation.GetLineFromTwoDimensionalDoubleArray(engineCurvesArray, 3),
                out alglib.spline1dinterpolant engineSpecFuelConsInterp);
            // Population of engine curves per RPM
            for (int iRotationalSpeed = 0; iRotationalSpeed < interpolatedArrayDim1; iRotationalSpeed++)
            {
                engineInterpolatedCurvesArray[0, iRotationalSpeed] = iRotationalSpeed;
                engineInterpolatedCurvesArray[1, iRotationalSpeed] = alglib.spline1dcalc(engineTorqueInterp, iRotationalSpeed);
                engineInterpolatedCurvesArray[2, iRotationalSpeed] = alglib.spline1dcalc(engineBrakingTorqueInterp, iRotationalSpeed);
                engineInterpolatedCurvesArray[3, iRotationalSpeed] = alglib.spline1dcalc(engineSpecFuelConsInterp, iRotationalSpeed);
            }

            return engineInterpolatedCurvesArray;
        }
        /// <summary>
        /// Gets the resultant gear ratios at the wheel by multiplication of the primary and final ratios by each gear ratio.
        /// </summary>
        /// <returns> An array which contains the resultant gear ratios at the wheel. </returns>
        private double[] _GetResultantGearRatiosAtWheel()
        {
            int arrayLength = Transmission.GearRatiosSet.GearRatios.Count;
            double[] resultantGearRatiosAtWheel = new double[arrayLength];
            for (int iGear = 0; iGear < arrayLength; iGear++)
            {
                double resultantGearRatio = Transmission.GearRatiosSet.GearRatios[iGear].Ratio * Transmission.PrimaryRatio * Transmission.FinalRatio;
                resultantGearRatiosAtWheel[iGear] = resultantGearRatio;
            }

            return resultantGearRatiosAtWheel;
        }
        /// <summary>
        /// Gets the resultant engine curves as function of the wheel's rpm by correcting the
        /// engine torques and rotation speeds with the transmission's ratios.
        /// </summary>
        /// <param name="engineInterpolatedCurvesArray"> Two dimensional array which contains the engine interpolated curves (one point per rpm). </param>
        /// <param name="resultantGearRatiosAtWheel"> Array which contains the resultant gear ratios at the wheel. </param>
        /// <returns> A nested array which contains the resultant engine curves at the wheel per gear. </returns>
        private double[,,] _GetResultantEngineCurvesAtWheelPerGear(double[,] engineInterpolatedCurvesArray, double[] resultantGearRatiosAtWheel)
        {
            // Array initialization
            int amountOfGears = Transmission.GearRatiosSet.GearRatios.Count;
            int curvesLength = ArrayManipulation.GetLineFromTwoDimensionalDoubleArray(engineInterpolatedCurvesArray, 0).Length;
            double[,,] resultantEngineCurvesAtWheelsPerGearArray = new double[4, curvesLength, amountOfGears];
            // Gears sweep "for" loop
            for (int iGear = 0; iGear < amountOfGears; iGear++)
            {
                // RPM sweep "for" loop
                for (int rpm = 0; rpm < curvesLength; rpm++)
                {
                    resultantEngineCurvesAtWheelsPerGearArray[0, rpm, iGear] = rpm / resultantGearRatiosAtWheel[iGear];
                    resultantEngineCurvesAtWheelsPerGearArray[1, rpm, iGear] = engineInterpolatedCurvesArray[1, rpm] * resultantGearRatiosAtWheel[iGear];
                    resultantEngineCurvesAtWheelsPerGearArray[2, rpm, iGear] = engineInterpolatedCurvesArray[2, rpm] * resultantGearRatiosAtWheel[iGear];
                    resultantEngineCurvesAtWheelsPerGearArray[3, rpm, iGear] = engineInterpolatedCurvesArray[3, rpm];
                }
            }
            return resultantEngineCurvesAtWheelsPerGearArray;
        }
        /// <summary>
        /// Gets the gears rpms ranges based on the resultant engine curves at the wheel per gear.
        /// </summary>
        /// <param name="resultantEngineCurvesAtWheelsPerGearDictionary"> Array which contains the resultant engine cuves at the wheel per gear. </param>
        /// <returns> Array which contains the starting and ending rpms of each gear on each column. </returns>
        private int[,] _GetGearsRPMRanges(double[,,] resultantEngineCurvesAtWheelsPerGearArray)
        {
            int amountOfGears = Transmission.GearRatiosSet.GearRatios.Count;
            int[,] gearsRPMRanges = new int[2, amountOfGears];
            // First gear starting rpm
            gearsRPMRanges[0, 0] = 0;
            // Gears sweep "for" loop
            for (int iGear = 0; iGear < amountOfGears - 1; iGear++)
            {
                double[,] currentGearCurve = ArrayManipulation.JoinArraysIn2DArray(ArrayManipulation.GetLineFromThreeDimensionalDoubleArray(resultantEngineCurvesAtWheelsPerGearArray, 0, iGear), ArrayManipulation.GetLineFromThreeDimensionalDoubleArray(resultantEngineCurvesAtWheelsPerGearArray, 1, iGear));
                double[,] nextGearCurve = ArrayManipulation.JoinArraysIn2DArray(ArrayManipulation.GetLineFromThreeDimensionalDoubleArray(resultantEngineCurvesAtWheelsPerGearArray, 0, iGear + 1), ArrayManipulation.GetLineFromThreeDimensionalDoubleArray(resultantEngineCurvesAtWheelsPerGearArray, 1, iGear + 1));
                int[] rpmIndexes = ArrayManipulation.GetCurvesMinimumDistancePointsIndexes(currentGearCurve, nextGearCurve);
                // Gear shifting ranges array update
                gearsRPMRanges[1, iGear] = rpmIndexes[0];
                gearsRPMRanges[0, iGear + 1] = rpmIndexes[1];
            }
            // Adds the limit rpm index to the ranges array
            gearsRPMRanges[1, Transmission.GearRatiosSet.GearRatios.Count - 1] = (int)Engine.EngineCurves.CurvesPoints[Engine.EngineCurves.CurvesPoints.Count() - 1].RotationalSpeed;

            return gearsRPMRanges;
        }
        /// <summary>
        /// Crosses the information between the resutant engine curves at the wheels for each gear and the gears shifting rpms indexes
        /// to assign the resultant curves to the results lists.
        /// </summary>
        /// <param name="resultantEngineCurvesAtWheelsPerGearArray"> Array which contains the resultant engine cuves at the wheel per gear. </param>
        /// <param name="gearsRPMRanges"> Array which contains the starting and ending rpms of each gear on each column. </param>
        private void _GetResultantEngineCurves(double[,,] resultantEngineCurvesAtWheelsPerGearArray,
            int[,] gearsRPMRanges)
        {
            for (int iGear = 0; iGear < Transmission.GearRatiosSet.GearRatios.Count; iGear++)
            {
                // Gets the wished array segments
                ArraySegment<double> resultantWheelEngineRPMs = new ArraySegment<double>(
                    ArrayManipulation.GetLineFromThreeDimensionalDoubleArray(resultantEngineCurvesAtWheelsPerGearArray, 0, iGear),
                    gearsRPMRanges[0, iGear],
                    gearsRPMRanges[1, iGear] - gearsRPMRanges[0, iGear]);
                ArraySegment<double> resultantWheelEngineTorques = new ArraySegment<double>(
                    ArrayManipulation.GetLineFromThreeDimensionalDoubleArray(resultantEngineCurvesAtWheelsPerGearArray, 1, iGear),
                    gearsRPMRanges[0, iGear],
                    gearsRPMRanges[1, iGear] - gearsRPMRanges[0, iGear]);
                ArraySegment<double> resultantWheelEngineBrakingTorques = new ArraySegment<double>(
                    ArrayManipulation.GetLineFromThreeDimensionalDoubleArray(resultantEngineCurvesAtWheelsPerGearArray, 2, iGear),
                    gearsRPMRanges[0, iGear],
                    gearsRPMRanges[1, iGear] - gearsRPMRanges[0, iGear]);
                ArraySegment<double> resultantWheelEngineSpecFuelCons = new ArraySegment<double>(
                    ArrayManipulation.GetLineFromThreeDimensionalDoubleArray(resultantEngineCurvesAtWheelsPerGearArray, 3, iGear),
                    gearsRPMRanges[0, iGear],
                    gearsRPMRanges[1, iGear] - gearsRPMRanges[0, iGear]);
                // Adds the data to the final arrays
                WheelRotationalSpeedCurve.AddRange(resultantWheelEngineRPMs);
                WheelTorqueCurve.AddRange(resultantWheelEngineTorques);
                WheelBrakingTorqueCurve.AddRange(resultantWheelEngineBrakingTorques);
                WheelSpecFuelConsCurve.AddRange(resultantWheelEngineSpecFuelCons);
                for (int iRPM = gearsRPMRanges[0, iGear]; iRPM < gearsRPMRanges[1, iGear]; iRPM++)
                    WheelGearCurve.Add(iGear + 1);
            }
        }
        #endregion
        #region Car Operation Speed Determination Methods
        /// <summary>
        /// Gets the car's minimum and maximum speeds based on the engine's and transmission's parameters.
        /// </summary>
        public void GetCarOperationSpeedRange()
        {
            LowestSpeed = _GetCarSpeedFromEngineSpeed(0, 1);
            HighestSpeed = _GetCarSpeedFromEngineSpeed(Engine.EngineCurves.CurvesPoints.Count - 1, Transmission.GearRatiosSet.GearRatios.Count);
        }
        /// <summary>
        /// Gets the car speed for given engine rpm and gear number.
        /// </summary>
        /// <param name="iRPM"> Zero-based index of the engine rpm. </param>
        /// <param name="gearNumber"> One-based index of the gear. </param>
        /// <returns> The speed of the car for the given engine rpm and gear number. </returns>
        private double _GetCarSpeedFromEngineSpeed(int iRPM, int gearNumber)
        {
            // Optimization parameters
            double tol = 1e-6;
            double error;
            // Optimization variables initialization
            double referenceWheelRadius = (FrontTire.TireModel.RO + RearTire.TireModel.RO) / 2;
            double speed;
            int iter = 0;
            // Optimization "do while" loop
            do
            {
                iter++;
                // Speed for current wheel radius
                speed = Engine.EngineCurves.CurvesPoints[iRPM].RotationalSpeed * referenceWheelRadius /
                    (Transmission.GearRatiosSet.GearRatios[gearNumber - 1].Ratio * Transmission.PrimaryRatio * Transmission.FinalRatio);
                // Updates the error (optimization criteria)
                double oldReferenceWheelRadius = referenceWheelRadius;
                referenceWheelRadius = _GetMeanWheelRadius(speed, 0);
                error = Math.Abs(referenceWheelRadius - oldReferenceWheelRadius);
            } while (error > tol);
            // Returns the found speed
            return speed;
        }
        #endregion
        #endregion
    }
}
