using System.Collections;
using System.Collections.Generic;
using FruitCasual;
using UnityEngine;

public class MainManager : MonoBehaviour
{
    public static MainManager Instance { get; private set; }

    [Header("Config Mergeable Object")]
    [SerializeField] private MergeableConfig fruitConfig;
    [SerializeField] private MapConfig mapConfig; // Temparature none

    [Header("Fruit Setup")]
    [SerializeField] private int RandomFruit;
    [SerializeField] private GameObject currentFruit;
    [SerializeField] private GameObject nextFruit;
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform fruitContainer;
    [SerializeField] private Transform upgradeContainer;

    [Header("Pooling Object Setting")]
    [SerializeField] private int numPool;
    [SerializeField] private List<GameObject> poolingObjects;


    [Header("Particle System")]
    [SerializeField] private ParticleManager particleManager;

    private GameObject instantGO;
    private Rigidbody2D rb;
    private Vector3 initialMousePos;
    private Vector3 mousePosition;
    private static int mergeCount = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    // Start is called before the first frame update
    void Start()
    {
        InitPooling();
        PickFruitRandom();
        SetupFruit();
    }

    private void Update()
    {
        Controller();
    }
    private void InitPooling()
    {
        int fruitEach = numPool / fruitConfig.objectMerges.Length;
        for (int i = 0; i < fruitConfig.objectMerges.Length; i++)
        {
            for (int j = 0; j < fruitEach; j++)
            {
                instantGO = Instantiate(fruitConfig.objectMerges[i], fruitContainer);
                instantGO.SetActive(false);
                poolingObjects.Add(instantGO);
            }
        }
    }

    private void PickFruitRandom()
    {
        currentFruit = GetFruitRandom();
        nextFruit = GetFruitRandom();
    }

    private void PickNextFruitRandom()
    {
        currentFruit = nextFruit;
        nextFruit = GetFruitRandom();
        SetupFruit();
    }

    private void SetupFruit()
    {
        currentFruit.transform.position = startPoint.transform.position;
        TurnOffCurrentFruitGravity();
        currentFruit.SetActive(true);
    }

    private GameObject GetFruitRandom()
    {
        int randNum = Random.Range(0, poolingObjects.Count);
        GameObject randomFruit = poolingObjects[randNum];
        poolingObjects.RemoveAt(randNum);
        return randomFruit;
    }

    private void ResetHasMerged()
    {
        Level level = currentFruit.GetComponent<Level>();
        level.hasMerged = false;
    }

    private void Controller()
    {
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            initialMousePos = Input.mousePosition;
        }

        if (Input.GetMouseButton(0))
        {

            mousePosition.z = 0f;

            currentFruit.transform.position = new Vector3(mousePosition.x, currentFruit.transform.position.y);

            Debug.Log("MousePosition: " + mousePosition);
        }

        if (Input.GetMouseButtonUp(0))
        {
            TurnOnCurrentFruitGravity();
            PickNextFruitRandom();
        }
    }
    private void TurnOffCurrentFruitGravity()
    {
        rb = currentFruit.GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
    }
    private void TurnOnCurrentFruitGravity()
    {
        rb = currentFruit.GetComponent<Rigidbody2D>();
        rb.gravityScale = 1;
    }
    public void UpgradeObject(GameObject fruitMerged1, GameObject fruitMerged2, GameObject upgradeObject)
    {
        mergeCount++;
        if (mergeCount == 2)
        {
            Vector3 middlePosition = Vector3.Lerp(fruitMerged1.transform.position, fruitMerged2.transform.position, 0.5f);


            GameObject newLevelObject = Instantiate(upgradeObject, middlePosition, Quaternion.identity, upgradeContainer).gameObject;

            // Check if the cast is successful
            if (newLevelObject != null)
            {
                poolingObjects.Add(fruitMerged1);
                poolingObjects.Add(fruitMerged2);
                mergeCount = 0;
            }
            else
            {
                // Handle the case where the cast is not successful
                Debug.LogError("Failed to cast instantiated object to GameObject.");
            }
        }
        Debug.Log("Upgrade Object Type: " + upgradeObject.GetType());
    }
}
