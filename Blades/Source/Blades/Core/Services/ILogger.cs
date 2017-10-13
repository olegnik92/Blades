using System;

namespace Blades.Core.Services
{
    /// <summary>
    /// Сервис логирования
    /// </summary>
    public interface ILogger: IBladesService
    {
        void Debug(string message);

        void Info(string message);

        void Warning(string message);

        void Error(Exception error);

        void Error(Error error);
    }
}