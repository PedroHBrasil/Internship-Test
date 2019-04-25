using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternshipTest.Vehicle
{
    /// <summary>
    /// Contains the observable collections
    /// </summary>
    public class TireModelMF52ViewModel
    {
        #region Properties
        public ObservableCollection<TireModelMF52Point> TireModelMF52Points { get; set; }
        #endregion
        #region Constructors
        public TireModelMF52ViewModel()
        {
            TireModelMF52Points = new ObservableCollection<TireModelMF52Point>();
        }
        public TireModelMF52ViewModel (List<TireModelMF52Point> tireModelMF52Points)
        {
            TireModelMF52Points = new ObservableCollection<TireModelMF52Point>(tireModelMF52Points);
        }
        #endregion
    }
}
