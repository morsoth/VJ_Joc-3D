using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 1.0f;
    [SerializeField] float jumpForce = 1.0f;
    [SerializeField] float gravity = 9.8f;

    public GameObject[] playerSkins;

    public Transform path;
    public TerrainManager terrainManager;

    int index = 0;
    [SerializeField] float totalPathDistance;
    [SerializeField] float distanceTraveled;

    float traveledPathPercentage;

    Rigidbody rb;
    Animator animator;
    bool isGrounded = true;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        rb.useGravity = false;

        animator = gameObject.GetComponentInChildren<Animator>();

        string playerSkin = PlayerPrefs.GetString("PlayerSkin", "dinosaur");

        for (int i = 0; i < playerSkins.Length; i++)
        {
            if (playerSkins[i].name == playerSkin)
            {
                GameObject skin = Instantiate(playerSkins[i], transform.Find("PlayerAnim"));
                break;
            }
        }

        traveledPathPercentage = 0f;
        distanceTraveled = 0f;
        totalPathDistance = CalculateTotalPathDistance();
    }

    void FixedUpdate()
    {
        if (index > path.childCount - 1)
        {
            terrainManager.NextStage();
            return;
        }

        Transform point = path.GetChild(index);

        Vector3 currentPositionXZ = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 pointPositionXZ = new Vector3(point.position.x, 0, point.position.z);

        Vector3 moveDirXZ = Vector3.MoveTowards(currentPositionXZ, pointPositionXZ, moveSpeed * Time.fixedDeltaTime);
        rb.MovePosition(new Vector3(moveDirXZ.x, rb.position.y, moveDirXZ.z));

        Vector3 directionToPoint = (pointPositionXZ - currentPositionXZ).normalized;
        if (directionToPoint != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToPoint);
            transform.rotation = targetRotation;
        }

        if (GetDistanceToPointXZ(point) < 0.001f)
        {
            index++;
        }

        distanceTraveled = CalculateDistanceTraveled();
        traveledPathPercentage = (distanceTraveled / totalPathDistance) * 100f;
        traveledPathPercentage = Mathf.Clamp(traveledPathPercentage, 0f, 100f);

        terrainManager.UpdatePercentage(traveledPathPercentage);

        rb.AddForce(Vector3.down * gravity, ForceMode.Acceleration);
    }

    void Update()
    {
        if ((Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0)) && isGrounded) {
            Jump();
            AudioManager.instance.PlaySFX(AudioManager.instance.jumpSound);
        }
    }

    void Jump()
    {
        rb.linearVelocity = Vector3.zero;
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isGrounded = false;
        animator.SetTrigger("Jump");
    }

    float GetDistanceXZ(Transform t1, Transform t2)
    {
        float distance = Mathf.Sqrt(Mathf.Pow(t1.position.x - t2.position.x, 2) + Mathf.Pow(t1.position.z - t2.position.z, 2));

        return distance;
    }

    float GetDistanceToPointXZ(Transform point)
	{
        float distance = Mathf.Sqrt(Mathf.Pow(transform.position.x - point.position.x, 2) + Mathf.Pow(transform.position.z - point.position.z, 2));

        return distance;
	}

    float CalculateTotalPathDistance()
    {
        float totalDistance = 0f;

        for (int i = 0; i < path.childCount - 1; i++)
        {
            totalDistance += GetDistanceXZ(path.GetChild(i), path.GetChild(i + 1));
        }

        return totalDistance;
    }

    float CalculateDistanceTraveled()
    {
        float traveled = 0f;

        for (int i = 0; i < index - 1; i++)
        {
            traveled += GetDistanceXZ(path.GetChild(i), path.GetChild(i + 1));
        }
        
        if (index < path.childCount)
        {
            traveled += GetDistanceXZ(transform, path.GetChild(index - 1));
        }
        
        return traveled;
    }

    void Die()
    {
        terrainManager.PlayerDie();
        AudioManager.instance.PlaySFX(AudioManager.instance.deathSound);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle")) 
        {
            Die();
        }
        else if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    void OnTriggerEnter(Collider collider) {
        if (collider.gameObject.CompareTag("Obstacle"))
        {
            Die();
        }
        else if (collider.gameObject.CompareTag("Coin"))
        {
            terrainManager.AddCoin();
            AudioManager.instance.PlaySFX(AudioManager.instance.coinSound);
            Destroy(collider.gameObject);
        }
    }
}
