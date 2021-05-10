import json
import websocket
ws=websocket.create_connection("ws://localhost:8000/main")
message={
    "function":"search_friend",
    "data":
    {
        "account":"1143550127",
    }
}
ws.send(json.dumps(message))
result=ws.recv()
print(result)