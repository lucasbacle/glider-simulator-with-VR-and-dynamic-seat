using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Wrapper class used to sotre the tile object along with useful information
 */

public class TerrainTile
{
    public GameObject tileObject;
    public HeightMap hm;
    public Vector2 coord;
	internal float minHeight, maxHeight;

	public TerrainTile(GameObject tileObject, HeightMap hm, Vector2 coord, float minHeight, float maxHeight)
    {
        this.hm = hm;
        this.tileObject = tileObject;
        this.coord = coord;
        this.minHeight = minHeight;
        this.maxHeight = maxHeight;
    }
}