import matplotlib.pyplot as plt
import numpy as np
from time import sleep
from array import *

# variables
UPDATE_INTERVAL_MS = 10

# constants for invers kinematics
lengthA = 35.7
lengthE = 151.5
lengthF = 136.5

#jointAngles = [[90.0, 10.1, 101.7],
#               [90.0, 11.2, 92.6],
#               [90.0, 18.2, 80.7],
#               [90.0, 32.5, 67.4],
#               [90.0, 55.7, 57.9],
#               [90.0, 71.2, 67.4],
#               [90.0, 74.1, 80.7],
#               [90.0, 70.5, 92.6],
#               [90.0, 63.3, 101.7],
#               [90.0, 55.5, 91.3],
#               [90.0, 43.0, 87.8],
#               [90.0, 27.4, 91.3],
#               [90.0, 10.1, 101.7]]

jointAngles1 = [
    [90.0, 10.1, 101.7],
    [90.0, 27.4, 91.3],
    [90.0, 43.0, 87.8],
    [90.0, 55.5, 91.3],
    [90.0, 63.3, 101.7],
    [90.0, 70.5, 92.6],
    [90.0, 74.1, 80.7],
    [90.0, 71.2, 67.4],
    [90.0, 55.7, 57.9],
    [90.0, 32.5, 67.4],
    [90.0, 18.2, 80.7],
    [90.0, 11.2, 92.6],
    [90.0, 10.1, 101.7]
]

jointAngles = [
    [90, 27.4, 91.3],
    [90, 35.5, 88.7],
    [90, 43.0, 87.8],
    [90, 49.7, 88.7],
    [90, 55.5, 91.3],
    [90, 62.7, 82.3],
    [90, 66.2, 73.3],
    [90, 64.7, 64.6],
    [90, 55.7, 57.9],
    [90, 41.3, 64.6],
    [90, 32.5, 73.3],
    [90, 28.3, 82.2],
    [90, 27.4, 91.3],
]

jointAngles_interpArray = np.empty((0,3))

currentPoints = jointAngles[0]

fig = plt.figure()
ax = fig.add_subplot(projection="3d")

# Define custom box dimensions and center
x_size = 396.5  # Length along x-axis
y_size = 240.0  # Length along y-axis
z_size = 300  # Length along z-axis

x_center = 0.0  # Center of the x-axis
y_center = 150.0  # Center of the y-axis
z_center = 0.0  # Center of the z-axis

def set_axes_custom(ax, x_size, y_size, z_size, x_center=0, y_center=0, z_center=0):
    """Set a custom rectangular box for 3D plot with specific sizes and centers."""
    ax.set_xlim([x_center - x_size / 2, x_center + x_size / 2])
    ax.set_ylim([y_center - y_size / 2, y_center + y_size / 2])
    ax.set_zlim([z_center - z_size / 2, z_center + z_size / 2])
    # Aspect ratio based on sizes
    ax.set_box_aspect([x_size, y_size, z_size])

def draw_Leg(base_points, length1, length2, angle1, angle2, colourSet, clear):

    angle1_rad = np.radians(angle1)
    angle2_rad = np.radians(angle2)

    X1_1 = base_points[0]
    Y1_1 = base_points[1]
    Z1_1 = base_points[2]


    X1_2 = X1_1 + np.cos(angle1_rad) * length1
    Y1_2 = Y1_1 + np.sin(angle1_rad) * length1

    X2_1 = X1_2
    Y2_1 = Y1_2

    X2_2 = X2_1 + (np.cos(angle1_rad + angle2_rad) * length2)
    Y2_2 = Y2_1 + (np.sin(angle1_rad + angle2_rad) * length2)

    if clear == 1:
        ax.clear()

    if colourSet == 1:
        ax.plot([X1_1, X1_2], [Z1_1, Z1_1], [Y1_1, Y1_2], "b-o")
        ax.plot([X2_1, X2_2],[Z1_1, Z1_1] , [Y2_1, Y2_2], "r-o")
    elif colourSet == 2:
        ax.plot([X1_1, X1_2],[Z1_1, Z1_1], [Y1_1, Y1_2], "g-o")
        ax.plot([X2_1, X2_2],[Z1_1, Z1_1], [Y2_1, Y2_2], "m-o")
    ax.set_xlim(-200, 496.5)
    ax.set_ylim(-50, 190)
    ax.set_zlim(-300, 20)
    ax.set_aspect('equal')
    plt.gca().set_aspect('equal', adjustable='box')
    plt.grid()

def sendFrame(targetPoints, currentPoints, totalMoveTime, phase_shift):
    steps = totalMoveTime // UPDATE_INTERVAL_MS
    pointIntervals = [0, 0, 0]

    for i in range(3):
        pointIntervals[i] = (targetPoints[i] - currentPoints[i]) * (1.0 / steps)

    jointArray = [0, 0, 0]

    for step in range(1, steps + 1):
        for s in range(0,3):
            jointArray[s] = currentPoints[s] + (step * pointIntervals[s])
            

        print(f"{jointArray[0]:.1f} {jointArray[1]:.1f} {jointArray[2]:.1f}")
        draw_Leg([0,0], 151.5, 136.5,  -90 - jointArray[1], 180- jointArray[2], 1, 1)
        draw_Leg([250,0], 151.5, 136.5,  -90 - jointArray[1], 180- jointArray[2], 2, 0)
        #ax.clear()
        plt.pause(UPDATE_INTERVAL_MS / 1000)
    
    currentPoints[0] = jointArray[0]
    currentPoints[1] = jointArray[1]
    currentPoints[2] = jointArray[2]
    print()

