using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternshipTest.Results
{
    /// <summary>
    /// Contains the results of a lap time simulation
    /// </summary>
    [Serializable]
    public class LapTimeSimulationResults : GenericInfo
    {
        private double lapTime;
        #region Properties
        #region Standard Array Results
        /// <summary>
        /// GGV Diarams used in the lap time simulation
        /// </summary>
        public List<Simulation.LapTimeSimulationSectorSetup> GGVDiagramsPerSector { get; set; }
        #region Cornering Limitation Only
        /// <summary>
        /// Speeds given only the lateral acceleration limitations [m/s]
        /// </summary>
        public double[] MaximumPossibleSpeeds { get; set; }
        /// <summary>
        /// Longitudinal acceleration given only the lateral acceleration limitations [m/s²]
        /// </summary>
        public double[] LongitudinalAccelerationsForMaximumPossibleSpeeds { get; set; }
        /// <summary>
        /// Lateral acceleration given only the lateral acceleration limitations [m/s²]
        /// </summary>
        public double[] LateralAccelerationsForMaximumPossibleSpeeds { get; set; }
        /// <summary>
        /// Gear numbers given only the lateral acceleration limitations
        /// </summary>
        public int[] GearNumbersForMaximumPossibleSpeeds { get; set; }
        #endregion
        #region Final Results
        /// <summary>
        /// Final speeds [m/s]
        /// </summary>
        public double[] Speeds { get; set; }
        /// <summary>
        /// Final longitudinal accelerations [m/s²]
        /// </summary>
        public double[] LongitudinalAccelerations { get; set; }
        /// <summary>
        /// Final lateral accelerations [m/s²]
        /// </summary>
        public double[] LateralAccelerations { get; set; }
        /// <summary>
        /// Final gear numbers.
        /// </summary>
        public int[] GearNumbers { get; set; }
        #endregion
        #endregion
        #region Path Related Results
        /// <summary>
        /// Sector of each path point.
        /// </summary>
        public int[] LocalSectors { get; set; }
        /// <summary>
        /// Path's resolution [m]
        /// </summary>
        private double PathResolution { get; set; }
        /// <summary>
        /// Path's local curvatures [1/m]
        /// </summary>
        public double[] PathCurvatures { get; set; }
        /// <summary>
        /// Path's X coordinates.
        /// </summary>
        public double[] CoordinatesX { get; set; }
        /// <summary>
        /// Path's Y coordinates.
        /// </summary>
        public double[] CoordinatesY { get; set; }
        #endregion
        #region Dynamic Outputs
        /// <summary>
        /// Local elapsed time [s]
        /// </summary>
        private double[] LocalElapsedTime { get; set; }
        /// <summary>
        /// Elapsed time [s].
        /// </summary>
        public double[] ElapsedTimes { get; set; }
        /// <summary>
        /// Elapsed distance [m]
        /// </summary>
        public double[] ElapsedDistances { get; set; }
        /// <summary>
        /// Car's longitudinal forces [N]
        /// </summary>
        public double[] LongitudinalForces { get; set; }
        /// <summary>
        /// Car's lateral forces [N]
        /// </summary>
        public double[] LateralForces { get; set; }
        /// <summary>
        /// Car's vertical loads.
        /// </summary>
        public double[] VerticalLoads { get; set; }
        public double[] TotalLongitudinalLoadTransfers { get; set; }
        public double[] UnsprungLongitudinalLoadTransfers { get; set; }
        public double[] SprungLongitudinalLoadTransfers { get; set; }
        public double[] InertiaEfficiencies { get; set; }
        #endregion
        #region Aerodynamics
        public double[] AeroDragCoefficients { get; set; }
        public double[] AeroSideForceCoefficients { get; set; }
        public double[] AeroLiftCoefficients { get; set; }
        public double[] AeroRollCoefficients { get; set; }
        public double[] AeroPitchCoefficients { get; set; }
        public double[] AeroYawCoefficients { get; set; }
        public double[] AeroDragForces { get; set; }
        public double[] AeroSideForces { get; set; }
        public double[] AeroLiftForces { get; set; }
        public double[] AeroRollMoments { get; set; }
        public double[] AeroPitchMoments { get; set; }
        public double[] AeroYawMoments { get; set; }
        #endregion
        #region Wheels
        /// <summary>
        /// Loads at the front wheels without weight [N].
        /// </summary>
        public double[] FrontWheelsLoadsWithoutWeight { get; set; }
        /// <summary>
        /// Loads at the rear wheels without weight [N].
        /// </summary>
        public double[] RearWheelsLoadsWithoutWeight { get; set; }
        /// <summary>
        /// Loads at the front wheels [N].
        /// </summary>
        public double[] FrontWheelsLoads { get; set; }
        /// <summary>
        /// Loads at the rear wheels [m].
        /// </summary>
        public double[] RearWheelsLoads { get; set; }
        /// <summary>
        /// Radiuses of the front wheels [m].
        /// </summary>
        public double[] FrontWheelsRadiuses { get; set; }
        /// <summary>
        /// Radiuses of the rear wheels [m].
        /// </summary>
        public double[] RearWheelsRadiuses { get; set; }
        public double[] FrontWheelsLongiudinalForces { get; set; }
        public double[] RearWheelsLongiudinalForces { get; set; }
        public double[] FrontWheelsTorques { get; set; }
        public double[] RearWheelsTorques { get; set; }
        public double[] FrontWheelsAngularSpeeds { get; set; }
        public double[] RearWheelsAngularSpeeds { get; set; }
        #endregion
        #region Suspension
        public double[] FrontSuspensionDeflections { get; set; }
        public double[] RearSuspensionDeflections { get; set; }
        public double[] FrontRideHeights { get; set; }
        public double[] RearRideHeights { get; set; }
        #endregion
        #region Engine
        public double[] EnginePowers { get; set; }
        public double[] EngineAvailablePowers { get; set; }
        public double[] EnginePowerUsages { get; set; }
        public double[] LocalFuelConsumptions { get; set; }
        public double[] FuelConsumptions { get; set; }
        #region Brakes
        public double[] FrontBrakesPowers { get; set; }
        public double[] RearBrakesPowers { get; set; }
        public double[] FrontBrakesAvailablePowers { get; set; }
        public double[] RearBrakesAvailablePowers { get; set; }
        public double[] FrontBrakesUsages { get; set; }
        public double[] RearBrakesUsages { get; set; }
        #endregion
        #endregion
        #region Key Performance Indicators:
        /// <summary>
        /// Final lap time [s]
        /// </summary>
        public double LapTime {
            get => double.Parse(lapTime.ToString("N4"));
            set => lapTime = value;
        }
        public double TotalFuelConsumption { get; set; }
        public int AmountOfGearShifts { get; set; }
        #endregion
        #endregion
        #region Constructors
        public LapTimeSimulationResults() { }
        public LapTimeSimulationResults(Path path)
        {
            _GetPathResults(path);
            _InitializeResultsArrays(path.AmountOfPointsInPath);
        }
        #endregion
        #region Methods
        /// <summary>
        /// Gets all of the lap time simulation results.
        /// </summary>
        public void GetAllResults()
        {
            // Initializes the results arrays
            for (int iPoint = 0; iPoint < Speeds.Length; iPoint++)
            {
                // Current sector index
                int iSector = LocalSectors[iPoint] - 1;
                // Results extraction
                _GetTimeResults(iPoint);
                _GetAerodynamicCoefficients(iPoint, iSector);
                _GetAerodynamicForcesAndMoments(iPoint, iSector);
                _GetCarVerticalLoad(iPoint, iSector);
                _GetTotalLongitudinalLoadTransfer(iPoint, iSector);
                _GetWheelsLoads(iPoint, iSector);
                _GetWheelsRadiuses(iPoint, iSector);
                _GetSuspensionDeflections(iPoint, iSector);
                _GetRideHeights(iPoint, iSector);
                _GetCarLateralForce(iPoint, iSector);
                _GetInertiaEfficiency(iPoint, iSector);
                _GetCarLongitudinalForce(iPoint, iSector);
                _GetWheelsLongitudinalForcesAndTorques(iPoint, iSector);
                _GetWheelsAngularSpeeds(iPoint, iSector);
                _GetEnginePower(iPoint, iSector);
                _GetEngineAvailablePower(iPoint, iSector);
                _GetEnginePowerUsage(iPoint, iSector);
                _GetBrakesPower(iPoint, iSector);
                _GetBrakesAvailablePowers(iPoint, iSector);
                _GetBrakesUsages(iPoint, iSector);
                _GetFuelConsumptionResults(iPoint, iSector);
            }
            LapTime = ElapsedTimes.Last();
            TotalFuelConsumption = FuelConsumptions.Last();
            AmountOfGearShifts = _GetAmountOfGearShifts();
        }
        #region Private
        /// <summary>
        /// Initializes the results arrays.
        /// </summary>
        /// <param name="amountOfPointsInPath"> Amount of points in path </param>
        private void _InitializeResultsArrays(int amountOfPointsInPath)
        {
            MaximumPossibleSpeeds = new double[amountOfPointsInPath];
            LongitudinalAccelerationsForMaximumPossibleSpeeds = new double[amountOfPointsInPath];
            LateralAccelerationsForMaximumPossibleSpeeds = new double[amountOfPointsInPath];
            GearNumbersForMaximumPossibleSpeeds = new int[amountOfPointsInPath];
            Speeds = new double[amountOfPointsInPath];
            LongitudinalAccelerations = new double[amountOfPointsInPath];
            LongitudinalForces = new double[amountOfPointsInPath];
            LateralAccelerations = new double[amountOfPointsInPath];
            LateralForces = new double[amountOfPointsInPath];
            GearNumbers = new int[amountOfPointsInPath];
            LocalElapsedTime = new double[amountOfPointsInPath];
            ElapsedTimes = new double[amountOfPointsInPath];
            VerticalLoads = new double[amountOfPointsInPath];
            TotalLongitudinalLoadTransfers = new double[amountOfPointsInPath];
            UnsprungLongitudinalLoadTransfers = new double[amountOfPointsInPath];
            SprungLongitudinalLoadTransfers = new double[amountOfPointsInPath];
            InertiaEfficiencies = new double[amountOfPointsInPath];
            AeroDragCoefficients = new double[amountOfPointsInPath];
            AeroSideForceCoefficients = new double[amountOfPointsInPath];
            AeroLiftCoefficients = new double[amountOfPointsInPath];
            AeroRollCoefficients = new double[amountOfPointsInPath];
            AeroPitchCoefficients = new double[amountOfPointsInPath];
            AeroYawCoefficients = new double[amountOfPointsInPath];
            AeroDragForces = new double[amountOfPointsInPath];
            AeroSideForces = new double[amountOfPointsInPath];
            AeroLiftForces = new double[amountOfPointsInPath];
            AeroRollMoments = new double[amountOfPointsInPath];
            AeroPitchMoments = new double[amountOfPointsInPath];
            AeroYawMoments = new double[amountOfPointsInPath];
            FrontWheelsLoadsWithoutWeight = new double[amountOfPointsInPath];
            RearWheelsLoadsWithoutWeight = new double[amountOfPointsInPath];
            FrontWheelsLoads = new double[amountOfPointsInPath];
            RearWheelsLoads = new double[amountOfPointsInPath];
            FrontWheelsRadiuses = new double[amountOfPointsInPath];
            RearWheelsRadiuses = new double[amountOfPointsInPath];
            FrontWheelsLongiudinalForces = new double[amountOfPointsInPath];
            RearWheelsLongiudinalForces = new double[amountOfPointsInPath];
            FrontWheelsTorques = new double[amountOfPointsInPath];
            RearWheelsTorques = new double[amountOfPointsInPath];
            FrontWheelsAngularSpeeds = new double[amountOfPointsInPath];
            RearWheelsAngularSpeeds = new double[amountOfPointsInPath];
            FrontSuspensionDeflections = new double[amountOfPointsInPath];
            RearSuspensionDeflections = new double[amountOfPointsInPath];
            FrontRideHeights = new double[amountOfPointsInPath];
            RearRideHeights = new double[amountOfPointsInPath];
            EnginePowers = new double[amountOfPointsInPath];
            EngineAvailablePowers = new double[amountOfPointsInPath];
            EnginePowerUsages = new double[amountOfPointsInPath];
            FrontBrakesPowers = new double[amountOfPointsInPath];
            RearBrakesPowers = new double[amountOfPointsInPath];
            FrontBrakesAvailablePowers = new double[amountOfPointsInPath];
            RearBrakesAvailablePowers = new double[amountOfPointsInPath];
            FrontBrakesUsages = new double[amountOfPointsInPath];
            RearBrakesUsages = new double[amountOfPointsInPath];
            LocalFuelConsumptions = new double[amountOfPointsInPath];
            FuelConsumptions = new double[amountOfPointsInPath];
        }
        /// <summary>
        /// Gets the results which are extracted from the path.
        /// </summary>
        /// <param name="path"> Path </param>
        private void _GetPathResults(Path path)
        {
            ElapsedDistances = path.ElapsedDistance.ToArray();
            PathResolution = path.Resolution;
            PathCurvatures = path.LocalCurvatures.ToArray();
            CoordinatesX = path.CoordinatesX.ToArray();
            CoordinatesY = path.CoordinatesY.ToArray();
        }
        /// <summary>
        /// Gets the time results.
        /// </summary>
        /// <param name="iPoint"> Index of the current point in path. </param>
        private void _GetTimeResults(int iPoint)
        {
            if (Speeds[iPoint] != 0) LocalElapsedTime[iPoint] = PathResolution / Speeds[iPoint];
            else LocalElapsedTime[iPoint] = 0;
            ElapsedTimes[iPoint] = LocalElapsedTime.Sum();
        }
        /// <summary>
        /// Gets the aerdynamic coefficients for the current point.
        /// </summary>
        /// <param name="iPoint"> Index of the current point in path. </param>
        /// <param name="iSector"> Index of the current sector. </param>
        private void _GetAerodynamicCoefficients(int iPoint, int iSector)
        {
            double[] aerodynamicCoefficients = GGVDiagramsPerSector[iSector].SectorGGVDiagram.GetAerodynamicCoefficients(new double[] { Speeds[iPoint], 0, LongitudinalAccelerations[iPoint] });
            AeroDragCoefficients[iPoint] = aerodynamicCoefficients[0];
            AeroSideForceCoefficients[iPoint] = aerodynamicCoefficients[1];
            AeroLiftCoefficients[iPoint] = aerodynamicCoefficients[2];
            AeroRollCoefficients[iPoint] = aerodynamicCoefficients[3];
            AeroPitchCoefficients[iPoint] = aerodynamicCoefficients[4];
            AeroYawCoefficients[iPoint] = aerodynamicCoefficients[5];
        }
        /// <summary>
        /// Gets the aerodynamics forces and moments
        /// </summary>
        /// <param name="iPoint"> Index of the current point in path. </param>
        /// <param name="iSector"> Index of the current sector. </param>
        private void _GetAerodynamicForcesAndMoments(int iPoint, int iSector)
        {
            AeroDragForces[iPoint] = GGVDiagramsPerSector[iSector].SectorGGVDiagram.GetAerodynamicForceOrMoment(AeroDragCoefficients[iPoint], Speeds[iPoint]);
            AeroSideForces[iPoint] = GGVDiagramsPerSector[iSector].SectorGGVDiagram.GetAerodynamicForceOrMoment(AeroSideForceCoefficients[iPoint], Speeds[iPoint]);
            AeroLiftForces[iPoint] = GGVDiagramsPerSector[iSector].SectorGGVDiagram.GetAerodynamicForceOrMoment(AeroLiftCoefficients[iPoint], Speeds[iPoint]);
            AeroRollMoments[iPoint] = GGVDiagramsPerSector[iSector].SectorGGVDiagram.GetAerodynamicForceOrMoment(AeroRollCoefficients[iPoint], Speeds[iPoint]);
            AeroPitchMoments[iPoint] = GGVDiagramsPerSector[iSector].SectorGGVDiagram.GetAerodynamicForceOrMoment(AeroPitchCoefficients[iPoint], Speeds[iPoint]);
            AeroYawMoments[iPoint] = GGVDiagramsPerSector[iSector].SectorGGVDiagram.GetAerodynamicForceOrMoment(AeroYawCoefficients[iPoint], Speeds[iPoint]);
        }
        /// <summary>
        /// Gets the car's total vertical load.
        /// </summary>
        /// <param name="iPoint"> Index of the current point in path. </param>
        /// <param name="iSector"> Index of the current sector. </param>
        private void _GetCarVerticalLoad(int iPoint, int iSector)
        {
            VerticalLoads[iPoint] = GGVDiagramsPerSector[iSector].SectorGGVDiagram.GetCarVerticalLoad(AeroLiftForces[iPoint]);
        }
        /// <summary>
        /// Get longitudinal weight transfers [N].
        /// </summary>
        /// <param name="iPoint"> Index of the current point in path. </param>
        /// <param name="iSector"> Index of the current sector. </param>
        private void _GetTotalLongitudinalLoadTransfer(int iPoint, int iSector)
        {
            TotalLongitudinalLoadTransfers[iPoint] = GGVDiagramsPerSector[iSector].SectorGGVDiagram.GetCarTotalLongitudinalLoadTransfer(LongitudinalAccelerations[iPoint]);
            UnsprungLongitudinalLoadTransfers[iPoint] = GGVDiagramsPerSector[iSector].SectorGGVDiagram.GetCarUnsprungLongitudinalLoadTransfer(LongitudinalAccelerations[iPoint]);
            SprungLongitudinalLoadTransfers[iPoint]= GGVDiagramsPerSector[iSector].SectorGGVDiagram.GetCarSprungLongitudinalLoadTransfer(TotalLongitudinalLoadTransfers[iPoint], UnsprungLongitudinalLoadTransfers[iPoint]);
        }
        /// <summary>
        /// Gets the front and rear wheels loads.
        /// </summary>
        /// <param name="iPoint"> Index of the current point in path. </param>
        /// <param name="iSector"> Index of the current sector. </param>
        private void _GetWheelsLoads(int iPoint, int iSector)
        {
            double[] wheelsLoadsWithoutWeight = GGVDiagramsPerSector[iSector].SectorGGVDiagram.GetWheelsLoadsWithoutWeight(AeroLiftForces[iPoint], AeroPitchMoments[iPoint], TotalLongitudinalLoadTransfers[iPoint]);
            FrontWheelsLoadsWithoutWeight[iPoint] = wheelsLoadsWithoutWeight[0];
            RearWheelsLoadsWithoutWeight[iPoint] = wheelsLoadsWithoutWeight[1];
            double[] wheelsLoads = GGVDiagramsPerSector[iSector].SectorGGVDiagram.GetWheelsLoads(wheelsLoadsWithoutWeight);
            FrontWheelsLoads[iPoint] = wheelsLoads[0];
            RearWheelsLoads[iPoint] = wheelsLoads[1];
        }
        /// <summary>
        /// Gets the front and rear wheels radiuses.
        /// </summary>
        /// <param name="iPoint"> Index of the current point in path. </param>
        /// <param name="iSector"> Index of the current sector. </param>
        private void _GetWheelsRadiuses(int iPoint, int iSector)
        {
            double[] wheelsRadiuses = GGVDiagramsPerSector[iSector].SectorGGVDiagram.GetWheelsRadiuses(new double[] { FrontWheelsLoads[iPoint], RearWheelsLoads[iPoint] });
            FrontWheelsRadiuses[iPoint] = wheelsRadiuses[0];
            RearWheelsRadiuses[iPoint] = wheelsRadiuses[1];
        }
        /// <summary>
        /// Gets the front and rear suspension deflections.
        /// </summary>
        /// <param name="iPoint"> Index of the current point in path. </param>
        /// <param name="iSector"> Index of the current sector. </param>
        private void _GetSuspensionDeflections(int iPoint, int iSector)
        {
            double[] suspensionDeflections = GGVDiagramsPerSector[iSector].SectorGGVDiagram.GetSuspensionDeflections(new double[] { FrontWheelsLoadsWithoutWeight[iPoint], RearWheelsLoadsWithoutWeight[iPoint] });
            FrontSuspensionDeflections[iPoint] = suspensionDeflections[0];
            RearSuspensionDeflections[iPoint] = suspensionDeflections[1];
        }
        /// <summary>
        /// Gets the car's front and rear heights [m].
        /// </summary>
        /// <param name="iPoint"> Index of the current point in path. </param>
        /// <param name="iSector"> Index of the current sector. </param>
        private void _GetRideHeights(int iPoint, int iSector)
        {
            double[] carHeights = GGVDiagramsPerSector[iSector].SectorGGVDiagram.GetCarHeights(new double[] { FrontWheelsRadiuses[iPoint], RearWheelsRadiuses[iPoint] }, new double[] { FrontSuspensionDeflections[iPoint], RearSuspensionDeflections[iPoint] });
            FrontRideHeights[iPoint] = carHeights[0];
            RearRideHeights[iPoint] = carHeights[1];
        }
        /// <summary>
        /// Gets the car's inertia efficiency.
        /// </summary>
        /// <param name="iPoint"> Index of the current point in path. </param>
        /// <param name="iSector"> Index of the current sector. </param>
        private void _GetInertiaEfficiency(int iPoint, int iSector)
        {
            InertiaEfficiencies[iPoint] = GGVDiagramsPerSector[iSector].SectorGGVDiagram.GetInertiaEfficiency(new double[] { FrontWheelsRadiuses[iPoint], RearWheelsLoads[iPoint] });
        }
        /// <summary>
        /// Gets the car's resultant lateral force.
        /// </summary>
        /// <param name="iPoint"> Index of the current point in path. </param>
        /// <param name="iSector"> Index of the current sector. </param>
        private void _GetCarLateralForce(int iPoint, int iSector)
        {
            LateralForces[iPoint] = GGVDiagramsPerSector[iSector].SectorGGVDiagram.GetCarLateralForce(LateralAccelerations[iPoint]);
        }
        /// <summary>
        /// Gets the car's resultant longitudinal force.
        /// </summary>
        /// <param name="iPoint"> Index of the current point in path. </param>
        /// <param name="iSector"> Index of the current sector. </param>
        private void _GetCarLongitudinalForce(int iPoint, int iSector)
        {
            LongitudinalForces[iPoint] = GGVDiagramsPerSector[iSector].SectorGGVDiagram.GetCarLongitudinalForce(LongitudinalAccelerations[iPoint], InertiaEfficiencies[iPoint]);
        }
        /// <summary>
        /// Gets the longitudinal forces and torques at the wheels.
        /// </summary>
        /// <param name="iPoint"> Index of the current point in path. </param>
        /// <param name="iSector"> Index of the current sector. </param>
        private void _GetWheelsLongitudinalForcesAndTorques(int iPoint, int iSector)
        {
            double[][] wheelsLongitudinalForcesAndTorques = GGVDiagramsPerSector[iSector].SectorGGVDiagram.GetWheelsTorquesAndLongitudinalForces(LongitudinalForces[iPoint], new double[] { FrontWheelsRadiuses[iPoint], RearWheelsRadiuses[iPoint] }, AeroDragForces[iPoint]);
            FrontWheelsLongiudinalForces[iPoint] = wheelsLongitudinalForcesAndTorques[0][0];
            RearWheelsLongiudinalForces[iPoint] = wheelsLongitudinalForcesAndTorques[0][1];
            FrontWheelsTorques[iPoint] = wheelsLongitudinalForcesAndTorques[1][0];
            RearWheelsTorques[iPoint] = wheelsLongitudinalForcesAndTorques[1][1];
        }
        /// <summary>
        /// Gets the wheels angular speeds.
        /// </summary>
        /// <param name="iPoint"> Index of the current point in path. </param>
        /// <param name="iSector"> Index of the current sector. </param>
        private void _GetWheelsAngularSpeeds(int iPoint, int iSector)
        {
            double[] wheelsAngularSpeeds = GGVDiagramsPerSector[iSector].SectorGGVDiagram.GetWheelsAngularSpeeds(Speeds[iPoint], new double[] { FrontWheelsRadiuses[iPoint], RearWheelsRadiuses[iPoint] });
            FrontWheelsAngularSpeeds[iPoint] = wheelsAngularSpeeds[0];
            RearWheelsAngularSpeeds[iPoint] = wheelsAngularSpeeds[1];
        }
        /// <summary>
        /// Gets the current engine power.
        /// </summary>
        /// <param name="iPoint"> Index of the current point in path. </param>
        /// <param name="iSector"> Index of the current sector. </param>
        private void _GetEnginePower(int iPoint, int iSector)
        {
            if (LongitudinalForces[iPoint] - AeroDragForces[iPoint] > 0)
            {
                EnginePowers[iPoint] = GGVDiagramsPerSector[iSector].SectorGGVDiagram.GetEnginePower(new double[] { FrontWheelsTorques[iPoint], RearWheelsTorques[iPoint] }, new double[] { FrontWheelsAngularSpeeds[iPoint], RearWheelsAngularSpeeds[iPoint] });
            }
            else EnginePowers[iPoint] = 0;
        }
        /// <summary>
        /// Gets the available engine power.
        /// </summary>
        /// <param name="iPoint"> Index of the current point in path. </param>
        /// <param name="iSector"> Index of the current sector. </param>
        private void _GetEngineAvailablePower(int iPoint, int iSector)
        {
            EngineAvailablePowers[iPoint] = GGVDiagramsPerSector[iSector].SectorGGVDiagram.GetEngineAvailablePower(new double[] { FrontWheelsAngularSpeeds[iPoint], RearWheelsAngularSpeeds[iPoint] });
        }
        /// <summary>
        /// Gets the engine power usage.
        /// </summary>
        /// <param name="iPoint"> Index of the current point in path. </param>
        /// <param name="iSector"> Index of the current sector. </param>
        private void _GetEnginePowerUsage(int iPoint, int iSector)
        {
            EnginePowerUsages[iPoint] = GGVDiagramsPerSector[iSector].SectorGGVDiagram.GetUsageRatio(EnginePowers[iPoint], EngineAvailablePowers[iPoint]);
        }
        /// <summary>
        /// Gets th current brakes powers.
        /// </summary>
        /// <param name="iPoint"> Index of the current point in path. </param>
        /// <param name="iSector"> Index of the current sector. </param>
        private void _GetBrakesPower(int iPoint, int iSector)
        {
            if (LongitudinalForces[iPoint] - AeroDragForces[iPoint] < 0)
            {
                double[] brakesPowers = GGVDiagramsPerSector[iSector].SectorGGVDiagram.GetBrakesPower(new double[] { FrontWheelsTorques[iPoint], RearWheelsTorques[iPoint] }, new double[] { FrontWheelsAngularSpeeds[iPoint], RearWheelsAngularSpeeds[iPoint] });
                FrontBrakesPowers[iPoint] = brakesPowers[0];
                RearBrakesPowers[iPoint] = brakesPowers[1];
            }
            else
            {
                FrontBrakesPowers[iPoint] = 0;
                RearBrakesPowers[iPoint] = 0;
            }
        }
        /// <summary>
        /// Gets the brakes available powers.
        /// </summary>
        /// <param name="iPoint"> Index of the current point in path. </param>
        /// <param name="iSector"> Index of the current sector. </param>
        private void _GetBrakesAvailablePowers(int iPoint, int iSector)
        {
            double[] brakesAvailablePowers = GGVDiagramsPerSector[iSector].SectorGGVDiagram.GetBrakesAvailablePowers(new double[] { FrontWheelsAngularSpeeds[iPoint], RearWheelsAngularSpeeds[iPoint] });
            FrontBrakesAvailablePowers[iPoint] = brakesAvailablePowers[0];
            RearBrakesAvailablePowers[iPoint] = brakesAvailablePowers[1];
        }
        /// <summary>
        /// Gets the brakes usage.
        /// </summary>
        /// <param name="iPoint"> Index of the current point in path. </param>
        /// <param name="iSector"> Index of the current sector. </param>
        private void _GetBrakesUsages(int iPoint, int iSector)
        {
            FrontBrakesUsages[iPoint] = GGVDiagramsPerSector[iSector].SectorGGVDiagram.GetUsageRatio(FrontBrakesPowers[iPoint], FrontBrakesAvailablePowers[iPoint]);
            RearBrakesUsages[iPoint] = GGVDiagramsPerSector[iSector].SectorGGVDiagram.GetUsageRatio(RearBrakesPowers[iPoint], RearBrakesAvailablePowers[iPoint]);
        }
        /// <summary>
        /// Gets the car's fuel consumption.
        /// </summary>
        /// <param name="iPoint"> Index of the current point in path. </param>
        /// <param name="iSector"> Index of the current sector. </param>
        private void _GetFuelConsumptionResults(int iPoint, int iSector)
        {
            if (EnginePowers[iPoint] > 0)
            {
                LocalFuelConsumptions[iPoint] = GGVDiagramsPerSector[iSector].SectorGGVDiagram.GetFuelConsumption(EnginePowers[iPoint], new double[] { FrontWheelsAngularSpeeds[iPoint], RearWheelsAngularSpeeds[iPoint] }, Speeds[iPoint], ElapsedDistances[iPoint]);
            }
            else LocalFuelConsumptions[iPoint] = 0;
            FuelConsumptions[iPoint] = LocalFuelConsumptions.Sum();
        }
        /// <summary>
        /// Gets the amount of gear shifts for the lap.
        /// </summary>
        /// <returns> Amount of gear shifts. </returns>
        private int _GetAmountOfGearShifts()
        {
            int amountOfGearShifts = 0;
            for (int iPoint = 1; iPoint < Speeds.Length; iPoint++)
            {
                if (GearNumbers[iPoint]<GearNumbers[iPoint-1])
                {
                    amountOfGearShifts++;
                }
            }
            return amountOfGearShifts;
        }
        #endregion
        #endregion
    }
}
