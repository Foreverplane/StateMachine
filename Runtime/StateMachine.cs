using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Foreverplane.StateMachine {
    public class StateMachine {
        public bool InState<T>(out T state) where T : class {
            var s = CurrentState as T;
            state = s;
            return s != null;
        }
        public State CurrentState { get; private set; }
        public State PreviousState { get; private set; }

        private readonly List<Transaction> _transactions = new List<Transaction>();

        private readonly HashSet<State> _states = new HashSet<State>();

        public Transaction<TFrom, TTo> AddTransaction<TFrom, TTo>(TFrom fromState, TTo toState) where TFrom : State where TTo : State {
            var transaction = new Transaction<TFrom, TTo>(fromState, toState);
            _transactions.Add(transaction);
            _states.Add(fromState);
            _states.Add(toState);
            fromState.AddTransaction(transaction);
            return transaction;
        }

        public void InvokeTransaction<TFrom, TTo>() where TFrom : State where TTo : State {
            for (var i = 0; i < _transactions.Count; i++) {
                var t = _transactions[i];
                if (t is Transaction<TFrom, TTo> transaction) {
                    transaction.Invoke();
                    CurrentState = transaction.ToState;
                    PreviousState = transaction.FromState;
                }
            }
        }

        public void SetInitialState<TState>() where TState : State {
            CurrentState = _states.OfType<TState>().First();
            CurrentState.OnEnter();
        }

        public void InvokeTrigger(string triggerName) {
            if (CurrentState == null) {
                Debug.Log($"{GetType().Name} ignore trigger {triggerName} because of CurrentState is null");
                return;
            }
            var stateTransactions = CurrentState.Transactions.ToArray();
            Debug.Log($"Found: <b>{stateTransactions.Count()}</b> state transactions");
            foreach (var transaction in stateTransactions) {
                Debug.Log($"<b>{GetType().Name}</b> Check condition for transaction <b>{transaction.FromState.GetType().Name}->{transaction.ToState.GetType().Name}</b>");
                if (transaction.CheckConditionByTriggerName(triggerName, this)) {
                    Debug.Log($"<b><color=green>SUCCESS!</color></b>");
                    transaction.Invoke();
                    CurrentState = transaction.ToState;
                    PreviousState = transaction.FromState;
                    break;
                }

            }
        }
    }

}