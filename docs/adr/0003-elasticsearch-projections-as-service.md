# 1. Migrate ElasticSearch.Projections to a service

Date: 2022-05-12

## Status

Accepted

## Context

ElasticSearch.Projections currently runs as a Scheduled Task in AWS, while the code behaves like a Service.
The projections used to run in batches of X items, after which the application would exit and wait to be recreated from the Scheduled Task definition.

We changed the code recently to run continuously, polling for changes. 
This way the projections are more responsive and reliable in terms of how fast events are being processed.

A side effect of this approach is deploys for scheduled tasks don't automatically kill the application, because they're expected to exit by themselves.
Therefore the application doesn't get updated until it crashes or we stop it manually

## Decision

We're migrating ElasticSearch.Projections in AWS from a Scheduled Task to a Service.

## Consequences

- The use of DistributedMutex will no longer be needed.
- The application will automatically be stopped and replaced by new deploys.
