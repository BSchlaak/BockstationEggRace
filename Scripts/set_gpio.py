#!/usr/bin/python2.7

import sys
from time import sleep
import RPi.GPIO as GPIO

GPIO.setmode(GPIO.BCM)

print("argv: {}".format(len(sys.argv)))
if len(sys.argv) > 1:
	print("Setting pin {} as output".format(sys.argv[1]))
	GPIO.setup(int(sys.argv[1]), GPIO.OUT)

	if len(sys.argv) > 2:
		print("Setting value of pin {} to {}".format(sys.argv[1], sys.argv[2]))
		GPIO.output(int(sys.argv[1]), int(sys.argv[2]))
	else:
		print("Toggling value of pin {} to 1".format(sys.argv[1]))
		GPIO.output(int(sys.argv[1]), 1)
		sleep(0.1)
		print("Toggling value of pin {} back to 0".format(sys.argv[1]))
		GPIO.output(int(sys.argv[1]), 0)
