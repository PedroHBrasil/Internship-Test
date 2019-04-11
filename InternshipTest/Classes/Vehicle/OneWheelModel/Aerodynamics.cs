using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternshipTest.Vehicle.OneWheel
{
    /// <summary>
    /// Contais the information about a one wheel model's vehicle aerodynamics.
    /// </summary>
    public class Aerodynamics : GenericInfo
    {
        #region Properties
        /// <summary>
        /// Aerodynamic map properties (wind speed, ride height, CD and CL).
        /// </summary>
        public AerodynamicMap AerodynamicMap { get; set; }
        /// <summary>
        /// Area of the vehicle's projection over the front plane projection [m²].
        /// </summary>
        public double FrontalArea { get; set; }
        /// <summary>
        /// Density of the environment air [kg/m³].
        /// </summary>
        public double AirDensity { get; set; }
        /// <summary>
        /// Wind speeds of the aerodynamic map [m/s].
        /// </summary>
        private List<double> AerodynamicMapWindSpeeds { get; set; }
        /// <summary>
        /// Ride heights of the aerodynamic map [m].
        /// </summary>
        private List<double> AerodynamicMapRideHeights { get; set; }
        #endregion
        #region Constructors
        public Aerodynamics() { }
        public Aerodynamics(string aeroID, string description, AerodynamicMap aeroMap, double frontalArea, double airDensity)
        {
            ID = aeroID;
            Description = description;
            AerodynamicMap = aeroMap;
            FrontalArea = Math.Abs(frontalArea);
            AirDensity = Math.Abs(airDensity);
            AerodynamicMapWindSpeeds = new List<double>();
            AerodynamicMapRideHeights = new List<double>();
        }
        #endregion
        #region Methods
        public override string ToString()
        {
            return ID;
        }
        /// <summary>
        /// Gets the aerodynamic map's wind speeds and ride heights.
        /// </summary>
        public void GetAerodynamicMapParameters()
        {
            // Aerodyanmic map row sweep "for" loop
            foreach (AerodynamicMapPoint aerodynamicMapPoint in AerodynamicMap.MapPoints)
            {
                // Current row parameters
                double aerodynamicMapRowWindSpeed = aerodynamicMapPoint.WindRelativeSpeed;
                double aerodynamicMapRowrideHeight = aerodynamicMapPoint.RideHeight;
                // New parameter indicators initialization
                bool isNewWindSpeed = true;
                bool isNewrideHeight = true;
                // New parameter indicators value determination
                foreach (double windSpeed in AerodynamicMapWindSpeeds)
                {
                    if (aerodynamicMapRowWindSpeed == windSpeed) isNewWindSpeed = false;
                }
                foreach (double rideHeight in AerodynamicMapRideHeights)
                {
                    if (aerodynamicMapRowrideHeight == rideHeight) isNewrideHeight = false;
                }
                // New parameter values registration
                if (isNewWindSpeed) AerodynamicMapWindSpeeds.Add(aerodynamicMapRowWindSpeed);
                if (isNewrideHeight) AerodynamicMapRideHeights.Add(aerodynamicMapRowrideHeight);
            }
            // Sort to ascending order
            AerodynamicMapWindSpeeds = AerodynamicMapWindSpeeds.OrderBy(d => d).ToList();
            AerodynamicMapRideHeights = AerodynamicMapRideHeights.OrderBy(d => d).ToList();
        }

        /// <summary>
        /// Interpolates the aerodynamic map to get aerodynamic coefficients.
        /// </summary>
        /// <param name="windSpeed"> Relative speed of the wind [m/s]. </param>
        /// <param name="rideHeight"> Distance between the car and the ground [m]. </param>
        /// <returns> Interpolated aerodynamic map point (wind speed, ride height, CD and CL). </returns>
        public AerodynamicMapPoint InterpolateAerodynamicMap(double windSpeed, double rideHeight)
        {
            AerodynamicMapPoint interpolatedAerodynamicMapPoint = new AerodynamicMapPoint();
            // Map interpolation based on the wind speed
            // Interpolated map list initialization
            List<AerodynamicMapPoint> aerodynamicMapInterpolatedByWindSpeed = new List<AerodynamicMapPoint>();
            // Checks if the wind speed is lower, higher or in the range of the aerodynamic map
            if (windSpeed <= AerodynamicMapWindSpeeds.Min()) // Case speed is lower
            {
                // Aerodynamic map rows sweep "for" loop
                for (int row = 0; row < AerodynamicMap.MapPoints.Count; row++)
                {
                    // Checks if the speed in this row is the lowest speed of the map and gets the object if it is
                    if (AerodynamicMap.MapPoints[row].WindRelativeSpeed == AerodynamicMapWindSpeeds.Min())
                        aerodynamicMapInterpolatedByWindSpeed.Add(AerodynamicMap.MapPoints[row]);
                }
            }
            else if (windSpeed >= AerodynamicMapWindSpeeds.Max()) // Case speed is higher
            {
                // Aerodynamic map rows sweep "for" loop
                for (int row = 0; row < AerodynamicMap.MapPoints.Count; row++)
                {
                    // Checks if the speed in this row is the highest speed of the map and gets the object if it is
                    if (AerodynamicMap.MapPoints[row].WindRelativeSpeed == AerodynamicMapWindSpeeds.Max())
                        aerodynamicMapInterpolatedByWindSpeed.Add(AerodynamicMap.MapPoints[row]);
                }
            }
            else // Case the speed is in the aerodynamic map values interval
            {
                // Finds the map's next lower speed compared to the wind speed
                int iWindSpeed = 0;
                while (windSpeed <= AerodynamicMapWindSpeeds[iWindSpeed]) iWindSpeed++;
                // Lower and higher wind speeds
                double lowerWindSpeed = AerodynamicMapWindSpeeds[iWindSpeed];
                double higherWindSpeed = AerodynamicMapWindSpeeds[iWindSpeed + 1];
                // Lower and upper aerodynamic maps lists initialization
                List<AerodynamicMapPoint> lowerAerodynamicMapPoints = new List<AerodynamicMapPoint>();
                List<AerodynamicMapPoint> higherAerodynamicMapPoints = new List<AerodynamicMapPoint>();
                // Gets the objects of the lower and upper aerodynamic maps
                for (int row = 0; row < AerodynamicMap.MapPoints.Count; row++)
                {
                    if (AerodynamicMap.MapPoints[row].WindRelativeSpeed == lowerWindSpeed)
                        lowerAerodynamicMapPoints.Add(AerodynamicMap.MapPoints[row]);
                    else if (AerodynamicMap.MapPoints[row].WindRelativeSpeed == higherWindSpeed)
                        higherAerodynamicMapPoints.Add(AerodynamicMap.MapPoints[row]);
                }
                // Linear interpolation ratio
                double windSpeedInterpolationRatio = (windSpeed - lowerWindSpeed) / (higherWindSpeed - lowerWindSpeed);
                // Interpolated map generation
                for (int iLowerWindSpeed = 0; iLowerWindSpeed < lowerAerodynamicMapPoints.Count; iLowerWindSpeed++)
                {
                    for (int iHigherWindSpeed = 0; iHigherWindSpeed < higherAerodynamicMapPoints.Count; iHigherWindSpeed++)
                    {
                        AerodynamicMapPoint lowerAerodynamicMapPoint = lowerAerodynamicMapPoints[iLowerWindSpeed];
                        AerodynamicMapPoint higherAerodynamicMapPoint = higherAerodynamicMapPoints[iHigherWindSpeed];
                        if (lowerAerodynamicMapPoint.RideHeight == higherAerodynamicMapPoint.RideHeight)
                        {
                            // Interpolated aerodynamic map point
                            AerodynamicMapPoint aerodynamicMapPoint = new AerodynamicMapPoint
                            {
                                WindRelativeSpeed = windSpeed,
                                RideHeight = lowerAerodynamicMapPoint.RideHeight,
                                DragCoefficient = lowerAerodynamicMapPoint.DragCoefficient + (higherAerodynamicMapPoint.DragCoefficient - lowerAerodynamicMapPoint.DragCoefficient) * windSpeedInterpolationRatio,
                                LiftCoefficient = lowerAerodynamicMapPoint.LiftCoefficient + (higherAerodynamicMapPoint.LiftCoefficient - lowerAerodynamicMapPoint.LiftCoefficient) * windSpeedInterpolationRatio
                            };
                            // Registering of the point
                            aerodynamicMapInterpolatedByWindSpeed.Add(aerodynamicMapPoint);
                        }
                    }
                }
            }

            // Map interpolation based on the car height
            // Checks if the car height is lower, higher or in the range of the aerodynamic map
            if (rideHeight <= AerodynamicMapRideHeights.Min()) // Case height is lower
            {
                // Aerodynamic map rows sweep "for" loop
                for (int row = 0; row < aerodynamicMapInterpolatedByWindSpeed.Count; row++)
                {
                    // Checks if the height in this row is the lowest height of the map and gets the object if it is
                    if (aerodynamicMapInterpolatedByWindSpeed[row].RideHeight == AerodynamicMapRideHeights.Min())
                        interpolatedAerodynamicMapPoint = aerodynamicMapInterpolatedByWindSpeed[row];
                }
            }
            else if (rideHeight >= AerodynamicMapRideHeights.Max()) // Case height is higher
            {
                // Aerodynamic map rows sweep "for" loop
                for (int row = 0; row < aerodynamicMapInterpolatedByWindSpeed.Count; row++)
                {
                    // Checks if the height in this row is the highest height of the map and gets the object if it is
                    if (aerodynamicMapInterpolatedByWindSpeed[row].RideHeight == AerodynamicMapRideHeights.Max())
                        interpolatedAerodynamicMapPoint = aerodynamicMapInterpolatedByWindSpeed[row];
                }
            }
            else
            {
                // Finds the map's next lower height compared to the car height
                int irideHeight = 0;
                while (rideHeight <= AerodynamicMapRideHeights[irideHeight]) irideHeight++;
                // Lower and higher wind speeds
                double lowerrideHeight = AerodynamicMapRideHeights[irideHeight];
                double higherrideHeight = AerodynamicMapRideHeights[irideHeight + 1];
                // Lower and upper aerodynamic map points initialization
                AerodynamicMapPoint lowerAerodynamicMapPoint = new AerodynamicMapPoint();
                AerodynamicMapPoint higherAerodynamicMapPoint = new AerodynamicMapPoint();
                // Gets the objects of the lower and upper aerodynamic map points
                for (int row = 0; row < aerodynamicMapInterpolatedByWindSpeed.Count; row++)
                {
                    if (aerodynamicMapInterpolatedByWindSpeed[row].RideHeight == lowerrideHeight)
                        lowerAerodynamicMapPoint = aerodynamicMapInterpolatedByWindSpeed[row];
                    else if (aerodynamicMapInterpolatedByWindSpeed[row].RideHeight == higherrideHeight)
                        higherAerodynamicMapPoint = aerodynamicMapInterpolatedByWindSpeed[row];
                }
                // Linear interpolation ratio
                double rideHeightInterpolationRatio = (rideHeight - lowerrideHeight) / (higherrideHeight - lowerrideHeight);
                // Interpolated aerodynamic map point
                AerodynamicMapPoint aerodynamicMapPoint = new AerodynamicMapPoint() {WindRelativeSpeed = windSpeed, RideHeight = rideHeight,
                    DragCoefficient = lowerAerodynamicMapPoint.DragCoefficient + (higherAerodynamicMapPoint.DragCoefficient - lowerAerodynamicMapPoint.DragCoefficient) * rideHeightInterpolationRatio,
                    LiftCoefficient = lowerAerodynamicMapPoint.LiftCoefficient + (higherAerodynamicMapPoint.LiftCoefficient - lowerAerodynamicMapPoint.LiftCoefficient) * rideHeightInterpolationRatio};
                // Method return value
                interpolatedAerodynamicMapPoint = aerodynamicMapPoint;
            }
            return interpolatedAerodynamicMapPoint;
        }

        #endregion
    }

    public class AerodynamicsMethods
    {
        /// <summary>
        /// Gets the aerodynamic drag force.
        /// </summary>
        /// <param name="aerodynamics"> Vehicle's aerodynamics parameters. </param>
        /// <param name="aerodynamicMapPoint"> Aerodynamic map point. </param>
        /// <param name="speed"> Wind's Relative Speed [m/s] </param>
        /// <returns> Aerodynamic drag force [N] </returns>
        public static double GetAerodynamicDrag(Aerodynamics aerodynamics, AerodynamicMapPoint aerodynamicMapPoint, double speed)
        {
            return aerodynamicMapPoint.DragCoefficient * aerodynamics.AirDensity * aerodynamics.FrontalArea * Math.Pow(speed, 2) / 2;
        }
        /// <summary>
        /// Gets the aerodynamic lift force.
        /// </summary>
        /// <param name="aerodynamics"> Vehicle's aerodynamics parameters. </param>
        /// <param name="aerodynamicMapPoint"> Aerodynamic map point. </param>
        /// <param name="speed"> Wind's Relative Speed [m/s] </param>
        /// <returns> Aerodynamic lift force [N] </returns>
        public static double GetAerodynamicLift(Aerodynamics aerodynamics, AerodynamicMapPoint aerodynamicMapPoint, double speed)
        {
            return aerodynamicMapPoint.LiftCoefficient * aerodynamics.AirDensity * aerodynamics.FrontalArea * Math.Pow(speed, 2) / 2;
        }
    }
}
