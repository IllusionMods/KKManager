using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace KKManager.Functions.Update {
    public interface IUpdateSource : IDisposable
    {
        //bool IsValidUri(Uri source);
        Task<List<UpdateTask>> GetUpdateItems(CancellationToken cancellationToken);
    }
}