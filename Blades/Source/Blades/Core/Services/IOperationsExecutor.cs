namespace Blades.Core.Services
{
    /// <summary>
    /// Сервис для полного корректного выполнения операций 
    /// (с проверкоми данных и пользователя, логированием и записью в историю операций)
    /// </summary>
    public interface IOperationsExecutor: IBladesService
    {

        /// <summary>
        /// Запускает полное выполнение операции
        /// </summary>
        /// <param name="operation">Экземпляр выполняемой операции</param>
        /// <param name="parentOperation">Операция, в рамках которой выполняется данная запускаемая операция</param>
        /// <typeparam name="TData">Тип входных данных операции</typeparam>
        /// <typeparam name="TResult">Тип возвращаемого результата</typeparam>
        /// <returns>
        /// Возвращает кортеж: 
        /// Result - результат операции (полезные данные)
        /// Report - отчет о выполнении операции (для истории)
        /// </returns>
        (TResult Result, ExecutionReport Report) Execute<TData, TResult>(Operation<TData, TResult> operation, Operation parentOperation = null);

        /// <summary>
        /// Запускает полное выполнение операции
        /// </summary>
        /// <param name="operation">Экземпляр выполняемой операции</param>
        /// <param name="parentOperation">Операция, в рамках которой выполняется данная запускаемая операция</param>
        /// <returns>
        /// Возвращает кортеж: 
        /// Result - результат операции (полезные данные)
        /// Report - отчет о выполнении операции (для истории)
        /// </returns>
        (object Result, ExecutionReport Report) Execute(Operation operation, Operation parentOperation = null);
    }
}