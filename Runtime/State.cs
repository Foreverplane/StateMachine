using System.Collections.Generic;
namespace Foreverplane.StateMachine {
    public abstract class State {

        private readonly HashSet<Transaction> _transactions = new HashSet<Transaction>();
        
        public virtual void OnExit() {
        }

        public virtual void OnEnter() {
        }

        internal void AddTransaction(Transaction transaction) {
            _transactions.Add(transaction);
        }

        public IEnumerable<Transaction> Transactions => _transactions;
    }
}
