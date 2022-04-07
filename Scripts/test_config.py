#!/usr/bin/python2.7

import ConfigParser

parser = ConfigParser.RawConfigParser()
file_path = "/usr/local/bin/EggRace/GPIO.ini"
parser.read(file_path)

start_io = parser.get('start', 'io')
start_base_state = parser.get('start', 'base_state')
print("Start: {} (Grundzustand: {})".format(start_io, start_base_state))

splitTime1_io = parser.get('splitTime1', 'io')
splitTime1_base_state = parser.get('splitTime1', 'base_state')
print("1. Zwischenzeit: {} (Grundzustand: {})".format(splitTime1_io, splitTime1_base_state))

splitTime2_io = parser.get('splitTime2', 'io')
splitTime2_base_state = parser.get('splitTime2', 'base_state')
print("2. Zwischenzeit: {} (Grundzustand: {})".format(splitTime2_io, splitTime2_base_state))

total_io = parser.get('totalTime', 'io')
total_base_state = parser.get('totalTime', 'base_state')
print("Gesamtzeit: {} (Grundzustand: {})".format(total_io, total_base_state))

