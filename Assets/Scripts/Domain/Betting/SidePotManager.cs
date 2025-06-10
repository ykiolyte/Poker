using System.Collections.Generic;
using System.Linq;

namespace Poker.Domain.Betting
{
    public static class SidePotManager
    {
        public static List<(int amount, List<int> contenders)> Build(Dictionary<int, int> contrib)
        {
            var list = contrib.Select(kv => (kv.Key, kv.Value)).ToList();
            var pots = new List<(int, List<int>)>();

            while (list.Count > 0)
            {
                list.Sort((a, b) => a.Value.CompareTo(b.Value));
                int minBet = list[0].Value;
                var contenders = new List<int>();

                for (int i = list.Count - 1; i >= 0; i--)
                {
                    contenders.Add(list[i].Key);
                    list[i] = (list[i].Key, list[i].Value - minBet);
                    if (list[i].Value == 0)
                        list.RemoveAt(i);
                }

                pots.Add((minBet * contenders.Count, contenders));
            }

            return pots;
        }
    }
}