using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Class that rappresents a part of a limb
public class LimbPart
{
    #region Variables
    private Rigidbody2D rigidbody;
    private Transform transform;
    private HingeJoint2D hingeJoint;
    private Vector2 offset;
    private Vector2 relativeOffset => this.offset.x * this.transform.right + this.offset.y * this.transform.up;
    private Vector2 avgTargetPosition;
    private Vector2 avgWorldPosition;
    private float worldRatio;
    private float deltaPosition;
    private bool isFlipped;
    private bool isVertical;
    #endregion


    // Constructor for the class LimbPart
    public LimbPart(Rigidbody2D rigidbody, bool isFlipped, bool isVertical)
    {
        this.rigidbody = rigidbody;
        this.transform = this.rigidbody.transform;
        this.hingeJoint = this.rigidbody.GetComponent<HingeJoint2D>();
        this.offset = this.hingeJoint.anchor;
        this.worldRatio = 0.95f;

        // Bool value to check if the arm is flipped or is positioned vertically
        this.isFlipped = isFlipped;
        this.isVertical = isVertical;
    }


    // Function that rotate a limb part toward a position
    public void RotateTo(Vector2 worldPosition, float force, float ratio)
    {
        // Finding how much has the world position moved
        this.avgWorldPosition = worldPosition * (1 - this.worldRatio) + this.avgWorldPosition * this.worldRatio;
        this.deltaPosition = (worldPosition - this.avgWorldPosition).magnitude > 1 ? 0f : 1 - (worldPosition - this.avgWorldPosition).magnitude;

        // Finding the target position relative to the root position
        Vector2 targetPosition = worldPosition - ((Vector2)this.transform.position + this.relativeOffset);
        targetPosition = this.isFlipped ? targetPosition * -1 : targetPosition;

        // Modifing the ratio based on the movement of the world position
        ratio = (1 - ratio) * this.deltaPosition * 0.95f;
        this.avgTargetPosition = targetPosition * (1 - ratio) + this.avgTargetPosition * ratio;

        // Finding the target angle and transforming it in case it's in a vertical position
        float targetAngle = FindAngle(this.avgTargetPosition);
        float referenceAngle = this.isVertical ? this.transform.eulerAngles.z - 90 : this.transform.eulerAngles.z;
        targetAngle = Mathf.DeltaAngle(referenceAngle, targetAngle);

        int sign = targetAngle > 0 ? 1 : -1;
        targetAngle = Mathf.Abs(targetAngle * 0.1f);
        targetAngle = targetAngle > 2 ? 2 : targetAngle;
        targetAngle *= 0.5f;
        targetAngle *= (1 + targetAngle);

        this.rigidbody.AddTorque(targetAngle * force * Time.deltaTime * sign);
    }


    // Function that finds the angle of a given vector
    private float FindAngle(Vector2 position)
    {
        float angle = Mathf.Acos(position.normalized.x) * Mathf.Rad2Deg;
        return position.normalized.y > 0 ? angle : 360f - angle;
    }

    // DEBUG FUNCTION

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere((Vector2)this.transform.position + this.relativeOffset, 0.1f);

        // Gizmos.color = Color.blue;
        // Gizmos.DrawWireSphere((Vector2)this.transform.position - this.relativeOffset, 0.1f);
    }
}
