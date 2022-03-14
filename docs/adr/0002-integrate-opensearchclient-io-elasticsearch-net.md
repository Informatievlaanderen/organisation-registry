# 2. Integrate OpenSearch Client code instead of ElasticSearch.Net package

Date: 2022-03-14

## Status

Accepted

## Context

We discovered a memory allocation problem in our version of ElasticSearch.Net (as described [here](https://github.com/elastic/elasticsearch-net/issues/4165)). Upgrading to a newer version of ElasticSearch.Net was not an option, since newer versions prevent connecting to OpenSearch (which we recently started using in our environments instead of ElasticSearch).

> In the meantime, for anyone using one of the above distributions of OpenSearch or Elasticsearch, we do not recommend updating to the latest version of any Elastic-maintained clients, as this may cause applications to break. Anyone who has already updated their clients and is experiencing issues can resolve these issues by using any of the client versions outlined in the OpenSearch documentation. 
>
> https://aws.amazon.com/blogs/opensource/keeping-clients-of-opensearch-and-elasticsearch-compatible-with-open-source/

However, since [OpenSearch-net is not a nuget package yet](https://github.com/opensearch-project/opensearch-net/issues/25) we need to integrate the code itself into our repository if we want to use it.

## Decision

We will copy the OpenSearch-net code into our repository to replace the ElasticSearch.Net NuGet package.

## Consequences

### Code Changes

The code changes are minimal, since OpenSearch-net is supposed to be a drop-in replacement for ElasticSearch.Net.

### Bugs and security

Since we copy the code, we run the risk of not being in sync with the actual OpenSearch-net code. 
We are aware of the risks of possible bugs and security issues that might develop over time.

## Future developments
### Update the code
We'll regularly evaluate whether the main repository has changed, and if those changes should reflect in our repository.

### Wait for NuGet package
Keep an eye out for [this issue on Github](https://github.com/opensearch-project/opensearch-net/issues/25).

### Migration from OpenSearch to ElasticSearch
Look into the possibility of moving back to ElasticSearch. Besides using a proven technology (ElasticSearch.Net), there might be organisational benefits as well now that AWS ST runs an ElasticSearch instance.