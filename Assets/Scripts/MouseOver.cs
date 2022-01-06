using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseOver : MonoBehaviour
{
    public Material selectedMaterial;
    MeshRenderer mesh;
    Material defaultMat;
    private void Start()
    {
        mesh = GetComponent<MeshRenderer>();
        defaultMat = mesh.material;
    }

    private void OnMouseOver()
    {
        mesh.material = selectedMaterial;
    }

    private void OnMouseExit()
    {
        mesh.material = defaultMat;
    }
}
