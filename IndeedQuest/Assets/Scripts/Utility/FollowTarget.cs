using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    private Transform _cachedTransform;

    public Transform Target;

    public Vector3 Offset;

    // Start is called before the first frame update
    private void Start()
    {
        _cachedTransform = transform;
    }

    private void LateUpdate()
    {
        _cachedTransform.position = Target.position + Offset;
        _cachedTransform.LookAt(Target);
    }
}
