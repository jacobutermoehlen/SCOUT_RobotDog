import numpy as np
import matplotlib.pyplot as plt

# Define the key points
P1 = np.array([-55, 0])
P2 = np.array([-80, 50])    #Control point
P3 = np.array([0, 75])
P4 = np.array([80, 50])     #Control point
P5 = np.array([55, 0])

# Generate the parameter t with 100 interpolation points
t = np.linspace(0, 1, 5)

# Compute the quadratic Bézier curve (left)
xl = (1 - t)**2 * P1[0] + 2 * (1 - t) * t * P2[0] + t**2 * P3[0]
yl = (1 - t)**2 * P1[1] + 2 * (1 - t) * t * P2[1] + t**2 * P3[1]

# Compute the quadratic Bézier curve (right)
xr = (1 - t)**2 * P5[0] + 2 * (1 - t) * t * P4[0] + t**2 * P3[0]
yr = (1 - t)**2 * P5[1] + 2 * (1 - t) * t * P4[1] + t**2 * P3[1]

# Plot the left Bézier curve
plt.plot(xl, yl, "b*", linewidth=1, label="Kurvenpunkte")
plt.plot(P1[0], P1[1], 'ro', markersize=10, markerfacecolor='r', label="Startpunkt")
plt.plot(P3[0], P3[1], 'go', markersize=10, markerfacecolor='g', label="Punkt 3")

# Plot the right Bézier curve
plt.plot(xr, yr, "b*", linewidth=1)
plt.plot(P5[0], P5[1], 'co', markersize=10, markerfacecolor='c', label="Endpunkt des Bodenkontaktes")

# Plot horizontal line
f = np.linspace(P1[0], P5[0], 9)
plt.plot(f, np.zeros_like(f), "b*", linewidth=1)

# Configure the plot
plt.title('Quadratische Bézier Kurve - Gehbewegung')
plt.xlabel('X')
plt.ylabel('Y')
plt.axis('equal')
plt.grid(True)
plt.legend(loc="center")

# Show the plot
plt.show()
