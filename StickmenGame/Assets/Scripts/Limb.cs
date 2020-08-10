using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Class that rappresents a limb
public class Limb : MonoBehaviour
{
    #region Variables
    public Vector2 targetPosition;
    public float force = 150f;
    [Range(0f, 1f)]
    public float ratio = 0.9f;
    public bool isFlipped;
    public bool isVertical;
    private List<LimbPart> limbParts = new List<LimbPart>();
    private Rigidbody2D rootRB;
    #endregion


    private void Awake()
    {
        this.rootRB = GetComponent<Rigidbody2D>();
        HingeJoint2D hinge = GetComponent<HingeJoint2D>();

        while (hinge != null && hinge.transform != this.rootRB)
        {
            limbParts.Add(new LimbPart(hinge.attachedRigidbody, this.isFlipped, this.isVertical));
            hinge = hinge.connectedBody.GetComponent<HingeJoint2D>();
        }
    }


    private void FixedUpdate()
    {
        int index = this.limbParts.Count;
        foreach (LimbPart limbPart in this.limbParts)
        {
            float actualForce = this.force * index * (index + 1) * 0.5f;
            limbPart.RotateTo(this.targetPosition, actualForce, this.ratio);
            index--;
        }
    }


    // DEBUG FUNCTION
    private void OnDrawGizmos()
    {
        foreach (LimbPart limbPart in this.limbParts)
        {
            limbPart.OnDrawGizmos();
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.targetPosition, 0.1f);
    }
}
