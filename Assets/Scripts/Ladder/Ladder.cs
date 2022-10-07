using Assets.Scripts.Hands;
using Assets.Scripts.Levels;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Assets.Scripts.Levels.LevelConfiguration;

namespace Assets.Scripts.Ladder
{
    public class Ladder : MonoBehaviour
    {
        [SerializeField] private LadderStepFabric _fabric;
        [SerializeField] private Transform[] _borders;
        [Space]
        [SerializeField] private HandsMover _hands;

        private const float BorderExtraStepsCount = 3f;

        private LevelConfiguration _levelConfiguration;
        private List<LadderStep> _steps = new();

        private bool _inited;

        public LadderStep NextFreeStep(LadderStep lastStep) => lastStep == null ? _steps[2] : _steps.Where(step => step.Id == lastStep.Id + 1).FirstOrDefault();
        public LadderStep NextFreeStep(int lastStepId) => _steps.Where(step => step.Id == lastStepId + 1).FirstOrDefault();

        public enum LadderSide
        {
            Left,
            Right,
            Default
        }

        public void Init(LevelConfiguration levelConfiguration)
        {
            if (_levelConfiguration != null && _steps.Count > 0)
            {
                foreach (var step in _steps)
                    Destroy(step.gameObject);

                _steps.Clear();
            }

            _levelConfiguration = levelConfiguration;

            if (!_inited)
                _fabric.Init(transform);

            ConfigureLadder();
            InitHands(_inited);

            _inited = true;
        }

        public void Restart()
        {
            InitHands(true);
        }

        public LadderStep GetNearestStep(float height, int maxStepIndex)
        {
            int stepsCountToSearch = 3;
            List<LadderStep> stepsToSearch = new List<LadderStep>();

            for (int i = maxStepIndex; i > 0; i--)
            {
                if (stepsToSearch.Count >= stepsCountToSearch)
                    break;

                LadderStep step = _steps[i];

                if (step.Height <= height)
                    stepsToSearch.Add(step);
            }


            if (stepsToSearch.Count > 0)
            {
                stepsToSearch.OrderBy(step => (step.Height - height));
                return stepsToSearch.First();
            }

            return null;
        }

        public LadderStep GetStepById(int id) => _steps.Where(step => step.Id == id).FirstOrDefault();

        private void ConfigureLadder()
        {
            ResizeBorders();
            CreateSteps();
        }

        private void InitHands(bool isReinit)
        {
            _hands.Init(_steps[0], _steps[1], isReinit);
        }

        private void ResizeBorders()
        {
            float totalHeight = GameConstants.LadderDeltaStep * (_levelConfiguration.StepsCount + BorderExtraStepsCount);

            foreach (var border in _borders)
            {
                Vector3 borderScale = border.transform.localScale;
                Vector3 position = border.transform.position;
               // position.y = - GameConstants.LadderDeltaStep;

                borderScale.y = totalHeight;
                border.transform.localScale = borderScale;
              //  border.transform.position = position;
            }
        }

        private void CreateSteps()
        {
            float currentHeight = 0f;
            Vector3 position = Vector3.zero;

            for (int i = 0; i < _levelConfiguration.StepsCount; i++)
            {
                position.y = currentHeight;

                LadderStep step = null;
                LadderStepType type = _levelConfiguration.GetRandomType(i);

                step = _fabric.CreateStep(type, position, SideByStepId(i));

                currentHeight += GameConstants.LadderDeltaStep;

                step.Init(i);
                _steps.Add(step);
            }

            position.y = currentHeight;
            var finishStep = _fabric.CreateFinishStep(position);
            finishStep.Init(_levelConfiguration.StepsCount);
            _steps.Add(finishStep);
            currentHeight += GameConstants.LadderDeltaStep;

            position.y = currentHeight;
            var finishButton = _fabric.CreateFinishButton(position);
            finishButton.Init(_levelConfiguration.StepsCount + 1);
            _steps.Add(finishButton);

            _steps.OrderBy(step => step.Id);
        }

        private LadderSide SideByStepId(int stepId) => stepId % 2 == 0 ? LadderSide.Right : LadderSide.Left;
    }
}
