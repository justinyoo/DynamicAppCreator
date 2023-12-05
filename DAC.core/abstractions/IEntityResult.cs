using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAC.core.abstractions
{
    public enum EntityResultStates
    {
        success,
        error
    }
    public interface IEntityResult<TEntity>
    {
        TEntity Result { get; set; }
        bool state { get; set; }
        Dictionary<string, string> Errors { get; set; }
        public string Message { get; set; }
    }
}
