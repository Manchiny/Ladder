using UniRx;
using UnityEngine;

public class Stamina : MonoBehaviour
{
    public const float MaxEnergyValue = 20f;
    private const float RecoveryPerSecond = 3f;

    private HandsMover _hands;

    private bool _inited;
    private bool _touched;

    public ReactiveProperty<float> CurrentEnergy { get; private set; }

    private void Update()
    {
        if (_inited == false || _touched)
            return;

        Debug.Log(CurrentEnergy.Value);

        if (_hands.CanMove)
            RecoverEnergy(RecoveryPerSecond * Time.deltaTime);
    }

    private void OnDisable()
    {
        Game.UserInput.Touched -= SetTouchedTrue;
        Game.UserInput.Untouched -= SetTouchedFalse;

        _hands.Taked -= ForceExpendEnergy;
    }

    public void Init(HandsMover hands)
    {
        if (_inited == false)
        {

            _hands = hands;
            CurrentEnergy = new();

            Game.UserInput.Touched += SetTouchedTrue;
            Game.UserInput.Untouched += SetTouchedFalse;

            _hands.Taked += ForceExpendEnergy;

            _inited = true;
        }

        CurrentEnergy.Value = MaxEnergyValue;
    }

    private void SetTouchedTrue() => _touched = true;
    private void SetTouchedFalse() => _touched = false;

    private void RecoverEnergy(float energyCount)
    {
        if (energyCount == MaxEnergyValue)
            return;

        if (CurrentEnergy.Value + energyCount < MaxEnergyValue)
            CurrentEnergy.Value += energyCount;
        else
            CurrentEnergy.Value = MaxEnergyValue;
    }

    private void ForceExpendEnergy(LadderStep step, Hand hand)
    {
        float energyCount = Game.Player.CalculateEndValueWithBoosts<StaminaBoost>(GameConstants.BaseÑonsumptionPerStep);
        ExpandEnergy(energyCount);
    }

    private void ExpandEnergy(float energyCount)
    {
        if (energyCount == 0)
            return;

        if (CurrentEnergy.Value - energyCount >= 0)
            CurrentEnergy.Value -= energyCount;
        else
            CurrentEnergy.Value = 0;
    }
}
