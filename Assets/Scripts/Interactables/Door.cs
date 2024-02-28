using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Door : MonoBehaviour, IDataObject, IGUIDObject
{
	[SerializeField] string guid = System.Guid.NewGuid().ToString();

	[SerializeField]
	private int lightsNeeded = 1;
	[SerializeField]
	private float openSpeed = .5f;

	private Animator animator;

	private bool? isClosed;

	[SerializeField]
	private List<GameObject> lightHit = new List<GameObject>();

	[Header("Audio")]
	[SerializeField]
	private SimpleAudioEvent puzzleCompleteSound;

	private void Awake()
	{
		lightReciever = GetComponentInChildren<LightReciever>();
		animator = GetComponentInChildren<Animator>();

		if (SaveDataHandler.saveData != null) Load(SaveDataHandler.saveData);
		if (isClosed == false) return;
		SpawnLights();

		// Do not open
		void SpawnLights()
		{
			if (lightsNeeded == 1)
			{
				SpawnALight();
			}
			else
			{
				SpawnMultipleLightsandspacethemgoodly();
			}

			void SpawnALight()
			{
				var gm = Instantiate(magicPrefab, transform);
                magicalSealingSprites.Add(gm);
            }

			MagicSeal SpawnALightWIthAReferneceToit()
			{
				return Instantiate(magicPrefab, transform);
			}

			void SpawnMultipleLightsandspacethemgoodly()
			{
				float spacing = 2.0f / (float)lightsNeeded;
				Vector3 startPos = new Vector3(0, 0, -0.5f);
				for (int i = 0; i < lightsNeeded; i++)
				{
					MagicSeal gm = SpawnALightWIthAReferneceToit();
                    magicalSealingSprites.Add(gm);

                    gm.transform.localScale *= (1.5f) / (float)lightsNeeded;
					gm.transform.localPosition += startPos + Vector3.forward * spacing * i;
				}
			}
		}
	}

	private LightReciever lightReciever;

	public async void LightHit()
	{
		if (lightHit.Contains(lightReciever.LatestGameObject)) return;
		lightHit.Add(lightReciever.LatestGameObject);

		int amount = lightHit.Count;
		await magicalSealingSprites[amount - 1].LightItUp();

		if (amount >= lightsNeeded)
		{

			for (int i = 0; i < magicalSealingSprites.Count; i++)
			{
				magicalSealingSprites[i].gameObject.SetActive(false);
			}

			if (isClosed == null)
			{
				OpenTheDoor();
			}

			isClosed = false;

			animator.SetBool("Closed", (bool)isClosed);
		}
	}

	private void OpenTheDoor()
	{
		animator.SetTrigger("Open");
		doorOpenParticle.GetAtPosAndRot<PooledMonoBehaviour>(transform.position, Quaternion.identity);

		AudioManager.Instance.PlaySoundEffect(puzzleCompleteSound);
	}

	private List<MagicSeal> magicalSealingSprites = new List<MagicSeal>();

	public void RegenerateGUID()
	{
		guid = System.Guid.NewGuid().ToString();
	}

	public Object GetObject()
	{
		return this;
	}

    [Header("VFX")]
    [SerializeField]
    private PooledMonoBehaviour doorOpenParticle;

    public void Save(SaveData data)
	{
		data.SetBool(guid, isClosed);
	}

	public void Load(SaveData data, bool reset = false)
	{
		bool? loaded = data.GetBool(guid);
		if (loaded == null) return;
		if (loaded == false)
		{
			isClosed = false;
			animator.SetTrigger("Open");
			return;
		}
	}

    [SerializeField]
    private MagicSeal magicPrefab;
}
