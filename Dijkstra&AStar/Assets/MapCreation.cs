using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UIElements;

public class MapCreation : MonoBehaviour
{

    public int dimensionGrid = 1;
    public Material material;
    public int size3D = 10;
    public Camera cam;

    private Enemy enemy = new Enemy();
    private Tile[] grille;
    private int currentHover;

    //Fonction servant à créer une tile du plateau de jeu
    private Tile generateSingleTile(int x, int y)
    {
        Tile tile = new Tile(x, y, size3D);
        tile.tileGameObject.transform.parent = transform;

        Mesh mesh = new Mesh();
        tile.tileGameObject.GetComponent<MeshFilter>().mesh = mesh;
        tile.tileGameObject.GetComponent<MeshRenderer>().material = material;

        //Initialise nos tiles
        Vector3[] vertices = new Vector3[4];
        vertices[0] = new Vector3(x * size3D, 0, y * size3D);
        vertices[1] = new Vector3(x * size3D, 0, (y + 1) * size3D);
        vertices[2] = new Vector3((x + 1) * size3D, 0, y * size3D);
        vertices[3] = new Vector3((x + 1) * size3D, 0, (y + 1) * size3D);

        int[] trig = { 0, 1, 2, 1, 3, 2 };

        mesh.vertices = vertices;
        mesh.triangles = trig;

        return tile;
    }

    //Fonction servant à créer un cube sur le plateau de jeu a une position pointe par la souris
    private GameObject generateSingleWall(int hitPosition)
    {
        Tile tile = grille[hitPosition];

        //Verification de si un mur est deja present ou non sur la case
        if (tile.wall == null)
        {
            BoxCollider tileCollider = grille[hitPosition].tileGameObject.GetComponent<BoxCollider>();

            Vector3 tileCenter = tileCollider.center;
            Vector3 tileSize = tileCollider.size;

            Vector3 cubeScale = new Vector3(tileSize.x, tileSize.y * 10, tileSize.z);
            Vector3 cubePosition = new Vector3(tileCenter.x, tileCenter.y + (cubeScale.y / 2), tileCenter.z);

            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

            cube.transform.localScale = cubeScale;
            cube.transform.position = cubePosition;

            tile.wall = cube;

            return cube;
        }

        return null;
    }

    //Viens generer tout le plateau de jeu
    private void generateAllTiles()
    {
        //Height
        for (int y = 0; y < dimensionGrid; y++)
        {
            //Width
            for (int x = 0; x < dimensionGrid; x++)
            {
                grille[x + y * dimensionGrid] = generateSingleTile(x, y);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        grille = new Tile[dimensionGrid * dimensionGrid];
        generateAllTiles();
        spawnEnnemy(0);

        StartCoroutine(enemy.processEnemyMovement(grille, dimensionGrid));

        //Debug Purpose
        //StartCoroutine(VerifGridProperty());
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        //Debug.DrawRay(ray.origin, ray.direction * 100, Color.green);
        processTileOvering(ray);
    }

    //Fait apparaitre l'ennemie au debut de la partie
    void spawnEnnemy(int pos)
    {
        enemy.enemyGameObject = generateSingleWall(pos);
        enemy.enemyGameObject.GetComponent<MeshRenderer>().material.color = Color.red;
    }

    //To be implemented
    void randomSpawnLife()
    {

    }

    //To be implemented
    void clearBoard()
    {
        
    }

    //Fonction qui va venir identifier si notre curseur pointe sur une tile du plateau ou non
    void processTileOvering(Ray ray)
    {
        RaycastHit info;
        
        //Si on est sur le plateau
        if (Physics.Raycast(ray, out info, 10000, LayerMask.GetMask("Tile", "Hover")))
        {
            //Get the index of the tile overlaped using the cursor position
            int hitPosition = ClampCursorPositionToTile(info.transform.gameObject);

            //Si nous nétions pas sur le plateau AVANT cette frame
            if (currentHover == -1)
            {
                currentHover = hitPosition;
                grille[currentHover].tileGameObject.layer = LayerMask.NameToLayer("Hover");
            }

            //Si on parcours le plateau de CASE EN CASE
            if (currentHover != hitPosition)
            {
                grille[currentHover].tileGameObject.layer = LayerMask.NameToLayer("Tile");
                currentHover = hitPosition;
                grille[hitPosition].tileGameObject.layer = LayerMask.NameToLayer("Hover");
            }

            //Detecte le clique gauche de la souris
            if(Input.GetMouseButton(0))
            {
                generateSingleWall(hitPosition);
            }

        }
        //Si on pas sur le plateau
        else
        {
            //Si on etait sur le plateau AVANT cette frame
            if (currentHover != -1)
            {
                grille[currentHover].tileGameObject.layer = LayerMask.NameToLayer("Tile");
                currentHover = -1;
            }
        }
    }

    //Viens convertir la position (transform) de la souris en indice de case du plateau (indice du tableau)
    private int ClampCursorPositionToTile(GameObject hitInfo)
    {
        for (int x = 0; x < dimensionGrid; x++)
        {
            for (int y = 0; y < dimensionGrid; y++)
            {
                if (grille[x + y * dimensionGrid].tileGameObject == hitInfo)
                    return (x + (y * dimensionGrid));
            }
        }

        //Si on n'overlap pas, on return -1
        return -1;
    }

    //DEBUG PURPOSE
    //Parcours de la grille de bas (gauche) à haut (droite)
    IEnumerator VerifGridProperty()
    {
        for (int y = 0; y < dimensionGrid; y++)
        {
            for (int x = 0; x < dimensionGrid; x++)
            {
                Tile tile = grille[x + y * dimensionGrid];
                Color currentColor = tile.tileGameObject.GetComponent<MeshRenderer>().material.color;

                tile.tileGameObject.GetComponent<MeshRenderer>().material.color = Color.red;
                yield return new WaitForSeconds(0.1f);
                tile.tileGameObject.GetComponent<MeshRenderer>().material.color = currentColor;
            }
        }
        yield return null;
    }
}
