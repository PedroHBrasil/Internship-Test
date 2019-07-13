using MathNet.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternshipTest.Simulation
{
    public class GGDiagram
    {
        #region Properties
        /// <summary>
        /// Speed at which the accelerations are calculated [m/s].
        /// </summary>
        public double Speed { get; set; }
        /// <summary>
        /// Amount of points of the GG diagram.
        /// </summary>
        public int AmountOfPoints { get; set; }
        /// <summary>
        /// Amount of directions of the GG diagram accelerations.
        /// </summary>
        public int AmountOfDirections { get; set; }
        /// <summary>
        /// Longitudinal accelerations [m/s²].
        /// </summary>
        public List<double> LongitudinalAccelerations { get; set; }
        /// <summary>
        /// Lateral accelerations [m/s²].
        /// </summary>
        public List<double> LateralAccelerations { get; set; }
        /// <summary>
        /// Curvatures [1/m].
        /// </summary>
        public List<double> Curvatures { get; set; }
        #endregion
        #region Constructors
        public GGDiagram()
        {
            LongitudinalAccelerations = new List<double>();
            LateralAccelerations = new List<double>();
        }
        public GGDiagram(OneWheelGGDiagram twoWheelGGDiagram)
        {
            Speed = twoWheelGGDiagram.Speed;
            AmountOfPoints = twoWheelGGDiagram.AmountOfPoints;
            AmountOfDirections = twoWheelGGDiagram.AmountOfDirections;
            LongitudinalAccelerations = twoWheelGGDiagram.LongitudinalAccelerations;
            LateralAccelerations = twoWheelGGDiagram.LateralAccelerations;
        }
        public GGDiagram(TwoWheelGGDiagram twoWheelGGDiagram)
        {
            Speed = twoWheelGGDiagram.Speed;
            AmountOfPoints = twoWheelGGDiagram.AmountOfPoints;
            AmountOfDirections = twoWheelGGDiagram.AmountOfDirections;
            LongitudinalAccelerations = twoWheelGGDiagram.LongitudinalAccelerations;
            LateralAccelerations = twoWheelGGDiagram.LateralAccelerations;
        }
        #endregion
        #region Methods
        /// <summary>
        /// Gets the curvatures associated with each point of the GG diagram.
        /// </summary>
        public void GetAssociatedCurvatures()
        {
            Curvatures = new List<double>();
            // Gets and registers the curvatures associated with each point of the GG diagram
            for (int iLateralAcceleration = 0; iLateralAcceleration < LateralAccelerations.Count; iLateralAcceleration++)
            {
                // Corrects the speed if it is equal to zero
                double currentSpeed = Speed;
                if (currentSpeed == 0) currentSpeed = 0.1;
                Curvatures.Add(LateralAccelerations[iLateralAcceleration] / Math.Pow(currentSpeed, 2));
            }
        }
        /// <summary>
        /// Filters the GG diagram so that there is one acceleration per direction.
        /// </summary>
        public void _FilterGGDiagramByDirections()
        {
            // Current accelerations arrays
            double[] currentLongitudinalAccelerations = LongitudinalAccelerations.ToArray();
            double[] currentLateralAccelerations = LateralAccelerations.ToArray();
            // Current GG Diagram's mean accelerations
            double meanLongitudinalAcceleration = currentLongitudinalAccelerations.Average();
            double meanLateralAcceleration = currentLateralAccelerations.Average();
            // Current accelerations directions array
            double[] currentAccelerationsDirections = new double[LongitudinalAccelerations.Count];
            for (int i = 0; i < LongitudinalAccelerations.Count; i++)
            {
                currentAccelerationsDirections[i] = Math.Atan2(LongitudinalAccelerations[i] - meanLongitudinalAcceleration, LateralAccelerations[i] - meanLateralAcceleration);
            }
            // Target directions array
            double[] targetDirections = Generate.LinearSpaced(AmountOfDirections + 1, -Math.PI, Math.PI);
            // New accelerations arrays
            double[] newLongitudinalAccelerations = new double[targetDirections.Length - 1];
            double[] newLateralAccelerations = new double[targetDirections.Length - 1];
            // New accelerations arrays determination
            for (int iTargetDirection = 0; iTargetDirection < targetDirections.Length - 1; iTargetDirection++)
            {
                // Indexes of the current accelerations in range
                List<int> indexesOfTheCurrentAccelerationsInRange = new List<int>();
                List<double> magnitudesOfTheCurrentAccelerationsInRange = new List<double>();
                for (int iCurrentAcceleration = 0; iCurrentAcceleration < currentAccelerationsDirections.Length; iCurrentAcceleration++)
                {
                    // Checks if the current acceleration's direction is in the current target direction interval
                    if (currentAccelerationsDirections[iCurrentAcceleration] >= targetDirections[iTargetDirection] && currentAccelerationsDirections[iCurrentAcceleration] < targetDirections[iTargetDirection + 1])
                    {
                        indexesOfTheCurrentAccelerationsInRange.Add(iCurrentAcceleration);
                        magnitudesOfTheCurrentAccelerationsInRange.Add(Math.Sqrt(Math.Pow(LongitudinalAccelerations[iCurrentAcceleration] - meanLongitudinalAcceleration, 2) + Math.Pow(LateralAccelerations[iCurrentAcceleration] - meanLateralAcceleration, 2)));
                    }
                }
                // Determination of the new acceleration
                if (indexesOfTheCurrentAccelerationsInRange.Count > 0)
                {
                    int newAccelerationIndexInIndexesArray = magnitudesOfTheCurrentAccelerationsInRange.IndexOf(magnitudesOfTheCurrentAccelerationsInRange.Max());
                    int newAccelerationIndex = indexesOfTheCurrentAccelerationsInRange[newAccelerationIndexInIndexesArray];
                    newLongitudinalAccelerations[iTargetDirection] = LongitudinalAccelerations[newAccelerationIndex];
                    newLateralAccelerations[iTargetDirection] = LateralAccelerations[newAccelerationIndex];
                }
            }
            // Gets the indexes of the directions which have accelerations
            List<int> nonZeroAccelerationsIndexes = new List<int>();
            for (int iDirection = 0; iDirection < newLongitudinalAccelerations.Length; iDirection++)
            {
                if (newLongitudinalAccelerations[iDirection] != 0 || newLateralAccelerations[iDirection] != 0)
                {
                    nonZeroAccelerationsIndexes.Add(iDirection);
                }
            }
            // Gets the remaining accelerations by interpolation
            for (int iNonZeroAcceleration = 0; iNonZeroAcceleration < nonZeroAccelerationsIndexes.Count; iNonZeroAcceleration++)
            {
                // Gets the current and next non-zero accelerations indexes
                int currentAccelerationIndex = nonZeroAccelerationsIndexes[iNonZeroAcceleration];
                int nextAccelerationIndex;
                if (iNonZeroAcceleration == nonZeroAccelerationsIndexes.Count - 1) nextAccelerationIndex = nonZeroAccelerationsIndexes[0];
                else nextAccelerationIndex = nonZeroAccelerationsIndexes[iNonZeroAcceleration + 1];
                // Checks if there are zero accelerations between these points. 
                if (nextAccelerationIndex - currentAccelerationIndex > 1)
                {
                    // Gets the accelerations in the points by interpolation
                    for (int iDirection = currentAccelerationIndex + 1; iDirection < nextAccelerationIndex; iDirection++)
                    {
                        double interpolationRatio = (iDirection - currentAccelerationIndex) / (nextAccelerationIndex - currentAccelerationIndex);
                        newLongitudinalAccelerations[iDirection] = newLongitudinalAccelerations[currentAccelerationIndex] + interpolationRatio * (newLongitudinalAccelerations[nextAccelerationIndex] - newLongitudinalAccelerations[currentAccelerationIndex]);
                        newLateralAccelerations[iDirection] = newLateralAccelerations[currentAccelerationIndex] + interpolationRatio * (newLateralAccelerations[nextAccelerationIndex] - newLateralAccelerations[currentAccelerationIndex]);
                    }
                }
                else if (iNonZeroAcceleration == nonZeroAccelerationsIndexes.Count() - 1 && (currentAccelerationIndex != newLongitudinalAccelerations.Length - 1 || nextAccelerationIndex != 0))
                {
                    // Gets the accelerations in the points by interpolation
                    for (int iDirection = currentAccelerationIndex + 1; iDirection < nextAccelerationIndex + newLongitudinalAccelerations.Length; iDirection++)
                    {
                        double interpolationRatio = (iDirection - currentAccelerationIndex) / (nextAccelerationIndex + (newLongitudinalAccelerations.Length - 1) - currentAccelerationIndex);
                        if (iDirection < newLongitudinalAccelerations.Length)
                        {
                            newLongitudinalAccelerations[iDirection] = newLongitudinalAccelerations[currentAccelerationIndex] + interpolationRatio * (newLongitudinalAccelerations[nextAccelerationIndex] - newLongitudinalAccelerations[currentAccelerationIndex]);
                            newLateralAccelerations[iDirection] = newLateralAccelerations[currentAccelerationIndex] + interpolationRatio * (newLateralAccelerations[nextAccelerationIndex] - newLateralAccelerations[currentAccelerationIndex]);
                        }
                        else
                        {
                            newLongitudinalAccelerations[iDirection - newLongitudinalAccelerations.Length] = newLongitudinalAccelerations[currentAccelerationIndex] + interpolationRatio * (newLongitudinalAccelerations[nextAccelerationIndex] - newLongitudinalAccelerations[currentAccelerationIndex]);
                            newLateralAccelerations[iDirection - newLongitudinalAccelerations.Length] = newLateralAccelerations[currentAccelerationIndex] + interpolationRatio * (newLateralAccelerations[nextAccelerationIndex] - newLateralAccelerations[currentAccelerationIndex]);
                        }
                    }
                }
            }
            // Writes the interpolated values to the lists
            LongitudinalAccelerations = newLongitudinalAccelerations.ToList();
            LateralAccelerations = newLateralAccelerations.ToList();
        }
        /// <summary>
        /// Gets the longitudinal acceleration based on the interpolation of the GG diagram by a lateral acceleration.
        /// </summary>
        /// <param name="lateralAcceleration"> Lateral acceleration [m/s²] </param>
        /// <param name="longitudinalAccelerationMode"> "Accelerating" or "Braking" </param>
        /// <returns></returns>
        public double GetLongitudinalAccelerationViaInterpolationBasedOnLateralAcceleration(double lateralAcceleration, string longitudinalAccelerationMode)
        {
            // Gets the lateral acceleration interval index
            int iAcceleration = _GetLateralAccelerationIndex(lateralAcceleration, longitudinalAccelerationMode);
            // Gets the index of the next lateral acceleration
            int iNextAcceleration;
            if (iAcceleration == LateralAccelerations.Count - 1) iNextAcceleration = 0;
            else iNextAcceleration = iAcceleration + 1;
            // Calculates the interpolation ratio
            double interpolationRatio = (lateralAcceleration - LateralAccelerations[iAcceleration]) /
                (LateralAccelerations[iNextAcceleration] - LateralAccelerations[iAcceleration]);
            // Adjusts the interpolation ratio if necessary
            if (interpolationRatio < 0) interpolationRatio = 0;
            else if (interpolationRatio > 1) interpolationRatio = 1;
            // Gets the longitudinal acceleration
            double longitudinalAcceleration = LongitudinalAccelerations[iAcceleration] +
                interpolationRatio * (LongitudinalAccelerations[iNextAcceleration] - LongitudinalAccelerations[iAcceleration]);
            // Returns the longitudinal acceleration
            return longitudinalAcceleration;
        }
        /// <summary>
        /// Gets the index of the laterala acceleration to be used in the interpolation.
        /// </summary>
        /// <param name="lateralAcceleration"> Lateral acceleration [m/s²] </param>
        /// <param name="longitudinalAccelerationMode"> "Accelerating" or "Braking" </param>
        /// <returns></returns>
        private int _GetLateralAccelerationIndex(double lateralAcceleration, string longitudinalAccelerationMode)
        {
            // Initializes the index and range indicator variables
            int iAcceleration;
            bool isInRange = false;
            // Checks if the lateral acceleration is in range
            if (lateralAcceleration >= LateralAccelerations.Min() &&
                lateralAcceleration <= LateralAccelerations.Max())
                isInRange = true;
            // If in range, finds the interval. Else, uses the maximum or minimum lateral acceleration index.
            if (isInRange)
            {
                for (iAcceleration = 0; iAcceleration < LateralAccelerations.Count; iAcceleration++)
                {
                    // Next acceleration index
                    int iNextAcceleration;
                    if (iAcceleration == LateralAccelerations.Count - 1) iNextAcceleration = 0;
                    else iNextAcceleration = iAcceleration + 1;
                    // Checks if the current interval contains the current lateral acceleration ad if it corresponds to the current acceleration mode
                    if (longitudinalAccelerationMode == "Braking" &&
                        LateralAccelerations[iAcceleration] <= lateralAcceleration &&
                        LateralAccelerations[iNextAcceleration] >= lateralAcceleration)
                        break;
                    else if (longitudinalAccelerationMode == "Accelerating" &&
                        LateralAccelerations[iAcceleration] >= lateralAcceleration &&
                        LateralAccelerations[iNextAcceleration] <= lateralAcceleration)
                        break;
                }
            }
            else
            {
                // Gets the index of the maximum or minimum value, depending on the acceleration sign
                if (lateralAcceleration > 0) iAcceleration = LateralAccelerations.IndexOf(LateralAccelerations.Max());
                else iAcceleration = LateralAccelerations.IndexOf(LateralAccelerations.Min());
            }
            // Returns the index of the lateral acceleration interval
            return iAcceleration;
        }
        #endregion
    }
}
