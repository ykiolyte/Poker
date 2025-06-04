// Assets/Scripts/Gameplay/PlayerHighlightView.cs
using UnityEngine;

namespace Poker.Gameplay
{
    [RequireComponent(typeof(Renderer))]
    public sealed class PlayerHighlightView : MonoBehaviour
    {
        [SerializeField] private Color highlightColor = Color.yellow;
        [SerializeField] private Color normalColor    = Color.white;

        Renderer _r;

        void Awake() => _r = GetComponent<Renderer>();

        public void SetHighlight(bool on) =>
            _r.material.color = on ? highlightColor : normalColor;
    }
}
