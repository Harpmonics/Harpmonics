using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script generates a mesh in real time. The mesh is composed of two long
/// segments (upper and lower) joined by a single joint, i.e [======]x[======]
/// where x is the joint. The intention is for the mesh to be easy to "bend" by
/// moving the vertices that make up the joint.
/// 
/// </summary>
[SelectionBase]
public class SegmentedLaser : MonoBehaviour
{
    /// <summary>
    /// Provides a visual representation in the editor of where the generated mesh will
    /// appear in the game
    /// </summary>
    private GameObject m_placeholder;

    /// <summary>
    /// The generated mesh will be assigned to this MeshFilter
    /// </summary>
    public MeshFilter m_output;

    /// <summary>
    /// The generated mesh will be assigned to this MeshCollider
    /// </summary>
    public MeshCollider m_collider;

    /// <summary>
    /// The renderer is used to switch materials depending on which state the beam
    /// is in. This is mostly useful for testing and debugging.
    /// </summary>
    public MeshRenderer m_renderer;

    /// <summary>
    /// Material to assign when the beam is not being touched
    /// </summary>
    public Material notouch;

    /// <summary>
    /// Material to assign when the beam is being touched but not grabbed/dragged
    /// </summary>
    public Material touch;

    /// <summary>
    /// Material to assign when the beam is being grabbed/dragged
    /// </summary>
    public Material grab;

    /// <summary>
    /// Radius of generated beam
    /// </summary>
    public float m_radius;

    /// <summary>
    /// Height of generated beam
    /// </summary>
    public float m_height;

    /// <summary>
    /// The size of the joint connecting the two segments relative to the total height
    /// The valid range is (0, 1), a value of 0.1 means the joint makes up 10% of the total height
    /// </summary>
    public float m_jointRelativeSize = 0.1f;

    /// <summary>
    /// The position (offset from mesh origin) of the joint
    /// The y coordinate is relative to the beam and has a valid range of(-.5 + m_jointRelativeSize, .5 - m_jointRelativeSize)
    /// The x and z coordinates are in absolute units for translating the joint in the xz-plane
    /// </summary>
    private Vector3 m_jointPos = new Vector3(0, 0, 0);

    /// <summary>
    /// The possible states the beam can be in
    /// </summary>
    private enum BeamState { Base, Touched, Grabbed };

    /// <summary>
    /// The current state of the beam
    /// </summary>
    private BeamState m_state = BeamState.Base; // TODO: make a bitfield?

    /// <summary>
    /// True if the beam is currently vibrating, false otherwise.
    /// </summary>
    private bool m_vibrating = false; // TODO: add to beamstate when its a bitfield

    /// <summary>
    /// The collider that initiated a grab on the beam
    /// </summary>
    private Collider m_grabber;

    /// <summary>
    /// The current velocity of the joint. Only relevant when the beam is vibrating
    /// </summary>
    private Vector3 m_jointVel = new Vector3(0, 0, 0);
    
    /// <summary>
    /// A MIDI output helper script for assisting with producing MIDI output
    /// </summary>
    public MIDIOutputHelper m_midiout;

    /// <summary>
    /// If larger than zero, the beam ignores any touch / grab actions.
    /// If larger than zero, is decremented by one each frame.
    /// Used to prevent the beam from being instantly touched again when releasing a grab
    /// (which would prevent it from entering the vibration phase)
    /// </summary>
    private int m_notouchframes = 0;

    /// <summary>
    /// Available types of MIDI controller outputs for beam interactions
    /// </summary>
    public enum InteractionOutputType
    {
        None,
        Volume,
        Pitchbend,
        Controller1,
        Controller2,
        Controller3,
        Controller4
    }

    /// <summary>
    /// The Control Change parameter for the "Volume" interaction output type
    /// </summary>
    public int m_volumeCC = 7;

    /// <summary>
    /// The Control Change parameter for the "Controller1" interaction output type
    /// </summary>
    public int m_controller1CC = 74; // Nord Lead 2 / Synth1VST Filter Cutoff

    /// <summary>
    /// The Control Change parameter for the "Controller2" interaction output type
    /// </summary>
    public int m_controller2CC = 42; // Nord Lead 2 / Synth1VST Filter Resonance

