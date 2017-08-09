using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blades.DataStore.Interfaces
{
    /// <summary>
    /// Dispose method for rollback and destroy
    /// Commit method for commit and destroy
    /// </summary>
    public interface ITransactRepository : IDisposable
    {

        /// <summary>
        /// Commit then Dispose
        /// </summary>
        void Commit();
    }
}
