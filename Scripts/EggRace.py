#!/usr/bin/python2.7

import ConfigParser
from datetime import datetime
import json
import RPi.GPIO as GPIO
import paho.mqtt.client as mqtt

BROKER_ADDRESS = "localhost"
CHANNEL_NAME = "/eggRace/measurements"
client = mqtt.Client()
client.connect(BROKER_ADDRESS)

parser = ConfigParser.RawConfigParser()
file_path = "/usr/local/bin/EggRace/GPIO.ini"
parser.read(file_path)

LS_1 = int(parser.get('start', 'io'))
LS_2 = int(parser.get('splitTime1', 'io'))
LS_3 = int(parser.get('splitTime2', 'io'))
LS_4 = int(parser.get('totalTime', 'io'))


def get_time():
	print("Getting current timestamp")
	ts = int(datetime.now().strftime("%H%M%S%f"))
	print("Got current timestamp: {}".format(ts))
	return ts

def publish_mqtt(data):
	print("Publishing to {}: {}".format(CHANNEL_NAME, data))
	client.publish(CHANNEL_NAME, json.dumps(data))

def start_callback(channel):
	print("Channel: {}".format(channel))
	ts = get_time()
	print("Setting timestamp as first time: {}".format(ts))
	data = [0, ts]
	publish_mqtt(data)

def zz1_callback(channel):
	ts = get_time()
	print("Setting timestamp as second time: {}".format(ts))
	data = [1, ts]
	publish_mqtt(data)

def zz2_callback(channel):
	ts = get_time()
	print("Setting timestamp as third time: {}".format(ts))
	data = [2, ts]
	publish_mqtt(data)

def ziel_callback(channel):
	ts = get_time()
	print("Setting timestamp as last time: {}".format(ts))
	data = [3, ts]
	publish_mqtt(data)


GPIO.setmode(GPIO.BCM)

if parser.get('start', 'base_state') == 0:
	GPIO.setup(LS_1, GPIO.IN, pull_up_down=GPIO.PUD_DOWN)
	GPIO.add_event_detect(LS_1, GPIO.RISING, callback=start_callback, bouncetime=300)
else:
	GPIO.setup(LS_1, GPIO.IN, pull_up_down=GPIO.PUD_UP)
	GPIO.add_event_detect(LS_1, GPIO.FALLING, callback=start_callback, bouncetime=300)

if parser.get('splitTime1', 'base_state') == 0:
	GPIO.setup(LS_2, GPIO.IN, pull_up_down=GPIO.PUD_DOWN)
	GPIO.add_event_detect(LS_2, GPIO.RISING, callback=zz1_callback, bouncetime=300)
else:
	GPIO.setup(LS_2, GPIO.IN, pull_up_down=GPIO.PUD_UP)
	GPIO.add_event_detect(LS_2, GPIO.FALLING, callback=zz1_callback, bouncetime=300)

if parser.get('splitTime2', 'base_state') == 0:
	GPIO.setup(LS_3, GPIO.IN, pull_up_down=GPIO.PUD_DOWN)
	GPIO.add_event_detect(LS_3, GPIO.RISING, callback=zz2_callback, bouncetime=300)
else:
	GPIO.setup(LS_3, GPIO.IN, pull_up_down=GPIO.PUD_UP)
	GPIO.add_event_detect(LS_3, GPIO.FALLING, callback=zz2_callback, bouncetime=300)

if parser.get('totalTime', 'base_state') == 0:
	GPIO.setup(LS_4, GPIO.IN, pull_up_down=GPIO.PUD_DOWN)
	GPIO.add_event_detect(LS_4, GPIO.RISING, callback=ziel_callback, bouncetime=300)
else:
	GPIO.setup(LS_4, GPIO.IN, pull_up_down=GPIO.PUD_UP)
	GPIO.add_event_detect(LS_4, GPIO.FALLING, callback=ziel_callback, bouncetime=300)

while True:
	pass

