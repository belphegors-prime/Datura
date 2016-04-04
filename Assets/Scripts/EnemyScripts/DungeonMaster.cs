using UnityEngine;
using System.Collections;


public class DungeonMaster : MonoBehaviour {
    public Room roomPrefab, hallwayPrefab;
    public Transform floorRef;
    public GameObject doorPrefab, hallToRoom, openHall;
    public float pHallway;
    public int roomLimit;


    float floorMaxX, floorMinX, floorMaxZ, floorMinZ, roomWidth, roomHeight;
    Hashtable posToRoom, dirToVector;
    Vector3 floorSize;
    float[] floorBnds;
    int numRooms = 0;
    
    void Connect(Room s, Room t)
    {
        Debug.Log("connecting" + s.name + " " + t.name);
        Vector3 dist = (s.transform.position - t.transform.position);
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
            else if( tPos.z < sPos.z)
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

        /*========HALLWAY TO ROOM CASE===========*/
        if (s.name.Equals("Hallway") ^ t.name.Equals("Hallway")){
            Room hall, room;
            if (s.name.Equals("Hallway"))
            {
                hall = s;
                room = t;
            }
            else
            {
                hall = t;
                room = s;
            }

            Vector3 hPos = hall.transform.position;
            Vector3 rPos = room.transform.position;
            string wall = "";
            if (hPos.z > rPos.z)
            {
                wall = "ZWall";
            }

            //connecting back
            else if (hPos.z < rPos.z)
            {
                wall = "zWall";
            }
            //connecting right
            else if (hPos.x > rPos.x)
            {
                wall = "XWall";
            }
            //connecting left
            else if (hPos.x < rPos.x)
            {
                wall = "xWall";
            }
            //null check and destroy wall leading to hallway
            if (room.transform.FindChild("Walls").FindChild(wall) != null)
            {
                //keep a copy of wall position
                Destroy(room.transform.FindChild("Walls").FindChild(wall).gameObject);
                room.transform.FindChild("Walls").FindChild(wall).parent = null;
            }
            //handle case where hallway and wall are perpendicular
            //hall is perpendicular to room if the hall is rotated about y axis and 
            if((hall.transform.rotation.y != 0f && System.Char.ToUpper(wall[0]) == 'Z') || (hall.transform.rotation.y == 0 && System.Char.ToUpper(wall[0]) == 'X'))
            {
                //find closest hallway wall to room and replace with prefab
                Transform h1 = hall.transform.GetChild(0);
                Transform h2 = hall.transform.GetChild(1);

                Transform hallWall;
                if (Vector3.Distance(h1.position, room.transform.position) < Vector3.Distance(h2.position, room.transform.position))
                    hallWall = h1;
                else
                    hallWall = h2;

                //create opening hall
                GameObject o = Instantiate(openHall);
                o.transform.position = hallWall.position;
                o.transform.rotation = hall.transform.rotation;
                o.transform.SetParent(hall.transform);

                //create hall to room connection
                GameObject c = Instantiate(hallToRoom);
                Vector3 v = (Vector3)dirToVector[wall[0]];
                Vector3 wallPos = room.transform.position + (roomWidth / 2) * v;

                //place connector directly between wall and hall
                c.transform.position = Vector3.Lerp(wallPos, hallWall.position, 0.5f);
                c.transform.rotation = hall.transform.rotation;
                c.transform.SetParent(hall.transform);

                //Destroy old wall
                Destroy(hallWall.gameObject);
                hallWall.parent = null;
            }
        }
        
        /*========HALL TO HALL CONNECT========*/ 
            
    }
    void ConstructHallway(Room creator, char cardinal)
    {
        int currentDepth = creator.depth;
        Room neighbor = Instantiate(hallwayPrefab) as Room;
        neighbor.transform.position = creator.transform.position;
        neighbor.transform.SetParent(transform);
        neighbor.SetUp(creator, currentDepth, "Hallway");

        Vector3 v = new Vector3();
        string w = "";
        float dist = 0f ;

        switch (cardinal)
        {
            //check if location is already occupied
            case 'z':
                v = Vector3.back;
                dist = Mathf.Abs(neighbor.transform.position.z - floorMinZ) / (floorSize.z);
                break;

            case 'Z':
                v = Vector3.forward;
                dist = Mathf.Abs(neighbor.transform.position.z - floorMaxZ) / (floorSize.z);
                break;

            case 'x':
                v = Vector3.back;
                dist = Mathf.Abs(neighbor.transform.position.x - floorMinX) / (floorSize.x);
                neighbor.transform.Rotate(Vector3.up, 90);
                break;

            case 'X':
                v = Vector3.forward;
                dist = Mathf.Abs(neighbor.transform.position.x - floorMaxX) / (floorSize.x);
                neighbor.transform.Rotate(Vector3.up, 90);
                break;
        }
      
        neighbor.transform.Translate(roomWidth * v);
        posToRoom.Add(neighbor.transform.position, neighbor);
        Vector3 next = neighbor.transform.position + (roomWidth * v);
        if (posToRoom.ContainsKey(next)){
            
            Connect(neighbor, posToRoom[next] as Room);
        }
        else
        {
            if (pHallway > Random.value)
            {
                ConstructHallway(neighbor, cardinal);
            }
            else ConstructRoom(neighbor, cardinal);
        }
    }
    void ConstructRoom(Room creator, char cardinal) {
        int currentDepth = creator.depth +1;
        Vector3 entryPos = creator.transform.position;
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

        neighbor.transform.Translate(roomWidth * v);
        posToRoom.Add(neighbor.transform.position, neighbor);
        if (neighbor.transform.FindChild("Walls").FindChild(w).gameObject != null)
        {
            Destroy(neighbor.transform.FindChild("Walls").FindChild(w).gameObject);
            neighbor.transform.FindChild("Walls").FindChild(w).parent = null;
        }
        SetDoors(neighbor);
        Expand(neighbor);
    }
   
