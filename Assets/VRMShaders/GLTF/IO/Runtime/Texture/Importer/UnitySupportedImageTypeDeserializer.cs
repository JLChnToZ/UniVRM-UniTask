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
    /// Unity の ImageConversion.LoadImage を用いて PNG/JPG の読み込みを実現する
    /// </summary>
    public sealed class UnitySupportedImageTypeDeserializer : ITextureDeserializer
    {

        #if UNITASK_IMPORTED
        public async UniTask<Texture2D> LoadTextureAsync(DeserializingTextureInfo textureInfo, IAwaitCaller awaitCaller)
        #else
        public async Task<Texture2D> LoadTextureAsync(DeserializingTextureInfo textureInfo, IAwaitCaller awaitCaller)
        #endif
        {
            if (textureInfo.ImageData == null) return null;

            try
            {
                var texture = new Texture2D(2, 2, TextureFormat.ARGB32, textureInfo.UseMipmap, textureInfo.ColorSpace == ColorSpace.Linear);
                texture.LoadImage(textureInfo.ImageData);
                await awaitCaller.NextFrame();

                texture.wrapModeU = textureInfo.WrapModeU;
                texture.wrapModeV = textureInfo.WrapModeV;
                texture.filterMode = textureInfo.FilterMode;
                return texture;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }
    }
}