import json
from pprint import pprint

paramdata = json.load(open('param.json'))
roomsize = paramdata['roomsize']
pprint(roomsize)