using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class PossesionShaderManager : MonoBehaviour
{
    public float maxStrength = 1;
    public float minStrength = 0;
    public float step = 3;

    float strength = 0;
    MeshRenderer mesh;
    bool activated = false;
    bool Activated
    {
        get
        {
            return activated;
        }
        set
        {
            activated = value;
        }
    }

    void Start()
    {
        mesh = GetComponent<MeshRenderer>();
        //in order to keep all possessable objects from exhibiting the effect when one is posessed, make new instance of the material
        mesh.material = new Material(mesh.material);
    }

    private float strengthTarget => activated ? maxStrength : minStrength;

    bool activateRunning = false;
    public IEnumerator Activate()
    {
        Activated = !Activated;
        if(activateRunning) yield break;
        activateRunning = true;
        while (Mathf.Abs(strength - strengthTarget) > float.Epsilon)
        {
            strength = Mathf.MoveTowards(strength, strengthTarget, Time.deltaTime * step);
            mesh.sharedMaterial.SetFloat("_EffectStrength", strength);
            yield return new WaitForEndOfFrame();
        }
        activateRunning = false;
    }
}
