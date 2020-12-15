using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Goat.AI
{
    public interface IState
    {
        void Tick();

        void OnEnter();

        void OnExit();
    }
}

