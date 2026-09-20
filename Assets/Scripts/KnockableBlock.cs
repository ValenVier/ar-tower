using UnityEngine;

public class KnockableBlock : MonoBehaviour
{
    [SerializeField] private float settleDelay = 0.5f;
    [SerializeField] private float toppleAngle = 40f;
    [SerializeField] private float displacementThreshold = 0.15f;

    public event System.Action OnKnocked;

    private Vector3 restPosition;
    private bool isSettled;
    private bool isKnocked;

    private void Start()
    {
        Invoke(nameof(RecordRestState), settleDelay);
    }

    private void RecordRestState()
    {
        restPosition = transform.position;
        isSettled = true;
    }

    private void FixedUpdate()
    {
        if (!isSettled || isKnocked) return;

        float tilt = Vector3.Angle(transform.up, Vector3.up);
        float displacement = Vector3.Distance(transform.position, restPosition);

        if (tilt > toppleAngle || displacement > displacementThreshold)
        {
            Knock();
        }
    }

    public void Knock()
    {
        if (isKnocked) return;
        isKnocked = true;
        OnKnocked?.Invoke();
    }
}
