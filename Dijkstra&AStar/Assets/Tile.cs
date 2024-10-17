using System;
using UnityEngine;

class Tile
{

    public GameObject tileGameObject;
    public GameObject wall = null;
    public int tileSize;

    //data dijkstra
    public int poids = 1;
    public Tile predescesseur = null;

    public Tile(int x, int y, int size) 
    {
        tileSize = size;
        tileGameObject = new GameObject(string.Format("X:{0}, Y:{1}", x, y));

        tileGameObject.AddComponent<MeshFilter>();
        tileGameObject.AddComponent<MeshRenderer>();

        tileGameObject.layer = LayerMask.NameToLayer("Tile");

        //Positionnement du box collider
        int offset = size / 2;
        Vector3 centerV3 = new Vector3(x * size + offset, 0, y * size + offset);
        Vector3 sizeV3 = new Vector3(size, 1, size);

        tileGameObject.AddComponent<BoxCollider>();
        tileGameObject.GetComponent<BoxCollider>().center = centerV3;
        tileGameObject.GetComponent<BoxCollider>().size = sizeV3;
    }
    
    //Fonction qui permet de parcourir notre chemin et de recuperer son coup
    public int distance()
    {
        if (predescesseur == null) { return 0; }
        return poids + predescesseur.distance();
    }
}