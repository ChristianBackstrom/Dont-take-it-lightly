using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(LightReciever))]
public class FinalRoomPillar : MonoBehaviour
{
    private FinalRoomHandler handler;
    private MagicSeal seal;

    private void OnEnable()
    {
        seal = GetComponentInChildren<MagicSeal>();
        handler = FindObjectOfType<FinalRoomHandler>();

        GetComponent<LightReciever>().OnLightReceived.AddListener(Light);
    }

    private void OnDisable()
    {
        GetComponent<LightReciever>().OnLightReceived.RemoveListener(Light);
    }

    [ContextMenu("Shoot")]
    public async void Light()
    {
        await seal.LightItUp();

        handler.ReportingIn(this);
    }

    public async void UnLight()
    {
        await seal.LightItDown();
    }
}
