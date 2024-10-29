namespace Samples.AspApp.Repos
{
	public interface ISomeRepo
	{
		Task<(int IntValue, string StringValue)> GetDataAsync(CancellationToken cancellationToken);
	}
}
