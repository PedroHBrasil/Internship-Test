using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternshipTest
{
    /*
     * Contains the inputs of a path section:
     *  - Type: string which determines if the section is a straight, a right curve or a left curve;
     *  - Length: double which represents section's length in m;
     *  - AngularLength: double which represents section's angular length in deg; 
     *  - InitialRadius: double which represents section's intial radius in m; and
     *  - FinalRadius: double which represents section's final radius in m; and
     */
    public class TabularPathSection
    {
        // Properties ---------------------------------------------------------------------------
        public SectionType Type { get; set; }
        public double Length { get; set; }
        public double InitialRadius { get; set; }
        public double FinalRadius { get; set; }

        public enum SectionType { Straight, Left, Right };
        // Constructors -------------------------------------------------------------------------
        public TabularPathSection()
        {
            Type = SectionType.Straight;
            Length = 10;
            InitialRadius = 0;
            FinalRadius = 0;
        }

        public TabularPathSection(SectionType type, double length, double initialRadius, double finalRadius)
        {
            Type = type;
            Length = length;
            InitialRadius = initialRadius;
            FinalRadius = finalRadius;
        }
    }
}
