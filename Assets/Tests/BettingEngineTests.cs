using NUnit.Framework;
using UnityEngine;
using Unity.Netcode;
using Tests.Utils;                // new
using Poker.Server;
using Poker.Game.Betting;
using Poker.GameLoop;
using Poker.Gameplay;
using Poker.Core.Config;

public class BettingEngineTests
{
    NetworkManager nm;
    NetworkBettingEngine engine;
    GameSettingsSO settings;
    TableServerController table;

    [SetUp]
    public void Setup()
    {
        nm = NetcodeTestUtility.CreateNetworkManager();

        settings = ScriptableObject.CreateInstance<GameSettingsSO>();
        settings.Init(10, 20);

        table = new GameObject("Table").AddComponent<TableServerController>();
        table.gameObject.AddComponent<NetworkObject>();
        ReflectionUtility.SetField(table, "gameSettings", settings);

        engine = new GameObject("Engine").AddComponent<NetworkBettingEngine>();
        engine.gameObject.AddComponent<NetworkObject>();
        ReflectionUtility.SetField(engine, "settings", settings);
        ReflectionUtility.SetField(engine, "table", table);

        nm.StartHost();
    }

    [TearDown] public void TearDown() => Object.DestroyImmediate(nm.gameObject);

    [Test]
    public void PostBlinds_TotalEqualsSBplusBB()
    {
        table.PostBlindsServerRpc(0, 1);
        Assert.AreEqual(30, table.Pots.Total);
    }

    [Test]
    public void RaiseCall_CompletesStreet()
    {
        engine.BeginStreet();
        engine.ReceiveRemoteAction(0, ActionType.Raise, 40);
        engine.ReceiveRemoteAction(1, ActionType.Call, 40);

        bool active = ReflectionUtility.GetField<bool>(engine, "bettingActive");
        Assert.IsFalse(active);
    }
}
