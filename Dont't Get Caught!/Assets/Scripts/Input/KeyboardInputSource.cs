using DontGetCaught.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DontGetCaught.Input
{
    /// <summary>
    /// Keyboard fallback input for gameplay before physical hardware is ready.
    /// Hold Space to simulate airflow; F and C toggle prop return states.
    /// </summary>
    public sealed class KeyboardInputSource : MonoBehaviour, IGameInputSource
    {
        [Header("Airflow")]
        [SerializeField] private Key inhaleKey = Key.Space;
        [SerializeField, Range(0f, 1f)] private float airflowStrength = 0.75f;
        [SerializeField, Range(0.05f, 1f)] private float airflowAdjustmentPerSecond = 0.5f;

        [Header("Prop Return Simulation")]
        [SerializeField] private Key forkToggleKey = Key.F;
        [SerializeField] private Key cupToggleKey = Key.C;
        [SerializeField] private bool forkReturned = true;
        [SerializeField] private bool cupReturned = true;

        private Keyboard keyboard;
        private float currentAirflowStrength;

        public GameInputSample CurrentSample { get; private set; } = GameInputSample.Disconnected;
        public string SourceName => "Keyboard";

        private void Awake()
        {
            currentAirflowStrength = airflowStrength;
            keyboard = Keyboard.current;
        }

        private void Update()
        {
            keyboard = Keyboard.current;
            if (keyboard == null)
            {
                CurrentSample = GameInputSample.Disconnected;
                return;
            }

            AdjustAirflowStrength();
            TogglePropStates();

            float airflow = keyboard[inhaleKey].isPressed ? currentAirflowStrength : 0f;
            CurrentSample = new GameInputSample(airflow, forkReturned, cupReturned, true);
        }

        private void AdjustAirflowStrength()
        {
            float adjustment = 0f;
            if (keyboard.upArrowKey.isPressed || keyboard.equalsKey.isPressed)
            {
                adjustment += 1f;
            }

            if (keyboard.downArrowKey.isPressed || keyboard.minusKey.isPressed)
            {
                adjustment -= 1f;
            }

            currentAirflowStrength = Mathf.Clamp01(
                currentAirflowStrength + adjustment * airflowAdjustmentPerSecond * Time.deltaTime);
        }

        private void TogglePropStates()
        {
            if (keyboard[forkToggleKey].wasPressedThisFrame)
            {
                forkReturned = !forkReturned;
            }

            if (keyboard[cupToggleKey].wasPressedThisFrame)
            {
                cupReturned = !cupReturned;
            }
        }
    }
}
