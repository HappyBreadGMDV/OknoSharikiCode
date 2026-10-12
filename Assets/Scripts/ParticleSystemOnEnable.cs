using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemOnEnable : MonoBehaviour
{
    public ParticleSystem[] particleSystems;

    private void OnEnable()
    {
        if (particleSystems == null || particleSystems.Length == 0)
            return;

        foreach (ParticleSystem ps in particleSystems)
        {
            if (ps == null) continue;
            // Optional: clear leftover particles for a clean restart
            // ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            ps.Play();
        }
    }

    // Optional: stop systems when this object is disabled
    // private void OnDisable()
    // {
    //     if (particleSystems == null) return;
    //     foreach (var ps in particleSystems)
    //     {
    //         if (ps != null && ps.isPlaying)
    //             ps.Stop();
    //     }
    // }
}