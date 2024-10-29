namespace Testing.Common.Abstraction
{
	public interface IContainerOptions
	{
		/// <summary>
		/// Port
		/// </summary>
		public int? Port { get; set; }

		/// <summary>
		/// Run admin too - can run in debug mode only
		/// </summary>
		public bool RunAdminTool { get; set; }
	}
}
