using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMoverP1 : MonoBehaviour
{
    void Update()
    {
        if(Input.GetKey(KeyCode.W)){
            transform.Translate(transform.right * 5f * Time.deltaTime * 1);
        }
        if(Input.GetKey(KeyCode.S)){
            transform.Translate(transform.right * 5f * Time.deltaTime * -1);
        }
        if(Input.GetKey(KeyCode.A)){
            transform.Translate(transform.forward * 5f * Time.deltaTime * 1);
        }
        if(Input.GetKey(KeyCode.D)){
            transform.Translate(transform.forward * 5f * Time.deltaTime * -1);
        }
    }
}
