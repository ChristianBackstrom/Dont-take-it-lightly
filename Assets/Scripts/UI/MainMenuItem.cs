using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenuItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
 {
    [SerializeField] float amount = 2f;
    [SerializeField] float speed = 3f;
    public bool mouse_over { get; private set;}

    float initial;
    bool returned = true;
    Quaternion targetAngle;
    Quaternion initialAngle;
    Vector3 initialScale;
    [SerializeField] Vector3 targetScale = new Vector3(1,1,1);

    public virtual void Awake()
    {
        targetAngle = Quaternion.Euler(0,0,transform.rotation.eulerAngles.z - amount);
        initialAngle = transform.rotation;
        initialScale = transform.localScale;
    }
    public virtual void Update()
    {
        if (mouse_over)
        {
            returned = false;
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * speed);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetAngle, Time.deltaTime * speed);
        }
        else if(!returned)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, initialScale, Time.deltaTime * speed);
            transform.rotation = Quaternion.Lerp(transform.rotation, initialAngle, Time.deltaTime * speed);
            if (Mathf.Approximately(transform.rotation.z, initialAngle.z))
            {
                returned = true;
                transform.rotation = initialAngle;
                transform.localScale = initialScale;
            }
        }
    }
 
    public void OnPointerEnter(PointerEventData eventData)
    {
        mouse_over = true;
    }
 
    public void OnPointerExit(PointerEventData eventData)
    {
        mouse_over = false;
    }
 }
