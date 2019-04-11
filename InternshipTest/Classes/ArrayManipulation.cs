using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternshipTest
{
    public class ArrayManipulation
    {
        /// <summary>
        /// Returns an array which is equal to the specified line of the specified two dimensional double array.
        /// </summary>
        /// <param name="twoDimArray"> Two dimensional array to take the line from. </param>
        /// <param name="iLine"> Zero-based index of the wished array line (1st dimension). </param>
        /// <returns> An array which contains the inputed array line data.  </returns>
        public static double[] GetLineFromTwoDimensionalDoubleArray(double[,] twoDimArray, int iLine)
        {
            int arrayLength = twoDimArray.GetLength(1);
            double[] lineArray = new double[arrayLength];
            for (int i = 0; i < arrayLength; i++)
            {
                lineArray[i] = twoDimArray[iLine, i];
            }
            return lineArray;
        }
        /// <summary>
        /// Returns an array which is equal to the specified line of the specified three dimensional double array.
        /// </summary>
        /// <param name="threeDimArray">Three dimensional array to take the line from. </param>
        /// <param name="iPage"> Zero-based index of the wished array page (3rd dimension). </param>
        /// <param name="iLine"> Zero-based index of the wished array line (1st dimension). </param>
        /// <returns> An array which contains the inputed array line data. </returns>
        public static double[] GetLineFromThreeDimensionalDoubleArray(double[,,] threeDimArray, int iLine, int iPage)
        {
            int arrayLength = threeDimArray.GetLength(1);
            double[] lineArray = new double[arrayLength];
            for (int i = 0; i < threeDimArray.GetLength(1); i++)
            {
                lineArray[i] = threeDimArray[iLine, i, iPage];
            }
            return lineArray;
        }

        public static int[] GetCurvesMinimumDistancePointsIndexes(double[,] curve1, double[,] curve2)
        {
            int iCurve1, iCurve2;
            int[] pointsIndexes = new int[2];
            double curvesMinimumDistance = Math.Pow(10, 10);
            // Curve 1 points sweep
            for (iCurve1 = 0; iCurve1 < curve1.GetLength(1); iCurve1++)
            {
                int iMinimumDistanceCurve2 = 0;
                double currentMinimumDistance = Math.Pow(10, 10);
                // Curve 2 points sweep
                for (iCurve2 = 0; iCurve2 < curve2.GetLength(1); iCurve2++)
                {
                    // Current distance calculation
                    double currentDistanceInnerLoop = Math.Sqrt(
                        Math.Pow(curve1[0, iCurve1] - curve2[0, iCurve2], 2) +
                        Math.Pow(curve1[1, iCurve1] - curve2[1, iCurve2], 2));
                    // Checks if the current distance is smaller than the current minimum distance
                    if (currentDistanceInnerLoop < currentMinimumDistance)
                    {
                        currentMinimumDistance = currentDistanceInnerLoop;
                        iMinimumDistanceCurve2 = iCurve2;
                    }
                }
                // Checks if the current distance is smaller than the minimum distance
                if (currentMinimumDistance<curvesMinimumDistance)
                {
                    curvesMinimumDistance = currentMinimumDistance;
                    pointsIndexes[0] = iCurve1;
                    pointsIndexes[1] = iMinimumDistanceCurve2;
                }
            }

            return pointsIndexes;
        }

        public static double[,] JoinArraysIn2DArray(double[] array1, double[] array2)
        {
            double[,] finalArray = new double[2, array1.Length];
            for (int i = 0; i < array1.Length; i++)
            {
                finalArray[0, i] = array1[i];
                finalArray[1, i] = array2[i];
            }
            return finalArray;
        }
        
    }
}
