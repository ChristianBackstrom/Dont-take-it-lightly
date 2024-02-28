using UnityEngine;

[CreateAssetMenu(menuName = "Movement/Data")]
public class MovementData : ScriptableObject
{
	public float Speed;
	public float RotationSmoothingSpeed;
}