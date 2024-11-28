using FluentTesting.Common.Abstraction;

namespace FluentTesting.Elasticsearch.Options
{
    public class ElasticsearchOptions : IContainerOptions
    {
        /// <inheritdoc/>
        public bool RunAdminTool { get; set; } = true;

        /// <inheritdoc/>
        public int? Port { get; set; } = 9200;

        /// <summary>
        /// Index patterns
        /// </summary>
        public string[] IndexPatterns { get; set; } = [];
    }
}
