using UnityEngine;

namespace Poker.Gameplay.Cards
{
    public static class CardFanUtility
    {
        /// <summary>
        /// Раскладывает карты веером по дуге, как ножницы.
        /// </summary>
        /// <param name="cardTransform">Transform карты</param>
        /// <param name="index">Порядковый номер карты</param>
        /// <param name="total">Общее число карт в руке</param>
        /// <param name="radius">Радиус дуги</param>
        /// <param name="angleStep">Градусов между картами</param>
        public static void ApplyFanArcOffset(Transform cardTransform, int index, int total, float radius = 0.08f, float angleStep = 12f)
        {
            float mid = (total - 1) / 2f;
            float angle = (index - mid) * angleStep;

            // Смещение по дуге
            float rad = Mathf.Deg2Rad * angle;
            Vector3 offset = new Vector3(Mathf.Sin(rad), 0f, -Mathf.Cos(rad)) * radius;

            cardTransform.localPosition = offset;
            cardTransform.localRotation = Quaternion.Euler(0, angle, 0);
        }
    }
}
