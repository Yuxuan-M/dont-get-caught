using DontGetCaught.Noodle;
using UnityEngine;

namespace DontGetCaught.Scoring
{
    /// <summary>
    /// Tracks score for completed noodles only.
    /// </summary>
    public sealed class ScoreManager : MonoBehaviour
    {
        [SerializeField] private DontGetCaught.Core.GameManager gameManager;
        [SerializeField] private NoodleEatingController noodleController;

        public int TotalScore { get; private set; }
        public event System.Action<int> ScoreChanged;

        private void OnEnable()
        {
            if (gameManager != null)
            {
                gameManager.StateChanged += HandleStateChanged;
            }

            if (noodleController != null)
            {
                noodleController.NoodleCompleted += HandleNoodleCompleted;
            }
        }

        private void OnDisable()
        {
            if (gameManager != null)
            {
                gameManager.StateChanged -= HandleStateChanged;
            }

            if (noodleController != null)
            {
                noodleController.NoodleCompleted -= HandleNoodleCompleted;
            }
        }

        private void HandleStateChanged(DontGetCaught.Core.GameState previous, DontGetCaught.Core.GameState next)
        {
            if (next == DontGetCaught.Core.GameState.Playing
                && (previous == DontGetCaught.Core.GameState.Ready
                    || previous == DontGetCaught.Core.GameState.Won
                    || previous == DontGetCaught.Core.GameState.Lost))
            {
                TotalScore = 0;
                ScoreChanged?.Invoke(TotalScore);
            }
        }

        private void HandleNoodleCompleted()
        {
            if (gameManager == null)
            {
                Debug.LogError("ScoreManager needs a GameManager reference.", this);
                return;
            }

            TotalScore += gameManager.Config.pointsPerCompletedNoodle;
            ScoreChanged?.Invoke(TotalScore);
            Debug.Log($"Noodle completed. Score: {TotalScore}", this);
        }
    }
}


