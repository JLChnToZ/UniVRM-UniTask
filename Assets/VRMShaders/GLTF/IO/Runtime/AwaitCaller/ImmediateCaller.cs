using System;
#if UNITASK_IMPORTED
using Cysharp.Threading.Tasks;
using Task = Cysharp.Threading.Tasks.UniTask;
#else
using System.Threading.Tasks;
#endif

namespace VRMShaders
{
    /// <summary>
    /// 同期実行
    /// </summary>
    public sealed class ImmediateCaller : IAwaitCaller
    {
        public Task NextFrame()
        {
#if UNITASK_IMPORTED
            return Task.CompletedTask;
#else
            return Task.FromResult<object>(null);
#endif
        }

        public Task Run(Action action)
        {
            action();
#if UNITASK_IMPORTED
            return Task.CompletedTask;
#else
            return Task.FromResult<object>(null);
#endif
        }

#if UNITASK_IMPORTED
        public UniTask<T> Run<T>(Func<T> action)
#else
        public Task<T> Run<T>(Func<T> action)
#endif
        {
            return Task.FromResult(action());
        }

        public Task NextFrameIfTimedOut() => NextFrame();
    }
}