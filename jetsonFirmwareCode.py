import matplotlib.pyplot as plt
import numpy as np
from time import sleep
from array import *
from smbus2 import SMBus
import socket
import threading
import serial

#TCP Socket Setup
controlServer_ip = '192.168.4.1'
controlServer_port = 12345
sensorServer_ip = '192.168.4.1'
sensorServer_port = 12346

#sensor serial setup
ser = serial.Serial(
    port="/dev/ttyTHS1",
    baudrate=115200,
    timeout=1
)

# variables
UPDATE_INTERVAL_MS = 10
ride_height = 240

# cycle preset 1
#P0_1 = np.array([-50, ride_height, 35.7])
#P1_1 = np.array([-75, ride_height - 40, 35.7])
#P2_1 = np.array([0, ride_height - 60, 35.7])
#P3_1 = np.array([75, ride_height - 40, 35.7])
#P4_1 = np.array([50, ride_height, 35.7])

#P0_1 = np.array([-30, ride_height, 70])
#P1_1 = np.array([-55, ride_height - 40, 70])
#P2_1 = np.array([0, ride_height - 60, 70])
#P3_1 = np.array([55, ride_height - 40, 70])
#P4_1 = np.array([30, ride_height, 70])

P0_1 = np.array([-45, ride_height, 60])
P1_1 = np.array([-70, ride_height - 50, 60])
P2_1 = np.array([0, ride_height - 75, 60])
P3_1 = np.array([70, ride_height - 50, 60])
P4_1 = np.array([45, ride_height, 60])


# Constants
PCA9685_ADDRESS = 0x40  # Default I2C address
MODE1 = 0x00
PRESCALE = 0xFE
LED0_ON_L = 0x06
FREQ = 330  # Frequency in Hz

# constants for invers kinematics
lengthA = 35.7
lengthE = 151.5
lengthF = 136.5

# constants for servo index
fl0 = 4
fl1 = 5
fl2 = 6

fr0 = 2
fr1 = 1
fr2 = 0

bl0 = 10
bl1 = 9
bl2 = 11

br0 = 13
br1 = 14
br2 = 15
    #calibration values
fl0_calibValue = -10
fl1_calibValue = 10.5
fl2_calibValue = +2.5

fr0_calibValue = -14
fr1_calibValue = -11
fr2_calibValue = -7

bl0_calibValue = -7.5
bl1_calibValue = -9.3
bl2_calibValue = 2

br0_calibValue = -8.5
br1_calibValue = -10
br2_calibValue = 3

walkingAngle = 0
walkingInterval = 0.5
walkingBool = True
walkingThread = None
walkingConstantlyThread = None

bus = SMBus(7)

def handle_message(message, conn):
    global walkingBool
    global walkingAngle
    global walkingInterval
    global walkingThread
    global ride_height
    global walkingConstantlyThread
    global P0_1
    global P1_1
    global P2_1
    global P3_1
    global P4_1

    if message == 'test123456789':
        print("Client said test123456789")
        conn.sendall(b"Hello, Client")
    elif message == 'standup':
        stand_up()
    elif "moveN" in message:
        print(message[5:])
        move_to_neutral(message[5:])
        conn.sendall("Moved".encode('utf-8'))
    elif "moveCForward" in message:
        print(message[12:])
        if "0" in message[12:]:
            walkingBool = False
        elif "1" in message[12:]:
            #walkingBool == True
            if walkingConstantlyThread is None or not walkingConstantlyThread.is_alive():
                walkingConstantlyThread = threading.Thread(target=move_c_forward, args=(walkingInterval, walkingAngle))
                walkingConstantlyThread.start()
            #walkingBool = False
    elif "moveForward" in message:
        print(message[11:])
        print(walkingAngle)
        if walkingThread is None or not walkingThread.is_alive():
            walkingThread = threading.Thread(target=move_forward, args=(int(message[11:]), walkingInterval, walkingAngle))
            walkingThread.start()
        #move_forward(int(message[11:]), 0.5, walkingAngle)
        print("Done")
    elif "changeAngle" in message:
        if message[0] == "c":
            walkingAngle = int(message[11:])
            conn.sendall("changedAngle".encode('utf-8'))
            print(walkingAngle)
    elif "changeRideHeight" in message:
        print(message)
        print("height")
        if message[0] == "c":
            print(message[16:])
            ride_height = int(message[16:])
            print(ride_height)
            conn.sendall("changedRideHeight".encode('utf-8'))
    elif "changeInter" in message:
        print(message[11:])
        walkingInterval = float(message[11:])
        print(walkingInterval)
        conn.sendall("changedInter".encode('utf-8'))
    elif "standUp" in message:
        stand_up()
    elif "makeInterpArray" in message:
        make_interp_array
    elif "turnLeft" in message:
        print()
    else:
        print(f"Received message: {message}")

