using Carp.Process;
public class LocationProcess : GroupProcess
{
    protected Team[] teams;

    public LocationProcess(Team[] _teams) : base(false)
    {
        teams = _teams;
    }
}
