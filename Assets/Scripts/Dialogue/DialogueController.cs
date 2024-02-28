using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueController : Singleton<DialogueController>
{
	[SerializeField] private GameObject dialogueWindow;

	[SerializeField] private float textAppearSpeed;

	[SerializeField] private GameObject edgarPortrait;
	[SerializeField] private GameObject samPortrait;


	private TMP_Text textContainer;
	private Animator animator;



	private bool isVisible = false;

	[HideInInspector] public bool isWordComplete = true;

	private DialogueSoundController soundController;

	protected override void Awake()
	{
		base.Awake();

		if (dialogueWindow != null)
		{
			textContainer = dialogueWindow.GetComponentInChildren<TMP_Text>();
		}

		animator = dialogueWindow.GetComponent<Animator>();

		soundController = FindObjectOfType<DialogueSoundController>();
	}

	public void ShowPrompt(string text)
	{
		if (dialogueWindow != null)
		{
			textContainer.text = "";

			ShowWindow();

			WriteText(text);
		}
	}



	public async void WriteText(string text)
	{
		char character = (char)text[0];
		if (text.StartsWith("E:"))
		{
			edgarPortrait.SetActive(true);
			samPortrait.SetActive(false);
			text = text.Substring(3);
		}
		else if (text.StartsWith("S:"))
		{
			samPortrait.SetActive(true);
			edgarPortrait.SetActive(false);
			text = text.Substring(3);
		}

		while (!isVisible)
		{
			await Task.Yield();
		}


		isWordComplete = false;

		int shownText = 0;

		string lowercaseText = text.ToLower();

		while (shownText < text.Length - 1)
		{
			if (isWordComplete)
			{
				FinishWord(text);
				return;
			}


			shownText++;

			textContainer.text = text.Substring(0, shownText);

			if (lowercaseText[shownText] >= 97 && lowercaseText[shownText] <= 122)
				soundController.PlayLetter(character, lowercaseText[shownText]);

			await Task.Delay(Mathf.RoundToInt((1 / textAppearSpeed) * 1000));
		}

		isWordComplete = true;
	}

	private void FinishWord(string text)
	{
		textContainer.text = text;
	}

	private void AnimationComplete()
	{
		isVisible = true;

		textContainer.text = "";
	}

	private void ShowWindow()
	{
		animator.SetTrigger("Show");
	}

	public void HideWindow()
	{
		animator.SetTrigger("Hide");

	}
}


