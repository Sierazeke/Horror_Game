using UnityEngine;

public class FixTreeClipping : MonoBehaviour
{
    public Terrain terrain;
    public float minDistance = 2f;

    void Start()
    {
        TreeInstance[] trees = terrain.terrainData.treeInstances;

        for (int i = 0; i < trees.Length; i++)
        {
            Vector3 posA = Vector3.Scale(trees[i].position, terrain.terrainData.size);

            for (int j = i + 1; j < trees.Length; j++)
            {
                Vector3 posB = Vector3.Scale(trees[j].position, terrain.terrainData.size);

                if (Vector3.Distance(posA, posB) < minDistance)
                {
                    // Push the second tree slightly away
                    Vector3 dir = (posB - posA).normalized;
                    posB += dir * (minDistance - Vector3.Distance(posA, posB));
                    trees[j].position = new Vector3(
                        posB.x / terrain.terrainData.size.x,
                        trees[j].position.y,
                        posB.z / terrain.terrainData.size.z
                    );
                }
            }
        }

        terrain.terrainData.treeInstances = trees;
        terrain.Flush();
    }
}
