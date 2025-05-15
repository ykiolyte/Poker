// Assets/Scripts/GameLoop/Extensions.cs
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Poker.GameLoop
{
    public static class Extensions
    {
        public static T PopFirst<T>(this List<T> list)
        {
            T item = list[0];
            list.RemoveAt(0);
            return item;
        }

        public static IEnumerator AsEnumerator(this Task task)
        {
            while (!task.IsCompleted) yield return null;
        }
    }
}
