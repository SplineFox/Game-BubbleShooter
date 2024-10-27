using System;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BubbleShooter
{
    public class SafeArea : MonoBehaviour, ILayoutElement
    {
        public enum Mode
        {
            Top,
            Bottom
        }

        [SerializeField] private Mode _mode;
        [SerializeField] private RectTransform _rootCanvasRectTransform;

        public float minWidth { get; }
        public float preferredWidth { get; }
        public float flexibleWidth { get; }
        public float preferredHeight { get; }
        public float flexibleHeight { get; }
        public int layoutPriority { get; }

        private float _minHeight;
        public float minHeight => _minHeight;
        
        private void OnTransformParentChanged()
        {
            _rootCanvasRectTransform = null;
        }

        public void CalculateLayoutInputHorizontal()
        {
            
        }

        public void CalculateLayoutInputVertical()
        {
            if (_rootCanvasRectTransform == null)
            {
#if UNITY_EDITOR
                if (EditorApplication.isPlaying)
#endif
                    Debug.LogError("Unexpected root game object type");
                
                _minHeight = 0;
                return;
            }

            switch (_mode)
            {
                case Mode.Top:
                {
                    var height = (Screen.height - Screen.safeArea.y - Screen.safeArea.height) / Screen.height * _rootCanvasRectTransform.sizeDelta.y;
                    _minHeight = height;
                    break;
                }
                case Mode.Bottom:
                {
                    var height = Screen.safeArea.y / Screen.height * _rootCanvasRectTransform.sizeDelta.y;
                    _minHeight = height;
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}