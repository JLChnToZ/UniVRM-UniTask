using System;
#if UNITASK_IMPORTED
using System.Threading;
using Cysharp.Threading.Tasks;
using Task = Cysharp.Threading.Tasks.UniTask;
#else
using System.Threading.Tasks;
#endif

namespace VRMShaders
{
    /// <summary>
    /// Runtime (Build 後と、Editor Playing) での非同期ロードを実現する AwaitCaller.
    /// NOTE: 簡便に実装されたものなので、最適化の余地はある.
    /// </summary>
    public sealed class RuntimeOnlyAwaitCaller : IAwaitCaller
    {
#if !UNITASK_IMPORTED
        private readonly NextFrameTaskScheduler _scheduler;
#endif
        private readonly float                  _timeOutInSeconds;
        private          float                  _lastTimeoutBaseTime;

        /// <summary>
        /// タイムアウト指定可能なコンストラクタ
        /// </summary>
        /// <param name="timeOutInSeconds">NextFrameIfTimedOutがタイムアウトと見なす時間(秒単位)</param>
        public RuntimeOnlyAwaitCaller(float timeOutInSeconds = 1f / 1000f)
        {
#if !UNITASK_IMPORTED
            _scheduler = new NextFrameTaskScheduler();
#endif
            _timeOutInSeconds = timeOutInSeconds;
            ResetLastTimeoutBaseTime();
        }

        public Task NextFrame()
        {
            ResetLastTimeoutBaseTime();
#if UNITASK_IMPORTED
            return Task.Yield(CancellationToken.None);
#else
            var tcs = new TaskCompletionSource<object>();
            _scheduler.Enqueue(() => tcs.SetResult(default));
            return tcs.Task;
#endif
        }

        public Task Run(Action action)
        {
#if UNITASK_IMPORTED
            return Task.RunOnThreadPool(action);
#else
            return Task.Run(action);
#endif
        }

#if UNITASK_IMPORTED
        public UniTask<T> Run<T>(Func<T> action)
        {
            return Task.RunOnThreadPool(action);
        }
#else
        public Task<T> Run<T>(Func<T> action)
        {
            return Task.Run(action);
        }
#endif

        public Task NextFrameIfTimedOut() => CheckTimeout() ? NextFrame() : Task.CompletedTask;

        private void ResetLastTimeoutBaseTime() => _lastTimeoutBaseTime = 0f;

        private bool LastTimeoutBaseTimeNeedsReset => _lastTimeoutBaseTime == 0f;

        private bool CheckTimeout()
        {
            float t = UnityEngine.Time.realtimeSinceStartup;
            if (LastTimeoutBaseTimeNeedsReset)
            {
                _lastTimeoutBaseTime = t;
            }
            return (t - _lastTimeoutBaseTime) >= _timeOutInSeconds;
        }
    }
}
