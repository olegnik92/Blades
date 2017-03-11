using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blades.Auth
{
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
