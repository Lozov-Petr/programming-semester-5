--Данные о Партизане 
SELECT * FROM A0_Pet JOIN A0_Pet_Type ON A0_Pet.Pet_Type_ID = A0_Pet_Type.Pet_Type_ID WHERE Nick = 'Partizan';

-- Список всех собак
SELECT A0_Pet.Nick, A0_Pet.Breed, A0_Pet.Age FROM A0_Pet JOIN A0_Pet_Type ON A0_Pet.Pet_Type_ID = A0_Pet_Type.Pet_Type_ID WHERE A0_Pet_Type.Name = 'Dog';

-- Средний взраст кошек
SELECT AVG(CAST (A0_Pet.Age AS DECIMAL)) FROM A0_Pet JOIN A0_Pet_Type ON A0_Pet.Pet_Type_ID = A0_Pet_Type.Pet_Type_ID WHERE A0_Pet_Type.Name = 'Cat';

-- Время и исполнители невыполненных заказов
SELECT A0_Person.First_Name, A0_Person.Last_Name, A0_Order.Time_Order 
FROM A0_Order
	 JOIN A0_Employee ON A0_Employee.Employee_ID = A0_Order.Employee_ID
	 JOIN A0_Person ON A0_Employee.Person_ID = A0_Person.Person_ID
WHERE A0_Order.Is_Done = 0;

-- Список хозяев собак (имя, фамилия, телефон)
SELECT A0_Person.First_Name, A0_Person.Last_Name, A0_Person.Phone
FROM A0_Pet
	 JOIN A0_Pet_Type ON A0_Pet.Pet_Type_ID = A0_Pet_Type.Pet_Type_ID
	 JOIN A0_Owner ON A0_Pet.Owner_ID = A0_Owner.Owner_ID
	 JOIN A0_Person ON A0_Person.Person_ID = A0_Owner.Person_ID
WHERE A0_Pet_Type.Name = 'DOG';

-- Все виды питомцев и клички представителей этих видов (внешнее соединение)
SELECT A0_Pet_Type.Name, A0_Pet.Nick
FROM A0_Pet
	 RIGHT JOIN A0_Pet_Type ON A0_Pet.Pet_Type_ID=A0_Pet_Type.Pet_Type_ID;