using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DTT.BubbleShooter
{
    public class BubbleControllerRefactor : MonoBehaviour
    {

        [Header("Init Data")]
        [SerializeField] float _flyingSpeed = 10;

        [Header("Runtime Data")]
        [SerializeField] int _bubbleValue = 0;
        [SerializeField] BubbleState _state;
        [SerializeField] float _bubbleSize;
        [SerializeField] float _bubbleMargin = 0.2f;
        [SerializeField] Vector3 _direction;
        [SerializeField] GridCell _gridCell;

        [Header("Child Component")]
        [SerializeField] SpriteRenderer _spriteRenderer;
        [SerializeField] Collider2D _collider2D;

        private BubbleGameControllerRefactor _bubbleGameController;

        public float BubbleSize { get => SpriteRenderer.sprite.bounds.size.x - _bubbleMargin; }
        public SpriteRenderer SpriteRenderer { get => _spriteRenderer; }
        public GridCell GridCell { get => _gridCell; }
        public BubbleState State { get => _state; }
        public Vector3 Direction { get => _direction; set => _direction = value; }
        public float FlyingSpeed { get => _flyingSpeed; }
        public int Value { get => _bubbleValue; }

        public void Init(int value, GridCell gridCell, BubbleGameControllerRefactor bubbleGameController)
        {
            _bubbleValue = value;
            _gridCell = gridCell;
            _bubbleGameController = bubbleGameController;
            if (_bubbleValue < 0 || _bubbleValue >= _bubbleGameController.BubbleLevelConfig.BubbleColorProbability.Count)
            {
                _bubbleValue = 0;
            }
            SpriteRenderer.sprite = _bubbleGameController.BubbleSprites[value];
        }

        public void SetGridCell(GridCell gridCell)
        {
            _gridCell = gridCell;
        }

        public void Update()
        {
            switch (_state)
            {
                case BubbleState.Pending:
                    break;
                case BubbleState.Flying:
                    transform.Translate(_bubbleGameController.Scale * _flyingSpeed * Time.deltaTime * _direction);
                    break;
                case BubbleState.Attaching:
                    break;
            }
        }

        public void ChangeToState(BubbleState state)
        {
            if (_state == state) return;
            _state = state;
            switch (_state)
            {
                case BubbleState.Pending:
                    _collider2D.isTrigger = true;
                    _spriteRenderer.sortingLayerName = "UIFront";
                    break;
                case BubbleState.Flying:
                    _collider2D.isTrigger = false;
                    _spriteRenderer.sortingLayerName = "UIFront";
                    break;
                case BubbleState.Attaching:
                    _collider2D.isTrigger = true;
                    _spriteRenderer.sortingLayerName = "Game";
                    break;
            }
        }

        [ContextMenu("Get Empty Cells Nearby")]
        public List<GridCell> GetEmptyCellsNearby()
        {
            List<GridCell> result = GetNeighborCells();
            foreach (GridCell cell in result.ToList())
            {
                if (cell.Bubble != null) result.Remove(cell);
            }
            return result;
        }

        [ContextMenu("Check Match Three")]
        public List<GridCell> CheckMatchThree()
        {
            List<GridCell> result = CheckMatchThree(new()).ToList();
            //string text = "Check Match Three: ";
            //foreach (var c in result)
            //{
            //    text += "\n " + c.ToString();
            //}
            //Debug.Log(text);
            return result;
        }
        public HashSet<GridCell> CheckMatchThree(HashSet<GridCell> existedCell)
        {
            existedCell ??= new HashSet<GridCell>();

            if (!existedCell.Add(_gridCell))
                return existedCell;

            foreach (var neighbor in GetNeighborCells())
            {
                if (neighbor.Bubble != null && neighbor.Bubble.Value == Value)
                {
                    if (!existedCell.Contains(neighbor))
                    {
                        existedCell.UnionWith(neighbor.Bubble.CheckMatchThree(existedCell));
                    }
                }
            }

            return existedCell;
        }
        private static readonly (int dx, int dy)[] OddRowOffsets = new (int, int)[]
        {
            (0, -1),  // Left
            (0, 1),   // Right
            (-1, 0),  // Up-left
            (-1, 1),  // Up-right
            (1, 0),   // Down-left
            (1, 1)    // Down-right
        };

        private static readonly (int dx, int dy)[] EvenRowOffsets = new (int, int)[]
        {
            (0, -1),   // Left
            (0, 1),    // Right
            (-1, -1),  // Up-left
            (-1, 0),   // Up-right
            (1, -1),   // Down-left
            (1, 0)     // Down-right
        };
        private List<GridCell> GetNeighborCells()
        {
            if (_gridCell.Coordinate.X == 0) return new List<GridCell>();
            var result = new List<GridCell>();
            var offsets = (_gridCell.Coordinate.X + _bubbleGameController.DropDownTurn) % 2 == 0 ? EvenRowOffsets : OddRowOffsets;

            foreach (var (dx, dy) in offsets)
            {
                int newX = _gridCell.Coordinate.X + dx;
                int newY = _gridCell.Coordinate.Y + dy;

                if (newX >= 0 && newX < _bubbleGameController.BubbleLevelConfig.GridHeightThreshold &&
                    newY >= 0 && newY < _bubbleGameController.BubbleLevelConfig.GridWidth)
                {
                    result.Add(_bubbleGameController.BubbleGridCells[newX][newY]);
                }
            }
            result.RemoveAll(x => x.Coordinate.X == 0);
            return result;
        }

        [ContextMenu("Is Connected To Top Row")]
        public bool IsConnectedToTopRow()
        {
            bool result = IsConnectedToTopRow(null);
            return result;
        }

        public bool IsConnectedToTopRow(HashSet<GridCell> visited = null)
        {
            if (_gridCell.Coordinate.X == 0) return true;
            if (visited == null)
                visited = new HashSet<GridCell>();

            GridCell currentCell = this._gridCell;
            if (visited.Contains(currentCell))
                return false;

            visited.Add(currentCell);

            if (_gridCell.Coordinate.X == 1)
                return true;

            foreach (var neighbor in GetNeighborCells())
            {
                if (neighbor.Bubble != null && !visited.Contains(neighbor))
                {
                    if (neighbor.Bubble.IsConnectedToTopRow(visited))
                        return true;
                }
            }

            return false;
        }

        [ContextMenu("PrintGrid")]
        public void PrintGrid()
        {
            Debug.Log(_gridCell);
        }

        public void ChangeDirection()
        {
            _direction = new Vector3(-1 * _direction.x, _direction.y, _direction.z);
        }

        public void AttachTop()
        {
            HashSet<GridCell> emptyCells = new();
            foreach (var cell in _bubbleGameController.BubbleGridCells[1])
            {
                if (cell.Bubble == null)
                {
                    emptyCells.Add(cell);
                }
            }
            if (emptyCells.Count > 0)
            {
                emptyCells = emptyCells
                            .OrderBy(cell => Vector3.Distance(transform.position,
                            cell.CellGameObject.transform.position)).ToHashSet();
                _bubbleGameController.Attach(this, emptyCells.ToList()[0]);
            }
            else
            {
                Debug.Log("Some thing wrong, do some reset here!");
                return;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (_state != BubbleState.Attaching) return;
            collision.TryGetComponent(out BubbleControllerRefactor otherBubble);
            if (otherBubble != null)
            {
                if (otherBubble.State == BubbleState.Flying)
                {
                    var emptyCells = GetEmptyCellsNearby();
                    if (emptyCells.Count == 0) Debug.LogError("Error, do something to reset here");
                    else
                    {
                        emptyCells = emptyCells
                            .OrderBy(cell => Vector3.Distance((cell.CellGameObject.transform.position - otherBubble.transform.position) * 0.5f + otherBubble.transform.position,
                            cell.CellGameObject.transform.position)).ToList();
                        _bubbleGameController.Attach(otherBubble, emptyCells[0]);
                    }
                }
            }
        }

        public override string ToString()
        {
            return $"{{{nameof(State)}={State.ToString()}, {nameof(Value)}={Value.ToString()}}}";
        }
    }

    [Serializable]
    public class Coordinate
    {
        [SerializeField] int _x; //column
        [SerializeField] int _y; //row
        public Coordinate(int x, int y)
        {
            _x = x;
            _y = y;
        }
        public int X { get => _x; set => _x = value; }
        public int Y { get => _y; set => _y = value; }

        public override bool Equals(object obj)
        {
            return obj is Coordinate coordinate &&
                   _x == coordinate._x &&
                   _y == coordinate._y;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_x, _y);
        }

        public override string ToString()
        {
            return $"{{{nameof(X)}={X.ToString()}, {nameof(Y)}={Y.ToString()}}}";
        }
    }

    [Serializable]
    public class AttachBubbleDistance : IComparable<AttachBubbleDistance>
    {
        [SerializeField] GridCell _targetGridCell;
        [SerializeField] float _distance;

        public GridCell TargetGridCell { get => _targetGridCell; set => _targetGridCell = value; }
        public float Distance { get => _distance; set => _distance = value; }

        public int CompareTo(AttachBubbleDistance other)
        {
            return _distance.CompareTo(other._distance);
        }

        public override string ToString()
        {
            return $"{{{nameof(TargetGridCell)}={TargetGridCell}, {nameof(Distance)}={Distance.ToString()}}}";
        }
    }
    public enum BubbleState
    {
        Pending,
        Flying,
        Attaching
    }
}