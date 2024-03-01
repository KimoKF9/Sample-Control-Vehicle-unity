using UnityEngine;
using System.Collections;

public class VehicleCameraControl : MonoBehaviour
{
	private Rigidbody playerRigid;
	public Transform playerCar;

	[System.Serializable]
    public class followVehicle{
		public float distance = 10.0f;
		public float height = 5.0f;
		public float defaultHeight = 0f;
		public float heightDamping = 2.0f;
		public float rotationDamping = 3.0f;
    }
	[System.Serializable]
    public class rotatecamera{
	//	public float Axis_x;
	  //  public float Axis_y;
	  //  public float sensitivity =1f;
	 //   public Vector3 rotate;
    }
	public followVehicle followVehicles;
	//public rotatecamera RotateCamera;

	

	
	void Start(){
		
		// Early out if we don't have a target
		if (!playerCar)
			return;
		playerRigid = playerCar.GetComponent<Rigidbody>();


		// Cursor.lockState = CursorLockMode.Locked;
		// StartCoroutine(Camera_Coroutine());
		
		 
	}
	
	void FixedUpdate(){
			//////////////
			// Early out if we don't have a target
		if (!playerCar || !playerRigid)
			return;

		//calculates speed in local space. positive if going forward, negative if reversing
		float speed = (playerRigid.transform.InverseTransformDirection(playerRigid.velocity).z) * 3f;
		
		// Calculate the current rotation angles.
		Vector3 wantedRotationAngle = playerCar.eulerAngles;
		float wantedHeight = playerCar.position.y + followVehicles.height;
		float currentRotationAngle = transform.eulerAngles.y;
		float currentHeight = transform.position.y;

		
		if(speed < -5)
			wantedRotationAngle.y = playerCar.eulerAngles.y + 180;
		
		// Damp the rotation around the y-axis
		currentRotationAngle = Mathf.LerpAngle (currentRotationAngle, wantedRotationAngle.y, followVehicles.rotationDamping * Time.deltaTime);
		
		// Damp the height
		currentHeight = Mathf.Lerp (currentHeight, wantedHeight, followVehicles.heightDamping * Time.deltaTime);
		
		// Convert the angle into a rotation
		Quaternion currentRotation = Quaternion.Euler (0, currentRotationAngle, 0);
		
		// Set the position of the camera on the x-z plane to:
		// distance meters behind the target
		transform.position = playerCar.position;
		transform.position -= currentRotation * Vector3.forward * followVehicles.distance;
	
		// Set the height of the camera
		transform.position = new Vector3(transform.position.x, currentHeight + followVehicles.defaultHeight, transform.position.z);

		// Always look at the target
		transform.LookAt (playerCar);

		//RotateCamera.Axis_y = Input.GetAxis("1");
		//RotateCamera.Axis_x = Input.GetAxis("2");
		//RotateCamera.rotate = new Vector3(0,RotateCamera.Axis_y  ,0);
		//transform.eulerAngles = transform.eulerAngles - RotateCamera.rotate ;
		//yield return new WaitForSeconds(4);
		//Debug.Log("Camera New");


		/*IEnumerator Camera_Coroutine(){
			Debug.Log("Camera old" +Time.deltaTime);
			yield return new WaitForSeconds(4);
			Debug.Log("Camera New");
			//RotateCamera.Axis_y = 0;
		}*/

	}
	
	


}