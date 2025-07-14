using System.Collections.Generic;
using UnityEngine;

public class SubDungeon
{
    public SubDungeon LeftChild => _leftChild;
    public SubDungeon RightChild => _rightChild;
    public Rect Rect => _rect;
    public Rect Room => _room;
    public List<Rect> Corridors => _corridors;
    
    private SubDungeon _leftChild, _rightChild;
    private Rect _rect;
    private Rect _room = new Rect(-1, -1, 0, 0); // i.e null
    private List<Rect> _corridors = new();

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="rect">The dimensions of the dungeon.</param>
    public SubDungeon(Rect rect)
    {
        this._rect = rect;
    }

    /// <summary>
    /// Checks if the subspace is a leaf in the BSP tree, i.e. whether it has any children.
    /// </summary>
    /// <returns>true if the node has no children; false otherwise.</returns>
    public bool IsLeaf()
    {
        return _leftChild == null && _rightChild == null;
    }

    public bool Split(int minRoomSize, int maxRoomSize)
    {
        if (!IsLeaf()) return false;

        // choose a vertical or horizontal split depending on the proportions
        // i.e. if too wide split vertically, or too long horizontally, 
        // or if nearly square choose vertical or horizontal at random
        bool splitHorizontally;

        if (_rect.width / _rect.height >= 1.25)
            splitHorizontally = false;
        else if (_rect.height / _rect.width >= 1.25)
            splitHorizontally = true;
        else
            splitHorizontally = Random.Range(0.0f, 1.0f) > 0.5;

        if (Mathf.Min(_rect.height, _rect.width) / 2 < minRoomSize) return false;

        if (splitHorizontally)
        {
            // split so that the resulting sub-dungeons widths are not too small
            // (since we are splitting horizontally) 
            int split = Random.Range(minRoomSize, (int)(_rect.width - minRoomSize));

            _leftChild = new SubDungeon(new Rect(_rect.x, _rect.y, _rect.width, split));
            _rightChild = new SubDungeon(
                new Rect(_rect.x, _rect.y + split, _rect.width, _rect.height - split));
        }
        else
        {
            int split = Random.Range(minRoomSize, (int)(_rect.height - minRoomSize));

            _leftChild = new SubDungeon(new Rect(_rect.x, _rect.y, split, _rect.height));
            _rightChild = new SubDungeon(
                new Rect(_rect.x + split, _rect.y, _rect.width - split, _rect.height));
        }

        return true;
    }

    public void CreateRoom()
    {
        _leftChild?.CreateRoom();
        _rightChild?.CreateRoom();

        if (_leftChild != null && _rightChild != null)
            CreateCorridorBetween(_leftChild, _rightChild);

        if (IsLeaf())
        {
            int roomWidth = (int)Random.Range(_rect.width / 2, _rect.width - 2);
            int roomHeight = (int)Random.Range(_rect.height / 2, _rect.height - 2);
            int roomX = (int)Random.Range(1, _rect.width - roomWidth - 1);
            int roomY = (int)Random.Range(1, _rect.height - roomHeight - 1);

            // room position will be absolute in the board, not relative to the sub-dungeon
            _room = new Rect(_rect.x + roomX, _rect.y + roomY, roomWidth, roomHeight);
        }
    }
    
    private void CreateCorridorBetween(SubDungeon left, SubDungeon right)
    {
        Rect leftRoom = left.GetRoom();
        Rect rightRoom = right.GetRoom();

        // attach the corridor to a random point in each room
        Vector2 leftPoint = new Vector2((int)Random.Range(leftRoom.x + 1, leftRoom.xMax - 1),
            (int)Random.Range(leftRoom.y + 1, leftRoom.yMax - 1));
        Vector2 rightPoint = new Vector2((int)Random.Range(rightRoom.x + 1, rightRoom.xMax - 1),
            (int)Random.Range(rightRoom.y + 1, rightRoom.yMax - 1));

        // always be sure that left point is on the left to simplify the code
        if (leftPoint.x > rightPoint.x)
            //Swap via deconstruction.
            (leftPoint, rightPoint) = (rightPoint, leftPoint);

        int width = (int)(leftPoint.x - rightPoint.x);
        int height = (int)(leftPoint.y - rightPoint.y);

        // if the points are not aligned horizontally
        if (width != 0)
        {
            // choose at random to go horizontal then vertical or the opposite
            if (Random.Range(0, 1) > 2)
            {
                // add a corridor to the right
                _corridors.Add(new Rect(leftPoint.x, leftPoint.y, Mathf.Abs(width) + 1, 1));

                // if left point is below right point go up
                // otherwise go down
                _corridors.Add(height < 0
                    ? new Rect(rightPoint.x, leftPoint.y, 1, Mathf.Abs(height)) 
                    : new Rect(rightPoint.x, leftPoint.y, 1, -Mathf.Abs(height)));
            }
            else
            {
                // go up or down
                _corridors.Add(height < 0
                    ? new Rect(leftPoint.x, leftPoint.y, 1, Mathf.Abs(height))
                    : new Rect(leftPoint.x, rightPoint.y, 1, Mathf.Abs(height)));

                // then go right
                _corridors.Add(new Rect(leftPoint.x, rightPoint.y, Mathf.Abs(width) + 1, 1));
            }
        }
        else
        {
            // if the points are aligned horizontally
            // go up or down depending on the positions
            _corridors.Add(height < 0
                ? new Rect((int)leftPoint.x, (int)leftPoint.y, 1, Mathf.Abs(height))
                : new Rect((int)rightPoint.x, (int)rightPoint.y, 1, Mathf.Abs(height)));
        }
    }

    private Rect GetRoom()
    {
        if (IsLeaf()) return _room;

        if (_leftChild != null)
        {
            Rect leftRoom = _leftChild.GetRoom();

            if (!Mathf.Approximately(leftRoom.x, -1)) return leftRoom;
        }

        if (_rightChild != null)
        {
            Rect rightRoom = _rightChild.GetRoom();

            if (!Mathf.Approximately(rightRoom.x, -1)) return rightRoom;
        }

        // workaround non-nullable structs
        return new Rect(-1, -1, 0, 0);
    }
}
