**EV Charging Session Management System**

**Overview**

A real-time EV charging session management system using React and .NET 7.0 that communicates via a private MQTT broker (Eclipse Mosquitto). The system tracks charging sessions, stores session data in MongoDB, and updates the charging status in real-time on the front end. The entire system is containerized using Docker.

**Technologies Used**
•	Backend: .NET 7, MongoDB
•	Frontend: React
•	Messaging: MQTT 2.0.15 (enabled for WebSocket support)
•	Containerization: Docker, Docker Compose

**Installation & Setup**
**Prerequisites**
Ensure you have the following installed:
•	Docker
•	.NET 7 SDK (for local backend development)
•	Node.js & npm (for local frontend development)
•	MongoDB 

1. Clone the Repository
  git clone https://github.com/keerthanab196/EVChargingSession.git
  cd EVChargingSession
2. Setup Backend (.NET 7 Web API)
  1.	Navigate to the backend folder:
      cd EVChargingSessionWebAPI/EVChargingSessionWebAPI
  2.	Update appsettings.json with MongoDB details:
      	{
  	    "MongoDB": {
        "ConnectionString": "mongodb://localhost:27017",
         "DatabaseName": "EVChargingDB"
           }
        }
  3.	Restore dependencies and run:
       dotnet restore
       dotnet run
3. Setup Frontend (React App)
  1.	Navigate to the frontend folder:
      cd FrontEnd/ev-charging-app
  2.	Install dependencies:
      npm install
  3.	Run the frontend:
      npm start
4. Setup MQTT Broker (Mosquitto)
     Run Mosquitto Broker Locally
     mosquitto -c mosquitto/config/mosquitto.conf
   
6. Run Everything with Docker Compose
To spin up the entire system using Docker:
docker-compose up --build

**API Endpoints**
**Method**	            **Endpoint**	                      **Description**
GET	                /charging/status	              Get the latest charging session
POST	              /charging/start	                Start a new charging session
POST	              /charging/stop/{sessionId}	    Stop a charging session
________________________________________
**MQTT Topics**
**Topic**	                      **Description**
charging/start	            Start a charging session
charging/stop	              Stop a charging session
charging/start_response	    Response when charging starts
charging/stop_response	    Response when charging stops
charging/energySimulation	  Publish real-time energy simulation
________________________________________
Notes
•	The backend and frontend communicate via MQTT broker.
•	REST APIs are only for external access and debugging; they are not used by the frontend.



