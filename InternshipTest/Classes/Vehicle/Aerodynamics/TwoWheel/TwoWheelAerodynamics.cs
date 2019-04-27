using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternshipTest.Vehicle
{
    public class TwoWheelAerodynamics : Aerodynamics
    {
        #region Fields
        /// <summary>
        /// Relative wind speeds of the aerodynamic map [m].
        /// </summary>
        private List<double> aerodynamicMapWindSpeeds;
        /// <summary>
        /// Front ride heights of the aerodynamic map [m].
        /// </summary>
        private List<double> aerodynamicMapFrontRideHeights;
        /// <summary>
        /// Rear ride heights of the aerodynamic map [m].
        /// </summary>
        private List<double> aerodynamicMapRearRideHeights;
        /// <summary>
        /// Car slip angles of the aerodynamic map [rad].
        /// </summary>
        private List<double> aerodynamicMapCarSlipAngles;
        #endregion
        #region Properties
        /// <summary>
        /// Aerodynamic map properties (wind speed, ride height, CD and CL).
        /// </summary>
        public TwoWheelAerodynamicMap AerodynamicMap { get; set; }
        #endregion
        #region Constructors
        public TwoWheelAerodynamics() { }
        public TwoWheelAerodynamics(string aeroID, string description, TwoWheelAerodynamicMap aeroMap, double frontalArea, double airDensity)
        {
            ID = aeroID;
            Description = description;
            AerodynamicMap = aeroMap;
            FrontalArea = Math.Abs(frontalArea);
            AirDensity = Math.Abs(airDensity);
        }
        #endregion
        #region Methods
        /// <summary>
        /// Gets the aerodynamic map's parameters.
        /// </summary>
        public void GetAerodynamicMapParameters()
        {
            // Aerodyanmic map row sweep "for" loop
            foreach (TwoWheelAerodynamicMapPoint aerodynamicMapPoint in AerodynamicMap.MapPoints)
            {
                aerodynamicMapWindSpeeds = new List<double>();
                aerodynamicMapFrontRideHeights = new List<double>();
                aerodynamicMapRearRideHeights = new List<double>();
                aerodynamicMapCarSlipAngles = new List<double>();
                // Current row parameters
                double aerodynamicMapRowWindSpeed = aerodynamicMapPoint.WindRelativeSpeed;
                double aerodynamicMapRowFrontRideHeight = aerodynamicMapPoint.FrontRideHeight;
                double aerodynamicMapRowRearRideHeight = aerodynamicMapPoint.RearRideHeight;
                double aerodynamicMapRowCarSlipAngle = aerodynamicMapPoint.CarSlipAngle;
                // New parameter indicators initialization
                bool isNewWindSpeed = true;
                bool isNewFrontRideHeight = true;
                bool isNewRearRideHeight = true;
                bool isNewCarSlipAngle = true;
                // New parameter indicators value determination
                foreach (double windSpeed in aerodynamicMapWindSpeeds)
                {
                    if (aerodynamicMapRowWindSpeed == windSpeed) isNewWindSpeed = false;
                }
                foreach (double frontRideHeight in aerodynamicMapFrontRideHeights)
                {
                    if (aerodynamicMapRowFrontRideHeight == frontRideHeight) isNewFrontRideHeight = false;
                }
                foreach (double rearRideHeight in aerodynamicMapRearRideHeights)
                {
                    if (aerodynamicMapRowRearRideHeight == rearRideHeight) isNewRearRideHeight = false;
                }
                foreach (double carSlipAngle in aerodynamicMapCarSlipAngles)
                {
                    if (aerodynamicMapRowCarSlipAngle == carSlipAngle) isNewCarSlipAngle = false;
                }
                // New parameter values registration
                if (isNewWindSpeed) aerodynamicMapWindSpeeds.Add(aerodynamicMapRowWindSpeed);
                if (isNewFrontRideHeight) aerodynamicMapFrontRideHeights.Add(aerodynamicMapRowFrontRideHeight);
                if (isNewRearRideHeight) aerodynamicMapRearRideHeights.Add(aerodynamicMapRowRearRideHeight);
                if (isNewCarSlipAngle) aerodynamicMapCarSlipAngles.Add(aerodynamicMapRowCarSlipAngle);
            }
            // Sort to ascending order
            aerodynamicMapWindSpeeds = aerodynamicMapWindSpeeds.OrderBy(d => d).ToList();
            aerodynamicMapFrontRideHeights = aerodynamicMapFrontRideHeights.OrderBy(d => d).ToList();
            aerodynamicMapRearRideHeights = aerodynamicMapRearRideHeights.OrderBy(d => d).ToList();
            aerodynamicMapCarSlipAngles = aerodynamicMapCarSlipAngles.OrderBy(d => d).ToList();
        }

        /// <summary>
        /// Gets an aerodynamic map point based on an aerodynamic map and input parameters.
        /// </summary>
        /// <param name="windSpeed"> Relative speed of the wind [m/s]. </param>
        /// <param name="frontRideHeight"> Distance from the ground to the front of the car in the vertical axis [m]. </param>
        /// <param name="rearRideHeight"> Distance from the ground to the rear of the car in the vertical axis [m]. </param>
        /// <param name="carSlipAngle"> Car slip angle [rad]. </param>
        /// <returns> Interpolated aerodynamic map point (wind speed, front ride height, rear ride height, car slip angle, CD, CS, CL, CPM and CYM). </returns>
        public TwoWheelAerodynamicMapPoint GetAerodynamicMapPointFromParameters(double windSpeed, double frontRideHeight, double rearRideHeight, double carSlipAngle)
        {
            TwoWheelAerodynamicMap newAerodynamicMap = new TwoWheelAerodynamicMap() { MapPoints = AerodynamicMap.MapPoints };
            newAerodynamicMap = _GetAerodynamicMapFromWindSpeed(newAerodynamicMap, windSpeed);
            newAerodynamicMap = _GetAerodynamicMapFromFrontRideHeight(newAerodynamicMap, frontRideHeight);
            newAerodynamicMap = _GetAerodynamicMapFromRearRideHeight(newAerodynamicMap, rearRideHeight);
            newAerodynamicMap = _GetAerodynamicMapFromCarSlipAngle(newAerodynamicMap, carSlipAngle);
            return newAerodynamicMap.MapPoints[0];
        }

        /// <summary>
        /// Gets an aerodynamic map based on an initial aerodynamic map and a wind speed.
        /// </summary>
        /// <param name="aerodynamicMap"> Initial aerodynamic map. </param>
        /// <param name="windSpeed"> Wind speed to determine the new aerodynamic map [m/s]. </param>
        /// <returns> Aerodynamic map determined by the wind speed. </returns>
        private TwoWheelAerodynamicMap _GetAerodynamicMapFromWindSpeed(TwoWheelAerodynamicMap aerodynamicMap, double windSpeed)
        {
            // Interpolated aerodynamic map intialization
            TwoWheelAerodynamicMap newAerodynamicMap = new TwoWheelAerodynamicMap() { MapPoints = new List<TwoWheelAerodynamicMapPoint>() };
            // Checks if the desired wind speed is in lower, higher or in the range of the aerodynamic map.
            if (windSpeed <= aerodynamicMapWindSpeeds.Min())
            {
                for (int iPoint = 0; iPoint < aerodynamicMap.MapPoints.Count; iPoint++)
                {
                    if (aerodynamicMap.MapPoints[iPoint].WindRelativeSpeed == aerodynamicMapWindSpeeds.Min())
                    {
                        newAerodynamicMap.MapPoints.Add(aerodynamicMap.MapPoints[iPoint]);
                    }
                }
            }
            else if (windSpeed >= aerodynamicMapWindSpeeds.Max())
            {
                for (int iPoint = 0; iPoint < aerodynamicMap.MapPoints.Count; iPoint++)
                {
                    if (aerodynamicMap.MapPoints[iPoint].WindRelativeSpeed == aerodynamicMapWindSpeeds.Max())
                    {
                        newAerodynamicMap.MapPoints.Add(aerodynamicMap.MapPoints[iPoint]);
                    }
                }
            }
            else
            {
                newAerodynamicMap = _InterpolateAerodynamicMapByWindSpeed(aerodynamicMap, windSpeed);
            }
            return newAerodynamicMap;
        }

        /// <summary>
        /// Gets a aerodynamic map by interpolation based on wind speed.
        /// </summary>
        /// <param name="aerodynamicMap"> Aerodynamic map to be interpolated. </param>
        /// <param name="windSpeed"> Desired wind speed [m/s]. </param>
        /// <returns> An aerodynamic map interpolated by wind speed. </returns>
        private TwoWheelAerodynamicMap _InterpolateAerodynamicMapByWindSpeed(TwoWheelAerodynamicMap aerodynamicMap, double windSpeed)
        {
            TwoWheelAerodynamicMap interpolatedAerodynamicMap = new TwoWheelAerodynamicMap() { MapPoints = new List<TwoWheelAerodynamicMapPoint>() };
            // Finds the map's next lower speed compared to the wind speed
            int iWindSpeed = 0;
            while (windSpeed <= aerodynamicMapWindSpeeds[iWindSpeed]) iWindSpeed++;
            // Lower and higher wind speeds
            double lowerWindSpeed = aerodynamicMapWindSpeeds[iWindSpeed];
            double higherWindSpeed = aerodynamicMapWindSpeeds[iWindSpeed + 1];
            // Lower and upper aerodynamic maps lists initialization
            TwoWheelAerodynamicMap lowerAerodynamicMap = new TwoWheelAerodynamicMap() { MapPoints = new List<TwoWheelAerodynamicMapPoint>() };
            TwoWheelAerodynamicMap higherAerodynamicMap = new TwoWheelAerodynamicMap() { MapPoints = new List<TwoWheelAerodynamicMapPoint>() };
            // Gets the objects of the lower and upper aerodynamic maps
            for (int row = 0; row < aerodynamicMap.MapPoints.Count; row++)
            {
                if (AerodynamicMap.MapPoints[row].WindRelativeSpeed == lowerWindSpeed)
                    lowerAerodynamicMap.MapPoints.Add(aerodynamicMap.MapPoints[row]);
                else if (AerodynamicMap.MapPoints[row].WindRelativeSpeed == higherWindSpeed)
                    higherAerodynamicMap.MapPoints.Add(aerodynamicMap.MapPoints[row]);
            }
            // Linear interpolation ratio
            double interpolationRatio = (windSpeed - lowerWindSpeed) / (higherWindSpeed - lowerWindSpeed);
            // Interpolated map generation
            for (int iLowerWindSpeed = 0; iLowerWindSpeed < lowerAerodynamicMap.MapPoints.Count; iLowerWindSpeed++)
            {
                for (int iHigherWindSpeed = 0; iHigherWindSpeed < higherAerodynamicMap.MapPoints.Count; iHigherWindSpeed++)
                {
                    TwoWheelAerodynamicMapPoint lowerAerodynamicMapPoint = lowerAerodynamicMap.MapPoints[iLowerWindSpeed];
                    TwoWheelAerodynamicMapPoint higherAerodynamicMapPoint = higherAerodynamicMap.MapPoints[iHigherWindSpeed];
                    if (lowerAerodynamicMapPoint.FrontRideHeight == higherAerodynamicMapPoint.FrontRideHeight && lowerAerodynamicMapPoint.RearRideHeight == higherAerodynamicMapPoint.RearRideHeight && lowerAerodynamicMapPoint.CarSlipAngle == higherAerodynamicMapPoint.CarSlipAngle)
                    {
                        // Interpolated aerodynamic map point
                        TwoWheelAerodynamicMapPoint aerodynamicMapPoint = new TwoWheelAerodynamicMapPoint
                        {
                            WindRelativeSpeed = windSpeed,
                            FrontRideHeight = lowerAerodynamicMapPoint.FrontRideHeight,
                            RearRideHeight = lowerAerodynamicMapPoint.RearRideHeight,
                            CarSlipAngle = lowerAerodynamicMapPoint.CarSlipAngle,
                            DragCoefficient = lowerAerodynamicMapPoint.DragCoefficient + (higherAerodynamicMapPoint.DragCoefficient - lowerAerodynamicMapPoint.DragCoefficient) * interpolationRatio,
                            SideForceCoefficient = lowerAerodynamicMapPoint.SideForceCoefficient + (higherAerodynamicMapPoint.SideForceCoefficient - lowerAerodynamicMapPoint.SideForceCoefficient) * interpolationRatio,
                            LiftCoefficient = lowerAerodynamicMapPoint.LiftCoefficient + (higherAerodynamicMapPoint.LiftCoefficient - lowerAerodynamicMapPoint.LiftCoefficient) * interpolationRatio,
                            PitchMomentCoefficient = lowerAerodynamicMapPoint.PitchMomentCoefficient + (higherAerodynamicMapPoint.PitchMomentCoefficient - lowerAerodynamicMapPoint.PitchMomentCoefficient) * interpolationRatio,
                            YawMomentCoefficient = lowerAerodynamicMapPoint.YawMomentCoefficient + (higherAerodynamicMapPoint.YawMomentCoefficient - lowerAerodynamicMapPoint.YawMomentCoefficient) * interpolationRatio
                        };
                        // Registering of the point
                        interpolatedAerodynamicMap.MapPoints.Add(aerodynamicMapPoint);
                    }
                }
            }
            return interpolatedAerodynamicMap;
        }

        /// <summary>
        /// Gets an aerodynamic map based on an initial aerodynamic map and a front ride height.
        /// </summary>
        /// <param name="aerodynamicMap"> Initial aerodynamic map. </param>
        /// <param name="frontRideHeight"> Front ride height to determine the new aerodynamic map [m]. </param>
        /// <returns> Aerodynamic map determined by the front ride height. </returns>
        private TwoWheelAerodynamicMap _GetAerodynamicMapFromFrontRideHeight(TwoWheelAerodynamicMap aerodynamicMap, double frontRideHeight)
        {
            // Interpolated aerodynamic map intialization
            TwoWheelAerodynamicMap newAerodynamicMap = new TwoWheelAerodynamicMap() { MapPoints = new List<TwoWheelAerodynamicMapPoint>() };
            // Checks if the desired wind speed is in lower, higher or in the range of the aerodynamic map.
            if (frontRideHeight <= aerodynamicMapFrontRideHeights.Min())
            {
                for (int iPoint = 0; iPoint < aerodynamicMap.MapPoints.Count; iPoint++)
                {
                    if (aerodynamicMap.MapPoints[iPoint].FrontRideHeight == aerodynamicMapFrontRideHeights.Min())
                    {
                        newAerodynamicMap.MapPoints.Add(aerodynamicMap.MapPoints[iPoint]);
                    }
                }
            }
            else if (frontRideHeight >= aerodynamicMapFrontRideHeights.Max())
            {
                for (int iPoint = 0; iPoint < aerodynamicMap.MapPoints.Count; iPoint++)
                {
                    if (aerodynamicMap.MapPoints[iPoint].FrontRideHeight == aerodynamicMapFrontRideHeights.Max())
                    {
                        newAerodynamicMap.MapPoints.Add(aerodynamicMap.MapPoints[iPoint]);
                    }
                }
            }
            else
            {
                newAerodynamicMap = _InterpolateAerodynamicMapByFrontRideHeight(aerodynamicMap, frontRideHeight);
            }
            return newAerodynamicMap;
        }

        /// <summary>
        /// Gets a aerodynamic map by interpolation based on front ride height.
        /// </summary>
        /// <param name="aerodynamicMap"> Aerodynamic map to be interpolated. </param>
        /// <param name="frontRideHeight"> Desired front ride height [m]. </param>
        /// <returns> An aerodynamic map interpolated by front ride height. </returns>
        private TwoWheelAerodynamicMap _InterpolateAerodynamicMapByFrontRideHeight(TwoWheelAerodynamicMap aerodynamicMap, double frontRideHeight)
        {
            TwoWheelAerodynamicMap interpolatedAerodynamicMap = new TwoWheelAerodynamicMap() { MapPoints = new List<TwoWheelAerodynamicMapPoint>() };
            // Finds the map's next lower speed compared to the wind speed
            int iFrontRideHeight = 0;
            while (frontRideHeight <= aerodynamicMapFrontRideHeights[iFrontRideHeight]) iFrontRideHeight++;
            // Lower and higher wind speeds
            double lowerFrontRideHeight = aerodynamicMapFrontRideHeights[iFrontRideHeight];
            double higherFrontRideHeight = aerodynamicMapFrontRideHeights[iFrontRideHeight + 1];
            // Lower and upper aerodynamic maps lists initialization
            TwoWheelAerodynamicMap lowerAerodynamicMap = new TwoWheelAerodynamicMap() { MapPoints = new List<TwoWheelAerodynamicMapPoint>() };
            TwoWheelAerodynamicMap higherAerodynamicMap = new TwoWheelAerodynamicMap() { MapPoints = new List<TwoWheelAerodynamicMapPoint>() };
            // Gets the objects of the lower and upper aerodynamic maps
            for (int row = 0; row < aerodynamicMap.MapPoints.Count; row++)
            {
                if (AerodynamicMap.MapPoints[row].FrontRideHeight == lowerFrontRideHeight)
                    lowerAerodynamicMap.MapPoints.Add(aerodynamicMap.MapPoints[row]);
                else if (AerodynamicMap.MapPoints[row].FrontRideHeight == higherFrontRideHeight)
                    higherAerodynamicMap.MapPoints.Add(aerodynamicMap.MapPoints[row]);
            }
            // Linear interpolation ratio
            double interpolationRatio = (frontRideHeight - lowerFrontRideHeight) / (higherFrontRideHeight - lowerFrontRideHeight);
            // Interpolated map generation
            for (int iLowerFrontRideHeight = 0; iLowerFrontRideHeight < lowerAerodynamicMap.MapPoints.Count; iLowerFrontRideHeight++)
            {
                for (int iHigherFrontRideHeight = 0; iHigherFrontRideHeight < higherAerodynamicMap.MapPoints.Count; iHigherFrontRideHeight++)
                {
                    TwoWheelAerodynamicMapPoint lowerAerodynamicMapPoint = lowerAerodynamicMap.MapPoints[iLowerFrontRideHeight];
                    TwoWheelAerodynamicMapPoint higherAerodynamicMapPoint = higherAerodynamicMap.MapPoints[iHigherFrontRideHeight];
                    if (lowerAerodynamicMapPoint.WindRelativeSpeed == higherAerodynamicMapPoint.WindRelativeSpeed && lowerAerodynamicMapPoint.RearRideHeight == higherAerodynamicMapPoint.RearRideHeight && lowerAerodynamicMapPoint.CarSlipAngle == higherAerodynamicMapPoint.CarSlipAngle)
                    {
                        // Interpolated aerodynamic map point
                        TwoWheelAerodynamicMapPoint aerodynamicMapPoint = new TwoWheelAerodynamicMapPoint
                        {
                            WindRelativeSpeed = lowerAerodynamicMapPoint.WindRelativeSpeed,
                            FrontRideHeight = frontRideHeight,
                            RearRideHeight = lowerAerodynamicMapPoint.RearRideHeight,
                            CarSlipAngle = lowerAerodynamicMapPoint.CarSlipAngle,
                            DragCoefficient = lowerAerodynamicMapPoint.DragCoefficient + (higherAerodynamicMapPoint.DragCoefficient - lowerAerodynamicMapPoint.DragCoefficient) * interpolationRatio,
                            SideForceCoefficient = lowerAerodynamicMapPoint.SideForceCoefficient + (higherAerodynamicMapPoint.SideForceCoefficient - lowerAerodynamicMapPoint.SideForceCoefficient) * interpolationRatio,
                            LiftCoefficient = lowerAerodynamicMapPoint.LiftCoefficient + (higherAerodynamicMapPoint.LiftCoefficient - lowerAerodynamicMapPoint.LiftCoefficient) * interpolationRatio,
                            PitchMomentCoefficient = lowerAerodynamicMapPoint.PitchMomentCoefficient + (higherAerodynamicMapPoint.PitchMomentCoefficient - lowerAerodynamicMapPoint.PitchMomentCoefficient) * interpolationRatio,
                            YawMomentCoefficient = lowerAerodynamicMapPoint.YawMomentCoefficient + (higherAerodynamicMapPoint.YawMomentCoefficient - lowerAerodynamicMapPoint.YawMomentCoefficient) * interpolationRatio
                        };
                        // Registering of the point
                        interpolatedAerodynamicMap.MapPoints.Add(aerodynamicMapPoint);
                    }
                }
            }
            return interpolatedAerodynamicMap;
        }

        /// <summary>
        /// Gets an aerodynamic map based on an initial aerodynamic map and a rear ride height.
        /// </summary>
        /// <param name="aerodynamicMap"> Initial aerodynamic map. </param>
        /// <param name="rearRideHeight"> Rear ride height to determine the new aerodynamic map [m]. </param>
        /// <returns> Aerodynamic map determined by the rear ride height. </returns>
        private TwoWheelAerodynamicMap _GetAerodynamicMapFromRearRideHeight(TwoWheelAerodynamicMap aerodynamicMap, double rearRideHeight)
        {
            // Interpolated aerodynamic map intialization
            TwoWheelAerodynamicMap newAerodynamicMap = new TwoWheelAerodynamicMap() { MapPoints = new List<TwoWheelAerodynamicMapPoint>() };
            // Checks if the desired wind speed is in lower, higher or in the range of the aerodynamic map.
            if (rearRideHeight <= aerodynamicMapRearRideHeights.Min())
            {
                for (int iPoint = 0; iPoint < aerodynamicMap.MapPoints.Count; iPoint++)
                {
                    if (aerodynamicMap.MapPoints[iPoint].RearRideHeight == aerodynamicMapRearRideHeights.Min())
                    {
                        newAerodynamicMap.MapPoints.Add(aerodynamicMap.MapPoints[iPoint]);
                    }
                }
            }
            else if (rearRideHeight >= aerodynamicMapRearRideHeights.Max())
            {
                for (int iPoint = 0; iPoint < aerodynamicMap.MapPoints.Count; iPoint++)
                {
                    if (aerodynamicMap.MapPoints[iPoint].RearRideHeight == aerodynamicMapRearRideHeights.Max())
                    {
                        newAerodynamicMap.MapPoints.Add(aerodynamicMap.MapPoints[iPoint]);
                    }
                }
            }
            else
            {
                newAerodynamicMap = _InterpolateAerodynamicMapByRearRideHeight(aerodynamicMap, rearRideHeight);
            }
            return newAerodynamicMap;
        }

        /// <summary>
        /// Gets a aerodynamic map by interpolation based on rear ride height.
        /// </summary>
        /// <param name="aerodynamicMap"> Aerodynamic map to be interpolated. </param>
        /// <param name="rearRideHeight"> Desired rear ride height [m]. </param>
        /// <returns> An aerodynamic map interpolated by rear ride height. </returns>
        private TwoWheelAerodynamicMap _InterpolateAerodynamicMapByRearRideHeight(TwoWheelAerodynamicMap aerodynamicMap, double rearRideHeight)
        {
            TwoWheelAerodynamicMap interpolatedAerodynamicMap = new TwoWheelAerodynamicMap() { MapPoints = new List<TwoWheelAerodynamicMapPoint>() };
            // Finds the map's next lower speed compared to the wind speed
            int iRearRideHeight = 0;
            while (rearRideHeight <= aerodynamicMapFrontRideHeights[iRearRideHeight]) iRearRideHeight++;
            // Lower and higher wind speeds
            double lowerRearRideHeight = aerodynamicMapFrontRideHeights[iRearRideHeight];
            double higherRearRideHeight = aerodynamicMapFrontRideHeights[iRearRideHeight + 1];
            // Lower and upper aerodynamic maps lists initialization
            TwoWheelAerodynamicMap lowerAerodynamicMap = new TwoWheelAerodynamicMap() { MapPoints = new List<TwoWheelAerodynamicMapPoint>() };
            TwoWheelAerodynamicMap higherAerodynamicMap = new TwoWheelAerodynamicMap() { MapPoints = new List<TwoWheelAerodynamicMapPoint>() };
            // Gets the objects of the lower and upper aerodynamic maps
            for (int row = 0; row < aerodynamicMap.MapPoints.Count; row++)
            {
                if (AerodynamicMap.MapPoints[row].RearRideHeight == lowerRearRideHeight)
                    lowerAerodynamicMap.MapPoints.Add(aerodynamicMap.MapPoints[row]);
                else if (AerodynamicMap.MapPoints[row].RearRideHeight == higherRearRideHeight)
                    higherAerodynamicMap.MapPoints.Add(aerodynamicMap.MapPoints[row]);
            }
            // Linear interpolation ratio
            double interpolationRatio = (rearRideHeight - lowerRearRideHeight) / (higherRearRideHeight - lowerRearRideHeight);
            // Interpolated map generation
            for (int iLowerRearRideHeight = 0; iLowerRearRideHeight < lowerAerodynamicMap.MapPoints.Count; iLowerRearRideHeight++)
            {
                for (int iHigherRearRideHeight = 0; iHigherRearRideHeight < higherAerodynamicMap.MapPoints.Count; iHigherRearRideHeight++)
                {
                    TwoWheelAerodynamicMapPoint lowerAerodynamicMapPoint = lowerAerodynamicMap.MapPoints[iLowerRearRideHeight];
                    TwoWheelAerodynamicMapPoint higherAerodynamicMapPoint = higherAerodynamicMap.MapPoints[iHigherRearRideHeight];
                    if (lowerAerodynamicMapPoint.WindRelativeSpeed == higherAerodynamicMapPoint.WindRelativeSpeed && lowerAerodynamicMapPoint.FrontRideHeight == higherAerodynamicMapPoint.FrontRideHeight && lowerAerodynamicMapPoint.CarSlipAngle == higherAerodynamicMapPoint.CarSlipAngle)
                    {
                        // Interpolated aerodynamic map point
                        TwoWheelAerodynamicMapPoint aerodynamicMapPoint = new TwoWheelAerodynamicMapPoint
                        {
                            WindRelativeSpeed = lowerAerodynamicMapPoint.WindRelativeSpeed,
                            FrontRideHeight = lowerAerodynamicMapPoint.FrontRideHeight,
                            RearRideHeight = rearRideHeight,
                            CarSlipAngle = lowerAerodynamicMapPoint.CarSlipAngle,
                            DragCoefficient = lowerAerodynamicMapPoint.DragCoefficient + (higherAerodynamicMapPoint.DragCoefficient - lowerAerodynamicMapPoint.DragCoefficient) * interpolationRatio,
                            SideForceCoefficient = lowerAerodynamicMapPoint.SideForceCoefficient + (higherAerodynamicMapPoint.SideForceCoefficient - lowerAerodynamicMapPoint.SideForceCoefficient) * interpolationRatio,
                            LiftCoefficient = lowerAerodynamicMapPoint.LiftCoefficient + (higherAerodynamicMapPoint.LiftCoefficient - lowerAerodynamicMapPoint.LiftCoefficient) * interpolationRatio,
                            PitchMomentCoefficient = lowerAerodynamicMapPoint.PitchMomentCoefficient + (higherAerodynamicMapPoint.PitchMomentCoefficient - lowerAerodynamicMapPoint.PitchMomentCoefficient) * interpolationRatio,
                            YawMomentCoefficient = lowerAerodynamicMapPoint.YawMomentCoefficient + (higherAerodynamicMapPoint.YawMomentCoefficient - lowerAerodynamicMapPoint.YawMomentCoefficient) * interpolationRatio
                        };
                        // Registering of the point
                        interpolatedAerodynamicMap.MapPoints.Add(aerodynamicMapPoint);
                    }
                }
            }
            return interpolatedAerodynamicMap;
        }

        /// <summary>
        /// Gets an aerodynamic map based on an initial aerodynamic map and a car slip angle.
        /// </summary>
        /// <param name="aerodynamicMap"> Initial aerodynamic map. </param>
        /// <param name="carSlipAngle"> Car slip angle to determine the new aerodynamic map [rad]. </param>
        /// <returns> Aerodynamic map determined by the car slip angle. </returns>
        private TwoWheelAerodynamicMap _GetAerodynamicMapFromCarSlipAngle(TwoWheelAerodynamicMap aerodynamicMap, double carSlipAngle)
        {
            // Interpolated aerodynamic map intialization
            TwoWheelAerodynamicMap newAerodynamicMap = new TwoWheelAerodynamicMap() { MapPoints = new List<TwoWheelAerodynamicMapPoint>() };
            // Checks if the desired wind speed is in lower, higher or in the range of the aerodynamic map.
            if (carSlipAngle <= aerodynamicMapCarSlipAngles.Min())
            {
                for (int iPoint = 0; iPoint < aerodynamicMap.MapPoints.Count; iPoint++)
                {
                    if (aerodynamicMap.MapPoints[iPoint].CarSlipAngle == aerodynamicMapCarSlipAngles.Min())
                    {
                        newAerodynamicMap.MapPoints.Add(aerodynamicMap.MapPoints[iPoint]);
                    }
                }
            }
            else if (carSlipAngle >= aerodynamicMapCarSlipAngles.Max())
            {
                for (int iPoint = 0; iPoint < aerodynamicMap.MapPoints.Count; iPoint++)
                {
                    if (aerodynamicMap.MapPoints[iPoint].CarSlipAngle == aerodynamicMapCarSlipAngles.Max())
                    {
                        newAerodynamicMap.MapPoints.Add(aerodynamicMap.MapPoints[iPoint]);
                    }
                }
            }
            else
            {
                newAerodynamicMap = _InterpolateAerodynamicMapByCarSlipAngle(aerodynamicMap, carSlipAngle);
            }
            return newAerodynamicMap;
        }

        /// <summary>
        /// Gets a aerodynamic map by interpolation based on car slip angle.
        /// </summary>
        /// <param name="aerodynamicMap"> Aerodynamic map to be interpolated. </param>
        /// <param name="carSlipAngle"> Desired car slip angle [rad]. </param>
        /// <returns> An aerodynamic map interpolated by car slip angle. </returns>
        private TwoWheelAerodynamicMap _InterpolateAerodynamicMapByCarSlipAngle(TwoWheelAerodynamicMap aerodynamicMap, double carSlipAngle)
        {
            TwoWheelAerodynamicMap interpolatedAerodynamicMap = new TwoWheelAerodynamicMap() { MapPoints = new List<TwoWheelAerodynamicMapPoint>() };
            // Finds the map's next lower speed compared to the wind speed
            int iCarSlipAngle = 0;
            while (carSlipAngle <= aerodynamicMapFrontRideHeights[iCarSlipAngle]) iCarSlipAngle++;
            // Lower and higher wind speeds
            double lowerCarSlipAngle = aerodynamicMapFrontRideHeights[iCarSlipAngle];
            double higherCarSlipAngle = aerodynamicMapFrontRideHeights[iCarSlipAngle + 1];
            // Lower and upper aerodynamic maps lists initialization
            TwoWheelAerodynamicMap lowerAerodynamicMap = new TwoWheelAerodynamicMap() { MapPoints = new List<TwoWheelAerodynamicMapPoint>() };
            TwoWheelAerodynamicMap higherAerodynamicMap = new TwoWheelAerodynamicMap() { MapPoints = new List<TwoWheelAerodynamicMapPoint>() };
            // Gets the objects of the lower and upper aerodynamic maps
            for (int row = 0; row < aerodynamicMap.MapPoints.Count; row++)
            {
                if (AerodynamicMap.MapPoints[row].CarSlipAngle == lowerCarSlipAngle)
                    lowerAerodynamicMap.MapPoints.Add(aerodynamicMap.MapPoints[row]);
                else if (AerodynamicMap.MapPoints[row].CarSlipAngle == higherCarSlipAngle)
                    higherAerodynamicMap.MapPoints.Add(aerodynamicMap.MapPoints[row]);
            }
            // Linear interpolation ratio
            double interpolationRatio = (carSlipAngle - lowerCarSlipAngle) / (higherCarSlipAngle - lowerCarSlipAngle);
            // Interpolated map generation
            for (int iLowerCarSlipAngle = 0; iLowerCarSlipAngle < lowerAerodynamicMap.MapPoints.Count; iLowerCarSlipAngle++)
            {
                for (int iHigherCarSlipAngle = 0; iHigherCarSlipAngle < higherAerodynamicMap.MapPoints.Count; iHigherCarSlipAngle++)
                {
                    TwoWheelAerodynamicMapPoint lowerAerodynamicMapPoint = lowerAerodynamicMap.MapPoints[iLowerCarSlipAngle];
                    TwoWheelAerodynamicMapPoint higherAerodynamicMapPoint = higherAerodynamicMap.MapPoints[iHigherCarSlipAngle];
                    if (lowerAerodynamicMapPoint.WindRelativeSpeed == higherAerodynamicMapPoint.WindRelativeSpeed && lowerAerodynamicMapPoint.FrontRideHeight == higherAerodynamicMapPoint.FrontRideHeight && lowerAerodynamicMapPoint.RearRideHeight == higherAerodynamicMapPoint.RearRideHeight)
                    {
                        // Interpolated aerodynamic map point
                        TwoWheelAerodynamicMapPoint aerodynamicMapPoint = new TwoWheelAerodynamicMapPoint
                        {
                            WindRelativeSpeed = lowerAerodynamicMapPoint.WindRelativeSpeed,
                            FrontRideHeight = lowerAerodynamicMapPoint.FrontRideHeight,
                            RearRideHeight = lowerAerodynamicMapPoint.RearRideHeight,
                            CarSlipAngle = carSlipAngle,
                            DragCoefficient = lowerAerodynamicMapPoint.DragCoefficient + (higherAerodynamicMapPoint.DragCoefficient - lowerAerodynamicMapPoint.DragCoefficient) * interpolationRatio,
                            SideForceCoefficient = lowerAerodynamicMapPoint.SideForceCoefficient + (higherAerodynamicMapPoint.SideForceCoefficient - lowerAerodynamicMapPoint.SideForceCoefficient) * interpolationRatio,
                            LiftCoefficient = lowerAerodynamicMapPoint.LiftCoefficient + (higherAerodynamicMapPoint.LiftCoefficient - lowerAerodynamicMapPoint.LiftCoefficient) * interpolationRatio,
                            PitchMomentCoefficient = lowerAerodynamicMapPoint.PitchMomentCoefficient + (higherAerodynamicMapPoint.PitchMomentCoefficient - lowerAerodynamicMapPoint.PitchMomentCoefficient) * interpolationRatio,
                            YawMomentCoefficient = lowerAerodynamicMapPoint.YawMomentCoefficient + (higherAerodynamicMapPoint.YawMomentCoefficient - lowerAerodynamicMapPoint.YawMomentCoefficient) * interpolationRatio
                        };
                        // Registering of the point
                        interpolatedAerodynamicMap.MapPoints.Add(aerodynamicMapPoint);
                    }
                }
            }
            return interpolatedAerodynamicMap;
        }
        #endregion
    }
}
