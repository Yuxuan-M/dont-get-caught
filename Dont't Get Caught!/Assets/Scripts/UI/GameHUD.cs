using System;
using DontGetCaught.Core;
using DontGetCaught.Scoring;
using TMPro;
using UnityEngine;

namespace DontGetCaught.UI
{
    /// <summary>
    /// Minimal text HUD for round time, lives, score, and current state.
    /// </summary>
    public sealed class GameHUD : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;
        [SerializeField] private ScoreManager scoreManager;
        [SerializeField] private TMP_Text timerText;
        [SerializeField] private TMP_Text livesText;
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private TMP_Text statusText;

        private int lastDisplayedSecond = -1;
        private string lastCaughtReason;

        private void OnEnable()
        {
            if (gameManager != null)
            {
                gameManager.TimerChanged += HandleTimerChanged;
                gameManager.LivesChanged += HandleLivesChanged;
                gameManager.StateChanged += HandleStateChanged;
                gameManager.PlayerCaught += HandlePlayerCaught;
                HandleTimerChanged(gameManager.RemainingSeconds);
                HandleLivesChanged(gameManager.RemainingLives);
                HandleStateChanged(GameState.Ready, gameManager.CurrentState);
            }

            if (scoreManager != null)
            {
                scoreManager.ScoreChanged += HandleScoreChanged;
                HandleScoreChanged(scoreManager.TotalScore);
            }
        }

        private void OnDisable()
        {
            if (gameManager != null)
            {
                gameManager.TimerChanged -= HandleTimerChanged;
                gameManager.LivesChanged -= HandleLivesChanged;
                gameManager.StateChanged -= HandleStateChanged;
                gameManager.PlayerCaught -= HandlePlayerCaught;
            }

            if (scoreManager != null)
            {
                scoreManager.ScoreChanged -= HandleScoreChanged;
            }
        }

        private void HandleTimerChanged(float secondsRemaining)
        {
            int totalSeconds = Mathf.CeilToInt(secondsRemaining);
            if (totalSeconds == lastDisplayedSecond)
            {
                return;
            }

            lastDisplayedSecond = totalSeconds;
            if (timerText != null)
            {
                timerText.text = TimeSpan.FromSeconds(totalSeconds).ToString(@"mm\:ss");
            }
        }

        private void HandleLivesChanged(int remainingLives)
        {
            if (livesText != null)
            {
                livesText.text = $"Lives: {remainingLives}";
            }
        }

        private void HandleScoreChanged(int totalScore)
        {
            if (scoreText != null)
            {
                scoreText.text = $"Score: {totalScore}";
            }
        }

        private void HandleStateChanged(GameState previous, GameState current)
        {
            if (statusText != null)
            {
                statusText.text = current switch
                {
                    GameState.Ready => "Ready",
                    GameState.Playing => "Playing",
                    GameState.TeacherWarning => "Teacher warning",
                    GameState.TeacherChecking => "Teacher checking",
                    GameState.CaughtRecovery => string.IsNullOrEmpty(lastCaughtReason)
                        ? "Put the props away"
                        : $"Caught: {lastCaughtReason} - put the props away",
                    GameState.Won => "Round complete",
                    GameState.Lost => "Game over",
                    _ => current.ToString()
                };
            }
        }

        private void HandlePlayerCaught(string reason, int remainingLives)
        {
            lastCaughtReason = reason;
            if (statusText != null)
            {
                statusText.text = $"Caught: {reason}";
            }
        }
    }
}


