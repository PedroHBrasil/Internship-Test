using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternshipTest
{
    public class TabularPathSectionViewModel
    {
        public ObservableCollection<TabularPathSection> TabularPathSections { get; set; }

        public TabularPathSectionViewModel()
        {
            TabularPathSections = new ObservableCollection<TabularPathSection>()
            {
                new TabularPathSection(TabularPathSection.SectionType.Straight, 50, 0, 0),
                new TabularPathSection(TabularPathSection.SectionType.Left, 31.41, 10, 10),
                new TabularPathSection(TabularPathSection.SectionType.Right, 15.70, 5, 5),
                new TabularPathSection(TabularPathSection.SectionType.Left, 31.41, 10, 10),
                new TabularPathSection(TabularPathSection.SectionType.Straight, 50, 0, 0),
                new TabularPathSection(TabularPathSection.SectionType.Left, 78.54, 25, 25)
            };
        }
    }
}
