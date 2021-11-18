using System.Threading;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;

namespace UniversalUnity.Helpers.Coroutines
{
    public static class CancellationTokenExtension
    {
        public static CancellationTokenSource CancelAndLinkToDestroy([CanBeNull] CancellationTokenSource source, [NotNull] Component component)
        {
            source?.Cancel();
            source = new CancellationTokenSource();
            source.RegisterRaiseCancelOnDestroy(component);
            return source;
        }
    }
}