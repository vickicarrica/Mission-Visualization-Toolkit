from skyfield.api import load, wgs84
import UnityEngine

ts = load.timescale()
t = ts.utc(2024, 1, 1)

active_url = 'http://celestrak.com/NORAD/elements/active.txt'
satellites = load.tle_file(active_url)
by_number = {sat.model.satnum: sat for sat in satellites}
satellite = by_number[31304]  #Change Sat here
positions = []
for x in range(1,32):
    for i in range(1,24):
        for y in range(1,60):
            t = ts.utc(2024, 1, x, i, y)
            geocentric = satellite.at(t)
            position = geocentric.position.km
            positions.append([str(position[0]), str(position[1]), str(position[2])])

key = "tleLoader"
UnityEngine.GameObject.FindGameObjectWithTag("external/pythonHelper").GetComponent("pythonOutput").add(key, positions)
