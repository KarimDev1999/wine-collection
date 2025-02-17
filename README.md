# **Wine Collection API**

##### **Simple ASP.NET Core Web API for managing a collection of wines**


### Steps to Run the Project:

Clone the repository

`git clone https://github.com/KarimDev1999/wine-collection.git`

`cd wine-collection`

Restore dependencies:
`dotnet restore`

Run migration:
    
    dotnet ef database update --startup-project ./Wine.API --project ./Wine.Infra

Run the application:

`cd ./Wine.API`
-> `dotnet run`

    {serverUrl}/swagger/index.html
    API Endpoints
    Wine Endpoints
    GET	/api/wines	Get all wines
    GET	/api/wines/{id}	Get a wine by ID
    POST /api/wines	Add a new wine (Requires JWT auth)
    PUT	/api/wines/{id}	Update a wine 
    DELETE	/api/wines/{id}	Delete a wine 

    Authentication Endpoint
    HTTP Method	Endpoint	Description
    POST	/api/token/generate	Generate a JWT token for testing

**Authentication:**
To access secured endpoint (POST), you need a valid JWT token:

    Generate a token:
        Send a POST request to /api/token/generate.
        Example response:
        {
          "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
        }


Then pass this token to swagger authentication input and make a call to create a wine.