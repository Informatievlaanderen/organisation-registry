FROM mcr.microsoft.com/dotnet/runtime-deps:7.0.5

LABEL maintainer "Digitaal Vlaanderen <digitaal.vlaanderen@vlaanderen.be>"

COPY / /app
WORKDIR /app

RUN apt-get update && \
    apt-get install curl jq -y && \
    chmod +x ./init.sh

EXPOSE 9002/tcp
ENV ASPNETCORE_URLS http://*:9002

ENTRYPOINT ["./init.sh"]