def receive_thread(conn):
    while True:
        try:
            data = conn.recv(1024).decode('utf-8')
            if not data:
                break
            handle_message(data, conn)
        except Exception as e:
            print(f"Error: {e}")
            break
    print("Connection closed.")
    conn.close()

def receiveSensor_thread(senConn):
    print("cool")
    while True:
        try:
            if ser.inWaiting() > 0:
                message = ser.readline().decode('utf-8').strip()
                senConn.sendall(message.encode('utf-8'))
                print(message + get_cpu_temperature())
                #sleep(5/1000)
        except Exception as e:
            print(f"Sensor Error: {e}")
            break
    print("Sensor Connection closed.")
    senConn.close()

def set_pwm_freq(bus, freq_hz):
    prescale_val = int(25000000.0 / (4096 * freq_hz) - 1)
    bus.write_byte_data(PCA9685_ADDRESS, MODE1, 0x10)  # Enter sleep mode
    bus.write_byte_data(PCA9685_ADDRESS, PRESCALE, prescale_val)
    bus.write_byte_data(PCA9685_ADDRESS, MODE1, 0x80)  # Restart PCA9685

def set_pwm(bus, channel, on, off):
    bus.write_byte_data(PCA9685_ADDRESS, LED0_ON_L + 4 * channel, on & 0xFF)
    bus.write_byte_data(PCA9685_ADDRESS, LED0_ON_L + 4 * channel + 1, on >> 8)
    bus.write_byte_data(PCA9685_ADDRESS, LED0_ON_L + 4 * channel + 2, off & 0xFF)
    bus.write_byte_data(PCA9685_ADDRESS, LED0_ON_L + 4 * channel + 3, off >> 8)

def angle_to_pwm(angle, min_pwm, max_pwm, freq):
        # Ensure angle is within bounds
    angle = max(0, min(180, angle))
    
    # Map angle to pulse width in microseconds
    pulse_width_us = min_pwm + (angle / 180) * (max_pwm - min_pwm)
    
    # Convert microseconds to PCA9685 12-bit range
    pulse_length_counts = int(pulse_width_us * 4096 / (1000000 / freq))
    return pulse_length_counts

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

def moveLeg_WCycle(points, totalMoveTime):
    currentPoints = jointAngles[0]
    
    for i in range(1,len(points)):
        sendFrame(points[i], currentPoints, totalMoveTime, 5)

