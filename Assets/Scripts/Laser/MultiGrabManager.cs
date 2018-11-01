using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is a quick solution for controlling the number of beams that can be grabbed
/// by a single collider.
/// </summary>
public class MultiGrabManager : MonoBehaviour
{
    /// <summary>
    /// Maximum number of beams that can be grabbed by each hand. Minimum value of 1
    /// </summary>
    public int m_limit = 1;

    private Dictionary<Collider, int> colliders = new Dictionary<Collider, int>();

    public bool CanGrab(Collider cd)
    {
        int value;

        if (colliders.TryGetValue(cd, out value))
        {
            return value < m_limit;
        }
        else
        {
            return true;
        }
    }

    public void RegisterGrab(Collider cd)
    {
        if (colliders.ContainsKey(cd))
        {
            colliders[cd] += 1;
        }
        else
        {
            colliders.Add(cd, 1);
        }
    }

    public void RegisterRelease(Collider cd)
    {
        if (colliders.ContainsKey(cd))
        {
            colliders[cd] -= 1;
        }
    }
}
