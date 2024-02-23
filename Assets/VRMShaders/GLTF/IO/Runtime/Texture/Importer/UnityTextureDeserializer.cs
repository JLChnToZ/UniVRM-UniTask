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
    public sealed class UnityTextureDeserializer : ITextureDeserializer
    {
#if UNITASK_IMPORTED
        public async UniTask<Texture2D> LoadTextureAsync(DeserializingTextureInfo textureInfo, IAwaitCaller awaitCaller)
#else
        public async Task<Texture2D> LoadTextureAsync(DeserializingTextureInfo textureInfo, IAwaitCaller awaitCaller)
#endif
        {
            switch (textureInfo.DataMimeType)
            {
                case "image/png":
                    break;
                case "image/jpeg":
                    break;
                default:
                    if (string.IsNullOrEmpty(textureInfo.DataMimeType))
                    {
                        Debug.Log($"Texture image MIME type is empty.");
                    }
                    else
                    {
                        Debug.Log($"Texture image MIME type `{textureInfo.DataMimeType}` is not supported.");
                    }
                    break;
            }

            var texture = new Texture2D(2, 2, TextureFormat.ARGB32, textureInfo.UseMipmap, textureInfo.ColorSpace == ColorSpace.Linear);
            if (textureInfo.ImageData != null)
            {
                texture.LoadImage(textureInfo.ImageData);
                texture.wrapModeU = textureInfo.WrapModeU;
                texture.wrapModeV = textureInfo.WrapModeV;
                texture.filterMode = textureInfo.FilterMode;
                await awaitCaller.NextFrame();
            }

            return texture;
        }
    }
}