def moveLeg(angle0, angle1, angle2, index, bus):
    if index == 0:
        
        pwm_value0 = angle_to_pwm(angle0 + fl0_calibValue, 590, 2800, 330)
        set_pwm(bus, fl0, 0, pwm_value0)                    # move 1st servo of front left leg

        pwm_value1 = angle_to_pwm(angle1 + fl1_calibValue, 590, 2800, 330)
        set_pwm(bus, fl1, 0, pwm_value1)                    # move 2nd servo of front left leg
        pwm_value2 = angle_to_pwm(180 - angle2 + fl2_calibValue, 590, 2800, 330)
        set_pwm(bus, fl2, 0, pwm_value2)                    # move 3rd servo of front left leg
    elif index == 1:
        
        pwm_value0 = angle_to_pwm(180 - angle0 + fr0_calibValue, 590, 2800, 330)
        set_pwm(bus, fr0, 0, pwm_value0)                    # move 1st servo of front right leg

        pwm_value1 = angle_to_pwm(180 - angle1 + fr1_calibValue, 590, 2800, 330)
        set_pwm(bus, fr1, 0, pwm_value1)                    # move 2nd servo of front right leg

        pwm_value2 = angle_to_pwm(angle2 + fr2_calibValue, 590, 2800, 330)
        set_pwm(bus, fr2, 0, pwm_value2)                    # move 3rd servo of fron right leg
    elif index == 2:

        pwm_value0 = angle_to_pwm(180 - angle0 + bl0_calibValue, 590, 2800, 330)
        set_pwm(bus, bl0, 0, pwm_value0)                    # move 1st servo of back left leg
        
        pwm_value1 = angle_to_pwm(angle1 + bl1_calibValue, 590, 2800, 330)
        set_pwm(bus, bl1, 0, pwm_value1)                    # move 2nd servo of back left leg

        pwm_value2 = angle_to_pwm(180 - angle2 + bl2_calibValue, 590, 2800, 330)
        set_pwm(bus, bl2, 0, pwm_value2)                    # move 3rd servo of back left leg
    elif index == 3:
        
        pwm_value0 = angle_to_pwm(angle0 + br0_calibValue, 590, 2800, 330)
        set_pwm(bus, br0, 0, pwm_value0)                    # move 1st servo of back right leg
    
        pwm_value1 = angle_to_pwm(180 - angle1 + br1_calibValue, 590, 2800, 330)
        set_pwm(bus, br1, 0, pwm_value1)                    # move 2nd servo of back right leg
        
        pwm_value2 = angle_to_pwm(angle2 + br2_calibValue, 590, 2800, 330)
        set_pwm(bus, br2, 0, pwm_value2)    

def makeFramesArray(targetPoints, currentPoints, totalMoveTime):
    steps = totalMoveTime // UPDATE_INTERVAL_MS
    pointIntervals = [0, 0, 0]

    if len(targetPoints) != 3 or len(currentPoints) != 3:
        raise ValueError("targetPoints and currentPoints must each contain exactly 3 elements.")


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
    print(delay)
    for i in range(len(angles)):
        #if shift_index == 0:
        #    draw_Leg([0,0], 151.5, 136.5,  -90 - angles[i- shift][1], 180- angles[i- shift][2], 1, 1)
        #else:
        #    draw_Leg([0,0], 151.5, 136.5,  -90 - angles[i][1], 180- angles[i][2], 1, 1)
        #    print(f"{angles[i][1]}  ---  {angles[i][2]}")

        with SMBus(7) as bus:  # Use I2C bus 1 on Jetson
            set_pwm_freq(bus, 330)

            #br
            pwm_value0 = angle_to_pwm(angles[i][0] + br0_calibValue)
            set_pwm(bus, 13, 0, pwm_value0)

            pwm_value1 = angle_to_pwm(180 - angles[i][1] + br1_calibValue)
            set_pwm(bus, 14, 0, pwm_value1)

            pwm_value2 = angle_to_pwm(angles[i][2] + br2_calibValue)
            set_pwm(bus, 15, 0, pwm_value2)


            #fl
            pwm_value3 = angle_to_pwm(angles[i][0] + 5)
            set_pwm(bus, 4, 0, pwm_value3)

            pwm_value4 = angle_to_pwm(angles[i][1] + 0)
            set_pwm(bus, 5, 0, pwm_value4)

            pwm_value5 = angle_to_pwm(180 - angles[i][2] + 0)
            set_pwm(bus, 6, 0, pwm_value5)

            #fr
            pwm_value6 = angle_to_pwm(angles[i - shift][2] + 0)
            set_pwm(bus, 0, 0, pwm_value6)

            pwm_value7 = angle_to_pwm(180 - angles[i - shift][1] + 0)
            set_pwm(bus, 1, 0, pwm_value7)

            pwm_value8 = angle_to_pwm(180 - angles[i - shift][0] + 0)
            set_pwm(bus, 2, 0, pwm_value8)

            #bl
            pwm_value9 = angle_to_pwm(180 -angles[i - shift][0] + 10)
            set_pwm(bus, 11, 0, pwm_value9)

            pwm_value10 = angle_to_pwm(angles[i - shift][1] + 0)
            set_pwm(bus, 10, 0, pwm_value10)

            pwm_value11 = angle_to_pwm(180 - angles[i - shift][2] + 0)
            set_pwm(bus, 9, 0, pwm_value11)

#
        #if shift_index == 5:    
        #    draw_Leg([250,0], 151.5, 136.5,  -90 - angles[i - shift][1], 180- angles[i - shift][2], 1, 0)
        #else:
        #    draw_Leg([250,0], 151.5, 136.5,  -90 - angles[i][1], 180- angles[i][2], 1, 0)
