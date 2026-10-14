using System;
using UnityEngine;

namespace DontGetCaught.Core
{
    /// <summary>
    /// Owns round state, timer, lives, and win/loss decisions.
    /// Other systems report events or request transitions; they never assign CurrentState.
    /// </summary>
    public sealed class GameManager : MonoBehaviour
    {
        [SerializeField] private GameConfig config = new GameConfig();
        // [TEST] no lobby for now
        [SerializeField] private bool startRoundOnPlay = true;

        public GameState CurrentState { get; private set; } = GameState.Ready;
        public float RemainingSeconds { get; private set; }
        public int RemainingLives { get; private set; }
        public GameConfig Config => config;
        public bool IsRoundActive => CurrentState == GameState.Playing
                                     || CurrentState == GameState.TeacherWarning
                                     || CurrentState == GameState.TeacherChecking
                                     || CurrentState == GameState.CaughtRecovery;

        public event Action<GameState, GameState> StateChanged;
        public event Action<float> TimerChanged;
        public event Action<int> LivesChanged;
        public event Action<string, int> PlayerCaught;
        public event Action<GameState> RoundEnded;

        private void Awake()
        {
            RemainingSeconds = Mathf.Max(1f, config.roundDurationSeconds);
            RemainingLives = Mathf.Max(1, config.startingLives);
        }

        private void Start()
        {
            if (startRoundOnPlay)
            {
                StartRound();
            }
        }

        private void Update()
        {
            if (CurrentState != GameState.Playing
                && CurrentState != GameState.TeacherWarning
                && CurrentState != GameState.TeacherChecking)
            {
                return;
            }

            RemainingSeconds = Mathf.Max(0f, RemainingSeconds - Time.deltaTime);
            TimerChanged?.Invoke(RemainingSeconds);

            if (RemainingSeconds <= 0f)
            {
                FinishRound(GameState.Won);
            }
        }

        /// <summary>Starts or restarts a round and resets its timer and lives.</summary>
        public void StartRound()
        {
            if (CurrentState == GameState.Playing)
            {
                return;
            }

            RemainingSeconds = Mathf.Max(1f, config.roundDurationSeconds);
            RemainingLives = Mathf.Max(1, config.startingLives);
            TimerChanged?.Invoke(RemainingSeconds);
            LivesChanged?.Invoke(RemainingLives);
            ChangeState(GameState.Playing);
        }

        /// <summary>Requests a legal teacher-flow state change.</summary>
        public void SetTeacherPhase(GameState nextPhase)
        {
            if (nextPhase != GameState.TeacherWarning
                && nextPhase != GameState.TeacherChecking
                && nextPhase != GameState.Playing)
            {
                Debug.LogWarning($"Rejected teacher phase: {nextPhase}.", this);
                return;
            }

            if (CurrentState == GameState.Playing
                || CurrentState == GameState.TeacherWarning
                || CurrentState == GameState.TeacherChecking)
            {
                ChangeState(nextPhase);
            }
        }

        /// <summary>Reports one failed teacher check and enters recovery or loss.</summary>
        public void ReportCaught(string reason)
        {
            if (CurrentState != GameState.TeacherChecking)
            {
                Debug.LogWarning("A catch can only be reported while the teacher is checking.", this);
                return;
            }

            RemainingLives = Mathf.Max(0, RemainingLives - 1);
            LivesChanged?.Invoke(RemainingLives);
            PlayerCaught?.Invoke(reason, RemainingLives);

            if (RemainingLives <= 0)
            {
                FinishRound(GameState.Lost);
                return;
            }

            ChangeState(GameState.CaughtRecovery);
        }

        /// <summary>Resumes play after the player finishes arranging the props.</summary>
        public void CompleteRecovery()
        {
            if (CurrentState == GameState.CaughtRecovery)
            {
                ChangeState(GameState.Playing);
            }
        }

        /// <summary>Ends a round with a win or loss result.</summary>
        public void FinishRound(GameState result)
        {
            if (result != GameState.Won && result != GameState.Lost)
            {
                Debug.LogWarning($"Rejected round result: {result}.", this);
                return;
            }

            if (!IsRoundActive)
            {
                return;
            }

            ChangeState(result);
            RoundEnded?.Invoke(result);
        }

        private void ChangeState(GameState nextState)
        {
            if (CurrentState == nextState)
            {
                return;
            }

            GameState previousState = CurrentState;
            CurrentState = nextState;
            StateChanged?.Invoke(previousState, nextState);
        }
    }
}



