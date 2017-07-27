using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blades.Commands.Interfaces;
using Blades.Core;
using System.Collections.Concurrent;
using Blades.Core.System;
using System.Reflection;

namespace Blades.Commands.Basis
{
    public class CommandEmiter : ICommandEmitter
    {
        private Dictionary<string, List<ICommandReceiver>> receiversMap;
        private ICommandsHistory history;
        public CommandEmiter(ICommandReceiverActivator activator, ICommandsHistory history)
        {
            this.history = history;
            var allTypes = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => a.GetTypes())
                    .Where(t => !t.GetTypeInfo().IsAbstract).ToList();

            var allReceivers = allTypes.Where(t => typeof(ICommandReceiver).IsAssignableFrom(t))
                    .Select(t => activator.Create(t))
                    .ToList();

            var allCommandTypes = allTypes.Where(t => typeof(Command).IsAssignableFrom(t));

            receiversMap = new Dictionary<string, List<ICommandReceiver>>();
            foreach (var commandType in allCommandTypes)
            {
                var commandReceiverType = typeof(ICommandReceiver<>).MakeGenericType(commandType);
                var receivers = allReceivers.Where(r => commandReceiverType.IsAssignableFrom(r.GetType())).ToList();
                receiversMap[commandType.FullName] = receivers;
            }
        }

        public Task Emit(Command command)
        {
            if (IsNotInit(command))
            {
                throw new ArgumentException("Command is not init");
            }

            return Task.Run(() =>
            {
                var receivers = receiversMap[command.GetType().FullName];
                var reports = new ConcurrentBag<OperationExecutionReport>();
                Parallel.For(0, receivers.Count, (i =>
                {
                    var report = ExecuteCommand(command, receivers[i]);
                    reports.Add(report);
                }));

                command.ExecutionReports.AddRange(reports);
                history.Put(command);
            });
        }

        public void Execute(Command command)
        {
            if (IsNotInit(command))
            {
                throw new ArgumentException("Command is not init");
            }

            foreach (var receiver in receiversMap[command.GetType().FullName])
            {
                var report = ExecuteCommand(command, receiver);
                command.ExecutionReports.Add(report);
            }
            history.Put(command);
        }


        private bool IsNotInit(Command command)
        {
            return Guid.Empty.Equals(command.Id) || command.User == null;
        }

        private static OperationExecutionReport ExecuteCommand(Command command, ICommandReceiver receiver)
        {
            OperationExecutionReport report;
            try
            {
                report = (OperationExecutionReport)((dynamic)receiver).OnReceive((dynamic)command);
            }
            catch (Exception e)
            {
                report = new OperationExecutionReport("Ошибка при обработке команды");
                report.Errors.Add(new Error(e));
            }

            return report;
        }
    }
}
