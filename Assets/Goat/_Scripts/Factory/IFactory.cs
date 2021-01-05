using UnityEngine;

namespace Goat.Factory
{
    /// <summary>
    /// Represents a factory.
    /// </summary>
    /// <typeparam name="T">Specifies the type to create.</typeparam>
    public interface IFactory<T>
    {
        T Create(Vector3 pos, GameObject parent);
    }
}