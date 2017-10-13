namespace Blades.Core.Services
{
    /// <summary>
    /// ServiceLocator для получения реализаций IBladesService 
    /// </summary>
    public interface IBladesServiceLocator: IBladesService
    {
        T GetInstance<T>() where T : IBladesService;
    }
}