    /// <summary>
    /// The Control Change parameter for the "Controller3" interaction output type
    /// </summary>
    public int m_controller3CC = 22; // Nord Lead 2 / Synth1VST LFO1 Amount

    /// <summary>
    /// The Control Change parameter for the "Controller4" interaction output type
    /// </summary>
    public int m_controller4CC = 5;  // Nord Lead 2 / Synth1VST Portamento Time

    /// <summary>
    /// The interaction output type for dragging a beam in the Z direction (depth-wise)
    /// </summary>
    public InteractionOutputType m_dragZInteractionType = InteractionOutputType.Controller1;

    /// <summary>
    /// The sensitivity (amplification factor) of the Z-direction drag beam interaction
    /// </summary>
    public float m_dragZInteractionSensitivity = 1.0f;

    /// <summary>
    /// The interaction output type for dragging a beam in the X direction (left/right)
    /// </summary>
    public InteractionOutputType m_dragXInteractionType = InteractionOutputType.Pitchbend;

    /// <summary>
    /// The sensitivity (amplification factor) of the X-direction drag beam interaction
    /// </summary>
    public float m_dragXInteractionSensitivity = 1.0f;

    /// <summary>
    /// The interaction output type for touching and/or dragging a beam in the Y direction (up/down)
    /// </summary>
    public InteractionOutputType m_heightInteractionType = InteractionOutputType.Volume;

    /// <summary>
    /// The sensitivity (amplification factor) of the Y-direction touch/drag beam interaction
    /// </summary>
    public float m_heightInteractionSensitivity = 1.0f;

    /// <summary>
    /// Create and/or set the properties of the placeholder object
    /// </summary>
    void UpdatePlaceholder()
    {
        if (m_placeholder == null)
        {
            if (transform.Find("Placeholder") != null)
            {
                m_placeholder = transform.Find("Placeholder").gameObject;
            }
            else
            {
                m_placeholder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                m_placeholder.name = "Placeholder";
                m_placeholder.transform.parent = this.transform;
            }
        }

        m_placeholder.transform.position = new Vector3(0, 0, 0);
        m_placeholder.transform.localPosition = new Vector3(0, 0, 0);

        // primitive cylinder has height 2 and radius 0.5
        m_placeholder.transform.localScale = new Vector3(m_radius / 0.5f, m_height / 2.0f, m_radius / 0.5f);
    }

    /// <summary>
    /// Hide the placeholder object
    /// </summary>
    void HidePlaceholder()
    {
        if (m_placeholder != null)
        {
            m_placeholder.GetComponent<Renderer>().enabled = false;
        }
    }

