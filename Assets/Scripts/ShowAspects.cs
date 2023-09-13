using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowAspects : MonoBehaviour
{
    [SerializeField] private List<GameObject> modelsHeads, modelsHairs;
    [SerializeField] private SkinnedMeshRenderer skinnedMesh;
    [SerializeField] private List<Material> textureSkin1, textureSkin2, textureSkin3;
    [SerializeField] private List<List<Material>> listas;
    [SerializeField] private Material material;


    private void Start()
    {
        listas = new List<List<Material>>();
        listas.Add(textureSkin1);
        listas.Add(textureSkin2);
        listas.Add(textureSkin3);
    }

    public void SetModelHead(int head)
    {
        for (int i = 0; i < modelsHeads.Count; i++)
        {
            if (i != head)
            {
                modelsHeads[i].SetActive(false);
            }
            else
            {
                modelsHeads[i].SetActive(true);
            }
        }
    }

    public void SetModelHair(int hair)
    {
        for (int i = 0; i < modelsHairs.Count; i++)
        {
            if (i != hair)
            {
                modelsHairs[i].SetActive(false);
            }
            else
            {
                modelsHairs[i].SetActive(true);
            }
        }
    }

    public void SetModelColor(int i, int j,int body)
    {
        int number = i;
        if (i > 2)
        {
            number -= 3;
        }
        if (body < 3)
        {
            skinnedMesh.material = listas[number][j];
        }
        else if(body >=3)
        {
            Material[] materials = new Material[] { material, listas[number][j] };
            skinnedMesh.materials = materials;
        }
    }
}
