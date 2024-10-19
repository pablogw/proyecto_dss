using System;
using System.Collections.Generic;

namespace Fire_Emblem.Models
{
    public abstract class Skill
{
    public string Name { get; protected set; }
    protected List<ISkillEffect> Effects { get; } = new List<ISkillEffect>();
    protected Func<Unit, Unit, bool> ActivationCondition { get; set; }

    protected Skill(string name, Func<Unit, Unit, bool> activationCondition = null)
    {
        Name = name;
        ActivationCondition = activationCondition ?? ((_, __) => true);
    }

    public abstract void ApplyEffect(Unit unit, Unit opponent);

    public virtual bool ShouldActivate(Unit unit, Unit opponent)
    {
        Console.WriteLine($"DEBUG: Checking activation for skill: {Name}");
        Console.WriteLine($"DEBUG: Unit: {unit.Name}, CurrentHP: {unit.CurrentHP}, MaxHP: {unit.MaxHP}");
        bool response = ActivationCondition(unit, opponent);
        Console.WriteLine($"DEBUG: Skill {Name} should activate: {response}");
        return response;
    }

    public Skill AddEffect(ISkillEffect effect)
    {
        Effects.Add(effect);
        return this;
    }
}

public class BonusSkill : Skill
{
    public BonusSkill(string name, Dictionary<string, int> bonusValues, Func<Unit, Unit, bool> activationCondition = null) 
        : base(name, activationCondition)
    {
        foreach (var kvp in bonusValues)
        {
            AddEffect(new StatModifier(kvp.Key, kvp.Value, true));
        }
    }

    public override void ApplyEffect(Unit unit, Unit opponent)
    {
        foreach (var effect in Effects)
        {
            Console.WriteLine("DEBUG: ENTRAMOS A LA FUNCION APPLYEFFECT DE HYBRID");
            effect.Apply(unit, opponent);
        }
    }
}

public class PenaltySkill : Skill
{
    public PenaltySkill(string name, Dictionary<string, int> penaltyValues, Func<Unit, Unit, bool> activationCondition = null) 
        : base(name, activationCondition)
    {
        foreach (var kvp in penaltyValues)
        {
            AddEffect(new StatModifier(kvp.Key, kvp.Value, false, false));
        }
    }

    public override void ApplyEffect(Unit unit, Unit opponent)
    {
        foreach (var effect in Effects)
        {
            effect.Apply(unit, opponent);
        }
    }
}

public class NeutralizeBonusSkill : Skill
{
    public NeutralizeBonusSkill(string name, Func<Unit, Unit, bool> activationCondition = null) 
        : base(name, activationCondition)
    {
        AddEffect(new NeutralizeEffect(true, false, false));
    }

    public override void ApplyEffect(Unit unit, Unit opponent)
    {
        foreach (var effect in Effects)
        {
            effect.Apply(unit, opponent);
        }
    }
}

public class NeutralizePenaltySkill : Skill
{
    public NeutralizePenaltySkill(string name, Func<Unit, Unit, bool> activationCondition = null) 
        : base(name, activationCondition)
    {
        AddEffect(new NeutralizeEffect(false, true, true));
    }

    public override void ApplyEffect(Unit unit, Unit opponent)
    {
        foreach (var effect in Effects)
        {
            effect.Apply(unit, opponent);
        }
    }
}

public class HybridSkill : Skill
{
    private List<ISkillEffect> Effects { get; } = new List<ISkillEffect>();
    private Func<Unit, Unit, bool> ActivationCondition { get; }

    public HybridSkill(string name, Func<Unit, Unit, bool> activationCondition = null) : base(name)
    {
        ActivationCondition = activationCondition ?? ((_, __) => true);
    }

    public override void ApplyEffect(Unit unit, Unit opponent)
    {
        foreach (var effect in Effects)
        {
            effect.Apply(unit, opponent);
        }
    }

    public override bool ShouldActivate(Unit unit, Unit opponent)
    {
        return ActivationCondition(unit, opponent);
    }

    public HybridSkill AddEffect(ISkillEffect effect)
    {
        Effects.Add(effect);
        return this;
    }
}

public interface ISkillEffect
{
    void Apply(Unit unit, Unit opponent);
}

public class StatModifier : ISkillEffect
{
    private string Stat { get; }
    private int Value { get; }
    private bool IsBonus { get; }
    private bool ApplyToUnit { get; }
    private bool ApplyToFirstAttack { get; }
    private bool ApplyToFollowUp { get; }

    public StatModifier(string stat, int value, bool isBonus = true, bool applyToUnit = true, 
                        bool applyToFirstAttack = false, bool applyToFollowUp = false)
    {
        Stat = stat;
        Value = value;
        IsBonus = isBonus;
        ApplyToUnit = applyToUnit;
        ApplyToFirstAttack = applyToFirstAttack;
        ApplyToFollowUp = applyToFollowUp;
    }

    public void Apply(Unit unit, Unit opponent)
    {
        var targetUnit = ApplyToUnit ? unit : opponent;
        if (IsBonus)
        {
            targetUnit.AddBonus(Stat, Value, ApplyToFirstAttack, ApplyToFollowUp);
        }
        else
        {
            targetUnit.AddPenalty(Stat, Value, ApplyToFirstAttack, ApplyToFollowUp);
        }
    }
}

public class NeutralizeEffect : ISkillEffect
{
    private bool NeutralizeBonuses { get; }
    private bool NeutralizePenalties { get; }
    private bool ApplyToUnit { get; }

    public NeutralizeEffect(bool neutralizeBonuses, bool neutralizePenalties, bool applyToUnit = true)
    {
        NeutralizeBonuses = neutralizeBonuses;
        NeutralizePenalties = neutralizePenalties;
        ApplyToUnit = applyToUnit;
    }

    public void Apply(Unit unit, Unit opponent)
    {
        var targetUnit = ApplyToUnit ? unit : opponent;
        if (NeutralizeBonuses) targetUnit.NeutralizeAllBonuses();
        if (NeutralizePenalties) targetUnit.NeutralizeAllPenalties();
    }
}

public class CustomEffect : ISkillEffect
{
    private Action<Unit, Unit> Effect { get; }

    public CustomEffect(Action<Unit, Unit> effect)
    {
        Effect = effect;
    }

    public void Apply(Unit unit, Unit opponent)
    {
        Effect(unit, opponent);
    }
}

}