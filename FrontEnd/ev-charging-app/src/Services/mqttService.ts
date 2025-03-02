//import { connect } from "http2";
import mqtt from "mqtt";

const connUrl="ws://localhost:9001";
const client=mqtt.connect(connUrl);

client.on('connect',()=>{
    console.log("Connected to MQTT broker");
    client.subscribe('charging/start_response');
    client.subscribe('charging/stop_response');
    client.subscribe('charging/energySimulation');
});

client.on('error',(err)=>{
    console.log(err);
});

export default client;