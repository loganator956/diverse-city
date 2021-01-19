using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseScript : MonoBehaviour
{
    public enum HouseOccupant
    {
        Empty, Circle, Square, Triangle
    }
    SpriteRenderer sr;
    private HouseOccupant occupant;
    public HouseOccupant Occupant
    {
        get { return occupant; }
        set
        {
            occupant = value;
            switch (occupant)
            {
                case HouseOccupant.Empty:
                    sr.sprite = OccupantSprites[0];
                    break;
                case HouseOccupant.Circle:
                    sr.sprite = OccupantSprites[1];
                    break;
                case HouseOccupant.Square:
                    sr.sprite = OccupantSprites[2];
                    break;
                case HouseOccupant.Triangle:
                    sr.sprite = OccupantSprites[3];
                    break;
            }
        }
    }
    /* 0 - Empty
     * 1 - Circle
     * 2 - Square
     * 3 - Triangle
     */
    public Sprite[] OccupantSprites;


    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    float tempTimer = 0f;
    void Update()
    {
        // tempTimer += Time.deltaTime;
        // if (tempTimer > 2f)
        // {
        //     Occupant = (HouseOccupant)Enum.GetValues(typeof(HouseOccupant)).GetValue(UnityEngine.Random.Range(0, Enum.GetValues(typeof(HouseOccupant)).Length));
        //     tempTimer = 0;
        // }
    }

}
