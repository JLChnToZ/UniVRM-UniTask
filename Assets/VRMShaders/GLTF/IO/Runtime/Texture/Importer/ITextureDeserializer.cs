using System;
#if UNITASK_IMPORTED
using Cysharp.Threading.Tasks;
#else
using System.Threading.Tasks;
#endif
using UnityEngine;

namespace VRMShaders
{
    /// <summary>
    /// 単純に Texture2D アセットを生成する機能
    /// </summary>
    public interface ITextureDeserializer
    {
        /// <summary>
        /// imageData をもとに Texture2D を生成する.
        /// await する場合は awaitCaller を用いて await しなければならない。(Editor では同期ロードをしなければならないため)
        /// </summary>
        #if UNITASK_IMPORTED
        UniTask<Texture2D> LoadTextureAsync(DeserializingTextureInfo textureInfo, IAwaitCaller awaitCaller);
        #else
        Task<Texture2D> LoadTextureAsync(DeserializingTextureInfo textureInfo, IAwaitCaller awaitCaller);
        #endif
    }
}
