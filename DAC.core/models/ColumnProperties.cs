using DAC.core.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAC.core.models
{
    public class ColumnProperties
    {

        /// <summary>
        /// Table based allow insert enabled.
        /// </summary>
        public bool AllowInsert { get; set; }
        /// <summary>
        /// Table based allow update enabled.
        /// </summary>
        public bool AllowUpdate { get; set; }
        /// <summary>
        /// Allow null value
        /// </summary>
        public bool AllowNull { get; set; }
        /// <summary>
        /// Custom Format Data
        /// </summary>
        public int CustomFormatter { get; set; }

        public bool isPrimary { get; set; }
        public bool isIdentity { get; set; }
        public bool isUnique { get; set; }
        public bool isIndex { get; set; }
        public bool IndexDescending { get; set; }
        public bool isReadonly { get; set; }
        public string CustomComputed { get; set; }
        public SelectBehaviors SelectBehavior { get; set; }
    }
}
