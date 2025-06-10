using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Poker.Gameplay;
using Poker.Presentation;
using Poker.UI;
using Poker.Server;

namespace Poker.Installers
{
    public class TableInstaller : MonoInstaller
    {
        [Header("Dependencies")]
        [SerializeField] private NetworkLock networkLock;
        [SerializeField] private NetworkTimer networkTimer;
        [SerializeField] private PlayerUIAdapter playerUIAdapter;
        [SerializeField] private RemotePlayerGateway remoteGatewayPrefab;
        [SerializeField] private NetworkBettingEngine bettingEngine;

        public override void InstallBindings()
        {
            // Core sync utilities
            Container.Bind<NetworkLock>().FromInstance(networkLock).AsSingle();
            Container.Bind<NetworkTimer>().FromInstance(networkTimer).AsSingle();

            // FSM
            Container.Bind<GameStateMachine>().AsSingle();

            // Dummy evaluator (заменить позже на HandEvaluatorService)
            Container.Bind<IHandRankingService>().To<DummyHandRanking>().AsTransient();

            // Gateways
            BindLocalPlayerGateway();
            BindBotPlayerGateway();
            BindRemotePlayerGateway();

            // Собираем список всех игроков
            Container.Bind<List<IPlayerGateway>>().FromMethod(ctx => new List<IPlayerGateway>
            {
                ctx.Container.ResolveId<IPlayerGateway>("Local"),
                ctx.Container.ResolveId<IPlayerGateway>("Bot"),
                ctx.Container.ResolveId<IPlayerGateway>("Remote")
            }).AsSingle();

            // Betting Engine (уже на сцене)
            Container.Bind<NetworkBettingEngine>().FromInstance(bettingEngine).AsSingle();
        }

        private void BindLocalPlayerGateway()
        {
            Container.Bind<IPlayerGateway>().WithId("Local")
                .FromMethod(_ => new LocalPlayerGateway(0, playerUIAdapter))
                .AsCached();
        }

        private void BindBotPlayerGateway()
        {
            Container.Bind<IPlayerGateway>().WithId("Bot")
                .FromMethod(ctx =>
                    new BotPlayerGateway(1,
                        new SimpleBotDecisionMaker(SimpleBotDecisionMaker.Aggression.Neutral),
                        ctx.Container.Resolve<IHandRankingService>(),
                        1000))
                .AsCached();
        }

        private void BindRemotePlayerGateway()
        {
            Container.Bind<IPlayerGateway>().WithId("Remote")
                .FromComponentInNewPrefab(remoteGatewayPrefab)
                .UnderTransformGroup("RemoteGateways")
                .AsCached()
                .OnInstantiated<RemotePlayerGateway>((ctx, gw) => gw.Initialise(2));
        }
    }
}
