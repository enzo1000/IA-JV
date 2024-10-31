using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Enemy
{
    public GameObject enemyGameObject = null;

    public Enemy() {}

    //Coroutine gerant les deplacements de l'ennemi tout les x secondes
    public IEnumerator processEnemyMovement(Tile[] grille, int dim)
    {
        //On initialise notre pts de depart (position de l'ennemy)
        int origine = getEnemyIndex(grille, dim);

        //On initialise notre pts d'arrive (position de notre seed)
        int toGo = getSeed(grille, dim);

        List<int> path = dijkstraPathfinding(origine, toGo, grille);

        //Affiche le chemin sous forme de cubes rouges (modifier la couleur en fonction de l'algo)
        GameObject.FindGameObjectWithTag("GameController").GetComponent<MapCreation>().createPathCube(path);

        //Si vrai alors on se dirige vers la graine
        if (path.Count > 0)
        {
            foreach (int tile in path)
            {
                yield return new WaitForSeconds(0.5f);
                translateVoisin(getEnemyIndex(grille, dim), tile, grille);
            }
            //Des qu'on a ete sur la graine, on clear le terrain et on re cherche la graine
            clearBoard(grille, dim);
            GameObject.FindGameObjectWithTag("GameController").GetComponent<MapCreation>().spawnSeed();
        }
        yield return new WaitForSeconds(0.5f);
    }

    //Fonction qui renvoi l'index de la seed a aller chercher avec notre
    // algo de recherche de chemin
    private int getSeed(Tile[] grille, int dim)
    {
        GameObject seed = GameObject.FindGameObjectWithTag("Seed");
        Vector3 pos = seed.transform.position;
        int tileSize = grille[0].tileSize;
        //Divise la position en x et en y arrondi a l'inferieur pour recuperer l'index de la "seed" sur la grille
        int seedIndex = Mathf.FloorToInt(pos.x / tileSize) + Mathf.FloorToInt(pos.z / tileSize) * dim;
        return seedIndex;
    }

    //Clear le terrain de tout elements / variables
    public void clearBoard(Tile[] grille, int dim)
    {
        //Remplacer les deux for dependant de x et y par un foreach
        for (int x = 0; x < dim; x++)
        {
            for (int y = 0; y < dim; y++)
            {
                Tile tile = grille[x + y * dim];

                //Reset les predescesseur pour re executer un dijkstra
                grille[x + y * dim].predescesseur = null;
                grille[x + y * dim].poids = 1;
                //Supprime les objets sur les Tiles (Wall) sauf l'enemy
                if (tile.onTop != null && tile.onTop.transform.tag != "Enemy")
                {
                    GameObject.Destroy(tile.onTop);
                    grille[x + y * dim].onTop = null;
                }
            }
        }
    }

    //Recupere l'index de l'ennemi sur la grille a partir de la position de sa boundingBox
    private int getEnemyIndex(Tile[] grille, int dim)
    {
        //Recupere la position de l'ennemi (genere a partir de la BB d'une tile)
        Vector3 pos = enemyGameObject.transform.position;
        int tileSize = grille[0].tileSize;
        //Divise la position en x et en y arrondi a l'inferieur pour recuperer l'index de l'ennemi sur la grille
        int enemyIndex = Mathf.FloorToInt(pos.x / tileSize) + Mathf.FloorToInt(pos.z / tileSize) * dim;

        return enemyIndex;
    }

    //Fonction qui a partir d'une coordonnee renvoi les voisins adjacents a la coordonne envoye
    private List<int> getVoisin(int i, Tile[] grille)
    {
        //TODO verifier si mur et pas seed (sinon boucle infini)
        List<int> voisin = new List<int>();
        int dim = (int)Mathf.Sqrt(grille.Length);

        //Verification bord gauche
        if (i % dim == 0)
        {
            //Ajout voisin de droite
            // Cas pas d'objet
            if (grille[i + 1].onTop == null)
            {
                voisin.Add(i + 1);
            }
            // Cas objet mais pas un mur
            else if (grille[i + 1].onTop.transform.tag != "Wall")
            {
                voisin.Add(i + 1);
            }
        }
        //Si pas un bord gauche alors verif bord droit
        else if ((i + 1) % dim == 0) 
        {
            if (grille[i - 1].onTop == null)
            {
                //Ajout voisin de gauche
                voisin.Add(i - 1);
            }
            else if (grille[i - 1].onTop.transform.tag != "Wall")
            {
                voisin.Add(i - 1);
            }
        }
        //Pas sur un bord donc fonctionnement normal
        else
        {
            if (grille[i + 1].onTop == null)
            {
                voisin.Add(i + 1);
            }
            else if (grille[i + 1].onTop.transform.tag != "Wall")
            {
                voisin.Add(i + 1);
            }

            if (grille[i - 1].onTop == null)
            {
                voisin.Add(i - 1);
            }
            else if (grille[i - 1].onTop.transform.tag != "Wall")
            {
                voisin.Add(i - 1);
            }
        }

        //Verification bord bas
        if (i < dim)
        {
            if (grille[i + dim].onTop == null)
            {
                //Ajout voisin du haut
                voisin.Add(i + dim);
            }
            else if (grille[i + dim].onTop.transform.tag != "Wall")
            {
                voisin.Add(i + dim);
            }
        }
        //Si pas un bord bas alors verif bord haut
        else if (i > (grille.Length - dim - 1))
        {
            if (grille[i - dim].onTop == null)
            {
                //Ajout voisin du bas
                voisin.Add(i - dim);
            }
            else if (grille[i - dim].onTop.transform.tag != "Wall")
            {
                voisin.Add(i - dim);
            }
        }
        //Pas sur un bord donc fonctionnement normal
        else
        {
            if (grille[i + dim].onTop == null)
            {
                voisin.Add(i + dim);
            }
            else if (grille[i + dim].onTop.transform.tag != "Wall")
            {
                voisin.Add(i + dim);
            }

            if (grille[i - dim].onTop == null)
            {
                voisin.Add(i - dim);
            }
            else if (grille[i - dim].onTop.transform.tag != "Wall")
            {
                voisin.Add(i - dim);
            }
        }
        return voisin;
    }

    //Fonction qui fait voyager l'ennemi d'une case a une autre a partir de l'index de la case donne
    private void translateVoisin(int position, int toGo, Tile[] grille)
    {
        grille[position].onTop = null;

        //Si on veut aller a un endroit qui est deja occupe : 
        if (grille[toGo].onTop != null)
        {
            GameObject.Destroy(grille[toGo].onTop);
        }
        grille[toGo].onTop = enemyGameObject;

        //Debug.Log(position + " --> " + toGo);

        Vector3 pos = grille[toGo].tileGameObject.GetComponent<BoxCollider>().center;
        Vector3 vectDist = pos - enemyGameObject.transform.position;
        vectDist.y = 0;

        enemyGameObject.transform.position += vectDist;
    }

    //Realise un algo de dijkstra entre un point a (ennemi : origin) a un point b (bille : dest)
    private List<int> dijkstraPathfinding(int origin, int dest, Tile[] grille)
    {
        //On a un graphe vide P, une liste de case visites et une frontiere de case a voisiner
        List<int> casesVisites = new List<int>();
        List<int> frontiere = new List<int>();

        //On met la case de depart a 0
        grille[origin].poids = 0;
        int index = origin;

        while (index != dest)
        {
            casesVisites.Add(index);
            //On recupere les voisins du points courant
            foreach (int vois in getVoisin(index, grille))
            {
                //Si le voisin n'est pas un point deja visite
                if (!casesVisites.Contains(vois) && !frontiere.Contains(vois))
                {
                    //On ajoute aux predecesseur du voisin notre index
                    grille[vois].predescesseur = grille[index];
                    frontiere.Add(vois);
                }
            }
            int indexMinusOne = index;
            float minDist = Mathf.Infinity;
            //Sur tout les membres de notre frontiere
            foreach (int vois in frontiere)
            {
                //On prend le chemin le plus court de notre frontiere
                if (grille[vois].distance() < minDist)
                {
                    minDist = grille[vois].distance();
                    index = vois;
                }
            }
            //Nous n'avons pas trouve de chemin
            if (frontiere.Count == 0)
            {
                break;
            }

            //On supprime de la frontiere le point sur lequel on se dirige
            frontiere.Remove(index);
        }

        //Renvoyer les predescesseur de dest
        int pathIndex = dest;
        List<int> path = new List<int>();

        //Recreer le chemin
        while (grille[pathIndex].predescesseur != null)
        {
            path.Insert(0, pathIndex);
            pathIndex = grille[pathIndex].predescesseur.index;
        }

        return path;
    }

    private void aStar(int origin, int toGo, Tile[] grille)
    {
        //Avec priorite en cle et indice en valeur
        List<int> openList = new List<int>();
        List<int> closedList = new List<int>();

        //Je fais un clear board ici mais a terme le faire dans la fonction associe
        foreach (Tile t in grille)
        {
            t.poids = 0;
        }

        openList.Add(origin);
        foreach (int u in openList)
        {
            if (u == toGo)
            {
                break;
            }
            else
            {
                foreach(int v in getVoisin(u, grille))
                {
                    if (!closedList.Contains(v))    //Ajouter la verification de v dans openList avec cout plus bas
                    {
                        grille[v].poids = grille[u].poids + 1;
                        //heuristique (specificite de A*)
                        grille[v].heuristique = grille[v].poids 
                            + Vector3.Distance(
                                grille[v].tileGameObject.GetComponent<BoxCollider>().center, 
                                grille[toGo].tileGameObject.GetComponent<BoxCollider>().center);
                        openList.Add(v);
                    }
                }
                closedList.Add(u);
            }
        }
    }
}