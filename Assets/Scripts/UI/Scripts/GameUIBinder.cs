using UnityEngine;
using Poker.UI;
using Poker.GameLoop;

namespace Poker.Infrastructure
{
    public sealed class GameUIBinder : MonoBehaviour
    {
        [SerializeField] private GameUIController canvasController;
        [SerializeField] private PokerPlayerController localPlayer;

        private void Awake()
        {
            if (canvasController == null || localPlayer == null)
            {
                Debug.LogError("[GameUIBinder] canvasController или localPlayer не назначен");
                enabled = false;
                return;
            }

            var presenter = localPlayer.GetComponent<PlayerUIController>()
                         ?? localPlayer.gameObject.AddComponent<PlayerUIController>();

            presenter.Initialize(canvasController);
            localPlayer.SetAsLocalPlayer();
        }
    }
}
