using DontGetCaught.Noodle;
using UnityEngine;
using UnityEngine.UI;

namespace DontGetCaught.UI
{
    /// <summary>
    /// Displays noodle progress with a standard Unity UI Slider.
    /// </summary>
    public sealed class NoodleProgressView : MonoBehaviour
    {
        [SerializeField] private NoodleEatingController noodleController;
        [SerializeField] private Slider progressSlider;

        private void OnEnable()
        {
            if (noodleController != null)
            {
                noodleController.ProgressChanged += HandleProgressChanged;
                HandleProgressChanged(noodleController.Progress);
            }
        }

        private void OnDisable()
        {
            if (noodleController != null)
            {
                noodleController.ProgressChanged -= HandleProgressChanged;
            }
        }

        private void HandleProgressChanged(float progress)
        {
            if (progressSlider != null)
            {
                progressSlider.value = progress;
            }
        }
    }
}
