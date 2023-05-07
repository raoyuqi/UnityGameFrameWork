using FrameWork.Core.Attributes;
using UnityEngine;

public class CamerFollow : MonoBehaviour
{
    [SerializeField, LabelText("移动速度")]
    private float FollowSpeed;

    [LabelText("跟随目标")]
    public Transform FollowTarget;

    private Vector3 m_Offset;

    // Start is called before the first frame update
    void Start()
    {
        this.m_Offset = this.transform.position - this.FollowTarget.position;
    }

    // Update is called once per frame
    void Update()
    {
        //if (this.FollowTarget != null)
        //    this.transform.position = Vector3.Lerp(this.transform.position, (this.FollowTarget.position + this.m_Offset), this.FollowSpeed * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if (this.FollowTarget != null)
        {
            var targetPos = this.FollowTarget.position + this.m_Offset;
            this.transform.position = Vector3.Lerp(this.transform.position, targetPos, this.FollowSpeed * Time.fixedDeltaTime);
        }
    }
}
