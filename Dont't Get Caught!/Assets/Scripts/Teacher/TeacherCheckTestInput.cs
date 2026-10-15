using DontGetCaught.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DontGetCaught.Teacher
{
    /// <summary>
    /// Temporary keyboard controls for exercising teacher-check transitions before TeacherAI exists.
    /// Disable or remove this component after the real teacher controller is connected.
    /// </summary>
    public sealed class TeacherCheckTestInput : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;
        [SerializeField] private Key beginCheckKey = Key.F9;
        [SerializeField] private Key resolveCheckKey = Key.F10;

        private void Update()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null || gameManager == null)
            {
                return;
            }

            if (keyboard[beginCheckKey].wasPressedThisFrame)
            {
                gameManager.BeginTeacherChecking();
            }

            if (keyboard[resolveCheckKey].wasPressedThisFrame)
            {
                gameManager.ResolveTeacherCheck();
            }
        }
    }
}
