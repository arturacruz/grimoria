public class BiteAbility : Ability
{
    public override string description => "Deal damage.";
    public override float levelToValueRatio => 1;

    public BiteAbility(Creature creature)
    {
        owner = creature;
    }
    
    public override void DoOnStart(Board allies, Board enemies) {}

    public override void DoAbility(Board allies, Board enemies)
    {
        //BattleManager.Instance.GetTarget(owner);
    }
}