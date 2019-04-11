using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternshipTest
{
    /// <summary>
    /// Contains the information about a tabular path's section.
    /// </summary>
    public class TabularPathSection
    {
        #region Section Types Enum
        /// <summary>
        /// Possible types of tabular section.
        /// </summary>
        public enum SectionType { Straight, Left, Right };
        #endregion
        #region Properties
        /// <summary>
        /// Section's type (Straight, Left or Right (Corners))
        /// </summary>
        public SectionType Type { get; set; }
        /// <summary>
        /// Section's length [m].
        /// </summary>
        public double Length { get; set; }
        /// <summary>
        /// Section's radius at the first point [m].
        /// </summary>
        public double InitialRadius { get; set; }
        /// <summary>
        /// Section's radius at the last point [m].
        /// </summary>
        public double FinalRadius { get; set; }
        #endregion
        #region Constructors
        public TabularPathSection() { }

        public TabularPathSection(SectionType type, double length, double initialRadius, double finalRadius)
        {
            Type = type;
            Length = length;
            InitialRadius = initialRadius;
            FinalRadius = finalRadius;
        }
        #endregion
    }
}
