namespace DontGetCaught.Input
{
    /// <summary>
    /// Implement this interface for keyboard, serial, or mixed input sources.
    /// </summary>
    public interface IGameInputSource
    {
        GameInputSample CurrentSample { get; }
        string SourceName { get; }
    }
}
