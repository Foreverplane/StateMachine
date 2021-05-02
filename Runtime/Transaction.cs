using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Foreverplane.StateMachine {
    public abstract class Transaction {
        public abstract State FromState { get; }
        public abstract State ToState { get; }
        public abstract bool CheckConditionByTriggerName(string triggerName, StateMachine stateMachine);
        public abstract void Invoke();
    }
    
      public class Transaction<TFrom, TTo> : Transaction where TFrom : State where TTo : State {

        public class TransactionTrigger {
            public string TriggerName { get; private set; }
            public Func<StateMachine, Transaction<TFrom, TTo>, bool> Condition { get; private set; }

            public TransactionTrigger(string triggerName, Func<StateMachine, Transaction<TFrom, TTo>, bool> condition) {
                this.TriggerName = triggerName;
                this.Condition = condition;
            }
        }

        private readonly TFrom _FromState;
        private readonly TTo _ToState;

        public Transaction(TFrom fromState, TTo toState) {
            _FromState = fromState;
            _ToState = toState;
        }



        public override State FromState => _FromState;

        public override State ToState => _ToState;
        public override bool CheckConditionByTriggerName(string triggerName, StateMachine stateMachine) {
            //var isDirty = false;
            //foreach (var transactionTrigger in _transactionTriggers)
            //{
            //    if (transactionTrigger.Condition.Invoke(stateMachine, this))
            //    {

            //    }
            //}
            var trigger = _transactionTriggers.FirstOrDefault(_ => _.TriggerName == triggerName);
            if (trigger == null) {
                Debug.Log($"<color=red>Transaction does not have any condition for trigger: <b>{triggerName}</b></color>");
            }

            return trigger != null && trigger.Condition.Invoke(stateMachine, this);
        }

        public override void Invoke() {
            Debug.Log($"Exit from state: <b>{_FromState.GetType().Name}</b>");
            _FromState.OnExit();
            Debug.Log($"Enter to state: <b>{_ToState.GetType().Name}</b>");
            _ToState.OnEnter();
        }

        private readonly HashSet<TransactionTrigger> _transactionTriggers = new HashSet<TransactionTrigger>();


        public Func<StateMachine, Transaction<TFrom, TTo>, bool> DefaultTransactionCondition = (machine, transaction) => machine.CurrentState.GetType() == transaction.FromState.GetType();
        public void AddTrigger(string triggerName, Func<StateMachine, Transaction<TFrom, TTo>, bool> condition = null) {
            var transactionTrigger = new TransactionTrigger(triggerName, condition ?? DefaultTransactionCondition);
            _transactionTriggers.Add(transactionTrigger);

        }
    }
}
