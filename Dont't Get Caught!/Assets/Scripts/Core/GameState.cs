namespace DontGetCaught.Core
{
    /// <summary>
    /// High-level phases of a single game round.
    /// GameManager is the only component that changes the current phase.
    /// </summary>
    public enum GameState
    {
        Ready,
        Playing,
        TeacherWarning,
        TeacherChecking,
        CaughtRecovery,
        Won,
        Lost
    }
}
