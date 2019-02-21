using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternshipTest.Vehicle.OneWheel
{
    /*
     * Contains the aerodynamic properties of a car:
     *  - AeroID: string which identifies the object;
     *  - AeroMapPoints: class which defines the aerodynamic map (coefficients as function of speed and car height);
     *  - FrontalArea: double which represents the vehicle's front view projection area [m²]; and
     *  - AirDensity: double which represents the density of the air [kg/m³].
     */
    class Aerodynamics
    {
        // Properties ------------------------------------------------------------------------------------------------------
        public string AeroID { get; set; }
        public List<AerodynamicMapPoint> AerodynamicMapPoints { get; set; }
        public double FrontalArea { get; set; }
        public double AirDensity { get; set; }

        public List<double> AerodynamicMapWindSpeeds { get; set; }
        public List<double> AerodynamicMapCarHeights { get; set; }

        // Constructors ----------------------------------------------------------------------------------------------------
        public Aerodynamics()
        {
            AeroID = "Default";
            AerodynamicMapPoints = new List<AerodynamicMapPoint>();
            FrontalArea = 1; // m²
            AirDensity = 1; // kg/m³
        }
        public Aerodynamics(string aeroID, List<AerodynamicMapPoint> aeroMapPoints, double frontalArea, double airDensity)
        {
            AeroID = aeroID;
            AerodynamicMapPoints = aeroMapPoints;
            FrontalArea = Math.Abs(frontalArea); // m²
            AirDensity = Math.Abs(airDensity); // kg/m³
            AerodynamicMapWindSpeeds = new List<double>();
            AerodynamicMapCarHeights = new List<double>();
            GetAerodynamicMapParameters();
        }
        // Methods ---------------------------------------------------------------------------------------------------------
        public override string ToString()
        {
            return AeroID;
        }

        private void GetAerodynamicMapParameters()
        {
            // Aerodyanmic map row sweep "for" loop
            foreach (AerodynamicMapPoint aerodynamicMapPoint in AerodynamicMapPoints)
            {
                // Current row parameters
                double aerodynamicMapRowWindSpeed = aerodynamicMapPoint.WindRelativeSpeed;
                double aerodynamicMapRowCarHeight = aerodynamicMapPoint.CarHeight;
                // New parameter indicators initialization
                bool isNewWindSpeed = true;
                bool isNewCarHeight = true;
                // New parameter indicators value determination
                foreach (double windSpeed in AerodynamicMapWindSpeeds)
                {
                    if (aerodynamicMapRowWindSpeed == windSpeed) isNewWindSpeed = false;
                }
                foreach (double carHeight in AerodynamicMapCarHeights)
                {
                    if (aerodynamicMapRowCarHeight == carHeight) isNewCarHeight = false;
                }
                // New parameter values registration
                if (isNewWindSpeed) AerodynamicMapWindSpeeds.Add(aerodynamicMapRowWindSpeed);
                if (isNewCarHeight) AerodynamicMapCarHeights.Add(aerodynamicMapRowCarHeight);
            }
            // Sort to ascending order
            AerodynamicMapWindSpeeds = AerodynamicMapWindSpeeds.OrderBy(d => d).ToList();
            AerodynamicMapCarHeights = AerodynamicMapCarHeights.OrderBy(d => d).ToList();
        }

        public AerodynamicMapPoint InterpolateAerodynamicMap(double windSpeed, double carHeight)
        {
            // Map interpolation based on the wind speed
            // Interpolated map list initialization
            List<AerodynamicMapPoint> aerodynamicMapInterpolatedByWindSpeed = new List<AerodynamicMapPoint>();
            // Checks if the wind speed is lower, higher or in the range of the aerodynamic map
            if (windSpeed <= AerodynamicMapWindSpeeds.Min()) // Case speed is lower
            {
                // Aerodynamic map rows sweep "for" loop
                for (int row = 0; row < AerodynamicMapPoints.Count; row++)
                {
                    // Checks if the speed in this row is the lowest speed of the map and gets the object if it is
                    if (AerodynamicMapPoints[row].WindRelativeSpeed == AerodynamicMapWindSpeeds.Min())
                        aerodynamicMapInterpolatedByWindSpeed.Add(AerodynamicMapPoints[row]);
                }
            }
            else if (windSpeed >= AerodynamicMapWindSpeeds.Max()) // Case speed is higher
            {
                // Aerodynamic map rows sweep "for" loop
                for (int row = 0; row < AerodynamicMapPoints.Count; row++)
                {
                    // Checks if the speed in this row is the highest speed of the map and gets the object if it is
                    if (AerodynamicMapPoints[row].WindRelativeSpeed == AerodynamicMapWindSpeeds.Max())
                        aerodynamicMapInterpolatedByWindSpeed.Add(AerodynamicMapPoints[row]);
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
                for (int row = 0; row < AerodynamicMapPoints.Count; row++)
                {
                    if (AerodynamicMapPoints[row].WindRelativeSpeed == lowerWindSpeed)
                        lowerAerodynamicMapPoints.Add(AerodynamicMapPoints[row]);
                    else if (AerodynamicMapPoints[row].WindRelativeSpeed == higherWindSpeed)
                        higherAerodynamicMapPoints.Add(AerodynamicMapPoints[row]);
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
                        if (lowerAerodynamicMapPoint.CarHeight == higherAerodynamicMapPoint.CarHeight)
                        {
                            // Interpolated aerodynamic map point
                            AerodynamicMapPoint aerodynamicMapPoint = new AerodynamicMapPoint(windSpeed,
                                lowerAerodynamicMapPoint.CarHeight + (higherAerodynamicMapPoint.CarHeight - lowerAerodynamicMapPoint.CarHeight) * windSpeedInterpolationRatio,
                                lowerAerodynamicMapPoint.DragCoefficient + (higherAerodynamicMapPoint.DragCoefficient - lowerAerodynamicMapPoint.DragCoefficient) * windSpeedInterpolationRatio,
                                lowerAerodynamicMapPoint.LiftCoefficient + (higherAerodynamicMapPoint.LiftCoefficient - lowerAerodynamicMapPoint.LiftCoefficient) * windSpeedInterpolationRatio);
                            // Registering of the point
                            aerodynamicMapInterpolatedByWindSpeed.Add(aerodynamicMapPoint);
                        }
                    }
                }
            }

            // Map interpolation based on the car height
            // Checks if the car height is lower, higher or in the range of the aerodynamic map
            if (carHeight <= AerodynamicMapCarHeights.Min()) // Case height is lower
            {
                // Aerodynamic map rows sweep "for" loop
                for (int row = 0; row < aerodynamicMapInterpolatedByWindSpeed.Count; row++)
                {
                    // Checks if the height in this row is the lowest height of the map and gets the object if it is
                    if (aerodynamicMapInterpolatedByWindSpeed[row].CarHeight == AerodynamicMapCarHeights.Min())
                        return aerodynamicMapInterpolatedByWindSpeed[row];
                }
            }
            else if (carHeight >= AerodynamicMapCarHeights.Max()) // Case height is higher
            {
                // Aerodynamic map rows sweep "for" loop
                for (int row = 0; row < aerodynamicMapInterpolatedByWindSpeed.Count; row++)
                {
                    // Checks if the height in this row is the highest height of the map and gets the object if it is
                    if (aerodynamicMapInterpolatedByWindSpeed[row].CarHeight == AerodynamicMapCarHeights.Max())
                        return aerodynamicMapInterpolatedByWindSpeed[row];
                }
            }
            else
            {
                // Finds the map's next lower height compared to the car height
                int iCarHeight = 0;
                while (carHeight <= AerodynamicMapCarHeights[iCarHeight]) iCarHeight++;
                // Lower and higher wind speeds
                double lowerCarHeight = AerodynamicMapWindSpeeds[iCarHeight];
                double higherCarHeight = AerodynamicMapWindSpeeds[iCarHeight + 1];
                // Lower and upper aerodynamic map points initialization
                AerodynamicMapPoint lowerAerodynamicMapPoint = new AerodynamicMapPoint();
                AerodynamicMapPoint higherAerodynamicMapPoint = new AerodynamicMapPoint();
                // Gets the objects of the lower and upper aerodynamic map points
                for (int row = 0; row < aerodynamicMapInterpolatedByWindSpeed.Count; row++)
                {
                    if (aerodynamicMapInterpolatedByWindSpeed[row].CarHeight == lowerCarHeight)
                        lowerAerodynamicMapPoint = aerodynamicMapInterpolatedByWindSpeed[row];
                    else if (aerodynamicMapInterpolatedByWindSpeed[row].CarHeight == higherCarHeight)
                        higherAerodynamicMapPoint = aerodynamicMapInterpolatedByWindSpeed[row];
                }
                // Linear interpolation ratio
                double carHeightInterpolationRatio = (carHeight - lowerCarHeight) / (higherCarHeight - lowerCarHeight);
                // Interpolated aerodynamic map point
                AerodynamicMapPoint aerodynamicMapPoint = new AerodynamicMapPoint(windSpeed, carHeight,
                    lowerAerodynamicMapPoint.DragCoefficient + (higherAerodynamicMapPoint.DragCoefficient - lowerAerodynamicMapPoint.DragCoefficient) * carHeightInterpolationRatio,
                    lowerAerodynamicMapPoint.LiftCoefficient + (higherAerodynamicMapPoint.LiftCoefficient - lowerAerodynamicMapPoint.LiftCoefficient) * carHeightInterpolationRatio);
                // Method return value
                return aerodynamicMapPoint;
            }
            return new AerodynamicMapPoint();
        }
    }
}
