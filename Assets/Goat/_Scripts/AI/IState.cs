using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is the interface/template of a state class
/// A state is controlled by the StateMachice
/// </summary>
namespace Goat.AI
{
    public interface IState
    {
        /// <summary>
        /// Happens every Update
        /// </summary>
        void Tick();

        /// <summary>
        /// Happens when the AI enters the state
        /// </summary>
        void OnEnter();

        /// <summary>
        /// Happens when the AI exits the state
        /// </summary>
        void OnExit();
    }
}

