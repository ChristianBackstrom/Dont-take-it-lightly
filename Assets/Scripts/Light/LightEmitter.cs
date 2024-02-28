using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEditor;
using UnityEngine;

public class LightEmitter : MonoBehaviour, IDataObject
{
	[Header("Setup")]
	[SerializeField]
	private LayerMask layerMask;

	[SerializeField]
	private LineRenderer lineRendPrefab;

	[Header("Lighting")]
	[SerializeField]
	private float lightDuration = 1f;

	[SerializeField]
	private float speedOfLight = 4f;

	private List<LineRenderer> lineRenderers = new List<LineRenderer>();

	private Gradient defaultColor;
	private LineRenderer lineRend;

	private int index = 0;
	private int started = 0;
	private int ended = 0;
	private int rays = 0;

	private void Start()
	{
		lineRend = Instantiate(lineRendPrefab, Vector3.zero, Quaternion.identity);

		lineRenderers.Add(lineRend);

		defaultColor = lineRenderers[0].colorGradient;
	}

	[ContextMenu("Shoot Light")]
	public void ShootLight()
	{
		Cleanup();
		StartCoroutine(BuildRayMesh());
	}

	public void Cleanup()
	{
		StopAllCoroutines();

		DisableLasers();
		rays = 0;
	}

	public void ShootLightCallback(Action callback)
	{
		StopAllCoroutines();

		StartCoroutine(ShootingLight(callback));
	}

	private IEnumerator ShootingLight(Action callback)
	{
        DisableLasers();
        rays = 0;
        yield return BuildRayMesh();

        callback?.Invoke();
    }

	private void DisableLasers()
	{
		for (int i = 0; i < lineRenderers.Count; i++)
		{
			lineRenderers[i].colorGradient = defaultColor;
			lineRenderers[i].transform.localScale = new Vector3(1, 1, 0);
			lineRenderers[i].enabled = false;
		}
	}

	private IEnumerator BuildRayMesh()
	{
		Vector3 origin = transform.position;
		Vector3 dir = transform.forward;
		LightTree Lights = ShootRay(origin, dir);

		started = 0;
		ended = 0;
		index = 0;
		StartCoroutine(ShootLaser(Lights));

		yield return null;
		while (started != ended)
		{
			yield return null;
		}

		yield return new WaitForSeconds(lightDuration);

		index = 0;
		yield return HandleRetractLasers(Lights, lineRenderers);
	}

	private IEnumerator ShootLaser(LightTree Light)
	{
		started += 1;
		int index = this.index++;

		List<Vector3> vertices = Light.Vertices;
		if (vertices == null)
		{
			yield break;
		}

		if (index >= lineRenderers.Count)
		{
			lineRenderers.Add(Instantiate(lineRend));
		}
		LineRenderer line = lineRenderers[index];

		line.enabled = true;

		if (Light.Color != null)
		{
			line.colorGradient = Light.Color;
		}

		if (Light.material != null)
		{
			line.sharedMaterial = Light.material;
		}

		line.SetPositions(vertices.ToArray());

		yield return AnimateLaser(line, Light);

		for (int i = 0; i < Light.LightChildren.Count; i++)
		{
			StartCoroutine(ShootLaser(Light.LightChildren[i]));
		}

		ended += 1;
	}

	private IEnumerator AnimateLaser(LineRenderer line, LightTree light)
	{
		float t = 0;

		float distance = Vector3.Distance(light.Vertices[0], light.Vertices[light.Vertices.Count - 1]);

		Vector3 startPosition = light.Vertices[0];
		Vector3 targetPosition = light.Vertices[light.Vertices.Count - 1];

		line.SetPosition(1, startPosition);

		while (t <= 1.0f)
		{
			t += Time.deltaTime * speedOfLight / distance;

			line.SetPosition(line.positionCount - 1, Vector3.Lerp(startPosition, targetPosition, t));

			yield return null;
		}

		line.SetPosition(line.positionCount - 1, targetPosition);
		light.RecieverFunction?.Invoke(this.gameObject);
	}

	private IEnumerator HandleRetractLasers(LightTree light, List<LineRenderer> lines)
	{
		yield return RetractLaser(lines[index++]);

		for (int i = 0; i < light.LightChildren.Count; i++)
		{
			StartCoroutine(HandleRetractLasers(light.LightChildren[i], lines));
		}
	}

