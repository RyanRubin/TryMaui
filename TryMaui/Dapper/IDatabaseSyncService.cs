using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TryMaui.Dapper
{
    public interface IDatabaseSyncService
    {
        Task<int> ExecuteSync(string srcConnectionStringName, string destConnectionStringName);
    }
}