#
        #
        #if shift_index == 5:
        #    draw_Leg([500,0], 151.5, 136.5,  -90 - angles[i- shift][1], 180- angles[i- shift][2], 2, 0)
        #else:
        #    draw_Leg([500,0], 151.5, 136.5,  -90 - angles[i][1], 180- angles[i][2], 2, 0)
#
        #if shift_index == 3:    
        #    draw_Leg([750,0], 151.5, 136.5,  -90 - angles[i - shift][1], 180- angles[i - shift][2], 2, 0)
        #else:
        #    draw_Leg([750,0], 151.5, 136.5,  -90 - angles[i][1], 180- angles[i][2], 2, 0)

        #draw_Leg([500,0], 151.5, 136.5,  -90 - angles[i][1], 180- angles[i][2], 2, 0)
        #draw_Leg([750,0], 151.5, 136.5,  -90 - angles[i][1], 180- angles[i - shift][2], 2, 0)
        #plt.pause(delay)
        sleep(delay)

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

def calcWalkCycle5(P0, P1, P2, P3, P4, bezierCurve_count, flatPoints_count, alpha):
    points_inOrder = np.empty((0,3))
    t = np.linspace(0, 1, bezierCurve_count)

    # Calculate Bezier Curve Points
    x1 = ((1 - t) ** 2 * P0[0]) + 2 * (1 - t) * t * P1[0] + t ** 2 * P2[0]
    y1 = ((1 - t) ** 2 * P0[1]) + 2 * (1 - t) * t * P1[1] + t ** 2 * P2[1]
    z1 = np.full_like(t, P2[2])

    x2 = ((1 - t) ** 2 * P4[0]) + 2 * (1 - t) * t * P3[0] + t ** 2 * P2[0]
    y2 = ((1 - t) ** 2 * P4[1]) + 2 * (1 - t) * t * P3[1] + t ** 2 * P2[1]
    z2 = z1

    # Generate flat Points
    fx = np.linspace(P0[0], P4[0], flatPoints_count)
    fy = np.full_like(fx, P0[1])
    fz = np.full_like(fx, P0[2])


    # rotate around x = 0 by angle alpha
    alpha_radi = np.radians(alpha)

    x1_rotated = np.cos(alpha_radi) * x1
    z1_rotated = np.sin(alpha_radi) * x1 + z1

    x2_rotated = np.cos(alpha_radi) * x2
    z2_rotated = np.sin(alpha_radi) * x2 + z2

    fx_rotated = np.cos(alpha_radi) * fx
    fz_rotated = np.sin(alpha_radi) * fx + fz

    points_inOrder = np.vstack((points_inOrder, np.column_stack((fx_rotated, fy, fz_rotated))))
    points_inOrder = np.vstack((points_inOrder, np.column_stack((x2_rotated[1:], y2[1:], z2_rotated[1:]))))
    points_inOrder = np.vstack((points_inOrder, np.column_stack((np.flipud(x1_rotated[:-1]), np.flipud(y1[:-1]), np.flipud(z1_rotated[:-1])))))
    
    jointAngles = np.round(calcInvKin(points_inOrder[:, 0], points_inOrder[:, 1], points_inOrder[:, 2]), 1)


    jointAngles_interpArray = np.empty((0,3))
    currentPoints = jointAngles[0]

    for i in range(1, len(jointAngles)):
        # Get the interpolated matrix for the current segment
        interpMatrix = makeFramesArray(jointAngles[i], currentPoints, 30)

        # Stack it to the final matrix
        jointAngles_interpArray = np.vstack((jointAngles_interpArray, interpMatrix))
    #print(points_inOrder)
    return jointAngles_interpArray

