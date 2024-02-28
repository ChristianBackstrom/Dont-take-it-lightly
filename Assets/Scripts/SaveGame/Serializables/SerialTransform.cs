using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] public class SerialTransform
{
   [SerializeField] public Vector3 position;
   [SerializeField] public Quaternion rotation;

   public SerialTransform(Transform transform, bool overrideHeight = false, float heightValue = 1f)
   {
      position = transform.position;
      rotation = transform.rotation;

      if (overrideHeight)
      {
         position.y = heightValue;
      }
   }

   public SerialTransform(Vector3 pos, Quaternion rot)
   {
      position = pos;
      rotation = rot;
   }

}