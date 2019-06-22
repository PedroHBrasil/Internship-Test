using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace InternshipTest.Simulation
{
    /// <summary>
    /// Contains the lap time simulation solver and basic results.
    /// </summary>
    [Serializable]
    public class LapTimeSimulation : GenericInfo
    {
        #region Fields
        public int progressCounter;
        #endregion
        #region Properties
        /// <summary>
        /// Simulation's path to be used.
        /// </summary>
        public Path Path { get; set; }
        /// <summary>
        /// List of GGV diagrams to be used per path sector.
        /// </summary>
        public List<LapTimeSimulationSectorSetup> GGVDiagramsPerSector { get; set; }
        /// <summary>
        /// Indicates the lap mode (first lap starts from speed = 0 and normal lap makes the speed at the end = speed at the beginning).
        /// </summary>
        public bool IsFirstLap { get; set; }
        public Results.LapTimeSimulationResults Results { get; set; }
        #endregion

        #region Constructors
        public LapTimeSimulation() { }
        public LapTimeSimulation(string id, string description, Path path, List<LapTimeSimulationSectorSetup> ggvDiagrams, bool isFirstLap)
        {
            ID = id;
            Description = description;
            Path = path;
            GGVDiagramsPerSector = ggvDiagrams;
            IsFirstLap = isFirstLap;
        }
        #endregion

        #region Methods

        #region Public
        /// <summary>
        /// Runs the lap time simulation.
        /// </summary>
        /// <returns> The lap time simulaton results object. </returns>
        public void RunLapTimeSimulation(object sender, DoWorkEventArgs e)
        {
            // Gets the maximum possible speeds at the path points, based on the lateral acceleration limitations.
            Results.LapTimeSimulationResults results = GetDynamicStatesBasedOnLateralAccelerationLimits(sender);
            // Gets the final dynamic states at the path points, based on the longitudinal acceleration limitations.
            results = GetFinalDynamicStates(sender, results);
            results.ID = ID;
            results.Description = Description;
            results.GGVDiagramsPerSector = GGVDiagramsPerSector;
            results.LocalSectors = Path.LocalSectorIndex.ToArray();
            results.GetAllResults();
            Results = results;
            e.Result = this;
        }
        /// <summary>
        /// Gets the resulting lp ime simulation results from the current thread and assigns it to the object of the main thread.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        #endregion

        #region Private/Protected
        #region To Apply the Lateral Acceleration Limitations
        /// <summary>
        /// Gets the car's dynamic states along the path based on the lateral acceleration limitations and the interpolation curves to be used in the process.
        /// </summary>
        /// <returns> The lap time simulaton results. </returns>
        protected Results.LapTimeSimulationResults GetDynamicStatesBasedOnLateralAccelerationLimits(object sender)
        {
            // Gets the interpolation curves to be used in the determination of the dynamic states associated with  the maximum possible speeds at the path
            List<Dictionary<string, double[]>> interpolationCurvesDictionaries = new List<Dictionary<string, double[]>>();
            for (int iSector = 0; iSector < GGVDiagramsPerSector.Count(); iSector++)
                interpolationCurvesDictionaries.Add(_GetInterpolationCurvesFromGGVDiagram(GGVDiagramsPerSector[iSector].SectorGGVDiagram));
            // Gets the dynamic states associated with the maximum possible speeds
            Results.LapTimeSimulationResults results = _GetDynamicStatesAssociatedWithTheMaximumPossibleSpeeds(sender, interpolationCurvesDictionaries);
            return results;
        }
        /// <summary>
        /// Gets the curves to be used in the interpolations to get the dynamic statesbased on thelateral acceleration limitations.
        /// </summary>
        /// <param name="ggvDiagram"> Current sector's GGV diagram. </param>
        /// <returns> A dictionary which contains the intrpolation curves. </returns>
        private Dictionary<string, double[]> _GetInterpolationCurvesFromGGVDiagram(GGVDiagram ggvDiagram)
        {
            // Initializes the arrays to use in the determination of the maximum speeds profile
            double[] GGVLongitudinalAccelerationAtMinimumLateralAccelerationPerSpeed = new double[ggvDiagram.Speeds.Count()];
            double[] GGVLongitudinalAccelerationAtMaximumLateralAccelerationPerSpeed = new double[ggvDiagram.Speeds.Count()];
            double[] GGVMinimumLateralAccelerationPerSpeed = new double[ggvDiagram.Speeds.Count()];
            double[] GGVMaximumLateralAccelerationPerSpeed = new double[ggvDiagram.Speeds.Count()];
            double[] GGVCurvatureAtMinimumLateralAccelerationPerSpeed = new double[ggvDiagram.Speeds.Count()];
            double[] GGVCurvatureAtMaximumLateralAccelerationPerSpeed = new double[ggvDiagram.Speeds.Count()];
            // Gets the GGV maximum and minimum parameters per speed
            for (int iSpeed = 0; iSpeed < ggvDiagram.Speeds.Count(); iSpeed++)
            {
                // Gets and registers the longitudinal accelerations
                GGVLongitudinalAccelerationAtMinimumLateralAccelerationPerSpeed[iSpeed] = ggvDiagram.GGDiagrams[iSpeed].LongitudinalAccelerations.Min();
                GGVLongitudinalAccelerationAtMaximumLateralAccelerationPerSpeed[iSpeed] = ggvDiagram.GGDiagrams[iSpeed].LongitudinalAccelerations.Max();
                // Gets and registers the lateral accelerations
                GGVMinimumLateralAccelerationPerSpeed[iSpeed] = ggvDiagram.GGDiagrams[iSpeed].LateralAccelerations.Min();
                GGVMaximumLateralAccelerationPerSpeed[iSpeed] = ggvDiagram.GGDiagrams[iSpeed].LateralAccelerations.Max();
                // Gets and registers the curvatures
                GGVCurvatureAtMinimumLateralAccelerationPerSpeed[iSpeed] = ggvDiagram.GGDiagrams[iSpeed].Curvatures.Min();
                GGVCurvatureAtMaximumLateralAccelerationPerSpeed[iSpeed] = ggvDiagram.GGDiagrams[iSpeed].Curvatures.Max();
            }
            // Initializes the dictionay and adds the curves to it
            Dictionary<string, double[]> interpolationCurves = new Dictionary<string, double[]>
            {
                { "GGVLongitudinalAccelerationAtMinimumLateralAccelerationPerSpeed", GGVLongitudinalAccelerationAtMinimumLateralAccelerationPerSpeed },
                { "GGVLongitudinalAccelerationAtMaximumLateralAccelerationPerSpeed", GGVLongitudinalAccelerationAtMaximumLateralAccelerationPerSpeed },
                { "GGVMinimumLateralAccelerationPerSpeed", GGVMinimumLateralAccelerationPerSpeed },
                { "GGVMaximumLateralAccelerationPerSpeed", GGVMaximumLateralAccelerationPerSpeed },
                { "GGVCurvatureAtMinimumLateralAccelerationPerSpeed", GGVCurvatureAtMinimumLateralAccelerationPerSpeed },
                { "GGVCurvatureAtMaximumLateralAccelerationPerSpeed", GGVCurvatureAtMaximumLateralAccelerationPerSpeed }
            };

            return interpolationCurves;
        }
        /// <summary>
        /// Gets the car's dynamic states along the path based on the lateral acceleration limitations.
        /// </summary>
        /// <param name="interpolationCurvesDictionaries"> Curves sets to be interpolated in the processing. </param>
        /// <returns> The lap time simulaton results initial guess. </returns>
        private Results.LapTimeSimulationResults _GetDynamicStatesAssociatedWithTheMaximumPossibleSpeeds(object sender, List<Dictionary<string, double[]>> interpolationCurvesDictionaries)
        {
            Results.LapTimeSimulationResults results = new Results.LapTimeSimulationResults(Path);
            // Path points "for" loop
            for (int iPathPoint = 0; iPathPoint < Path.AmountOfPointsInPath; iPathPoint++)
            {
                // Selects the current interpolation curves set to be used
                Dictionary<string, double[]> currentInterpolationCurves = interpolationCurvesDictionaries[Path.LocalSectorIndex[iPathPoint] - 1];
                // Current path point's local curvature
                double currentCurvature = Path.LocalCurvatures[iPathPoint];
                // Evaluates if the local curvature is in the GGV diagram range or not
                bool isCurvatureInRange = _CheckIfCurvatureIsInGGVRange(currentCurvature, currentInterpolationCurves);
                // Selects the current GGV diagram
                GGVDiagram currentGGVDiagram = GGVDiagramsPerSector[Path.LocalSectorIndex[iPathPoint] - 1].SectorGGVDiagram;
                // Gets the current point maximum speed and associated parameters
                Results.LapTimeSimulationResults resultsForCurrentPoint;
                if (isCurvatureInRange) resultsForCurrentPoint = _GetPathDynamicStatesForMaximumPossibleSpeedsByInterpolation(currentCurvature, currentInterpolationCurves, currentGGVDiagram);
                else resultsForCurrentPoint = _GetPathDynamicStatesForMaximumPossibleSpeedsByExtrapolation(currentCurvature, currentInterpolationCurves, currentGGVDiagram);
                // Adds the current point results to the complete set of results
                results.MaximumPossibleSpeeds[iPathPoint] = resultsForCurrentPoint.MaximumPossibleSpeeds[0];
                results.LongitudinalAccelerationsForMaximumPossibleSpeeds[iPathPoint] = resultsForCurrentPoint.LongitudinalAccelerationsForMaximumPossibleSpeeds[0];
                results.LateralAccelerationsForMaximumPossibleSpeeds[iPathPoint] = resultsForCurrentPoint.LateralAccelerationsForMaximumPossibleSpeeds[0];
                results.GearNumbersForMaximumPossibleSpeeds[iPathPoint] = resultsForCurrentPoint.GearNumbersForMaximumPossibleSpeeds[0];
                // Progress counter update and report
                progressCounter++;
                int progress = (int)((double)(progressCounter + 1) / (Path.AmountOfPointsInPath * 3) * 100);
                (sender as BackgroundWorker).ReportProgress(progress);
            }
            return results;
        }
        /// <summary>
        /// Checks if the current path curvature is in the range of curvatures covered by the current GGV diagram.
        /// </summary>
        /// <param name="curvature"> Path curvature [1/m] </param>
        /// <param name="interpolationCurves"> Curves used in the current interpolation. </param>
        /// <returns> A boolean which indicates if the curvature is in range or not. </returns>
        private bool _CheckIfCurvatureIsInGGVRange(double curvature, Dictionary<string, double[]> interpolationCurves)
        {
            // Checks if the curvature is out of the GGV range (for limit lateral accelerations)
            if ((curvature < 0 &&
                (curvature < interpolationCurves["GGVCurvatureAtMinimumLateralAccelerationPerSpeed"].Min() ||
                curvature > interpolationCurves["GGVCurvatureAtMinimumLateralAccelerationPerSpeed"].Max())) ||
                (curvature > 0 &&
                (curvature < interpolationCurves["GGVCurvatureAtMaximumLateralAccelerationPerSpeed"].Min() ||
                curvature > interpolationCurves["GGVCurvatureAtMaximumLateralAccelerationPerSpeed"].Max())))
                return false;
            else return true;
        }
        /// <summary>
        /// Gets the car's dynamic states along the path based on the lateral acceleration limitations by interpolation.
        /// </summary>
        /// <param name="curvature"> Path's local curvature [1/m] </param>
        /// <param name="interpolationCurves"> Curves to be used in the interpolation. </param>
        /// <param name="ggvDiagram"> GGV diagram used in the interpolation </param>
        /// <returns> The lap time simulaton results initial guess. </returns>
        private Results.LapTimeSimulationResults _GetPathDynamicStatesForMaximumPossibleSpeedsByInterpolation(double curvature, Dictionary<string, double[]> interpolationCurves, GGVDiagram ggvDiagram)
        {
            // Current accelerations initialization
            double currentLongitudinalAcceleration;
            double currentLateralAcceleration;
            // Current accelerations values by interpolation
            if (curvature < 0)
            {
                // Gets the interpolation objects (Longitudinal acceleration and lateral acceleration)
                alglib.spline1dbuildlinear(interpolationCurves["GGVCurvatureAtMinimumLateralAccelerationPerSpeed"],
                    interpolationCurves["GGVLongitudinalAccelerationAtMinimumLateralAccelerationPerSpeed"],
                    out alglib.spline1dinterpolant longitudinalAccelerationForMinLateralInterp);
                alglib.spline1dbuildlinear(interpolationCurves["GGVCurvatureAtMinimumLateralAccelerationPerSpeed"],
                    interpolationCurves["GGVMinimumLateralAccelerationPerSpeed"],
                    out alglib.spline1dinterpolant minLateralAccelerationInterp);
                // Interpolates the curves to get the accelerations
                currentLongitudinalAcceleration = alglib.spline1dcalc(longitudinalAccelerationForMinLateralInterp, curvature);
                currentLateralAcceleration = alglib.spline1dcalc(minLateralAccelerationInterp, curvature);
            }
            else
            {
                // Gets the interpolation objects (Longitudinal acceleration and lateral acceleration)
                alglib.spline1dbuildlinear(interpolationCurves["GGVCurvatureAtMaximumLateralAccelerationPerSpeed"],
                    interpolationCurves["GGVLongitudinalAccelerationAtMaximumLateralAccelerationPerSpeed"],
                    out alglib.spline1dinterpolant longitudinalAccelerationForMaxLateralInterp);
                alglib.spline1dbuildlinear(interpolationCurves["GGVCurvatureAtMaximumLateralAccelerationPerSpeed"],
                    interpolationCurves["GGVMaximumLateralAccelerationPerSpeed"],
                    out alglib.spline1dinterpolant maxLateralAccelerationInterp);
                // Interpolates the curves to get the accelerations
                currentLongitudinalAcceleration = alglib.spline1dcalc(longitudinalAccelerationForMaxLateralInterp, curvature);
                currentLateralAcceleration = alglib.spline1dcalc(maxLateralAccelerationInterp, curvature);
            }
            // Current speed
            double currentSpeed;
            if (curvature == 0) currentSpeed = ggvDiagram.GetCarHighestSpeed();
            else currentSpeed = Math.Sqrt(currentLateralAcceleration / curvature);
            // Current gear
            int currentGear = ggvDiagram.GetGearNumberFromCarSpeed(currentSpeed);
            // Registers the results and return them
            Results.LapTimeSimulationResults results = new Results.LapTimeSimulationResults()
            {
                MaximumPossibleSpeeds = new double[] { currentSpeed },
                LongitudinalAccelerationsForMaximumPossibleSpeeds = new double[] { currentLongitudinalAcceleration },
                LateralAccelerationsForMaximumPossibleSpeeds = new double[] { currentLateralAcceleration },
                GearNumbersForMaximumPossibleSpeeds = new int[] { currentGear }
            };
            return results;
        }
        /// <summary>
        /// Gets the car's dynamic states along the path based on the lateral acceleration limitations by extrapolation.
        /// </summary>
        /// <param name="curvature"> Path's local curvature [1/m] </param>
        /// <param name="interpolationCurves"> Curves to be used in the interpolation. </param>
        /// <param name="ggvDiagram"> GGV diagram used in the interpolation </param>
        /// <returns> The lap time simulaton results initial guess. </returns>
        private Results.LapTimeSimulationResults _GetPathDynamicStatesForMaximumPossibleSpeedsByExtrapolation(double curvature, Dictionary<string, double[]> interpolationCurves, GGVDiagram ggvDiagram)
        {
            // Current accelerations initialization
            double currentLongitudinalAcceleration;
            double currentLateralAcceleration;
            // Selects the values of the accelerations based on the curvature sign and if it is higher or lower than the GGV's limit accelerations
            if (curvature < 0 && curvature <= interpolationCurves["GGVCurvatureAtMinimumLateralAccelerationPerSpeed"].Min())
            {
                currentLongitudinalAcceleration = interpolationCurves["GGVLongitudinalAccelerationAtMinimumLateralAccelerationPerSpeed"][ggvDiagram.AmountOfSpeeds - 1];
                currentLateralAcceleration = interpolationCurves["GGVMinimumLateralAccelerationPerSpeed"][ggvDiagram.AmountOfSpeeds - 1];
            }
            else if (curvature < 0 && curvature >= interpolationCurves["GGVCurvatureAtMinimumLateralAccelerationPerSpeed"].Max())
            {
                currentLongitudinalAcceleration = interpolationCurves["GGVLongitudinalAccelerationAtMinimumLateralAccelerationPerSpeed"][0];
                currentLateralAcceleration = interpolationCurves["GGVMinimumLateralAccelerationPerSpeed"][0];
            }
            else if (curvature > 0 && curvature <= interpolationCurves["GGVCurvatureAtMaximumLateralAccelerationPerSpeed"].Min())
            {
                currentLongitudinalAcceleration = interpolationCurves["GGVLongitudinalAccelerationAtMaximumLateralAccelerationPerSpeed"][0];
                currentLateralAcceleration = interpolationCurves["GGVMaximumLateralAccelerationPerSpeed"][0];
            }
            else if (curvature > 0 && curvature >= interpolationCurves["GGVCurvatureAtMaximumLateralAccelerationPerSpeed"].Max())
            {
                currentLongitudinalAcceleration = interpolationCurves["GGVLongitudinalAccelerationAtMaximumLateralAccelerationPerSpeed"][ggvDiagram.AmountOfSpeeds - 1];
                currentLateralAcceleration = interpolationCurves["GGVMaximumLateralAccelerationPerSpeed"][ggvDiagram.AmountOfSpeeds - 1];
            }
            else
            {
                currentLongitudinalAcceleration = 0;
                currentLateralAcceleration = 0;
            }
            // Gets the maximum possible speed for the current point
            double currentSpeed;
            if (curvature == 0) currentSpeed = ggvDiagram.GetCarHighestSpeed();
            else currentSpeed = Math.Sqrt(currentLateralAcceleration / curvature);
            // Current gear
            int currentGear = ggvDiagram.GetGearNumberFromCarSpeed(currentSpeed);
            // Registers the results and return them
            Results.LapTimeSimulationResults results = new Results.LapTimeSimulationResults()
            {
                MaximumPossibleSpeeds = new double[] { currentSpeed },
                LongitudinalAccelerationsForMaximumPossibleSpeeds = new double[] { currentLongitudinalAcceleration },
                LateralAccelerationsForMaximumPossibleSpeeds = new double[] { currentLateralAcceleration },
                GearNumbersForMaximumPossibleSpeeds = new int[] { currentGear }
            };
            return results;
        }
        #endregion
        #region To Apply the Longitudinal Acceleration Limitations
        /// <summary>
        /// Gets the final dynamic states based on the longitudinal acceleration limitations.
        /// </summary>
        /// <param name="results"> Results initial guess (from the lateral acceleration limitations) </param>
        /// <returns> Final results </returns>
        protected Results.LapTimeSimulationResults GetFinalDynamicStates(object sender, Results.LapTimeSimulationResults results)
        {
            // Uses the previously calculated dynamic states as the initial guess for the final states
            for (int iPathPoint = 0; iPathPoint < Path.AmountOfPointsInPath; iPathPoint++)
            {
                results.Speeds[iPathPoint] = results.MaximumPossibleSpeeds[iPathPoint];
                results.LongitudinalAccelerations[iPathPoint] = results.LongitudinalAccelerationsForMaximumPossibleSpeeds[iPathPoint];
                results.LateralAccelerations[iPathPoint] = results.LateralAccelerationsForMaximumPossibleSpeeds[iPathPoint];
                results.GearNumbers[iPathPoint] = results.GearNumbersForMaximumPossibleSpeeds[iPathPoint];
            }
            // Applies the braking limitations
            results = _ApplyBrakingLimitations(sender, results);
            // Applies the accelerating limitations
            results = _ApplyAcceleratingLimitations(sender, results);

            return results;
        }
        /// <summary>
        /// Applies the braking limitations to the dynamic states.
        /// </summary>
        /// <param name="results"> Results initial guess (from the lateral acceleration limitations) </param>
        /// <returns> Results after appliance of the braking limitations. </returns>
        private Results.LapTimeSimulationResults _ApplyBrakingLimitations(object sender, Results.LapTimeSimulationResults results)
        {
            // Checks if the simulation mode is set to first lap to determine the first point to apply the braking limitation
            int firstAnalysisPointIndex;
            if (IsFirstLap) firstAnalysisPointIndex = Path.AmountOfPointsInPath - 2; // Skips the last point of the path
            else firstAnalysisPointIndex = Path.AmountOfPointsInPath - 1; // Starts at the last point of the path
            // Path points sweep
            for (int iPoint = firstAnalysisPointIndex; iPoint >= 0; iPoint--)
            {
                // Determines the index of the reference point
                int iReferencePoint;
                if (iPoint == 0) iReferencePoint = Path.AmountOfPointsInPath - 1; // Chooses the last point as the reference point
                else iReferencePoint = iPoint - 1; // Chooses the previous point as the reference point
                // Checks if the maximum possible speed at the reference point is equal or higher the one of the current point
                if (results.Speeds[iReferencePoint] >= results.Speeds[iPoint])
                {
                    // Updates the point dynamic state
                    results = _UpdateCurrentPointDynamicStateForCarLimitation(iPoint, "Braking", results);
                }
                // Progress counter update and report
                progressCounter++;
                int progress = (int)((double)(progressCounter + 1) / (Path.AmountOfPointsInPath * 3) * 100);
                (sender as BackgroundWorker).ReportProgress(progress);
            }
            // Checks if it is a normal lap and continues to apply the limitation so that there's a continuous behaviour
            if (!IsFirstLap)
            {
                // Current point index
                int iPoint = Path.AmountOfPointsInPath;
                int iReferencePoint;
                do
                {
                    // Point index update
                    iPoint--;
                    // Gets the reference point index
                    if (iPoint == 0) iReferencePoint = Path.AmountOfPointsInPath - 1; // Chooses the last point as the reference point
                    else iReferencePoint = iPoint - 1; // Chooses the previous point as the reference point
                    // Updates the point dynamic state
                    results = _UpdateCurrentPointDynamicStateForCarLimitation(iPoint, "Braking", results);
                } while (results.Speeds[iReferencePoint] >= results.Speeds[iPoint]);
            }
            return results;
        }
        /// <summary>
        /// Applies the accelerating limitations to the dynamic states.
        /// </summary>
        /// <param name="results"> Results after appliance of the braking limitations. </param>
        /// <returns> Final results </returns>
        private Results.LapTimeSimulationResults _ApplyAcceleratingLimitations(object sender, Results.LapTimeSimulationResults results)
        {
            // Checks if the simulation mode is set to first lap and adjusts the first point dynamic state in this case.
            int firstAnalysisPointIndex;
            if (IsFirstLap)
            {
                // Sets the first analysis point index value
                firstAnalysisPointIndex = 1;
                // Adjusts the first path point's dynamic state parameters
                results.Speeds[0] = 0;
                results.LongitudinalAccelerations[0] = GGVDiagramsPerSector[0].SectorGGVDiagram.GGDiagrams[0].LongitudinalAccelerations.Max();
                results.LateralAccelerations[0] = 0;
                results.GearNumbers[0] = 1;
            }
            else firstAnalysisPointIndex = 0;
            // Gear shifting variables initialization
            bool isGearShifting = false;
            double gearShiftingElapsedTime = 0;
            int lastGear = results.GearNumbers[0];
            // Path points sweep
            for (int iPoint = firstAnalysisPointIndex; iPoint < Path.AmountOfPointsInPath; iPoint++)
            {
                // Determines the index of the reference point
                int iReferencePoint;
                if (iPoint == 0) iReferencePoint = Path.AmountOfPointsInPath - 1; // Chooses the last point as the reference point
                else iReferencePoint = iPoint - 1; // Chooses the previous point as the reference point
                // Checks if the maximum possible speed at the reference point is equal or higher the one of the current point
                if (results.Speeds[iReferencePoint] <= results.Speeds[iPoint])
                {
                    // Updates the point dynamic state
                    if (!isGearShifting) results = _UpdateCurrentPointDynamicStateForCarLimitation(iPoint, "Accelerating", results);
                    // Updates the gear shifting indicator
                    if (results.GearNumbers[iPoint] > lastGear && results.GearNumbers[iReferencePoint] != 0)
                    {
                        lastGear = results.GearNumbers[iPoint];
                        isGearShifting = true;
                        gearShiftingElapsedTime = 0;
                    }
                    // Updates the current dynamic state for the gear shifting situation
                    if (isGearShifting)
                    {
                        // Lists update
                        results.Speeds[iPoint] = results.Speeds[iReferencePoint];
                        results.LongitudinalAccelerations[iPoint] = 0;
                        results.LateralAccelerations[iPoint] = results.LateralAccelerations[iReferencePoint];
                        results.GearNumbers[iPoint] = 0;
                        // Gear shifting elapsed time update
                        gearShiftingElapsedTime += Path.Resolution / (results.Speeds[iReferencePoint]);
                        // Current sector index
                        int iCurrentSector = Path.LocalSectorIndex[iPoint] - 1;
                        // Checks if the gear shifting is over
                        if (gearShiftingElapsedTime >= GGVDiagramsPerSector[iCurrentSector].SectorGGVDiagram.GetGearShiftTime()) isGearShifting = false;
                    }
                }
                else lastGear = results.GearNumbers[iPoint];
                // Progress counter update and report
                progressCounter++;
                int progress = (int)((double)(progressCounter + 1) / (Path.AmountOfPointsInPath * 3) * 100);
                (sender as BackgroundWorker).ReportProgress(progress);
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
                    if (iPoint == 0) iReferencePoint = Path.AmountOfPointsInPath - 1; // Chooses the last point as the reference point
                    else iReferencePoint = iPoint - 1; // Chooses the previous point as the reference point
                    // Updates the point dynamic state
                    if (!isGearShifting) results = _UpdateCurrentPointDynamicStateForCarLimitation(iPoint, "Accelerating", results);
                    // Updates the gear shifting indicator
                    if (results.GearNumbers[iPoint] > lastGear && results.GearNumbers[iReferencePoint] != 0)
                    {
                        lastGear = results.GearNumbers[iPoint];
                        isGearShifting = true;
                        gearShiftingElapsedTime = 0;
                    }
                    // Updates the current dynamic state for the gear shifting situation
                    if (isGearShifting)
                    {
                        // Lists update
                        results.Speeds[iPoint] = results.Speeds[iReferencePoint];
                        results.LongitudinalAccelerations[iPoint] = 0;
                        results.LateralAccelerations[iPoint] = results.LateralAccelerations[iReferencePoint];
                        results.GearNumbers[iPoint] = 0;
                        // Gear shifting elapsed time update
                        gearShiftingElapsedTime += Path.Resolution / (results.Speeds[iReferencePoint]);
                        // Current sector index
                        int iCurrentSector = Path.LocalSectorIndex[iPoint] - 1;
                        // Checks if the gear shifting is over
                        if (gearShiftingElapsedTime >= GGVDiagramsPerSector[iCurrentSector].SectorGGVDiagram.GetGearShiftTime()) isGearShifting = false;
                    }
                } while (results.Speeds[iReferencePoint] >= results.Speeds[iPoint]);
            }
            return results;
        }
        /// <summary>
        /// Updates the dynamic state of a given point considering accelerating or braking situation.
        /// </summary>
        /// <param name="iPoint"> Index of the point to be updated </param>
        /// <param name="limitationMode"> "Braking" or "Accelerating" </param>
        /// <param name="results"> Results initial guess. </param>
        /// <returns> The results after the dynamic state update. </returns>
        private Results.LapTimeSimulationResults _UpdateCurrentPointDynamicStateForCarLimitation(int iPoint, string limitationMode, Results.LapTimeSimulationResults results)
        {
            int iReferencePoint;
            double currentSpeed;
            // Checks the limitation mode to be applied. Then, based on the mode, selects the appropriate reference point and calclates the speed of the current point.
            if (limitationMode == "Braking")
            {
                if (iPoint == Path.AmountOfPointsInPath - 1) iReferencePoint = 0;
                else iReferencePoint = iPoint + 1;
                // Reference point speed and longitudinal acceleration
                double referenceSpeed = results.Speeds[iReferencePoint];
                double referenceLongitudinalAcceleration = results.LongitudinalAccelerations[iReferencePoint];
                // Current speed
                currentSpeed = Math.Sqrt(Math.Pow(referenceSpeed, 2) - 2 * referenceLongitudinalAcceleration * Path.Resolution);
            }
            else
            {
                if (iPoint == 0) iReferencePoint = Path.AmountOfPointsInPath - 1;
                else iReferencePoint = iPoint - 1;
                // Reference point speed and longitudinal acceleration
                double referenceSpeed = results.Speeds[iReferencePoint];
                double referenceLongitudinalAcceleration = results.LongitudinalAccelerations[iReferencePoint];
                // Current speed
                currentSpeed = Math.Sqrt(Math.Pow(referenceSpeed, 2) + 2 * referenceLongitudinalAcceleration * Path.Resolution);
            }
            // Checks if the calculated current speed is higher than the maximum possible speed at this point and corrects it if this is the case
            if (currentSpeed > results.MaximumPossibleSpeeds[iPoint])
            {
                currentSpeed = results.MaximumPossibleSpeeds[iPoint];
            }

            // Selects the GGV diagram to be used in the interpolation (DRS?)
            int iSectorCurrentPoint = Path.LocalSectorIndex[iPoint] - 1;
            GGVDiagram ggvDiagram = GGVDiagramsPerSector[iSectorCurrentPoint].SectorGGVDiagram;
            // Gets the current GG diagram based on interpolation by the speed
            GGDiagram interpolatedGGDiagram = ggvDiagram.GetGGDiagramForASpeed(currentSpeed);
            // Current point's local curvature
            double currentCurvature = Path.LocalCurvatures[iPoint];
            // Current point's lateral acceleration
            double currentLateralAcceleration = Math.Pow(currentSpeed, 2) * currentCurvature;
            // Gets the longitudinal acceleration via interpolation based on the lateral acceleration
            double currentLongitudinalAcceleration;
            currentLongitudinalAcceleration = interpolatedGGDiagram.GetLongitudinalAccelerationViaInterpolationBasedOnLateralAcceleration(currentLateralAcceleration, limitationMode);
            if (limitationMode == "Braking" && currentLongitudinalAcceleration > 0) currentLongitudinalAcceleration = 0;
            else if (limitationMode == "Accelerating" && currentLongitudinalAcceleration < 0) currentLongitudinalAcceleration = 0;
            // Registers the values in the lists
            results.Speeds[iPoint] = currentSpeed;
            results.LongitudinalAccelerations[iPoint] = currentLongitudinalAcceleration;
            results.LateralAccelerations[iPoint] = currentLateralAcceleration;
            results.GearNumbers[iPoint] = ggvDiagram.GetGearNumberFromCarSpeed(currentSpeed);
            return results;
        }


        #endregion
        #endregion

        #endregion
    }
}
