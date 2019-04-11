using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternshipTest
{
    /// <summary>
    /// Contains the basic information of a project's object.
    /// </summary>
    [Serializable]
    public class GenericInfo
    {
        #region Properties
        /// <summary>
        /// Object's identification.
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// Object's main details.
        /// </summary>
        public string Description { get; set; }
        #endregion
        #region Constructors
        public GenericInfo() { }
        public GenericInfo(string id, string description)
        {
            ID = id;
            Description = description;
        }
        #endregion
        #region Methods
        public override string ToString()
        {
            return ID;
        }
        #endregion
    }
}
