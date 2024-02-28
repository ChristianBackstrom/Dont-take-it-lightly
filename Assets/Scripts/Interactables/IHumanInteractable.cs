using UnityEngine;

public interface IHumanInteractable
{
    public bool IsInteractable(Transform interactor, out float distance);
    public bool Interact(Transform interactor);
    public void Highlight();
    public void ResetHighlighter();
}
