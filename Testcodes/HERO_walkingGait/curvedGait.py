import matplotlib.pyplot as plt

pointf_0 = (15, 0)
pointf_1 = (-15, -50)

pointb_0 = (0, -150)
pointb_1 = (0, -200)


x0_values = [pointf_0[0], pointf_1[0]]
y0_values = [pointf_0[1], pointf_1[1]]

x1_values = [pointf_0[0] + 80, pointf_1[0] + 80]
y1_values = [pointf_0[1], pointf_1[1]]

x2_values = [pointb_0[0], pointb_1[0]]
y2_values = [pointb_0[1], pointb_1[1]]

x3_values = [pointb_0[0] + 80, pointb_1[0] + 80]
y3_values = [pointb_0[1], pointb_1[1]]

# Create the plot
plt.figure()
plt.plot(x0_values, y0_values, 'bo-', label='vordere Beine')
plt.plot(x1_values, y1_values, 'bo-')
plt.plot(x2_values, y2_values, 'ro-', label='hintere Beine')
plt.plot(x3_values, y3_values, 'ro-')
plt.legend(loc='center')
plt.axis('equal')
plt.title('kurve')
plt.show()