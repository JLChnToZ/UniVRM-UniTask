using System;
using System.IO;
#if UNITASK_IMPORTED
using Cysharp.Threading.Tasks;
#else
using System.Threading.Tasks;
#endif
using UnityEngine;
using VRMShaders;

namespace UniGLTF
{
    public static class GltfUtility
    {
#if UNITASK_IMPORTED
        public static async UniTask<RuntimeGltfInstance> LoadAsync(string path, IAwaitCaller awaitCaller = null, IMaterialDescriptorGenerator materialGenerator = null)
#else
        public static async Task<RuntimeGltfInstance> LoadAsync(string path, IAwaitCaller awaitCaller = null, IMaterialDescriptorGenerator materialGenerator = null)
#endif
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException(path);
            }

            if (awaitCaller == null)
            {
                Debug.LogWarning("GltfUtility.LoadAsync: awaitCaller argument is null. ImmediateCaller is used as the default fallback. When playing, we recommend RuntimeOnlyAwaitCaller.");
                awaitCaller = new ImmediateCaller();
            }

            using (GltfData data = new AutoGltfFileParser(path).Parse())
            using (var loader = new UniGLTF.ImporterContext(data, materialGenerator: materialGenerator))
            {
                return await loader.LoadAsync(awaitCaller);
            }
        }

#if UNITASK_IMPORTED
        public static async UniTask<RuntimeGltfInstance> LoadBytesAsync(string path, byte[] bytes, IAwaitCaller awaitCaller = null, IMaterialDescriptorGenerator materialGenerator = null)
#else
        public static async Task<RuntimeGltfInstance> LoadBytesAsync(string path, byte[] bytes, IAwaitCaller awaitCaller = null, IMaterialDescriptorGenerator materialGenerator = null)
#endif
        {
            if (bytes == null)
            {
                throw new ArgumentNullException("bytes");
            }

            if (awaitCaller == null)
            {
                Debug.LogWarning("GltfUtility.LoadAsync: awaitCaller argument is null. ImmediateCaller is used as the default fallback. When playing, we recommend RuntimeOnlyAwaitCaller.");
                awaitCaller = new ImmediateCaller();
            }

            using (GltfData data = new GlbBinaryParser(bytes, path).Parse())
            using (var loader = new UniGLTF.ImporterContext(data, materialGenerator: materialGenerator))
            {
                return await loader.LoadAsync(awaitCaller);
            }
        }
    }
}
