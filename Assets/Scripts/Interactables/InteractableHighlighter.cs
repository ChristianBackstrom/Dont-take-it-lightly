using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableHighlighter : MonoBehaviour, IDataObject
{

    [Header("Light configuration")]
    [SerializeField] float farIntensity = 2f;
    [SerializeField] float closeIntensity = 8f;
    //[SerializeField] float intensifySpeed = 1f;
    [SerializeField] float dimSpeed = 1f;


    [Header("Current Light Intensity")]
    public float currStrength;


    [Header("Colours")]
    [SerializeField] Color colourWhenInteractedWith = Color.blue; //These are just meant to be the default colors
    [SerializeField] Color baseColour = Color.white;
    //reference to current color.
    public Color currColour;

    MeshRenderer mesh;

    bool isClose;
    bool isInteractedWith = false;

    [SerializeField]
    float lerpSpeed = 1f;
    float lerpTimer = 0;

    void Start()
    {
        mesh = this.gameObject.GetComponentInChildren<MeshRenderer>();

        // For security
        if (mesh == null)
        {
            Destroy(this); // Were just destroying the script
        }
    }

    void Update()
    {
        currColour = CurrentColour();
        //store strength of last frame
        float oldStrength = currStrength;
        if (isInteractedWith)
        {
            //stick lerptimer += time.deltatime into a clamp between 0 and 1
            lerpTimer = Mathf.Clamp01(lerpTimer += Time.deltaTime);
            SetColour(LerpColour(lerpTimer, baseColour, colourWhenInteractedWith));
            currStrength += 8 * Time.fixedDeltaTime;
        }
        else
        {
            //same here but subtract instead of add
            lerpTimer = Mathf.Clamp01(lerpTimer -= Time.deltaTime);
            SetColour(LerpColour(lerpTimer, baseColour, colourWhenInteractedWith));
            currStrength -= dimSpeed * Time.fixedDeltaTime;
        }

        //clamp current strength to the confines of farintensity and closeintensity
        currStrength = Mathf.Clamp(currStrength, farIntensity, closeIntensity);




        //return here if we dont need to change strength this frame
        if (oldStrength == currStrength) return;
        mesh.material.SetFloat("_Strength", currStrength);
    }


    void SetColour(Color colour)
    {
        mesh.material.SetColor("_Colour", colour);
    }
    public Color LerpColour(float t, Color baseColour, Color targetColour)
    {
        Color LerpedColor = Color.Lerp(baseColour, targetColour, t * lerpSpeed);
        return LerpedColor;
    }
    Color CurrentColour()
    {
        return mesh.material.GetColor("_Color");
    }
    //<Summary>
    //is called from each interactable in their contractually obligated interact() functions
    public void InteractedWith()
    {
        isInteractedWith = !isInteractedWith;
    }
    //<Summary>
    //is called from each interactable in their contractually obligated resethighlighter() functions 
    //-and from this script's local implementation of Load()
    public void Reset()
    {
        isInteractedWith = false;
    }

    public void Save(SaveData data)
    {
        
    }
    public void Load(SaveData data, bool reset = false)
    {
        Reset();
    }
}
