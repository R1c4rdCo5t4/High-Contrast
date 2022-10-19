using UnityEngine;
using System.Collections;

public class EnemyFollow : MonoBehaviour
{

	public Transform target;
	public Rigidbody2D rigidBody;
	public float angleChangingSpeed;
	public float movementSpeed;
	private float angle;
	private Vector2 lastPosition;

	

	void FixedUpdate ()
	{
		Vector2 direction = (Vector2)target.position - (Vector2)transform.position;
		angle = Mathf.Atan2 (direction.y, direction.x) * Mathf.Rad2Deg - 90f;
		transform.rotation = Quaternion.Euler (0f, 0f, angle);
		lastPosition = transform.position;
		if (Vector2.Distance (((Vector2)target.position), rigidBody.position) >= 10.0f)
		{
//			Vector2 direction = (Vector2)target.position - rigidBody.position;
//			direction.Normalize ();
//			float rotateAmount = Vector3.Cross (direction, transform.up).z;
//			rigidBody.angularVelocity = -angleChangingSpeed * rotateAmount;
			rigidBody.velocity = transform.up * movementSpeed;
		} else
		{
			rigidBody.velocity = -1 * transform.up * movementSpeed;
		}
	}

	

}