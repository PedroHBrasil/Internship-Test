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
        public List<TabItemExt> ChartsTabs { get; set; }
        #endregion
        #region Constructors
        public AnalysisTemplate() { }
        public AnalysisTemplate(string id, string description, List<TabItemExt> chartsTabs)
        {
            ID = id;
            Description = description;
            ChartsTabs = chartsTabs;
        }
        #endregion
        #region Methods

        #endregion
    }
}
