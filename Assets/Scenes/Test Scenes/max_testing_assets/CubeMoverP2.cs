using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMoverP2 : MonoBehaviour
{
    void Update()
    {
        if(Input.GetKey(KeyCode.UpArrow)){
            transform.Translate(transform.right * 5f * Time.deltaTime * 1);
        }
        if(Input.GetKey(KeyCode.DownArrow)){
            transform.Translate(transform.right * 5f * Time.deltaTime * -1);
        }
        if(Input.GetKey(KeyCode.LeftArrow)){
            transform.Translate(transform.forward * 5f * Time.deltaTime * 1);
        }
        if(Input.GetKey(KeyCode.RightArrow)){
            transform.Translate(transform.forward * 5f * Time.deltaTime * -1);
        }
    }
}
