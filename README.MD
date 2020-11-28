# Http Bucket

## Background
I use services like Requestbin to capture HTTP requests alot while developing or troubleshooting. This is my 'lightweight' take on such a service, doing the things that I need and that work in a docker/kubernetes environment.

## Features
* Requests are persisted in memory
* Urls can be shared and websockets will push updates to connected clients
* A new bucket can created by a get to /ui/{some-new-guid} or by using the button on /ui
* By default an HTTP/200 OK is returned. If you want something else just add the response status code before the bucket id.

## How to use
```
https://somehost/{HTTP-RESPOSNE-CODE}/{BASE64-ENCODED-LIST-OF '|' SEPARATED-HTTP-HEADERS}/{BASE64-ENCODED-RESPONSE-BODY}/{THE BUCKET GUID}/{whatever-url}
```

Example
```
https://somehost/201/Q29udGVudC10eXBlPWFwcGxpY2F0aW9uL2pzb24=/eyJpY2VjcmVhbXRydWNrIiA6IHRydWV9/fe6d3ace-538c-41c3-8141-c9203ab6efb0/foo
```

This would log the request and return a HTTP/201 with header "content-type: application/json" and body {"icecreamtruck" : true}

## Technology
* Dotnet 5.0 and Asp.Net Core 5
* SignalR Core

## Run in docker
docker run -p 8080:80 magohl/httpbucket:latest

Run in docker behind one (or more) reverse proxies
docker run -p 8080:80 -e ASPNETCORE_FORWARDEDHEADERS_ENABLED=true magohl/httpbucket:latest

## How does it look?
![HttpBucket_v1 0](https://user-images.githubusercontent.com/1846780/78944514-5c5a0600-7abe-11ea-8ebe-6a7cfd73ed7f.gif)

## Todo
[] Clean bucket button
[] OIDC login to create/view buckets
[] Simple DB backend
[x] Return headers and response body based on request input