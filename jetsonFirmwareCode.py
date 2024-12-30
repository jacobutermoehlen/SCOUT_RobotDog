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


def draw_Leg(base_points, length1, length2, angle1, angle2, colourSet, clear):

    angle1_rad = np.radians(angle1)
    angle2_rad = np.radians(angle2)

    X1_1 = base_points[0]
    Y1_1 = base_points[1]

    X1_2 = X1_1 + np.cos(angle1_rad) * length1
    Y1_2 = Y1_1 + np.sin(angle1_rad) * length1

    X2_1 = X1_2
    Y2_1 = Y1_2

    X2_2 = X2_1 + (np.cos(angle1_rad + angle2_rad) * length2)
    Y2_2 = Y2_1 + (np.sin(angle1_rad + angle2_rad) * length2)

    if clear == 1:
        ax.clear()

    if colourSet == 1:
        ax.plot([X1_1, X1_2], [Y1_1, Y1_2], "b-o")
        ax.plot([X2_1, X2_2], [Y2_1, Y2_2], "r-o")
    elif colourSet == 2:
        ax.plot([X1_1, X1_2], [Y1_1, Y1_2], "g-o")
        ax.plot([X2_1, X2_2], [Y2_1, Y2_2], "m-o")
    ax.set_xlim(-200, 950)
    ax.set_ylim(-300, 20)
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
    delay = time / angles.shape[0]
    print(angles.shape[0])
    #print(delay)
    for i in range(len(angles)):
        if shift_index == 0:
            draw_Leg([0,0], 151.5, 136.5,  -90 - angles[i- shift][1], 180- angles[i- shift][2], 1, 1)
        else:
            draw_Leg([0,0], 151.5, 136.5,  -90 - angles[i][1], 180- angles[i][2], 1, 1)
            print(f"{angles[i][1]}  ---  {angles[i][2]}")

        if shift_index == 5:    
            draw_Leg([250,0], 151.5, 136.5,  -90 - angles[i - shift][1], 180- angles[i - shift][2], 1, 0)
        else:
            draw_Leg([250,0], 151.5, 136.5,  -90 - angles[i][1], 180- angles[i][2], 1, 0)

        
        if shift_index == 5:
            draw_Leg([500,0], 151.5, 136.5,  -90 - angles[i- shift][1], 180- angles[i- shift][2], 2, 0)
        else:
            draw_Leg([500,0], 151.5, 136.5,  -90 - angles[i][1], 180- angles[i][2], 2, 0)

        if shift_index == 3:    
            draw_Leg([750,0], 151.5, 136.5,  -90 - angles[i - shift][1], 180- angles[i - shift][2], 2, 0)
        else:
            draw_Leg([750,0], 151.5, 136.5,  -90 - angles[i][1], 180- angles[i][2], 2, 0)

        #draw_Leg([500,0], 151.5, 136.5,  -90 - angles[i][1], 180- angles[i][2], 2, 0)
        #draw_Leg([750,0], 151.5, 136.5,  -90 - angles[i][1], 180- angles[i - shift][2], 2, 0)
        plt.pause(delay)


def calcInvKin(X, Y, Z):
    lengthD = np.sqrt((np.sqrt(Z ** 2 + Y ** 2) ** 2) - (lengthA ** 2))

    # Calculate omega (servo0)
    omegaRadi = np.arctan2(Z, Y) + np.arctan2(lengthD, lengthA)
    omega = omegaRadi * (180.0 / np.pi)

    # Calculate phi (servo2)
    lengthG = np.sqrt(lengthD ** 2 + X ** 2)
    phiRadi = np.arccos((lengthG ** 2 - lengthE ** 2 - lengthF ** 2) / (-2 * lengthE * lengthF))
    phi = phiRadi * (180.0 / np.pi)

    # Calculate theta (servo1)
    thetaRadi = np.arctan2(X, lengthD) + np.arcsin((np.sin(phiRadi) / lengthG) * lengthF)
    theta = thetaRadi * (180.0 / np.pi)

    return np.column_stack((omega, theta, phi))




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

jointAngles2 = [
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

points_inOrder = np.empty((0,3))
bezierCurvePoint_Count = 5
t = np.linspace(0,1, bezierCurvePoint_Count)

# Define control points
P0 = np.array([-50, 200])
P1 = np.array([-75, 160])
P2 = np.array([0, 140])
P3 = np.array([75, 160])
P5 = np.array([50, 200])

# Calculate Bezier Curves points
x1 = ((1 - t) ** 2 * P0[0]) + 2 * (1 - t) * t * P1[0] + t ** 2 * P2[0]
y1 = ((1 - t) ** 2 * P0[1]) + 2 * (1 - t) * t * P1[1] + t ** 2 * P2[1]

x2 = ((1 - t) ** 2 * P5[0]) + 2 * (1 - t) * t * P3[0] + t ** 2 * P2[0]
y2 = ((1 - t) ** 2 * P5[1]) + 2 * (1 - t) * t * P3[1] + t ** 2 * P2[1]

# Generate flat points
f = np.linspace(P0[0], P5[0], bezierCurvePoint_Count)

# Combine points into points_inOrder
points_inOrder = np.vstack((points_inOrder, np.column_stack((f, np.full_like(f, 200), np.full_like(f, lengthA)))))
points_inOrder = np.vstack((points_inOrder, np.column_stack((x2[1:], y2[1:], np.full_like(y2[1:], lengthA)))))
points_inOrder = np.vstack((points_inOrder, np.column_stack((np.flipud(x1[:-1]), np.flipud(y1[:-1]), np.full_like(y1[:-1], lengthA)))))

jointAngles = np.round(calcInvKin(points_inOrder[:, 0], points_inOrder[:, 1], points_inOrder[:, 2]), 1)



jointAngles_interpArray = np.empty((0,3))
currentPoints = jointAngles[0]

fig, ax = plt.subplots(figsize=(6,6))


#moveLeg(jointAngles, 100)
currentPoints = jointAngles[0]
    
for i in range(1, len(jointAngles)):
    # Get the interpolated matrix for the current segment
    interpMatrix = makeFramesArray(jointAngles[i], currentPoints, 100)

    # Stack it to the final matrix
    jointAngles_interpArray = np.vstack((jointAngles_interpArray, interpMatrix))

#print(jointAngles_interpArray.shape)
print(len(jointAngles_interpArray))
for i in range(5): 
    sendFrame_shift(jointAngles_interpArray, len(jointAngles_interpArray) // 2, 5, 3)




#draw_Leg([0,0], 151.5, 136.5,  -90 - 25, 180- 30)


#draw_Leg([0,0], 151.5, 136.5, -90- 45, 180- 45)

plt.show()