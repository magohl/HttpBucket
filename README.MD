# Http Bucket

## Background
I use services like Requestbin to capture HTTP requests alot while developing or troubleshooting. This is my 'lightweight' take on such a service, doing the things that I need and that work in a docker/kubernetes environment.

## Features
* Requests are persisted in memory. (Might add SQLite or MongoDB later)
* Urls can be shared and websockets will push updates to connected clients
* A new bucket can created by a get to /ui/{some-new-guid} or by using the button on /ui
* By default an HTTP/200 OK is returned. If you want something else just add the response status code before the bucket id.

## Technology
* Asp.Net Core 3.1
* SignalR Core

## Run in docker (examples)
Run in docker
docker run -p 8080:80 magohl/httpbucket:latest

Run in docker behind one (or more) reverse proxies
docker run -p 8080:80 -e ASPNETCORE_FORWARDEDHEADERS_ENABLED=true magohl/httpbucket:latest

## How does it look?
![HttpBucket_v1 0](https://user-images.githubusercontent.com/1846780/78944514-5c5a0600-7abe-11ea-8ebe-6a7cfd73ed7f.gif)

## Todo
* Clean bucket button
* OIDC login to create/view buckets
* Return small payloads in body using parameters