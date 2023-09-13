using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialSwitcher : MonoBehaviour
{
    public Material[] objectMaterials; // Array of available materials
    public Material[] materials; // Array of available materials
    public float switchInterval = 3f; // Switch interval in seconds
    public int materialIndex = 0; // Index of the material to be selected
    public int new_material = 0; // Index of the material to be selected

    private Renderer objectRenderer; // Object's renderer

    private void Start()
    {
        objectRenderer = GetComponent<Renderer>(); // Get the object's renderer

        if (objectMaterials.Length > 0)
        {
            StartCoroutine(SwitchMaterialCoroutine()); // Start the material switch coroutine
        }
        else
        {
            Debug.LogError("No material assigned to MaterialSwitcher!");
        }
    }

    private System.Collections.IEnumerator SwitchMaterialCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(switchInterval); // Wait for the defined interval

            new_material = (new_material + 1) % materials.Length; // Move to the next material index
            objectMaterials[materialIndex] = materials[new_material];
            objectRenderer.materials = objectMaterials; // Apply the material to the object
        }
    }
}