# functions for movement
def stand_up():
    global bus
    startCords = calcInvKin(0, 100, 35.7)
    midCords = calcInvKin(0, 190, 35.7)
    endCords = calcInvKin(0, 200,35.7)
    print(startCords.shape)
    for i in range(4):
        moveLeg(startCords[0][0], startCords[0][1], startCords[0][2], i, bus)
    sleep(2)
    standUp_Array = makeFramesArray(midCords[0], startCords[0], 400)
    standUp_Array = np.vstack((standUp_Array, makeFramesArray(endCords[0], midCords[0], 400)))

    print(standUp_Array.shape)
    for j in range(len(standUp_Array)):
        for l in range(4):
            moveLeg(standUp_Array[j][0], standUp_Array[j][1], standUp_Array[j][2], l, bus)
        sleep(0.02)   
        #draw_Leg([0,0], 151.5, 136.5,  -90 - standUp_Array[j][1], 180- standUp_Array[j][2], 1, 1)
        #plt.pause(0.002)


    #plt.show()

def move_to_neutral(height):
    global bus
    #bus = SMBus(7)
    #set_pwm_freq(bus,330)
    neutralCords = calcInvKin(0, float(height), 35.7)[0]       # calc. Inverse Kinematics for neutral position for specific height
    print("Calculated Kin")
    #for l in range(4):                                                  
    #    moveLeg(neutralCords[0], neutralCords[1], neutralCords[2], l)   #move each leg to neutral position to height
    moveLeg(neutralCords[0], neutralCords[1], neutralCords[2], 0, bus) 
    moveLeg(neutralCords[0], neutralCords[1], neutralCords[2], 1, bus)
    moveLeg(neutralCords[0], neutralCords[1], neutralCords[2], 2, bus) 
    moveLeg(neutralCords[0], neutralCords[1], neutralCords[2], 3, bus)  

def make_interp_array():
    points_inOrder = np.empty((0,3))
    bezierCurvePoint_Count = 5
    t = np.linspace(0,1, bezierCurvePoint_Count)

    # Define control points
    P0 = np.array([-50, 200])
    P1 = np.array([-75, 160])
    P2 = np.array([0, 140])
    P3 = np.array([75, 160])
    P4 = np.array([50, 200])

    # Calculate Bezier Curves points
    x1 = ((1 - t) ** 2 * P0[0]) + 2 * (1 - t) * t * P1[0] + t ** 2 * P2[0]
    y1 = ((1 - t) ** 2 * P0[1]) + 2 * (1 - t) * t * P1[1] + t ** 2 * P2[1]

    x2 = ((1 - t) ** 2 * P4[0]) + 2 * (1 - t) * t * P3[0] + t ** 2 * P2[0]
    y2 = ((1 - t) ** 2 * P4[1]) + 2 * (1 - t) * t * P3[1] + t ** 2 * P2[1]

    # Generate flat points
    f = np.linspace(P0[0], P4[0], bezierCurvePoint_Count)

    # Combine points into points_inOrder
    points_inOrder = np.vstack((points_inOrder, np.column_stack((f, np.full_like(f, 200), np.full_like(f, lengthA)))))
    points_inOrder = np.vstack((points_inOrder, np.column_stack((x2[1:], y2[1:], np.full_like(y2[1:], lengthA)))))
    points_inOrder = np.vstack((points_inOrder, np.column_stack((np.flipud(x1[:-1]), np.flipud(y1[:-1]), np.full_like(y1[:-1], lengthA)))))

    jointAngles = np.round(calcInvKin(points_inOrder[:, 0], points_inOrder[:, 1], points_inOrder[:, 2]), 1)


    jointAngles_interpArray = np.empty((0,3))
    currentPoints = jointAngles[0]

    for i in range(1, len(jointAngles)):
        # Get the interpolated matrix for the current segment
        interpMatrix = makeFramesArray(jointAngles[i], currentPoints, 100)

        # Stack it to the final matrix
        jointAngles_interpArray = np.vstack((jointAngles_interpArray, interpMatrix))

