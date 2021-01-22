using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;

namespace Goat.AI
{
    [System.Serializable]
    public class StateMachine
    {
        /// <summary>
        /// Current state
        /// </summary>
        private IState currentState;
        /// <summary>
        /// Dictionary of all transitions
        /// </summary>
        [SerializeField] private Dictionary<Type, List<Transition>> transitions = new Dictionary<Type, List<Transition>>();
        /// <summary>
        /// List of transitions that can occur out of the current state
        /// </summary>
        [SerializeField, ReadOnly] private List<Transition> currentTransitions = new List<Transition>();
        /// <summary>
        /// List of transitions that can happen at any time
        /// </summary>
        [SerializeField, ReadOnly] private List<Transition> anyTransitions = new List<Transition>();

        private static List<Transition> EmptyTransitions = new List<Transition>(capacity: 0);

        public IState CurrentState => currentState;

        /// <summary>
        /// Happens every Update
        /// </summary>
        public void Tick()
        {
            var transition = GetTransition();
            if (transition != null)
                SetState(transition.To);

            currentState?.Tick();
        }

        /// <summary>
        /// Sets the new state that came from the transition
        /// </summary>
        /// <param name="state"> New state</param>
        public void SetState(IState state)
        {
            if (state == currentState)
                return;

            // Call OnExit for previous state
            currentState?.OnExit();
            // Set new state
            currentState = state;

            transitions.TryGetValue(currentState.GetType(), out currentTransitions);
            if (currentTransitions == null)
                currentTransitions = EmptyTransitions;

            // Call OnEnter for new state
            currentState.OnEnter();
        }

        /// <summary>
        /// Add a new transition to the StateMachine that the AI uses
        /// </summary>
        /// <param name="from"> Previous state </param>
        /// <param name="to"> Next state </param>
        /// <param name="predicate"> Condition that should initiate the transition </param>
        public void AddTransition(IState from, IState to, Func<bool> predicate)
        {
            if (transitions.TryGetValue(from.GetType(), out var _transitions) == false)
            {
                _transitions = new List<Transition>();
                transitions[from.GetType()] = _transitions;
            }

            _transitions.Add(new Transition(to, predicate));
        }

        /// <summary>
        /// Add a state which can be transitioned from our of any state
        /// </summary>
        /// <param name="state"> Next state </param>
        /// <param name="predicate"> Condition that should initiate the transition </param>
        public void AddAnyTransition(IState state, Func<bool> predicate)
        {
            anyTransitions.Add(new Transition(state, predicate));
        }

        /// <summary>
        /// Get a transition form the current state into the next state if the transition condition is true
        /// </summary>
        /// <returns> Returns the transitions which condition is true </returns>
        private Transition GetTransition()
        {
            for (int i = 0; i < anyTransitions.Count; i++)
                if (anyTransitions[i].Condition())
                    return anyTransitions[i];

            for (int i = 0; i < currentTransitions.Count; i++)
                if (currentTransitions[i].Condition())
                    return currentTransitions[i];

            return null;
        }

        /// <summary>
        /// Transition
        /// </summary>
        [System.Serializable]
        private class Transition
        {
            public Func<bool> Condition { get; }
            public IState To { get; }

            public Transition(IState to, Func<bool> condition)
            {
                To = to;
                Condition = condition;
            }
        }
    }
}