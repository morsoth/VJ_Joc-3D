using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 1.0f;
    [SerializeField] float jumpForce = 1.0f;
    [SerializeField] float gravity = 9.8f;

    public GameObject[] playerSkins;

    public Transform path;
    public TerrainManager terrainManager;

    int index = 0;
    Rigidbody rb;
    bool isGrounded = true;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        //path = GameObject.Find("Path").transform;

        rb.useGravity = false;

        int random = Random.Range(0, playerSkins.Length);
        Instantiate(playerSkins[random], transform.position, Quaternion.identity, this.transform);
    }

    void FixedUpdate()
    {
        if (index > path.childCount - 1)  terrainManager.GenerateLevel();

        Transform point = path.GetChild(index);

        Vector3 currentPositionXZ = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 pointPositionXZ = new Vector3(point.position.x, 0, point.position.z);

        Vector3 moveDirXZ = Vector3.MoveTowards(currentPositionXZ, pointPositionXZ, moveSpeed * Time.fixedDeltaTime);
        rb.MovePosition(new Vector3(moveDirXZ.x, rb.position.y, moveDirXZ.z));

        if (GetDistanceToPointXZ(point) < 0.001f) index++;

        rb.AddForce(Vector3.down * gravity, ForceMode.Acceleration);
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Space) && isGrounded) Jump();
    }

    void Jump()
    {
        rb.linearVelocity = Vector3.zero;
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isGrounded = false;
    }

    float GetDistanceToPointXZ(Transform point)
	{
        float distance = Mathf.Sqrt(Mathf.Pow(transform.position.x - point.position.x, 2) + Mathf.Pow(transform.position.z - point.position.z, 2));

        return distance;
	}

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground")) isGrounded = true;
    }

    void OnTriggerEnter(Collider collider) {
        if (collider.gameObject.CompareTag("Obstacle"))
        {
            Debug.Log("DIE");
            terrainManager.GenerateLevel();
        }
    }
}
