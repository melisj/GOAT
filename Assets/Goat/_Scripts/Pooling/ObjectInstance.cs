using UnityEngine;

namespace Goat.Pooling
{
    [System.Serializable]
    public class ObjectInstance
    {
        [SerializeField] private GameObject gameObject;
        [SerializeField] private Transform transform;
        [SerializeField] private bool hasPoolObjectComponent;
        [SerializeField] private IPoolObject poolObjectScript;

        public ObjectInstance(GameObject objectInstance)
        {
            gameObject = objectInstance;
            gameObject.name = objectInstance.name + "(" + gameObject.GetInstanceID() + ")";
            transform = gameObject.transform;
            gameObject.SetActive(false);
            poolObjectScript = gameObject.GetComponent<IPoolObject>();
            if (poolObjectScript != null)
            {
                hasPoolObjectComponent = true;
            }
        }

        public GameObject GameObject { get => gameObject; private set => gameObject = value; }

        public void GetObject(Vector3 pos, Quaternion rot, int poolKey)
        {
            if (!gameObject.activeInHierarchy)
            {
                gameObject.SetActive(true);
            }
            transform.position = pos;
            transform.rotation = rot;

            if (hasPoolObjectComponent)
            {
                poolObjectScript.OnGetObject(this, poolKey);
            }
        }

        public void SetParent(Transform parent)
        {
            transform.SetParent(parent);
        }
    }
}