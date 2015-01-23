from cv2 import *
from numpy import * 

img = imread("text.bmp")

realImg = img.copy()

img = GaussianBlur(img, (3, 3), 0)
img = Laplacian(img, IPL_DEPTH_32F, 3)

_, img = threshold(img, 50, 255, THRESH_BINARY)

structuringElement = getStructuringElement(MORPH_RECT, (5, 4))

img = dilate(img, structuringElement)
img = erode(img, structuringElement)

img = cvtColor(img, COLOR_BGR2GRAY)

h, w = img.shape
mask = zeros((h+2, w+2), uint8)

imshow("Internal", img)

for i in range(0,w) :
	for j in range(0,h) :
		if img[j,i] == 255 :
		 	_, (left, top, width, height) = floodFill(img, mask, (i,j), 0)
		 	rectangle(realImg, (left, top - 1), (left + width, top + height - 1), (0,0,255))


# contours, _ = findContours(img, RETR_EXTERNAL, CHAIN_APPROX_NONE)

#for contour in contours:
#    left, top, width, height = boundingRect(contour)
#    rectangle(realImg, (left, top), (left + width, top + height), (0,0,255))


imshow("Result", realImg)
imwrite("result.bmp", realImg)
waitKey(0)
