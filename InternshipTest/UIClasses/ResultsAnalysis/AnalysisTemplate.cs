using Syncfusion.Windows.Tools.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternshipTest.UIClasses.ResultsAnalysis
{
    public class AnalysisTemplate : GenericInfo
    {
        #region Properties
        public List<LapTimeSimulation2DChartParameters> ChartsParameters { get; set; }
        #endregion
        #region Constructors
        public AnalysisTemplate() { }
        public AnalysisTemplate(string id, string description, List<LapTimeSimulation2DChartParameters> chartParameters)
        {
            ID = id;
            Description = description;
            ChartsParameters = chartParameters;
        }
        #endregion
    }
}
