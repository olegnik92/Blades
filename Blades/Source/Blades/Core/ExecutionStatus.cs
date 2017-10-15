namespace Blades.Core
{
    public enum ExecutionStatus
    {
        /// <summary>
        /// В процессе обработки действия возникли системные ошибки
        /// </summary>
        InfrastructureErrors,
        
        /// <summary>
        /// Ошибка при аутентификации пользователя
        /// </summary>
        AuthenticationErrors,
        
        /// <summary>
        /// У пользователя не хватает прав 
        /// </summary>
        PermissionsValidationErrors,
        
        /// <summary>
        /// Переданы некорректные данные
        /// </summary>
        DataValidationErrors,
        
        /// <summary>
        /// Выполнение действия прервано необработанным исключением
        /// </summary>
        ExecutionCrushErrors,
        
        /// <summary>
        /// В ходе выполнения действия были обнаружены ошибки
        /// </summary>
        ExecutionProcessedErrors,
        
        /// <summary>
        /// Выполнение завершено успешно
        /// </summary>
        ExecutionWithoutErrors
    }
}