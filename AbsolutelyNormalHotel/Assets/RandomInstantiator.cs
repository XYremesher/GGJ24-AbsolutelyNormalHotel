using UnityEngine;

public class RandomInstantiator : MonoBehaviour
{
    public GameObject objectToInstantiate; // The prefab you want to instantiate
    public int width = 10; // Width of the area
    public int height = 10; // Height of the area
    public int numberOfObjects = 5; // Number of objects to instantiate

    void Start()
    {
        InstantiateObjects();
    }

    void InstantiateObjects()
    {
        for (int i = 0; i < numberOfObjects; i++)
        {
            // Generate a random position within the defined area
            Vector3 randomPosition = transform.position + new Vector3(
                Random.Range(-width / 2f, width / 2f),
                0,
                Random.Range(-height / 2f, height / 2f));

            // Generate a random Y rotation
            float randomYRotation = Random.Range(0f, 360f);
            Quaternion randomRotation = Quaternion.Euler(0, randomYRotation, 0);

            // Instantiate the object at the random position with a random Y rotation
            Instantiate(objectToInstantiate, randomPosition, randomRotation);
        }
    }


    // Draw a gizmo in the editor
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow; // Set the color of the gizmo
        Vector3 position = transform.position + new Vector3(0, 0.01f, 0); // Slightly above ground to ensure visibility
        Vector3 size = new Vector3(width, 0.01f, height); // Create a thin rectangle

        // Draw a wireframe cube gizmo with the given position and size
        Gizmos.DrawWireCube(position, size);
    }
}