def move_forward(reps, time, angle):
    global walkingInterval
    global walkingAngle
    global bus
    global ride_height
    global P0_1
    global P1_1
    global P2_1
    global P3_1
    global P4_1
    jointAngles_interpArray0 = calcWalkCycle5(P0_1, P1_1, P2_1, P3_1, P4_1, 5, 9, angle)    #for fl leg [0]
    jointAngles_interpArray1 = calcWalkCycle5(P0_1, P1_1, P2_1, P3_1, P4_1, 5, 9, -angle)   #for fr leg [1]
    jointAngles_interpArray2_3 = calcWalkCycle5(P0_1, P1_1, P2_1, P3_1, P4_1, 5, 9, 0)

    #print(jointAngles_interpArray0)
    angle2 = angle
    ride_height2 = 230

    shift = len(jointAngles_interpArray0) // 2 - 1
    delay = time / reps / len(jointAngles_interpArray0)

    for i in range(reps):
        print("test1")
        for j in range(len(jointAngles_interpArray0)):
            if walkingAngle != angle2 or ride_height != ride_height2:
                jointAngles_interpArray0 = calcWalkCycle5(P0_1, P1_1, P2_1, P3_1, P4_1, 5, 9, walkingAngle)    #for fl leg [0]
                jointAngles_interpArray1 = calcWalkCycle5(P0_1, P1_1, P2_1, P3_1, P4_1, 5, 9, -walkingAngle)   #for fr leg [1]
                jointAngles_interpArray2_3 = calcWalkCycle5(P0_1, P1_1, P2_1, P3_1, P4_1, 5, 9, 0)
                angle2 = walkingAngle
                ride_height2 = ride_height

            delay = walkingInterval / reps / len(jointAngles_interpArray0)
            moveLeg(jointAngles_interpArray0[j][0], jointAngles_interpArray0[j][1], jointAngles_interpArray0[j][2], 0, bus)                          #move front-left leg

            moveLeg(jointAngles_interpArray1[j - shift][0], jointAngles_interpArray1[j - shift][1], jointAngles_interpArray1[j - shift][2], 1, bus)  #move front-right leg

            moveLeg(jointAngles_interpArray2_3[j - shift][0], jointAngles_interpArray2_3[j - shift][1], jointAngles_interpArray2_3[j - shift][2], 2, bus)  #move back-left leg

            moveLeg(jointAngles_interpArray2_3[j][0], jointAngles_interpArray2_3[j][1], jointAngles_interpArray2_3[j][2], 3, bus)                          #move back-right leg
            #moveLeg(jointAngles_interpArray1[j][0], jointAngles_interpArray1[j][1], jointAngles_interpArray1[j][2], 3, bus)
            sleep(delay)

def move_c_forward(time, angle):  #move forward constantly until walkBool is false
    global walkingBool
    global walkingInterval
    global walkingAngle
    global bus
    global ride_height
    global P0_1
    global P1_1
    global P2_1
    global P3_1
    global P4_1
    
    jointAngles_interpArray0 = calcWalkCycle5(P0_1, P1_1, P2_1, P3_1, P4_1, 5, 9, angle)    #for fl leg [0]
    jointAngles_interpArray1 = calcWalkCycle5(P0_1, P1_1, P2_1, P3_1, P4_1, 5, 9, -angle)   #for fr leg [1]
    jointAngles_interpArray2_3 = calcWalkCycle5(P0_1, P1_1, P2_1, P3_1, P4_1, 5, 9, 0)
    #print(jointAngles_interpArray0)
    angle2 = angle
    ride_height2 = 240

    shift = len(jointAngles_interpArray0) // 2 + 1
    delay = time / 5 / len(jointAngles_interpArray0)

    while(walkingBool == True):
        print("test1")
        print(walkingAngle)
        for j in range(len(jointAngles_interpArray0)):
            if walkingAngle != angle2 or ride_height != ride_height2:
                P0_1 = np.array([-45, ride_height, 60])
                P1_1 = np.array([-70, ride_height - 50, 60])
                P2_1 = np.array([0, ride_height - 75, 60])
                P3_1 = np.array([70, ride_height - 50, 60])
                P4_1 = np.array([45, ride_height, 60])
                jointAngles_interpArray0 = calcWalkCycle5(P0_1, P1_1, P2_1, P3_1, P4_1, 5, 9, walkingAngle)    #for fl leg [0]
                jointAngles_interpArray1 = calcWalkCycle5(P0_1, P1_1, P2_1, P3_1, P4_1, 5, 9, -walkingAngle)   #for fr leg [1]
                jointAngles_interpArray2_3 = calcWalkCycle5(P0_1, P1_1, P2_1, P3_1, P4_1, 5, 9, 0)
                angle2 = walkingAngle
                ride_height2 = ride_height

            delay = walkingInterval / 5 / len(jointAngles_interpArray0)
            moveLeg(jointAngles_interpArray0[j][0], jointAngles_interpArray0[j][1], jointAngles_interpArray0[j][2], 0, bus)                          #move front-left leg

            moveLeg(jointAngles_interpArray1[j - shift][0], jointAngles_interpArray1[j - shift][1], jointAngles_interpArray1[j - shift][2], 1, bus)  #move front-right leg

            moveLeg(jointAngles_interpArray2_3[j - shift][0], jointAngles_interpArray2_3[j - shift][1], jointAngles_interpArray2_3[j - shift][2], 2, bus)  #move back-left leg
            #moveLeg(jointAngles_interpArray1[j - shift][0], jointAngles_interpArray1[j - shift][1], jointAngles_interpArray1[j - shift][2], 2, bus)

            moveLeg(jointAngles_interpArray2_3[j][0], jointAngles_interpArray2_3[j][1], jointAngles_interpArray2_3[j][2], 3, bus)                          #move back-right leg
            #moveLeg(jointAngles_interpArray0[j][0], jointAngles_interpArray0[j][1], jointAngles_interpArray0[j][2], 3, bus)
            sleep(delay)
    walkingBool = True    

