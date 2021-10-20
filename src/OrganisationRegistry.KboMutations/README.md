# OrganisationRegistry.KboMutations

The KboMutations program is a scheduled program, designed to fetch the changes (or mutations) in organisation data.

Using an ftp provided by Magda as a gateway to the KBO, we periodically fetch csv files containing possibly relevant changes.

The program consists of 3 major steps:
1. Fetch all mutation files provided by Magda.
2. Persist all mutations in each file to a queue.
3. Archive these files, so we don't process them again in a subsequent run.

The queued mutations are later picked up by a scheduled process in `OrganisationRegistry.Api`.

## Certificates

Magda requires the use of certificates when connecting to the ftp server.

For local development, these certificates can be specified by path (see KboMutations configuration).

In production settings, these are provided via the Parameter Store as env variables to the docker image.
From there, the base64 encoded certs are saved as local files (see `init.sh` and KboMutations configuration).

## Ftp interaction

Magda requires the use of sftp to communicate with the ftp server.

Since at the time of writing there no viable .NET core libraries to communicate with sftp,
we've opted to marshall `curl` through our own code.
