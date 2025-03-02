import React, { useEffect, useState } from "react";
import { parseJsonSourceFileConfigFileContent } from "typescript";
import client from "../Services/mqttService";
import ".././EvCharging.css";

const EVCharging: React.FC = () => {
  const [sessionId, setSessionId] = useState(0);
  const [status, setStatus] = useState("Stopped");
  const [startTime, setStartTime] = useState("NA");
  const [endTime, setEndTime] = useState("NA");
  const [energyConsumption, setEnergyConsumption] = useState(0);

  useEffect(() => {
    const mqtthandler = (topic: string, msg: Buffer) => {
      const message = JSON.parse(msg.toString());
      if (topic === "charging/start_response") {
        setSessionId(message.sessionId || 1);
        setStatus(message.status || "Charging");
        setStartTime(message.startTime || "NA");
      } else if (topic === "charging/stop_response") {
        setStatus(message.status || "Charging");
        setEndTime(message.endTime || "NA");
        setEnergyConsumption(message.energyConsumed || 0);
      } else if (topic === "charging/energySimulation") {
        setEnergyConsumption(message.energyConsumed || 0);
      }
    };

    client.on("message", mqtthandler);

    return () => {
      client.off("message", mqtthandler); // Cleanup listener on unmount
    };
  }, []);

  const handlerToStartCharging = () => {
    client.publish("charging/start", JSON.stringify({ command: "start" }));
  };

  const handlerToStopCharging = () => {
    client.publish("charging/stop", sessionId.toString());
  };

  return (
    <div className="container">
      <h2 className="heading">
        <b>Charging controller</b>
      </h2>
      <div className="box">
        <div className="buttons">
          <button
            type="button"
            className="btn btn-success"
            onClick={handlerToStartCharging}
            disabled={status === "Charging"}
          >
            Start Charging
          </button>
          <button
            type="button"
            className="btn btn-danger"
            onClick={handlerToStopCharging}
            disabled={status === "Stopped"}
          >
            Stop Charging
          </button>
        </div>

        <div className="status-display">
          <p>
            <strong> Status:</strong>
            <span> {status}</span>
          </p>
          <p>
            <strong>Session start time:</strong> <span>{startTime}</span>
          </p>
          <p>
            <strong>Session end time:</strong> <span>{endTime}</span>
          </p>
          <p>
            <strong> Energy consumed:</strong>
            <span>{energyConsumption} kwh</span>
          </p>
        </div>
      </div>
    </div>
  );
};
export default EVCharging;
