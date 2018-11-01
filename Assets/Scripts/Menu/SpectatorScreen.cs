using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

using System;

[Serializable]
// Taken from https://forum.unity.com/threads/propertydrawer-for-datetime-class-not-getting-called.490129/#post-3196807
public class SerializedDateTime : ISerializationCallbackReceiver
{
    public DateTime dateTime;

    [SerializeField, Header("Leave blank to turn off")]
    private string _dateTime;

    public static implicit operator DateTime(SerializedDateTime udt)
    {
        return (udt.dateTime);
    }

    public static implicit operator SerializedDateTime(DateTime dt)
    {
        return new SerializedDateTime() { dateTime = dt };
    }

    public void OnAfterDeserialize()
    {
        DateTime.TryParse(_dateTime, out dateTime);
    }

    public void OnBeforeSerialize()
    {
        _dateTime = dateTime.ToString();
    }
}

public class SpectatorScreen : MonoBehaviour
{
    public GameObject infoPanel;
    public GameObject loadingDisplay;

    [Tooltip("Time to fade in or out various components.")]
    public float fadeDuration = 0.5f;
    
    [Tooltip("Time to discourage more participants from joining in. The VR headset will still function normally.")]
    public SerializedDateTime demoEndTime;

    protected DateTime realDemoEndTime;

    protected Text infoCounter;
    protected Text infoText;

    protected UnityEngine.Video.VideoPlayer logoVideo;

    /// <summary>
    /// Used for demo end time, to avoid replaying the video
    /// </summary>
    protected bool hasShownVideo = false;

    protected Dictionary<object, float> targetAlpha = new Dictionary<object, float>();

    /// <summary>
    /// Gets the currently known target alpha for a given object.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="missingDefault">Target alpha to assume if none is known.</param>
    /// <returns></returns>
    protected float GetTargetAlpha(GameObject obj, float missingDefault = 0f)
    {
        if (!targetAlpha.ContainsKey(obj))
            return missingDefault;

        float alpha;
        targetAlpha.TryGetValue(obj, out alpha);

        return alpha;
    }

    /// <summary>
    /// Sets alpha on all components on this gameobject or any of its children.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="alpha">Alpha value, between 0 and 1</param>
    protected void SetAlpha(GameObject obj, float alpha, float fadeDuration = -1f)
    {
        targetAlpha.Remove(obj);
        targetAlpha.Add(obj, alpha);

        Graphic[] graphics = obj.GetComponentsInChildren<Graphic>();

        if (fadeDuration < 0)
            fadeDuration = this.fadeDuration;

        foreach (Graphic g in graphics)
            g.CrossFadeAlpha(alpha, fadeDuration, true);
    }

    /// <summary>
    /// Trigger the loading screen for spectators.
    /// </summary>
    public void TriggerLoading(float fadeDuration = -1f)
    {
        if (fadeDuration < 0)
            fadeDuration = this.fadeDuration;

        logoVideo.Play();

        SetAlpha(loadingDisplay, 1f);
    }

	// Use this for initialization
	void Start()
    {
        loadingDisplay.transform.Find("Video Player").gameObject.SetActive(true);

        logoVideo = loadingDisplay.GetComponentInChildren<UnityEngine.Video.VideoPlayer>();

        logoVideo.Prepare();
        
        // Keep playback speed as it is
        //logoVideo.playbackSpeed = Mathf.Min(((logoVideo.frameCount - (ulong)logoVideo.frame) / logoVideo.frameRate) / 2f, 10f);

        realDemoEndTime = demoEndTime.dateTime;

        // Unspecified end time, no need to stop displaying
        if (demoEndTime.dateTime == DateTime.MinValue)
        {
            realDemoEndTime = DateTime.MaxValue;
        }

        gameObject.SetActiveRecursively(true);

        infoCounter = infoPanel.transform.Find("Counter").GetComponent<Text>();
        infoText = infoPanel.transform.Find("Info").GetComponent<Text>();

        SetAlpha(infoPanel, 0f, 0);

        StartCoroutine(DelayedStart());
	}

    /// <summary>
    /// Activate certain features with a delay to compensate for the 0.5 second initial lag when loading a scene
    /// </summary>
    /// <returns></returns>
    IEnumerator DelayedStart()
    {
        // If the demo is over, then the screen shouldn't fade away
        if ((realDemoEndTime - DateTime.Now).TotalSeconds > 0)
        {
            yield return new WaitForSeconds(0.5f);

            // Fade the loading screen back so return from load is smooth
            SetAlpha(loadingDisplay, 0f);
        }
    }
	
	// Update is called once per frame
	void Update()
    {
        TimeSpan demoDuration = (realDemoEndTime - DateTime.Now);

        if (demoDuration.TotalMinutes < 10)
        {
            float infoAlpha = 1f;

            if (GetTargetAlpha(infoPanel, 1f) < 1f)
            {
                SetAlpha(infoPanel, 1f);
            }

            if (demoDuration.TotalSeconds < 0)
            {
                infoText.text = "SORRY, WE'RE CLOSED";

                if (GetTargetAlpha(infoCounter.gameObject, 1f) > 0f)
                {
                    SetAlpha(infoCounter.gameObject, 0f, 0f);

                    if (!hasShownVideo)
                    {
                        hasShownVideo = true;

                        logoVideo.playbackSpeed = 1f;
                        logoVideo.Play();
                    }

                    SetAlpha(loadingDisplay, 1f, 2f);
                }
            }
            else
            {
                infoText.text = "Demo time remaining";

                infoCounter.text = demoDuration.ToString("mm\\:ss");
            }
        }
	}
}
