using Blades.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blades.Auth.Interfaces;
using Blades.Core;
using Blades.DataStore.Interfaces;

namespace BladesStartUp.Operations
{
    public abstract class TransactOperation<TData, TResult, TRepo> : PermissionedOperation<TData, TResult> where TRepo : ITransactRepository
    {
        private ITransactRepositoryFactory repoFactory;
        public TransactOperation(IPermissionRequirementChecker checker, ITransactRepositoryFactory repoFactory) : base(checker)
        {
            this.repoFactory = repoFactory;
        }

        public override TResult Execute(out OperationExecutionReport executionReport)
        {
            var repo = repoFactory.GetRepository<TRepo>();
            var result = Transaction(repo, out executionReport);
            repo.Commit();
            return result;
        }

        public abstract TResult Transaction(TRepo repo, out OperationExecutionReport executionReport);
    }
}
