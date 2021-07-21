import UnityEngine
from skyfield.api import load
from skyfield.elementslib import osculating_elements_of

ts = load.timescale()

planets = load('de421.bsp')
earth = planets['earth']
moon = planets['moon']
kElements = []

for x in range(1,32):
    for i in range(1,24):
        t = ts.utc(2024, 1, x, i)
        #moon barycenter --> earth
        position = (-(earth - moon)).at(t)
        elements = osculating_elements_of(position)

        e = elements.eccentricity
        a = elements.semi_major_axis.km
        i = elements.inclination.radians
        longOfAscNode = elements.longitude_of_ascending_node.radians
        argOfPeriapsis = elements.argument_of_periapsis.radians
        T = elements.period_in_days*86400
        m = elements.mean_anomaly.radians
        v = elements.true_anomaly.radians
        kElements.append([
            str(e), str(a), str(i), str(longOfAscNode), str(argOfPeriapsis), str(T), str(m), str(v)
        ])

key = "keplerRetriever"
UnityEngine.GameObject.FindGameObjectWithTag("external/pythonHelper").GetComponent("pythonOutput").add(key, kElements)