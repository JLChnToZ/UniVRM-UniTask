using System.IO;
using UnityEngine;

namespace UniVRM10
{
    public static class TestAsset
    {
        public static string AliciaPath
        {
            get
            {
                return Path.GetFullPath(Application.dataPath + "/../Tests/Models/Alicia_vrm-0.51/AliciaSolid_vrm-0.51.vrm")
                    .Replace("\\", "/");
            }
        }

        public static Vrm10Instance LoadAlicia()
        {
            var task = Vrm10.LoadPathAsync(AliciaPath, canLoadVrm0X: true);
#if UNITASK_IMPORTED
            var instance = task.GetAwaiter().GetResult();
#else
            task.Wait();
            var instance = task.Result;
#endif

            return instance.GetComponent<Vrm10Instance>();
        }
    }
}
