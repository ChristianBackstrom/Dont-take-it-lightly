using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerEntry
{
	public InputHandler inputHandler;
	public string prompt;
	public GameObject instance;
}
public class InputDetector : MonoBehaviour, IRebindOperation
{

	public InputData inputData;
	[SerializeField] InputHandler playerOne;
	[SerializeField] InputHandler playerTwo;

	private Controls controls;

	public Dictionary<InputDevice, PlayerEntry> connectedDevices = new();


	private float timer = 0;
	private const float Cooldown = .2f;



	private void Awake()
	{
		controls = new Controls();

		controls.Player.Join.performed += OnJoinedPressed;
		controls.Player.Join.Enable();
	}

	private void OnDisable()
	{
		controls.Player.Join.performed -= OnJoinedPressed;

	}

	private void OnJoinedPressed(InputAction.CallbackContext context)
	{
		JoinPlayer(context.control.device);
	}

	private void Update()
	{
		timer += Time.deltaTime;
		if (timer < Cooldown) return;

		List<string> unityDevices = InputSystem.devices.ToList().Select(x => x.displayName).ToList();

		foreach (var device in connectedDevices)
		{
			if (!unityDevices.Contains(device.Key.displayName))
			{
				print(device.Key.displayName);
				connectedDevices.Remove(device.Key);
				device.Value.inputHandler.Controls = null;
			}
		}
	}

	private void JoinPlayer(InputDevice device)
	{
		if (connectedDevices.ContainsKey(device)) return;

		Controls playerControls = new Controls();

		playerControls.devices = new[] { device };
		Debug.Log(device);


		if (playerOne.Controls == null)
		{
			var data = inputData.GetEntry(device.displayName);
			if (data == null)
			{
				print("P1: No Inputdata detected");
			}

			PlayerEntry entry = new()
			{
				inputHandler = playerOne,
				prompt = data.prompt,
				//todo: add button prompts prefabs to inputdata scriptable object
				instance = data.p1Prefab
			};

			connectedDevices.Add(device, entry);
			playerControls.Player.Move.Enable();

			string rebinds = PlayerPrefs.GetString("rebinds0");
			if (!string.IsNullOrEmpty(rebinds))
			{
				playerControls.LoadBindingOverridesFromJson(rebinds);
			}

			playerOne.Controls = playerControls;
			playerOne.Device = device;

			return;
		}

		if (playerTwo.Controls == null)
		{
			var data = inputData.GetEntry(device.displayName);
			if (data == null)
			{
				print("P2: No Inputdata detected");
			}
			PlayerEntry entry = new()
			{
				inputHandler = playerTwo,
				prompt = data.prompt,
				//todo: add button prompt prefabs to inputdata scriptable object
				instance = data.p2Prefab
			};
			//send entry to interactor here?

			connectedDevices.Add(device, entry);
			playerControls.Player.Move.Enable();

			string rebinds = PlayerPrefs.GetString("rebinds1");
			if (!string.IsNullOrEmpty(rebinds))
			{
				playerControls.LoadBindingOverridesFromJson(rebinds);
			}

			playerTwo.Controls = playerControls;
			playerTwo.Device = device;

			return;
		}
	}

	public void UpdateControlRebinds(int player)
	{
		Controls controls = new Controls();
		if (player == 0)
		{
			controls = playerOne.Controls;
		}
		if (player == 1)
		{
			controls = playerTwo.Controls;
		}

		string rebinds = PlayerPrefs.GetString("rebinds" + player);
		if (!string.IsNullOrEmpty(rebinds))
		{
			controls.Disable();
			controls.LoadBindingOverridesFromJson(rebinds);
			controls.Enable();
		}

	}
}