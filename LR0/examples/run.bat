LR0.exe 1.g > 1.g.dot
LR0.exe 2.g > 2.g.dot
LR0.exe 3.g > 3.g.dot

dot 1.g.dot -O -Tpng
dot 2.g.dot -O -Tpng
dot 3.g.dot -O -Tpng