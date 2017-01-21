using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Terminal : MonoBehaviour
{
	private static Terminal instance;

	public static Terminal Instance {
		get {
			return instance;
		}
	}

	public Text contentTransform;
	public ScrollRect scrollRect;
	public InputField inputField;
	private string terminalText = "";

	// Initialization before "start"
	void Awake ()
	{
		instance = this;
	}

	void Start ()
	{
		contentTransform.text = "";
	}

	public void AppendText (string text)
	{
		terminalText += text;
		Canvas.ForceUpdateCanvases ();
		scrollRect.verticalScrollbar.value = 0f;
		Canvas.ForceUpdateCanvases ();
	}

	public void AppendLine (string text)
	{
		terminalText += "\n" + text;
		Canvas.ForceUpdateCanvases ();
		scrollRect.verticalScrollbar.value = 0f;
		Canvas.ForceUpdateCanvases ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		contentTransform.text = terminalText + "\n\n";
		inputField.Select ();
		inputField.ActivateInputField ();
	}

	public void EnableInput ()
	{
		inputField.gameObject.SetActive (true);
	}

	public void DisableInput ()
	{
		inputField.gameObject.SetActive (false);
	}

	public void OnEndEditing ()
	{
		GameManager.Instance.OnCommandInput (inputField.text);
		inputField.text = "";
	}

	void FixedUpdate ()
	{
	}
}