def start_controlServer():
    server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    server_socket.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
    server_socket.bind((controlServer_ip, controlServer_port))
    server_socket.listen(1)
    print(f"Server listening on {controlServer_ip}:{controlServer_port}")

    while True:
        conn, addr = server_socket.accept()
        print(f"Connection established with {addr}")
        threading.Thread(target=receive_thread, args=(conn,)).start()
        #plt.show()
        #while True:
        #    message = "test"
        #    conn.sendall(message.encode('utf-8'))

def start_sensorServer():
    sensorServer_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    sensorServer_socket.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
    sensorServer_socket.bind((sensorServer_ip, sensorServer_port))
    sensorServer_socket.listen(1)
    print(f"Sensor server listening on {sensorServer_ip}:{sensorServer_port}")

    while True:
        senConn, senAddr = sensorServer_socket.accept()
        print(f"Sensor Connection established with {senAddr}")
        threading.Thread(target=receiveSensor_thread, args=(senConn,)).start()


def get_cpu_temperature():
    try:
        with open('/sys/devices/virtual/thermal/thermal_zone0/temp', 'r') as f:
            temp = f.read().strip()
            temp_c = int(temp) / 1000.0
            return f"{temp_c:.2f}°C"
    except Exception as e:
        return f"TempError: {e}"

jointAngles0 = [[90.0, 10.1, 101.7],
               [90.0, 11.2, 92.6],
               [90.0, 18.2, 80.7],
               [90.0, 32.5, 67.4],
               [90.0, 55.7, 57.9],
               [90.0, 71.2, 67.4],
               [90.0, 74.1, 80.7],
               [90.0, 70.5, 92.6],
               [90.0, 63.3, 101.7],
               [90.0, 55.5, 91.3],
               [90.0, 43.0, 87.8],
               [90.0, 27.4, 91.3],
               [90.0, 10.1, 101.7]]

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


fig, ax = plt.subplots(figsize=(6,6))

    
#for i in range(1, len(jointAngles)):
#    # Get the interpolated matrix for the current segment
#    interpMatrix = makeFramesArray(jointAngles[i], currentPoints, 100)
#
#    # Stack it to the final matrix
#    jointAngles_interpArray = np.vstack((jointAngles_interpArray, interpMatrix))
#
##for i in range(0): 
#    sendFrame_shift(jointAngles_interpArray, len(jointAngles_interpArray) // 2, 5, 3)

# Initialize I2C
#with SMBus(7) as bus:  # Use I2C bus 1 on Jetson
#    set_pwm_freq(bus, FREQ)
#    
#    # Set servo to 90° on channel 0
#    pwm_value = angle_to_pwm(45)
#    set_pwm(bus, 15, 0, pwm_value)
#    sleep(0.5)
#    pwm_value = angle_to_pwm(90)
#    set_pwm(bus, 15, 0, pwm_value)
#    
#    print(f"Set servo to 90°, PWM: {pwm_value}")
#
#plt.show()

if __name__ == "__main__":
    print("Started")
    jointAngles_interpArray = np.empty((0,3))
    set_pwm_freq(bus,330)
    controlThread = threading.Thread(target=start_controlServer)
    controlThread.start()
    start_sensorServer()
    