using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Canvas))]
public class VirtualKeyboard : MonoBehaviour
{
    [Tooltip("Template to base all buttons of, should have a Text and Button element.")]
    public GameObject buttonTemplate;

    [Tooltip("Scoreboard to input strings to.")]
    public ScoreboardDisplay scoreboard;

    /// <summary>
    /// Should all UI elements be recreated (for editor preview)?
    /// </summary>
    private bool dirty = false;

    /// <summary>
    /// Is the scoreboard currently being animated in?
    /// </summary>
    private bool isAnimating = false;

    /// <summary>
    /// Current string the user has written.
    /// </summary>
    private string currentString;

    private void SetupUI()
    {
        // Remove old elements
        List<GameObject> oldElements = new List<GameObject>();

        for (int i = 0; i < this.transform.childCount; i++)
        {
            oldElements.Add(this.transform.GetChild(i).gameObject);
        }

        foreach (GameObject obj in oldElements)
        {
            // Don't remove the button template if it's stored as a child
            if (obj == buttonTemplate)
                continue;

            DestroyImmediate(obj);
        }

        Canvas canvas = GetComponent<Canvas>();

        canvas.renderMode = RenderMode.WorldSpace;
        canvas.transform.localScale = new Vector3(0.005f, 0.005f, 0.005f);
        
        Font fontName = Font.CreateDynamicFontFromOSFont("Arial", 32);

        List<char[]> keyboard = new List<char[]>();

        // Alphanumeric characters on Swedish QWERTY keyboard (including space bar), ← represents backspace
        keyboard.Add(new char[] { '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '←' });
        keyboard.Add(new char[] { 'Q', 'W', 'E', 'R', 'T', 'Y', 'U', 'I', 'O', 'P', 'Å'});
        keyboard.Add(new char[] { 'A', 'S', 'D', 'F', 'G', 'H', 'J', 'K', 'L', 'Ö', 'Ä'});
        keyboard.Add(new char[] { 'Z', 'X', 'C', 'V', 'B', 'N', 'M', ',', '.', '-' });
        keyboard.Add(new char[] { ' ' });

        Vector2 buttonSize = buttonTemplate.GetComponent<UnityEngine.UI.Image>().rectTransform.sizeDelta * buttonTemplate.transform.localScale.x;

        float spacing = (buttonSize.y * 0.1f);

        // Offsets to match the QWERTY layout
        List<float> xOffsets = new List<float>();

        xOffsets.Add(0);
        xOffsets.Add(buttonSize.x * 0.5f);
        xOffsets.Add(buttonSize.x * 0.75f);
        xOffsets.Add(buttonSize.x * 1.25f + spacing);
        xOffsets.Add((buttonSize.x + spacing) * 5.35f);

        float rowY = (buttonSize.y + spacing) * keyboard.Count / 2 - (buttonSize.y + spacing)/2;

        for(int i = 0; i < keyboard.Count; i++)
        {
            char[] row = keyboard[i];

            float colX = xOffsets[i] - (buttonSize.x + spacing) * 5.33f;

            for (int i2 = 0; i2 < row.Length; i2++)
            {
                // Create a new button from the template
                GameObject objButton = Instantiate(buttonTemplate, this.transform);

                objButton.SetActive(true);

                objButton.name = "Key " + row[i2];

                // Widen the spacebar since that's the only key that can't be small.
                if (row[i2] == ' ')
                {
                    Vector2 curDelta = objButton.GetComponent<UnityEngine.UI.Image>().rectTransform.sizeDelta;
                    objButton.GetComponent<UnityEngine.UI.Image>().rectTransform.sizeDelta = new Vector2(curDelta.x * 6.75f, curDelta.y);
                }

                UnityEngine.UI.Button button = objButton.GetComponent<UnityEngine.UI.Button>();

                // Must use local variable for below lambda
                char c = row[i2];
                
                button.onClick.AddListener(() => OnKeyClick(c));

                UnityEngine.UI.Text text = objButton.GetComponentInChildren<UnityEngine.UI.Text>();

                text.text = "" + row[i2];

                objButton.transform.localPosition = new Vector3(colX, rowY, 0);

                colX += buttonSize.x + spacing;
            }

            rowY -= buttonSize.y + spacing;
        }
    }

    public void OnKeyClick(char key)
    {
        if (key == '←')
        {
            if (currentString.Length == 0)
                return;

            currentString = currentString.Substring(0, currentString.Length - 1);
        }
        else
        {
            currentString += key;
        }

        if (scoreboard != null)
        {
            scoreboard.UpdateUserName(currentString);
        }
    }

    /// <summary>
    /// Animate the user's score moving into the scoreboard.
    /// </summary>
    public void StartAnimation()
    {
        isAnimating = true;

        StartCoroutine(AnimationCoroutine());
    }

    private void SetTextAlpha(UnityEngine.UI.Text text, float alpha)
    {
        text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);
    }

    private void LerpTextAlpha(UnityEngine.UI.Text text, float target, float lerpFactor)
    {
        // Already reached target
        if ((target - text.color.a) < 10e-3)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, target);
            return;
        }

        float diff = (target - text.color.a) / Mathf.Abs(target - text.color.a);

        text.color = new Color(text.color.r, text.color.g, text.color.b, Mathf.Clamp(text.color.a + diff * lerpFactor, 0, 1));
    }

    IEnumerator AnimationCoroutine()
    {
        // If the children aren't active, we apparently can't find their components
        for (int i = 0; i < this.transform.childCount; i++)
        {
            GameObject element = this.transform.GetChild(i).gameObject;

            if (element != buttonTemplate)
                element.SetActive(true);
        }

        float alpha = 0f;

        while (true)
        {
            alpha += Time.deltaTime * 2;

            if (alpha > 1)
                alpha = 1;

            foreach (CanvasRenderer canvasRenderer in this.transform.GetComponentsInChildren<CanvasRenderer>())
            {
                if (canvasRenderer.transform.parent.gameObject == buttonTemplate)
                    continue;

                canvasRenderer.SetAlpha(alpha);
            }

            yield return null;

            if (1 - alpha < 10e-5)
                break;
        }
    }

    void Start()
    {
        if (Application.isPlaying)
        {
            SetupUI();

            // Hide all buttons and disable them for now
            foreach(CanvasRenderer canvasRenderer in this.transform.GetComponentsInChildren<CanvasRenderer>())
            {
                if (canvasRenderer.transform.parent.gameObject == this.gameObject)
                    continue;

                canvasRenderer.SetAlpha(0);
                canvasRenderer.transform.parent.gameObject.SetActive(false);
            }
        }
    }

    void Reset()
    {
        dirty = true;
    }

    void OnValidate()
    {
        dirty = true;
    }

    void Update()
    {
        // Rebuilding is only done in the editor
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            if (dirty)
            {
                dirty = false;
                SetupUI();
            }
        }
#endif
    }
}