    /// <summary>
    /// Generate the mesh. Note that this also re-creates the MeshCollider which causes Unity
    /// to re-fire any OnTriggerX events that were already taking place (e.g even if already touching
    /// the mesh and already having received an OnTriggerEnter event, a new OnTriggerEnter event will
    /// be generated simply due to the MeshCollider being recomputed)
    /// </summary>
    void GenerateMesh()
    {
        Mesh mesh = m_output.mesh;

        mesh.Clear();

        // a cuboid for now, for simplicity
        
        Vector3 pos = transform.position;
        
        float bottom = -m_height / 2.0f;
        float top = m_height / 2.0f;

        float jointAbsoluteSize = m_height * m_jointRelativeSize;
        float jointLower = (m_jointPos.y * m_height) - (jointAbsoluteSize / 2.0f);
        float jointUpper = (m_jointPos.y * m_height) + (jointAbsoluteSize / 2.0f);

        mesh.vertices = new Vector3[]
        {
            // bottom 4 vertices
            new Vector3(m_radius, bottom, m_radius), // Ab
            new Vector3(-m_radius, bottom, m_radius), // Bb
            new Vector3(-m_radius, bottom, -m_radius), // Cb
            new Vector3(m_radius, bottom, -m_radius), // Db

            // joint lower 4 vertices
            new Vector3(m_radius + m_jointPos.x , jointLower, m_radius + m_jointPos.z), // Jba
            new Vector3(-m_radius + m_jointPos.x, jointLower, m_radius + m_jointPos.z), // Jbb
            new Vector3(-m_radius + m_jointPos.x, jointLower, -m_radius + m_jointPos.z), // Jbc
            new Vector3(m_radius + m_jointPos.x, jointLower, -m_radius + m_jointPos.z), // Jbd

            // joint upper 4 vertices
            new Vector3(m_radius + m_jointPos.x, jointUpper, m_radius + m_jointPos.z), // Jta
            new Vector3(-m_radius + m_jointPos.x, jointUpper, m_radius + m_jointPos.z), // Jtb
            new Vector3(-m_radius + m_jointPos.x, jointUpper, -m_radius + m_jointPos.z), // Jtc
            new Vector3(m_radius + m_jointPos.x, jointUpper, -m_radius + m_jointPos.z), // Jtd
            
            // top 4 vertices
            new Vector3(m_radius, top, m_radius), // At
            new Vector3(-m_radius, top, m_radius), // Bt
            new Vector3(-m_radius, top, -m_radius), // Ct
            new Vector3(m_radius, top, -m_radius) // Dt
        };

        // for my sanity
        int Ab = 0;
        int Bb = 1;
        int Cb = 2;
        int Db = 3;
        int Jba = 4;
        int Jbb = 5;
        int Jbc = 6;
        int Jbd = 7;
        int Jta = 8;
        int Jtb = 9;
        int Jtc = 10;
        int Jtd = 11;
        int At = 12;
        int Bt = 13;
        int Ct = 14;
        int Dt = 15;

        mesh.triangles = new int[] // clockwise seems to generate correct normals
        {
            // bottom face
            Ab, Db, Bb,
            Bb, Db, Cb,

            // top face
            At, Dt, Bt,
            Bt, Dt, Ct,



            // AB side lower segment face
            Ab, Jba, Bb,
            Bb, Jba, Jbb,

            // AB side joint face
            Jba, Jta, Jbb,
            Jbb, Jta, Jtb,

            // AB side upper segment face
            Jta, At, Jtb,
            Jtb, At, Bt,



            // BC side lower segment face
            Bb, Jbb, Cb,
            Cb, Jbb, Jbc,

            // BC side joint face
            Jbb, Jtb, Jbc,
            Jbc, Jtb, Jtc,

            // BC side upper segment face
            Jtb, Bt, Jtc,
            Jtc, Bt, Ct,



            // CD side lower segment face
            Cb, Jbc, Db,
            Db, Jbc, Jbd,

            // CD side joint face
            Jbc, Jtc, Jbd,
            Jbd, Jtc, Jtd,

            // CD side upper segment face
            Jtc, Ct, Jtd,
            Jtd, Ct, Dt,



            // DA side lower segment face
            Db, Jbd, Ab,
            Ab, Jbd, Jba,

            // DA side joint face
            Jbd, Jtd, Jba,
            Jba, Jtd, Jta,

            // DA side upper segment face
            Jtd, Dt, Jta,
            Jta, Dt, At,
        };

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        // not sure if texture coords are necessary
        //mesh.uv = new Vector2[]
        //{
        //    new Vector2(0, 0),
        //    new Vector2(0, 0),
        //    new Vector2(0, 0),
        //    new Vector2(0, 0),
        //    new Vector2(0, 0),
        //    new Vector2(0, 0),
        //    new Vector2(0, 0),
        //    new Vector2(0, 0),
        //    new Vector2(0, 0),
        //    new Vector2(0, 0),
        //    new Vector2(0, 0),
        //    new Vector2(0, 0),
        //    new Vector2(0, 0),
        //    new Vector2(0, 0),
        //    new Vector2(0, 0),
        //    new Vector2(0, 0)
        //};

        m_collider.sharedMesh = mesh;
    }

    /// <summary>
    /// Compute the contact point along the beam (offset from the beam when dragging).
    /// </summary>
    /// <param name="cd">The collider that is making contact</param>
    void SetContactPoint(Collider cd)
    {
        Vector3 mypos = transform.position;
        float bottom = mypos.y -(m_height / 2.0f);
        float top = mypos.y + (m_height / 2.0f);

        float cpos = cd.transform.position.y;

        float offset = ((cpos - bottom) / (top - bottom)) - 0.5f;

        //Debug.Log("================================ OFFSET: " + offset);

        if (offset > -0.5f + m_jointRelativeSize && offset < 0.5f - m_jointRelativeSize)
        {
            m_jointPos.y = offset;

            m_jointPos.x = cd.transform.position.x - mypos.x;
            m_jointPos.z = cd.transform.position.z - mypos.z;
        }
    }

