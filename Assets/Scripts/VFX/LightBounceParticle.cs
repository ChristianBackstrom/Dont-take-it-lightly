using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBounceParticle : MonoBehaviour
{
	[SerializeField]
	private ParticleSystem bounceParticle;
	private Vector3 position;
	private Vector3 direction;



	public void Bounced()
	{
		if (bounceParticle == null)
		{
			Debug.LogError("particle system does not exist in children");
			return;
		}

        bounceParticle.transform.position = position;
        bounceParticle.transform.rotation = Quaternion.LookRotation(direction);

		bounceParticle.Play();
	}
}
