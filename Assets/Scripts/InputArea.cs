using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BubbleShooter
{
    public class InputArea : MonoBehaviour, IPointerDownHandler
    {
        public event Action<Vector3> Clicked;

        public void OnPointerDown(PointerEventData eventData)
        {
            var pointerWorldPosition = Camera.main.ScreenToWorldPoint(eventData.position);
            pointerWorldPosition.z = 0;

            Clicked?.Invoke(pointerWorldPosition);
        }
    }
}