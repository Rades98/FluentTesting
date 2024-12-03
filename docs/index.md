# Overview
Library providing testing mechanisms to run application in memory with needed third party dependencies like Kafka, RabbitMq, NoSQL databases, SQL

|Technology|Description| Support  |
|-|:-|:-:|
| .NET8|in memory factory| &#x2713; |
| ASP.NET|in memory factory| &#x2713; | 
| Kafka|Test container with kafka and schema registry with topics creation| &#x2713; | 
| Elasticsearch| Test container with elastic| &#x2713; |
| RabbitMQ|Test container with rabbit supporting exchanges and routing keys creation| X |
| MsSQL|Test container with SQL supporting init seed| &#x2713; |
| Redis|Test container with Redis| &#x2713; |
| Mongo|Test container with Mongo supporting init seed | &#x2713; |
| Azure Blob |Test container with Azure Blob | &#x2713; |

## Release notes


| Version | Task | Info |
|-|:-|:-:|
| 1.0.0| | Beggining of this master piece |
| 2.0.0| | Fixed namespaces |
| 2.1.0| | Added azurite - support for Azure blob |
