using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace UniversalUnity.Helpers.Coroutines
{
    public static class CancellationTokenExtension
    {
        public static CancellationTokenSource CancelDisposeAndLinkToDestroy(CancellationTokenSource source, Component component)
        {
            source?.Cancel();
            source?.Dispose();
            source = new CancellationTokenSource();
            source.RegisterRaiseCancelOnDestroy(component);
            return source;
        }
    }
}