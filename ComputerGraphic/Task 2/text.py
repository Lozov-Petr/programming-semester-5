import cv2

start   = cv2.imread("text.bmp")
gauss   = cv2.GaussianBlur(start, (3, 3), 0)
laplace = cv2.Laplacian(gauss, 3, 0, 5, 100)
result  = cv2.convertScaleAbs(laplace)

cv2.imwrite("result.bmp", result)