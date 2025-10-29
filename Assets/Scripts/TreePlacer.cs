using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

public class TreePlacer : MonoBehaviour
{
    [Header("Tree Placement Settings")]
    public Terrain targetTerrain;
    public float minDistanceBetweenTrees = 2.0f;
    public int maxPlacementAttempts = 100;

    [Header("Tree Types and Probabilities")]
    public TreeType[] treeTypes = new TreeType[5];

    [Header("Placement Area")]
    public bool useCustomArea = false;
    public Vector2 customAreaStart = Vector2.zero;
    public Vector2 customAreaSize = new Vector2(100, 100);

    [System.Serializable]
    public class TreeType
    {
        public string treeName;
        public GameObject treePrefab;
        [Range(0, 1)]
        public float probability = 0.2f;
    }

    // Public method that can be called from editor buttons
    public void MassPlaceTrees()
    {
        if (targetTerrain == null)
        {
            targetTerrain = GetComponent<Terrain>();
            if (targetTerrain == null)
            {
                Debug.LogError("No terrain found! Please assign a terrain.");
                return;
            }
        }

        // Validate tree types
        if (!ValidateTreeTypes())
            return;

        ClearExistingTrees();
        PlaceTreesWithSpacing();

        Debug.Log("Tree placement completed!");
    }

    // Public method for clearing trees
    public void ClearExistingTrees()
    {
        if (targetTerrain != null)
        {
            targetTerrain.terrainData.treeInstances = new TreeInstance[0];
            Debug.Log("All trees cleared from terrain.");
        }
    }

    private bool ValidateTreeTypes()
    {
        float totalProbability = 0f;

        for (int i = 0; i < treeTypes.Length; i++)
        {
            if (treeTypes[i].treePrefab == null)
            {
                Debug.LogError($"Tree prefab is missing for tree type: {treeTypes[i].treeName}");
                return false;
            }

            if (string.IsNullOrEmpty(treeTypes[i].treeName))
            {
                treeTypes[i].treeName = $"TreeType_{i}";
            }

            totalProbability += treeTypes[i].probability;
        }

        if (totalProbability <= 0)
        {
            Debug.LogError("Total probability must be greater than 0!");
            return false;
        }

        return true;
    }

    private void PlaceTreesWithSpacing()
    {
        TerrainData terrainData = targetTerrain.terrainData;
        List<TreeInstance> treeInstances = new List<TreeInstance>();
        List<Vector3> placedPositions = new List<Vector3>();

        // Get placement area bounds
        Vector3 terrainSize = terrainData.size;
        Vector2 startPos, size;

        if (useCustomArea)
        {
            startPos = customAreaStart;
            size = customAreaSize;
        }
        else
        {
            startPos = Vector2.zero;
            size = new Vector2(terrainSize.x, terrainData.size.z);
        }

        // Try to place trees
        int treesPlaced = 0;
        int attempts = 0;

        while (attempts < maxPlacementAttempts && treesPlaced < CalculateMaxTrees(terrainData))
        {
            Vector3 randomPosition = GetRandomPositionOnTerrain(terrainData, startPos, size);

            if (IsPositionValid(randomPosition, placedPositions, minDistanceBetweenTrees))
            {
                TreeType selectedTreeType = GetRandomTreeType();
                TreeInstance treeInstance = CreateTreeInstance(randomPosition, selectedTreeType, terrainData);

                treeInstances.Add(treeInstance);
                placedPositions.Add(randomPosition);
                treesPlaced++;
            }

            attempts++;
        }

        // Apply the tree instances to the terrain
        terrainData.treeInstances = treeInstances.ToArray();
        terrainData.RefreshPrototypes();

        Debug.Log($"Successfully placed {treesPlaced} trees with proper spacing.");
    }

    private Vector3 GetRandomPositionOnTerrain(TerrainData terrainData, Vector2 start, Vector2 size)
    {
        float x = Random.Range(start.x, start.x + size.x);
        float z = Random.Range(start.y, start.y + size.y);
        float y = terrainData.GetHeight(Mathf.RoundToInt(x), Mathf.RoundToInt(z)) / terrainData.size.y;

        return new Vector3(x / terrainData.size.x, y, z / terrainData.size.z);
    }
    private bool IsPositionValid(Vector3 newPosition, List<Vector3> existingPositions, float minDistance)
    {
        foreach (Vector3 existingPos in existingPositions)
        {
            // Convert to world space distance
            float distance = Vector3.Distance(newPosition, existingPos);
            if (distance < minDistance)
            {
                return false;
            }
        }
        return true;
    }

    private TreeType GetRandomTreeType()
    {
        float randomValue = Random.Range(0f, 1f);
        float cumulativeProbability = 0f;

        foreach (TreeType treeType in treeTypes)
        {
            cumulativeProbability += treeType.probability;
            if (randomValue <= cumulativeProbability)
            {
                return treeType;
            }
        }

        // Fallback to first tree type
        return treeTypes[0];
    }

    private TreeInstance CreateTreeInstance(Vector3 position, TreeType treeType, TerrainData terrainData)
    {
        // Find the tree prototype index
        int prototypeIndex = -1;
        for (int i = 0; i < terrainData.treePrototypes.Length; i++)
        {
            if (terrainData.treePrototypes[i].prefab == treeType.treePrefab)
            {
                prototypeIndex = i;
                break;
            }
        }

        // If prototype doesn't exist, add it
        if (prototypeIndex == -1)
        {
            prototypeIndex = AddTreePrototype(treeType.treePrefab, terrainData);
        }

        TreeInstance treeInstance = new TreeInstance();
        treeInstance.position = position;
        treeInstance.prototypeIndex = prototypeIndex;
        treeInstance.widthScale = Random.Range(0.8f, 1.2f);
        treeInstance.heightScale = Random.Range(0.8f, 1.2f);
        treeInstance.color = Color.white;
        treeInstance.lightmapColor = Color.white;

        return treeInstance;
    }

    private int AddTreePrototype(GameObject treePrefab, TerrainData terrainData)
    {
        TreePrototype[] existingPrototypes = terrainData.treePrototypes;
        TreePrototype[] newPrototypes = new TreePrototype[existingPrototypes.Length + 1];

        for (int i = 0; i < existingPrototypes.Length; i++)
        {
            newPrototypes[i] = existingPrototypes[i];
        }

        newPrototypes[existingPrototypes.Length] = new TreePrototype()
        {
            prefab = treePrefab
        };

        terrainData.treePrototypes = newPrototypes;
        terrainData.RefreshPrototypes();

        return existingPrototypes.Length;
    }

    private int CalculateMaxTrees(TerrainData terrainData)
    {
        // Calculate reasonable maximum based on terrain size and min distance
        float area = terrainData.size.x * terrainData.size.z;
        float treeArea = Mathf.PI * Mathf.Pow(minDistanceBetweenTrees / 2, 2);
        return Mathf.FloorToInt(area / treeArea);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(TreePlacer))]
public class TreePlacerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TreePlacer treePlacer = (TreePlacer)target;

        GUILayout.Space(10);

        if (GUILayout.Button("Mass Place Trees"))
        {
            treePlacer.MassPlaceTrees();
        }

        if (GUILayout.Button("Clear All Trees"))
        {
            treePlacer.ClearExistingTrees();
        }
    }
}
#endif