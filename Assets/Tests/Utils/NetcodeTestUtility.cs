using UnityEngine;
using Unity.Netcode;

namespace Tests.Utils
{
    public static class NetcodeTestUtility
    {
        public static NetworkManager CreateNetworkManager()
        {
            var go = new GameObject("TestNM");
            var nm = go.AddComponent<NetworkManager>();
            nm.NetworkConfig = new NetworkConfig          // ← исправлено
            {
                EnableSceneManagement = false
            };
            return nm;
        }
    }
}
