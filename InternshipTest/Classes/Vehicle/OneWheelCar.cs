using MathNet.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternshipTest.Vehicle
{
    /// <summary>
    /// Contains a one wheel model's car information.
    /// </summary>
    [Serializable]
    public class OneWheelCar
    {
        #region Properties
        /// <summary>
        /// Car's identification.
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// Car's setup identification.
        /// </summary>
        public string SetupID { get; set; }
        /// <summary>
        /// Car's main information.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Car's inertia information.
        /// </summary>
        public OneWheelInertia Inertia { get; set; }
        /// <summary>
        /// Car's tire subsystem.
        /// </summary>
        public Tire Tire { get; set; }
        /// <summary>
        /// Car's engine subsystem.
        /// </summary>
        public Engine Engine { get; set; }
        /// <summary>
        /// Car's transmission subsystem.
        /// </summary>
        public Transmission Transmission { get; set; }
        /// <summary>
        /// Car's aerodynamics subsystem.
        /// </summary>
        public OneWheelAerodynamics Aerodynamics { get; set; }
        /// <summary>
        /// Car's suspension subsystem.
        /// </summary>
        public SimplifiedSuspension Suspension { get; set; }
        /// <summary>
        /// Car's brakes susbsystem.
        /// </summary>
        public OneWheelBrakes Brakes { get; set; }
        
        /// <summary>
        /// Car's suspension equivalent heave stiffness (considers the tire stiffness) [N/m].
        /// </summary>
        private double EquivalentHeaveStiffness { get; set; }
        /// <summary>
        /// Car's lowest operation speed [m/s].
        /// </summary>
        public double LowestSpeed { get; set; }
        /// <summary>
        /// Car's highest operation speed [m/s].
        /// </summary>
        public double HighestSpeed { get; set; }

        /// <summary>
        /// Wheel's rotational speed vector [rad/s].
        /// </summary>
        public List<double> WheelRotationalSpeedCurve { get; set; }
        /// <summary>
        /// Wheel's equivalent engine torque vector [N*m]
        /// </summary>
        public List<double> WheelTorqueCurve { get; set; }
        /// <summary>
        /// Wheel's gear number vector.
        /// </summary>
        public List<double> WheelGearCurve { get; set; }
        /// <summary>
        /// Wheel's equivalent engine braking torque vector [N*m]
        /// </summary>
        public List<double> WheelBrakingTorqueCurve { get; set; }
        /// <summary>
        /// Wheel's engine specific fuel consumption vector [kg/W]
        /// </summary>
        public List<double> WheelSpecFuelConsCurve { get; set; }
        #endregion
        #region Constructors
        public OneWheelCar() { }
        public OneWheelCar(string carID, string setupID, string description, OneWheelInertia inertia, Tire tire,
            Engine engine, Transmission transmission, OneWheelAerodynamics aerodynamics,
            SimplifiedSuspension suspension, OneWheelBrakes brakes)
        {
            ID = carID;
            SetupID = setupID;
            Description = description;
            Inertia = inertia;
            Tire = tire;
            Engine = engine;
            Transmission = transmission;
            Aerodynamics = aerodynamics;
            Suspension = suspension;
            Brakes = brakes;
        }
        #endregion
        #region Methods
        #region General Public Methods
        /// <summary>
        /// Sets up what is displayed at the UI's listbox.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "C: " + ID + " - S: " + SetupID;
        }
        /// <summary>
        /// Gets the equivalent heave stiffness of the car, given the tire and suspension stiffnesses.
        /// </summary>
        public void GetEquivalentHeaveStiffness()
        {
            EquivalentHeaveStiffness = (Suspension.HeaveStiffness * Tire.VerticalStiffness * 4) / (Suspension.HeaveStiffness + Tire.VerticalStiffness * 4);
        }
        /// <summary>
        /// Gets the current gear number based on the car's speed.
        /// </summary>
        /// <param name="speed"> Car's speed [m/s] </param>
        /// <returns> Gear number </returns>
        public int GetGearNumberFromCarSpeed(double speed)
        {
            // Gets the aerodynamic coefficients
            OneWheelAerodynamicMapPoint interpolatedAerodynamicMapPoint = GetAerodynamicCoefficients(speed);
            // Lift force [N]
            double liftForce = -interpolatedAerodynamicMapPoint.LiftCoefficient * Aerodynamics.FrontalArea * Aerodynamics.AirDensity * Math.Pow(speed, 2) / 2;
            // Tire resultant Fz [N]
            double tireFz = (liftForce + Inertia.TotalMass * Inertia.Gravity) / 4;
            // Calculates the wheel radius [m]
            double wheelRadius = Tire.TireModel.RO - tireFz / Tire.VerticalStiffness;
            // Wheel rotational speed [rad/s]
            double wheelCenterAngularSpeed = speed / wheelRadius;
            // Gear interpolation object
            alglib.spline1dbuildlinear(WheelRotationalSpeedCurve.ToArray(), WheelGearCurve.ToArray(), out alglib.spline1dinterpolant wheelGearCurveInterp);
            // Current gear number
            return (int)Math.Ceiling(alglib.spline1dcalc(wheelGearCurveInterp, wheelCenterAngularSpeed));
        }
        /// <summary>
        /// Gets the aerodynamic coefficients by interpolation of the aerodynamic map.
        /// </summary>
        /// <param name="speed"> Car's speed [m/s] </param>
        /// <returns> Interpolated aerodynamic map point </returns>
        public OneWheelAerodynamicMapPoint GetAerodynamicCoefficients(double speed)
        {
            double tol = 1e-6;
            double error;
            double rideHeight = Suspension.RideHeight;
            double liftForce;
            OneWheelAerodynamicMapPoint interpolatedAerodynamicMapPoint;
            int iter = 0;
            do
            {
                iter++;
                // Aerodynamic map interpolation
                interpolatedAerodynamicMapPoint = Aerodynamics.GetAerodynamicMapPointFromParameters(speed, rideHeight);
                // Calculates the lift force
                liftForce = -interpolatedAerodynamicMapPoint.LiftCoefficient * Aerodynamics.FrontalArea * Aerodynamics.AirDensity * Math.Pow(speed, 2) / 2;
                // New car height [m]
                double oldrideHeight = rideHeight;
                rideHeight = Suspension.RideHeight - liftForce / EquivalentHeaveStiffness;
                // Error update
                error = Math.Abs(rideHeight - oldrideHeight) * 1000;
            } while (error > tol && iter<100);
            return interpolatedAerodynamicMapPoint;
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
            int[,] gearsRPMRanges = _GetGearsRPMRanges(resultantEngineCurvesAtWheelsPerGearArray, resultantGearRatiosAtWheel);
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
            double[,] engineCurvesArray = new double[4, arrayDim1 ];
            for (int iPoint = 0; iPoint < arrayDim1; iPoint++)
            {
                engineCurvesArray[0, iPoint] = Engine.EngineCurves.CurvesPoints[iPoint].RotationalSpeed;
                engineCurvesArray[1,iPoint] = Engine.EngineCurves.CurvesPoints[iPoint].Torque * Engine.MaxThrottle;
                engineCurvesArray[2,iPoint] = Engine.EngineCurves.CurvesPoints[iPoint].BrakingTorque;
                engineCurvesArray[3,iPoint] = Engine.EngineCurves.CurvesPoints[iPoint].SpecFuelCons;
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
        private int[,] _GetGearsRPMRanges(double[,,] resultantEngineCurvesAtWheelsPerGearArray, double[] resultantGearRatiosAtWheel)
        {
            int amountOfGears = Transmission.GearRatiosSet.GearRatios.Count;
            int[,] gearsRPMRanges = new int[2, amountOfGears];
            // First gear starting rpm
            gearsRPMRanges[0, 0] = 0;
            // Gears sweep "for" loop
            for (int iGear = 0; iGear < amountOfGears - 1; iGear++)
            {
                /*// Index variables of the current and next gear curves
                int iCurrentGearCurves = ArrayManipulation.GetLineFromThreeDimensionalDoubleArray(resultantEngineCurvesAtWheelsPerGearArray, 0, iGear).Count();
                int iNextGearCurves = ArrayManipulation.GetLineFromThreeDimensionalDoubleArray(resultantEngineCurvesAtWheelsPerGearArray, 0, iGear + 1).Count();
                // Current scalling factors
                double speedCurveScallingFactor = 1 / (ArrayManipulation.GetLineFromThreeDimensionalDoubleArray(resultantEngineCurvesAtWheelsPerGearArray, 0, iGear).Max());
                double torqueCurveScallingFactor = 1 / (ArrayManipulation.GetLineFromThreeDimensionalDoubleArray(resultantEngineCurvesAtWheelsPerGearArray, 1, iGear + 1).Max());
                // Current gear rpm sweep "do while" loop error criteria initialization
                double currentDistanceBetweenCurrentGearPointAndNextGearTorqueVsRPMCurve = Math.Pow(10, 10);
                double previousDistanceBetweenCurrentGearPointAndNextGearTorqueVsRPMCurve;
                // Current gear rpm sweep "do while" loop
                do
                {
                    // Current gear curve index update
                    iCurrentGearCurves--;
                    // Next gear rpm sweep "do while" loop error criteria initialization
                    double currentDistanceBetweenCurrentAndNextGearTorqueVsRPMPoints = Math.Pow(10, 10);
                    double previousDistanceBetweenCurrentAndNextGearTorqueVsRPMPoints;
                    // Next gear rpm sweep "do while" loop
                    do
                    {
                        // Next gear curve index update
                        iNextGearCurves--;
                        // Error criteria update
                        previousDistanceBetweenCurrentAndNextGearTorqueVsRPMPoints = currentDistanceBetweenCurrentAndNextGearTorqueVsRPMPoints;
                        currentDistanceBetweenCurrentAndNextGearTorqueVsRPMPoints = Math.Sqrt(
                            Math.Pow(
                                resultantEngineCurvesAtWheelsPerGearArray[0, iCurrentGearCurves, iGear] -
                                resultantEngineCurvesAtWheelsPerGearArray[0, iNextGearCurves, iGear + 1], 2) * speedCurveScallingFactor +
                            Math.Pow(
                                resultantEngineCurvesAtWheelsPerGearArray[1, iCurrentGearCurves, iGear] -
                                resultantEngineCurvesAtWheelsPerGearArray[1, iNextGearCurves, iGear + 1], 2) * torqueCurveScallingFactor);
                    } while (previousDistanceBetweenCurrentAndNextGearTorqueVsRPMPoints > currentDistanceBetweenCurrentAndNextGearTorqueVsRPMPoints);
                    // Error criteria update
                    previousDistanceBetweenCurrentGearPointAndNextGearTorqueVsRPMCurve = currentDistanceBetweenCurrentGearPointAndNextGearTorqueVsRPMCurve;
                    currentDistanceBetweenCurrentGearPointAndNextGearTorqueVsRPMCurve = currentDistanceBetweenCurrentAndNextGearTorqueVsRPMPoints;
                } while (previousDistanceBetweenCurrentGearPointAndNextGearTorqueVsRPMCurve > currentDistanceBetweenCurrentGearPointAndNextGearTorqueVsRPMCurve);*/
                /*
                double[] currentGearSpeeds = ArrayManipulation.GetLineFromThreeDimensionalDoubleArray(resultantEngineCurvesAtWheelsPerGearArray, 0, iGear);
                double[] currentGearTorques = ArrayManipulation.GetLineFromThreeDimensionalDoubleArray(resultantEngineCurvesAtWheelsPerGearArray, 1, iGear);
                double[] nextGearSpeeds = ArrayManipulation.GetLineFromThreeDimensionalDoubleArray(resultantEngineCurvesAtWheelsPerGearArray, 0, iGear + 1);
                double[] nextGearTorques = ArrayManipulation.GetLineFromThreeDimensionalDoubleArray(resultantEngineCurvesAtWheelsPerGearArray, 1, iGear + 1);
                double intersectionSpeed = _GetCurvesIntersectionXCoordinate(currentGearSpeeds, currentGearTorques, nextGearSpeeds, nextGearTorques);*/
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
                /*
                // Converts from rpm to rad/s
                for (int i = 0; i < resultantEngineCurvesAtWheelsPerGearArray.GetLength(1); i++)
                {
                    resultantEngineCurvesAtWheelsPerGearArray[0, i, iGear] *= ArrayManipulation.GetLineFromThreeDimensionalDoubleArray(resultantEngineCurvesAtWheelsPerGearArray, 0, iGear)[i] * (2 * Math.PI / 60);
                }*/
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
            double wheelRadius = Tire.TireModel.RO;
            double speed;
            int iter = 0;
            // Optimization "do while" loop
            do
            {
                iter++;
                // Speed for current wheel radius
                speed = (Engine.EngineCurves.CurvesPoints[iRPM].RotationalSpeed * wheelRadius /
                    (Transmission.GearRatiosSet.GearRatios[gearNumber - 1].Ratio * Transmission.PrimaryRatio * Transmission.FinalRatio));
                // Gets the aerodynamic coefficients
                OneWheelAerodynamicMapPoint interpolatedAerodynamicMapPoint = GetAerodynamicCoefficients(speed);
                // Updates the lift force
                double liftForce = -interpolatedAerodynamicMapPoint.LiftCoefficient * Aerodynamics.FrontalArea * Aerodynamics.AirDensity * Math.Pow(speed, 2) / 2;
                // Updates the tire vertical load
                double tireFz = (liftForce + Inertia.TotalMass * Inertia.Gravity) / 4;
                // Updates the wheel radius
                double oldWheelRadius = wheelRadius;
                wheelRadius = Tire.TireModel.RO - tireFz / Tire.VerticalStiffness;
                // Updates the error (optimization criteria)
                error = Math.Abs(wheelRadius - oldWheelRadius);
            } while (error > tol);
            // Returns the found speed
            return speed;
        }
        #endregion
        #endregion
    }
}
