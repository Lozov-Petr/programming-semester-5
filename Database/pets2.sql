-- 1 Питомцы по возрасту
SELECT Age, COUNT(*) FROM A0_Pet GROUP BY Age;

-- 2 Типы питомцев по возрасту
SELECT Name, Age, COUNT(*) FROM A0_Pet
	JOIN A0_Pet_Type ON A0_Pet.Pet_Type_ID = A0_Pet.Pet_Type_ID
	GROUP BY Age, Name;
	
-- Типы питомцев - средний возраст
SELECT Name, AVG(CAST (A0_Pet.Age AS DECIMAL)) FROM A0_Pet
	JOIN A0_Pet_Type ON A0_Pet.Pet_Type_ID = A0_Pet_Type.Pet_Type_ID
	GROUP BY Name
	
	HAVING AVG(CAST (A0_Pet.Age AS DECIMAL)) < 6;
	
-- Заказчики - количество выполненных заказов
SELECT Last_name, COUNT(*) FROM A0_Owner
	JOIN A0_Person ON A0_Person.Person_ID = A0_Owner.Person_ID
	JOIN A0_Order ON A0_Order.Owner_ID = A0_Owner.Owner_ID
	WHERE A0_Order.Is_Done = 1
	GROUP BY A0_Owner.Owner_ID, Last_Name
	HAVING COUNT(*) > 5;