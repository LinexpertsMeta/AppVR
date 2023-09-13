using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomBlink : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private List<Material> materialsEyes;
    [SerializeField] private Material material;

    public float time, timeMax;
    private bool blinking;

    public bool firstMaterial;
    // Start is called before the first frame update
    private void Start()
    {
        blinking = false;
        meshRenderer = GetComponent<MeshRenderer>();
        time = 0;
        timeMax = Random.Range(4, 8);
    }

    private void Update()
    {
        time = time + Time.deltaTime;
        if(time > timeMax && !blinking)
        {
            blinking = true;
            SetModelColor(1);
            StartCoroutine(StopBlinking());
        }
    }

    public void SetModelColor(int i)
    {
        if (firstMaterial)
        {
            meshRenderer.material = materialsEyes[i];
        }
        else
        {
            Material[] newMaterials = new Material[] { material, materialsEyes[i] };
            meshRenderer.materials = newMaterials;
        }
    }

    IEnumerator StopBlinking()
    {
        yield return new WaitForSeconds(1);
        SetModelColor(0);
        timeMax = Random.Range(4, 8);
        blinking = false;
        time = 0;
    }

}
