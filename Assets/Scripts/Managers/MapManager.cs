using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;

public class MapManager : MonoBehaviour, IDataPersistence
{
    public static MapManager Instance { get; private set; }

    [SerializeField] private NavMeshSurface mapGroundNav;

    [Header("Small Buildings")]
    [SerializeField] private Transform[] smallBuildingPoints;
    [SerializeField] private Vector3[] smallBuildingPositions; // this one is for saving, cant serialized tranform
    [SerializeField] private BuildingController[] smallBuildingArray;

    [Header("Extraction Point")]
    [SerializeField] private ExtractionPoint extractionPoint;
    [SerializeField] private Transform[] extractionPoints;

    [SerializeField] private bool dynamicOCC = false;

    private float angle = 0;
    float[] angles = { 0, 90, 180, 270 };

    // array of meshes to diable when too fall from player
    private List<Occludee> occludees;
    public List<Occludee> Occludees => occludees;

    public Transform[] SmallBuildingPoints => smallBuildingPoints;

    private void Awake()
    {
        CharacterSpawner.Instance.SpawnPlayer();
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        occludees = new List<Occludee>();
        smallBuildingPositions = new Vector3[smallBuildingPoints.Length];
        RotateMap();

        // initialize smallBuildingPositions array
        for (int i = 0; i < smallBuildingPoints.Length; i++)
        {
            smallBuildingPositions[i] = smallBuildingPoints[i].position;
        }
    }


    private void Start()
    {
        if (DataPersistenceManager.Instance.IsNewGame || DataPersistenceManager.Instance.InitializeDataIfNull)
        {
            smallBuildingPositions = SuffleVector3Array(smallBuildingPositions);
            MoveBuildingsToPoints(smallBuildingArray, smallBuildingPositions);
            SpawnExtractionPoint();
            Player.Instance.PlayerTransform.position = GetRandomSmallBuilding().PlayerSpawnPoint.position;
            EnemySpawnManager.Instance.SpawnZombieInAreas();
        }
    }

    private void Update()
    {
        if (dynamicOCC && Time.frameCount % 2 == 0) { DoDynamicOcclusion(); }
    }

    private void DoDynamicOcclusion()
    {
        // do occlusion for gameobject inside "Map"
        foreach (Occludee o in occludees)
        {
            o.DoOcclusion();
        }
    }

    private void RotateMap(float _angle = -1)
    {
        angle = _angle == -1 ? angles[Random.Range(0, angles.Length)] : _angle;
        Vector3 currentAngle = transform.localEulerAngles;
        transform.localEulerAngles = new Vector3(currentAngle.x, angle, currentAngle.z);
    }

    private void MoveBuildingsToPoints(BuildingController[] buildings, Vector3[] points)
    {
        if (buildings.Length != points.Length)
        {
            // Debug.Log($"Number of buildings{buildings.Length} is not equal to number of points{points.Length}");
            // return;
        }

        for (int i = 0; i < buildings.Length; i++)
        {
            // buildings[i].transform.position = points[i].position;
            // buildings[i].RotateBuilding(points[i].transform.localEulerAngles.y);
            buildings[i].transform.position = points[i];
            buildings[i].RotateBuilding(GetAngleFromVector(points[i]));
        }

        mapGroundNav.BuildNavMesh();
    }

    private float GetAngleFromVector(Vector3 position)
    {
        for (int i = 0; i < smallBuildingPoints.Length; i++)
        {
            if (smallBuildingPoints[i].position == position) return smallBuildingPoints[i].transform.localEulerAngles.y;
        }
        return -1;
    }

    public BuildingController GetRandomSmallBuilding()
    {
        return smallBuildingArray[Random.Range(0, smallBuildingArray.Length)];
    }

    private Vector3[] SuffleVector3Array(Vector3[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            Vector3 obj = array[i];
            int randomizeArray = Random.Range(0, array.Length);
            array[i] = array[randomizeArray];
            array[randomizeArray] = obj;
        }
        return array;
    }

    private void SpawnExtractionPoint()
    {
        Vector3 pos = extractionPoints[Random.Range(0, extractionPoints.Length)].position;
        extractionPoint.transform.position = pos;
    }

    public void ToggleExtractionPoint(bool on)
    {
        extractionPoint.ToggleOnOff(on);
    }

    public void LoadData(GameData data)
    {
        angle = data.MapAngle;
        smallBuildingPositions = data.SBPositions.ToArray();
        RotateMap(angle);
        MoveBuildingsToPoints(smallBuildingArray, smallBuildingPositions);
        extractionPoint.transform.position = data.ExtractionPointPos;
        extractionPoint.ToggleOnOff(data.IsExtractionPointAvailable);

        EnemySpawnManager.Instance.SpawnZombieInAreas(true);
    }

    public void SaveData(ref GameData data)
    {
        data.MapAngle = angle;
        data.SBPositions = smallBuildingPositions.ToList();
        if (extractionPoint != null)
        {
            data.ExtractionPointPos = extractionPoint.transform.position;
            data.IsExtractionPointAvailable = extractionPoint.gameObject.activeSelf;
        }
    }
}
