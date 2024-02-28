using UnityEngine;
using UnityEngine.Events;

public class Pushable : MonoBehaviour, IHumanInteractable
{

    [Header("Data")]
    [SerializeField]
    private InteractableData data;

    [Header("Base")]
    [SerializeField]
    protected UnityEvent OnClick;


    InteractableHighlighter highlighter;

    private void Start()
    {
        highlighter = GetComponentInChildren<InteractableHighlighter>();
    }

    public virtual bool Interact(Transform interactor)
    {
        if (!IsInteractable(interactor, out float dist))
        {
            return false;
        }

        if (!interactor.GetComponent<Pusher>().Push(this.gameObject)) return false;
        highlighter.InteractedWith();





        OnClick?.Invoke();
        return true;
    }

    public bool IsInteractable(Transform interactor, out float distance)
    {
        distance = Vector3.Distance(interactor.position, transform.position);
        bool dist = distance < data.Range;

        return dist;
    }
    public void Highlight()
    {
        highlighter.currStrength += 8 * Time.fixedDeltaTime;
    }
    public void ResetHighlighter()
    {
        highlighter.Reset();
    }
}
