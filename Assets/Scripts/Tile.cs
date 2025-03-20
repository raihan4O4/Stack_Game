using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField]
    private GameObject slicedTile;
    private float distance;
    private float maxDistance;
    private float stepLength;
    private bool moveForward;
    public bool moveX;


    private void Start()
    {
        maxDistance = 5.0f;
        distance = maxDistance;

        moveForward = false;

        if (moveX)
            transform.Translate(distance, 0, 0);
        else
            transform.Translate(0, 0, distance);
    }

    private void Update()
    {
        stepLength = Time.deltaTime * 6.0f;
        if (moveX)
            MoveAtX();
        else
            MoveAtZ();
    }


    void MoveAtX()
    {
        if (moveForward)
        {
            if (distance < maxDistance)
            {
                transform.Translate(stepLength, 0, 0);
                distance += stepLength;
            }
            else
            {
                moveForward = false;
            }
        }
        else
        {
            if (distance > -maxDistance)
            {
                transform.Translate(-stepLength, 0, 0);
                distance -= stepLength;
            }
            else
            {
                moveForward = true;
            }
        }
    }
    void MoveAtZ()
    {
        if (moveForward)
        {
            if (distance < maxDistance)
            {
                transform.Translate(0, 0, stepLength);
                distance += stepLength;
            }
            else
            {
                moveForward = false;
            }
        }
        else
        {
            if (distance > -maxDistance)
            {
                transform.Translate(0, 0, -stepLength);
                distance -= stepLength;
            }
            else
            {
                moveForward = true;
            }
        }

    }
    public void ScaleTile()
    {
        if (Mathf.Abs(distance) > 0.2f)
        {
            float slicedLength = Mathf.Abs(distance);

            if(moveX)
            {
                if (transform.localScale.x < slicedLength)
                {
                    Rigidbody rb = gameObject.AddComponent<Rigidbody>();
                    rb.useGravity = false; 
                    rb.mass = 0.5f;
                    rb.linearDamping = 0.2f; 
                    rb.angularDamping = 0.05f;
                    float rotationForce = 0.5f;
                    Vector3 gravity = Physics.gravity * 0.10f;
                    float randomX = Random.Range(-rotationForce, rotationForce);
                    float randomY = Random.Range(-rotationForce, rotationForce);
                    float randomZ = Random.Range(-rotationForce, rotationForce);
                    rb.AddTorque(new Vector3(randomX, randomY, randomZ), ForceMode.Impulse);
                    TileSpawner.Instance.GameOver();
                    Destroy(gameObject, 3f);
                    return;
                }
                GameObject _slicedTile = Instantiate(slicedTile);
                _slicedTile.transform.localScale = new Vector3(slicedLength, transform.localScale.y, transform.localScale.z);
                _slicedTile.transform.position = new Vector3(transform.position.x + (distance > 0 ? 1 : -1) * (transform.localScale.x - slicedLength)/2, 
                    transform.position.y, transform.position.z);
                _slicedTile.GetComponent<Renderer>().sharedMaterial.color = GetComponent<Renderer>().sharedMaterial.color;

                transform.localScale -= new Vector3(slicedLength, 0, 0);
                transform.Translate((distance > 0 ? -1 : 1) * slicedLength / 2, 0, 0);

            }
            else
            {
                if (transform.localScale.z < slicedLength)
                {
                    gameObject.AddComponent<Rigidbody>();
                    TileSpawner.Instance.GameOver();
                    return;
                }
                GameObject _slicedTile = Instantiate(slicedTile);
                _slicedTile.transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, slicedLength);
                _slicedTile.transform.position = new Vector3(transform.position.x, transform.position.y, 
                    transform.position.z + (distance > 0 ? 1 : -1) * (transform.localScale.x - slicedLength) / 2);
                _slicedTile.GetComponent<Renderer>().sharedMaterial.color = GetComponent<Renderer>().sharedMaterial.color;


                transform.localScale -= new Vector3(0, 0, slicedLength);
                transform.Translate(0, 0, (distance > 0 ? -1 : 1) * slicedLength / 2);
            }
        }
        //perfect
        else
        {
            // Perfect placement
            if (moveX)
                transform.Translate(-distance, 0, 0);
            else
                transform.Translate(0, 0, -distance);
            
            // Spawn perfect effect
            if (TileSpawner.Instance != null)
            {
                // Offset the effect position slightly above the tile to make it visible
                Vector3 effectPosition = transform.position + new Vector3(0, 1.5f, 0);
                TileSpawner.Instance.SpawnPerfectEffect(effectPosition);
            }
        }

        Destroy(this);
    }
}
