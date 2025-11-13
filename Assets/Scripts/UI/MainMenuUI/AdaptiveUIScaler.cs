using UnityEngine;
using YG;

namespace EachOneMatters.UI.MainMenuUI
{
    public class AdaptiveUIScaler : MonoBehaviour
    {
        [Header("Настройки масштаба")]
        [Tooltip("Масштаб для мобильных устройств")]
        [SerializeField] private Vector3 _mobileScale = new Vector3(1.4f, 1.4f, 1);

        [Tooltip("Масштаб для десктопа")]
        [SerializeField] private Vector3 _desktopScale = new Vector3(1f, 1f, 1);

        [Header("Элементы для масштабирования")]
        [SerializeField] private RectTransform[] _uiElements;

        private void Start()
        {
            ApplyScale();
        }

        private void ApplyScale()
        {
            Vector3 targetScale = YG2.envir.isMobile ? _mobileScale : _desktopScale;

            foreach (RectTransform rect in _uiElements)
            {
                if (rect != null)
                {
                    rect.localScale = targetScale;
                }
            }
        }
    }
}