using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using UnityAtoms;

public static class Extensions
{
    public static GameObject SetActiveOnCompare(this GameObject obj, GameObject comparedObj)
    {
        obj.SetActive(obj == comparedObj);
        return obj;
    }

    public static bool Swap<T>(this T[] objectArray, int x, int y)
    {
        // check for out of range
        if (objectArray.Length <= y || objectArray.Length <= x) return false;

        // swap index x and y
        T buffer = objectArray[x];
        objectArray[x] = objectArray[y];
        objectArray[y] = buffer;

        return true;
    }

    public static bool NotNull(this Sequence seq)
    {
        return seq?.IsActive() ?? false;
    }

    public static Vector3 GetNearest(this List<Vector3> positions, Vector3 myPos)
    {
        if (positions.Count <= 0) return Vector3.zero;
        Vector3 nearestPos = positions[0];
        float nearestDist = 0;

        for (int i = 0; i < positions.Count; i++)
        {
            float currentDist = (myPos - positions[i]).sqrMagnitude;
            if (currentDist < nearestDist)
            {
                nearestDist = currentDist;
                nearestPos = positions[i];
            }
        }
        return nearestPos;
    }

    public static void Rotate<T>(this T[] array, int count)
    {
        if (array == null || array.Length < 2) return;
        count %= array.Length;
        if (count == 0) return;
        int left = count < 0 ? -count : array.Length + count;
        int right = count > 0 ? count : array.Length - count;
        if (left <= right)
        {
            for (int i = 0; i < left; i++)
            {
                var temp = array[0];
                Array.Copy(array, 1, array, 0, array.Length - 1);
                array[array.Length - 1] = temp;
            }
        }
        else
        {
            for (int i = 0; i < right; i++)
            {
                var temp = array[array.Length - 1];
                Array.Copy(array, 0, array, 1, array.Length - 1);
                array[0] = temp;
            }
        }
    }

    public static void Rotate<T>(this List<T> list, int count)
    {
        for (int i = 0; i < count; i++)
        {
            T first = list[0];
            list.RemoveAt(0);
            list.Add(first);
        }
    }

    public static void SetActiveRenderer(this GameObject obj, bool value)
    {
        Renderer rend = obj.GetComponentInChildren<Renderer>();
        rend.enabled = value;
    }

    public static void RegisterSafe<T>(this AtomEvent<T> evt, Action<T> action)
    {
        if (evt != null)
            evt.Register(action);
    }

    public static void RegisterSafe<T>(this AtomEvent<T> evt, IAtomListener<T> listener)
    {
        if (evt != null)
            evt.RegisterListener(listener);
    }

    public static void UnregisterSafe<T>(this AtomEvent<T> evt, Action<T> action)
    {
        if (evt != null)
            evt.Unregister(action);
    }

    public static void UnregisterSafe<T>(this AtomEvent<T> evt, IAtomListener<T> listener)
    {
        if (evt != null)
            evt.UnregisterListener(listener);
    }

    public static void UnregisterAllSafe<T>(this AtomEvent<T> evt)
    {
        if (evt != null)
            evt.UnregisterAll();
    }
}