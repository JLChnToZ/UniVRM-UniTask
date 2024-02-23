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
    /// WebGL など Thread が無いもの向け
    /// </summary>
    public sealed class RuntimeOnlyNoThreadAwaitCaller : IAwaitCaller
    {
#if !UNITASK_IMPORTED
        private readonly NextFrameTaskScheduler _scheduler;
#endif
        private readonly float                  _timeoutInSeconds;
        private          float                  _lastTimeoutBaseTime;

        /// <summary>
        /// タイムアウト指定可能なコンストラクタ
        /// </summary>
        /// <param name="timeoutInSeconds">NextFrameIfTimedOutがタイムアウトと見なす時間(秒単位)</param>
        public RuntimeOnlyNoThreadAwaitCaller(float timeoutInSeconds = 1f / 1000f)
        {
#if !UNITASK_IMPORTED
            _scheduler = new NextFrameTaskScheduler();
#endif
            _timeoutInSeconds = timeoutInSeconds;
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
            try
            {
                action();
#if UNITASK_IMPORTED
                return Task.CompletedTask;
#else
                return Task.FromResult<object>(null);
#endif
            }
            catch (Exception ex)
            {
                return Task.FromException(ex);
            }
        }

#if UNITASK_IMPORTED
        public UniTask<T> Run<T>(Func<T> action)
#else
        public Task<T> Run<T>(Func<T> action)
#endif
        {
            try
            {
                return Task.FromResult(action());
            }
            catch (Exception ex)
            {
                return Task.FromException<T>(ex);
            }
        }

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
            return (t - _lastTimeoutBaseTime) >= _timeoutInSeconds;
        }
    }
}