using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternshipTest.Vehicle
{
    /// <summary>
    /// Contais the information about a one wheel model's vehicle aerodynamics.
    /// </summary>
    [Serializable]
    public class OneWheelAerodynamics : Aerodynamics
    {
        #region Fields
        /// <summary>
        /// Relative wind speeds of the aerodynamic map [m].
        /// </summary>
        private List<double> aerodynamicMapWindSpeeds;
        /// <summary>
        /// Ride heights of the aerodynamic map [m].
        /// </summary>
        private List<double> aerodynamicMapRideHeights;
        #endregion
        #region Properties
        /// <summary>
        /// Aerodynamic map properties (wind speed, ride height, CD and CL).
        /// </summary>
        public OneWheelAerodynamicMap AerodynamicMap { get; set; }
        #endregion
        #region Constructors
        public OneWheelAerodynamics() { }
        public OneWheelAerodynamics(string aeroID, string description, OneWheelAerodynamicMap aeroMap, double frontalArea, double airDensity)
        {
            ID = aeroID;
            Description = description;
            AerodynamicMap = aeroMap;
            FrontalArea = Math.Abs(frontalArea);
            AirDensity = Math.Abs(airDensity);
            aerodynamicMapWindSpeeds = new List<double>();
            aerodynamicMapRideHeights = new List<double>();
        }
        #endregion
        #region Methods
        /// <summary>
        /// Gets the aerodynamic map's parameters.
        /// </summary>
        public void GetAerodynamicMapParameters()
        {
            // Aerodyanmic map row sweep "for" loop
            foreach (OneWheelAerodynamicMapPoint aerodynamicMapPoint in AerodynamicMap.MapPoints)
            {
                // Current row parameters
                double aerodynamicMapRowWindSpeed = aerodynamicMapPoint.WindRelativeSpeed;
                double aerodynamicMapRowrideHeight = aerodynamicMapPoint.RideHeight;
                // New parameter indicators initialization
                bool isNewWindSpeed = true;
                bool isNewrideHeight = true;
                // New parameter indicators value determination
                foreach (double windSpeed in aerodynamicMapWindSpeeds)
                {
                    if (aerodynamicMapRowWindSpeed == windSpeed) isNewWindSpeed = false;
                }
                foreach (double rideHeight in aerodynamicMapRideHeights)
                {
                    if (aerodynamicMapRowrideHeight == rideHeight) isNewrideHeight = false;
                }
                // New parameter values registration
                if (isNewWindSpeed) aerodynamicMapWindSpeeds.Add(aerodynamicMapRowWindSpeed);
                if (isNewrideHeight) aerodynamicMapRideHeights.Add(aerodynamicMapRowrideHeight);
            }
            // Sort to ascending order
            aerodynamicMapWindSpeeds = aerodynamicMapWindSpeeds.OrderBy(d => d).ToList();
            aerodynamicMapRideHeights = aerodynamicMapRideHeights.OrderBy(d => d).ToList();
        }
        
        /// <summary>
        /// Gets an aerodynamic map point based on an aerodynamic map and input parameters.
        /// </summary>
        /// <param name="windSpeed"> Relative speed of the wind [m/s]. </param>
        /// <param name="rideHeight"> Distance from the ground to the car in the vertical axis [m]. </param>
        /// <returns> Interpolated aerodynamic map point (wind speed, ride height, CD and CL). </returns>
        public OneWheelAerodynamicMapPoint GetAerodynamicMapPointFromParameters(double windSpeed, double rideHeight)
        {
            OneWheelAerodynamicMap newAerodynamicMap = new OneWheelAerodynamicMap() { MapPoints = AerodynamicMap.MapPoints };
            newAerodynamicMap = _GetAerodynamicMapFromWindSpeed(newAerodynamicMap, windSpeed);
            newAerodynamicMap = _GetAerodynamicMapFromRideHeight(newAerodynamicMap, rideHeight);
            return newAerodynamicMap.MapPoints[0];
        }

        /// <summary>
        /// Gets an aerodynamic map based on an initial aerodynamic map and a wind speed.
        /// </summary>
        /// <param name="aerodynamicMap"> Initial aerodynamic map. </param>
        /// <param name="windSpeed"> Wind speed to determine the new aerodynamic map [m/s]. </param>
        /// <returns> Aerodynamic map determined by the wind speed. </returns>
        private OneWheelAerodynamicMap _GetAerodynamicMapFromWindSpeed(OneWheelAerodynamicMap aerodynamicMap, double windSpeed)
        {
            // Interpolated aerodynamic map intialization
            OneWheelAerodynamicMap newAerodynamicMap = new OneWheelAerodynamicMap() { MapPoints = new List<OneWheelAerodynamicMapPoint>() };
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
        private OneWheelAerodynamicMap _InterpolateAerodynamicMapByWindSpeed(OneWheelAerodynamicMap aerodynamicMap, double windSpeed)
        {
            OneWheelAerodynamicMap interpolatedAerodynamicMap = new OneWheelAerodynamicMap() { MapPoints = new List<OneWheelAerodynamicMapPoint>() };
            // Finds the map's next lower speed compared to the wind speed
            int iWindSpeed = 0;
            while (windSpeed <= aerodynamicMapWindSpeeds[iWindSpeed]) iWindSpeed++;
            // Lower and higher wind speeds
            double lowerWindSpeed = aerodynamicMapWindSpeeds[iWindSpeed];
            double higherWindSpeed = aerodynamicMapWindSpeeds[iWindSpeed + 1];
            // Lower and upper aerodynamic maps lists initialization
            OneWheelAerodynamicMap lowerAerodynamicMap = new OneWheelAerodynamicMap() { MapPoints = new List<OneWheelAerodynamicMapPoint>() };
            OneWheelAerodynamicMap higherAerodynamicMap = new OneWheelAerodynamicMap() { MapPoints = new List<OneWheelAerodynamicMapPoint>() };
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
                    OneWheelAerodynamicMapPoint lowerAerodynamicMapPoint = lowerAerodynamicMap.MapPoints[iLowerWindSpeed];
                    OneWheelAerodynamicMapPoint higherAerodynamicMapPoint = higherAerodynamicMap.MapPoints[iHigherWindSpeed];
                    if (lowerAerodynamicMapPoint.RideHeight == higherAerodynamicMapPoint.RideHeight)
                    {
                        // Interpolated aerodynamic map point
                        OneWheelAerodynamicMapPoint aerodynamicMapPoint = new OneWheelAerodynamicMapPoint
                        {
                            WindRelativeSpeed = windSpeed,
                            RideHeight = lowerAerodynamicMapPoint.RideHeight,
                            DragCoefficient = lowerAerodynamicMapPoint.DragCoefficient + (higherAerodynamicMapPoint.DragCoefficient - lowerAerodynamicMapPoint.DragCoefficient) * interpolationRatio,
                            LiftCoefficient = lowerAerodynamicMapPoint.LiftCoefficient + (higherAerodynamicMapPoint.LiftCoefficient - lowerAerodynamicMapPoint.LiftCoefficient) * interpolationRatio
                        };
                        // Registering of the point
                        interpolatedAerodynamicMap.MapPoints.Add(aerodynamicMapPoint);
                    }
                }
            }
            return interpolatedAerodynamicMap;
        }

        /// <summary>
        /// Gets an aerodynamic map based on an initial aerodynamic map and a ride height.
        /// </summary>
        /// <param name="aerodynamicMap"> Initial aerodynamic map. </param>
        /// <param name="rideHeight"> Ride height to determine the new aerodynamic map [m]. </param>
        /// <returns> Aerodynamic map determined by the ride height. </returns>
        private OneWheelAerodynamicMap _GetAerodynamicMapFromRideHeight(OneWheelAerodynamicMap aerodynamicMap, double rideHeight)
        {
            // Interpolated aerodynamic map intialization
            OneWheelAerodynamicMap newAerodynamicMap = new OneWheelAerodynamicMap() { MapPoints = new List<OneWheelAerodynamicMapPoint>() };
            // Checks if the desired wind speed is in lower, higher or in the range of the aerodynamic map.
            if (rideHeight <= aerodynamicMapRideHeights.Min())
            {
                for (int iPoint = 0; iPoint < aerodynamicMap.MapPoints.Count; iPoint++)
                {
                    if (aerodynamicMap.MapPoints[iPoint].RideHeight == aerodynamicMapRideHeights.Min())
                    {
                        newAerodynamicMap.MapPoints.Add(aerodynamicMap.MapPoints[iPoint]);
                    }
                }
            }
            else if (rideHeight >= aerodynamicMapRideHeights.Max())
            {
                for (int iPoint = 0; iPoint < aerodynamicMap.MapPoints.Count; iPoint++)
                {
                    if (aerodynamicMap.MapPoints[iPoint].RideHeight == aerodynamicMapRideHeights.Max())
                    {
                        newAerodynamicMap.MapPoints.Add(aerodynamicMap.MapPoints[iPoint]);
                    }
                }
            }
            else
            {
                newAerodynamicMap = _InterpolateAerodynamicMapByRideHeight(aerodynamicMap, rideHeight);
            }
            return newAerodynamicMap;
        }

        /// <summary>
        /// Gets a aerodynamic map by interpolation based on ride height.
        /// </summary>
        /// <param name="aerodynamicMap"> Aerodynamic map to be interpolated. </param>
        /// <param name="rideHeight"> Desired ride height [m]. </param>
        /// <returns> An aerodynamic map interpolated by ride height. </returns>
        private OneWheelAerodynamicMap _InterpolateAerodynamicMapByRideHeight(OneWheelAerodynamicMap aerodynamicMap, double rideHeight)
        {
            OneWheelAerodynamicMap interpolatedAerodynamicMap = new OneWheelAerodynamicMap() { MapPoints = new List<OneWheelAerodynamicMapPoint>() };
            // Finds the map's next lower speed compared to the wind speed
            int iRideHeight = 0;
            while (rideHeight <= aerodynamicMapRideHeights[iRideHeight]) iRideHeight++;
            // Lower and higher wind speeds
            double lowerRideHeight = aerodynamicMapRideHeights[iRideHeight];
            double higherRideHeight = aerodynamicMapRideHeights[iRideHeight + 1];
            // Lower and upper aerodynamic maps lists initialization
            OneWheelAerodynamicMap lowerAerodynamicMap = new OneWheelAerodynamicMap() { MapPoints = new List<OneWheelAerodynamicMapPoint>() };
            OneWheelAerodynamicMap higherAerodynamicMap = new OneWheelAerodynamicMap() { MapPoints = new List<OneWheelAerodynamicMapPoint>() };
            // Gets the objects of the lower and upper aerodynamic maps
            for (int row = 0; row < aerodynamicMap.MapPoints.Count; row++)
            {
                if (AerodynamicMap.MapPoints[row].RideHeight == lowerRideHeight)
                    lowerAerodynamicMap.MapPoints.Add(aerodynamicMap.MapPoints[row]);
                else if (AerodynamicMap.MapPoints[row].RideHeight == higherRideHeight)
                    higherAerodynamicMap.MapPoints.Add(aerodynamicMap.MapPoints[row]);
            }
            // Linear interpolation ratio
            double interpolationRatio = (rideHeight - lowerRideHeight) / (higherRideHeight - lowerRideHeight);
            // Interpolated map generation
            for (int iLowerRideHeight = 0; iLowerRideHeight < lowerAerodynamicMap.MapPoints.Count; iLowerRideHeight++)
            {
                for (int iHigherRideHeight = 0; iHigherRideHeight < higherAerodynamicMap.MapPoints.Count; iHigherRideHeight++)
                {
                    OneWheelAerodynamicMapPoint lowerAerodynamicMapPoint = lowerAerodynamicMap.MapPoints[iLowerRideHeight];
                    OneWheelAerodynamicMapPoint higherAerodynamicMapPoint = higherAerodynamicMap.MapPoints[iHigherRideHeight];
                    if (lowerAerodynamicMapPoint.WindRelativeSpeed == higherAerodynamicMapPoint.WindRelativeSpeed)
                    {
                        // Interpolated aerodynamic map point
                        OneWheelAerodynamicMapPoint aerodynamicMapPoint = new OneWheelAerodynamicMapPoint
                        {
                            WindRelativeSpeed = lowerAerodynamicMapPoint.WindRelativeSpeed,
                            RideHeight = rideHeight,
                            DragCoefficient = lowerAerodynamicMapPoint.DragCoefficient + (higherAerodynamicMapPoint.DragCoefficient - lowerAerodynamicMapPoint.DragCoefficient) * interpolationRatio,
                            LiftCoefficient = lowerAerodynamicMapPoint.LiftCoefficient + (higherAerodynamicMapPoint.LiftCoefficient - lowerAerodynamicMapPoint.LiftCoefficient) * interpolationRatio
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
