using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleHandler : MonoBehaviour
{
    public float lifetime;

    public void Awake()
    {
        Destroy(gameObject, lifetime);
    }
}
