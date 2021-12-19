using System.Threading;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;

namespace UniversalUnity.Helpers.Coroutines
{
    public static class CancellationTokenSourceExtensions
    {
        /// <summary>
        /// Cancels specified <see cref="CancellationTokenSource"/> and links it to the <paramref name="component"/>'s Destroy call.
        /// </summary>
        public static void CancelAndLinkToDestroy(this CancellationTokenSource source, [NotNull] Component component)
        {
            source?.Cancel();
            source = new CancellationTokenSource();
            source.RegisterRaiseCancelOnDestroy(component);
        }
        
        /// <summary>
        /// Cancels specified <see cref="CancellationTokenSource"/> and links it to the <paramref name="obj"/>'s Destroy call.
        /// </summary>
        public static void CancelAndLinkToDestroy(this CancellationTokenSource source, [NotNull] GameObject obj)
        {
            source?.Cancel();
            source = new CancellationTokenSource();
            source.RegisterRaiseCancelOnDestroy(obj);
        }
    }
}