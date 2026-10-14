namespace DontGetCaught.Input
{
    /// <summary>
    /// One normalized snapshot from any input device.
    /// Hardware adapters should map their raw readings into this shared format.
    /// </summary>
    public readonly struct GameInputSample
    {
        public float Airflow { get; }
        public bool ForkReturned { get; }
        public bool CupReturned { get; }
        public bool IsConnected { get; }

        public GameInputSample(float airflow, bool forkReturned, bool cupReturned, bool isConnected)
        {
            Airflow = Clamp01(airflow);
            ForkReturned = forkReturned;
            CupReturned = cupReturned;
            IsConnected = isConnected;
        }

        public static GameInputSample Disconnected => new GameInputSample(0f, false, false, false);

        private static float Clamp01(float value)
        {
            if (value < 0f) return 0f;
            return value > 1f ? 1f : value;
        }
    }
}
