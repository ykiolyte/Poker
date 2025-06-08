using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Poker.Gameplay;
using Poker.Presentation;
using Poker.UI;

namespace Poker.Installers
{
    public class TableInstaller : MonoInstaller
    {
        [Header("Dependencies")]
        [SerializeField] private NetworkLock _networkLock;
        [SerializeField] private NetworkTimer _networkTimer;
        [SerializeField] private PlayerUIAdapter _uiAdapter;

        public override void InstallBindings()
        {
            Container.Bind<NetworkLock>().FromInstance(_networkLock).AsSingle();
            Container.Bind<NetworkTimer>().FromInstance(_networkTimer).AsSingle();
            Container.Bind<GameStateMachine>().AsSingle();

            Container.Bind<IHandRankingService>().To<DummyHandRanking>().AsTransient();

            Container.Bind<IPlayerGateway>().WithId("Local")
            .FromMethod(ctx =>
                new LocalPlayerGateway(0, _uiAdapter)).AsCached();

            Container.Bind<IPlayerGateway>().WithId("Bot")
                .FromMethod(ctx =>
                    new BotPlayerGateway(1,
                        new SimpleBotDecisionMaker(SimpleBotDecisionMaker.Aggression.Neutral),
                        ctx.Container.Resolve<IHandRankingService>(),
                        1000)).AsCached();

            Container.Bind<IPlayerGateway>().WithId("Remote")
                .FromMethod(ctx => new RemotePlayerGatewayStub()).AsCached();

            Container.Bind<List<IPlayerGateway>>()
                .FromMethod(ctx => new List<IPlayerGateway>
                {
                    ctx.Container.ResolveId<IPlayerGateway>("Local"),
                    ctx.Container.ResolveId<IPlayerGateway>("Bot"),
                    ctx.Container.ResolveId<IPlayerGateway>("Remote")
                }).AsSingle();

            Container.Bind<BettingRoundService>().AsSingle()
                .WithArguments(
                    Container.Resolve<List<IPlayerGateway>>(),
                    Container.Resolve<GameStateMachine>(),
                    Container.Resolve<NetworkTimer>(),
                    Container.Resolve<NetworkLock>());
        }
    }
}
