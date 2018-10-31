using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Records the motion of controllers after user presses to record and playbacks the motion again
/// </summary>
public class ControllerPlayback : MonoBehaviour
{
    [Tooltip("Controller model to use for all playback clones.")]
    public GameObject controllerTemplate;

    private struct PlaybackState
    {
        public Vector3 position;
        public Quaternion rotation;
        public float triggerActuation;

        public InputManager.InputState.ActiveFunction buttons;

        public PlaybackState(Vector3 position, Quaternion rotation, float triggerActuation, InputManager.InputState.ActiveFunction buttons = InputManager.InputState.ActiveFunction.None)
        {
            this.position = new Vector3(position.x, position.y, position.z);
            this.rotation = new Quaternion(rotation.x, rotation.y, rotation.z, rotation.w);
            this.triggerActuation = triggerActuation;

            this.buttons = buttons;
        }
    }

    private class RecordStruct
    {
        public GameObject controller;
        public LinkedList<PlaybackState> recording;

        public IEnumerator<PlaybackState> iterator = null;
    }

    private List<RecordStruct> recordingControllers = new List<RecordStruct>();
    private List<RecordStruct> playingControllers = new List<RecordStruct>();

    private Dictionary<int, LinkedList<PlaybackState>> recordedPlaybacks = new Dictionary<int, LinkedList<PlaybackState>>();

    private int nextOffset = 0;

    /// <summary>
    /// Maximum number of sequences per controller
    /// </summary>
    private const int NUM_SEQUENCES = 4;


    private Dictionary<GameObject, int> controllerPlaybackOffset = new Dictionary<GameObject, int>();

	void Start()
    {
        InputManager.AddCallback(OnControllerCallback);
	}

    private void OnControllerCallback(InputManager.InputState inputState)
    {
        if (inputState.Buttons.HasFlag(InputManager.InputState.ActiveFunction.SequenceRecord))
        {
            // Ensure that only one recording takes place at a time.
            foreach (RecordStruct s in recordingControllers)
            {
                if (s.controller == inputState.Controller)
                    return;
            }

            recordingControllers.Add(new RecordStruct() { controller = inputState.Controller, recording = new LinkedList<PlaybackState>() });
        }
        else if ((inputState.Buttons & InputManager.InputState.ActiveFunction.SequencePlaybacks) > 0)
        {
            bool isRecording = false;


            foreach (RecordStruct s in recordingControllers)
            {
                if (s.controller == inputState.Controller)
                    isRecording = true;
            }

            int offset = -1;

            if (!controllerPlaybackOffset.TryGetValue(inputState.Controller, out offset))
            {
                offset = nextOffset++;

                controllerPlaybackOffset.Add(inputState.Controller, offset);
            }

            int playbackIndex = 0;

            switch(inputState.Buttons & InputManager.InputState.ActiveFunction.SequencePlaybacks)
            {
                case InputManager.InputState.ActiveFunction.SequencePlayback0:
                    playbackIndex = 0;
                    break;
                case InputManager.InputState.ActiveFunction.SequencePlayback1:
                    playbackIndex = 1;
                    break;
                case InputManager.InputState.ActiveFunction.SequencePlayback2:
                    playbackIndex = 2;
                    break;
                case InputManager.InputState.ActiveFunction.SequencePlayback3:
                    playbackIndex = 3;
                    break;
            }

            if (isRecording)
            {
                RecordStruct recStruct = null;

                foreach (RecordStruct s in recordingControllers)
                {
                    if (s.controller == inputState.Controller)
                    {
                        recStruct = s;
                        break;
                    }
                }

                // Last frame is the frame that will destroy the controller clone, so it won't be seen
                recStruct.recording.AddLast(new PlaybackState(Vector3.zero, Quaternion.identity, 0));


                recordedPlaybacks.Remove(offset * NUM_SEQUENCES + playbackIndex);
                recordedPlaybacks.Add(offset * NUM_SEQUENCES + playbackIndex, recStruct.recording);

                recordingControllers.Remove(recStruct);
            }
            else
            {
                LinkedList<PlaybackState> recording;

                if (recordedPlaybacks.TryGetValue(offset * NUM_SEQUENCES + playbackIndex, out recording))
                {
                    GameObject controller = Instantiate(controllerTemplate);

                    controller.SetActive(true);

                    controller.transform.parent = null;

                    // Update() may not be called directly after cloning, to avoid odd starting positions, we place the controller at the first frame
                    controller.transform.position = recording.First.Value.position;
                    controller.transform.rotation = recording.First.Value.rotation;

                    InputManager.AddUserController(controller);

                    playingControllers.Add(new RecordStruct() { controller = controller, recording = recording, iterator = recording.GetEnumerator() });
                }
            }
        }
        else
        {
            // Record changes to input state too.
            foreach (RecordStruct s in recordingControllers)
            {
                if (s.controller == inputState.Controller)
                {
                    // Add buttons to last recorded state (good enough approximation)
                    PlaybackState lastState = s.recording.Last.Value;

                    lastState.buttons |= inputState.Buttons;

                    s.recording.Last.Value = lastState;
                }
            }
        }
    }
	
	void Update()
    {
        // TODO: Record in lower FPS and interpolate

		foreach(RecordStruct s in recordingControllers)
        {
            s.recording.AddLast(new PlaybackState(s.controller.transform.position, s.controller.transform.rotation, InputManager.GetInputState(s.controller).TriggerActuation));
        }

        for(int i = playingControllers.Count-1; i >= 0; i--)
        {
            RecordStruct s = playingControllers[i];

            PlaybackState curState = s.iterator.Current;

            s.controller.transform.position = curState.position;
            s.controller.transform.rotation = curState.rotation;

            // Emulate input state changes
            InputManager.GetInputState(s.controller).TriggerActuation = curState.triggerActuation;

            if (curState.buttons != InputManager.InputState.ActiveFunction.None)
            {
                InputManager.GetInputState(s.controller).AddButton(curState.buttons);
                InputManager.GetInputState(s.controller).RemoveButton(InputManager.InputState.ActiveFunction.All);
            }

            if (!s.iterator.MoveNext())
            {
                InputManager.RemoveUserController(s.controller);
                Destroy(s.controller);

                playingControllers.RemoveAt(i);
            }
        }
	}
}
