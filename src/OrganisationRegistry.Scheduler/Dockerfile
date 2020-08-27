FROM alpine:3.10.3

RUN apk --no-cache add curl

CMD curl -d "{ 'type': 'CheckIfDayHasPassed' }" -H "Content-Type: application/json" -H "Authorization: Bearer $API_BEARER" -X POST $API_URI -v; curl -d "{ 'type': 'SyncFromKbo' }" -H "Content-Type: application/json" -H "Authorization: Bearer $API_BEARER" -X POST $API_URI -v
