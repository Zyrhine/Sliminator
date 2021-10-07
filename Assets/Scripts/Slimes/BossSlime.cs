public sealed class BossSlime : Slime
{
    protected override void Update()
    {
        if (!Alive) return;
        base.Update();
    }
}
