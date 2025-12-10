using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target; // 玩家或骨骼（如头部）

    [Header("Offsets")]
    public Vector3 positionOffset = new Vector3(0f, 1.6f, -3f);
    public Vector3 lookOffset = new Vector3(0f, 1.4f, 0f);

    [Header("Smoothing")]
    public float followLerp = 10f;
    public float rotateLerp = 10f;

    [Header("Use Parenting")]
    public bool attachAsChild = false; // 勾选后会把相机父对象设为 target

    void Start()
    {
        if (attachAsChild && target != null)
        {
            transform.SetParent(target);
            transform.localPosition = positionOffset;
            transform.localRotation = Quaternion.identity;
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        if (!attachAsChild)
        {
            Vector3 desiredPos = target.TransformPoint(positionOffset);
            transform.position = Vector3.Lerp(transform.position, desiredPos, followLerp * Time.deltaTime);

            Vector3 lookPos = target.TransformPoint(lookOffset);
            Quaternion desiredRot = Quaternion.LookRotation((lookPos - transform.position).normalized, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRot, rotateLerp * Time.deltaTime);
        }
        else
        {
            // 作为子物体时，保持局部位移与朝向
            transform.localPosition = Vector3.Lerp(transform.localPosition, positionOffset, followLerp * Time.deltaTime);
        }
    }
}
