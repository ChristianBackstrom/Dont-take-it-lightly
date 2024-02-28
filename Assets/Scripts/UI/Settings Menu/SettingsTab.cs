using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsTab : MonoBehaviour
{

    public bool tabEnabled = false;
    bool returned = true;


    /* things set by settingsWindow */
    public int index {private get; set;}
    public SettingsWindow settingsWindow {private get; set;}

    
    /* things for scale function */
    [SerializeField] float speed = 3f;
    Vector3 initialScale;
    [SerializeField] Vector3 targetScale = new Vector3(1.1f,1.1f,1.1f);
    [SerializeField] Sprite selectedSprite;
    private Sprite originalSprite;
    private Image image;



    public virtual void Awake()
    {
        initialScale = transform.localScale;
        image = GetComponent<Image>();
        if (image == null) return;
        originalSprite = image.sprite;
    }

    public virtual void Update()
    {
        if (tabEnabled)
        {
            if (image.sprite != selectedSprite) image.sprite = selectedSprite;
            returned = false;
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.unscaledDeltaTime * speed);
        }
        else if(!returned)
        {
            if (image.sprite != originalSprite) image.sprite = originalSprite;
            transform.localScale = Vector3.Lerp(transform.localScale, initialScale, Time.unscaledDeltaTime * speed);
            if (Mathf.Approximately(transform.localScale.z, initialScale.z))
            {
                returned = true;
                transform.localScale = initialScale;
            }
        }
    }

    public void Clicked()
    {
        if (tabEnabled) return;
        settingsWindow.ChangeTab(index);
    }
}
