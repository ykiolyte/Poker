// Assets/Scripts/Server/ServerBlindBootstrapper.cs
using UnityEngine;
using Poker.Server;
using Poker.GameLoop;

namespace Poker.Server
{
    public sealed class ServerBlindBootstrapper : MonoBehaviour
    {
        [SerializeField] private TableServerController server;
        [SerializeField] private DealerButtonService dealer;

        void Awake()
        {
            if (server == null) server = FindAnyObjectByType<TableServerController>();
            if (dealer == null) dealer = FindAnyObjectByType<DealerButtonService>();

            var gsm = FindAnyObjectByType<GameStateManager>();
            gsm.RoundStarted += OnPreflopStarted;
        }

        void OnPreflopStarted()
        {
            if (!server.IsServer) return;
            server.PostBlindsServerRpc(
                dealer.SmallBlindIndex,
                dealer.BigBlindIndex);
        }
    }
}