	private IEnumerator RetractLaser(LineRenderer line)
	{
		if (!line.enabled)
		{
			yield break;
		}

		float distance = Vector3.Distance(line.GetPosition(0), line.GetPosition(line.positionCount - 1));

		float t = 0;

		Vector3 startPosition = line.GetPosition(0);
		Vector3 targetPosition = line.GetPosition(line.positionCount - 1);

		while (t <= 1.0f)
		{
			t += Time.deltaTime * speedOfLight / distance;

			line.SetPosition(0, Vector3.Lerp(startPosition, targetPosition, t));

			yield return null;
		}

		line.SetPosition(0, targetPosition);
	}

	public LightTree ShootRay(Vector3 origin, Vector3 dir, Gradient color = null, Material mat = null)
	{
		if (rays++ > 100)
		{
			return new LightTree();
		}

		if (Physics.Raycast(origin, dir, out RaycastHit hitInfo, 200, layerMask))
		{
			List<Vector3> vertices = new List<Vector3>();
			vertices.Add(origin);
			vertices.Add(hitInfo.point);

			Action<GameObject> func = null;
			if (hitInfo.collider.TryGetComponent(out LightReciever lightRec))
			{
				if (lightRec.RequireColor)
				{
					print(lightRec.RequiredColor.colorKeys[0] + " is not the same as " + color);
					if (color != null && lightRec.RequiredColor.colorKeys[0].color == color.colorKeys[0].color)
					{
                        func = lightRec.LightHit;
                    }
                }
				else
				{
                    func = lightRec.LightHit;
                }
            }

			if (hitInfo.collider.TryGetComponent(out HoleGenerator hole))
			{
				hole.Point = hitInfo.point;
				hole.HitNormal = -hitInfo.normal;
				func = hole.MakeHole;
			}

			LightTree light = new LightTree(vertices, origin, dir, func, color, mat);

			CheckMirror(dir, hitInfo, light);
			CheckPrism(dir, hitInfo, light);
			CheckPortal(hitInfo, light);

            return light;
		}

		return new LightTree();
	}

	private void CheckMirror(Vector3 dir, RaycastHit hitInfo, LightTree light)
	{
		if (hitInfo.collider.gameObject.CompareTag("Mirror"))
		{
			dir = Vector3.Reflect(dir, hitInfo.normal);
			dir.y = 0;
			LightTree item = ShootRay(hitInfo.point, dir, light.Color, light.material);
			if (item.Vertices != null)
			{
				light.LightChildren.Add(item);
			}
		}
	}

	private void CheckPrism(Vector3 dir, RaycastHit hitInfo, LightTree light)
	{
		if (hitInfo.collider.gameObject.TryGetComponent(out Prism prism))
		{
			List<Transform> sides = prism.LightRefractions(dir, out List<Gradient> colors, out List<Material> mats);

			for (int i = 0; i < sides.Count; i++)
			{
				LightTree item = ShootRay(sides[i].position, sides[i].forward, colors.Count > 0 ? item.Color = colors[i] : null, mats.Count > 0 ? item.material = mats[i] : null);

				if (item.Vertices != null)
				{
					light.LightChildren.Add(item);
				}
			}
		}
	}

    private void CheckPortal(RaycastHit hitInfo, LightTree light)
    {
        if (hitInfo.collider.gameObject.TryGetComponent(out Portal portal))
        {
            Vector3 direction = portal.GetOtherDirection(out Vector3 pos);

            LightTree item = ShootRay(pos, direction, light.Color, light.material);

            if (item.Vertices != null)
            {
                light.LightChildren.Add(item);
            }
        }
    }

    private void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawRay(transform.position, transform.forward);
	}

	public void Save(SaveData data)
	{

	}

	public void Load(SaveData data, bool reset = false)
	{
		DisableLasers();
	}
}

public struct LightTree
{
	public List<Vector3> Vertices;
	public Vector3 Origin;
	public Vector3 Direction;
	public Gradient Color;
	public Material material;

	public Action<GameObject> RecieverFunction;

	public List<LightTree> LightChildren;

	public LightTree(List<Vector3> vertices, Vector3 origin, Vector3 direction, Action<GameObject> recieverFunction, Gradient color, Material mat)
	{
		Vertices = vertices;
		Origin = origin;
		Direction = direction;
		RecieverFunction = recieverFunction;
		LightChildren = new List<LightTree>();
		Color = color;
		material = mat;
	}
}
