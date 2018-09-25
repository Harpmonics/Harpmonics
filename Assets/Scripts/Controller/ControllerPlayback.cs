using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Records the motion of controllers after user presses to record and playbacks the motion again
/// </summary>
public class ControllerPlayback : MonoBehaviour
{

    private struct PlaybackState
    {
        public Vector3 position;
        public Quaternion rotation;

        public PlaybackState(Vector3 position, Quaternion rotation)
        {
            this.position = new Vector3(position.x, position.y, position.z);
            this.rotation = new Quaternion(rotation.x, rotation.y, rotation.z, rotation.w);
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
                recStruct.recording.AddLast(new PlaybackState(Vector3.zero, Quaternion.identity));


                recordedPlaybacks.Add(offset * NUM_SEQUENCES + playbackIndex, recStruct.recording);

                recordingControllers.Remove(recStruct);
            }
            else
            {
                LinkedList<PlaybackState> recording;

                if (recordedPlaybacks.TryGetValue(offset * NUM_SEQUENCES + playbackIndex, out recording))
                {

                    GameObject controller = Instantiate(inputState.Controller);

                    controller.transform.parent = null;

                    // Update() may not be called directly after cloning, to avoid odd starting positions, we place the controller at the first frame
                    controller.transform.position = recording.First.Value.position;
                    controller.transform.rotation = recording.First.Value.rotation;

                    InputManager.AddUserController(controller);

                    playingControllers.Add(new RecordStruct() { controller = controller, recording = recording, iterator = recording.GetEnumerator() });
                }
            }
        }
    }
	
	void Update()
    {
        // TODO: Record in lower FPS and interpolate

		foreach(RecordStruct s in recordingControllers)
        {
            s.recording.AddLast(new PlaybackState(s.controller.transform.position, s.controller.transform.rotation));
        }

        for(int i = playingControllers.Count-1; i >= 0; i--)
        {
            RecordStruct s = playingControllers[i];

            PlaybackState curState = s.iterator.Current;

            s.controller.transform.position = curState.position;
            s.controller.transform.rotation = curState.rotation;

            if (!s.iterator.MoveNext())
            {
                InputManager.RemoveUserController(s.controller);
                Destroy(s.controller);

                playingControllers.RemoveAt(i);
            }
        }
	}
}
