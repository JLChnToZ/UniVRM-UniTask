using System;
using UnityEngine;
using VRMShaders;
#if UNITASK_IMPORTED
using Cysharp.Threading.Tasks;
#endif

namespace UniGLTF
{
    public static class ImporterContextExtensions
    {
        /// <summary>
        /// Build unity objects from parsed gltf
        /// </summary>
        public static RuntimeGltfInstance Load(this ImporterContext self)
        {
            var meassureTime = new ImporterContextSpeedLog();
            var task = self.LoadAsync(new ImmediateCaller(), meassureTime.MeasureTime);
            #if UNITASK_IMPORTED
            return task.GetAwaiter().GetResult();
            #else
            if (!task.IsCompleted)
            {
                throw new Exception();
            }
            if (task.IsFaulted)
            {
                throw new AggregateException(task.Exception);
            }

            if (Symbols.VRM_DEVELOP)
            {
                Debug.Log($"{self.Data.TargetPath}: {meassureTime.GetSpeedLog()}");
            }

            return task.Result;
            #endif
        }
    }
}
