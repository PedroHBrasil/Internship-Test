using MathNet.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternshipTest.Simulation
{
    [Serializable]
    class LapTimeSimulation
    {
        // Fields
        public double[] GGVLongitudinalAccelerationAtMinimumLateralAccelerationPerSpeed;
        public double[] GGVLongitudinalAccelerationAtMaximumLateralAccelerationPerSpeed;
        public double[] GGVMinimumLateralAccelerationPerSpeed;
        public double[] GGVMaximumLateralAccelerationPerSpeed;
        public double[] GGVCurvatureAtMinimumLateralAccelerationPerSpeed;
        public double[] GGVCurvatureAtMaximumLateralAccelerationPerSpeed;
        // Properties
        public GGVDiagram SimulationGGVDiagram { get; set; }
        public Path SimulationPath { get; set; }
        public bool IsFirstLap { get; set; }
        // Dynamic states of the maximum possible speeds at each point (initial guess of the dynamic states profile)
        public List<double> PathMaximumPossibleSpeeds { get; set; }
        public List<double> PathLongitudinalAccelerationsForMaximumPossibleSpeeds { get; set; }
        public List<double> PathLateralAccelerationsForMaximumPossibleSpeeds { get; set; }
        public List<int> PathGearsForMaximumPossibleSpeeds { get; set; }
        // Final dynamic states found by the simulation
        public List<double> PathSpeeds { get; set; }
        public List<double> PathLongitudinalAccelerations { get; set; }
        public List<double> PathLateralAccelerations { get; set; }
        public List<int> PathGearNumbers { get; set; }
        // Constructors
        public LapTimeSimulation(GGVDiagram diagram, Path path, bool isFirstLap)
        {
            SimulationGGVDiagram = diagram;
            SimulationPath = path;
            IsFirstLap = isFirstLap;

            // Initializes the arrays to store the data
            GGVLongitudinalAccelerationAtMinimumLateralAccelerationPerSpeed = new double[SimulationGGVDiagram.Speeds.Count()];
            GGVLongitudinalAccelerationAtMaximumLateralAccelerationPerSpeed = new double[SimulationGGVDiagram.Speeds.Count()];
            GGVMinimumLateralAccelerationPerSpeed = new double[SimulationGGVDiagram.Speeds.Count()];
            GGVMaximumLateralAccelerationPerSpeed = new double[SimulationGGVDiagram.Speeds.Count()];
            GGVCurvatureAtMinimumLateralAccelerationPerSpeed = new double[SimulationGGVDiagram.Speeds.Count()];
            GGVCurvatureAtMaximumLateralAccelerationPerSpeed = new double[SimulationGGVDiagram.Speeds.Count()];
            // Initializes the lists of the maximum possible speeds dynamic states
            PathMaximumPossibleSpeeds = new List<double>();
            PathLongitudinalAccelerationsForMaximumPossibleSpeeds = new List<double>();
            PathLateralAccelerationsForMaximumPossibleSpeeds = new List<double>();
            PathGearsForMaximumPossibleSpeeds = new List<int>();

            GetMaximumPossibleSpeeds();
            GetPathSpeeds();
        }
        // Methods
        public override string ToString()
        {
            return "GGV: (" + SimulationGGVDiagram + ") - Path: " + SimulationPath;
        }
        private void GetMaximumPossibleSpeeds()
        {
            // Gets the GGV's maximum and minimum lateral accelerations (and associated parameters) per speed
            GetGGVLimitLateralAccelerationParametersPerSpeed();
            // Interpolates the GGV diagram to get the maximum speeds at each point of the path
            InterpolateGGVDiagramToGetPathMaximumSpeedsProfile();
        }
        #region GetPathMaximumPossibleSpeedsMethods
        private void GetGGVLimitLateralAccelerationParametersPerSpeed()
        {
            // Gets the GGV maximum and minimum parameters per speed
            for (int iSpeed = 0; iSpeed < SimulationGGVDiagram.Speeds.Count(); iSpeed++)
            {
                // Gets and registers the longitudinal accelerations
                GGVLongitudinalAccelerationAtMinimumLateralAccelerationPerSpeed[iSpeed] = SimulationGGVDiagram.GGDiagrams[iSpeed].LongitudinalAccelerations.Min();
                GGVLongitudinalAccelerationAtMaximumLateralAccelerationPerSpeed[iSpeed] = SimulationGGVDiagram.GGDiagrams[iSpeed].LongitudinalAccelerations.Max();
                // Gets and registers the lateral accelerations
                GGVMinimumLateralAccelerationPerSpeed[iSpeed] = SimulationGGVDiagram.GGDiagrams[iSpeed].LateralAccelerations.Min();
                GGVMaximumLateralAccelerationPerSpeed[iSpeed] = SimulationGGVDiagram.GGDiagrams[iSpeed].LateralAccelerations.Max();
                // Gets and registers the curvatures
                GGVCurvatureAtMinimumLateralAccelerationPerSpeed[iSpeed] = SimulationGGVDiagram.GGDiagrams[iSpeed].Curvatures.Min();
                GGVCurvatureAtMaximumLateralAccelerationPerSpeed[iSpeed] = SimulationGGVDiagram.GGDiagrams[iSpeed].Curvatures.Max();
            }
        }
        private void InterpolateGGVDiagramToGetPathMaximumSpeedsProfile()
        {
            // Path points "for" loop
            for (int iPathPoint = 0; iPathPoint < SimulationPath.AmountOfPointsInPath; iPathPoint++)
            {
                // Current path point's local curvature
                double currentCurvature = SimulationPath.LocalCurvatures[iPathPoint];
                // Evaluates if the local curvature is in the GGV diagram range or not
                bool isCurvatureInRange = CheckIfCurvatureIsInGGVRange(currentCurvature);
                // Gets the current point maximum speed and associated parameters
                if (isCurvatureInRange)
                {
                    GetPathMaximumPossibleSpeedsByInterpolation(currentCurvature);
                }
                else
                {
                    GetPathMaximumPossibleSpeedsByExtrapolation(currentCurvature);
                }
            }
        }
        #region GGVInterpolationToGetMaximumPossibleSpeeds
        private bool CheckIfCurvatureIsInGGVRange(double curvature)
        {
            // Checks if the curvature is out of the GGV range (for limit lateral accelerations)
            if ((curvature < 0 &&
                (curvature < GGVCurvatureAtMinimumLateralAccelerationPerSpeed.Min() ||
                curvature > GGVCurvatureAtMinimumLateralAccelerationPerSpeed.Max())) ||
                (curvature > 0 &&
                (curvature < GGVCurvatureAtMaximumLateralAccelerationPerSpeed.Min() ||
                curvature > GGVCurvatureAtMaximumLateralAccelerationPerSpeed.Max())))
                return false;
            else return true;
        }
        private void GetPathMaximumPossibleSpeedsByInterpolation(double curvature)
        {

            // Current accelerations initialization
            double currentLongitudinalAcceleration;
            double currentLateralAcceleration;
            // Current accelerations values by interpolation
            if (curvature < 0)
            {
                // Gets the interpolation objects (Longitudinal acceleration and lateral acceleration)
                alglib.spline1dbuildlinear(GGVCurvatureAtMinimumLateralAccelerationPerSpeed,
                    GGVLongitudinalAccelerationAtMinimumLateralAccelerationPerSpeed,
                    out alglib.spline1dinterpolant longitudinalAccelerationForMinLateralInterp);
                alglib.spline1dbuildlinear(GGVCurvatureAtMinimumLateralAccelerationPerSpeed,
                    GGVMinimumLateralAccelerationPerSpeed,
                    out alglib.spline1dinterpolant minLateralAccelerationInterp);
                // Interpolates the curves to get the accelerations
                currentLongitudinalAcceleration = alglib.spline1dcalc(longitudinalAccelerationForMinLateralInterp, curvature);
                currentLateralAcceleration = alglib.spline1dcalc(minLateralAccelerationInterp, curvature);
            }
            else
            {
                // Gets the interpolation objects (Longitudinal acceleration and lateral acceleration)
                alglib.spline1dbuildlinear(GGVCurvatureAtMaximumLateralAccelerationPerSpeed,
                    GGVLongitudinalAccelerationAtMaximumLateralAccelerationPerSpeed,
                    out alglib.spline1dinterpolant longitudinalAccelerationForMaxLateralInterp);
                alglib.spline1dbuildlinear(GGVCurvatureAtMaximumLateralAccelerationPerSpeed,
                    GGVMaximumLateralAccelerationPerSpeed,
                    out alglib.spline1dinterpolant maxLateralAccelerationInterp);
                // Interpolates the curves to get the accelerations
                currentLongitudinalAcceleration = alglib.spline1dcalc(longitudinalAccelerationForMaxLateralInterp, curvature);
                currentLateralAcceleration = alglib.spline1dcalc(maxLateralAccelerationInterp, curvature);
            }
            // Current speed
            double currentSpeed;
            if (curvature == 0) currentSpeed = SimulationGGVDiagram.Car.HighestSpeed;
            else currentSpeed = Math.Sqrt(currentLateralAcceleration * SimulationGGVDiagram.Car.Inertia.Gravity / curvature) * 3.6;
            // Current gear
            int currentGear = SimulationGGVDiagram.Car.GetGearNumberFromCarSpeed(currentSpeed);
            // Registers the parameters in the lists
            PathMaximumPossibleSpeeds.Add(currentSpeed);
            PathLateralAccelerationsForMaximumPossibleSpeeds.Add(currentLateralAcceleration);
            PathLongitudinalAccelerationsForMaximumPossibleSpeeds.Add(currentLongitudinalAcceleration);
            PathGearsForMaximumPossibleSpeeds.Add(currentGear);
        }
        private void GetPathMaximumPossibleSpeedsByExtrapolation(double curvature)
        {
            // Current accelerations initialization
            double currentLongitudinalAcceleration;
            double currentLateralAcceleration;
            // Selects the values of the accelerations based on the curvature sign and if it is higher or lower than the GGV's limit accelerations
            if (curvature < 0 && curvature <= GGVCurvatureAtMinimumLateralAccelerationPerSpeed.Min())
            {
                currentLongitudinalAcceleration = GGVLongitudinalAccelerationAtMinimumLateralAccelerationPerSpeed[SimulationGGVDiagram.AmountOfSpeeds - 1];
                currentLateralAcceleration = GGVMinimumLateralAccelerationPerSpeed[SimulationGGVDiagram.AmountOfSpeeds - 1];
            }
            else if (curvature < 0 && curvature >= GGVCurvatureAtMinimumLateralAccelerationPerSpeed.Max())
            {
                currentLongitudinalAcceleration = GGVLongitudinalAccelerationAtMinimumLateralAccelerationPerSpeed[0];
                currentLateralAcceleration = GGVMinimumLateralAccelerationPerSpeed[0];
            }
            else if (curvature > 0 && curvature <= GGVCurvatureAtMaximumLateralAccelerationPerSpeed.Min())
            {
                currentLongitudinalAcceleration = GGVLongitudinalAccelerationAtMaximumLateralAccelerationPerSpeed[0];
                currentLateralAcceleration = GGVMaximumLateralAccelerationPerSpeed[0];
            }
            else if (curvature > 0 && curvature >= GGVCurvatureAtMaximumLateralAccelerationPerSpeed.Max())
            {
                currentLongitudinalAcceleration = GGVLongitudinalAccelerationAtMaximumLateralAccelerationPerSpeed[SimulationGGVDiagram.AmountOfSpeeds - 1];
                currentLateralAcceleration = GGVMaximumLateralAccelerationPerSpeed[SimulationGGVDiagram.AmountOfSpeeds - 1];
            }
            else
            {
                currentLongitudinalAcceleration = 0;
                currentLateralAcceleration = 0;
            }
            // Gets the maximum possible speed for the current point
            double currentSpeed;
            if (curvature == 0) currentSpeed = SimulationGGVDiagram.Car.HighestSpeed;
            else currentSpeed = Math.Sqrt(currentLateralAcceleration * SimulationGGVDiagram.Car.Inertia.Gravity / curvature) * 3.6;
            // Current gear
            int currentGear = SimulationGGVDiagram.Car.GetGearNumberFromCarSpeed(currentSpeed);
            // Registers the parameters in the lists
            PathMaximumPossibleSpeeds.Add(currentSpeed);
            PathLateralAccelerationsForMaximumPossibleSpeeds.Add(currentLateralAcceleration);
            PathLongitudinalAccelerationsForMaximumPossibleSpeeds.Add(currentLongitudinalAcceleration);
            PathGearsForMaximumPossibleSpeeds.Add(currentGear);
        }
        #endregion
        #endregion

        private void GetPathSpeeds()
        {
            // Dynamic state initial guess
            PathSpeeds = new List<double>(PathMaximumPossibleSpeeds);
            PathLongitudinalAccelerations = new List<double>(PathLongitudinalAccelerationsForMaximumPossibleSpeeds);
            PathLateralAccelerations = new List<double>(PathLateralAccelerationsForMaximumPossibleSpeeds);
            PathGearNumbers = new List<int> (PathGearsForMaximumPossibleSpeeds);
            // Applies the braking limitation to the path
            ApplyBrakingLimitations();
            // Applies the accelerating limitation to the path
            ApplyAcceleratingLimitations();
        }
        private void ApplyBrakingLimitations()
        {
            // Checks if the simulation mode is set to first lap to determine the first point to apply the braking limitation
            int firstAnalysisPointIndex;
            if (IsFirstLap) firstAnalysisPointIndex = SimulationPath.AmountOfPointsInPath - 2; // Skips the last point of the path
            else firstAnalysisPointIndex = SimulationPath.AmountOfPointsInPath - 1; // Starts at the last point of the path
            // Path points sweep
            for (int iPoint = firstAnalysisPointIndex; iPoint >= 0; iPoint--)
            {
                // Determines the index of the reference point
                int iReferencePoint;
                if (iPoint == 0) iReferencePoint = SimulationPath.AmountOfPointsInPath - 1; // Chooses the last point as the reference point
                else iReferencePoint = iPoint - 1; // Chooses the previous point as the reference point
                // Checks if the maximum possible speed at the reference point is equal or higher the one of the current point
                if (PathSpeeds[iReferencePoint] >= PathSpeeds[iPoint])
                {
                    // Updates the point dynamic state
                    UpdateCurrentPointDynamicStateForCarLimitation(iPoint, "Braking");
                }
            }
            // Checks if it is a normal lap and continues to apply the limitation so that there's a continuous behaviour
            if (!IsFirstLap)
            {
                // Current point index
                int iPoint = SimulationPath.AmountOfPointsInPath;
                int iReferencePoint;
                do
                {
                    // Point index update
                    iPoint--;
                    // Gets the reference point index
                    if (iPoint == 0) iReferencePoint = SimulationPath.AmountOfPointsInPath - 1; // Chooses the last point as the reference point
                    else iReferencePoint = iPoint - 1; // Chooses the previous point as the reference point
                    // Updates the point dynamic state
                    UpdateCurrentPointDynamicStateForCarLimitation(iPoint, "Braking");
                } while (PathSpeeds[iReferencePoint] >= PathSpeeds[iPoint]);
            }
        }
        private void ApplyAcceleratingLimitations()
        {
            // Checks if the simulation mode is set to first lap and adjusts the first point dynamic state in this case.
            int firstAnalysisPointIndex;
            if (IsFirstLap)
            {
                // Sets the first analysis point index value
                firstAnalysisPointIndex = 1;
                // Adjusts the first path point's dynamic state parameters
                PathSpeeds[0] = 0;
                PathLongitudinalAccelerations[0] = SimulationGGVDiagram.GGDiagrams[0].LongitudinalAccelerations.Max();
                PathLateralAccelerations[0] = 0;
                PathGearNumbers[0] = 1;
            }
            else firstAnalysisPointIndex = 0;
            // Gear shifting variables initialization
            bool isGearShifting = false;
            double gearShiftingElapsedTime = 0;
            int lastGear = PathGearNumbers[0];
            // Path points sweep
            for (int iPoint = firstAnalysisPointIndex; iPoint < SimulationPath.AmountOfPointsInPath; iPoint++)
            {
                // Determines the index of the reference point
                int iReferencePoint;
                if (iPoint == 0) iReferencePoint = SimulationPath.AmountOfPointsInPath - 1; // Chooses the last point as the reference point
                else iReferencePoint = iPoint - 1; // Chooses the previous point as the reference point
                // Checks if the maximum possible speed at the reference point is equal or higher the one of the current point
                if (PathSpeeds[iReferencePoint] <= PathSpeeds[iPoint])
                {
                    // Updates the point dynamic state
                    if (!isGearShifting) UpdateCurrentPointDynamicStateForCarLimitation(iPoint, "Accelerating");
                    // Updates the gear shifting indicator
                    if (PathGearNumbers[iPoint] > lastGear && PathGearNumbers[iReferencePoint] != 0)
                    {
                        lastGear = PathGearNumbers[iPoint];
                        isGearShifting = true;
                        gearShiftingElapsedTime = 0;
                    }
                    // Updates the current dynamic state for the gear shifting situation
                    if (isGearShifting)
                    {
                        // Lists update
                        PathSpeeds[iPoint] = PathSpeeds[iReferencePoint];
                        PathLongitudinalAccelerations[iPoint] = 0;
                        PathLateralAccelerations[iPoint] = PathLateralAccelerations[iReferencePoint];
                        PathGearNumbers[iPoint] = 0;
                        // Gear shifting elapsed time update
                        gearShiftingElapsedTime += ((double)SimulationPath.Resolution / 1000) / (PathSpeeds[iReferencePoint] / 3.6);
                        // Checks if the gear shifting is over
                        if (gearShiftingElapsedTime >= SimulationGGVDiagram.Car.Transmission.GearShiftTime) isGearShifting = false;
                    }
                }
                else lastGear = PathGearNumbers[iPoint];
            }
            // Checks if it is a normal lap and continues to apply the limitation so that there's a continuous behaviour
            if (!IsFirstLap)
            {
                // Current point index
                int iPoint = -1;
                int iReferencePoint;
                do
                {
                    // Point index update
                    iPoint++;
                    // Gets the reference point index
                    if (iPoint == 0) iReferencePoint = SimulationPath.AmountOfPointsInPath - 1; // Chooses the last point as the reference point
                    else iReferencePoint = iPoint - 1; // Chooses the previous point as the reference point
                    // Updates the point dynamic state
                    if (!isGearShifting) UpdateCurrentPointDynamicStateForCarLimitation(iPoint, "Accelerating");
                    // Updates the gear shifting indicator
                    if (PathGearNumbers[iPoint] > lastGear && PathGearNumbers[iReferencePoint] != 0)
                    {
                        lastGear = PathGearNumbers[iPoint];
                        isGearShifting = true;
                        gearShiftingElapsedTime = 0;
                    }
                    // Updates the current dynamic state for the gear shifting situation
                    if (isGearShifting)
                    {
                        // Lists update
                        PathSpeeds[iPoint] = PathSpeeds[iReferencePoint];
                        PathLongitudinalAccelerations[iPoint] = 0;
                        PathLateralAccelerations[iPoint] = PathLateralAccelerations[iReferencePoint];
                        PathGearNumbers[iPoint] = 0;
                        // Gear shifting elapsed time update
                        gearShiftingElapsedTime += ((double)SimulationPath.Resolution / 1000) / (PathSpeeds[iReferencePoint] / 3.6);
                        // Checks if the gear shifting is over
                        if (gearShiftingElapsedTime >= SimulationGGVDiagram.Car.Transmission.GearShiftTime) isGearShifting = false;
                    }
                } while (PathSpeeds[iReferencePoint] >= PathSpeeds[iPoint]);
            }
        }

        private void UpdateCurrentPointDynamicStateForCarLimitation(int iPoint, string limitationMode)
        {
            // Index of the reference point
            int iReferencePoint;
            double currentSpeed;
            if (limitationMode == "Braking")
            {
                if (iPoint == SimulationPath.AmountOfPointsInPath - 1) iReferencePoint = 0;
                else iReferencePoint = iPoint + 1;
                // Reference point speed and longitudinal acceleration
                double referenceSpeed = PathSpeeds[iReferencePoint] / 3.6;
                double referenceLongitudinalAcceleration = PathLongitudinalAccelerations[iReferencePoint] * SimulationGGVDiagram.Car.Inertia.Gravity;
                // Current speed
                currentSpeed = Math.Sqrt(Math.Pow(referenceSpeed, 2) - 2 * referenceLongitudinalAcceleration * (double)SimulationPath.Resolution / 1000);
            }
            else
            {
                if (iPoint == 0) iReferencePoint = SimulationPath.AmountOfPointsInPath - 1;
                else iReferencePoint = iPoint - 1;
                // Reference point speed and longitudinal acceleration
                double referenceSpeed = PathSpeeds[iReferencePoint] / 3.6;
                double referenceLongitudinalAcceleration = PathLongitudinalAccelerations[iReferencePoint] * SimulationGGVDiagram.Car.Inertia.Gravity;
                // Current speed
                currentSpeed = Math.Sqrt(Math.Pow(referenceSpeed, 2) + 2 * referenceLongitudinalAcceleration * (double)SimulationPath.Resolution / 1000);
            }
            // Checks if the calculated current speed is higher than the maximum possible speed at this point and corrects it if this is the case
            if (currentSpeed > PathMaximumPossibleSpeeds[iPoint] / 3.6) currentSpeed = PathMaximumPossibleSpeeds[iPoint] / 3.6;
            // Gets the current GG diagram based on interpolation by the speed
            GGDiagram interpolatedGGDiagram = SimulationGGVDiagram.GetGGDiagramFromInterpolationBySpeed(currentSpeed * 3.6, SimulationGGVDiagram.Car);
            // Current point's local curvature
            double currentCurvature = SimulationPath.LocalCurvatures[iPoint];
            // Current point's lateral acceleration
            double currentLateralAcceleration = Math.Pow(currentSpeed, 2) * currentCurvature / SimulationGGVDiagram.Car.Inertia.Gravity;
            // Gets the longitudinal acceleration via interpolation based on the lateral acceleration
            double currentLongitudinalAcceleration = interpolatedGGDiagram.GetLongitudinalAccelerationViaInterpolationBasedOnLateralAcceleration
                (currentLateralAcceleration, limitationMode);
            // Registers the values in the lists
            PathSpeeds[iPoint] = currentSpeed * 3.6;
            PathLongitudinalAccelerations[iPoint] = currentLongitudinalAcceleration;
            PathLateralAccelerations[iPoint] = currentLateralAcceleration;
            PathGearNumbers[iPoint] = SimulationGGVDiagram.Car.GetGearNumberFromCarSpeed(currentSpeed * 3.6);
        }
    }
}
