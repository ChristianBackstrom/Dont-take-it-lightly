using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class LightningController : MonoBehaviour
{
	[Header("Lightning Data")]
	[SerializeField]
	private float lightningCooldown;

	[SerializeField]
	private float lightningLength = 1f;
	[SerializeField]
	private GameObject[] lightnings;

	private float timer = 0;

	private void Awake()
	{
		lightnings = transform.GetComponentsInChildren<Light>(true).Select(x => x.gameObject).ToArray();

		foreach (GameObject lightning in lightnings)
		{
			lightning.SetActive(false);
		}
	}

	private void Update()
	{
		timer += Time.deltaTime;
		if (timer >= lightningCooldown)
		{
			timer = 0;
			Strike();
		}
	}



	private async void Strike()
	{
		int index = Random.Range(0, lightnings.Length - 1);

		lightnings[index].SetActive(true);
		lightnings[index].GetComponent<Light>().enabled = false;

		await Task.Delay(Mathf.RoundToInt(100));

		lightnings[index].GetComponent<Light>().enabled = true;

		await Task.Delay(Mathf.RoundToInt(lightningLength * 1000 - 100));

		lightnings[index].SetActive(false);
	}

	private void OnValidate()
	{
		if (lightningCooldown < lightningLength)
		{
			lightningCooldown = lightningLength + 1;
		}
	}
}
