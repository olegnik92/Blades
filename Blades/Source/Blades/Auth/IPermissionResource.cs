using System;
using System.Collections.Generic;

namespace Blades.Auth
{
    /// <summary>
    /// Описпние ресурса, на который пользователи могут получать разрешение
    /// </summary>
    public interface IPermissionResource
    {
        /// <summary>
        /// Идентификатор ресурса
        /// </summary>
        Guid Id { get; }

        
        /// <summary>
        /// Наименование ресурса
        /// </summary>
        string Name { get; }


        /// <summary>
        /// Идентификатор родительского ресурса.
        /// Может используеться для формирования дерева ресурсов.
        /// </summary>
        Guid ParentResourceId { get; }

        
        /// <summary>
        /// Типы прав доступа актуальные для этого ресурса.
        /// Допустимо объединение флагов по "или".
        /// </summary>
        PermissionType ActualPermissions { get; }


        /// <summary>
        /// Метод позволяющий устанавливать связь между ресурсами
        /// Служит для автоматического наделения правами пользователя на другие ресурсы.
        /// </summary>
        /// <param name="userPermissions">Права пользователя на данный ресурс</param>
        /// <returns>Набор прав на другие (связанные) ресурсы которые получит пользователь</returns>
        Dictionary<Guid, PermissionType> GetAssociatedPermissions(PermissionType userPermissions);
    }
}