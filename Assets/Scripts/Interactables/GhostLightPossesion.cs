using UnityEngine;
using UnityEngine.Events;

public class GhostLightPossesion : MonoBehaviour, IGhostInteractable
{
    [Header("Data")]
    [SerializeField]
    private InteractableData data;

    [Header("Base")]
    [SerializeField]
    protected UnityEvent OnClick;

    private LightEmitter light;

    private bool cooledDowned = true;
    private float timer = 0;

    private void Start()
    {
        light = GetComponent<LightEmitter>();
    }

    private void Update()
    {
        if (!cooledDowned)
        {
            timer += Time.deltaTime;
            if (timer > data.Cooldown)
            {
                timer = 0;
                cooledDowned = true;
            }
        }
    }

    public virtual bool Interact(Transform interactor)
    {
        if (!IsInteractable(interactor, out float dist))
        {
            return false;
        }

        interactor.GetComponent<GhostPossesion>().Possess(gameObject, false, false);
        light.ShootLightCallback(interactor.GetComponent<GhostPossesion>().StopPossesion);

        OnClick?.Invoke();
        cooledDowned = false;
        return true;
    }

    public bool IsInteractable(Transform interactor, out float distance)
    {
        distance = Vector3.Distance(interactor.position, transform.position);
        bool dist = distance < data.Range;

        return dist && cooledDowned;
    }
    public void Highlight()
    {

    }
    public void ResetHighlighter()
    {
        
    }
}
