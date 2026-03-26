using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DTT.BubbleShooter
{
    [CreateAssetMenu(fileName = "BubbleLevelConfigSO", menuName = "ScriptableObjects/BubbleLevelConfig", order = 0)]
    public class BubbleLevelConfigSO : ScriptableObject
    {
        [SerializeField] public BubbleLevelConfig _bubbleLevelConfig;

        public BubbleLevelConfig BubbleLevelConfig { get => _bubbleLevelConfig; }
    }


    [Serializable]
    public class BubbleLevelConfig
    {
        [SerializeField]
        [Tooltip("The probability to when spawn a new bubble, the number element of this list should match with sprite number")]
        private List<float> _bubbleColorProbability = new();

        [SerializeField]
        [Tooltip("The amount of missed shots the turret has to shot until a new row appears.")]
        private int _maxMissedShotsTillNewRow = 10;

        [SerializeField]
        [Tooltip("The amount of missed shots the turret has to shot until a new row appears.")]
        private int _minMissedShotsTillNewRow = 5;

        [SerializeField]
        [Tooltip("The amount of shot the turret has to shot until a new row appears")]
        private int _maxShotsTillNewRow = 20;

        [SerializeField]
        [Tooltip("The amount of shot the turret has to shot until a new row appears")]
        private int _minShotsTillNewRow = 10;

        [SerializeField]
        [Tooltip("The amount of time  a new row appears")]
        private float _maxTimeTillNewRow = 30;

        [SerializeField]
        [Tooltip("The amount of time until a new row appears")]
        private float _minTimeTillNewRow = 10;

        [SerializeField]
        [Tooltip("The amount of time reduce when a new row appears")]
        private float _dropTimeTillNewRow = 1;

        [Tooltip("Represents the amount of bubbles each row will have.")]
        [SerializeField] private int _gridWidth = 7;

        [SerializeField]
        [Tooltip("The height of rows the grid of bubbles will be instantiated with.")]
        private int _initialGridHeight = 7;

        [SerializeField]
        [Tooltip("The height the grid has to reach for the game to be game over.")]
        private int _gridHeightThreshold = 14;

        [SerializeField]
        [Tooltip("The minimum value a numbered bubble can have upon initalization of the grid.")]
        private int _minimumBubbleNumber = 1;

        [SerializeField]
        [Tooltip("The maximum value a numbered bubble can have upon initalization of the grid.")]
        private int _maximumBubbleNumber = 6;

        [SerializeField]
        [Tooltip("The minimum chance a bubble pair has to have from the bubble pool to be generated in the turret.")]
        private float _chanceToPopThreshold = 50.0f;

        public List<float> BubbleColorProbability { get => _bubbleColorProbability; set => _bubbleColorProbability = value; }
        public int MaxMissedShotsTillNewRow { get => _maxMissedShotsTillNewRow; set => _maxMissedShotsTillNewRow = value; }
        public int MinMissedShotsTillNewRow { get => _minMissedShotsTillNewRow; set => _minMissedShotsTillNewRow = value; }
        public int MaxShotsTillNewRow { get => _maxShotsTillNewRow; set => _maxShotsTillNewRow = value; }
        public int MinShotsTillNewRow { get => _minShotsTillNewRow; set => _minShotsTillNewRow = value; }
        public float MaxTimeTillNewRow { get => _maxTimeTillNewRow; set => _maxTimeTillNewRow = value; }
        public float MinTimeTillNewRow { get => _minTimeTillNewRow; set => _minTimeTillNewRow = value; }
        public float DropTimeTillNewRow { get => _dropTimeTillNewRow; set => _dropTimeTillNewRow = value; }
        public int GridWidth { get => _gridWidth; set => _gridWidth = value; }
        public int InitialGridHeight { get => _initialGridHeight; set => _initialGridHeight = value; }
        public int GridHeightThreshold { get => _gridHeightThreshold; set => _gridHeightThreshold = value; }
        public int MinimumBubbleNumber { get => _minimumBubbleNumber; set => _minimumBubbleNumber = value; }
        public int MaximumBubbleNumber { get => _maximumBubbleNumber; set => _maximumBubbleNumber = value; }
        public float ChanceToPopThreshold { get => _chanceToPopThreshold; set => _chanceToPopThreshold = value; }
    }
}