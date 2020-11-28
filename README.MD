# Http Bucket

## Background
I use services like Requestbin to capture HTTP requests alot while developing or troubleshooting. This is my 'lightweight' take on such a service, doing the things that I need and that work in a docker/kubernetes environment.

## Features
* Requests persisted in memory
* Websockets will push updates to connected clients
* By default an HTTP/200 OK is returned. If you want something else just add the response status code before the bucket id.
* GET, POST, PUT, DELETE

## How to use

### Create bucket
Create a new bucket by doing a simple get to /ui/{some-new-guid} or by using the 'button' on /ui

### UI
```
https://somehost/ui/{BUCKET-GUID}
```

### Return custom Http Response Code
This request url would log the request and return a HTTP/405 with an empty body
```
https://somehost/{HTTP-RESPOSNE-CODE}/{BUCKET-GUID}/{whatever-url}
```
```
https://somehost/405/fe6d3ace-538c-41c3-8141-c9203ab6efb0/foo/bar
```

### Return custom Http Response code, headers and body
This request URL would log the request and return a HTTP/201 with header "content-type: application/json" and body {"icecreamtruck" : true}
```
https://somehost/{HTTP-RESPOSNE-CODE}/{BASE64-ENCODED-LIST-OF '|' SEPARATED-HTTP-HEADERS}/{BASE64-ENCODED-RESPONSE-BODY}/{BUCKET-GUID}/{whatever-url}
```
```
https://somehost/201/Q29udGVudC10eXBlPWFwcGxpY2F0aW9uL2pzb24=/eyJpY2VjcmVhbXRydWNrIiA6IHRydWV9/fe6d3ace-538c-41c3-8141-c9203ab6efb0/foo
```



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
- Clean bucket button
- OIDC login to create/view buckets
- Simple DB backend
- ~~Return headers and response body based on request input~~
