using NUnit.Framework;
using VRMShaders;

namespace UniVRM10.Test
{
    public sealed class Vrm10ApiTests
    {
        [Test]
        public void LoadImmediately()
        {
            var loadTask = Vrm10.LoadPathAsync(
                TestAsset.AliciaPath,
                canLoadVrm0X: true,
                awaitCaller: new ImmediateCaller()
            );

#if UNITASK_IMPORTED
            Assert.AreEqual(Cysharp.Threading.Tasks.UniTaskStatus.Succeeded, loadTask.Status);
#else
            Assert.AreEqual(true, loadTask.IsCompleted);
#endif
        }
    }
}