using System;
using System.IO;
#if UNITASK_IMPORTED
using Cysharp.Threading.Tasks;
#else
using System.Threading.Tasks;
#endif
using UniGLTF;
using UnityEngine;
using VRMShaders;

namespace VRM
{
    public static class VrmUtility
    {
        public delegate IMaterialDescriptorGenerator MaterialGeneratorCallback(VRM.glTF_VRM_extensions vrm);
        public delegate void MetaCallback(VRMMetaObject meta);

#if UNITASK_IMPORTED
        public static async UniTask<RuntimeGltfInstance> LoadAsync(string path,
            IAwaitCaller awaitCaller = null,
            MaterialGeneratorCallback materialGeneratorCallback = null,
            MetaCallback metaCallback = null,
            ITextureDeserializer textureDeserializer = null,
            bool loadAnimation = false
            )
#else
        public static async Task<RuntimeGltfInstance> LoadAsync(string path,
            IAwaitCaller awaitCaller = null,
            MaterialGeneratorCallback materialGeneratorCallback = null,
            MetaCallback metaCallback = null,
            ITextureDeserializer textureDeserializer = null,
            bool loadAnimation = false
            )
#endif
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException(path);
            }

            if (awaitCaller == null)
            {
                Debug.LogWarning("VrmUtility.LoadAsync: awaitCaller argument is null. ImmediateCaller is used as the default fallback. When playing, we recommend RuntimeOnlyAwaitCaller.");
                awaitCaller = new ImmediateCaller();
            }

            using (GltfData data = new AutoGltfFileParser(path).Parse())
            {
                try
                {
                    var vrm = new VRMData(data);
                    IMaterialDescriptorGenerator materialGen = default;
                    if (materialGeneratorCallback != null)
                    {
                        materialGen = materialGeneratorCallback(vrm.VrmExtension);
                    }
                    using (var loader = new VRMImporterContext(
                               vrm,
                               textureDeserializer: textureDeserializer,
                               materialGenerator: materialGen,
                               loadAnimation: loadAnimation))
                    {
                        if (metaCallback != null)
                        {
                            var meta = await loader.ReadMetaAsync(awaitCaller, true);
                            metaCallback(meta);
                        }
                        return await loader.LoadAsync(awaitCaller);
                    }
                }
                catch (NotVrm0Exception)
                {
                    // retry
                    Debug.LogWarning("file extension is vrm. but not vrm ?");
                    using (var loader = new UniGLTF.ImporterContext(data))
                    {
                        return await loader.LoadAsync(awaitCaller);
                    }
                }
            }
        }


#if UNITASK_IMPORTED    
        public static async UniTask<RuntimeGltfInstance> LoadBytesAsync(string path,
            byte[] bytes,
            IAwaitCaller awaitCaller = null,
            MaterialGeneratorCallback materialGeneratorCallback = null,
            MetaCallback metaCallback = null,
            ITextureDeserializer textureDeserializer = null,
            bool loadAnimation = false
            )
#else
        public static async Task<RuntimeGltfInstance> LoadBytesAsync(string path,
            byte[] bytes,
            IAwaitCaller awaitCaller = null,
            MaterialGeneratorCallback materialGeneratorCallback = null,
            MetaCallback metaCallback = null,
            ITextureDeserializer textureDeserializer = null,
            bool loadAnimation = false
            )
#endif
        {
            if (bytes == null)
            {
                throw new ArgumentNullException("bytes");
            }

            if (awaitCaller == null)
            {
                Debug.LogWarning("VrmUtility.LoadAsync: awaitCaller argument is null. ImmediateCaller is used as the default fallback. When playing, we recommend RuntimeOnlyAwaitCaller.");
                awaitCaller = new ImmediateCaller();
            }

            using (GltfData data = new GlbBinaryParser(bytes, path).Parse())
            {
                try
                {
                    var vrm = new VRMData(data);
                    IMaterialDescriptorGenerator materialGen = default;
                    if (materialGeneratorCallback != null)
                    {
                        materialGen = materialGeneratorCallback(vrm.VrmExtension);
                    }
                    using (var loader = new VRMImporterContext(
                               vrm,
                               textureDeserializer: textureDeserializer,
                               materialGenerator: materialGen,
                               loadAnimation: loadAnimation))
                    {
                        if (metaCallback != null)
                        {
                            var meta = await loader.ReadMetaAsync(awaitCaller, true);
                            metaCallback(meta);
                        }
                        return await loader.LoadAsync(awaitCaller);
                    }
                }
                catch (NotVrm0Exception)
                {
                    // retry
                    Debug.LogWarning("file extension is vrm. but not vrm ?");
                    using (var loader = new UniGLTF.ImporterContext(data))
                    {
                        return await loader.LoadAsync(awaitCaller);
                    }
                }
            }
        }
    }
}
