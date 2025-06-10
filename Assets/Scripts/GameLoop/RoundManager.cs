// Assets/Scripts/GameLoop/RoundManager.cs
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
    /// Управляет одной раздачей: тасовка, раздача, борд, подсчёт победителя.
    /// Добавлен метод ClearTable() для удаления предыдущих карт.
    /// </summary>
    public sealed class RoundManager : MonoBehaviour
    {
        #region Inspector -----------------------------------------------------

        [Header("Scene refs")]
        private List<PokerPlayerController> players;

        [SerializeField] private TableView  tableView;
        [SerializeField] private CardPool   cardPool;
        [SerializeField] private AssetReference cardPrefab;

        [Header("Timings, sec")]
        [SerializeField] private float dealDelay  = .3f;
        [SerializeField] private float boardDelay = .4f;

        #endregion

        private DeckManager deck;
        private CardFactory factory;
        private WaitForSeconds wd, wb;

        public readonly List<CardDataSO> BoardCards = new();   // community-карты

        #region Unity ---------------------------------------------------------

        void Awake()
        {
            deck = new DeckManager();
            wd   = new WaitForSeconds(dealDelay);
            wb   = new WaitForSeconds(boardDelay);

            players = FindObjectsByType<PokerPlayerController>(FindObjectsSortMode.None).ToList();

            // инициализация моделей
            for (int i = 0; i < players.Count; i++)
                players[i].InjectModel(new PokerPlayerModel(i, 1000));

            if (cardPrefab != null && cardPrefab.RuntimeKeyIsValid())
            {
                factory = new CardFactory(cardPool, cardPrefab);
            }
            else
            {
                Debug.LogWarning("[RoundManager] Card prefab не назначен – будут fallback-карты");
                factory = new CardFactory(cardPool, null);
            }

            Debug.Log($"[Init] Found {players.Count} players");
        }

        #endregion

        #region Public API (корутины) ----------------------------------------

        /// <summary>Подготовка новой раздачи: сброс стола и тасование новой колоды.</summary>
        public IEnumerator SetupNewHandRoutine()
        {
            ClearTable();

            var allCards = Resources.LoadAll<CardDataSO>("Configs");
            deck.InitializeDeck(allCards);
            deck.Shuffle();

            BoardCards.Clear();
            foreach (var p in players) p.ResetForNewHand();

            Debug.Log($"[Round] New hand → deck of {allCards.Length} cards ready");
            yield break;
        }

        public IEnumerator DealHoleCardsRoutine()
        {
            for (int cardIndex = 0; cardIndex < 2; cardIndex++)
            {
                foreach (var player in players)
                {
                    var cardData = deck.DrawCard();
                    var cardView = factory.Create(cardData);

                    var anchor = player.GetCardAnchor(isLeftHand: cardIndex == 0);
                    cardView.transform.SetPositionAndRotation(anchor.position, anchor.rotation);
                    cardView.transform.SetParent(anchor, worldPositionStays: true);

                    // веерная раскладка
                    CardFanUtility.ApplyFanArcOffset(cardView.transform, cardIndex, 2);

                    player.Model.DealCard(cardData);
                    yield return wd;
                }
            }
        }

        public IEnumerator RevealFlopRoutine()  => RevealBoardRoutine(3);
        public IEnumerator RevealTurnRoutine()  => RevealBoardRoutine(1);
        public IEnumerator RevealRiverRoutine() => RevealBoardRoutine(1);

        public List<PokerPlayerController> EvaluateWinners()
        {
            Debug.Log("[Eval] Community: " +
                      string.Join(", ", BoardCards.Select(c => $"{c.rank} {c.suit}")));

            var results = players.ToDictionary(
                p => p,
                p => HandEvaluator.EvaluateBestHand(p.Model.HoleCards.ToList(), BoardCards));

            var bestScore = results.Values.Aggregate((a, b) =>
                a.Score.CompareTo(b.Score) > 0 ? a : b).Score;

            var winners = results
                .Where(kv => kv.Value.Score.CompareTo(bestScore) == 0)
                .Select(kv => kv.Key)
                .ToList();

            foreach (var w in winners)
            {
                var hr = results[w];
                var combo = string.Join(", ",
                    hr.Combination.Select(c => $"{c.rank} {c.suit}"));
                Debug.Log($"🏆 P#{players.IndexOf(w)+1} wins: {hr.Score.Rank} [{combo}]");
            }
            return winners;
        }

        #endregion

        #region Helpers ------------------------------------------------------

        /// <summary>Удаляет ВСЕ визуальные карты с рук игроков и борда.</summary>
        private void ClearTable()
        {
            // 1) борд
            tableView.ResetBoard();

            // 2) руки игроков
            foreach (var p in players)
            {
                var view = p.GetComponent<PlayerView>();
                view?.ResetView();
            }
        }

        private IEnumerator RevealBoardRoutine(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var card = deck.DrawCard();
                BoardCards.Add(card);

                yield return tableView.ShowBoardCardAsync(card, factory);
                yield return wb;
            }
        }

        #endregion
    }
}
