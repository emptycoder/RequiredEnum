using RequiredEnum.Sample;

var test = (RequiredNumbers) Random.Shared.Next(0, Enum.GetNames(typeof(RequiredNumbers)).Length);
switch (test)
{
    case RequiredNumbers.Zero:
        break;
    case RequiredNumbers.One:
        break;
    case RequiredNumbers.Two:
        break;
    default:
        throw new ArgumentOutOfRangeException();
}

namespace RequiredEnum.Sample
{
	internal enum RequiredNumbers
	{
		Zero,
		One,
		Two
	}
}
