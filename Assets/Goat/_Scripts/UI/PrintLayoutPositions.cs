using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class PrintLayoutPositions : MonoBehaviour
{
    [Button]
    private void PrintPositions()
    {
        LayoutGroup layoutGroup = transform.GetComponent<LayoutGroup>();
        for (int i = 0; i < transform.childCount; i++)
        {
            //layoutGroup.enabled = false;
            Transform child = transform.GetChild(i);
            print(child.position);
            //Canvas.ForceUpdateCanvases();
            //layoutGroup.enabled = true;
        }
    }

    [Button]
    private void PrintTransformPos(Transform trans)
    {
        print(trans.position);
    }
}