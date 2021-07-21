from jplephem.spk import SPK
import UnityEngine

kernel = SPK.open('de421.bsp')
output = []
t = 2460310.5
for x in range(1, 32):
    for i in range (24):
        position, velocity = kernel[3,399].compute_and_differentiate(t)
        position2, velocity2 = kernel[3,301].compute_and_differentiate(t)
        p = position - position2
        v = velocity - velocity2
        output.append([
            str(-p[0]), str(-p[1]), str(-p[2]),
            str(-v[0]), str(-v[1]), str(-v[2])
        ])

        t+=0.0416

key = "pointRetriever"
UnityEngine.GameObject.FindGameObjectWithTag("external/pythonHelper").GetComponent("pythonOutput").add(key, output)
#print(positions)