    /*TODO
        Fix bug: doors are skipped over leaving gaps
    */
    //builds and connects rooms and hallways
    void Expand(Room r)
    {
        if (numRooms > roomLimit)
        {
            Debug.Log(numRooms + " is greater than " + roomLimit);
            return;
        }
        int currentDepth = r.depth + 1;
        
        
        //foreach newly generated door decide whether to build hallway or additional room
        //foreach(Transform door in r.transform.FindChild("Doors"))
        for(int i = 0; i < r.transform.FindChild("Doors").childCount; i++)
        {
            Transform door = r.transform.FindChild("Doors").GetChild(i);
            //avoid pitfalls, check how far we are from floor boundries
            /*if (Mathf.Abs(door.position.x) == floorMaxX || Mathf.Abs(door.position.z) == floorMaxZ)
            {
                Destroy(door.gameObject);
                door.parent = null;
                continue;
            }*/
            //make sure door doesn't lead into already constructed area
            Vector3 v;
            switch (door.name[0])
            {
                case 'z':
                    v = Vector3.back;
                    break;
                case 'Z':
                    v = Vector3.forward;
                    break;
                case 'x':
                    v = Vector3.left;
                    break;
                case 'X':
                    v = Vector3.right;
                    break;
                default:
                    v = Vector3.zero;
                    break;
            }
            Vector3 pos = r.transform.position + (v * roomWidth);
            //if door leads to already constructed area, connect the two
            if (posToRoom.ContainsKey(pos))
            {
                Room s = posToRoom[pos] as Room;
                Debug.Log("connecting" + r.name + " " + s.name);
                if (s.name.Substring(4).Equals("Room")) i--;
                Connect(r, s);
            }
            else
            {
                //determine whether to build hallway or room
                if (pHallway > Random.value)
                    ConstructHallway(r, door.name[0]);
                else
                    ConstructRoom(r, door.name[0]);
            }
           
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
            Debug.Log(r.name + " " + wall.name);
            //determine whether to expand on a wall based on distance to floor boundry
            switch (wall.name[0])
            {
                case 'z':
                    pDoor = Mathf.Abs(wall.position.z - floorMinZ) / (floorSize.z);
                    break;
                case 'Z':
                    pDoor = Mathf.Abs(wall.position.z - floorMaxZ) / (floorSize.z);
                    Debug.Log(r.name +"pDoorZ" + pDoor);
                    break;
                case 'x':
                    pDoor = Mathf.Abs(wall.position.x - floorMinX) / (floorSize.x);
                    break;
                case 'X':
                    pDoor = Mathf.Abs(wall.position.x - floorMaxX) / (floorSize.x);
                    Debug.Log(r.name+" pDoorX" + pDoor);
                    break;
                default:
                    pDoor = 0;
                    break;
            }
            
            //pDoor is large when there is more space to expand into
            //delete wall to allow access through door
            if(pDoor > Random.value)
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
        int rand = (int) Random.Range(0, 4-float.Epsilon);
        if(rand % 2 == 1)
        {
            entryPos.z = floorBnds[rand];
            if (rand == 3) entryPos.z += roomWidth/2;
            else entryPos.z -= roomWidth/2;
        }
        else
        {
            entryPos.x = floorBnds[rand];
            if (rand == 2) entryPos.x += roomWidth/2;
            else entryPos.x -= roomWidth/2;
        }
       
        Room entry = Instantiate(roomPrefab, entryPos, Quaternion.identity) as Room;
        entry.depth = 0;
        entry.name = "Room" + numRooms++;
        posToRoom.Add(entryPos, entry);
        entry.transform.SetParent(transform);
        SetDoors(entry);
        Expand(entry);
    }
    void BuildDungeon()
    {
        SetEntryRoom();
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
    }
	void Start () {

        SetReferences();
        BuildDungeon();
	}
    
}
