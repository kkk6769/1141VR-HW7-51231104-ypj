using UnityEngine;

public class Player : MonoBehaviour
{
	[Header("Refs")]
	public Director director;

	[Header("Camera / Look")] 
	public Camera playerCamera; // 若为空会自动找 MainCamera
	public float mouseSensitivity = 150f;
	public float pitchMin = -80f;
	public float pitchMax = 80f;

	[Header("Movement")]
	public float moveSpeed = 5f;
	public float runMultiplier = 1.6f;
	public bool useCharacterControllerIfPresent = true;

	private float pitch;
	private CharacterController controller;

	void Start()
	{
		if (playerCamera == null)
		{
			var cam = Camera.main;
			if (cam != null) playerCamera = cam;
		}
		controller = GetComponent<CharacterController>();
		LockCursor(true);
	}

	void Update()
	{
		HandleMouseLook();
		HandleMove();

		if (Input.GetKeyDown(KeyCode.Escape))
			LockCursor(false);
		if (Input.GetMouseButtonDown(0) && Cursor.lockState != CursorLockMode.Locked)
			LockCursor(true);
	}

	// 角色接到硬币时由 drop 调用
	public void OnCollectCoin(int value)
	{
		director?.AddMoney(value);
	}

	private void HandleMouseLook()
	{
		if (playerCamera == null) return;

		float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
		float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

		// 水平旋转作用在玩家（Yaw）
		transform.Rotate(0f, mouseX, 0f);

		// 垂直旋转作用在相机（Pitch）
		pitch -= mouseY;
		pitch = Mathf.Clamp(pitch, pitchMin, pitchMax);

		Vector3 camEuler = playerCamera.transform.localEulerAngles;
		camEuler.x = pitch;
		// 保持相机与玩家水平对齐
		camEuler.y = 0f;
		camEuler.z = 0f;
		playerCamera.transform.localEulerAngles = camEuler;
	}

	private void HandleMove()
	{
		float x = Input.GetAxisRaw("Horizontal");
		float y = Input.GetAxisRaw("Vertical");
		Vector2 input = new Vector2(x, y).normalized;

		// 基于相机方向的平面移动
		Vector3 fwd = (playerCamera != null ? playerCamera.transform.forward : transform.forward);
		Vector3 right = (playerCamera != null ? playerCamera.transform.right : transform.right);
		fwd.y = 0f; right.y = 0f;
		fwd.Normalize(); right.Normalize();

		Vector3 moveDir = (fwd * input.y + right * input.x);
		float speed = moveSpeed * (Input.GetKey(KeyCode.LeftShift) ? runMultiplier : 1f);

		if (useCharacterControllerIfPresent && controller != null)
		{
			controller.SimpleMove(moveDir * speed);
		}
		else
		{
			transform.position += moveDir * speed * Time.deltaTime;
		}
	}

	private void LockCursor(bool locked)
	{
		Cursor.visible = !locked;
		Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
	}
}

