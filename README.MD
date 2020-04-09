# Information
I tend to use services like Requestbin to capture and disect HTTP requests while developing or troubleshooting microservices or interfaces. Others have either lacked features, being way to complex or just not suitable in onprem/container/kubernetes environment.

This is 'my take' on such a service in a lightweight fashion, doing the things that I need.

Currently requests are persisted in memory only. Might add SQLite or MongoDB later.

![HttpBucket_v1 0](https://user-images.githubusercontent.com/1846780/78944514-5c5a0600-7abe-11ea-8ebe-6a7cfd73ed7f.gif)


## Technology
- Asp.Net Core 3.1
- SignalR Core

# Docker (examples)
Run in docker
docker run -p 8080:80 magohl/httpbucket:0.5

Run in docker behind one (or more) reverse proxies
docker run -p 8080:80 -e ASPNETCORE_FORWARDEDHEADERS_ENABLED=true magohl/httpbucket:0.5
