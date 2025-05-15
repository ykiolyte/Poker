using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Poker.Gameplay.Cards;
using Poker.Gameplay.Factories;

namespace Poker.GameLoop
{
    /// <summary>
    /// Управляет одной раздачей (Texas Hold’em) — от тасовки до объявления победителя.
    /// </summary>
    public sealed class RoundManager : MonoBehaviour
    {
        #region Inspector

        [Header("Сцена")]
        [SerializeField] private List<PokerPlayerController> players = new();
        [SerializeField] private TableView               tableView;
        [SerializeField] private CardPool                cardPool;
        [SerializeField] private AssetReference          cardPrefab;

        [Header("Тайминги (сек)")]
        [SerializeField] private float dealDelay  = 0.30f;
        [SerializeField] private float boardDelay = 0.40f;
        [SerializeField] private float handPause  = 3.00f;

        #endregion

        private DeckManager  deck;
        private CardFactory  factory;
        private WaitForSeconds wd, wb, wh;

        /// <summary>Всегда актуальный список community-карт (флоп → ривер).</summary>
        private readonly List<CardDataSO> boardCards = new();

        #region Life-cycle ----------------------------------------------------

        private void Awake()
        {
            deck    = new DeckManager();
            factory = new CardFactory(cardPool, cardPrefab);

            wd = new WaitForSeconds(dealDelay);
            wb = new WaitForSeconds(boardDelay);
            wh = new WaitForSeconds(handPause);
        }

        private void Start() => StartCoroutine(GameLoop());

        private IEnumerator GameLoop()
        {
            while (true)
            {
                SetupNewHand();

                yield return DealHoleCards();   // две карманные
                yield return RevealBoard(3);    // флоп
                yield return RevealBoard(1);    // терн
                yield return RevealBoard(1);    // ривер

                EvaluateWinners();

                yield return wh;                // пауза и новая раздача
            }
        }

        #endregion

        #region Hand flow -----------------------------------------------------

        private void SetupNewHand()
        {
            var all = Resources.LoadAll<CardDataSO>("Configs");
            deck.InitializeDeck(all);
            deck.Shuffle();

            tableView.ResetBoard();
            boardCards.Clear();
            foreach (var p in players) p.ResetForNewHand();

            Debug.Log($"[Round] Loaded {all.Length} cards");
        }

        private IEnumerator DealHoleCards()
        {
            for (int r = 0; r < 2; r++)
            {
                foreach (var p in players)
                {
                    var card = deck.DrawCard();
                    Debug.Log($"[Deal] P#{players.IndexOf(p)+1} gets {card.rank} of {card.suit}");
                    yield return RunTask(p.DealHoleCardAsync(card, factory));
                    yield return wd;
                }
            }
        }

        private IEnumerator RevealBoard(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var card = deck.DrawCard();
                boardCards.Add(card);    // важно сохранить порядок

                Debug.Log($"[Board] Slot{boardCards.Count-1}: {card.rank} of {card.suit}");
                yield return RunTask(tableView.ShowBoardCardAsync(card, factory));
                yield return wb;
            }
        }

        private void EvaluateWinners()
        {
            Debug.Log("[Eval] Community: " +
                      string.Join(", ", boardCards.Select(c => $"{c.rank} {c.suit}")));

            foreach (var p in players)
            {
                var hole = p.Model.HoleCards;
                Debug.Log($"[Eval] P#{players.IndexOf(p)+1} hole: " +
                          string.Join(", ", hole.Select(c => $"{c.rank} {c.suit}")));
            }

            // оцениваем всех игроков
            var results = players.ToDictionary(
                p => p,
                p => HandEvaluator.EvaluateBestHand(p.Model.HoleCards.ToList(), boardCards)
            );

            // находим лучшую руку
            var best = results.Values.Aggregate((a, b) =>
                         a.Score.CompareTo(b.Score) > 0 ? a : b);

            var winners = results
                .Where(kv => kv.Value.Score.CompareTo(best.Score) == 0)
                .Select(kv => kv.Key);

            foreach (var w in winners)
            {
                var hr    = results[w];
                var combo = string.Join(", ",
                    hr.Combination.Select(c => $"{c.rank} {c.suit}"));

                Debug.Log($"🏆 P#{players.IndexOf(w)+1} wins: {hr.Score.Rank} [{combo}]");
            }
        }

        #endregion

        #region Utils ---------------------------------------------------------

        private static IEnumerator RunTask(Task task)
        {
            while (!task.IsCompleted) yield return null;
        }

        #endregion
    }
}
