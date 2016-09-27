using UnityEngine;
using System.Collections;


public class CameraManager : MonoBehaviour {
    public static CameraManager instance;
    private Transform myTransform;

    //TP_CAMERA

    public Transform TargetLookAt;

    public float Distance = 5f;
    public float DistanceMin = 3f;
    public float DistanceMax = 10f;
    public float DistanceSmooth = 0.05f;
    public float X_MouseSensitivity = 5f;
    public float Y_MouseSensitivity = 5f;
    public float MouseWheelSensitivity = 5f;
    public float X_Smooth = 0.05f;
    public float Y_Smooth = 0.1f;
    public float Y_MinLimit = -40f;
    public float Y_MaxLimit = 80f;

    private float mouseX = 0f;
    private float mouseY = 0f;
    private float velX = 0f;
    private float velY = 0f;
    private float velZ = 0f;
    private float velDistance = 0f;
    private float startDistance = 0f;
    private Vector3 position = new Vector3(0.02f, 5.49f, 1.5f);
    private Vector3 desiredPosition = new Vector3(0.02f, 5.49f, 1.5f);
    private float desiredDistance = 0f;




    // Use this for initialization
    void Awake () {
        instance = this;
        myTransform = this.transform;
        Distance = Mathf.Clamp(Distance, DistanceMin, DistanceMax);
        startDistance = Distance;
        Reset();
    }
	
	// Update is called once per frame
	void LateUpdate () {

        if (LevelManager.instance == null) return;

        switch (LevelManager.instance.GetEstadoAtual()) {
            case (EstadoAtual.INPUT):

                //Debug.Log("InputCamera");
                HandlePlayerInput();

                CalculateDesiredPosition();

                UpdatePosition();

                break;
        }
	}



    void HandlePlayerInput()
    {
        var deadZone = 0.01f;

        if (Input.GetMouseButton(1))
        {
            //Debug.Log("Pressionado");
            mouseX += Input.GetAxis("Mouse X") * X_MouseSensitivity;
            mouseY -= Input.GetAxis("Mouse Y") * Y_MouseSensitivity;
        }

        // Limitar a rotação do mouseY
        mouseY = Helper.ClampAngle(mouseY, Y_MinLimit, Y_MaxLimit);


        if (Input.GetAxis("Mouse ScrollWheel") < -deadZone || Input.GetAxis("Mouse ScrollWheel") > deadZone)
        {
            desiredDistance = Mathf.Clamp(Distance - Input.GetAxis("Mouse ScrollWheel") * MouseWheelSensitivity, DistanceMin, DistanceMax);
        }
    }

    void CalculateDesiredPosition()
    {

        Distance = Mathf.SmoothDamp(Distance, desiredDistance, ref velDistance, DistanceSmooth);

        desiredPosition = CalculatePosition(mouseY, mouseX, Distance);

    }

    Vector3 CalculatePosition(float rotationX, float rotationY, float distance)
    {

        Vector3 direction = new Vector3(0, 0, -distance);
        Quaternion rotation = Quaternion.Euler(rotationX, rotationY, 0);

        return TargetLookAt.position + rotation * direction;
    }

    void UpdatePosition()
    {
        var posX = Mathf.SmoothDamp(position.x, desiredPosition.x, ref velX, X_Smooth);
        var posY = Mathf.SmoothDamp(position.y, desiredPosition.y, ref velY, Y_Smooth);
        var posZ = Mathf.SmoothDamp(position.z, desiredPosition.z, ref velZ, X_Smooth);
        position = new Vector3(posX, posY, posZ);

        myTransform.position = position;

        myTransform.LookAt(TargetLookAt);

    }

    public void Reset()
    {
        mouseX = 0;
        mouseY = 10;
        Distance = startDistance;
        desiredDistance = Distance;
    }

    public static void UseExistingOrCreateNewMainCamera()
    {
        GameObject tempCamera;

        if (Camera.main != null)
        {
            tempCamera = Camera.main.gameObject;
        }
    }


}
