using EZCameraShake;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CameraShakeSettings {

    public float Magnitude;
    public float Roughness;
    public float FadeIn;
    public float FadeOut;

    public Vector3 PosInfluence;
    public Vector3 RotInfluence;

    public void Shake()
    {
        CameraShaker.Instance.ShakeOnce(
            Magnitude, Roughness, FadeIn, FadeOut, PosInfluence, RotInfluence);
    }
}
