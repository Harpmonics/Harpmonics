using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatJudge : MonoBehaviour {

    public GameObject prefabHitEffect;
    public GameObject prefabMissEffect;

    public Transform parentHitEffect;
    public Transform parentMissEffect;

    public AudioSource hitSound;

    public float forgiveRange = 0.25f;

    HashSet<int> judgedBeat = new HashSet<int>();

    public bool Judge(bool withEffect = true)
    {
        int currentBeat = Mathf.RoundToInt(BeatTime.beat);
        return Judge(currentBeat, withEffect);
    }

    public bool Judge(int beat, bool withEffect = true)
    {
        if (beat < 0 || judgedBeat.Contains(beat) || Mathf.Abs(BeatTime.beat - beat) > forgiveRange)
        {
            if (withEffect) TriggerEffect(false);
            return false;
        }
        judgedBeat.Add(beat);
        if (withEffect) TriggerEffect(true);
        return true;
    }

    void TriggerEffect(bool hitOrMiss)
    {
        if (hitOrMiss) // hitted
        {
            if (hitSound != null)
                hitSound.Play();
            if (prefabHitEffect != null)
                Instantiate(prefabHitEffect, parentHitEffect);
        }
        else // missed
        {
            if (prefabMissEffect != null)
                Instantiate(prefabMissEffect, parentMissEffect);
        }
    }

    void Start()
    {
        if (parentHitEffect == null) parentHitEffect = transform;
        if (parentMissEffect == null) parentMissEffect = transform;
    }

}
