using System;
using System.ComponentModel;

namespace Blades.Auth
{
    /// <summary>
    /// Возможные типы разрешений в системе
    /// </summary>
    [Flags]
    public enum PermissionType
    {
        [Description("Создание")]
        Create = 1,

        [Description("Чтение")]
        Read = 2,

        [Description("Изменение")]
        Update = 4,

        [Description("Удаление")]
        Delete = 8,

        [Description("Уведомление")]
        Notify = 16,

        [Description("Выполнение")]
        Execute = 32
    }
}