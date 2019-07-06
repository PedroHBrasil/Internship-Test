using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternshipTest.Vehicle
{
    /// <summary>
    /// Contains the aerodynamics information of a two wheel vehicle model object.
    /// </summary>
    [Serializable]
    public class TwoWheelAerodynamics : Aerodynamics
    {
        #region Fields
        /// <summary>
        /// Front ride heights of the aerodynamic map [m].
        /// </summary>
        public List<double> aerodynamicMapFrontRideHeights;
        /// <summary>
        /// Rear ride heights of the aerodynamic map [m].
        /// </summary>
        public List<double> aerodynamicMapRearRideHeights;
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
            aerodynamicMapFrontRideHeights = new List<double>();
            aerodynamicMapRearRideHeights = new List<double>();
            // Aerodyanmic map row sweep "for" loop
            foreach (TwoWheelAerodynamicMapPoint aerodynamicMapPoint in AerodynamicMap.MapPoints)
            {
                // Current row parameters
                double aerodynamicMapRowFrontRideHeight = aerodynamicMapPoint.FrontRideHeight;
                double aerodynamicMapRowRearRideHeight = aerodynamicMapPoint.RearRideHeight;
                // New parameter indicators initialization
                bool isNewFrontRideHeight = true;
                bool isNewRearRideHeight = true;
                // New parameter indicators value determination
                foreach (double frontRideHeight in aerodynamicMapFrontRideHeights)
                {
                    if (aerodynamicMapRowFrontRideHeight == frontRideHeight) isNewFrontRideHeight = false;
                }
                foreach (double rearRideHeight in aerodynamicMapRearRideHeights)
                {
                    if (aerodynamicMapRowRearRideHeight == rearRideHeight) isNewRearRideHeight = false;
                }
                // New parameter values registration
                if (isNewFrontRideHeight) aerodynamicMapFrontRideHeights.Add(aerodynamicMapRowFrontRideHeight);
                if (isNewRearRideHeight) aerodynamicMapRearRideHeights.Add(aerodynamicMapRowRearRideHeight);
            }
            // Sort to ascending order
            aerodynamicMapFrontRideHeights = aerodynamicMapFrontRideHeights.OrderBy(d => d).ToList();
            aerodynamicMapRearRideHeights = aerodynamicMapRearRideHeights.OrderBy(d => d).ToList();
        }

        /// <summary>
        /// Gets an aerodynamic map point based on an aerodynamic map and input parameters.
        /// </summary>
        /// <param name="windSpeed"> Relative speed of the wind [m/s]. </param>
        /// <param name="frontRideHeight"> Distance from the ground to the front of the car in the vertical axis [m]. </param>
        /// <param name="rearRideHeight"> Distance from the ground to the rear of the car in the vertical axis [m]. </param>
        /// <param name="carSlipAngle"> Car slip angle [rad]. </param>
        /// <returns> Interpolated aerodynamic map point (wind speed, front ride height, rear ride height, car slip angle, CD, CS, CL, CPM and CYM). </returns>
        public TwoWheelAerodynamicMapPoint GetAerodynamicMapPointFromParameters(double frontRideHeight, double rearRideHeight)
        {
            TwoWheelAerodynamicMap newAerodynamicMap = new TwoWheelAerodynamicMap() { MapPoints = AerodynamicMap.MapPoints };
            newAerodynamicMap = _GetAerodynamicMapFromFrontRideHeight(newAerodynamicMap, frontRideHeight);
            newAerodynamicMap = _GetAerodynamicMapFromRearRideHeight(newAerodynamicMap, rearRideHeight);
            return newAerodynamicMap.MapPoints[0];
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
            while (frontRideHeight >= aerodynamicMapFrontRideHeights[iFrontRideHeight]) iFrontRideHeight++;
            // Lower and higher wind speeds
            double lowerFrontRideHeight = aerodynamicMapFrontRideHeights[iFrontRideHeight - 1];
            double higherFrontRideHeight = aerodynamicMapFrontRideHeights[iFrontRideHeight];
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
                    if (lowerAerodynamicMapPoint.RearRideHeight == higherAerodynamicMapPoint.RearRideHeight)
                    {
                        // Interpolated aerodynamic map point
                        TwoWheelAerodynamicMapPoint aerodynamicMapPoint = new TwoWheelAerodynamicMapPoint
                        {
                            FrontRideHeight = frontRideHeight,
                            RearRideHeight = lowerAerodynamicMapPoint.RearRideHeight,
                            DragCoefficient = lowerAerodynamicMapPoint.DragCoefficient + (higherAerodynamicMapPoint.DragCoefficient - lowerAerodynamicMapPoint.DragCoefficient) * interpolationRatio,
                            LiftCoefficient = lowerAerodynamicMapPoint.LiftCoefficient + (higherAerodynamicMapPoint.LiftCoefficient - lowerAerodynamicMapPoint.LiftCoefficient) * interpolationRatio,
                            DownforceDistribution = lowerAerodynamicMapPoint.DownforceDistribution + (higherAerodynamicMapPoint.DownforceDistribution - lowerAerodynamicMapPoint.DownforceDistribution) * interpolationRatio
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
            while (rearRideHeight >= aerodynamicMapRearRideHeights[iRearRideHeight]) iRearRideHeight++;
            // Lower and higher wind speeds
            double lowerRearRideHeight = aerodynamicMapRearRideHeights[iRearRideHeight - 1];
            double higherRearRideHeight = aerodynamicMapRearRideHeights[iRearRideHeight];
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
                    if (lowerAerodynamicMapPoint.FrontRideHeight == higherAerodynamicMapPoint.FrontRideHeight)
                    {
                        // Interpolated aerodynamic map point
                        TwoWheelAerodynamicMapPoint aerodynamicMapPoint = new TwoWheelAerodynamicMapPoint
                        {
                            FrontRideHeight = lowerAerodynamicMapPoint.FrontRideHeight,
                            RearRideHeight = rearRideHeight,
                            DragCoefficient = lowerAerodynamicMapPoint.DragCoefficient + (higherAerodynamicMapPoint.DragCoefficient - lowerAerodynamicMapPoint.DragCoefficient) * interpolationRatio,
                            LiftCoefficient = lowerAerodynamicMapPoint.LiftCoefficient + (higherAerodynamicMapPoint.LiftCoefficient - lowerAerodynamicMapPoint.LiftCoefficient) * interpolationRatio,
                            DownforceDistribution = lowerAerodynamicMapPoint.DownforceDistribution + (higherAerodynamicMapPoint.DownforceDistribution - lowerAerodynamicMapPoint.DownforceDistribution) * interpolationRatio
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
