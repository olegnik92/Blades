﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blades.Commands.Interfaces;
using Blades.Core;
using System.Collections.Concurrent;

namespace Blades.Commands.Basis
{
    public class CommandEmiter : ICommandEmitter
    {
        private Dictionary<string, List<ICommandReceiver>> receiversMap;
        public CommandEmiter(ICommandReceiverActivator activator)
        {
            var allTypes = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => a.GetTypes())
                    .Where(t => !t.IsAbstract).ToList();

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

        public Task Emit(Command command, UserInfo user)
        {
            command.Init(user);
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
            });
        }

        public void Execute(Command command, UserInfo user)
        {
            command.Init(user);
            foreach (var receiver in receiversMap[command.GetType().FullName])
            {
                var report = ExecuteCommand(command, receiver);
                command.ExecutionReports.Add(report);
            }
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