    /// <summary>
    /// Create the placeholder when the script loads
    /// </summary>
    void Awake()
    {
        UpdatePlaceholder();
    }

    /// <summary>
    /// Update the placeholder when inspector values are modified
    /// </summary>
    void OnValidate()
    {
        UpdatePlaceholder();
    }

    /// <summary>
    /// When starting Play mode, hide the placeholder and generate the mesh
    /// </summary>
    void Start ()
    {
        HidePlaceholder();
        GenerateMesh();
        m_renderer.material = notouch;
    }
    
    /// <summary>
    /// Called on each frame. Performs dampened vibration. Performs transition out of "no-touch frames".
    /// Performs continuation of an active drag.
    /// </summary>
    void Update ()
    {
        if (m_state == BeamState.Grabbed)
        {
            ContinueGrab();
        }

        if (m_vibrating)
        {
            m_jointVel = m_jointVel - (0.05f * m_jointPos);
            m_jointVel *= 0.95f;

            if (m_jointVel.magnitude < 0.0001f)
            {
                // TODO: add a "returning" state that lerps the beam back to zero
                // TODO: .. even at this small threshold there is a visible "snap"
                m_vibrating = false;
                m_jointVel = new Vector3(0, 0, 0);
                m_jointPos = new Vector3(0, 0, 0);
            }

            m_jointPos = m_jointPos + m_jointVel;
            GenerateMesh();
        }

        if (m_notouchframes > 0)
        {
            --m_notouchframes;
        }
    }

    /// <summary>
    /// Begin grabbing / dragging the beam.
    /// </summary>
    /// <param name="cd">The collider that is initiating a grab on the beam</param>
    void BeginGrab(Collider cd)
    {
        m_grabber = cd;
        m_state = BeamState.Grabbed;
        m_vibrating = false;
        m_renderer.material = grab;
        m_midiout.PlayNote();
        SetContactPoint(cd);
    }

    /// <summary>
    /// Update the beam while it is being dragged (i.e make the joint follow the object that is grabbing it)
    /// This method emits the interaction outputs for dragging in X, Y and Z.
    /// </summary>
    void ContinueGrab()
    {
        if (InputManager.GetInputState(m_grabber).TriggerActuation > 0.5)
        {
            SetContactPoint(m_grabber);

            if (Mathf.Abs(m_jointPos.x) > 0.01f)
            {
                int direction = (m_jointPos.x > 0) ? 1 : -1;
                float amount = 0.5f + Mathf.Clamp(direction * m_dragXInteractionSensitivity * Mathf.Abs(m_jointPos.x), -0.5f, 0.5f);

                //Debug.Log("===================== DRAG_X AMOUNT " + amount);

                SendInteractionOutput(m_dragXInteractionType, amount);
            }
            else
            {
                SendInteractionOutput(m_dragXInteractionType, 0.5f);
            }

            if (Mathf.Abs(m_jointPos.z) > 0.01f)
            {
                int direction = (m_jointPos.z > 0) ? -1 : 1;
                float amount = 0.5f + Mathf.Clamp(direction * m_dragZInteractionSensitivity * Mathf.Abs(m_jointPos.z), -0.5f, 0.5f);

                //Debug.Log("===================== DRAG_Z AMOUNT " + amount);

                SendInteractionOutput(m_dragZInteractionType, amount);
            }
            else
            {
                SendInteractionOutput(m_dragZInteractionType, 0.5f);
            }

            SendInteractionOutput(m_heightInteractionType, (m_jointPos.y * m_heightInteractionSensitivity) + 0.5f);

            GenerateMesh();
        }
        else
        {
            SendInteractionOutput(m_dragXInteractionType, 0.5f);
            SendInteractionOutput(m_dragZInteractionType, 0.5f);
            EndGrab();
        }
    }

