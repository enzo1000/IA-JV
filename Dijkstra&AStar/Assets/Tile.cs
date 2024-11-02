using UnityEngine;

class Tile
{
    public GameObject onTop = null;     //Accede a l'element sur la tile (Wall, Seed, Enemy ...)
    public GameObject tileGameObject;   //Accede a la tile en elle meme
    public int tileSize;

    //data pour dijkstra
    public int poids = 1;
    public Tile predescesseur = null;
    public int index = 0;

    //data pour A*
    public float aStarCost;

    public Tile(int x, int y, int dimensionGrid, int size) 
    {
        tileSize = size;
        tileGameObject = new GameObject(string.Format("X:{0}, Y:{1}", x, y));
        index = x + y * dimensionGrid;

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

    public float distanceAStar(Vector3 toGo)
    {
        Vector3 from = tileGameObject.GetComponent<BoxCollider>().center;

        if (predescesseur == null) { return Vector3.Distance(from, toGo); }
        return poids + predescesseur.distanceAStar(toGo);
    }
}