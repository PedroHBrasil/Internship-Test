using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternshipTest
{
    /// <summary>
    /// Contains the information of a path.
    /// </summary>
    public class Path : GenericInfo
    {
        #region Properties
        // Direct inputs
        /// <summary>
        /// Resolution to generate the points [m].
        /// </summary>
        public double Resolution { get; set; }
        /// <summary>
        /// Set of sectors to divide the path.
        /// </summary>
        public PathSectorsSet SectorsSet { get; set; }
        /// <summary>
        /// Set of tabular sections which defin the path points.
        /// </summary>
        public TabularPathSectionsSet TabularSectionsSet { get; set; }
        // Direct points properties
        /// <summary>
        /// X axis coordinates of the path points [m].
        /// </summary>
        public List<double> CoordinatesX { get; set; }
        /// <summary>
        /// Y axis coordinates of the path points [m].
        /// </summary>
        public List<double> CoordinatesY { get; set; }
        /// <summary>
        /// Local elapsed distances per point [m].
        /// </summary>
        public List<double> ElapsedDistance { get; set; }
        /// <summary>
        /// Local radius at each point [m].
        /// </summary>
        private List<double> LocalRadius { get; set; }
        /// <summary>
        /// Local curvature at each point [m].
        /// </summary>
        public List<double> LocalCurvatures { get; set; }
        /// <summary>
        /// Local path's tangent direction at each point [rad].
        /// </summary>
        private List<double> LocalTangentDirection { get; set; }
        // Key values
        /// <summary>
        /// Total length of the path [m].
        /// </summary>
        private double PathLength { get; set; }
        /// <summary>
        /// Amount of points that the path contains.
        /// </summary>
        public int AmountOfPointsInPath { get; set; }
        // Points geographical atributes
        /// <summary>
        /// Elapsed distances at which the sections switch [m].
        /// </summary>
        private List<double> SectionsSwitchDistances { get; set; }
        /// <summary>
        /// Index of the section at each point.
        /// </summary>
        private List<int> LocalSectionIndex { get; set; }
        /// <summary>
        /// Index of the sector at each point.
        /// </summary>
        public List<int> LocalSectorIndex { get; set; }
        /// <summary>
        /// Index of the point in the local section.
        /// </summary>
        private List<int> IndexInLocalSection { get; set; }
        /// <summary>
        /// Amount of points at each section.
        /// </summary>
        private List<int> AmountOfPointsInSections { get; set; }
        #endregion
        #region Constructors
        public Path() { }

        public Path(string pathID, string description, PathSectorsSet sectors, TabularPathSectionsSet sections, double resolution)
        {
            ID = pathID;
            Description = description;
            SectorsSet = sectors;
            TabularSectionsSet = sections;
            Resolution = Math.Abs(resolution);
        }
        #endregion
        #region Methods
        public override string ToString()
        {
            return ID;
        }
        #region TabularSectionsBasedPathGeneration
        /// <summary>
        /// Generates the path parameters from the tabular data inputs.
        /// </summary>
        public void GeneratePathPointsParametersFromTabular()
        {
            // Lists initialization
            CoordinatesX = new List<double>();
            CoordinatesY = new List<double>();
            ElapsedDistance = new List<double>();
            LocalRadius = new List<double>();
            LocalCurvatures = new List<double>();
            LocalTangentDirection = new List<double>();
            SectionsSwitchDistances = new List<double>();
            LocalSectionIndex = new List<int>();
            LocalSectorIndex = new List<int>();
            IndexInLocalSection = new List<int>();
            AmountOfPointsInSections = new List<int>();

            _GetInitialPoint();
            _GetPathLength();
            _AssociatePointsToSectionsAndSectors();
            _GetPointsParameters();
        }
        /// <summary>
        /// Gets the first point of the path and its associated parameters.
        /// </summary>
        private void _GetInitialPoint()
        {
            // Starting points parameters
            CoordinatesX.Add(0); CoordinatesY.Add(0);
            ElapsedDistance.Add(0); LocalRadius.Add(TabularSectionsSet.Sections[0].InitialRadius);
            LocalTangentDirection.Add(0);
            if (LocalRadius[0] == 0) LocalCurvatures.Add(0);
            else LocalCurvatures.Add(1 / LocalRadius[0]);
        }
        /// <summary>
        /// Gets the length of the path.
        /// </summary>
        private void _GetPathLength()
        {
            PathLength = 0;
            for (int iSection = 0; iSection < TabularSectionsSet.Sections.Count; iSection++)
            {
                // Path length update
                PathLength += TabularSectionsSet.Sections[iSection].Length;
                // Section switch distances
                SectionsSwitchDistances.Add(PathLength);
            }
        }
        /// <summary>
        /// Associates each path point to their respective sections and sectors.
        /// </summary>
        private void _AssociatePointsToSectionsAndSectors()
        {
            // Amount of points in path
            AmountOfPointsInPath = (int)(PathLength / Resolution);
            // Initialization of section index and point index in section variables
            int currentSectionIndex = 0;
            int currentSectorIndex = 1;
            int currentIndexInLocalSection = 0;
            LocalSectionIndex.Add(currentSectionIndex);
            LocalSectorIndex.Add(currentSectorIndex);
            IndexInLocalSection.Add(currentIndexInLocalSection);
            // Association of points to sections and elapsed distance "for" loop
            for (int iPoint = 1; iPoint < AmountOfPointsInPath; iPoint++)
            {
                // Index in local section update
                currentIndexInLocalSection++;
                // Updates the elapsed distance
                ElapsedDistance.Add(ElapsedDistance[iPoint - 1] + Resolution);
                // Checks if the section has changed
                if (ElapsedDistance[iPoint] > SectionsSwitchDistances[currentSectionIndex])
                {
                    // Updates the section
                    if (currentSectionIndex < TabularSectionsSet.Sections.Count - 1) currentSectionIndex++;
                    // Amount of points in sections update
                    AmountOfPointsInSections.Add(currentIndexInLocalSection);
                    // Resets the point index in section counter
                    currentIndexInLocalSection = 0;
                }
                // Checks if there is more than one sector
                if (SectorsSet.Sectors.Count > currentSectorIndex)
                {
                    // Checks if the sector has changed
                    if (ElapsedDistance[iPoint] > SectorsSet.Sectors[currentSectorIndex].StartDistance)
                    {
                        currentSectorIndex++;
                    }
                }
                // List updates
                LocalSectionIndex.Add(currentSectionIndex);
                IndexInLocalSection.Add(currentIndexInLocalSection);
                LocalSectorIndex.Add(currentSectorIndex);
            }
            AmountOfPointsInSections.Add(currentIndexInLocalSection);
        }
        /// <summary>
        /// Gets the associated parameters of the points (coordinates, local radius and path's tangent direction).
        /// </summary>
        private void _GetPointsParameters()
        {
            // Points coordinates, local radius and tangent direction determination loop
            for (int iPoint = 1; iPoint < AmountOfPointsInPath; iPoint++)
            {
                // Current section index
                int currentSectionIndex = LocalSectionIndex[iPoint];
                // Is the section type a straight?
                if (TabularSectionsSet.Sections[currentSectionIndex].Type == TabularPathSection.SectionType.Straight)
                {
                    // Local radius, curvature and local tangent direction update
                    LocalRadius.Add(0);
                    LocalCurvatures.Add(0);
                    LocalTangentDirection.Add(LocalTangentDirection[iPoint - 1]);
                }
                else
                {
                    // Current radius and section's angular length
                    double initialRadius = TabularSectionsSet.Sections[currentSectionIndex].InitialRadius;
                    double finalRadius = TabularSectionsSet.Sections[currentSectionIndex].FinalRadius;
                    double currentRadius = initialRadius + (finalRadius - initialRadius) *
                        IndexInLocalSection[iPoint] / AmountOfPointsInSections[currentSectionIndex];
                    double currentAngularLength = TabularSectionsSet.Sections[currentSectionIndex].Length /
                        AmountOfPointsInSections[currentSectionIndex] / currentRadius;
                    // Decides the sign o the angular displacement based on the section type (direction)
                    if (TabularSectionsSet.Sections[currentSectionIndex].Type == TabularPathSection.SectionType.Left)
                    {
                        currentRadius = -currentRadius;
                        currentAngularLength = -currentAngularLength;
                    }
                    // Add local radius and curvature to lists
                    LocalRadius.Add(currentRadius);
                    LocalCurvatures.Add(1 / currentRadius);
                    // Local tangent direction
                    LocalTangentDirection.Add(LocalTangentDirection[iPoint - 1] +
                        currentAngularLength);
                }
                // Coordinates increments
                double deltaX = Resolution * Math.Cos(LocalTangentDirection[iPoint]);
                double deltaY = Resolution * Math.Sin(LocalTangentDirection[iPoint]);
                // Coordinates update
                CoordinatesX.Add(CoordinatesX[iPoint - 1] + deltaX);
                CoordinatesY.Add(CoordinatesY[iPoint - 1] + deltaY);
            }
        }
        #endregion
        #endregion
    }
}
