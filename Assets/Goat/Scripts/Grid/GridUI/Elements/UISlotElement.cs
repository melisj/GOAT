using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Goat.Grid.Interactions.UI
{
    public class UISlotElement : MonoBehaviour
    {
        public virtual void InitUI() { }

        public virtual void SetUI(object[] args) { }
    }
}