I often use services Requestbin or similar to capture and look at HTTP requests whilde developing or troubleshooting microservices/interfaces. Sometimes they lack features i need or are not suitable for onprem/container/kubernetes deployment. Other times they are just to complex for a quick use.

This is my own take on such a service doing the things that I (often) need.

Currently the requests are persisted in memory only. Might add database support through SQLite och simliar.

Asp.Net Core 3.1
SignalR Core

Run in docker
docker run -p 8080:80 magohl/httpbucket:0.5

Run in docker behind one (or more) reverse proxies
docker run -p 8080:80 -e ASPNETCORE_FORWARDEDHEADERS_ENABLED=true magohl/httpbucket:0.5
