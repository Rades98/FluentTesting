# About

![icon](https://github.githubassets.com/assets/GitHub-Mark-ea2971cee799.png){: style="width:30px; height:30px;" } [GitHub repo](https://github.com/Rades98/FluentTesting)

![icon](https://content.linkedin.com/content/dam/me/business/en-us/amp/brand-site/v2/bg/LI-Bug.svg.original.svg){: style="width:30px; height:30px;" } [Radek Řezníček](https://www.linkedin.com/in/radek-%C5%99ezn%C3%AD%C4%8Dek-545638163/)

# Overview

Library providing testing mechanisms to run application in memory with needed third party dependencies like Kafka, RabbitMq, NoSQL databases, SQL

|Technology|Description| Support  |	![nuget](https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQJwDPg5pC-Uvd60lmcIl_0OBnwGG7s5KjVVw&s){: style="width:20px; height:20px;" }  nuget |
|-|:-|:-:|:- | 
| .NET8|in memory factory| &#x2713; |																[Common](https://www.nuget.org/packages/FluentTesting.Common/) |
| ASP.NET|in memory factory| &#x2713; |																[Asp](https://www.nuget.org/packages/FluentTesting.Asp/) |
| Kafka|Test container with kafka and schema registry with topics creation| &#x2713; | 				[Kafka](https://www.nuget.org/packages/FluentTesting.Kafka/) |
| Elasticsearch| Test container with elastic| &#x2713; |										    [Elasticsearch](https://www.nuget.org/packages/FluentTesting.Elasticsearch/) |
| RabbitMQ|Test container with rabbit supporting exchanges and routing keys creation| &#x2713; |	[RabbitMq](https://www.nuget.org/packages/FluentTesting.RabbitMq/) |
| MsSQL|Test container with SQL supporting init seed| &#x2713; |									[Sql](https://www.nuget.org/packages/FluentTesting.Sql/) |
| Redis|Test container with Redis| &#x2713; |														[Redis](https://www.nuget.org/packages/FluentTesting.Redis/) |
| Mongo|Test container with Mongo supporting init seed | &#x2713; |									[Mongo](https://www.nuget.org/packages/FluentTesting.Mongo/) |
| Azure Blob |Test container with Azure Blob | &#x2713; |											[Azurite](https://www.nuget.org/packages/FluentTesting.Azurite/) |
| PostgreSQL | Container with PostgreSQL supporting init seed and snapshot management | &#x2718; |	- |
| MariaDB | Container with MariaDB supporting init seed and snapshot management | &#x2718; |		- |
| Azure service bus | | &#x2718; |																	- |

## Release notes


| Version | Task | Info | 
|-|:-|:-|
| 1.0.0 | | Beggining of this master piece |
| 2.0.0 | | Fixed namespaces |
| 2.1.0 | | Added azurite - support for Azure blob |
| 2.1.2 | | Fixed azurite - mea culpa  |
| 2.3.0 | | Fixed stuff around websocket which didnt work and fixed Configuration binding with Timespans|
| 2.4.0 | [#36](https://github.com/Rades98/FluentTesting/issues/36) | Added support for RabbitMq|
| 2.4.1 | [#44](https://github.com/Rades98/FluentTesting/issues/44) | Added sql backup-restore mechanism |
| 2.4.2 | [#51](https://github.com/Rades98/FluentTesting/issues/51) | Added sql data obtaining mechanism + Directory.Build.props instead of common.props |
| 2.5.0 | [#54](https://github.com/Rades98/FluentTesting/issues/54) | Added Blob assert + dataInfo obtaining extensions + seed support |
| 2.6.0 | [#57](https://github.com/Rades98/FluentTesting/issues/57) | Added redis assert + keys/value obtaining extensions + seed support |
| 2.7.0 | [#61](https://github.com/Rades98/FluentTesting/issues/61) | Removed redundant regex needs of SetAssertionRegex |
|       | [#62](https://github.com/Rades98/FluentTesting/issues/62) | Fixed optionality of seed for Redis and Blob |
| 2.7.1 | [#65](https://github.com/Rades98/FluentTesting/issues/65) | Fixed nullable properties in SQL obtained objects |
|       | [#66](https://github.com/Rades98/FluentTesting/issues/66) | Fixed bool mapping |
|       | [#67](https://github.com/Rades98/FluentTesting/issues/67) | Added extension to obtain collection |
|       | [#68](https://github.com/Rades98/FluentTesting/issues/68) | Fixed blob work with spaces in names |
| 2.7.2 |  | Fixed SQL extensions, so they don't need to satisfy new() generic policy |
| 2.7.3 |  | SQL assertion and data retrieval nullable fields fix |
| 2.7.4 |  | Fixed custom port usage |
| 2.7.5 |  | Fixed custom port usage for Blob |
| 2.7.6 |  | Fixed SQL extension to work with NUMERIC() as decimal |
| 2.7.7 |  | Invariant culture in SQL object mapper |