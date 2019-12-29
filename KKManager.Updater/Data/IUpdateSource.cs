using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace KKManager.Updater.Data
{
    public interface IUpdateSource : IDisposable
    {
        //bool IsValidUri(Uri source);
        Task<List<UpdateTask>> GetUpdateItems(CancellationToken cancellationToken);
    }
}