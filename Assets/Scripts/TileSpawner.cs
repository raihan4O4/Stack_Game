using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TileSpawner : MonoBehaviour
{
    public static TileSpawner Instance;

    [SerializeField] private GameObject tile;
    [SerializeField] private GameObject bottomTile;
    [SerializeField] private GameObject GameOverPanel;
    [SerializeField] private GameObject StartPanel;
    [SerializeField] private GameObject camera;
    [SerializeField] private List<GameObject> perfectEffect;

    public TMP_Text Score;
    private List<GameObject> stack;
    private bool hasGameStarted;
    private bool hasGameFinished;
    private int modifier;
    private int colorIndex;

    List<Color32> colorSpectrum = new()
{
    new Color32(255, 102, 153, 255),  // Vibrant Pink
    new Color32(255, 128, 170, 255),  // Rose Pink
    new Color32(255, 153, 179, 255),  // Salmon Pink
    new Color32(255, 179, 128, 255),  // Peach
    new Color32(255, 204, 102, 255),  // Goldenrod
    new Color32(255, 255, 153, 255),  // Canary Yellow
    new Color32(204, 255, 153, 255),  // Lime Sherbet
    new Color32(153, 255, 204, 255),  // Mint Green
    new Color32(128, 255, 229, 255),  // Aquamarine
    new Color32(102, 204, 255, 255),  // Sky Blue
    new Color32(153, 179, 255, 255),  // Periwinkle
    new Color32(204, 153, 255, 255),  // Lavender
    new Color32(255, 153, 255, 255),  // Orchid
    new Color32(255, 102, 204, 255)   // Fuchsia
};


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        stack = new List<GameObject>();
        hasGameFinished = false;
        hasGameStarted = true;

        modifier = 1;
        colorIndex = 0;

        stack.Add(bottomTile);
        stack[0].GetComponent<Renderer>().material.color = colorSpectrum[0];
        SpawnTile();    
    }

    private void Update()
    {
        if (hasGameFinished || !hasGameStarted) return;
        if(Input.GetMouseButtonDown(0))
        {

            if(stack.Count > 1)
            {
                stack[^1].GetComponent<Tile>().ScaleTile();
            }

            if (hasGameFinished) return;
            StartCoroutine(MoveCamera());

            Score.text = (stack.Count-1).ToString();
            SpawnTile();
        }
    }

    private void SpawnTile()
    {
        GameObject belowTile = stack[^1];;

        GameObject activeTile = Instantiate(tile);
        stack.Add(activeTile);

        if(stack.Count > 2)
        {
            activeTile.transform.localScale = belowTile.transform.localScale;
        }

        activeTile.transform.position = new Vector3(belowTile.transform.position.x,
            belowTile.transform.position.y + belowTile.transform.localScale.y, belowTile.transform.position.z);

        colorIndex += modifier;
        if(colorIndex == colorSpectrum.Count || colorIndex == -1)
        {
            modifier *= -1;
            colorIndex += 2 * modifier;

        }

        activeTile.GetComponent<Renderer>().material.color = colorSpectrum[colorIndex];
        activeTile.GetComponent<Tile>().moveX = stack.Count % 2 == 0;
    }

    IEnumerator MoveCamera()
    {
        float moveLength = 1f;

        // Ensure SkyboxRotator component exists
        if (!camera.GetComponent<SkyboxRotator>())
        {
            camera.AddComponent<SkyboxRotator>();
        }

        while(moveLength > 0f)
        {
            float stepLength = 0.05f;
            moveLength -= stepLength;
            camera.transform.Translate(0, stepLength, 0, Space.World);
            yield return new WaitForSeconds(0.05f);
        }
    }

    public void GameOver()
    {
        GameOverPanel.SetActive(true);
        hasGameFinished = true;
        StartCoroutine(EndCamera());
    }

    IEnumerator EndCamera()
    {
        Vector3 temp = camera.transform.position;
        Vector3 final = new Vector3(temp.x, temp.y - stack.Count * 0.4f, temp.z);
        float cameraSizeFinal = stack.Count * 0.65f;
        while (camera.GetComponent<Camera>().orthographicSize < cameraSizeFinal)
        {
            camera.GetComponent<Camera>().orthographicSize += 0.2f;
            temp = camera.transform.position;
            temp = Vector3.Lerp(temp, final, 0.2f);
            camera.transform.position = temp;
            yield return new WaitForSeconds(0.01f);
        }
        camera.transform.position = final;
    }

    public void StartButton()
    {
        if(hasGameFinished)
        {
            SceneManager.LoadScene(0);
        }
        else
        {

            StartPanel.SetActive(false);
            hasGameStarted = true;
        }
    }

    public void SpawnPerfectEffect(Vector3 position)
    {
        if (perfectEffect.Count == 0) return;
        int randomIndex = Random.Range(0, perfectEffect.Count);
        GameObject effect = Instantiate(perfectEffect[randomIndex], position, Quaternion.identity);
        Destroy(effect, 4f);
    }
}
