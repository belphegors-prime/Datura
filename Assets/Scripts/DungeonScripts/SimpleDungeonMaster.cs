using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SimpleDungeonMaster : MonoBehaviour
{
    public Room roomPrefab;
    public Transform floorRef;
    public GameObject wallPrefab, worldPortalPrefab, enemySpawner;
    public int roomLimit;

    float floorMaxX, floorMinX, floorMaxZ, floorMinZ, roomWidth, roomHeight;
    Hashtable posToRoom, dirToVector;
    Vector3 floorSize;
    Queue<Room> roomsToExpand;
    float[] floorBnds;
    int numRooms = 0;
    static ArrayList rooms;
    PlayerController player;


    public static ArrayList GetRooms()
    {
        return (ArrayList) rooms.Clone();
    }
    void Connect(Room s, Room t)
    {
        //Debug.Log("connecting" + s.name + " " + t.name);
        //Vector3 dist = (s.transform.position - t.transform.position);
        Vector3 sPos = s.transform.position; Vector3 tPos = t.transform.position;
        /* ===================2 ROOMS CASE================*/
        if (s.name.Substring(0, 4).Equals("Room") && t.name.Substring(0, 4).Equals("Room"))
        {
            Transform sWalls = s.transform.FindChild("Walls");
            Transform sDoors = s.transform.FindChild("Doors");
            Transform tWalls = t.transform.FindChild("Walls");
            Transform tDoors = t.transform.FindChild("Doors");
            string sw = "", sd = "", tw = "", td = "";

            //determine direction of connection

            //connecting forward
            if (tPos.z > sPos.z)
            {
                sw = "ZWall"; sd = "ZDoor"; tw = "zWall"; td = "zDoor";
            }

            //connecting back
            else if (tPos.z < sPos.z)
            {
                sw = "zWall"; sd = "zDoor"; tw = "ZWall"; td = "ZDoor";
            }
            //connecting right
            else if (tPos.x > sPos.x)
            {
                sw = "XWall"; sd = "XDoor"; tw = "xWall"; td = "xDoor";
            }
            //connecting left
            else if (tPos.x < sPos.x)
            {
                sw = "xWall"; sd = "xDoor"; tw = "XWall"; td = "XDoor";
            }

            //null check and delete
            if (sWalls.FindChild(sw) != null)
            {
                Destroy(sWalls.FindChild(sw).gameObject);
                sWalls.FindChild(sw).parent = null;
            }

            if (sDoors.FindChild(sd) != null)
            {
                Destroy(sDoors.FindChild(sd).gameObject);
                sDoors.FindChild(sd).parent = null;
            }

            if (tWalls.FindChild(tw) != null)
            {
                Destroy(tWalls.FindChild(tw).gameObject);
                tWalls.FindChild(tw).parent = null;
            }
            if (tDoors.FindChild(td) != null)
            {
                Destroy(tDoors.FindChild(td).gameObject);
                tDoors.FindChild(td).parent = null;
            }
        }
    }
    void ActivateEnemySpawner()
    {
        enemySpawner.SetActive(true);
    }
    void CloseRemainingRooms()
    {
        Room[] r = (Room[])roomsToExpand.ToArray();
        
        for (int i = 0; i < roomsToExpand.Count; i++)
        {
            //Debug.Log(r[i].name);
            foreach (Transform door in r[i].transform.Find("Doors"))
            {
                Vector3 dir = (Vector3) dirToVector[door.name[0]];
                if (r[i].transform.Find("Walls").Find(door.name[0] + "Wall") == null
                    && !(posToRoom.ContainsKey(r[i].transform.position + dir)))
                {
                    GameObject w = (GameObject)Instantiate(wallPrefab);
                    w.transform.position = door.GetComponent<Renderer>().bounds.center;
                    w.transform.rotation = new Quaternion(0, door.rotation.y, 0, door.rotation.w);
                    w.transform.SetParent(r[i].transform);
                    //w.transform.localRotation = door.transform.localRotation;
                }
                Destroy(door.gameObject);
            }
        }
    }

    //takes a room and a direction and creates a new room in that direction
    void ConstructRoom(Room creator, char cardinal)
    {
        if (numRooms > roomLimit) return;
        int currentDepth = creator.depth + 1;
        Vector3 entryPos = creator.transform.position;
        
        //build new room
        Room neighbor = Instantiate(roomPrefab) as Room;
        neighbor.transform.position = entryPos;
        neighbor.transform.SetParent(transform);
        neighbor.SetUp(creator, currentDepth, "Room" + numRooms++);

        //move neighbor and delete walls based on direction of creator
        Vector3 v = new Vector3();
        string w = "", d = "";
        switch (cardinal)
        {
            case 'z':
                v = Vector3.back;
                w = "ZWall";
                break;

            case 'Z':
                v = Vector3.forward;
                w = "zWall";
                break;

            case 'x':
                v = Vector3.left;
                w = "XWall";
                break;

            case 'X':
                v = Vector3.right;
                w = "xWall";
                break;
        }
        d = w[0] + "Door";
        //translate in direction of cardinal
        neighbor.transform.Translate(roomWidth * v);
        posToRoom.Add(neighbor.transform.position, neighbor);
        if (neighbor.transform.FindChild("Walls").FindChild(w).gameObject != null)
        {
            //destroy wall between rooms
            Destroy(neighbor.transform.FindChild("Walls").FindChild(w).gameObject);
            neighbor.transform.FindChild("Walls").FindChild(w).parent = null;

            //destroy redundant door between rooms
            Destroy(neighbor.transform.FindChild("Doors").FindChild(d).gameObject);
            neighbor.transform.FindChild("Doors").FindChild(d).parent = null;
        }
        SetDoors(neighbor);
        roomsToExpand.Enqueue(neighbor);
        rooms.Add(neighbor);
    }
    

    //builds and connects rooms and hallways
    void Expand()
    {
        while (roomsToExpand.Count > 0)
        {
            if (numRooms > roomLimit)
            {
                //Debug.Log(numRooms + " is greater than " + roomLimit);
                CloseRemainingRooms();
                return;
            }
            Room r = roomsToExpand.Peek() as Room;
            //int currentDepth = r.depth + 1;


            //foreach newly generated door decide whether to build hallway or additional room
            for (int i = 0; i < r.transform.FindChild("Doors").childCount; i++)
            {
                Transform door = r.transform.FindChild("Doors").GetChild(i);
                //make sure door doesn't lead into already constructed area
                Vector3 v = (Vector3) dirToVector[door.name[0]];
                Vector3 pos = r.transform.position + (v * roomWidth);

                //if door leads to already constructed area, connect the two
                //TODO just delete wall of pre-existing room, or delete door
                //let RNG decide?
                if (posToRoom.ContainsKey(pos))
                {
                    Destroy(door.gameObject);
                    door.parent = null;
                    i--;
                }
                else
                {
                    ConstructRoom(r, door.name[0]);
                }
            }
            roomsToExpand.Dequeue();
        }

    }
    //deletes walls based on distance function to randomly create doors
    void SetDoors(Room r)
    {
        //separate variable to track door index

        for (int i = 0; i < r.transform.FindChild("Walls").childCount; i++)
        {

            float pDoor;
            Transform wall = r.transform.FindChild("Walls").GetChild(i);
            //Debug.Log(r.name + " " + wall.name);
            //determine whether to expand on a wall based on distance to floor boundry
            switch (wall.name[0])
            {
                case 'z':
                    pDoor = Mathf.Abs(wall.position.z - floorMinZ) / (floorSize.z);
                    break;
                case 'Z':
                    pDoor = Mathf.Abs(wall.position.z - floorMaxZ) / (floorSize.z);
                   // Debug.Log(r.name + "pDoorZ" + pDoor);
                    break;
                case 'x':
                    pDoor = Mathf.Abs(wall.position.x - floorMinX) / (floorSize.x);
                    break;
                case 'X':
                    pDoor = Mathf.Abs(wall.position.x - floorMaxX) / (floorSize.x);
                   // Debug.Log(r.name + " pDoorX" + pDoor);
                    break;
                default:
                    pDoor = 0;
                    break;
            }
            
            //pDoor is large when there is more space to expand into
            //delete wall to allow access through door
            if (pDoor   > Random.value)
            {
                Destroy(r.transform.FindChild("Walls").FindChild(wall.name).gameObject);
                wall.parent = null;
                i--;
            }
            //else delete door
            else
            {
                string doorname = wall.name[0] + "Door";
                Destroy(r.transform.FindChild("Doors").FindChild(doorname).gameObject);
                r.transform.FindChild("Doors").FindChild(doorname).parent = null;

            }

        }
    }

    void SetEntryRoom()
    {
        Vector3 entryPos = new Vector3();
        int rand = (int)Random.Range(0, 4 - float.Epsilon);
        char side;
        /*randomly decide which side to place the entrance
        odd indices are Z, evens are X, minimum bounds are the larger indices
        */
        if (rand % 2 == 1)
        {
            entryPos.z = floorBnds[rand];
            if (rand == 3)
            {
                entryPos.z += roomWidth / 2;
                side = 'z';
            }
            else
            {
                entryPos.z -= roomWidth / 2;
                side = 'Z';
            }
        }
        else
        {
            entryPos.x = floorBnds[rand];
            if (rand == 2)
            {
                side = 'x';
                entryPos.x += roomWidth / 2;
            }
            else
            {
                side = 'X';
                entryPos.x -= roomWidth / 2;
            }
        }

        Room entry = Instantiate(roomPrefab, entryPos, Quaternion.identity) as Room;
        entry.depth = 0;
        entry.name = "Room" + numRooms++;
        posToRoom.Add(entryPos, entry);
        entry.transform.SetParent(transform);
        //place player's starting position as room
        player.transform.position = entryPos;

        //create entrance portal and delete conflicting door and wall
        GameObject worldPortal = Instantiate(worldPortalPrefab) as GameObject;
        Transform entDoor, entWall;
        entDoor = entry.transform.FindChild("Doors").FindChild(side + "Door");
        entWall = entry.transform.FindChild("Walls").FindChild(side + "Wall");

        worldPortal.transform.position = entDoor.position;
        worldPortal.transform.rotation = entDoor.rotation;
        worldPortal.transform.SetParent(entry.transform);
        
        Destroy(entDoor.gameObject);
        entDoor.parent = null;
        Destroy(entWall.gameObject);
        entWall.parent = null;

        SetDoors(entry);
        roomsToExpand.Enqueue(entry);
        rooms.Add(entry);
    }


    void SetReferences()
    {
        floorMaxX = floorRef.GetComponent<Renderer>().bounds.max.x;
        floorMinX = floorRef.GetComponent<Renderer>().bounds.min.x;
        floorMaxZ = floorRef.GetComponent<Renderer>().bounds.max.z;
        floorMinZ = floorRef.GetComponent<Renderer>().bounds.min.z;

        floorBnds = new float[4];
        floorBnds[0] = floorMaxX;
        floorBnds[1] = floorMaxZ;
        floorBnds[2] = floorMinX;
        floorBnds[3] = floorMinZ;

        floorSize = floorRef.GetComponent<Renderer>().bounds.size;
        roomWidth = roomPrefab.transform.GetChild(0).GetChild(0).GetComponent<Renderer>().bounds.size.x;
        roomHeight = roomPrefab.transform.GetChild(0).GetChild(0).GetComponent<Renderer>().bounds.size.y;

        posToRoom = new Hashtable();

        dirToVector = new Hashtable();
        dirToVector.Add('z', Vector3.back);
        dirToVector.Add('Z', Vector3.forward);
        dirToVector.Add('x', Vector3.left);
        dirToVector.Add('X', Vector3.right);

        roomsToExpand = new Queue<Room>();
        rooms = new ArrayList();

        player = PlayerController.player;
    }
    void Start()
    {
        SetReferences();
        SetEntryRoom();
        Expand();
        ActivateEnemySpawner();
    }

}
