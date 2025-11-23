using NSubstitute.AutoSub;

namespace PagePlay.Tests.Infrastructure.TestBases;

public class SetupUnitTestFor<T> where T : class
{
    protected T SUT { get; private set; }
    protected AutoSubstitute Mocker { get; private set; }

    protected SetupUnitTestFor()
    {
        Mocker = new AutoSubstitute();
        SUT = Mocker.CreateInstance<T>();
    }
}
