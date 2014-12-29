from cv2 import *

img = imread("text.bmp")

realImg = img.copy()

img = GaussianBlur(img, (3, 3), 0)
img = Laplacian(img, IPL_DEPTH_32F, 3)

_, img = threshold(img, 50, 255, THRESH_BINARY)

img = dilate(img, getStructuringElement(MORPH_RECT, (3, 3), (1, 1)))
img = erode(img, getStructuringElement(MORPH_RECT, (3, 3), (1, 1)))
img = dilate(img, getStructuringElement(MORPH_RECT, (3, 3), (1, 1)))

img = cvtColor(img, COLOR_BGR2GRAY)

contours, _ = findContours(img, RETR_EXTERNAL, CHAIN_APPROX_NONE)

for contour in contours:
    left, top, width, height = boundingRect(contour)
    rectangle(realImg, (left, top), (left + width, top + height), (0,0,255))


imshow("Result", realImg)
imwrite("result.bmp", realImg)
waitKey(0)
