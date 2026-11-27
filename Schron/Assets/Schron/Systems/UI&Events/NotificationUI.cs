using System.Collections;
using UnityEngine;
using TMPro;
using Schron.Core;

namespace Schron.UI
{
    public class NotificationUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private TextMeshProUGUI notificationText;

        [Header("Timing")]
        [SerializeField] private float visibleDuration = 2.0f;
        [SerializeField] private float fadeDuration = 0.5f;

        private Coroutine activeRoutine;

        private void Awake()
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
                canvasGroup.blocksRaycasts = false;
            }
        }

        private void OnEnable()
        {
            GameEvents.OnNotification += ShowNotification;
        }

        private void OnDisable()
        {
            GameEvents.OnNotification -= ShowNotification;
        }

        public void ShowNotification(string message)
        {
            if (notificationText == null || canvasGroup == null) return;

            if (activeRoutine != null)
                StopCoroutine(activeRoutine);

            activeRoutine = StartCoroutine(AnimateNotification(message));
        }

        // zajmuje się animacją powiadomienia. najpierw pokazuje się przez 2 sekundy a potem zanika
        private IEnumerator AnimateNotification(string message)
        {
            notificationText.text = message;
            canvasGroup.alpha = 1f;

            // 2 sekundy widać
            yield return new WaitForSeconds(visibleDuration);

            // zanikanie
            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
                yield return null;
            }

            canvasGroup.alpha = 0f;
            activeRoutine = null;
        }
    }
}