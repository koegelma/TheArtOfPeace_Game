using UnityEngine;
using RaymarcherPackage;

/// <summary>
/// Simple music reaction logic for bass-line [mostly]
/// </summary>
public class SimpleMusicReaction : MonoBehaviour
{ 
    [Space]
    public AudioSource targetAudio;
    public int sampleDataLength = 1024;
    [Space]
    public float transitionSmooth = 8.0f;
    public float reactionAmplify = 1.0f;
    public Vector2 minMax = new Vector2(0.8f, 0.98f);
    public RM_Object targetRaymarcherObj;

    private float clipAmplify;
    private float[] clipSampleData;

    private void Awake()
    {
        clipSampleData = new float[sampleDataLength];
    }

    private void Update()
    {
        targetAudio.clip.GetData(clipSampleData, targetAudio.timeSamples);
        clipAmplify = 0.0f;
        foreach (var samp in clipSampleData)
            clipAmplify += Mathf.Abs(samp);
        clipAmplify /= sampleDataLength;

        targetRaymarcherObj.rmParamA = Mathf.Lerp(targetRaymarcherObj.rmParamA, (clipAmplify * reactionAmplify), transitionSmooth * Time.deltaTime);
        targetRaymarcherObj.rmParamA = Mathf.Clamp(targetRaymarcherObj.rmParamA, minMax.x, minMax.y);
    }
}