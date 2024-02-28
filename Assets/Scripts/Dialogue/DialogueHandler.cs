using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class DialogueHandler : MonoBehaviour, IDataObject, IGUIDObject
{
	[SerializeField] string guid;

	[SerializeField]
	private Dialogue[] dialogues;



	private Interactor interactor;
	private InputHandler humanInput;

	private GhostInteractor ghostInteractor;
	private InputHandler ghostInput;
	[SerializeField]
	private string[] dialogueText;
	private Dialogue activeDialogue;
	private int shownDialogue;
	private int currentDialogueEvent;

	private bool isDialogueActive = false;
	private int oldDialogue = -1;

	private void Awake()
	{
		interactor = FindObjectOfType<Interactor>();
		ghostInteractor = FindObjectOfType<GhostInteractor>();

		humanInput = interactor.GetComponent<InputHandler>();
		ghostInput = ghostInteractor.GetComponent<InputHandler>();
		if (SaveDataHandler.saveData != null)
		{
			this.oldDialogue = SaveDataHandler.saveData.GetInt(guid);
		}
	}

	private void Start()
	{
		SetDialogue(0);
	}

	private void Update()
	{
		if (isDialogueActive)
		{
			if (humanInput.Interact != null && humanInput.Interact.WasPressedThisFrame())
			{
				AcceptedInput();
			}

			if (ghostInput.Interact != null && ghostInput.Interact.WasPressedThisFrame())
			{
				AcceptedInput();
			}
		}

		void AcceptedInput()
		{
			if (!DialogueController.Instance.isWordComplete)
			{
				DialogueController.Instance.isWordComplete = true;
				return;
			}

			if (shownDialogue < dialogueText.Length - 1)
			{
				NextDialogue();
				return;
			}

			EndDialogue();

		}
	}





	public void SetDialogue(int index)
	{
		if (index <= oldDialogue) return;

		interactor.GetComponent<Movement>().Lock();
		ghostInteractor.GetComponent<Movement>().Lock();

		oldDialogue = index;
		interactor.enabled = false;
		ghostInteractor.enabled = false;

		activeDialogue = dialogues[index];
		dialogueText = dialogues[index].Dialogues.text.Trim(' ').Split('\n');

		shownDialogue = 0;
		isDialogueActive = true;
		DialogueController.Instance.ShowPrompt(dialogueText[shownDialogue]);
	}

	private void NextDialogue()
	{
		shownDialogue++;

		if (dialogueText[shownDialogue].StartsWith('#'))
		{
			activeDialogue.Events[currentDialogueEvent]?.Invoke();

			currentDialogueEvent++;
			shownDialogue++;
		}

		if (shownDialogue >= dialogueText.Length)
		{
			EndDialogue();
			return;
		}

		DialogueController.Instance.WriteText(dialogueText[shownDialogue]);
	}

	private void EndDialogue()
	{
		interactor.enabled = true;
		ghostInteractor.enabled = true;
		isDialogueActive = false;

		interactor.GetComponent<Movement>().Unlock();
		ghostInteractor.GetComponent<Movement>().Unlock();


		DialogueController.Instance.HideWindow();
	}

	public void TestEvent()
	{
		print("invoked");
	}


	public void LoadEvents()
	{
		for (int i = 0; i < dialogues.Length; i++)
		{
			if (dialogues[i].Dialogues == null)
			{
				continue;
			}

			dialogues[i].Events = new List<UnityEvent>();

			foreach (string line in dialogues[i].Dialogues.text.Split('\n'))
			{
				if (line.StartsWith('#'))
				{
					dialogues[i].Events.Add(new UnityEvent());
				}
			}
		}
	}

	/* guid for saving */
	public void RegenerateGUID()
	{
		guid = System.Guid.NewGuid().ToString();
	}
	public Object GetObject()
	{
		return this;
	}

	/* save system */
	public void Save(SaveData saveData)
	{
		saveData.SetInt(guid, oldDialogue);
	}

	public void Load(SaveData saveData, bool reset = false)
	{
		if (isDialogueActive) EndDialogue();
		this.oldDialogue = saveData.GetInt(guid);
	}
}

[System.Serializable]
public struct Dialogue
{
	public TextAsset Dialogues;
	public List<UnityEvent> Events;
}

#if UNITY_EDITOR

[CustomEditor(typeof(DialogueHandler))]
public class DialogueHandlerEditor : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		DialogueHandler dialogueHandler = (DialogueHandler)target;

		if (GUILayout.Button("Load Events"))
		{
			dialogueHandler.LoadEvents();
		}
	}
}

#endif