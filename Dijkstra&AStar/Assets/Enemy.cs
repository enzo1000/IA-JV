using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Enemy
{
    public GameObject enemyGameObject = null;

    public Enemy() {}

    //Coroutine gerant les deplacement de l'ennemi tout les x secondes
    public IEnumerator processEnemyMovement(Tile[] grille, int dim)
    {
        while (true)
        {
            int rand = -1;
            int index = getEnemyIndex(grille, dim);

            //la grille est initialisé à l'infini
            dijkstraPathfinding(index, 5, grille);

            List<int> vois = getVoisin(index, grille);

            if (vois.Count > 0)
            {
                rand = Mathf.RoundToInt(UnityEngine.Random.Range(0, vois.Count));
            }

            if (rand != -1)
            {
                translateVoisin(index, vois[rand], grille);
            }

            yield return new WaitForSeconds(1f);
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
        List<int> voisin = new List<int>();
        int dim = (int)Mathf.Sqrt(grille.Length);

        //Verification bord gauche
        if (i % dim == 0)
        {
            if (grille[i + 1].wall == null)
            {
                //Ajout voisin de droite
                voisin.Add(i + 1);
            }
        }
        //Si pas un bord gauche alors verif bord droit
        else if (i % (dim - 1) == 0) 
        {
            if (grille[i - 1].wall == null)
            {
                //Ajout voisin de gauche
                voisin.Add(i - 1);
            }
        }
        //Pas sur un bord donc fonctionnement normal
        else
        {
            if (grille[i + 1].wall == null)
            {
                voisin.Add(i + 1);
            }
            if (grille[i - 1].wall == null)
            {
                voisin.Add(i - 1);
            }
        }

        //Verification bord bas
        if (i < dim)
        {
            if (grille[i + dim].wall == null)
            {
                //Ajout voisin du haut
                voisin.Add(i + dim);
            }
        }
        //Si pas un bord bas alors verif bord haut
        else if (i > (grille.Length - dim - 1))
        {
            if (grille[i - dim].wall == null)
            {
                //Ajout voisin du bas
                voisin.Add(i - dim);
            }
        }
        //Pas sur un bord donc fonctionnement normal
        else
        {
            if (grille[i + dim].wall == null)
            {
                voisin.Add(i + dim);
            }
            if (grille[i - dim].wall == null)
            {
                voisin.Add(i - dim);
            }
        }

        return voisin;
    }

    //Fonction qui fait voyager l'ennemi d'une case a une autre a partir de l'index de la case donne
    private void translateVoisin(int position, int toGo, Tile[] grille)
    {
        grille[position].wall = null;
        grille[toGo].wall = enemyGameObject;

        Debug.Log(position + " --> " + toGo);

        Vector3 pos = grille[toGo].tileGameObject.GetComponent<BoxCollider>().center;
        Vector3 vectDist = pos - enemyGameObject.transform.position;
        vectDist.y = 0;

        //Debug.Log(vectDist.x + " " + vectDist.y + " " + vectDist.z);

        enemyGameObject.transform.position += vectDist;
    }

    //Realise un algo de dijkstra entre un point a (ennemi : origin) a un point b (bille : dest)
    private void dijkstraPathfinding(int origin, int dest, Tile[] grille)
    {
        //On a un graphe vide P, une liste de case visites et une frontiere de case a voisiner
        List<int> P = new List<int>();
        List<int> casesVisites = new List<int>();
        List<int> frontiere = new List<int>();

        //On met la case de départ a 0 (position du joueur)
        grille[origin].poids = 0;

        //recupere les voisins du point initial (b) dans frontiere
        foreach (int vois in getVoisin(origin, grille))
        {
            //Pas la peine de verifier s'il fait parti de notre graphe
            if (!P.Contains(vois))
            {
                frontiere.Add(vois);
            }
        }

        float minDist = Mathf.Infinity;
        int index = 0;

        //Sur tout les membres de notre frontiere
        foreach (int vois in frontiere)
        {
            //On prend le chemin le plus court dans notre frontiere
            if (grille[vois].distance() < minDist)
            {
                minDist = grille[vois].distance();
                index = vois;
            }
        }

        //Si P ne contient pas notre voisin alors

        //if (!P.Contains(vois))
        //{ }

            //ajouter de nouveaux voisins
            //recuperer le chemin le plus court ...
            //  a chaque recuperation de chemin le plus cours, changer le poids de la tuile pour
            //  notifier de l'avancé du chemin
            //Continuer tant que toute tuiles pas explorés ou point de dest pas atteint.

            //getPlusCoursChemin();

            List<int> voisin = getVoisin(origin, grille);
    }
}