using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternshipTest
{
    /*
     * Contains a path's parameters:
     *  - PathID: string which identifies the object;
     *  - Resolution: integer which represents the path's resolution in mm;
     *  - sections: list of objects which represents the path's sections (components);
     *  - CoordinatesX: list of doubles which represents the path's points X coordinates in m;
     *  - CoordinatesY: list of doubles which represents the path's points Y coordinates in m;
     *  - ElapsedDistance: list of doubles which represents the path's elapsed distance in m;
     *  - LocalRadius: list of doubles which represents the path's points local radius in m; and
     *  - LocalTangentDirection: list of doubles which represents the path's tangents directions as deg,
     *  where 0 means right and 180 means left.
     */
    class Path
    {
        // Properties ------------------------------------------------------------------
        // Direct inputs
        public string PathID { get; set; }
        public int Resolution { get; set; }
        public List<PathSector> Sectors { get; set; }
        public List<TabularPathSection> TabularSections { get; set; }
        // Direct points properties
        public List<double> CoordinatesX { get; set; }
        public List<double> CoordinatesY { get; set; }
        public List<double> ElapsedDistance { get; set; }
        public List<double> LocalRadius { get; set; }
        public List<double> LocalCurvatures { get; set; }
        public List<double> LocalTangentDirection { get; set; }
        // Key values
        public double PathLength { get; set; }
        public int AmountOfPointsInPath { get; set; }
        // Points geographical atributes
        public List<double> SectionsSwitchDistances { get; set; }
        public List<int> LocalSectionIndex { get; set; }
        public List<int> LocalSectorIndex { get; set; }
        public List<int> IndexInLocalSection { get; set; }
        public List<int> AmountOfPointsInSections { get; set; }
        // Constructors ----------------------------------------------------------------
        public Path()
        {
            PathID = "Default";
            Resolution = 100;
            Sectors = new List<PathSector>();
        }

        public Path(string pathID, int resolution, List<PathSector> sectors, List<TabularPathSection> sections)
        {
            PathID = pathID;
            Resolution = resolution;
            Sectors = sectors;
            TabularSections = sections;

            // Lists initializtion
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

            GeneratePathPointsParametersFromTabular();
        }
        // Methods
        public override string ToString()
        {
            return PathID;
        }
        #region TabularSectionsBasedPathGeneration
        private void GeneratePathPointsParametersFromTabular()
        {
            GetInitialPoint();
            GetPathLength();
            AssociatePointsToSectionsAndSectors();
            GetPointsParameters();
        }
        private void GetInitialPoint()
        {
            // Starting points parameters
            CoordinatesX.Add(0); CoordinatesY.Add(0);
            ElapsedDistance.Add(0); LocalRadius.Add(TabularSections[0].InitialRadius);
            LocalTangentDirection.Add(0);
            if (LocalRadius[0] == 0) LocalCurvatures.Add(0);
            else LocalCurvatures.Add(1 / LocalRadius[0]);
        }
        private void GetPathLength()
        {
            PathLength = 0;
            for (int iSection = 0; iSection < TabularSections.Count; iSection++)
            {
                // Path length update
                PathLength += TabularSections[iSection].Length;
                // Section switch distances
                SectionsSwitchDistances.Add(PathLength);
            }
        }
        private void AssociatePointsToSectionsAndSectors()
        {
            // Amount of points in path
            AmountOfPointsInPath = (int)PathLength * 1000 / Resolution;
            // Initialization of section index and point index in section variables
            int currentSectionIndex = 0;
            int currentSectorIndex = 0;
            int currentIndexInLocalSection = 0;
            LocalSectionIndex.Add(0);
            LocalSectorIndex.Add(0);
            IndexInLocalSection.Add(0);
            // Association of points to sections and elapsed distance "for" loop
            for (int iPoint = 1; iPoint < AmountOfPointsInPath; iPoint++)
            {
                // Index in local section update
                currentIndexInLocalSection++;
                // Updates the elapsed distance
                ElapsedDistance.Add(ElapsedDistance[iPoint - 1] + (double)Resolution / 1000);
                // Checks if the section has changed
                if (ElapsedDistance[iPoint] > SectionsSwitchDistances[currentSectionIndex])
                {
                    // Updates the section
                    if (currentSectionIndex < TabularSections.Count - 1) currentSectionIndex++;
                    // Amount of points in sections update
                    AmountOfPointsInSections.Add(currentIndexInLocalSection);
                    // Resets the point index in section counter
                    currentIndexInLocalSection = 0;
                }
                // Checks if the sector has changed
                if (ElapsedDistance[iPoint] > Sectors[currentSectorIndex].SectorStartDistance)
                {
                    // Updates the sector
                    if (currentSectorIndex < Sectors.Count - 1) currentSectorIndex++;
                }
                // List updates
                LocalSectionIndex.Add(currentSectionIndex);
                IndexInLocalSection.Add(currentIndexInLocalSection);
                LocalSectorIndex.Add(currentSectorIndex);
            }
            AmountOfPointsInSections.Add(currentIndexInLocalSection);
        }
        private void GetPointsParameters()
        {
            // Points coordinates, local radius and tangent direction determination loop
            for (int iPoint = 1; iPoint < AmountOfPointsInPath; iPoint++)
            {
                // Current section index
                int currentSectionIndex = LocalSectionIndex[iPoint];
                // Is the section type a straight?
                if (TabularSections[currentSectionIndex].Type == TabularPathSection.SectionType.Straight)
                {
                    // Local radius, curvature and local tangent direction update
                    LocalRadius.Add(0);
                    LocalCurvatures.Add(0);
                    LocalTangentDirection.Add(LocalTangentDirection[iPoint - 1]);
                }
                else
                {
                    // Current radius and section's angular length
                    double initialRadius = TabularSections[currentSectionIndex].InitialRadius;
                    double finalRadius = TabularSections[currentSectionIndex].FinalRadius;
                    double currentRadius = initialRadius + (finalRadius - initialRadius) *
                        IndexInLocalSection[iPoint] / AmountOfPointsInSections[currentSectionIndex];
                    double currentAngularLength = TabularSections[currentSectionIndex].Length /
                        AmountOfPointsInSections[currentSectionIndex] / currentRadius;
                    // Decides the sign o the angular displacement based on the section type (direction)
                    if (TabularSections[currentSectionIndex].Type == TabularPathSection.SectionType.Left)
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
                double deltaX = (double)Resolution / 1000 * Math.Cos(LocalTangentDirection[iPoint]);
                double deltaY = (double)Resolution / 1000 * Math.Sin(LocalTangentDirection[iPoint]);
                // Coordinates update
                CoordinatesX.Add(CoordinatesX[iPoint - 1] + deltaX);
                CoordinatesY.Add(CoordinatesY[iPoint - 1] + deltaY);
            }
        }
        #endregion
    }
}
