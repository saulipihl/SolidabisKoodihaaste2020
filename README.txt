This file covers both the backend and client.

~~~~~~~~ USED TECHNOLOGIES ~~~~~~~~
 - Backend: .NET Framework 4.6.1, MVC
 - Client: Angular 9, Typescript, Sass

~~~~~~~~ INSTALLATION ~~~~~~~~

Have only used/tested the system locally.

Get the code:
git clone https://github.com/saulipihl/SolidabisKoodihaaste2020.git

Backend can be started either in Visual Studio, or set up in IIS after building it.
Ensure that the following application settings are configured in web.config:
 - "BusDataFileName" = "reittiopas.json" (where the bus data is located)
 - "BusStopLocationsFileName" = "bus_stop_locations.json" (locations of the bus stops on the map)
 - (in customHeaders section) "Access-Control-Allow-Origin" = add the url of the client here so that the requests are accepted

Client can be put running in IIS or Angular CLI can be used. With it, typing ng serve in command prompt.
Ensure that in the files in src/environments "apiUrl" is set to point to the address of the backend + 'api/', e.g. 'http://localhost:57966/api/'

~~~~~~~~ SOLUTION ~~~~~~~~
The structure of the solution is simple: client that fetches data from the backend. The backend reads the data from the provided json file on each api call to ensure if data changes.
In backend, there is only one controller that has the endpoints for the client. The client can ask fetch bus stop or calculate the shortest route.
The algorithm to calculate the shortest bus route is documented in RouteCalculator.cs.

I haven't done graphical/drawing front-end stuff before, so implementing the client was a challenge. There is just one image of the map, which can be zoomed in or out. This is handled by changing the image's width. The bus stops are generated on it with the locations that are defined in the backend.
I created some features to enhance the user experience, like loading symbols, bus stop validation and error handling.