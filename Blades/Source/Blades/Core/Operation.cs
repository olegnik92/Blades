using System;
using System.Collections.Generic;

namespace Blades.Core
{
    /// <summary>
    /// Базовый тип для опрераций - едениц функциональности в системе.
    /// Используется в системных целях.
    /// Наследоваться следует от обопщенного типа Operation(TData, TResult)
    /// </summary>
    public abstract class Operation
    {
        /// <summary>
        /// Идентификатор экземпляра оперции
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// Идентификатор экземпляра оперции, из которой вызвана текущая операция
        /// </summary>
        public Guid ParentOpearionId { get; set; }

        /// <summary>
        /// Пользователь, от имени которого выполняется операция
        /// </summary>
        public virtual UserInfo User { get; set; }
        
        /// <summary>
        /// Валидация входных данных
        /// </summary>
        /// <returns>Список ошибок валидации</returns>
        public virtual List<Error> GetDataValidationErrors()
        {
            return null;
        }

        /// <summary>
        /// Проверка прав пользователя на выполнение операции
        /// </summary>
        /// <returns>Список ошибок прав доступа</returns>
        public virtual List<Error> GetPermissionsValidationErrors()
        {
            return null;
        }

        /// <summary>
        /// Флаг - следует ли сохранять информацию о выполнении данной операции в истории.
        /// </summary>
        public virtual bool SaveInHistory => false;
    }
    
    /// <summary>
    /// Базовый тип для опрераций - едениц функциональности в системе.
    /// </summary>
    /// <typeparam name="TData">Тип входных данных операции</typeparam>
    /// <typeparam name="TResult">Тип возвращаемого результата</typeparam>
    public abstract class Operation<TData, TResult> : Operation
    {
        /// <summary>
        /// Входные данные операции
        /// </summary>
        public virtual TData Data { get; set; }

        /// <summary>
        /// Полезная работа, осуществляемая операцией (действие)
        /// </summary>
        /// <returns>
        /// Возвращает кортеж: 
        /// Result - результат операции (полезные данные)
        /// Report - отчет о выполнении операции (для истории)
        /// </returns>
        public abstract (TResult Result, ExecutionReport Report) Execute();
    }
}