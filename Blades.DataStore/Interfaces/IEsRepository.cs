using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Blades.DataStore.Interfaces
{
    public interface IEsRepository : ITransactRepository, Blades.Es.Interfaces.IEsRepository
    {
    }
}
