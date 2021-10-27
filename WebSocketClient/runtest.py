import websocket
import time
import sys
import ssl

def print_line():
    print("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~")

def printtime():
    t = time.localtime()
    current_time = time.strftime("%H:%M:%S", t)
    print(current_time)

def on_message(ws, message):
    print_line()
    printtime()
    print(message)
    print_line()

def on_error(ws, error):
    print_line()
    printtime()
    print(error)

def on_close(ws, close_status_code, close_msg):
    print_line()
    printtime()
    print("### closed ###")

def on_ping(app,msg):
    print_line()
    printtime()
    print("Got ping")

def on_pong(app,msg):
    print_line()
    printtime()
    print("Got pong")

def on_open(ws):
    print_line()
    printtime()
    print("Succesfully connected!")
    print("sending message")
    ws.send("{}")
    print("done sending message!")
    print_line()
       
websocket.enableTrace(True)

#"wss://54.235.229.25:80/connect"
wsurl = sys.argv[1]
ws = websocket.WebSocketApp(wsurl,
on_open=on_open,
on_message=on_message,
on_error=on_error,
on_close=on_close,
on_ping=on_ping,
on_pong=on_pong)

ws.run_forever(sslopt={"cert_reqs":ssl.CERT_NONE})