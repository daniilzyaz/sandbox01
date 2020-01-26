INTEGRATION

There are two ways to integrate additional user interfaces, either through Web API or Component.

1. Web API:
The Web API integration is a lightweight integration,- minimum effort required.
This Web Api has one call to get the processes and one websocket channel for two-way communication, 
over which the high load warning events are received, they are listed respectively below.
- http://somehost/api/processes
- ws://somehost/ws

2. Component:
Integration with component is more laborious, but more flexible and saves the network calls, in case of desktop applications Integration.
Component integration require reference to X.Monitor.Core, this component has public MonitorService with all the necessary methods to get the processes information.
	
	
NOTES
- Pinging support needs to be added to keep the track of WebSocket connections.
- The code is only a proof of concept and is not optimized.
   

	