def moveLeg(points, totalMoveTime):
    currentPoints = jointAngles[0]
    
    for i in range(1,len(points)):
        sendFrame(points[i], currentPoints, totalMoveTime, 5)

def makeFramesArray(targetPoints, currentPoints, totalMoveTime):
    steps = totalMoveTime // UPDATE_INTERVAL_MS
    pointIntervals = [0, 0, 0]

    for i in range(3):
        pointIntervals[i] = (targetPoints[i] - currentPoints[i]) * (1.0 / steps)

    jointArray = [0, 0, 0]
    interpArray = []
    for step in range(1, steps + 1):
        jointArray = [
            round(currentPoints[0] + (step * pointIntervals[0]), 1),
            round(currentPoints[1] + (step * pointIntervals[1]), 1),
            round(currentPoints[2] + (step * pointIntervals[2]), 1)
        ]
        interpArray.append(jointArray)

    currentPoints[0] = jointArray[0]
    currentPoints[1] = jointArray[1]
    currentPoints[2] = jointArray[2]
    return np.array(interpArray)

def sendFrame_shift(angles, shift, shift_index, time):
    delay = time / angles.shape[0] / 1000
    print(delay)
    for i in range(len(angles)):
        if shift_index == 0:
            draw_Leg([0,0,0], 151.5, 136.5,  -90 - angles[i- shift][1], 180- angles[i- shift][2], 1, 1)
        else:
            draw_Leg([0,0,0], 151.5, 136.5, -90 - angles[i][1], 180- angles[i][2], 1, 1)

        if shift_index == 1:    
            draw_Leg([296.5,0,0], 151.5, 136.5,  -90 - angles[i - shift][1], 180- angles[i - shift][2], 2, 0)
        else:
            draw_Leg([296.5,0,0], 151.5, 136.5,  -90 - angles[i][1], 180- angles[i][2], 2, 0)

        
        if shift_index == 1:
            draw_Leg([0,0,140], 151.5, 136.5,  -90 - angles[i- shift][1], 180- angles[i- shift][2], 2, 0)
        else:
            draw_Leg([0,0,140], 151.5, 136.5,  -90 - angles[i][1], 180- angles[i][2], 2, 0)

        if shift_index == 3:    
            draw_Leg([296.5,0,140], 151.5, 136.5,  -90 - angles[i - shift][1], 180- angles[i - shift][2], 1, 0)
        else:
            draw_Leg([296.5,0,140], 151.5, 136.5,  -90 - angles[i][1], 180- angles[i][2], 1, 0)

        #draw_Leg([500,0], 151.5, 136.5,  -90 - angles[i][1], 180- angles[i][2], 2, 0)
        #draw_Leg([750,0], 151.5, 136.5,  -90 - angles[i][1], 180- angles[i - shift][2], 2, 0)
        plt.pause(delay)


def calcInvKin(X, Y, Z):
    lengthD = np.sqrt((np.sqrt(Z ** 2 + Y ** 2) ** 2) - (lengthA ** 2))

    #Calculate omega (servo0)
    omegaRadi = np.atan(Z / Y) + np.atan(lengthD / lengthA)
    omega = omegaRadi * (180.0 / np.pi)

    #Calculate phi (servo2)
    lengthG = np.sqrt(lengthD ** 2 + X ** 2)
    phiRadi = np.acos(lengthG ** 2 - lengthE ** 2 - lengthF ** 2 / (-2 * lengthE * lengthF))
    phi = phiRadi * (180.0 / np.pi)

    #Calculat theta (servo1)
    thetaRadi = np.atan(X / lengthD) + np.asin((np.sin(phiRadi) / lengthG) * lengthF)
    theta = thetaRadi * (180.0 / np.pi)

    return np.array([omega, theta, phi])


#set_axes_custom(ax, x_size, y_size, z_size, x_center, y_center, z_center)



#moveLeg(jointAngles, 100)
currentPoints = jointAngles[0]
    
for i in range(1, len(jointAngles)):
    # Get the interpolated matrix for the current segment
    interpMatrix = makeFramesArray(jointAngles[i], currentPoints, 50)

    # Stack it to the final matrix
    jointAngles_interpArray = np.vstack((jointAngles_interpArray, interpMatrix))

#print(jointAngles_interpArray.shape)
for i in range(10): 
    sendFrame_shift(jointAngles_interpArray, len(jointAngles_interpArray) // 2, 1, 100)




#draw_Leg([0,0], 151.5, 136.5,  -90 - 25, 180- 30)


#draw_Leg([0,0], 151.5, 136.5, -90- 45, 180- 45)

plt.show()