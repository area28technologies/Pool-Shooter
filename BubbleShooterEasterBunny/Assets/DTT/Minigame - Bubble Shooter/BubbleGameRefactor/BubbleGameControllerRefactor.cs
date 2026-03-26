using A28.PlatformSdk;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace DTT.BubbleShooter
{
    public class BubbleGameControllerRefactor : MonoBehaviour
    {
        [Header("Init Data")]
        [SerializeField] BubbleLevelConfigSO _bubbleLevelConfigSO;
        [SerializeField] List<Sprite> _bubbleSprites = new List<Sprite>();
        [SerializeField] float _rowMargin = 0.15f;

        [Header("Runtime Data")]
        [SerializeField] BubbleLevelConfig _bubbleLevelConfig;
        [SerializeField] int _score = 0;
        [SerializeField] List<List<GridCell>> _bubbleGridCells = new();
        [SerializeField] float _cellSize = 1.0f;
        [SerializeField] float _trueGridWidth = 1f;
        [SerializeField] float _scale = 1f;
        [SerializeField] float _wallScale = 1f;
        [SerializeField] int _dropDownTurn = 0;
        [SerializeField] float _totalProbability = 0;
        [SerializeField] GameState _gameState = GameState.Playing;
        [SerializeField] Coroutine _warningCoroutine;
        [SerializeField] int _currentMissedShotsTillNewRow;
        [SerializeField] int _missedShotsCount;
        [SerializeField] int _currentShotsTillNewRow;
        [SerializeField] int _shotsCount;
        [SerializeField] float _currentTimeTillNewRow;
        [SerializeField] float _timeCount;
        [SerializeField] Coroutine _timeDownTillNewRowCoroutine;

        [Header("Prefab")]
        [SerializeField] GameObject _gridPrefab;
        [SerializeField] BubbleControllerRefactor _bubblePrefab;
        [SerializeField] ParticleSystem _bubblePopParticle;

        [Header("Child Component")]
        [SerializeField] Transform _bubbleGridHolder;
        [SerializeField] Transform _wallHolder;
        [SerializeField] Transform _warningLine;
        [SerializeField] Animator _warningAnimator;
        [SerializeField] List<WallControllerRefactor> _walls;
        [SerializeField] TurretControllerRefactor _turret;
        [SerializeField] ScoreDisplayControllerRefactor _scoreDisplay;
        [SerializeField] GameOverDisplayControllerRefactor _gameOverDisplay;

        [Header("Safe Rect UI")]
        [SerializeField] RectTransform _gridTopSafeRectUI;
        [SerializeField] RectTransform _gridBottomSafeRectUI;
        [SerializeField] RectTransform _gridLeftSafeRectUI;
        [SerializeField] RectTransform _gridRightSafeRectUI;

        [Header("Audio")]
        [SerializeField] List<AudioClip> _ballLandAudioClips;
        [SerializeField] List<AudioClip> _ballBoundAudioClips;
        [SerializeField] List<AudioClip> _shotAudioClips;
        [SerializeField] List<AudioClip> _attachAudioClips;
        [SerializeField] List<AudioClip> _cancleAudioClips;
        [SerializeField] List<AudioClip> _finishAudioClips;
        [SerializeField] List<AudioClip> _swapAudioClips;
        [SerializeField] AudioSource _warningAudioSource;
        [SerializeField] List<AudioClip> _warningAudioClips;

        public List<Sprite> BubbleSprites { get => _bubbleSprites; set => _bubbleSprites = value; }
        public BubbleControllerRefactor BubblePrefab { get => _bubblePrefab; }
        public Transform BubbleGridHolder { get => _bubbleGridHolder; }
        public BubbleLevelConfig BubbleLevelConfig { get => _bubbleLevelConfig; }
        public List<List<GridCell>> BubbleGridCells { get => _bubbleGridCells; }
        public int DropDownTurn { get => _dropDownTurn; }
        public GameState GameState
        {
            get => _gameState; set
            {
                _gameState = value;
            }
        }
        public float Scale { get => _scale; }
        public int Score { get => _score; }
        public List<AudioClip> BallLandAudioClips { get => _ballLandAudioClips; }
        public List<AudioClip> BallBoundAudioClips { get => _ballBoundAudioClips; }
        public List<AudioClip> ShotAudioClips { get => _shotAudioClips; }
        public List<AudioClip> AttachAudioClips { get => _attachAudioClips; }
        public List<AudioClip> CancleAudioClips { get => _cancleAudioClips; }
        public List<AudioClip> FinishAudioClips { get => _finishAudioClips; }
        public List<AudioClip> WarningAudioClips { get => _warningAudioClips; }
        public List<AudioClip> SwapAudioClips { get => _swapAudioClips; }
        public int MissedShotsCount
        {
            get => _missedShotsCount; set
            {
                _missedShotsCount = value;
                CheckNextDifficult();
            }
        }
        public int ShotsCount
        {
            get => _shotsCount; set
            {
                _shotsCount = value;
                CheckNextDifficult();
            }
        }

        public void OnEnable()
        {
            Init();
        }

        public void Init()
        {
            GameState = GameState.Starting;
            _bubbleLevelConfig = Instantiate(_bubbleLevelConfigSO).BubbleLevelConfig;
            _cellSize = BubblePrefab.BubbleSize;
            InitGridAndTurret();
            InitBubbleColorProbability();
            InitWarningLine();
            InitLevelDifficult();
            _dropDownTurn = 0;

            A28Sdk.Instance.StartRound((success) =>
            {
                if (success)
                {
                    StartCoroutine(OnStartRound());
                }
            });

            IEnumerator OnStartRound()
            {
                yield return new WaitForSeconds(0.6f);
                GameState = GameState.Playing;
            }
        }

        public void InitGridAndTurret()
        {
            _bubbleGridCells.Clear();
            for (int i = 0; i < _bubbleLevelConfig.GridHeightThreshold; i++)
            {
                _bubbleGridCells.Add(new());
                for (int j = 0; j < _bubbleLevelConfig.GridWidth; j++)
                {
                    _bubbleGridCells[i].Add(new());
                    _bubbleGridCells[i][j].CellGameObject = ObjectPooling.Instance.SpawnObject(_gridPrefab, _bubbleGridHolder, new Vector2(j * _cellSize, -i * (_cellSize - _rowMargin)), Quaternion.identity).transform;
                    _bubbleGridCells[i][j].CellGameObject.name = $"GridCell {i} {j}";
                    _bubbleGridCells[i][j].Coordinate = new(i, j);

                    if (i % 2 == 1) _bubbleGridCells[i][j].CellGameObject.localPosition += new Vector3(_cellSize / 2, 0);
                    if (i < _bubbleLevelConfig.InitialGridHeight)
                    {
                        _bubbleGridCells[i][j].Bubble = ObjectPooling.Instance.SpawnObject(BubblePrefab.gameObject, _bubbleGridCells[i][j].CellGameObject, Vector3.zero, Quaternion.identity).GetComponent<BubbleControllerRefactor>();
                        _bubbleGridCells[i][j].Bubble.Init(GetBubbleBaseOnProbability(), _bubbleGridCells[i][j], this);
                        _bubbleGridCells[i][j].Bubble.ChangeToState(BubbleState.Attaching);
                    }
                }
            }

            _trueGridWidth = (_bubbleLevelConfig.GridWidth + 0.5f) * _cellSize;
            var gridScreenWidth = _gridRightSafeRectUI.transform.position.x - _gridLeftSafeRectUI.transform.position.x;

            _scale = (gridScreenWidth / _trueGridWidth);
            _wallScale = _scale * ((float)_bubbleLevelConfig.GridWidth / 7f);

            _bubbleGridHolder.localScale = Vector3.one * _scale;
            _bubbleGridHolder.position = new Vector3(_gridLeftSafeRectUI.transform.position.x + _cellSize * 0.5f * _scale,
                                             _gridTopSafeRectUI.transform.position.y - _cellSize * 0.5f * _scale + _cellSize * _scale);

            _turret.transform.localScale = Vector3.one * _scale;
            Vector3 turretPosition = _gridBottomSafeRectUI.transform.position;
            turretPosition.z = 0f;
            _turret.transform.position = turretPosition;
            _turret.Init(this);

            _wallHolder.localScale = Vector3.one * _wallScale;
            foreach (WallControllerRefactor wall in _walls)
            {
                wall.Init(this);
            }
        }

        public void InitWarningLine()
        {
            _scoreDisplay.Init(this);

            _warningLine.transform.position = new(
                    _warningLine.transform.position.x,
                    _bubbleGridCells[_bubbleGridCells.Count - 2][0].CellGameObject.transform.position.y - _cellSize * _scale / 2f,
                    _warningLine.transform.position.z
                );
        }

        public void InitBubbleColorProbability()
        {
            if (_bubbleSprites.Count != _bubbleLevelConfig.BubbleColorProbability.Count)
            {
                Debug.LogError("Bubble Sprite and Probability number does not match!");
                return;
            }
            _totalProbability = _bubbleLevelConfig.BubbleColorProbability.Sum(probability => probability);

        }

        public void ChangeScreenScaleHandle(float width)
        {
            _scale = (width / _trueGridWidth);
            _wallScale = _scale * ((float)_bubbleLevelConfig.GridWidth / 7f);
            _bubbleGridHolder.localScale = Vector3.one * _scale;
            _bubbleGridHolder.position = new Vector3(_gridLeftSafeRectUI.transform.position.x + _cellSize * 0.5f * _scale,
                                             _gridTopSafeRectUI.transform.position.y - _cellSize * 0.5f * _scale + _cellSize * _scale);

            _turret.transform.localScale = Vector3.one * _scale;
            Vector3 turretPosition = _gridBottomSafeRectUI.transform.position;
            turretPosition.z = 0f;
            _turret.transform.position = turretPosition;
            _turret.Init(this);

            _wallHolder.localScale = Vector3.one * _wallScale;

        }

        #region Level Difficulty
        private void InitLevelDifficult()
        {
            _currentMissedShotsTillNewRow = _bubbleLevelConfig.MaxMissedShotsTillNewRow;
            _currentShotsTillNewRow = _bubbleLevelConfig.MaxShotsTillNewRow;
            _currentTimeTillNewRow = _bubbleLevelConfig.MaxTimeTillNewRow;
            _missedShotsCount = 0;
            _shotsCount = 0;
            _timeCount = 0;
            _timeDownTillNewRowCoroutine = StartCoroutine(TimeDownTillNewRow());
        }

        public void CheckNextDifficult()
        {
            if (_shotsCount >= _currentShotsTillNewRow || _missedShotsCount >= _currentMissedShotsTillNewRow)
            {
                StartNextDifficult();
            }
        }
        public void StartNextDifficult()
        {
            _currentMissedShotsTillNewRow -= 1;
            if (_currentMissedShotsTillNewRow < _bubbleLevelConfig.MinMissedShotsTillNewRow) _currentMissedShotsTillNewRow = _bubbleLevelConfig.MinMissedShotsTillNewRow;

            _currentShotsTillNewRow -= 1;
            if (_currentShotsTillNewRow < _bubbleLevelConfig.MinShotsTillNewRow) _currentShotsTillNewRow = _bubbleLevelConfig.MinShotsTillNewRow;

            _currentTimeTillNewRow -= _bubbleLevelConfig.DropTimeTillNewRow;
            if (_currentTimeTillNewRow < _bubbleLevelConfig.MinTimeTillNewRow) _currentTimeTillNewRow = _bubbleLevelConfig.MinTimeTillNewRow;

            _missedShotsCount = 0;
            _shotsCount = 0;
            _timeCount = 0;

            if (_timeDownTillNewRowCoroutine != null) StopCoroutine(_timeDownTillNewRowCoroutine);
            _timeDownTillNewRowCoroutine = StartCoroutine(TimeDownTillNewRow());

            DropDown();
        }

        IEnumerator TimeDownTillNewRow()
        {
            yield return new WaitForSeconds(1f);
            _timeCount++;
            if (_timeCount >= _currentTimeTillNewRow)
            {
                StartNextDifficult();
            }
            _timeDownTillNewRowCoroutine = StartCoroutine(TimeDownTillNewRow());
        }

        #endregion

        #region Warning
        private void StartWarningCheck()
        {
            if (_warningCoroutine != null) StopCoroutine(_warningCoroutine);
            _warningCoroutine = StartCoroutine(WarningCoroutine());
        }
        private void StopWarningCheck()
        {
            if (_warningCoroutine != null) StopCoroutine(_warningCoroutine);
            _warningAudioSource.Stop();
        }
        IEnumerator WarningCoroutine()
        {
            yield return new WaitForSeconds(0.1f);
            bool isWarning = false;
            for (int j = 0; j < _bubbleGridCells[_bubbleGridCells.Count - 2].Count; j++)
            {
                if (_bubbleGridCells[_bubbleGridCells.Count - 2][j].Bubble != null)
                {
                    isWarning = true;
                    break;
                }
            }
            if (isWarning)
            {
                if (!_warningAudioSource.isPlaying)
                {
                    int random = Random.Range(0, _warningAudioClips.Count);
                    _warningAudioSource.clip = _warningAudioClips[random];
                    _warningAudioSource.volume = RCGSoundSetting.Instance.CurrentSoundVolume;
                    _warningAudioSource.Play();
                    _warningAnimator.Play("Twinkling");
                }
            }
            else
            {
                _warningAudioSource.Stop();
                _warningAnimator.Play("Normal");
                StopCoroutine(_warningCoroutine);
            }
            _warningCoroutine = StartCoroutine(WarningCoroutine());
        }
        #endregion

        #region Grid and Bubble Logic
        public int GetBubbleBaseOnProbability()
        {
            float roll = Random.Range(0, 100);
            float cumulative = 0f;

            for (int i = 0; i < _bubbleLevelConfig.BubbleColorProbability.Count; i++)
            {
                cumulative += _bubbleLevelConfig.BubbleColorProbability[i];
                if (roll < cumulative)
                    return i;
            }
            return 0;
        }
        
        public void DropDown()
        {
            _dropDownTurn++;
            if (CheckEndGame()) return;

            for (int i = _bubbleLevelConfig.GridHeightThreshold - 1; i > 0; i--)
            {
                for (int j = 0; j < _bubbleLevelConfig.GridWidth; j++)
                {
                    _bubbleGridCells[i][j].Bubble = _bubbleGridCells[i - 1][j].Bubble;
                }
            }
            for (int i = 0; i < _bubbleLevelConfig.GridHeightThreshold; i++)
            {
                for (int j = 0; j < _bubbleLevelConfig.GridWidth; j++)
                {
                    _bubbleGridCells[i][j].CellGameObject.transform.localPosition = new Vector2(j * _cellSize, -i * (_cellSize - _rowMargin));
                    if ((i + _dropDownTurn) % 2 == 1) _bubbleGridCells[i][j].CellGameObject.localPosition += new Vector3(_cellSize / 2, 0);
                    if (_bubbleGridCells[i][j].Bubble != null)
                    {
                        _bubbleGridCells[i][j].Bubble.SetGridCell(_bubbleGridCells[i][j]);
                        _bubbleGridCells[i][j].Bubble.transform.localPosition = new(0, _cellSize / 2f, 0);
                        _bubbleGridCells[i][j].Bubble.transform.DOLocalMove(Vector3.zero, 0.5f);
                        _bubbleGridCells[i][j].Bubble.SpriteRenderer.sortingLayerName = "Game";
                    }
                }
            }

            for (int j = 0; j < _bubbleLevelConfig.GridWidth; j++)
            {
                _bubbleGridCells[0][j].Bubble = ObjectPooling.Instance.SpawnObject(BubblePrefab.gameObject, _bubbleGridCells[0][j].CellGameObject, Vector3.zero, Quaternion.identity).GetComponent<BubbleControllerRefactor>();
                _bubbleGridCells[0][j].Bubble.Init(GetBubbleBaseOnProbability(), _bubbleGridCells[0][j], this);
                _bubbleGridCells[0][j].Bubble.SpriteRenderer.sortingLayerName = "Game";
                _bubbleGridCells[0][j].Bubble.ChangeToState(BubbleState.Attaching);
            }
            StartWarningCheck(); 
            CheckEndGame();
        }
        IEnumerator DefaultDropDownCoroutine()
        {
            yield return new WaitForSeconds(10);
            yield return new WaitUntil(() => _turret.IsShootable);
            DropDown();
            yield return new WaitForSeconds(0.6f);
            for (int i = 0; i < _bubbleLevelConfig.GridHeightThreshold; i++)
            {
                for (int j = 0; j < _bubbleLevelConfig.GridWidth; j++)
                {
                    if (_bubbleGridCells[i][j].Bubble != null)
                    {
                        _bubbleGridCells[i][j].Bubble.transform.localPosition = Vector3.zero;
                    }
                }
            }
            StartCoroutine(DefaultDropDownCoroutine());
        }
        public void Attach(BubbleControllerRefactor bubble, GridCell cell)
        {
            cell.Bubble = bubble;
            bubble.ChangeToState(BubbleState.Attaching);
            bubble.SetGridCell(cell);
            cell.Bubble.transform.DOLocalMove(Vector3.zero, 0.1f / cell.Bubble.FlyingSpeed);
            RCGSoundSetting.Instance.PlaySFX(AttachAudioClips);
            StartWarningCheck();
            StartCoroutine(ScoreMatchThree(cell));
        }
        private IEnumerator ScoreMatchThree(GridCell cell)
        {
            yield return new WaitForSeconds(0.1f / cell.Bubble.FlyingSpeed + 0.01f);
            int popBubbleCount = 0;
            var matchThreeCheckList = cell.Bubble.CheckMatchThree();
            if (matchThreeCheckList.Count >= 3)
            {
                foreach (var c in matchThreeCheckList)
                {
                    PlayBubblePopParticle(c.Bubble.transform.position);
                    ObjectPooling.Instance.ReturnObjectToPool(c.Bubble.gameObject);
                    c.Bubble = null;
                    popBubbleCount++;
                }
                for (int i = 0; i < _bubbleLevelConfig.GridHeightThreshold; i++)
                {
                    for (int j = 0; j < _bubbleLevelConfig.GridWidth; j++)
                    {
                        if (_bubbleGridCells[i][j].Bubble != null)
                        {
                            if (!_bubbleGridCells[i][j].Bubble.IsConnectedToTopRow())
                            {
                                PlayBubblePopParticle(_bubbleGridCells[i][j].Bubble.transform.position);
                                ObjectPooling.Instance.ReturnObjectToPool(_bubbleGridCells[i][j].Bubble.gameObject);
                                _bubbleGridCells[i][j].Bubble = null;
                                popBubbleCount++;
                            }
                        }
                    }
                }
                _score += CalculateScore(popBubbleCount);
            }
            else
            {
                MissedShotsCount++;
            }

            A28Sdk.Instance.TrackRoundEvent(_score);

            //_scoreDisplay.UpdateDisplayData();
            if (CheckEndGame()) yield break;
            _turret.Reload();
        }
        public static int CalculateScore(int numberBubblePop)
        {
            int result = numberBubblePop * (10 + ((int)(numberBubblePop / 2f) - 1) * 5);
            return result;
        }
        private void PlayBubblePopParticle(Vector3 position)
        {
            StartCoroutine(PlayBubblePopParticleCoroutine(position));
        }
        IEnumerator PlayBubblePopParticleCoroutine(Vector3 position)
        {
            var particle = ObjectPooling.Instance.SpawnObject(_bubblePopParticle.gameObject, _bubbleGridHolder, Vector3.zero, Quaternion.identity).GetComponent<ParticleSystem>();
            particle.transform.position = position;
            particle.Play();
            yield return new WaitUntil(() => particle.isStopped);
            ObjectPooling.Instance.ReturnObjectToPool(particle.gameObject);
        }
        #endregion

        #region End Game
        private bool CheckEndGame()
        {
            for (int j = 0; j < _bubbleLevelConfig.GridWidth; j++)
            {
                if (_bubbleGridCells[_bubbleGridCells.Count - 1][j].Bubble != null)
                {
                    if (GameState != GameState.Ending)
                    {
                        GameState = GameState.Ending;
                        StopWarningCheck();
                        RCGSoundSetting.Instance.PlaySFX(FinishAudioClips);
                        StartCoroutine(EndGameCoroutine());
                        return true;
                    }
                }
            }
            return false;
        }
        IEnumerator EndGameCoroutine()
        {
            yield return new WaitForSeconds(1f);
            A28Sdk.Instance.EndRound(Score, () =>
            {
                SceneManager.LoadScene("Main");
            });

            _gameOverDisplay.gameObject.SetActive(true);
            _gameOverDisplay.Init(Score);
        }
        #endregion

    }

    [Serializable]
    public class GridCell
    {
        [SerializeField] Transform _cellGameObject;
        [SerializeField] BubbleControllerRefactor _bubble;
        [SerializeField] Coordinate _coordinate;

        public Transform CellGameObject { get => _cellGameObject; set => _cellGameObject = value; }
        public BubbleControllerRefactor Bubble
        {
            get
            {
                return _bubble;
            }
            set
            {
                _bubble = value;
                if (_bubble != null) _bubble.transform.parent = _cellGameObject;
            }
        }

        public Coordinate Coordinate { get => _coordinate; set => _coordinate = value; }

        public override bool Equals(object obj)
        {
            return obj is GridCell cell &&
                   EqualityComparer<Coordinate>.Default.Equals(_coordinate, cell._coordinate);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_coordinate);
        }
        public override string ToString()
        {
            return $"{{{nameof(CellGameObject)}={CellGameObject}, {nameof(Bubble)}={Bubble}, {nameof(Coordinate)}={Coordinate}}}";
        }
    }

    public enum GameState
    {
        Starting,
        Playing,
        Ending
    }
}