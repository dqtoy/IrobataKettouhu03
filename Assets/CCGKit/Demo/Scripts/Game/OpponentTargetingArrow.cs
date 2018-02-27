using UnityEngine;

public class OpponentTargetingArrow : TargetingArrow
{
    protected override void Update()
    {
    }

    protected override void LateUpdate()
    {
        uvOffset += (uvAnimationRate * Time.deltaTime);
        lineRenderer.material.SetTextureOffset("_MainTex", uvOffset);
    }

    public void SetTarget(GameObject go)
    {
        var pos = go.transform.position;
        UpdateLength(pos);
        CreateTarget(pos);
    }
}