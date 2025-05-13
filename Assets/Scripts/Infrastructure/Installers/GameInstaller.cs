using Poker.Gameplay.Cards;
using Poker.Gameplay.Factories;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;
using Poker.Core.Config;

namespace Poker.Infrastructure.Installers
{
    /// <summary>
    /// Регистрирует зависимости: пул карт, фабрика карт и конфиг‑SO.
    /// Подключается к сцене единым префабом «Installer».
    /// </summary>
    public sealed class GameInstaller : MonoInstaller
    {
        [Header("Refs")]
        [SerializeField] private CardPool cardPool;
        [SerializeField] private GameSettingsSO gameSettings;
        [SerializeField] private AssetReference cardPrefabAsset;

        public override void InstallBindings()
        {
            // Конфиги
            Container.BindInstance(gameSettings).AsSingle();

            // Пул и фабрика
            Container.BindInstance(cardPool).AsSingle();
            Container.Bind<CardFactory>().AsSingle()
                      .WithArguments(cardPool, cardPrefabAsset);
        }
    }
}
