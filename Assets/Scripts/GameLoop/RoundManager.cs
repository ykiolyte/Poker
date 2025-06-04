using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;                 //  ←  для AssetReference
using Poker.Gameplay.Cards;
using Poker.Gameplay.Factories;

namespace Poker.GameLoop
{
    /// <summary>
    /// Управляет одной раздачей: тасовка, раздача, борд, подсчёт победителя.
    /// Стадиями рулит GameStateManager через публичные корутины.
    /// </summary>
    public sealed class RoundManager : MonoBehaviour
    {
        #region Inspector -----------------------------------------------------

        [Header("Scene refs")]
        
        private List<PokerPlayerController> players;

        [SerializeField] private TableView  tableView;
        [SerializeField] private CardPool   cardPool;
        [SerializeField] private AssetReference cardPrefab;   // ← ВЕРНУЛИ ПОЛЕ

        [Header("Timings, sec")]
        [SerializeField] private float dealDelay  = .3f;
        [SerializeField] private float boardDelay = .4f;

        #endregion

        private DeckManager deck;
        private CardFactory factory;
        private WaitForSeconds wd, wb;

        public readonly List<CardDataSO> BoardCards = new();   // community-карты

        #region Unity ---------------------------------------------------------

        private void Awake()
        {
            deck = new DeckManager();
            wd = new WaitForSeconds(dealDelay);
            wb = new WaitForSeconds(boardDelay);

            players = FindObjectsByType<PokerPlayerController>(FindObjectsSortMode.None).ToList();

            // Инициализация моделей для каждого игрока
            for (int i = 0; i < players.Count; i++)
            {
                var model = new PokerPlayerModel(id: i, stack: 1000);
                players[i].InjectModel(model);
            }

            if (cardPrefab == null || !cardPrefab.RuntimeKeyIsValid())
            {
                Debug.LogWarning("RoundManager: Card Prefab (Addressable) не задан.");
            }

            factory = new CardFactory(cardPool, cardPrefab);

            players = FindObjectsByType<PokerPlayerController>(FindObjectsSortMode.None).ToList();
            
            Debug.Log($"[Init] Found {players.Count} players");


        }



        #endregion

        #region Public API (корутины) ----------------------------------------

        public IEnumerator SetupNewHandRoutine()
        {
            var all = Resources.LoadAll<CardDataSO>("Configs");
            deck.InitializeDeck(all);
            deck.Shuffle();

            BoardCards.Clear();
            tableView.ResetBoard();
            foreach (var p in players) p.ResetForNewHand();

            Debug.Log($"[Round] New hand → deck of {all.Length} cards ready");
            yield break;
        }

        public IEnumerator DealHoleCardsRoutine()
        {
            for (int cardIndex = 0; cardIndex < 2; cardIndex++)
            {
                foreach (var player in players)
                {
                    var cardData = deck.DrawCard();
                    Debug.Log($"[Deal] P#{players.IndexOf(player)+1} gets {cardData.rank} {cardData.suit}");
                    var cardView = factory.Create(cardData);
                    cardView.Initialize(cardData);

                    var anchor = player.GetCardAnchor(isLeftHand: false);

                    cardView.transform.SetPositionAndRotation(anchor.position, anchor.rotation);
                    cardView.transform.SetParent(anchor, worldPositionStays: true);

                    // ВЕЕРНАЯ РАСКЛАДКА
                    int totalCardsInHand = 2; // если ты знаешь заранее
                    CardFanUtility.ApplyFanArcOffset(cardView.transform, cardIndex, totalCardsInHand);


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

        private IEnumerator RevealBoardRoutine(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var card = deck.DrawCard();
                BoardCards.Add(card);

                Debug.Log($"[Board] Slot{BoardCards.Count-1}: {card.rank} of {card.suit}");
                yield return tableView.ShowBoardCardAsync(card, factory);
                yield return wb;
            }
        }

        private static IEnumerator RunTask(Task task)
        {
            while (!task.IsCompleted) yield return null;
        }

        #endregion
    }
}
