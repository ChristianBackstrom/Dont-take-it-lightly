using UnityEngine;
using UnityEngine.Events;

public class Moveable : MonoBehaviour, IHumanInteractable
{
    [Header("Data")]
    [SerializeField]
    private InteractableData data;

    [Header("Base")]
    [SerializeField]
    protected UnityEvent OnClick;

    private Transform interactor;
    private Transform model;

    private Vector3 offset;

    private bool cooledDowned = true;
    private bool isMoving = false;
    private float timer = 0;

    private void Awake()
    {
        model = transform.GetChild(0);
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

        HandleMovement();
    }

    private void HandleMovement()
    {
        if (!isMoving) return;

        Vector3 moveablePosition = interactor.position + interactor.forward;
        moveablePosition.y = transform.position.y;

        transform.position = moveablePosition;

        SnapGhost();
    }

    private void SnapGhost()
    {
        int x = Mathf.RoundToInt(this.transform.position.x);
        int z = Mathf.RoundToInt(this.transform.position.z);

        model.position = new Vector3(x, model.position.y, z);
    }

    public void BindToPlayer(Transform interactor)
    {
        this.interactor = interactor;
        offset = interactor.position - transform.position;
        offset.y = 0;

        model.GetComponent<Collider>().enabled = false;
        isMoving = true;
    }

    public void UnBind()
    {

        isMoving = false;
        model.GetComponent<Collider>().enabled = true;
    }



    public virtual bool Interact(Transform interactor)
    {
        if (!IsInteractable(interactor, out float dist))
        {
            return false;
        }

        OnClick?.Invoke();

        if (!isMoving)
        {
            BindToPlayer(interactor);
        }
        else
        {
            UnBind();
        }

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