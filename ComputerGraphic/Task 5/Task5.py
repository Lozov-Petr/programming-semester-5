from cv2 import *
from math import cos, sin, pi, sqrt, pow

def transformPoint(point, center, coef, alpha):
    x = point[0] - center[0]
    y = point[1] - center[1]
    newX =  x * cos(alpha) + y * sin(alpha)
    newY = -x * sin(alpha) + y * cos(alpha)
    return (coef * newX + center[0], coef * newY + center[1])

def distance(x, y): 
    return sqrt(pow(x[0] - y[0], 2) + pow(x[1] - y[1], 2))

def percentMatching(matches, keypoints, changedKeypoints, midPoint, alpha, coef):
    epsilon = 1
    acc = 0
    for mc in matches:
        changePoint = changedKeypoints[mc.trainIdx].pt
        transPoint = transformPoint(keypoints[mc.queryIdx].pt, midPoint, coef, alpha)
        if distance(transPoint, changePoint) <= epsilon: acc += 1

    return float(acc) / float(len(matches)) * 100

coef = 0.5
alphaGrad = 45
alpha = pi * alphaGrad / 180

img = imread("mandril.bmp")

h, w = img.shape[0], img.shape[1]
midPoint = (h / 2, w / 2)


rotateMatrix = getRotationMatrix2D(midPoint, alphaGrad, coef)
rotatedImg = warpAffine(img, rotateMatrix, (h, w))

sift = SIFT()

keypoints, des = sift.detectAndCompute(img, None)
rotatedKeypoints, rotatedDes = sift.detectAndCompute(rotatedImg, None)

FLANN_INDEX_KDTREE = 0
indexParams = dict(algorithm = FLANN_INDEX_KDTREE, trees = 5)
searchParams = dict(checks = 50)
flann = FlannBasedMatcher(indexParams, searchParams)

matches = flann.match(des, rotatedDes)

result = percentMatching(matches, keypoints, rotatedKeypoints, midPoint, alpha, coef)
strResult = str(result) + "%"

print strResult

file = open("result.txt", 'w')
file.write(strResult)
file.close()
