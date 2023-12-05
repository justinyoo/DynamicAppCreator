using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAC.core.models
{
    public class ColumnMaskValidation
    {
        /// <summary>
        /// Mask is optional
        /// </summary>
        public string Mask { get; set; }

        /// <summary>
        /// Regex rule
        /// </summary>
        public string ValidationRule { get; set; }
        /// <summary>
        /// Validation Error Message
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}
