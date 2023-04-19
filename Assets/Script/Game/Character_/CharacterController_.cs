using FrameWork.Core.Attributes;
using UnityEngine;

public class CharacterController_ : MonoBehaviour
{
    [SerializeField, LabelText("移动速度")]
    private float MoveSpeed;

    [SerializeField, LabelText("旋转速度")]
    private float RoteSpeed;

    // 与地面之间的高度大于该值时，认为在空中
    [SerializeField]
    private float m_CheckInAirDistance = 0.6f;

    [SerializeField]
    private float m_GravityFactorWhenRise = 1f;

    [SerializeField]
    private float m_GravityFactorWhenFall = 2f;

    //private CharacterController m_CharacterController;

    private Rigidbody m_Rigidbody;

    private int GroundLayerMask
    {
        get { return 1 << LayerMask.NameToLayer("Ground"); }
    }

    // Start is called before the first frame update
    void Start()
    {
        this.m_Rigidbody = this.transform.GetComponent<Rigidbody>();
        //this.m_CharacterController = this.transform.GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        this.AdjustForwardDir();
        this.AdjustGravity();
    }

    private void FixedUpdate()
    {
        var h = Input.GetAxis("Horizontal");
        var v = Input.GetAxis("Vertical");

        if (h != 0 || v != 0)
        {
            //this.transform.Translate(0, 0, this.m_Ver * this.MoveSpeed * Time.deltaTime);
            //this.transform.Rotate(0, this.m_Hor * this.RoteSpeed, Time.deltaTime, 0);

            var dir = new Vector3(h, 0, v);
            //this.m_CharacterController.Move(dir * this.MoveSpeed * Time.deltaTime);
            //this.transform.rotation = Quaternion.LookRotation(dir);

            this.transform.forward = dir.normalized;
            AdjustForwardDir();
            this.m_Rigidbody.MovePosition(this.transform.position + this.transform.forward * this.MoveSpeed * Time.deltaTime);
        }

        this.AdjustGravity();
    }

    private void AdjustForwardDir()
    {
        var planeNormalDir = Vector3.up;
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1, GroundLayerMask))
        {
            planeNormalDir = hit.normal;
        }
        var forward = Vector3.ProjectOnPlane(transform.forward, planeNormalDir);
        this.transform.rotation = Quaternion.LookRotation(forward, planeNormalDir);
    }

    private void AdjustGravity()
    {
        float gravityFactor = 0;
        if (this.m_Rigidbody.velocity.y > 0)
        {
            gravityFactor = this.m_GravityFactorWhenRise;
        }
        else if (this.m_Rigidbody.velocity.y < 0)
        {
            gravityFactor = this.m_GravityFactorWhenFall;
        }

        if (gravityFactor != 0)
        {
            this.m_Rigidbody.AddForce(Vector3.up * gravityFactor * Physics.gravity.y, ForceMode.Force);
        }
    }
}
