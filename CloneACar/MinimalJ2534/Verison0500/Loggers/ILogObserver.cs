namespace Minimal_J2534_0500
{
    /// <summary>
    /// based on Observer Design Pattern
    /// all classes that subscribe to log must implement this interface
    /// </summary>
    internal interface ILogObserver
    {
        void Update();
    }
}