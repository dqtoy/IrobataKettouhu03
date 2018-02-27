using UnityEngine;

public class BoardSpell : MonoBehaviour
{
    public TargetingArrow targetingArrow;

    private void OnDestroy()
    {
        if (targetingArrow != null)
        {
            Destroy(targetingArrow.gameObject);
            targetingArrow = null;
        }
    }
}