    /// <summary>
    /// End the grabbed state, enter the vibrating phase
    /// </summary>
    void EndGrab()
    {
        m_notouchframes = 10;
        m_grabber = null;
        m_state = BeamState.Base;
        m_vibrating = true;
        m_renderer.material = notouch;
        m_midiout.StopNote();
    }

    /// <summary>
    /// Check for trigger actuation to determine whether a triggering collider is initiating a grab
    /// </summary>
    /// <param name="cd">The collider that is possibly initiating a grab</param>
    void CheckGrab(Collider cd)
    {
        if (InputManager.GetInputState(cd).TriggerActuation > 0.5)
        {
            BeginGrab(cd);
        }
    }

    /// <summary>
    /// Send a MIDI control message corresponding to the configured type for a given mode of interaction
    /// </summary>
    /// <param name="tpe">The type of interaction being performed</param>
    /// <param name="amount">The amount of action being applied. Range [0,1]</param>
    void SendInteractionOutput(InteractionOutputType tpe, float amount)
    {
        switch (tpe)
        {
            case InteractionOutputType.Volume:
                m_midiout.SendController(m_volumeCC, (int)(127 * amount));
                break;
            
            case InteractionOutputType.Pitchbend:
                m_midiout.SendPitchBend((int)(16383 * amount) - 8192);
                break;
            
            case InteractionOutputType.Controller1:
                m_midiout.SendController(m_controller1CC, (int)(127 * amount));
                break;
            
            case InteractionOutputType.Controller2:
                m_midiout.SendController(m_controller2CC, (int)(127 * amount));
                break;
            
            case InteractionOutputType.Controller3:
                m_midiout.SendController(m_controller3CC, (int)(127 * amount));
                break;
            
            case InteractionOutputType.Controller4:
                m_midiout.SendController(m_controller4CC, (int)(127 * amount));
                break;
            
            default:
                break;
        }
    }
    
    /// <summary>
    /// A trigger collider can take the beam from the base state to the touched state and can
    /// also cancel current vibration. Touching the beam also plays the assigned note and sends
    /// the configured MIDI output for the "height" interaction type.
    /// </summary>
    /// <param name="cd">The collider that is entering the beam</param>
    void OnTriggerEnter(Collider cd)
    {
        if (!InputManager.IsUserInput(cd))
        {
            return;
        }

        if (m_notouchframes > 0)
        {
            return;
        }

        if (m_vibrating)
        {
            m_vibrating = false;
            m_jointVel = new Vector3(0, 0, 0);
            m_jointPos = new Vector3(0, 0, 0);
            GenerateMesh();
        }

        if (m_state == BeamState.Base || m_state == BeamState.Touched)
        {
            m_state = BeamState.Touched;
            m_renderer.material = touch;
            SetContactPoint(cd);

            SendInteractionOutput(m_heightInteractionType, (m_jointPos.y * m_heightInteractionSensitivity) + 0.5f);

            m_midiout.PlayNote();
            CheckGrab(cd);
        }
    }
    
    /// <summary>
    /// Called as a collider remains in contact with the beam over several frames. Note that this will not
    /// be called when dragging the beam as during that state the MeshCollider is recomputed in every frame
    /// causing Unity to consider the collider to always be "entering" rather than "staying".
    /// </summary>
    /// <param name="cd">The collider that is remaining in contact with the beam</param>
    void OnTriggerStay(Collider cd)
    {
        if (!InputManager.IsUserInput(cd))
        {
            return;
        }

        if (m_state == BeamState.Touched)
        {
            SetContactPoint(cd);
            m_midiout.PlayNote();
            CheckGrab(cd);
        }
    }

    /// <summary>
    /// Called when a trigger collider is leaving the beam. Transitions the beam from the "touched"
    /// state to the "base" state and stops any current note.
    /// </summary>
    /// <param name="cd">The collider that is leaving the beam</param>
    void OnTriggerExit(Collider cd)
    {
        if (!InputManager.IsUserInput(cd))
        {
            return;
        }
        
        if (m_state == BeamState.Touched)
        {
            m_state = BeamState.Base;
            m_renderer.material = notouch;
            m_midiout.StopNote();
        }
    }
}
